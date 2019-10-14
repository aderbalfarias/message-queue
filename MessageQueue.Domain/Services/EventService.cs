using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Domain.Services
{
    public class EventService : IEventService
    {
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<CommandService> _logger;
        private readonly IMessageSession _messageSession;
        
        public EventService(IBaseRepository baseRepository, 
            ILogger<CommandService> logger,
            IMessageSession messageSession)
        {
            _baseRepository = baseRepository;
            _logger = logger;
            _messageSession = messageSession;
        }

        public async Task PublishMessageAsync()
        {
            try
            {
                var message = new MessageEventEntity
                {
                    Id = 1,
                    Description = "Test Event message using NServiceBus"
                };

                await _messageSession.Publish(message).ConfigureAwait(false);

                _logger.LogInformation($"Message id {message.Id} published successfully");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error trying to publish message: {e}");
            }
        }
    }
}
