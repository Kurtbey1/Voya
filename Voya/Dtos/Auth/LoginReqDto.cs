using System.ComponentModel.DataAnnotations;

namespace Voya.Dtos.Auth
{
    public class LoginReqDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Email is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; } = string.Empty; 
    }
}
