using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class User
    {
        public User()
        {
            Userdepartment = new HashSet<Userdepartment>();
            Userinstitution = new HashSet<Userinstitution>();
            Userrole = new HashSet<Userrole>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int? UserStatusId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModfiedDate { get; set; }

        public virtual ICollection<Userdepartment> Userdepartment { get; set; }
        public virtual ICollection<Userinstitution> Userinstitution { get; set; }
        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
