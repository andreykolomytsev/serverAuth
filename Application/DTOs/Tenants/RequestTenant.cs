using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Tenants
{
    public class RequestTenant
    {
        [Required]
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Required]
        public string INN { get; set; }

        public string OGRN { get; set; }

        public string KPP { get; set; }

        public string OKPO { get; set; }
    }
}
