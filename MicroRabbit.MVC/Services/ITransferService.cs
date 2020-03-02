using System.Threading.Tasks;
using MicroRabbit.MVC.Models.DTO;

namespace MicroRabbit.MVC.Services
{
    public interface ITransferService
    {
        Task Transfer(TransferDto transferDto);
    }
}
