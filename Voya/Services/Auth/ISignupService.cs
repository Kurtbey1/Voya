using Voya.Data;
using Microsoft.EntityFrameworkCore;
using Voya.Options;
using Voya.Services;
using Voya.Services.Common;
using Voya.Dtos.Auth;
namespace Voya.Services.Auth
{
    public interface ISignupService
    {
        Task<Result<SignupResDto>> SignUpAsync(SignupReqDto req);

        Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto req);


    }
}
