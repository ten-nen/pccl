using IdentityModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Pccl.Auth
{
    public class DefaultAuthTokenProvider : IAuthTokenProvider
    {
        private IConfiguration _configuration;
        public DefaultAuthTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtSecurityToken(AuthedUser user, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Issuer,_configuration.GetValue<string>("App:Identity:ValidIssuer")),
                new Claim(JwtClaimTypes.Audience,_configuration.GetValue<string>("App:Identity:ValidAudience")),
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.Name??string.Empty),
                new Claim(JwtClaimTypes.PhoneNumber, user.Phone??string.Empty) };

            if (user.Roles != null && user.Roles.Any())
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("App:Identity:SecretKey"))), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(string access_token)
        {
            if (string.IsNullOrWhiteSpace(access_token))
                throw new ArgumentNullException();

            var refresh_token = new StringBuilder(SecurityHasher.MD5Hash(access_token));
            for (int i = 0; i < 3; i++)
            {
                refresh_token.Append(SecurityHasher.MD5Hash(refresh_token.ToString()));
            }
            return refresh_token.ToString();
            //var randomNumber = new byte[64];
            //using var rng = RandomNumberGenerator.Create();
            //rng.GetBytes(randomNumber);
            //return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,
                ValidIssuer = _configuration.GetValue<string>("App:Identity:ValidIssuer"),
                ValidAudience = _configuration.GetValue<string>("App:Identity:ValidAudience"),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("App:Identity:SecretKey"))),
                RequireExpirationTime = true,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;

        }
    }
}
