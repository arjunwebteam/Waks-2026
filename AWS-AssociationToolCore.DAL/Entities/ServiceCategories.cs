using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class ServiceCategories
    {
        public Int64 RId { get; set; }
        public Int64 ServiceCategoryId { get; set; }
        public Int64 ChapterId { get; set; }
        public string CategoryName { get; set; }
        public string IconUrl { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        
        public string ChapterName { get; set; }
    }
}
