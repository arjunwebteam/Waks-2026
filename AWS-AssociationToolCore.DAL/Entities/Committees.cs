using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
  public  class Committees
    {
        public Int64 CommitteeId { get; set; }

        public Int64 CommitteeMemberId { get; set; }

        public Int64 RId { get; set; }      

        public string Name { get; set; }

        public string PhoneNo { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Email { get; set; }

        public string ImageUrl { get; set; }

        public Int64 DisplayOrder { get; set;}

        public Boolean IsActive { get; set; }

        public string Description { get; set; }

        public string Designation { get; set; }
      
        public string UpdatedBy { get; set; }
 
        public DateTime UpdatedTime { get; set; }

        public Int64 CommitteeMemberCount { get; set; }

        public string SecretaryEmail { get; set; }

        public string CoordinatorEmail { get; set; }

        //Category


        public Int64 CommitteeCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Type { get; set; }

        public List<Committees> lstCommittees { get; set; }

        public List<CommitteeCategories> lstCommitteeCategories { get; set; }

    }
}
