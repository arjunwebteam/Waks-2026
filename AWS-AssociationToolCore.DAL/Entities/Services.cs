using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
  public  class Services
    {
        public Int64 RId { get; set; }
        public Int64 ServiceId { get; set; }
        public Int64 ChapterId { get; set; }
        public Int64 CategoryId { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceType { get; set; }
        public string OrganisedBy { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal EstimationAmount { get; set; }
        public string ServiceStatus { get; set; }
        public Boolean IsDisplay { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public string ChapterName { get; set; }
        public string CategoryName { get; set; }
        public string ServiceTypeName { get; set; }
        public Int64 DisplayOrder { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalAmount { get; set; }
        public Int64 ServiceDonationsCount { get; set; }
    }


    public class ServiceImages
    {
        public Int64 RId { get; set; }
        public Int64 ServiceImageId { get; set; }
        public Int64 ServiceId { get; set; }
        public string ImageUrl { get; set; }
        public Boolean IsDefault { get; set; }
        public string ImageDescription { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ServiceTitle { get; set; }
    }

    public class ServiceContributions
    { 
        public Int64 ContributionId { get; set; }
        public Int64 ServiceId { get; set; }
        public Int64 ServiceDonationId { get; set; } 
        public string Percentage { get; set; }
        public string ServiceTitle { get; set; }
        public decimal CAmount { get; set; }
        public decimal Amount { get; set; }
    }



}
