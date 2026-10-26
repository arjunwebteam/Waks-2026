using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class VolunteerCategories
    {

        public Int64 RId { get; set; }

        public Int64 VolunteerCategoryId { get; set; }
        public Int64 ChapterId { get; set; }
        public string CategoryName { get; set; }

        public Int32 OrderNo { get; set; }

        public Boolean IsActive { get; set; }

        public Int32 VolunteerCount { get; set; }
        public string ChapterName { get; set; }
        public string Type { get; set; }
        public Int32 VolunteerHours { get; set; }
        public Int64 WaksCategoryId { get; set; }
        public Int64 KannadaCategoryId { get; set; }
    }
}
