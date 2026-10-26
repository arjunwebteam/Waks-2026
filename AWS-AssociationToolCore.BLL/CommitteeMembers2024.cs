using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;
using System.Data;
namespace AWS_AssociationToolCore.BLL
{
  public  class CommitteeMembers2024
    {
        AWS_AssociationToolCore.DAL.CommitteeMembers2024 _CommitteeMembers2024 = new AWS_AssociationToolCore.DAL.CommitteeMembers2024();

        #region Methods

        public Int64 InsertCommitteeMembers2024(Entities.CommitteeMembers2024 objCommitteeMembers2024)
        {
            Int64 _status = 0;
            if (objCommitteeMembers2024 != null)
            {
                _status = _CommitteeMembers2024.InsertCommitteeMembers2024(objCommitteeMembers2024);

            }
            return _status;
        }

        public Int64 DeleteCommitteeMember(Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers2024.DeleteCommitteeMember(CommitteeMemberId);
            return _status;
        }

        public Int64 UpdateCommitteeMemberDisplayOrder(int DisplayOrder, Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers2024.UpdateCommitteeMemberDisplayOrder(DisplayOrder, CommitteeMemberId);
            return _status;
        }

        public Int64 CommitteeMembers2024UpdateStatus(Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            _status = _CommitteeMembers2024.CommitteeMembers2024UpdateStatus(CommitteeMemberId);
            return _status;
        }



        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.CommitteeMembers2024> GetCommitteeMembers2024List(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers2024> lstCommitteeMembers2024 = new List<AWS_AssociationToolCore.Entities.CommitteeMembers2024>();
            DataTable dt = _CommitteeMembers2024.GetCommitteeMembers2024List(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers2024 objlstCommitteeMembers2024 = new AWS_AssociationToolCore.Entities.CommitteeMembers2024();

                    objlstCommitteeMembers2024.RId = Convert.ToInt64(dr["RId"].ToString());
                    objlstCommitteeMembers2024.CommitteeMemberId = Convert.ToInt64(dr["CommitteeMemberId"].ToString());
                    objlstCommitteeMembers2024.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objlstCommitteeMembers2024.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeMembers2024.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : null);

                    lstCommitteeMembers2024.Add(objlstCommitteeMembers2024);
                }

            }
            return lstCommitteeMembers2024;
        }        

        public AWS_AssociationToolCore.Entities.CommitteeMembers2024 GetCommitteeMemberById(Int64 CommitteeMemberId, ref int status)
        {
            AWS_AssociationToolCore.Entities.CommitteeMembers2024 objCommitteeMembers2024 = new AWS_AssociationToolCore.Entities.CommitteeMembers2024();
            DataTable dt = new DataTable();
            if (CommitteeMemberId != 0)
            {
                dt = _CommitteeMembers2024.GetCommitteeMemberById(CommitteeMemberId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objCommitteeMembers2024.CommitteeCategoryId = Convert.ToInt64(dt.Rows[0]["CommitteeCategoryId"].ToString());
                    objCommitteeMembers2024.MemberId = Convert.ToInt64(dt.Rows[0]["MemberId"].ToString());
                    objCommitteeMembers2024.CommitteeMemberId = Convert.ToInt64(dt.Rows[0]["CommitteeMemberId"].ToString());
                    objCommitteeMembers2024.DisplayOrder = (dt.Rows[0]["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["DisplayOrder"]) : 0);
                    objCommitteeMembers2024.Designation = (dt.Rows[0]["Designation"] != DBNull.Value ? dt.Rows[0]["Designation"].ToString() : null);

                }
            }
            return objCommitteeMembers2024;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeMembers2024> GetCommitteeMembers2024ListByVariable(string Search, string Location, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers2024> lstCommitteeMembers2024 = new List<AWS_AssociationToolCore.Entities.CommitteeMembers2024>();
            DataTable dt = _CommitteeMembers2024.GetCommitteeMembers2024ListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CommitteeMembers2024.GetCommitteeMembers2024ListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers2024 objCommitteeMembers2024 = new AWS_AssociationToolCore.Entities.CommitteeMembers2024();

                    objCommitteeMembers2024.CommitteeMemberId = Convert.ToInt64(dr["CommitteeMemberId"].ToString());
                    objCommitteeMembers2024.MemberId = Convert.ToInt64 (dr["MemberId"].ToString());
                    objCommitteeMembers2024.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommitteeMembers2024.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : null);

                    lstCommitteeMembers2024.Add(objCommitteeMembers2024);
                }
            }
            return lstCommitteeMembers2024;
        }

        public List<AWS_AssociationToolCore.Entities.Members> CommitteeMembers2024List(Int64 CommitteeCategoryId, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Members> lstMembers = new List<AWS_AssociationToolCore.Entities.Members>();
            DataTable dt = _CommitteeMembers2024.CommitteeMembers2024List(CommitteeCategoryId, ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CommitteeMembers2024.CommitteeMembers2024List(CommitteeCategoryId, ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
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
