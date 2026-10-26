using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
  public  class Videos
    {
      public Int64 RId { get; set; }

      public Int64 VideoId { get; set; }

      public Int64 VideoCategoryId { get; set; }

      public string CategoryName { get; set; }

      public string Heading { get; set; }

      public string VideoUrl { get; set; }

      public string VideoDescription { get; set; }

      public Int64 DisplayOrder { get; set; }

      public Boolean IsHome { get; set; }
  
      public Boolean IsActive { get; set; }

      public string UpdatedBy { get; set; }

      public DateTime UpdatedTime { get; set; }
    }
}
