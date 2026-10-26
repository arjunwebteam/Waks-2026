using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class Donors
    {
        public Int64 RId { get; set; }

        public Int64  DonorId {get ; set ;}
		public Int64 DonationCategoryId { get; set; }
		public Int64 ChapterId { get; set; }

		public Int64 MemberId { get; set; }

        public string  FirstName  {get ; set ;}

	    public string  LastName  {get ; set ;}

	    public string  Email  {get ; set ;}

	    public string  PhoneNo  {get ; set ;}

	    public string  Address  {get ; set ;}

	    public string  DonationProgram  {get ; set ;}

	    public string  DonationCause  {get ; set ;}

	    public bool  IsActive  {get ; set ;}

	    public Decimal  Amount  {get ; set ;}

	    public string  TransactionId  {get ; set ;}

	    public Int64  PaymentStatusId  {get ; set ;}

        public string PaymentStatus { get; set; }

	    public Int64  PaymentMethodId  {get ; set ;}

        public string PaymentMethod { get; set; }

	    public DateTime  OrderDate  {get ; set ;}

	    public string  PaymentBy  {get ; set ;}

	    public string  InsertedBy  {get ; set ;}

	    public DateTime  InsertedDate  {get ; set ;}

	    public string  UpdatedBy  {get ; set ;}

	    public DateTime  UpdatedDate  {get ; set ;}

        public string CardNumber { get; set; }

        public string CSVMonth { get; set; }

        public string CSVYear { get; set; }

        public string Cvv { get; set; }

	//Donation Info
	    public Int64 DonorInfoId { get; set; }
		public string ChapterName { get; set; }

		public string Country { get; set; }
		public string City { get; set; }

		public string State { get; set; }
		public string ExipryMonth { get; set; }
		public string ExipryYear { get; set; }

		public string ZipCode { get; set; }
		public string Spouse { get; set; }
		public Int64 NoofPayments { get; set; }
		public string TypeofCard { get; set; }
		public string FrequencyofDonation { get; set; }
		public string SameBilling { get; set; }

		public string BillingAdress { get; set; }
		public string BillingCountry { get; set; }
		public string BillingCity { get; set; }
		public string BillingState { get; set; }
		public string BillingZipCode { get; set; }
		public string KnowledgeAcurate { get; set; }
		public string Field1 { get; set; }
		public string Field2 { get; set; }
		public string Field3 { get; set; }
	

	}
}
