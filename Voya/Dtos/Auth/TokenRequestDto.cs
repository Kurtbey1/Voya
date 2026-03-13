using System.ComponentModel.DataAnnotations;

namespace Voya.Dtos.Auth
{
    public class TokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;

    }
}
