using NServiceBus;

namespace MessageQueue.Domain.Entities
{
    public class MessageCommandEntity : ICommand
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
