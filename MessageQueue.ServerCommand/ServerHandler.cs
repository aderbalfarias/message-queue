using MessageQueue.Domain.Entities;
using NServiceBus;
using System.Threading.Tasks;

namespace MessageQueue.ServerCommand
{
    public class ServerHandler : IHandleMessages<MessageCommandEntity>
    {
        public Task Handle(MessageCommandEntity message, IMessageHandlerContext context)
        {
            // Implement logic and log

            return Task.CompletedTask;
        }
    }
}
