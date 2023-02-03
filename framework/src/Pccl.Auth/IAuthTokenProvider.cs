using System;
using System.Security.Claims;

namespace Pccl.Auth
{
    public interface IAuthTokenProvider
    {
        string GenerateJwtSecurityToken(AuthedUser user, DateTime expires);
        string GenerateRefreshToken(string access_token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
