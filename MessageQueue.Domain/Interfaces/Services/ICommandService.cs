using System.Threading.Tasks;

namespace MessageQueue.Domain.Interfaces.Services
{
    public interface ICommandService
    {
        Task SendMessageAsync();
    }
}
