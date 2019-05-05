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
        public int? DepartmentHead { get; set; }
        public int? DepartmentAssistantHead { get; set; }
        public int? ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public DateTime? DepartmentFrom { get; set; }

        public virtual User ContactPersonNavigation { get; set; }
        public virtual User DepartmentAssistantHeadNavigation { get; set; }
        public virtual User DepartmentHeadNavigation { get; set; }
        public virtual ICollection<Institutiondepartment> Institutiondepartment { get; set; }
        public virtual ICollection<Userdepartment> Userdepartment { get; set; }
    }
}
