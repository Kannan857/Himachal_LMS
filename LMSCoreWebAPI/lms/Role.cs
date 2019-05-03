using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Role
    {
        public Role()
        {
            Userrole = new HashSet<Userrole>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
