using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NServiceBus;
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

        public Task SendMessage()
        {


            return Task.CompletedTask;
        }
    }
}
