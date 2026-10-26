using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class ExibhitsRegistrations
    {
        public Int64 RId { get; set; }

        public Int64 ExibhitsRegistrationId { get; set; }

	    public string VendorName  { get; set; }

	    public string ExhibitType  { get; set; }

	    public string FirstName  { get; set; }

	    public string LastName  { get; set; }

	    public string Email  { get; set; }

	    public string PhoneNo  { get; set; }

	    public string Address  { get; set; }

	    public string City  { get; set; }

	    public string State  { get; set; }

	    public string ZipCode  { get; set; }

	    public bool IsActive  { get; set; }

	    public Int32 NoofBooths  { get; set; }

	    public string BoothSize  { get; set; }

	    public decimal Amount  { get; set; }

	    public string TransactionId  { get; set; }

	    public Int64 PaymentStatusId  { get; set; }

	    public Int64 PaymentMethodId  { get; set; }

	    public DateTime OrderDate  { get; set; }

	    public string PaymentBy  { get; set; }

	    public string InsertedBy  { get; set; }

	    public DateTime InsertedDate  { get; set; }

	    public string UpdatedBy   { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

    }
}
