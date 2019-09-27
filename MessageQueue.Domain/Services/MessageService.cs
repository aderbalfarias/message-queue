using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Domain.Services
{
    public class MessageService : IMessageService
    {
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<MessageService> _logger;
        private readonly IMessageSession _messageSession;
        
        public MessageService(IBaseRepository baseRepository, 
            ILogger<MessageService> logger,
            IMessageSession messageSession)
        {
            _baseRepository = baseRepository;
            _logger = logger;
            _messageSession = messageSession;
        }

        public async Task SendMessageAsync()
        {
            try
            {
                var message = new MessageEntity
                {
                    Id = 1,
                    Description = "Test message"
                };

                await _messageSession.Send(message).ConfigureAwait(false);

                _logger.LogInformation($"Message id {message.Id} sent successfully");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error trying to send message: {e}");
            }
        }
    }
}
