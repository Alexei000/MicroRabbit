using System.Collections.Generic;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Data.Models;

namespace MicroRabbit.Transfer.Data.Repository
{
    public class TransferRepository : ITransferRepository
    {
        private readonly TransferDbContext _context;

        public TransferRepository(TransferDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TransferLog> GetTransferLogs()
        {
            return _context.TransferLogs;
        }

        public void Add(TransferLog log)
        {
            _context.TransferLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
