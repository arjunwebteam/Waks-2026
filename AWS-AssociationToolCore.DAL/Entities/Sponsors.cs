using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
   public class Sponsors
    {
       public Int64 RId { get; set; }

       public Int64 SponsorId { get; set; }

       public Int64 SponsorCategoryId { get; set; }

       public string LogoUrl { get; set; }

       public string RedirectUrl { get; set; }

       public string CategoryName { get; set; }

        public Boolean IsActive { get; set; }

        public Int64 DisplayOrder { get; set; }

        public string Target { get; set; }

        public DateTime InsertedTime { get; set; }

        public string InsertedBy { get; set; }
    }
}
