using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
  public  class EventCompetitions
    {
        public Int64 RId { get; set; }
        public Int64 EventCompetitionId { get; set; }

        public DateTime Validity { get; set; }
        public string CompetitionName { get; set; }

        public Decimal Price { get; set; }

        public Int64 DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public decimal NonMemberAmount { get; set; }

        public Int64 EventId { get; set; }

        public string EventName { get; set; }

    }
}
