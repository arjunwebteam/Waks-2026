using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class ExhibitsRegistrations
    {
        public Int64 RId { get; set; }

        public Int64 ExhibitId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; } 

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public Int64 NoOfBooths { get; set; } 

        public string SigeOfBooth { get; set; } 

        public string BusinessName { get; set; }

        public string Category { get; set; } 

        public bool IsActive { get; set; }

        public Decimal Amount { get; set; }        

        public Int64 OrderNo { get; set; } 

        public decimal AmountPaid { get; set; }

        public decimal AmountDue { get; set; }

        public string TransactionId { get; set; }

        public Int64 PaymentStatusId { get; set; }

        public Int64 PaymentMethodId { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }
         
        public string PaymentBy { get; set; }

        public String Date { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string CardNumber { get; set; }

        public string CSVMonth { get; set; }

        public string CSVYear { get; set; }

        public string Cvv { get; set; }

        public string CSVExpiry { get; set; } 

        public string PromoCode { get; set; }
        public string Zelle { get; set; }

    }
}
