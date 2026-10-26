using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class Events
    {
        public Int64 RId { get; set; }

        public Int64 EventId {get;set;}
        public Int64 ChapterId { get; set; }

        public string ChapterName { get; set; }
        public string EventType { get; set; }
        public string EventName  {get;set;}

	    public DateTime StartDate  {get;set;}

	    public DateTime EndDate  {get;set;}

	    public DateTime RegistrationStartDate {get;set;}

	    public DateTime RegistrationEndDate  {get;set;}

	    public string BannerUrl  {get;set;}

	    public Int64 EventCategoryId  {get;set;}

        public string EventCategory { get; set; }

	    public int MemberCount  {get;set;}

	    public string Location  {get;set;}

	    public string City  {get;set;}

	    public string StateName  {get;set;}

	    public string ZipCode  {get;set;}

	    public string EventDetails  {get;set;}

	    public string ContactEmail  {get;set;}	   
       
	    public Boolean IsRegistration  {get;set;}

        public Boolean IsCulturalRegistration { get; set; }

	    public string PageTitle  {get;set;}

	    public string MetaDescription  {get;set;}

	    public string MetaKeywords  {get;set;}

	    public string TopLine  {get;set;}

	    public string UpdatedBy  {get;set;}

        public DateTime UpdatedTime { get; set; }

        public bool IsChild { get; set; }

        public Decimal ChildAmount { get; set; }

        public Decimal NonChildAmount { get; set; }

        public bool IsAdult { get; set; }

        public Decimal AdultAmount { get; set; }

        public Decimal NonAdultAmount { get; set; }

        public bool IsCouple { get; set; }

        public Decimal CoupleAmount { get; set; }

        public Decimal NonCoupleAmount { get; set; }

        public bool IsFamily { get; set; }

        public Decimal FamilyAmount { get; set; }

        public Decimal NonFamilyAmount { get; set; }

        public bool IsVIP { get; set; }

        public Decimal VIPAmount { get; set; }

        public Decimal NonVIPAmount { get; set; }

        public bool IsVIPSingleAdult { get; set; }

        public Decimal VIPSingleAdultAmount { get; set; }

        public Decimal NonVIPSingleAdultAmount { get; set; }

        public bool IsVIPCouple { get; set; }

        public Decimal VIPCoupleAmount { get; set; }

        public Decimal NonVIPCoupleAmount { get; set; }

        public bool IsVIPChild { get; set; }

        public Decimal VIPChildAmount { get; set; }

        public Decimal NonVIPChildAmount { get; set; }

        public string XMLRegistrations { get; set; }

        public string XMLRegistrationsCat { get; set; }

        public Int32 EventRegistrationTypesCount { get; set; }

        public EventUserInfo objEventUserInfo = new EventUserInfo();

        public Int64 UsersCount { get; set; }
        public Int64 CUsersCount { get; set; }

        public string ChapterIds { get; set; }

        public ChapterEvents objChapterEvents = new ChapterEvents();

        public List<ChapterEvents> lstChapterEvents { get; set; }

        public string XMLDates { get; set; }

        public string RegistrationCategoryIds { get; set; }

        public decimal MemberAmount { get; set; }

        public decimal NAmount { get; set; }

        public string CategoryName { get; set; }
        public string Flyer { get; set; }
        public string IsQRCode { get; set; }
        public string EmaisToBrevo { get; set; }
        public DateTime EarlyBirdCutOfDate { get; set; }
        public bool IsEarlyBird { get; set; }
        public bool IsMemberAmount { get; set; }
        public bool IsNonMemberAmount { get; set; }

        public EventDates objEventDates = new EventDates();
        public DateTime sdate { get; set; }

        public DateTime edate { get; set; }

        public bool IsActive { get; set; }

    }

    public class EventUserInfo
    {
        public Int64 RId { get; set; }

        public Int64 EventId { get; set; }

        public Decimal MemberAmount { get; set; }

        public Decimal NonMemberAmount { get; set; }

        public Int64 MemberId { get; set; }

        public string EventName { get; set; }

        public string Location { get; set; }
        public string Honeypot { get; set; }
        public string recaptchaToken { get; set; }


        public Int64 EventUserInfoId  { get; set; }

	    public string FirstName  { get; set; }

	    public string LastName { get; set; }
	    public string payamount { get; set; }

	    public string Email  { get; set; }

	    public string Gender  { get; set; }

	    public string Address  { get; set; }

	    public string City  { get; set; }
	    public string IsQRCode { get; set; }

	    public string State  { get; set; }

        public string ZipCode { get; set; }

        public string CardNumber { get; set; }

        public string CSVMonth { get; set; }

        public string CSVYear { get; set; }

        public string Cvv { get; set; }

        public string Mobile  { get; set; }

        public string ItemName { get; set; }

        public string ItemCategory { get; set; }

        public string ItemDescription { get; set; }

        public int ItemDuration { get; set; }

	    public Boolean IsApproved  { get; set; }

	    public string DocumentUrl  { get; set; }

	    public string UpdatedBy  { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime InsertedTime { get; set; }

        public Int32 ChildCount { get; set; }

        public Int32 AdultCount { get; set; }

        public Int32 CoupleCount { get; set; }

        public Int32 FamilyCount { get; set; }

        public Int32 VIPCount { get; set; }

        public Int32 VIPSingleAdultCount { get; set; }

        public Int32 VIPChildCount { get; set; }

        public Int32 VIPCoupleCount { get; set; }

        public string Comments { get; set; }

        public string OtherPaymentId { get; set; }

        public string MemberAge { get; set; }
        // EventOrder

        public Int64 EventOrderId { get; set; }

        public DateTime ApprovedDate { get; set; }

        public Int64 PaymentStatusId { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public Int64 PaymentMethodId { get; set; }

        public string TransactionId { get; set; }

        public Decimal AmountPaid { get; set; }

        public Boolean IsAttended { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //Event Category

        public Int64 EventCategoryId { get; set; }

        public string EventCategory { get; set; }

        public Entities.EventOrderDetails objEventOrderDetails = new EventOrderDetails();

        public List<EventParticipants> lstEventParticipant = new List<EventParticipants>();

        public string TicketTypesWithCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string XMLRegistrationsCounts { get; set; }


        public string SgHoneypot { get; set; }

        public string Status { get; set; }
        public string Comment { get; set; }
        public string CheckinCount { get; set; }

        public string Zelle { get; set; }
        public string ReceiptUrl { get; set; }
        public string ZellePartnerName { get; set; }
        public string ChequeHolderName { get; set; }




    }

    public class EventParticipants
    {
        public Int64 RId { get; set; }

        public Int64   EventParticipantId  { get; set; }

	    public Int64  EventId  { get; set; }

	    public Int64   EventUserInfoId { get; set; }

	    public string   FirstName  { get; set; }

	    public string LastName { get; set; }

	    public string Email { get; set; }

	    public string Age { get; set; }

	    public string Mobile { get; set; }

	    public Boolean IsApproved { get; set; }

	    public string UpdatedBy  { get; set; }        

	    public DateTime UpdatedTime  { get; set; }
       

    }

    public class EventOrderDetails
    {
         public Int64 EventOrderId  { get; set; }

         public Int64 EventId  { get; set; }

	     public Int64 EventUserInfoId   { get; set; }

	     public Boolean IsApproved   { get; set; }

	     public DateTime ApprovedDate   { get; set; }

	     public Int64 PaymentStatusId   { get; set; }

         public string PaymentStatus { get; set; }

         public string PaymentMethod { get; set; }

	     public DateTime PaymentDate   { get; set; }

	     public Int64 PaymentMethodId   { get; set; }

         public string TransactionId { get; set; }

	     public Decimal AmountPaid   { get; set; }

	     public Boolean IsAttended  { get; set; }

	     public string InsertedBy   { get; set; }

	     public DateTime InsertedDate   { get; set; }

	     public string UpdatedBy   { get; set; }

         public DateTime UpdatedTime { get; set; }

         public Int32 ChildCount { get; set; }

         public Int32 AdultCount { get; set; }

         public Int32 CoupleCount { get; set; }

         public Int32 FamilyCount { get; set; }

         public Int32 VIPCount { get; set; }

         public Int32 VIPSingleAdultCount { get; set; }

         public Int32 VIPChildCount { get; set; }

         public Int32 VIPCoupleCount { get; set; }

         public string SulekhaId { get; set; }
    }


    public class EventComments
    {
        public Int64 EventCommentId { get; set; }
        public Int64 EventUserInfoId { get; set; }
        public string Status { get; set; }
        public string AdminComment { get; set; }
        public string Comment { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string EmployeeRefUrl { get; set; }
        public string EmployeeCode { get; set; }
        public Int32 CheckinCount { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }

    }

    // ViewModel to receive the form data
    public class EventCheckInModel
    {
        public string EventUserInfoId { get; set; }
        public string EventId { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public List<RegistrationCheckInItem> RegistrationItems { get; set; }
    }

    public class RegistrationCheckInItem
    {
        public string RegistrationTitle { get; set; }
        public int CheckinCount { get; set; }
        public int TotalCount { get; set; }
        public Int64 EventRegistrationTypeId { get; set; }
        public decimal Amount { get; set; }
    }


}
