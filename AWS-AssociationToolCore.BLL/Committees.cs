using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;
using System.Data;
namespace AWS_AssociationToolCore.BLL
{
   public class Committees
    {
        AWS_AssociationToolCore.DAL.Committees _Committees = new AWS_AssociationToolCore.DAL.Committees();
        AWS_AssociationToolCore.DAL.CommitteeCategories _CommitteeCategories = new AWS_AssociationToolCore.DAL.CommitteeCategories();
        //string CommiteeImage = System.Configuration.ConfigurationManager.AppSettings["adminsiteurl"] + "committees/thumbphoto/";

        #region Admin

        #region Methods

        public Int64 InsertCommittees(Entities.Committees objCommittees, ref string ImageUrl)
        {
            Int64 _status = 0;
            if (objCommittees != null )
            {
                _status = _Committees.InsertCommittees(objCommittees, ref ImageUrl);

            }
            return _status;
        }

        public Int64 DeleteCommittee(Int64 CommitteeId)
        {
            Int64 _status = 0;
            _status = _Committees.DeleteCommittee(CommitteeId);
            return _status;
        }

        public Int64 UpdateCommitteeStatus(Int64 CommitteeId)
        {
            Int64 _status = 0;
            _status = _Committees.UpdateCommitteeStatus(CommitteeId);
            return _status;
        }

        public Int64 UpdateCommitteesDisplayOrder(int DisplayOrder, Int64 CommitteeId)
        {
            Int64 _status = 0;
            _status = _Committees.UpdateCommitteesDisplayOrder(DisplayOrder, CommitteeId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.Committees> GetCommitteesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Committees> lstCommittees = new List<Entities.Committees>();
            DataTable dt = _Committees.GetCommitteesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Committees objlstCommittees = new AWS_AssociationToolCore.Entities.Committees();

                    objlstCommittees.CommitteeId =Convert.ToInt32( dr["CommitteeId"].ToString());
                    objlstCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objlstCommittees.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
                    objlstCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objlstCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objlstCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objlstCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objlstCommittees.ImageUrl = dr["ImageUrl"].ToString();
                    objlstCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objlstCommittees.IsActive =Convert.ToBoolean(dr["IsActive"]);
                    objlstCommittees.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
                    objlstCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstCommittees.UpdatedTime =Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstCommittees.Add(objlstCommittees);
                }

            }
            return lstCommittees;
        }

