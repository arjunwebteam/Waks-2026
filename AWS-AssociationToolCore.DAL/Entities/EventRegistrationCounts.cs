using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class EventRegistrationCounts
    {
        public Int64 RId { get; set; }
        public Int64 EventRegCountId { get; set; }
        public Int64 EventId { get; set; }
        public Int64 EventUserInfoId { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }
        public Int64 Count { get; set; }
        public decimal EarlyBirdMemberAmount { get; set; }
        public decimal Amount { get; set; }
        public Int64 VisitCount { get; set; }
        public string Field1 { get; set; }
        public bool Field2 { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string TransactionId { get; set; }
        public string RegistrationTitle { get; set; }

        public DateTime CheckInDate { get; set; }

        public Int32 TotalTickets { get; set; }
        public Int32 CheckedInCount { get; set; }
        public string OverallStatus { get; set; }
        public string CheckInDetails { get; set; }
        public string CheckinCount { get; set; }
        public string SulekhaId { get; set; }
    }
    public class EventRegCheckIn
    {
        public Int64 EventCheckInId { get; set; }
        public Int64 EventRegCountId { get; set; }
        public Int64 EventUserInfoId { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public Int64 EventId { get; set; }
        public Int64 EventCommentId { get; set; }
        public string EmployeeRefUrl { get; set; }
        public string EmployeeCode { get; set; }
        public Int64 EventRegTicketId { get; set; }
    }

    public class EventsTickets
    {
        public Int64 Barcode { get; set; }
        public Int64 EventId { get; set; }
        public Int64 EventUserInfoId { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }
        public string BarCodeImage { get; set; }
        public Int64 Count { get; set; }
        public decimal Amount { get; set; }
        public string OrderBy { get; set; }
        public string Field1 { get; set; }
        public bool Field2 { get; set; }
        public string Field3 { get; set; }
        public string RegistrationTitle { get; set; }

    }
    public class EventRegTickets
    {
        // Existing ticket info
        public Int64 EventRegTicketId { get; set; }
        public Int64 EventRegCountId { get; set; }
        public Int64 EventId { get; set; }
        public Int64 EventUserInfoId { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }
        public Int32 SequenceNo { get; set; }

        // User info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }  // Maps to EUI.Mobile
        public string Gothram { get; set; }
        public string Nakshatram { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Event & Item info
        public string EventName { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDuration { get; set; }

        // Payment info
        public string OtherPaymentId { get; set; }
        public string Zelle { get; set; }
        public string ZellePartnerName { get; set; }
        public string ChequeHolderName { get; set; }
        public string PaymentMethod { get; set; }  // PM.PaymentMethod
        public string PaymentStatus { get; set; }  // PS.PaymentStatus
        public decimal PaidAmount { get; set; }    // ERC.Amount
        public int PurchasedCount { get; set; }    // ERC.Count

        // Approval & status
        public bool IsApproved { get; set; }
        public string Status { get; set; }             // EC.Status AS TicketStatus
        public string TicketComments { get; set; }     // EC.Comments AS TicketComments
        public string Comments { get; set; }           // EUI.Comments

        // Event category
        public string EventCategory { get; set; }       // ECAT.EventCategory
        public Int64 EventCategoryId { get; set; }      // ECAT.EventCategoryId

        // Ticket & check-in
        public DateTime CheckInDate { get; set; }
        public string CheckInStatus { get; set; }
        public string TicketId { get; set; }
        public string SeatNo { get; set; }

        // Documents & metadata
        public string DocumentUrl { get; set; }         // EUI.DocumentUrl
        public string ReceiptUrl { get; set; }          // EUI.ReceiptUrl
        public string InsertedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }      // nullable in case not updated
        public DateTime GeneratedDate { get; set; }
        public string RegistrationTitle { get; set; }
    }
}
