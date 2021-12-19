using System;

namespace Application.DTOs.Auth
{
    public class ResponseAuthentication
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string AccessToken { get; set; }

        public bool IsOutDated { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        public string RedirectURL { get; set; }
    }
}
