using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class PerformanceCategory
    {
        public Int64 RId { get; set; }

        public Int64 PerformanceCategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal MemberAmount { get; set; }

        public decimal NonMemberAmount { get; set; }

        public Int64 OrderNo { get; set; }

        public bool IsActive { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
