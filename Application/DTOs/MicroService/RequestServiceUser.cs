using System.Collections.Generic;

namespace Application.DTOs.MicroService
{
    public class RequestServiceUser
    {
        public List<UserServiceIds> Users { get; set; }
    }

    public class UserServiceIds
    {
        public string UserId { get; set; }
    }
}
