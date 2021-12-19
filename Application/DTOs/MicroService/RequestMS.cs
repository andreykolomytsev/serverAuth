using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MicroService
{  
    public class RequestMS
    {
        [Required]
        public string FullName { get; set; }

        public string Description { get; set; }

        [Required]
        public string URL { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }
    }
}
