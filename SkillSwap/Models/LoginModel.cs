using System.ComponentModel.DataAnnotations;

namespace SkillSwap.Models
{
    public class LoginModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? grant_type { get; set; }
    }
}
