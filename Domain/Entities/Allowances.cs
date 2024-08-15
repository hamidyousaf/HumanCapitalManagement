using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Allowances : BaseEntity<int>
    {
        public string? AllowanceDescription { get; set; }
        public string? AllowancePeriod { get; set; }
        public string? ApplyCriteria { get; set; }
        public bool AdditionWithOverTime { get; set; } = false;
        public bool IsTaxable { get; set; } = false;
        public float? TaxPercentage { get; set; }
    }
}
