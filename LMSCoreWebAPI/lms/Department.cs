using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Department
    {
        public Department()
        {
            Institutiondepartment = new HashSet<Institutiondepartment>();
            Userdepartment = new HashSet<Userdepartment>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }

        public virtual ICollection<Institutiondepartment> Institutiondepartment { get; set; }
        public virtual ICollection<Userdepartment> Userdepartment { get; set; }
    }
}
