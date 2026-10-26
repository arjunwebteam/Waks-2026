using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class CulturalRegistrations
    {
        public Int64 CulturalRegistrationId { get; set; }

        public Int64 ChapterId { get; set; }

        public Int64 RId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }

        public string AAALifetimeMember { get; set; }

        public string CoordinatorName { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }

        public string OrganizationName { get; set; }

        public string PerformanceCategory { get; set; }

        public string SpecifyCategory { get; set; }

        public string NameofthePerformance { get; set; }

        public string PerformanceDuration { get; set; }

        public string NumberofParticipants { get; set; }

        public string ParticipantAgeGroup { get; set; }

        public string SpecialRequirements { get; set; }

        public string BriefDescriptionofthePerformance { get; set; }

        public string CardNumber { get; set; }

        public string CSVMonth { get; set; }

        public string CSVYear { get; set; }

        public string Cvv { get; set; }

        public Int64 PaymentStatusId { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public Int64 PaymentMethodId { get; set; }

        public string PaymentMethod { get; set; }

        public string OtherPaymentId { get; set; }

        public string TransactionId { get; set; }

        public decimal AmountPaid { get; set; }

        public string Zelle { get; set; }

        public string ReceiptUrl { get; set; }

        public string ZellePartnerName { get; set; }

        public string ChequeHolderName { get; set; }

        public Boolean IsActive { get; set; }

        public Int64 EventId { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string AAAInstagram { get; set; }
        public string EventName { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string Honeypot { get; set; }
        public string recaptchaToken { get; set; }

        public CulturalParticipants objCulturalParticipants = new CulturalParticipants();

        public List<CulturalParticipants> lstCulturalParticipants { get; set; }

        public List<PaymentStatus> lstPaymentStatus { get; set; }

        public List<PaymentMethods> lstPaymentMethod { get; set; }

        public class CulturalParticipants
        {
            public Int64 RId { get; set; }

            public Int64 ParticipantID { get; set; }

            public Int64 CulturalRegistrationId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string PhoneNumber { get; set; }

            public Int64 Age { get; set; }

            public string Gender { get; set; }

            public string Address { get; set; }

            public string InsertedBy { get; set; }

            public DateTime InsertedDate { get; set; }

            public string UpdatedBy { get; set; }

            public DateTime UpdatedDate { get; set; }

        }


    }
}
