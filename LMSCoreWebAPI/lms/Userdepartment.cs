using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Userdepartment
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
    }
}
