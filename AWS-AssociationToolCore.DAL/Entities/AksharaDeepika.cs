using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class PatrikaRegistrations
    {
        public Int64 RId { get; set; }
        public Int64 PatrikaId { get; set; }
        public Int64 ChapterId { get; set; }
        public string Title { get; set; }
        public string BannerUrl { get; set; }
        public string DocumentUrl { get; set; }
        public Int64 OrderNo { get; set; }
        public Boolean IsActive { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string ChapterName { get; set; }
    }
}
