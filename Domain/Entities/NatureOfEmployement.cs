using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class NatureOfEmployement : BaseEntity<int>
    {
        public string? NatureDescription { get; set; }
    }
}
