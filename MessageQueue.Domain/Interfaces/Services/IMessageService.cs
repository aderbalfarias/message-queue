using System.Threading.Tasks;

namespace MessageQueue.Domain.Interfaces.Services
{
    public interface IMessageService
    {
        Task SendMessage();
    }
}
