using System.Collections.Generic;
using MicroRabbit.Transfer.Data.Models;

namespace MicroRabbit.Transfer.Application.Interfaces
{
    public interface ITransferService
    {
        IEnumerable<TransferLog> GetTransfers();
    }
}
