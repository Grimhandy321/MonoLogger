using System.ComponentModel.DataAnnotations;


public enum MessageType
{
    Error,
    Message,
    Warning,
}

namespace Monologer.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public int Magnitude { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
        public MessageType Type { get; set; }
    }
}
