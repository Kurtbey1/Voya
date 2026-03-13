namespace Voya.Dtos.Auth
{
    public class LoginResDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        
    }
}
