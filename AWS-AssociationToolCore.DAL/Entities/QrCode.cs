using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class QrCode
    {
        public Int64 RId { get; set; }
        public Int64 QrCodeId { get; set; }
        public string qrLevel { get; set; }
        public string labelText { get; set; }
        public Int32 pixelSize { get; set; }
        public string RefUrl { get; set; }
        public string fileName { get; set; }
        public Boolean IsActive { get; set; }
        public Int64 OrderNo { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    } 
}
