using MessageQueue.Domain.Entities;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace MessageQueue.ServerCommand
{
    public class ServerHandler : IHandleMessages<MessageCommandEntity>
    {
        private readonly ILog nsbLog = LogManager.GetLogger<ServerHandler>();

        public Task Handle(MessageCommandEntity message, IMessageHandlerContext context)
        {
            nsbLog.Info($"Message {message.Id} received");
            
            // Implement logic and log



            return Task.CompletedTask;
        }
    }
}
