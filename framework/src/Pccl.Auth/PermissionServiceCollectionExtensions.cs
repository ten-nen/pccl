using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Pccl.Auth
{
    public static class PermissionServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth<PermissionChecker>(this IServiceCollection services, IConfiguration configuration) where PermissionChecker : IPermissionChecker
        {
            var policies = configuration.GetSection("App:AuthPolicies").GetChildren().Select(x=>x.Value);
            if (policies.Count() == 0) return services;
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        //var roles = context.User.FindAll(JwtClaimTypes.Role).Select(x => x.Value) 为0
                        //取消JwtClaimTypes.Name、JwtClaimTypes.Role在验证时映射为ClaimTypes.Name、ClaimTypes.Role
                        if (options.SecurityTokenValidators.FirstOrDefault() is JwtSecurityTokenHandler jwtSecurityTokenHandler)
                            jwtSecurityTokenHandler.MapInboundClaims = false;


                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            //NameClaimType和RoleClaimType需与Token中的ClaimType一致，在IdentityServer中也是使用的JwtClaimTypes，否则会造成User.Identity.Name为空等问题
                            NameClaimType = JwtClaimTypes.Name,
                            RoleClaimType = JwtClaimTypes.Role,
                            //ValidIssuer(Token颁发机构), ValidAudience(颁发给谁) ,用于与TokenClaims中的Issuer和Audience进行对比，不一致则验证失败（与发放Token中的Claims对应）
                            ValidIssuer = configuration.GetValue<string>("App:Identity:ValidIssuer"),
                            ValidAudience = configuration.GetValue<string>("App:Identity:ValidAudience"),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            //IssuerSigningKey(签名秘钥)
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("App:Identity:SecretKey"))),
                            // 是否要求Token的Claims中必须包含Expires
                            RequireExpirationTime = true,
                            ValidateLifetime = false
                        };
                    });


            services.AddAuthorization(options =>
            {
                foreach (var policy in policies)
                {
                    options.AddPolicy(policy, p => p.AddRequirements(new PermissionAuthorizationRequirement(policy.ToLower())).RequireClaim(JwtClaimTypes.Id).RequireClaim(JwtClaimTypes.Role));
                }
            });

            services.AddScoped<AuthedUser>();
            services.AddSingleton<IPermissionDefinitionProvider, RoutePermissionDefinitionProvider>();
            services.AddScoped(typeof(IPermissionChecker), typeof(PermissionChecker));
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>(); //权限授权
            services.AddScoped<IAuthorizationHandler, ResourcePermissionAuthorizationHandler>(); //资源授权
            services.AddSingleton<IAuthTokenProvider, DefaultAuthTokenProvider>();
            return services;
        }
    }
}
