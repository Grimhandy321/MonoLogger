using MonoLogger.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public enum MessageType
{
    Error,
    Info,
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
        public MessageType Type { get; set; }

        [JsonIgnore]  
        [SwaggerSchema(ReadOnly = true)]   
        public int UserId { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public User User { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
    }
}
