using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.GroupMasterDTO
{
    public class LoginUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
