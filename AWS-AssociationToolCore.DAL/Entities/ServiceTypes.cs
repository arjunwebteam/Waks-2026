using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
  public  class ServiceTypes
    {
        public Int64 RId { get; set; }
        public Int64 ServiceTypeId { get; set; }
        public string ServiceType { get; set; }
        public string Descriptione { get; set; }
        public Boolean IsActive { get; set; }
        public Int64 OrderNo { get; set; }

        public DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }

     
    }
}
