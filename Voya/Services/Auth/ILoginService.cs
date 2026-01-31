using Voya.Dtos.Auth;
using Voya.Services.Common;

namespace Voya.Services.Auth
{
    public interface ILoginService
    {
        public Task<Result<LoginResDto>> LoginAsync(LoginReqDto req);
    }
}
