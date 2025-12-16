using System.ComponentModel.DataAnnotations;

namespace MonoLogger.Entities
{
    public class UserDto
    {

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
