using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
  public class SponsorCategories
    {
        public Int64 RId { get; set; }
        
        public Int64 SponsorCategoryId { get; set; }
        public Int64 ChapterId { get; set; }
        public string CategoryName { get; set; }
      
        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }

        public Int64 SponsorsCount { get; set; }
        public string ChapterName { get; set; }
        public Boolean IsActive { get; set; }
        public Int32 DisplayOrder { get; set; }
    }
}
