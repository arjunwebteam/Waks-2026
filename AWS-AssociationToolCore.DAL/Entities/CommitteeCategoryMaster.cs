using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
  public  class CommitteeCategoryMaster
    {
        public Int64 RId { get; set; }
        public Int64 CommitteeCategoryId { get; set; }
        public string CategoryName { get; set; }
        public Int64 YearId { get; set; }
        public Int32 PageLevel { get; set; }
        public Int64 PageParentId { get; set; }
        public string IdPath { get; set; }

        public Int64 Position { get; set; }
        public Boolean IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }

        public bool ParentActive { get; set; }
        public string ParentName { get; set; }
        public int SubMenuItemCount { get; set; }
        public int SubCategoryCount { get; set; }
        public string ChapterIds { get; set; }
        public List<ChapterCommittees> lstChapterCommittees { get; set; }



    }
}
