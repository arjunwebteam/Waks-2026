using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class RegistrationCategories
    {
        public Int64 RId { get; set; }

        public Int64 RegistrationCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Type { get; set; }

        public bool IsPayment { get; set; }

        public string Description { get; set; }

        public Int32 OrderNo { get; set; }

        public bool IsActive { get; set; }

        public string InstertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public Decimal MemberAmount { get; set; }

        public Decimal NAmount { get; set; }


        public Int64 Form1Count { get; set; }
        public Int64 Form2Count { get; set; }
        public Int64 Form3Count { get; set; }
        public Int64 Form4Count { get; set; }
        public Int64 Form5Count { get; set; }
        public Int64 Form6Count { get; set; }


        public class EventRegCategoriesList
        {

            public Int64 EventId { get; set; }

            public string RegistrationCategoryIds { get; set; }

            public Decimal MemberAmount { get; set; }

            public Decimal NAmount { get; set; }

        }
    }
}
