using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Banks : BaseEntity<int>
    {
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountNo { get; set; }
        public string? AccountTitle { get; set; }
    }
}
