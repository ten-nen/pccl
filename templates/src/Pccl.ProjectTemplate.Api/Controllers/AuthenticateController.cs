
using IdentityModel;
using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.Auth;
using Microsoft.AspNetCore.Mvc;
using Pccl.Audit;

namespace Pccl.ProjectTemplate.Api.Controllers
{
    [Route("api/auth")]
    public class AuthenticateController : ControllerBase
    {
        protected readonly IUserService UserService;
        protected readonly IAuthTokenProvider AuthTokenProvider;
        public AuthenticateController(IUserService userService, IAuthTokenProvider authTokenProvider)
        {
            UserService = userService;
            AuthTokenProvider = authTokenProvider;
        }

        [HttpPost]
        public async Task<UserAuthDto> AuthAsync([FromBody] UserLoginDto dto)
        {
            var r = new UserAuthDto();
            var user = await UserService.GetAsync(new UserQueryDto() { Phone = dto.Phone, Password = SecurityHasher.HashPassword(dto.Password) });
            if (user == null)
                throw new BusinessException("账号或密码错误..");

            r.expires_in = 60 * 60 * 24 * 7;

            var authedUser = new AuthedUser(user.Id, user.Name, user.Phone, user.UserRoles.Select(x => x.Name).ToList());
            r.access_token = AuthTokenProvider.GenerateJwtSecurityToken(authedUser, DateTime.Now.AddSeconds(r.expires_in));
            r.refresh_token = AuthTokenProvider.GenerateRefreshToken(r.access_token);
            return r;
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<UserAuthDto> RefreshToken([FromBody] UserTokenDto dto)
        {
            var r = new UserAuthDto();
            var oldRefreshToken = AuthTokenProvider.GenerateRefreshToken(dto.access_token);
            if (dto.refresh_token != oldRefreshToken)
                throw new BusinessException("Invalid access token or refresh token");

            var principal = AuthTokenProvider.GetPrincipalFromExpiredToken(dto.access_token);

            if (principal == null)
                throw new BusinessException("Invalid access token or refresh token");

            var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Expiration).Value);

            var dateTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            var expires = dateTime.AddSeconds(utcExpiryDate).ToLocalTime();

            if (expires > DateTime.Now)
                throw new BusinessException("Token has not yet expired");

            if (expires.AddDays(7) <= DateTime.Now)
                throw new BusinessException("Refresh Token has expired");

            if (!Guid.TryParse(principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Id).Value, out var id))
                throw new BusinessException("Invalid access token or refresh token");

            var user = await UserService.GetAsync(new UserQueryDto() { Id = id });
            if (user == null)
                throw new BusinessException("Invalid access token or refresh token");

            var authedUser = new AuthedUser(user.Id, user.Name, user.Phone, user.UserRoles.Select(x => x.Name).ToList());
            r.expires_in = 3600;
            r.access_token = AuthTokenProvider.GenerateJwtSecurityToken(authedUser, DateTime.Now.AddSeconds(r.expires_in));
            r.refresh_token = AuthTokenProvider.GenerateRefreshToken(r.access_token);
            return r;
        }
    }

}
