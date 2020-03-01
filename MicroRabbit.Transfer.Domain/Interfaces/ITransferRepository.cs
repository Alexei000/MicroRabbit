using System.Collections.Generic;
using MicroRabbit.Transfer.Data.Models;

namespace MicroRabbit.Transfer.Application.Interfaces
{
    public interface ITransferRepository
    {
        IEnumerable<TransferLog> GetTransferLogs();
        void Add(TransferLog log);
    }
}
