using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Shifts : BaseEntity<int>
    {
        public string? ShiftName { get; set; }
        public TimeOnly ShiftStartTime { get; set; }
        public TimeOnly ShiftEndTime { get; set; }
        public int ShiftHours { get; set; }
    }
}
