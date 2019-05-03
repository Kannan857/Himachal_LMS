using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Userinstitution
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? InstitutionId { get; set; }

        public virtual Institution Institution { get; set; }
        public virtual User User { get; set; }
    }
}
