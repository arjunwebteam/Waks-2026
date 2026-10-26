using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
   public class Photos
    {
       public Int64 PhotoId { get; set; }

       public Int64 PhotoCategoryId { get; set; }

       public string CategoryName { get; set; }

        public string ImageUrl { get; set; }

       public string ImageDescription { get; set; }

       public Boolean DefaultImage { get; set; }

       public Int64 DisplayOrder { get; set; }

       public Boolean IsActive { get; set; }

       public bool IsHome { get; set; }
       
       public string UpdatedBy { get; set; }

       public DateTime UpdatedTime { get; set; }

       public string AlbumLink { get; set; }

        
    }
}
