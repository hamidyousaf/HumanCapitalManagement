using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentMode : BaseEntity<int>
    {
        public string? PaymentModeDescription { get; set; }
        public string? PaymentModeAbbreviation { get; set; }
    }
}
