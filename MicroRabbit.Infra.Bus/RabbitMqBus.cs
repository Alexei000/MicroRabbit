using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMqBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public RabbitMqBus(IMediator mediator)
        {
            _mediator = mediator;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            string eventName = @event.GetType().Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            string message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", eventName, null, body);
        }

        public void Subscribe<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            string eventName = typeof(TEvent).Name;
            var handlerType = typeof(THandler);

            if (!_eventTypes.Contains(typeof(TEvent)))
                _eventTypes.Add(typeof(TEvent));

            if (_handlers.ContainsKey(eventName))
                _handlers.Add(eventName, new List<Type>());

            if (_handlers[eventName].Any(s => s == handlerType))
                throw new ArgumentException($"Handler type {handlerType.Name} is already registered for {eventName}");

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<TEvent>();
        }

        private void StartBasicConsume<TEvent>() where TEvent : Event
        {
            var factory = new ConnectionFactory {HostName = "127.0.0.1", DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(TEvent).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += ConsumerReceived;

            channel.BasicConsume(eventName, true, consumer);
;        }

        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            string eventName = e.RoutingKey;
            string message = Encoding.UTF8.GetString(e.Body);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);
                    if (handler == null)
                        continue;

                    //TODO: check if this can be made typed and avoid messy dynamic invocation at the end
                    Type eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    object @event = JsonConvert.DeserializeObject(message, eventType);
                    Type concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    await (Task) concreteType.GetMethod("Handle").Invoke(handler, new[] {@event});
                }
            }
        }
    }
}
