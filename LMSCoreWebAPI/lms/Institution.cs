using System;
using System.Collections.Generic;

namespace LMSCoreWebAPI.lms
{
    public partial class Institution
    {
        public Institution()
        {
            Institutiondepartment = new HashSet<Institutiondepartment>();
            Userinstitution = new HashSet<Userinstitution>();
        }

        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public virtual ICollection<Institutiondepartment> Institutiondepartment { get; set; }
        public virtual ICollection<Userinstitution> Userinstitution { get; set; }
    }
}
