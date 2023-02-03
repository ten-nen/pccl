using Microsoft.AspNetCore.Builder;

namespace Pccl.Auth
{
    public static class PermissionAppBuilderExtensions
    {
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
