using Monologer.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MonoLogger.Entities
{
    public class User
    {
        [Key]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public string AccessKey { get; set; } = string.Empty;


        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public List<Message> Messages { get; set; } = new();
    }
}
