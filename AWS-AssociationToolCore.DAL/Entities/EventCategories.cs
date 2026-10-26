using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class EventCategories
    {
        public Int64 RId { get; set; }

        public Int64 EventCategoryId { get; set; }

        public string EventCategory { get; set; }

        public Int64 ChapterId { get; set; }
        public Boolean IsActive { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }
    }
}
