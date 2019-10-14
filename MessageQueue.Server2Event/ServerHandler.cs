using MessageQueue.Domain.Entities;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;
using System.Transactions;

namespace MessageQueue.Server2Event
{
    public class ServerHandler : IHandleMessages<MessageEventEntity>
    {
        private readonly ILog nsbLog = LogManager.GetLogger<ServerHandler>();

        public Task Handle(MessageEventEntity message, IMessageHandlerContext context)
        {
            nsbLog.Info($"Message {message.Id} received at {typeof(ServerHandler)}");
            
            using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Implement logic and log
            }

            return Task.CompletedTask;
        }
    }
}
