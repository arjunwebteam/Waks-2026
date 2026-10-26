using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class CovidVaccine
    {
        public Int64 RId { get; set; }
        public Int64 CovidVaccineId { get; set; }
        public Int64 MemberId { get; set; }
        public Int64 ChapterId { get; set; }
        public string Email { get; set; }
        public string COVIDVaccinefor { get; set; }
        public string KNameOfChild { get; set; }
        public DateTime KDOB { get; set; }
        public string KNameOfParent { get; set; }
        public string KMailingAddress { get; set; }
        public string KPhoneNumber { get; set; }
        public string KHaveAnInsurance { get; set; }
        public string KAdditionalInformation { get; set; }
        public string ABoosterShotSelection { get; set; }
        public string AName { get; set; }
        public DateTime ADOB { get; set; }
        public string AMailingAddress { get; set; }
        public string APhoneNumber { get; set; }
        public string APreferredTimeSlots { get; set; }
        public string APrintTimeSlot { get; set; }
        public bool IsActive { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string ChapterName { get; set; }
    }
}
