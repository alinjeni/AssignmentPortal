using System.ComponentModel.DataAnnotations;

namespace AssignmentPortal.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
