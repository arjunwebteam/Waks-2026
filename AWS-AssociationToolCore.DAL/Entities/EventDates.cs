using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class EventDates
    {
        public Int64 RId { get; set; }
        public Int64 EventDatesId { get; set; } 
        public Int64 EventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Field1 { get; set; }
        public DateTime Field2 { get; set; }
        public int OrderNo { get; set; }
        public Boolean IsActive { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
