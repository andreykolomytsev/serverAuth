using Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Tenants
{
    public class ResponseTenantUser
    {
        public string TenantName { get; set; }

        public List<ResponseUser> Permissions { get; set; }
    }
}
