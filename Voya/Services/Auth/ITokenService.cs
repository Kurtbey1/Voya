using System.Security.Claims;
using Voya.Dtos.Auth;
using Voya.Models;
namespace Voya.Services.Auth
{
    public interface ITokenService
    {
        // The CPU is responsable to Genertate Tokens so there is no need to create the function as Task (Async)   
        string CreateToken(User user);

        Task<TokenResponseDto> Refresh(TokenRequestDto request);

        Task<TokenResponseDto> GenerateTokenAsync(User user);

        Task RevokeTokenAsync(string userId);
    }
}
