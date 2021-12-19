namespace Application.DTOs.Tenants
{
    public class ResponseTenant
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string INN { get; set; }

        public string OGRN { get; set; }

        public string KPP { get; set; }

        public string OKPO { get; set; }

        public ResponseTenant Tenant { get; set; }
    }
}
