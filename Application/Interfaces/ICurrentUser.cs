using System;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface ICurrentUser
    {
        public string UserId { get; }

        public string TenantId { get; }     

        public List<string> Permissions { get; }

        public List<string> Roles { get; }

        public List<string> Services { get; }
    }
}
