using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Server1Event
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
                nsbLog.Info($"Message {message.Id} received at {typeof(ServerHandler).FullName}");

                //_baseRepository.GetObjectAsync<>(t => t.Id == x)
                // Implement logic and log

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                nsbLog.Info($"Message {message.Id} throw exception at {typeof(ServerHandler).FullName}");
                return Task.FromException(e);
            }
        }
    }
}
