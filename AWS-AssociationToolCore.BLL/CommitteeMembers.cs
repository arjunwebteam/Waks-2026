using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;
using System.Data;
namespace AWS_AssociationToolCore.BLL
{
  public  class CommitteeMembers
    {
        AWS_AssociationToolCore.DAL.CommitteeMembers _CommitteeMembers = new AWS_AssociationToolCore.DAL.CommitteeMembers();

        #region Methods

        public Int64 InsertCommitteeMembers(Entities.CommitteeMembers objCommitteeMembers)
        {
            Int64 _status = 0;
            if (objCommitteeMembers != null)
            {
                _status = _CommitteeMembers.InsertCommitteeMembers(objCommitteeMembers);

            }
            return _status;
        }

        public Int64 DeleteCommitteeMember(Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers.DeleteCommitteeMember(CommitteeMemberId);
            return _status;
        }

        public Int64 UpdateCommitteeMemberDisplayOrder(int DisplayOrder, Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers.UpdateCommitteeMemberDisplayOrder(DisplayOrder, CommitteeMemberId);
            return _status;
        }

        public Int64 CommitteeMembersUpdateStatus(Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers.CommitteeMembersUpdateStatus(CommitteeMemberId);
            return _status;
        }



        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.CommitteeMembers> GetCommitteeMembersList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers> lstCommitteeMembers = new List<AWS_AssociationToolCore.Entities.CommitteeMembers>();
            DataTable dt = _CommitteeMembers.GetCommitteeMembersList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers objlstCommitteeMembers = new AWS_AssociationToolCore.Entities.CommitteeMembers();

                    objlstCommitteeMembers.RId = Convert.ToInt64(dr["RId"].ToString());
                    objlstCommitteeMembers.CommitteeMemberId = Convert.ToInt64(dr["CommitteeMemberId"].ToString());
                    objlstCommitteeMembers.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objlstCommitteeMembers.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeMembers.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : null);

                    lstCommitteeMembers.Add(objlstCommitteeMembers);
                }

            }
            return lstCommitteeMembers;
        }        

        public AWS_AssociationToolCore.Entities.CommitteeMembers GetCommitteeMemberById(Int64 CommitteeMemberId, ref int status)
        {
            AWS_AssociationToolCore.Entities.CommitteeMembers objCommitteeMembers = new AWS_AssociationToolCore.Entities.CommitteeMembers();
            DataTable dt = new DataTable();
            if (CommitteeMemberId != 0)
            {
                dt = _CommitteeMembers.GetCommitteeMemberById(CommitteeMemberId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objCommitteeMembers.CommitteeCategoryId = Convert.ToInt64(dt.Rows[0]["CommitteeCategoryId"].ToString());
                    objCommitteeMembers.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["ChapterId"]) : 0);
                    objCommitteeMembers.MemberId = Convert.ToInt64(dt.Rows[0]["MemberId"].ToString());
                    objCommitteeMembers.CommitteeMemberId = Convert.ToInt64(dt.Rows[0]["CommitteeMemberId"].ToString());
                    objCommitteeMembers.DisplayOrder = (dt.Rows[0]["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["DisplayOrder"]) : 0);
                    objCommitteeMembers.Designation = (dt.Rows[0]["Designation"] != DBNull.Value ? dt.Rows[0]["Designation"].ToString() : null);
                    objCommitteeMembers.Description = (dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : null);

                }
            }
            return objCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeMembers> GetCommitteeMembersListByVariable(string Search, string Location, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers> lstCommitteeMembers = new List<AWS_AssociationToolCore.Entities.CommitteeMembers>();
            DataTable dt = _CommitteeMembers.GetCommitteeMembersListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CommitteeMembers.GetCommitteeMembersListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers objCommitteeMembers = new AWS_AssociationToolCore.Entities.CommitteeMembers();

                    objCommitteeMembers.CommitteeMemberId = Convert.ToInt64(dr["CommitteeMemberId"].ToString());
                    objCommitteeMembers.MemberId = Convert.ToInt64 (dr["MemberId"].ToString());
                    objCommitteeMembers.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommitteeMembers.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : null);

                    lstCommitteeMembers.Add(objCommitteeMembers);
                }
            }
            return lstCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.Members> CommitteeMembersList(Int64 CommitteeCategoryId, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Members> lstMembers = new List<AWS_AssociationToolCore.Entities.Members>();
            DataTable dt = _CommitteeMembers.CommitteeMembersList(CommitteeCategoryId, ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CommitteeMembers.CommitteeMembersList(CommitteeCategoryId, ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.RId = Convert.ToInt64(dr["RId"].ToString());
                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.CommitteeMemberId = (dr["CommitteeMemberId"] != DBNull.Value ? Convert.ToInt32(dr["CommitteeMemberId"]) : 0);
                    objCommittees.CommitteeCategoryId = (dr["CommitteeCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["CommitteeCategoryId"]) : 0);
                    objCommittees.Name = dr["Name"].ToString();
                    objCommittees.MobilePhone = dr["MobilePhone"].ToString();
                    objCommittees.Address = dr["Address"].ToString();
                    objCommittees.City = dr["City"].ToString();
                    objCommittees.State = dr["State"].ToString();
                    objCommittees.Email = dr["Email"].ToString();
                    objCommittees.ProfileImage = dr["ProfileImage"].ToString();
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);                    
                    objCommittees.IsApproved = (dr["IsApproved"] != DBNull.Value ? Convert.ToBoolean(dr["IsApproved"]) : false);
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    objCommittees.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);


                    lstMembers.Add(objCommittees);
                }
            }
            return lstMembers;
        }

        #endregion
    }
}
