using System;

namespace Application.Parameters
{
    public class TokenFilter
    {
        public string Token { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Expires { get; set; }

        public string CreatedByIp { get; set; }

        public string CreatedByBrowser { get; set; }

        public string UserId { get; set; }
    }
}
