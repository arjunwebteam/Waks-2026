using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
   public class PhotoCategories
    {
       public Int64 RId { get; set; }

        public Int64 PhotoCategoryId { get; set; }



        public Int64 ChapterId { get; set; }
        public string CategoryName { get; set; }

        public Int64 Year { get; set; }

       public string UpdatedBy { get; set; }

       public string ImageUrl { get; set; }
        public string AlbumLink { get; set; }
        public DateTime UpdatedTime { get; set; }

        public Int32 PhotoCount { get; set; }

       public Int64 PhotosCount { get; set; }

    }
}
