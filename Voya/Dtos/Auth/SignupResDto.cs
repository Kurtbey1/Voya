namespace Voya.Dtos.Auth
{
    public class SignupResDto
    {
        public long User_ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = "Please check your email for the verification code.";
    }
}
