using NServiceBus;

namespace MessageQueue.Domain.Entities
{
    public class MessageEntity : ICommand
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
