using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class Volunteers
    {
        AWS_AssociationToolCore.DAL.Volunteers _Volunteers = new AWS_AssociationToolCore.DAL.Volunteers();
        
       
        #region Admin

        #region Methods

        public Int64 InsertVolunteers(Entities.Volunteers objVolunteers)
        {
            Int64 _status = 0;
            if (objVolunteers != null)
            {
                _status = _Volunteers.InsertVolunteers(objVolunteers);

            }
            return _status;
        }

        public Int64 DeleteVolunteer(Int64 VolunteerId)
        {
            Int64 _status = 0;
            _status = _Volunteers.DeleteVolunteer(VolunteerId);
            return _status;
        }

        public Int64 UpdateVolunteerstatus(Int64 VolunteerId)
        {
            Int64 _status = 0;
            _status = _Volunteers.UpdateVolunteerstatus(VolunteerId);
            return _status;
        }

        public Int64 UpdateVolunteerHours(string HoursSpent, Int64 VolunteerId)
        {
            Int64 _status = 0;
            _status = _Volunteers.UpdateVolunteerHours(HoursSpent, VolunteerId);
            return _status;
        }
        
        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.Volunteers GetVolunteerById(Int64 VolunteerId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();
            DataTable dt = new DataTable();
            if (VolunteerId != 0)
            {
                dt = _Volunteers.GetVolunteerById(VolunteerId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objVolunteers.VolunteerId = Convert.ToInt64(dt.Rows[0]["VolunteerId"].ToString());
                    objVolunteers.VolunteerCategoryId = (dt.Rows[0]["VolunteerCategoryId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["VolunteerCategoryId"]) : 0);
                    objVolunteers.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);
                    objVolunteers.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objVolunteers.LastName = dt.Rows[0]["LastName"].ToString();
                    objVolunteers.Email = dt.Rows[0]["Email"].ToString();
                    objVolunteers.PhoneNo = (dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null);
                    objVolunteers.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                    objVolunteers.Comments = (dt.Rows[0]["Comments"] != DBNull.Value ? dt.Rows[0]["Comments"].ToString() : null);
                    objVolunteers.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objVolunteers.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    objVolunteers.Country = (dt.Rows[0]["Country"] != DBNull.Value ? dt.Rows[0]["Country"].ToString() : null);
                    objVolunteers.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                    objVolunteers.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : null);
                    objVolunteers.ZipCode = (dt.Rows[0]["ZipCode"] != DBNull.Value ? dt.Rows[0]["ZipCode"].ToString() : null);
                    objVolunteers.Address2 = (dt.Rows[0]["Address2"] != DBNull.Value ? dt.Rows[0]["Address2"].ToString() : null);
                    objVolunteers.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                    objVolunteers.ChapterName = (dt.Rows[0]["ChapterName"] != DBNull.Value ? dt.Rows[0]["ChapterName"].ToString() : null);

                }
                
            }
            return objVolunteers;
        }

        public AWS_AssociationToolCore.Entities.Volunteers GetVolunteerInfoById(Int64 VolunteerId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();
            DataTable dt = new DataTable();
            if (VolunteerId != 0)
            {
                dt = _Volunteers.GetVolunteerInfoById(VolunteerId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objVolunteers.VolunteerId = Convert.ToInt64(dt.Rows[0]["VolunteerId"].ToString());
                    objVolunteers.VolunteerCategoryId = (dt.Rows[0]["VolunteerCategoryId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["VolunteerCategoryId"]) : 0);
                    objVolunteers.WaksCategoryId = (dt.Rows[0]["WaksCategoryId"] != DBNull.Value ? dt.Rows[0]["WaksCategoryId"].ToString() : null); 
                    objVolunteers.KannadaCategoryId = (dt.Rows[0]["KannadaCategoryId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["KannadaCategoryId"]) : 0);
                    objVolunteers.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);
                    objVolunteers.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objVolunteers.LastName = dt.Rows[0]["LastName"].ToString();
                    objVolunteers.Email = dt.Rows[0]["Email"].ToString();
                    objVolunteers.PhoneNo = (dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null);
                    objVolunteers.HoursSpent = (dt.Rows[0]["HoursSpent"] != DBNull.Value ? dt.Rows[0]["HoursSpent"].ToString() : null);
                    objVolunteers.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                    objVolunteers.Comments = (dt.Rows[0]["Comments"] != DBNull.Value ? dt.Rows[0]["Comments"].ToString() : null);
                    objVolunteers.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objVolunteers.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    objVolunteers.CategoryName = (dt.Rows[0]["CategoryName"] != DBNull.Value ? dt.Rows[0]["CategoryName"].ToString() : null);
                    objVolunteers.EventName = (dt.Rows[0]["EventName"] != DBNull.Value ? dt.Rows[0]["EventName"].ToString() : null);
                    objVolunteers.Country = (dt.Rows[0]["Country"] != DBNull.Value ? dt.Rows[0]["Country"].ToString() : null);
                    objVolunteers.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                    objVolunteers.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : null);
                    objVolunteers.ZipCode = (dt.Rows[0]["ZipCode"] != DBNull.Value ? dt.Rows[0]["ZipCode"].ToString() : null);
                    objVolunteers.Address2 = (dt.Rows[0]["Address2"] != DBNull.Value ? dt.Rows[0]["Address2"].ToString() : null);
                    objVolunteers.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                    objVolunteers.ChapterName = (dt.Rows[0]["ChapterName"] != DBNull.Value ? dt.Rows[0]["ChapterName"].ToString() : null);
                    objVolunteers.WaksCategoryName = (dt.Rows[0]["WaksCategoryName"] != DBNull.Value ? dt.Rows[0]["WaksCategoryName"].ToString() : null);
                    objVolunteers.KannadaCategoryName = (dt.Rows[0]["KannadaCategoryName"] != DBNull.Value ? dt.Rows[0]["KannadaCategoryName"].ToString() : null);

                }
            }
            return objVolunteers;
        }

        public List<AWS_AssociationToolCore.Entities.Volunteers> GetVolunteersListByVariable(
            Int64 WaksCategoryId,
            Int64 KannadaCategoryId,
            Int64 EventId,
            string Search,
            string Sort,
            int PageNo,
            int Items,
            Int64 ChapterId,
            ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Volunteers> lstVolunteers =
                new List<AWS_AssociationToolCore.Entities.Volunteers>();

            DataTable dt = _Volunteers.GetVolunteersListByVariable(
                WaksCategoryId, KannadaCategoryId, EventId,
                Search, Sort, PageNo, Items, ChapterId, ref Total);

            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Volunteers.GetVolunteersListByVariable(
                    WaksCategoryId, KannadaCategoryId, EventId,
                    Search, Sort, PageNo - 1, Items, ChapterId, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Volunteers objVolunteers =
                        new AWS_AssociationToolCore.Entities.Volunteers();

                    objVolunteers.RId = Convert.ToInt64(dr["RId"].ToString());
                    objVolunteers.VolunteerId = Convert.ToInt64(dr["VolunteerId"].ToString());
                    objVolunteers.FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null;
                    objVolunteers.LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null;

                    // ✅ Kannada category name
                    objVolunteers.CategoryName = dr["CategoryName"] != DBNull.Value
                                                 ? dr["CategoryName"].ToString() : null;

                    // ✅ Waks category name
                    objVolunteers.WaksCategoryName = dr["WaksCategoryName"] != DBNull.Value
                                                     ? dr["WaksCategoryName"].ToString() : null;

                    objVolunteers.PhoneNo = dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null;
                    objVolunteers.Address = dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null;
                    objVolunteers.Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null;
                    objVolunteers.HoursSpent = dr["HoursSpent"] != DBNull.Value ? dr["HoursSpent"].ToString() : null;
                    objVolunteers.IsActive = dr["IsActive"] != DBNull.Value
                                                 ? Convert.ToBoolean(dr["IsActive"]) : false;
                    objVolunteers.UpdatedBy = dr["UpdatedBy"].ToString();
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    lstVolunteers.Add(objVolunteers);
                }
            }

            return lstVolunteers;
        }
        public Entities.Volunteers VolunteerValidationByEmail(string Email, ref int status)
        {
            AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();
            DataTable dt = _Volunteers.VolunteerValidationByEmail(Email, ref status);

            if (Email != null && Email.Trim() != "")
            {
                if (dt.Rows.Count == 1)
                {
                    objVolunteers.VolunteerId = Convert.ToInt64(dt.Rows[0]["VolunteerId"]);
                    objVolunteers.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objVolunteers.LastName = dt.Rows[0]["LastName"].ToString();
                    objVolunteers.Email = dt.Rows[0]["Email"].ToString();          
                }
            }
            return objVolunteers;
        }


        public List<AWS_AssociationToolCore.Entities.State> GetStatesByCountryID(Int64 CountryID, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.State> lststate = new List<AWS_AssociationToolCore.Entities.State>();

            DataTable dt = _Volunteers.GetStatesByCountryID(CountryID, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.State objstate = new AWS_AssociationToolCore.Entities.State();
                    
                    objstate.StateID = (dr["StateID"].ToString());
                    objstate.StateName = (dr["StateName"] != DBNull.Value ? (dr["StateName"].ToString()) : null);


                    lststate.Add(objstate);
                }

            }
            return lststate;
        }
        public List<AWS_AssociationToolCore.Entities.country> GetCountriesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.country> lstcountry = new List<Entities.country>();
            DataTable dt = _Volunteers.GetCountriesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.country objcountry = new AWS_AssociationToolCore.Entities.country();
                    objcountry.CountryID = Convert.ToInt64(dr["CountryID"].ToString());
                    objcountry.CountryName = (dr["CountryName"] != DBNull.Value ? (dr["CountryName"].ToString()) : null);


                    lstcountry.Add(objcountry);
                }

            }
            return lstcountry;
        }


        #endregion

        #endregion

        #region Front-end

        public List<AWS_AssociationToolCore.Entities.Volunteers> FEGetVolunteersList(Int64 VolunteerCategoryId, Int64 EventId, string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Volunteers> lstVolunteers = new List<AWS_AssociationToolCore.Entities.Volunteers>();
            DataTable dt = _Volunteers.FEGetVolunteersList(VolunteerCategoryId, EventId, Email, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Volunteers.FEGetVolunteersList(VolunteerCategoryId, EventId, Email, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();

                    objVolunteers.RId = Convert.ToInt64(dr["RId"].ToString());
                    objVolunteers.VolunteerId = Convert.ToInt64(dr["VolunteerId"]);
                    objVolunteers.FirstName = dr["FirstName"].ToString();
                    objVolunteers.LastName = dr["LastName"].ToString();
                    objVolunteers.Email = dr["Email"].ToString();
                    objVolunteers.PhoneNo = dr["PhoneNo"].ToString();
                    objVolunteers.Address = dr["Address"].ToString();
                    objVolunteers.HoursSpent = dr["HoursSpent"].ToString();
                    //objVolunteers.Comments = dr["Comments"].ToString();
                    objVolunteers.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    objVolunteers.UpdatedBy = dr["UpdatedBy"].ToString();
                    objVolunteers.CategoryName = dr["CategoryName"].ToString();
                    //objVolunteers.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                    //objVolunteers.InsertedBy = dr["InsertedBy"].ToString();
                    lstVolunteers.Add(objVolunteers);
                }

            }
            return lstVolunteers;
        }

        public Int64 FEVolunteerInsert(Entities.Volunteers objVolunteer)
        {
            Int64 _status = 0;
            if (objVolunteer != null)
            {
                _status = _Volunteers.FEVolunteerInsert(objVolunteer);

            }
            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.Volunteers> FEVolunteersList(Int64 MemberId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Volunteers> lstVolunteers = new List<AWS_AssociationToolCore.Entities.Volunteers>();
            DataTable dt = _Volunteers.FEVolunteersList(MemberId, ref Total);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();

                    objVolunteers.VolunteerId = Convert.ToInt64(dr["VolunteerId"]);
                    objVolunteers.MemberId = Convert.ToInt64(dr["MemberId"]);
                    objVolunteers.FirstName = dr["FirstName"].ToString();
                    objVolunteers.LastName = dr["LastName"].ToString();
                    objVolunteers.Email = dr["Email"].ToString();
                    objVolunteers.PhoneNo = dr["PhoneNo"].ToString();
                    objVolunteers.Address = dr["Address"].ToString();
                    objVolunteers.HoursSpent = dr["HoursSpent"].ToString();
                    objVolunteers.Comments = dr["Comments"].ToString();
                    objVolunteers.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    objVolunteers.UpdatedBy = dr["UpdatedBy"].ToString();
                    objVolunteers.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                    objVolunteers.InsertedBy = dr["InsertedBy"].ToString();
                    objVolunteers.EventName = (dt.Rows[0]["EventName"] != DBNull.Value ? dt.Rows[0]["EventName"].ToString() : null);
                    lstVolunteers.Add(objVolunteers);
                }

            }
            return lstVolunteers;
        }


        public List<AWS_AssociationToolCore.Entities.Volunteers> FEGetVolunteersMemberIdList(Int64 MemberId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Volunteers> lstVolunteers = new List<AWS_AssociationToolCore.Entities.Volunteers>();
            DataTable dt = _Volunteers.GetVolunteersMemberIdList(MemberId,ref status);
            if (dt.Rows.Count == 0)
            {
                dt = _Volunteers.GetVolunteersMemberIdList(MemberId, ref status);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Volunteers objVolunteers = new AWS_AssociationToolCore.Entities.Volunteers();

                    objVolunteers.VolunteerId = Convert.ToInt64(dr["VolunteerId"].ToString());
                    objVolunteers.FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null);
                    objVolunteers.LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null);
                    objVolunteers.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
                    objVolunteers.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objVolunteers.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objVolunteers.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objVolunteers.UpdatedBy = dr["UpdatedBy"].ToString();
                    objVolunteers.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    objVolunteers.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objVolunteers.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    objVolunteers.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);

                    
                    lstVolunteers.Add(objVolunteers);
                }

            }
            return lstVolunteers;
        }

        #endregion
    }
}
