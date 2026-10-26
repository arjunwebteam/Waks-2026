using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    
    public class TeluguAmmaiRegistrations
    {
        public Int64 RId { get; set; }
        public Int64 TeluguAmmaiRegistrationId { get; set; }
        public Int64 ChapterId { get; set; }
        public string RegestrationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public DateTime DOB { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentRelation { get; set; }
        public string ParentEmail { get; set; }
        public string SchoolGrade { get; set; }
        public string Gender { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string AlternativePhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CompetitionCategory1 { get; set; }
        public string CompetitionCategory2 { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public string TransactionId { get; set; }
        public Int64 PaymentStatusId { get; set; }
        public Int64 PaymentMethodId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentBy { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ChapterName { get; set; }

        public string CardNumber { get; set; }

        public string CSVExpiry { get; set; }

        public string CSVMonth { get; set; }
        public string CSVYear { get; set; }

        public string Cvv { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }
        public Int64 EventId { get; set; }
        public Int64 RegistrationCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string EventName { get; set; }

        public int Age { get; set; }
        public string MaritalStatu { get; set; }
        public string District { get; set; }

        public List<TeluguAmmaiParticipants> lstTeluguAmmaiParticipants = new List<TeluguAmmaiParticipants>();
    }

    public class TeluguAmmaiParticipants
    {
        public Int64 RId { get; set; }

        public Int64 TeluguAmmaiParticipantId { get; set; }

        public Int64 EventId { get; set; }

        public Int64 TeluguAmmaiRegistrationId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Age { get; set; }

        public string Mobile { get; set; }

        public Boolean IsApproved { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }


    }

    public class Districts
    {
        public Int64 DistrictId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
