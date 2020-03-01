using MicroRabbit.Transfer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Banking.Data.Context
{
    public class TransferDbContext : DbContext
    {
        public DbSet<TransferLog> TransferLogs { get; set; }

        public TransferDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
