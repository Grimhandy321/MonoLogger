using System.ComponentModel.DataAnnotations;

namespace Monologer.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
