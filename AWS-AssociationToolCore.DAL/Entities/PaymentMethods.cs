using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class PaymentMethods
    {

        public Int64 RId { get; set; }
        private Int64 _PaymentMethodId;
        public bool PaymentMode { get; set; }

        public Int64 PaymentMethodId
        {
            get { return _PaymentMethodId; }
            set { _PaymentMethodId = value; }
        }
        private string _PaymentMethod;

        public string PaymentMethod
        {
            get { return _PaymentMethod; }
            set { _PaymentMethod = value; }
        }

        public bool IsActive { get; set; }
    }


}
