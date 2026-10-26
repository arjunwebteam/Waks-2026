using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
    public class TeluguAmmaiRegistrations
    {
        AWS_AssociationToolCore.DAL.TeluguAmmaiRegistrations _TeluguAmmaiRegistrations = new AWS_AssociationToolCore.DAL.TeluguAmmaiRegistrations();

        public Int64 FETeluguAmmaiRegistrationsInsert(Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations, ref Int64 TeluguAmmaiRegistrationId)
        {
            Int64 _status = 0;
            if (objTeluguAmmaiRegistrations != null)
            {
                _status = _TeluguAmmaiRegistrations.FETeluguAmmaiRegistrationsInsert(objTeluguAmmaiRegistrations, ref TeluguAmmaiRegistrationId);

            }
            return _status;
        }

        public Int64 UpdateTeluguAmmaiRegistrations(Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations)
        {
            Int64 _status = 0;
            if (objTeluguAmmaiRegistrations != null)
            {
                _status = _TeluguAmmaiRegistrations.UpdateTeluguAmmaiRegistrations(objTeluguAmmaiRegistrations);

            }
            return _status;
        }

        public Entities.TeluguAmmaiRegistrations GetTeluguAmmaiRegistrationsGetById(Int64 TeluguAmmaiRegistrationId, ref int status)
        {
            DataSet ds = _TeluguAmmaiRegistrations.GetTeluguAmmaiRegistrationsGetById(TeluguAmmaiRegistrationId, ref status);
            Entities.TeluguAmmaiRegistrations objServices = new Entities.TeluguAmmaiRegistrations();
            List<Entities.TeluguAmmaiParticipants> lstTeluguAmmaiParticipants = new List<Entities.TeluguAmmaiParticipants>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objServices.TeluguAmmaiRegistrationId = Convert.ToInt64(ds.Tables[0].Rows[0]["TeluguAmmaiRegistrationId"]);
                objServices.ChapterId = (ds.Tables[0].Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["ChapterId"]) : 0);
                objServices.RegestrationID = (ds.Tables[0].Rows[0]["RegestrationID"] != DBNull.Value ? ds.Tables[0].Rows[0]["RegestrationID"].ToString() : null);
                objServices.FirstName = (ds.Tables[0].Rows[0]["FirstName"] != DBNull.Value ? ds.Tables[0].Rows[0]["FirstName"].ToString() : null);
                objServices.LastName = (ds.Tables[0].Rows[0]["LastName"] != DBNull.Value ? ds.Tables[0].Rows[0]["LastName"].ToString() : null);
                objServices.EmailId = (ds.Tables[0].Rows[0]["EmailId"] != DBNull.Value ? ds.Tables[0].Rows[0]["EmailId"].ToString() : null);
                objServices.DOB = (ds.Tables[0].Rows[0]["DOB"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["DOB"].ToString()) : DateTime.MinValue);
                objServices.ParentFirstName = (ds.Tables[0].Rows[0]["ParentFirstName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ParentFirstName"].ToString() : null);
                objServices.ParentLastName = (ds.Tables[0].Rows[0]["ParentLastName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ParentLastName"].ToString() : null);
                objServices.ParentRelation = (ds.Tables[0].Rows[0]["ParentRelation"] != DBNull.Value ? ds.Tables[0].Rows[0]["ParentRelation"].ToString() : null);
                objServices.ParentEmail = (ds.Tables[0].Rows[0]["ParentEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ParentEmail"].ToString() : null);
                objServices.SchoolGrade = (ds.Tables[0].Rows[0]["SchoolGrade"] != DBNull.Value ? ds.Tables[0].Rows[0]["SchoolGrade"].ToString() : null);
                objServices.Gender = (ds.Tables[0].Rows[0]["Gender"] != DBNull.Value ? ds.Tables[0].Rows[0]["Gender"].ToString() : null);
                objServices.PrimaryPhoneNumber = (ds.Tables[0].Rows[0]["PrimaryPhoneNumber"] != DBNull.Value ? ds.Tables[0].Rows[0]["PrimaryPhoneNumber"].ToString() : null);
                objServices.AlternativePhoneNumber = (ds.Tables[0].Rows[0]["AlternativePhoneNumber"] != DBNull.Value ? ds.Tables[0].Rows[0]["AlternativePhoneNumber"].ToString() : null);
                objServices.Address1 = (ds.Tables[0].Rows[0]["Address1"] != DBNull.Value ? ds.Tables[0].Rows[0]["Address1"].ToString() : null);
                objServices.Address2 = (ds.Tables[0].Rows[0]["Address2"] != DBNull.Value ? ds.Tables[0].Rows[0]["Address2"].ToString() : null);
                objServices.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objServices.State = (ds.Tables[0].Rows[0]["State"] != DBNull.Value ? ds.Tables[0].Rows[0]["State"].ToString() : null);
                objServices.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objServices.CompetitionCategory1 = (ds.Tables[0].Rows[0]["CompetitionCategory1"] != DBNull.Value ? ds.Tables[0].Rows[0]["CompetitionCategory1"].ToString() : null);
                objServices.CompetitionCategory2 = (ds.Tables[0].Rows[0]["CompetitionCategory2"] != DBNull.Value ? ds.Tables[0].Rows[0]["CompetitionCategory2"].ToString() : null);
                objServices.TotalAmount = (ds.Tables[0].Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["TotalAmount"].ToString()) : 0);
                objServices.IsActive = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"].ToString());
                objServices.TransactionId = (ds.Tables[0].Rows[0]["TransactionId"] != DBNull.Value ? ds.Tables[0].Rows[0]["TransactionId"].ToString() : null);
                objServices.PaymentStatusId = (ds.Tables[0].Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["PaymentStatusId"].ToString()) : 0);
                objServices.PaymentMethodId = (ds.Tables[0].Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["PaymentMethodId"].ToString()) : 0);
                objServices.OrderDate = (ds.Tables[0].Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["OrderDate"].ToString()) : DateTime.MinValue);
                objServices.PaymentBy = (ds.Tables[0].Rows[0]["PaymentBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["PaymentBy"].ToString() : null);

                objServices.InsertedBy = ds.Tables[0].Rows[0]["InsertedBy"].ToString();
                objServices.InsertedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["InsertedDate"]);

                objServices.UpdatedBy = ds.Tables[0].Rows[0]["UpdatedBy"].ToString();
                objServices.UpdatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedDate"]);
                objServices.Age = (ds.Tables[0].Rows[0]["Age"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["Age"]) : 0);
                objServices.MaritalStatu = (ds.Tables[0].Rows[0]["MaritalStatu"] != DBNull.Value ? ds.Tables[0].Rows[0]["MaritalStatu"].ToString() : null);
                objServices.District = (ds.Tables[0].Rows[0]["District"] != DBNull.Value ? ds.Tables[0].Rows[0]["District"].ToString() : null);

                objServices.PaymentStatus = (ds.Tables[0].Rows[0]["PaymentStatus"] != DBNull.Value ? ds.Tables[0].Rows[0]["PaymentStatus"].ToString() : null);
            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.TeluguAmmaiParticipants objKidsParticipants = new Entities.TeluguAmmaiParticipants();

                    objKidsParticipants.TeluguAmmaiParticipantId = Convert.ToInt64(dr["TeluguAmmaiParticipantId"]);
                    objKidsParticipants.EventId = Convert.ToInt64(dr["EventId"]);
                    objKidsParticipants.FirstName = dr["FirstName"].ToString();
                    objKidsParticipants.LastName = dr["LastName"].ToString();
                    objKidsParticipants.Email = dr["Email"].ToString();
                    objKidsParticipants.Age = (dr["Age"] != DBNull.Value ? dr["Age"].ToString() : null);
                    objKidsParticipants.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objKidsParticipants.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objKidsParticipants.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstTeluguAmmaiParticipants.Add(objKidsParticipants);
                }

            }

            objServices.lstTeluguAmmaiParticipants = lstTeluguAmmaiParticipants;

            return objServices;
        }

        public List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations> GetTeluguAmmaiRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations> lstTeluguAmmaiRegistrations = new List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations>();
            DataTable dt = _TeluguAmmaiRegistrations.GetTeluguAmmaiRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _TeluguAmmaiRegistrations.GetTeluguAmmaiRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations = new AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations();

                    objTeluguAmmaiRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objTeluguAmmaiRegistrations.TeluguAmmaiRegistrationId = Convert.ToInt64(dr["TeluguAmmaiRegistrationId"].ToString());
                    objTeluguAmmaiRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objTeluguAmmaiRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objTeluguAmmaiRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objTeluguAmmaiRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objTeluguAmmaiRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objTeluguAmmaiRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objTeluguAmmaiRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objTeluguAmmaiRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objTeluguAmmaiRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objTeluguAmmaiRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objTeluguAmmaiRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objTeluguAmmaiRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objTeluguAmmaiRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);

                    lstTeluguAmmaiRegistrations.Add(objTeluguAmmaiRegistrations);
                }
            }
            return lstTeluguAmmaiRegistrations;
        }

        public Int64 DeleteTeluguAmmaiRegistrations(Int64 TeluguAmmaiRegistrationId)
        {
            Int64 _status = 0;
            if (TeluguAmmaiRegistrationId != 0)
            {
                _status = _TeluguAmmaiRegistrations.DeleteTeluguAmmaiRegistrations(TeluguAmmaiRegistrationId);
            }
            return _status;
        }


        #region FE

        public List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations> FEGetKidsRegistrationsList(string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations> lstTeluguAmmaiRegistrations = new List<AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations>();
            DataTable dt = _TeluguAmmaiRegistrations.FEGetKidsRegistrationsList(Email, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _TeluguAmmaiRegistrations.FEGetKidsRegistrationsList(Email, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations = new AWS_AssociationToolCore.Entities.TeluguAmmaiRegistrations();

                    objTeluguAmmaiRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objTeluguAmmaiRegistrations.TeluguAmmaiRegistrationId = Convert.ToInt64(dr["TeluguAmmaiRegistrationId"].ToString());
                    objTeluguAmmaiRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objTeluguAmmaiRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objTeluguAmmaiRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objTeluguAmmaiRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objTeluguAmmaiRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objTeluguAmmaiRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objTeluguAmmaiRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objTeluguAmmaiRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objTeluguAmmaiRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objTeluguAmmaiRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objTeluguAmmaiRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objTeluguAmmaiRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objTeluguAmmaiRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objTeluguAmmaiRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);

                    lstTeluguAmmaiRegistrations.Add(objTeluguAmmaiRegistrations);
                }
            }
            return lstTeluguAmmaiRegistrations;
        }

        public List<Entities.TeluguAmmaiRegistrations> GetTeluguAmmaiDistrictList(ref int status)
        {
            DataTable dt = _TeluguAmmaiRegistrations.GetTeluguAmmaiDistrictList(ref status);
            List<Entities.TeluguAmmaiRegistrations> lstDistrict = new List<Entities.TeluguAmmaiRegistrations>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.TeluguAmmaiRegistrations objdistrict = new Entities.TeluguAmmaiRegistrations();

                    
                    objdistrict.District = (dr["District"] != DBNull.Value ? dr["District"].ToString() : null);
                  
                    lstDistrict.Add(objdistrict);
                }
            }

            return lstDistrict;
        }

        public List<Entities.Districts> GetCountryList(Int64 eid, ref int status)
        {
            DataTable dt = _TeluguAmmaiRegistrations.GetDistrictList(eid, ref status);
            List<Entities.Districts> lstDistricts = new List<Entities.Districts>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Districts objdistrict = new Entities.Districts();


                    objdistrict.Name = (dr["Name"] != DBNull.Value ? dr["Name"].ToString() : null);

                    lstDistricts.Add(objdistrict);
                }
            }

            return lstDistricts;
        }

        #endregion

        #region KidsParticipants

        public Int64 InsertTeluguAmmaiParticipant(Entities.TeluguAmmaiParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _TeluguAmmaiRegistrations.InsertTeluguAmmaiParticipant(objKidsParticipants);
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(Entities.TeluguAmmaiParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _TeluguAmmaiRegistrations.UpdateEventParticipant(objKidsParticipants);
            }
            return _status;
        }

        #endregion

    }
}
