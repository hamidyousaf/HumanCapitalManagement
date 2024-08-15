using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Departments : BaseEntity<int>
    {
        public string? DepartmentName { get; set; }
        public int MaximumAllowedStrength { get; set; }
        public string? DepartmentAbbreviation { get; set; }
        
    }
}
