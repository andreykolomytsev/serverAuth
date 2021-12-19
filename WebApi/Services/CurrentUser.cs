using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Application.Constants;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebApi.Services
{
    /// <summary>
    /// Текущий авторизованный пользователь
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            TenantId = httpContextAccessor.HttpContext?.User?.FindFirstValue(AllPermissionTypes.Tenant);
            Permissions = httpContextAccessor.HttpContext?.User?.Claims.Where(x => x.Type == AllPermissionTypes.Permission).Select(s => s.Value).ToList();
            Roles = httpContextAccessor.HttpContext?.User?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(s => s.Value).ToList();
            Services = httpContextAccessor.HttpContext?.User?.Claims.Where(x => x.Type == AllPermissionTypes.Service).Select(s => s.Value).ToList();
        }

        public string UserId { get; }

        public string TenantId { get; }

        public List<string> Permissions { get; }

        public List<string> Roles { get; }

        public List<string> Services { get; }
    }
}