        public AWS_AssociationToolCore.Entities.Committees GetCommitteeById(Int64 CommitteeId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Committees objCommittees = new AWS_AssociationToolCore.Entities.Committees();
            DataTable dt = new DataTable();
            if (CommitteeId != 0)
            {
                dt = _Committees.GetCommitteeById(CommitteeId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objCommittees.CommitteeId = Convert.ToInt64(dt.Rows[0]["CommitteeId"].ToString());
                    objCommittees.Name = (dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : null);
                    objCommittees.PhoneNo = (dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null);
                    objCommittees.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                    objCommittees.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                    objCommittees.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : null);
                    objCommittees.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : null);
                    objCommittees.ImageUrl = (dt.Rows[0]["ImageUrl"] != DBNull.Value ? dt.Rows[0]["ImageUrl"].ToString() : null);
                    objCommittees.DisplayOrder = (dt.Rows[0]["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["DisplayOrder"]) : 0);
                    objCommittees.IsActive =Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objCommittees.Description = (dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : "");
                    objCommittees.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());
                  
                }
            }
            return objCommittees;
        }
        public List<AWS_AssociationToolCore.Entities.CommitteeMembers> CommitteesGetListByeditId(Int64 CommitteeCategoryId, Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers> lstCommitteeMembers = new List<AWS_AssociationToolCore.Entities.CommitteeMembers>();
            DataTable dt = _Committees.CommitteesGetListByeditId(CommitteeCategoryId, ChapterId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers objlstCommitteeMembers = new AWS_AssociationToolCore.Entities.CommitteeMembers();

                    objlstCommitteeMembers.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objlstCommitteeMembers.Name = dr["Name"].ToString();

                    lstCommitteeMembers.Add(objlstCommitteeMembers);
                }

            }
            return lstCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeMembers> GetCommitteeMembersListById(Int64 CommitteeCategoryId,Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeMembers> lstCommitteeMembers = new List<AWS_AssociationToolCore.Entities.CommitteeMembers>();
            DataTable dt = _Committees.GetCommitteeMembersListById(CommitteeCategoryId, ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeMembers objlstCommitteeMembers = new AWS_AssociationToolCore.Entities.CommitteeMembers();

                    objlstCommitteeMembers.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objlstCommitteeMembers.Name = dr["Name"].ToString();

                    lstCommitteeMembers.Add(objlstCommitteeMembers);
                }

            }
            return lstCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.Committees> GetCommitteesListByVariable(Int64 CommitteeCategoryId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Committees> lstCommittees = new List<AWS_AssociationToolCore.Entities.Committees>();
            DataTable dt = _Committees.GetCommitteesListByVariable(CommitteeCategoryId,Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Committees.GetCommitteesListByVariable(CommitteeCategoryId,Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Committees objCommittees = new AWS_AssociationToolCore.Entities.Committees();

                    objCommittees.RId = Convert.ToInt64(dr["RId"].ToString());
                    objCommittees.CommitteeId = Convert.ToInt64(dr["CommitteeId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsActive =Convert.ToBoolean(dr["IsActive"]);
                    objCommittees.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeMemberCount"].ToString());

                    lstCommittees.Add(objCommittees);
                }
            }
            return lstCommittees;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeCategories> GetCommitteeYears(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            DataTable dt = _Committees.GetCommitteeYears(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objlstCommitteeCategories.Year = dr["Year"].ToString();

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }

            }
            return lstCommitteeCategories;
        }

        public AWS_AssociationToolCore.Entities.Committees GetEventCommitteeByChapterId(Int64 ChapterId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Committees objCommittees = new AWS_AssociationToolCore.Entities.Committees();
            DataTable dt = new DataTable();
            if (ChapterId != 0)
            {
                dt = _Committees.GetEventCommitteeByChapterId(ChapterId, ref status);
                if (dt.Rows.Count == 1)
                { 
                    objCommittees.SecretaryEmail = (dt.Rows[0]["SecretaryEmail"] != DBNull.Value ? dt.Rows[0]["SecretaryEmail"].ToString() : null);
                    //objCommittees.CoordinatorEmail = (dt.Rows[0]["CoordinatorEmail"] != DBNull.Value ? dt.Rows[0]["CoordinatorEmail"].ToString() : null); 

                }
            }
            return objCommittees;
        }

        #endregion

        #endregion

        #region Front End

        public List<AWS_AssociationToolCore.Entities.Members> FECommitteesGetList(ref List<AWS_AssociationToolCore.Entities.CommitteeCategoryMaster> lstCommitteeCategories, ref List<AWS_AssociationToolCore.Entities.Members> lstCommitteeMembers, ref List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeTypes, string Year, Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Members> lstCommittees = new List<AWS_AssociationToolCore.Entities.Members>();
             
                DataSet ds = _Committees.FECommitteesGetList(Year, ChapterId, ref status);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                        objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                        objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                        objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                        objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                        objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                        objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                        objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                        objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                        objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                        objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                        objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                        objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                        //objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                        objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                        objCommittees.CategoryName = dr["CategoryName"].ToString();
                        objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                        objCommittees.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : "");
                        //objCommittees.Type = dr["Type"].ToString();

                        lstCommittees.Add(objCommittees);
                    }
                }
            DataTable dt1 = ds.Tables[1];
            if (dt1.Rows.Count != 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategoryMaster objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategoryMaster();

                    objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    //objlstCommitteeCategories.Type = dr["Type"].ToString();

                   
                    //objlstCommitteeCategories.Position = Convert.ToInt32(dr["Position"]);
                    //objlstCommitteeCategories.Position = (dt.Rows[0]["Position"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["Position"]) : 0);

                    //objlstCommitteeCategories.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);

                    //objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    //objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    //objlstCommitteeCategories.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeCount"]);

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }
            }

        
             
            return lstCommittees;
        }

        public List<AWS_AssociationToolCore.Entities.Members> APPFECommitteesCategoryGetList(ref List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories, ref List<AWS_AssociationToolCore.Entities.Members> lstCommitteeMembers, ref List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeTypes, string Type, Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Members> lstCommittees = new List<AWS_AssociationToolCore.Entities.Members>();

            DataSet ds = _Committees.APPFECommitteesCategoryGetList(Type, ChapterId, ref status);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommittees.CategoryName = dr["CategoryName"].ToString();
                    objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                    objCommittees.Type = dr["Type"].ToString();

                    lstCommittees.Add(objCommittees);
                }
            }
            DataTable dt1 = ds.Tables[1];
            if (dt1.Rows.Count != 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommittees.CategoryName = dr["CategoryName"].ToString();
                    objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                    objCommittees.Type = dr["Type"].ToString();

                    lstCommitteeMembers.Add(objCommittees);
                }
            }

            DataTable dt2 = ds.Tables[2];
            if (dt2.Rows.Count != 0)
            {
                foreach (DataRow dr in dt2.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeTypes = new AWS_AssociationToolCore.Entities.CommitteeCategories();


                    objlstCommitteeTypes.Type = (dr["Type"] != DBNull.Value ? dr["Type"].ToString() : "");

                    lstCommitteeTypes.Add(objlstCommitteeTypes);
                }
            }

            DataTable dt3 = ds.Tables[3];
            if (dt3.Rows.Count != 0)
            {
                foreach (DataRow dr in dt3.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstCommitteeCategories.Type = dr["Type"].ToString();
                    objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                    objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objlstCommitteeCategories.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeCount"]);

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }
            }

            return lstCommittees;
        }

        public List<AWS_AssociationToolCore.Entities.Members> APPFECommitteesGetList(ref List<AWS_AssociationToolCore.Entities.Members> lstCommitteeMembers, string Type, Int64 ChapterId, ref int status)
        { 
            DataSet ds = _Committees.APPFECommitteesGetList(Type, ChapterId, ref status);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommittees.CategoryName = dr["CategoryName"].ToString();
                    objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                    objCommittees.Type = dr["Type"].ToString();

                    lstCommitteeMembers.Add(objCommittees);
                }
            }
            
            return lstCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.Members> FEHelplineCommitteesGetList(ref List<AWS_AssociationToolCore.Entities.Members> lstCommitteeMembers, string Type, Int64 ChapterId, ref int status)
        {
            DataSet ds = _Committees.FEHelplineCommitteesGetList(Type, ChapterId, ref status);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommittees.CategoryName = dr["CategoryName"].ToString();
                    objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                    objCommittees.Type = dr["Type"].ToString();

                    lstCommitteeMembers.Add(objCommittees);
                }
            }

            return lstCommitteeMembers;
        }

        public List<AWS_AssociationToolCore.Entities.Members> FEPastCommitteesGetList(ref List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories, string Type, string Year, Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Members> lstCommittees = new List<AWS_AssociationToolCore.Entities.Members>();
            
                DataSet ds = _Committees.FEPastCommitteesGetList(Type, Year, ChapterId, ref status);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AWS_AssociationToolCore.Entities.Members objCommittees = new AWS_AssociationToolCore.Entities.Members();

                    objCommittees.MemberId = Convert.ToInt64(dr["MemberId"].ToString());
                    objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
                    objCommittees.MobilePhone = (dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null);
                    objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCommittees.ProfileImage = (dr["ProfileImage"] != DBNull.Value ? dr["ProfileImage"].ToString() : null);
                    objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    objCommittees.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objCommittees.Occupation = (dr["Occupation"] != DBNull.Value ? dr["Occupation"].ToString() : "");
                    objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommittees.CategoryName = dr["CategoryName"].ToString();
                    objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
                    objCommittees.Type = dr["Type"].ToString();

                    lstCommittees.Add(objCommittees);
                    }
                }
                DataTable dt1 = ds.Tables[1];
                if (dt1.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                    {
                        AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                        objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                        objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                        objlstCommitteeCategories.Type = dr["Type"].ToString();
                        objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                        objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objlstCommitteeCategories.Year = dr["Year"].ToString();
                        objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                        objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                        objlstCommitteeCategories.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeCount"]);

                        lstCommitteeCategories.Add(objlstCommitteeCategories);
                    }
                }
            return lstCommittees;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeCategoryMaster> Committeeyears(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = new List<Entities.CommitteeCategoryMaster>();
            DataTable dt = _Committees.GetCommitteeyears(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategoryMaster objlstCommittees = new AWS_AssociationToolCore.Entities.CommitteeCategoryMaster();

                    objlstCommittees.YearId = Convert.ToInt32(dr["YearId"].ToString());

                    lstCommitteeCategoryMaster.Add(objlstCommittees);
                }

            }
            return lstCommitteeCategoryMaster;
        }
















        #endregion
        //#region API
        //public List<AWS_AssociationToolCore.Entities.Committees> APIFECommitteesGetList(ref List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories, string Type, Int64 ChapterId, ref int status)
        //{
        //    List<AWS_AssociationToolCore.Entities.Committees> lstCommittees = new List<AWS_AssociationToolCore.Entities.Committees>();
        //    if (Type != "")
        //    {
        //        DataSet ds = _Committees.FECommitteesGetList(Type, ChapterId, ref status);
        //        DataTable dt = ds.Tables[0];
        //        string ImageAPI= "API";
        //        if (dt.Rows.Count != 0)
        //        {
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                AWS_AssociationToolCore.Entities.Committees objCommittees = new AWS_AssociationToolCore.Entities.Committees();

        //                objCommittees.CommitteeId = Convert.ToInt64(dr["CommitteeId"].ToString());
        //                objCommittees.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);
        //                objCommittees.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
        //                objCommittees.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
        //                objCommittees.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
        //                objCommittees.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
        //                objCommittees.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
        //                if(ImageAPI=="API")
        //                {
        //                    objCommittees.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? CommiteeImage + dr["ImageUrl"].ToString() : "");
        //                }
        //                else
        //                {
        //                    objCommittees.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : null);
        //                }
        //                objCommittees.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
        //                objCommittees.IsActive = Convert.ToBoolean(dr["IsActive"]);
        //                objCommittees.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : "");
        //                objCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
        //                objCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
        //                objCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
        //                objCommittees.CategoryName = dr["CategoryName"].ToString();
        //                objCommittees.Designation = (dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : "");
        //                objCommittees.Type = dr["Type"].ToString();

        //                lstCommittees.Add(objCommittees);
        //            }
        //        }
        //        DataTable dt1 = ds.Tables[1];
        //        if (dt1.Rows.Count != 0)
        //        {
        //            foreach (DataRow dr in dt1.Rows)
        //            {
        //                AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

        //                objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
        //                objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
        //                objlstCommitteeCategories.Type = dr["Type"].ToString();
        //                objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
        //                objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
        //                objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
        //                objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
        //                objlstCommitteeCategories.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeCount"]);

        //                lstCommitteeCategories.Add(objlstCommitteeCategories);
        //            }
        //        }
        //    }
        //    return lstCommittees;
        //}

        //#endregion
    }

}
