using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Deductions : BaseEntity<int>
    {
        public string? DeductionDescription { get; set; }
        public string? DeductionPeriod { get; set; }
        public string? ApplyCriteria { get; set; }
        public float LimitAmount { get; set; }
        public float? DeductionPercentage { get; set; }
    }
}
