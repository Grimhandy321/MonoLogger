using Monologer.Entities;
using System.ComponentModel.DataAnnotations;


namespace MonoLogger.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int AccessKey { get; set; }
        public List<Message> Messages { get; set; } = new();
    }
}
