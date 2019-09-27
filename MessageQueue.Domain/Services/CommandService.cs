using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace MessageQueue.Domain.Services
{
    public class CommandService : ICommandService
    {
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<CommandService> _logger;
        private readonly IMessageSession _messageSession;
        
        public CommandService(IBaseRepository baseRepository, 
            ILogger<CommandService> logger,
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
                var message = new MessageCommandEntity
                {
                    Id = 1,
                    Description = "Test command message"
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
