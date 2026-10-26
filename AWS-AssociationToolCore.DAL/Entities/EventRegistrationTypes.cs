using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class EventRegistrationTypes
    {
        public Int64 EventRegistrationTypeId { get; set; }
        public Int64 EventId { get; set; }
        public decimal MemberAmount { get; set; }
        public decimal NonMemberAmount { get; set; }
        public string RegistrationTitle { get; set; }
        public Int64 RegCount { get; set; }
        public int OrderNo { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }

        public Int64 MemberCount { get; set; }
        public Int64 EventRegCount { get; set; }
        public Int64 MERegCount { get; set; }
        public decimal EarlyBirdMemberAmount { get; set; }
        public decimal EarlyBirdNonMemberAmount { get; set; }
        public decimal EarlyBirdRegCount { get; set; }

    }
}
