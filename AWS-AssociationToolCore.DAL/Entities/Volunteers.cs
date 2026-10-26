using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class Volunteers
    {
        public Int64 RId { get; set; }

        public Int64 VolunteerId { get; set; }

        public Int64 VolunteerCategoryId { get; set; }

        public Int64 EventId { get; set; }

	    public string FirstName  { get; set; }

	    public string LastName  { get; set; }

	    public string Email  { get; set; }

	    public string PhoneNo  { get; set; }

	    public string Address  { get; set; }

        public string Comments { get; set; }

        public string HoursSpent { get; set; }

        public string Field1 { get; set; }

        public string Field2 { get; set; }

	    public bool IsActive  { get; set; }

	    public string InsertedBy  { get; set; }

	    public DateTime InsertedDate { get; set; }

	    public string UpdatedBy  { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string CategoryName { get; set; }

        public DateTime StartDate { get; set; }
        public string EventName { get; set; }

        public Int64 MemberId { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Address2 { get; set; }
        public Int64 ChapterId { get; set; }

        public string ChapterName { get; set; }

        public string SelfCity { get; set; }
        public string SelfDistrict { get; set; }
        public string SelfPhoneNo { get; set; }
        public string SelfName { get; set; }
        public string SelfRelation { get; set; }
        public string VolunteerCategories { get; set; }
        public List<VolunteerIntrests> lstVolunteerIntrests { get; set; }
        public string WaksCategoryId { get; set; }
        public Int64 KannadaCategoryId { get; set; }
        public string SgHoneypot { get; set; }
        public string WaksCategoryName { get; set; }
        public string KannadaCategoryName { get; set; }

    }

    public class VolunteerIntrests
    {
        public Int64 VolunteerIntrestsId { get; set; }

        public Int64 VolunteerId { get; set; }

        public Int64 VolunteerCategoryId { get; set; }

        public string Field1 { get; set; } 

    }
    public class country
    {
        public Int64 CountryID { get; set; }
        public string CountryName { get; set; }

        public List<State> States { get; set; }
    }
    public class State
    {
        public string StateID { get; set; }
        public string StateName { get; set; }
    }
}
