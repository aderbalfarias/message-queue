using MessageQueue.Domain.Entities;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Server1Event
{
    public class ServerHandler : IHandleMessages<MessageEventEntity>
    {
        private readonly ILog nsbLog = LogManager.GetLogger<ServerHandler>();

        public Task Handle(MessageEventEntity message, IMessageHandlerContext context)
        {
            try
            {
                nsbLog.Info($"Message {message.Id} received at {typeof(ServerHandler)}");

                // Implement logic and log

                return Task.CompletedTask;
            }
            catch(Exception e)
            {
                return Task.FromException(e);
            }
        }
    }
}
