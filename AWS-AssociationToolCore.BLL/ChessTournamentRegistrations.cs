using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
    public class ChessTournamentRegistrations
    {
        AWS_AssociationToolCore.DAL.ChessTournamentRegistrations _ChessTournamentRegistrations = new AWS_AssociationToolCore.DAL.ChessTournamentRegistrations();

        public Int64 FEChessTournamentRegistrationsInsert(Entities.ChessTournamentRegistrations objChessTournamentRegistrations, ref Int64 ChessTournamentRegistrationId)
        {
            Int64 _status = 0;
            if (objChessTournamentRegistrations != null)
            {
                _status = _ChessTournamentRegistrations.FEChessTournamentRegistrationsInsert(objChessTournamentRegistrations, ref ChessTournamentRegistrationId);

            }
            return _status;
        }

        public Int64 UpdateChessTournamentRegistrations(Entities.ChessTournamentRegistrations objChessTournamentRegistrations)
        {
            Int64 _status = 0;
            if (objChessTournamentRegistrations != null)
            {
                _status = _ChessTournamentRegistrations.UpdateChessTournamentRegistrations(objChessTournamentRegistrations);

            }
            return _status;
        }

        public Entities.ChessTournamentRegistrations GetChessTournamentRegistrationsGetById(Int64 ChessTournamentRegistrationId, ref int status)
        {
            DataSet ds = _ChessTournamentRegistrations.GetChessTournamentRegistrationsGetById(ChessTournamentRegistrationId, ref status);
            Entities.ChessTournamentRegistrations objServices = new Entities.ChessTournamentRegistrations();
            List<Entities.ChessTournamentParticipants> lstChessTournamentParticipants = new List<Entities.ChessTournamentParticipants>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objServices.ChessTournamentRegistrationId = Convert.ToInt64(ds.Tables[0].Rows[0]["ChessTournamentRegistrationId"]);
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
                    Entities.ChessTournamentParticipants objKidsParticipants = new Entities.ChessTournamentParticipants();

                    objKidsParticipants.ChessTournamentParticipantId = Convert.ToInt64(dr["ChessTournamentParticipantId"]);
                    objKidsParticipants.EventId = Convert.ToInt64(dr["EventId"]);
                    objKidsParticipants.FirstName = dr["FirstName"].ToString();
                    objKidsParticipants.LastName = dr["LastName"].ToString();
                    objKidsParticipants.Email = dr["Email"].ToString();
                    objKidsParticipants.Age = (dr["Age"] != DBNull.Value ? dr["Age"].ToString() : null);
                    objKidsParticipants.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objKidsParticipants.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objKidsParticipants.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstChessTournamentParticipants.Add(objKidsParticipants);
                }

            }

            objServices.lstChessTournamentParticipants = lstChessTournamentParticipants;

            return objServices;
        }

        public List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations> GetChessTournamentRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations> lstChessTournamentRegistrations = new List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations>();
            DataTable dt = _ChessTournamentRegistrations.GetChessTournamentRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ChessTournamentRegistrations.GetChessTournamentRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ChessTournamentRegistrations objChessTournamentRegistrations = new AWS_AssociationToolCore.Entities.ChessTournamentRegistrations();

                    objChessTournamentRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objChessTournamentRegistrations.ChessTournamentRegistrationId = Convert.ToInt64(dr["ChessTournamentRegistrationId"].ToString());
                    objChessTournamentRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objChessTournamentRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objChessTournamentRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objChessTournamentRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objChessTournamentRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objChessTournamentRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objChessTournamentRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objChessTournamentRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objChessTournamentRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objChessTournamentRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objChessTournamentRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objChessTournamentRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objChessTournamentRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objChessTournamentRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objChessTournamentRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objChessTournamentRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objChessTournamentRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);

                    lstChessTournamentRegistrations.Add(objChessTournamentRegistrations);
                }
            }
            return lstChessTournamentRegistrations;
        }

        public Int64 DeleteChessTournamentRegistrations(Int64 ChessTournamentRegistrationId)
        {
            Int64 _status = 0;
            if (ChessTournamentRegistrationId != 0)
            {
                _status = _ChessTournamentRegistrations.DeleteChessTournamentRegistrations(ChessTournamentRegistrationId);
            }
            return _status;
        }


        #region FE

        public List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations> FEGetChessTournamentList(string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations> lstChessTournamentRegistrations = new List<AWS_AssociationToolCore.Entities.ChessTournamentRegistrations>();
            DataTable dt = _ChessTournamentRegistrations.FEGetChessTournamentList(Email, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ChessTournamentRegistrations.FEGetChessTournamentList(Email, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ChessTournamentRegistrations objChessTournamentRegistrations = new AWS_AssociationToolCore.Entities.ChessTournamentRegistrations();

                    objChessTournamentRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objChessTournamentRegistrations.ChessTournamentRegistrationId = Convert.ToInt64(dr["ChessTournamentRegistrationId"].ToString());
                    objChessTournamentRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objChessTournamentRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objChessTournamentRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objChessTournamentRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objChessTournamentRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objChessTournamentRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objChessTournamentRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objChessTournamentRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objChessTournamentRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objChessTournamentRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objChessTournamentRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objChessTournamentRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objChessTournamentRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objChessTournamentRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objChessTournamentRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objChessTournamentRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objChessTournamentRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);

                    lstChessTournamentRegistrations.Add(objChessTournamentRegistrations);
                }
            }
            return lstChessTournamentRegistrations;
        }

        public List<Entities.ChessTournamentRegistrations> GetTeluguAmmaiDistrictList(ref int status)
        {
            DataTable dt = _ChessTournamentRegistrations.GetTeluguAmmaiDistrictList(ref status);
            List<Entities.ChessTournamentRegistrations> lstDistrict = new List<Entities.ChessTournamentRegistrations>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ChessTournamentRegistrations objdistrict = new Entities.ChessTournamentRegistrations();


                    objdistrict.District = (dr["District"] != DBNull.Value ? dr["District"].ToString() : null);

                    lstDistrict.Add(objdistrict);
                }
            }

            return lstDistrict;
        }

        public List<Entities.Districts> GetCountryList(Int64 eid, ref int status)
        {
            DataTable dt = _ChessTournamentRegistrations.GetDistrictList(eid, ref status);
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

        public Int64 InsertChessTournamentParticipants(Entities.ChessTournamentParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _ChessTournamentRegistrations.InsertChessTournamentParticipants(objKidsParticipants);
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(Entities.ChessTournamentParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _ChessTournamentRegistrations.UpdateEventParticipant(objKidsParticipants);
            }
            return _status;
        }

        #endregion
    }
}
