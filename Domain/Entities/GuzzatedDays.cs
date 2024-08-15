using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GuzzatedDays : BaseEntity<int>
    {
        public string GuzzatedDayName { get; set; }
        public DateOnly GuzzatedDayDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
