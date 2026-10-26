using System;

namespace AWS_AssociationToolCore.Models
{
    /// <summary>
    /// View model for generating PDF tickets
    /// </summary>
    public class TicketPdfViewModel
    {
        // Event Information
        public string EventName { get; set; }
        public long EventId { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public string EventDescription { get; set; }
        public string LogoUrl { get; set; }
        public string CalendarUrl { get; set; }
        public string LocationUrl { get; set; }
        public string HOLDERUrl { get; set; }
        public string TicketDetailsUrl { get; set; }
        public string routeUrl { get; set; }

        // Ticket Information
        public string TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string RegistrationTitle { get; set; }
        public string Category { get; set; }

        // User/Sponsor Information
        public string SponsorName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Payment Information
        public string TransactionId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        // QR Code
        public string QRCodeUrl { get; set; }

        // Organization Information
        public string SiteName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        

        // Helper properties for date formatting
        public string Month => StartDate.ToString("MMM").ToUpper();
        public string Year => StartDate.Year.ToString();
        public string Day => StartDate.Day.ToString();
        public string DaySuffix
        {
            get
            {
                int day = StartDate.Day;
                switch (day)
                {
                    case 1:
                    case 21:
                    case 31:
                        return "ST";
                    case 2:
                    case 22:
                        return "ND";
                    case 3:
                    case 23:
                        return "RD";
                    default:
                        return "TH";
                }
            }
        }
        public string Time => StartDate.ToString("hh:mm tt");
    }
}