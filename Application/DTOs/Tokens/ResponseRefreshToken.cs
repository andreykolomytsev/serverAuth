using System;
using Application.DTOs.Users;

namespace Application.DTOs.Tokens
{
    public class ResponseRefreshToken
    {
        public string Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; }

        public string CreatedByBrowser { get; set; }

        public DateTime? Revoked { get; set; }

        public string RevokedByIp { get; set; }

        public string ReplacedByToken { get; set; }

        public ResponseUser User { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public bool IsActive => Revoked == null && !IsExpired;
    }
}
