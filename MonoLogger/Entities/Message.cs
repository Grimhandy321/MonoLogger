using MonoLogger.Entities;
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
        [Required]
        public string Text { get; set; } = string.Empty;
        public int Magnitude { get; set; } = 0;
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public MessageType Type { get; set; }
    }
}
