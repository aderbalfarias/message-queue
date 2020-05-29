using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Server2Event
{
    public class ServerHandler : IHandleMessages<MessageEventEntity>
    {
        private readonly ILog nsbLog = LogManager.GetLogger<ServerHandler>();
        private readonly IBaseRepository _baseRepository;

        public ServerHandler(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public Task Handle(MessageEventEntity message, IMessageHandlerContext context)
        {
            try
            {
                nsbLog.Info($"Message {message.Id} received at {typeof(ServerHandler)}");


                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }
    }
}
