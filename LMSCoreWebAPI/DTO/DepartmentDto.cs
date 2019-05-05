using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSCoreWebAPI.DTO
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
        public int? DepartmentHead { get; set; }
        public int? DepartmentAssistantHead { get; set; }
        public int? ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public DateTime? DepartmentFrom { get; set; }

        public int CreatedUserId { get; set; }

        public int InstitutionId { get; set; }
    }
}
