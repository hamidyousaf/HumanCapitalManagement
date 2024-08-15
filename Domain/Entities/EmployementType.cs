using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EmployementType : BaseEntity<int>
    {
        public int TypeDescription { get; set; }
        public int WorkingDays { get; set; }
        public int ConfirmationDays { get; set; }
        public int Grade { get; set; }
        public float OverTimeRate { get; set; }
        public string? OverTimeBase { get; set; }
        public bool GatePassDeductionApplied { get; set; }
        public bool LateDeductionApplied { get; set; }
        public int NoOfLates { get; set; }
        public int NoOfDays { get; set; }
        public int OverTimeMinimumLimit { get; set; }
        public string? DeductionFrom { get; set; }
    }
}
