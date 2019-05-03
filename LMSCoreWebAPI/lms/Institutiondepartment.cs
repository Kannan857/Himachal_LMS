using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Institutiondepartment
    {
        public int Id { get; set; }
        public int? InstitutionId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
