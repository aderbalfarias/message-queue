using NServiceBus;

namespace MessageQueue.Domain.Entities
{
    public class MessageEventEntity : IEvent
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
