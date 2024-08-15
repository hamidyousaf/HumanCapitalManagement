using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Designations : BaseEntity<int>
    {
        public string? DesignationDescription { get; set; }
        public string? DesignationAbbreviation { get; set; }
    }
}
