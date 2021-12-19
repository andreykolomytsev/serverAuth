using System;
using Application.DTOs.Users;

namespace Application.DTOs.Tokens
{
    public class ResponseAccessToken
    {
        public string Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; }

        public string CreatedByBrowser { get; set; }

        public ResponseUser User { get; set; }

        public bool IsOutDated { get; set; }

        public bool IsActive { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
    }
}
