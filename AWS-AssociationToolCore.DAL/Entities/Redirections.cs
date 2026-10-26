using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class Redirections
    {
        public Int64 RId { get; set; }
        public Int64 RedirectionId { get; set; }
        public string FromUrl { get; set; }
        public string ToUrl { get; set; }
        public string Target { get; set; }
     
        public bool IsActive { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

      
    }
}
