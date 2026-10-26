using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class PaymentStatus
    {
        private Int64 _RId;

        public Int64 RId
        {
            get { return _RId; }
            set { _RId = value; }
        }
        private Int64 _PaymentStatusId;

        public Int64 PaymentStatusId
        {
            get { return _PaymentStatusId; }
            set { _PaymentStatusId = value; }
        }
        private string _PaymentStatus;

        public string PaymentStatus1
        {
            get { return _PaymentStatus; }
            set { _PaymentStatus = value; }
        }
    }
}
