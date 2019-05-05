using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class User
    {
        public User()
        {
            DepartmentContactPersonNavigation = new HashSet<Department>();
            DepartmentDepartmentAssistantHeadNavigation = new HashSet<Department>();
            DepartmentDepartmentHeadNavigation = new HashSet<Department>();
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
        public int? MobileNumber { get; set; }
        public short IsVerified { get; set; }
        public string UniqueId { get; set; }
        public string ResetPasswordToken { get; set; }

        public virtual ICollection<Department> DepartmentContactPersonNavigation { get; set; }
        public virtual ICollection<Department> DepartmentDepartmentAssistantHeadNavigation { get; set; }
        public virtual ICollection<Department> DepartmentDepartmentHeadNavigation { get; set; }
        public virtual ICollection<Userdepartment> Userdepartment { get; set; }
        public virtual ICollection<Userinstitution> Userinstitution { get; set; }
        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
