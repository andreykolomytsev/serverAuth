namespace Application.DTOs.MicroService
{
    public class ResponseMS
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string URL { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }

        public bool Selected { get; set; }
    }
}
