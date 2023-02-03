using Pccl.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pccl.ProjectTemplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("default")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected AuthedUser CurrentUser => HttpContext.RequestServices.GetRequiredService<AuthedUser>();
    }
}
