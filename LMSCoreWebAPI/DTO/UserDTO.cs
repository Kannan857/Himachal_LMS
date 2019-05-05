using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSCoreWebAPI.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }       
        public int InstitutionId { get; set; }
        public string InstitutionUrl { get; set; }
        public string InstitutionName { get; set; }
        public string MobileNumber { get; set; }
        public bool IsAdmin { get; set; }
        public List<string> Role { get; set; }
        public List<string> Department { get; set; }

    }
}
