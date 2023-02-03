using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Pccl.Auth
{
    public class AuthedUser
    {
        public AuthedUser(IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor.HttpContext.User)
        {
        }
        public AuthedUser(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                return;
            Id = Guid.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.Id).Value);
            Name = claimsPrincipal.FindFirst(JwtClaimTypes.Name).Value;
            Phone = claimsPrincipal.FindFirst(JwtClaimTypes.PhoneNumber).Value;
            Roles = claimsPrincipal.FindAll(JwtClaimTypes.Role).Select(x => x.Value).ToList();
        }
        public AuthedUser(Guid id, string name, string phone, List<string> roles)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Roles = roles;
        }

        public Guid Id { get; set; }
        public List<string> Roles { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
