using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace MicroRabbit.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // domain bus
            services.AddScoped<IEventBus, RabbitMqBus>();

            services.AddScoped<IEventBus, RabbitMqBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMqBus(sp.GetService<IMediator>(), scopeFactory);
            });

            // subscription
            services.AddScoped<TransferEventHandler>();

            // domain events
            services.AddScoped<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();
                
            // commands
            services.AddScoped<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();
            // application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransferService, TransferService>();

            // data
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransferRepository, TransferRepository>();

            services.AddScoped<BankingDbContext>();
            services.AddScoped<TransferDbContext>();


        }
    }
}
