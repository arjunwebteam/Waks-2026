using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
    public class KidsRegistrations
    {
        AWS_AssociationToolCore.DAL.KidsRegistrations _KidsRegistrations = new AWS_AssociationToolCore.DAL.KidsRegistrations();

        public Int64 FEKidsRegistrationsInsert(Entities.KidsRegistrations objKidsRegistrations, ref Int64 KidsRegistrationsId)
        {
            Int64 _status = 0;
            if (objKidsRegistrations != null)
            {
                _status = _KidsRegistrations.FEKidsRegistrationsInsert(objKidsRegistrations, ref KidsRegistrationsId);

            }
            return _status;
        }

        public Int64 UpdateKidsRegistrations(Entities.KidsRegistrations objKidsRegistrations)
        {
            Int64 _status = 0;
            if (objKidsRegistrations != null)
            {
                _status = _KidsRegistrations.UpdateKidsRegistrations(objKidsRegistrations);

            }
            return _status;
        }

        public Entities.KidsRegistrations GetKidsRegistrationsGetById(Int64 KidsRegistrationsId, ref int status)
        {
            DataSet ds = _KidsRegistrations.GetKidsRegistrationsGetById(KidsRegistrationsId, ref status);
            Entities.KidsRegistrations objServices = new Entities.KidsRegistrations();
            List<Entities.KidsParticipants> lstKidsParticipants = new List<Entities.KidsParticipants>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objServices.KidsRegistrationsId = Convert.ToInt64(ds.Tables[0].Rows[0]["KidsRegistrationsId"]);                
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

                objServices.PaymentStatus = (ds.Tables[0].Rows[0]["PaymentStatus"] != DBNull.Value ? ds.Tables[0].Rows[0]["PaymentStatus"].ToString() : null);
            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.KidsParticipants objKidsParticipants = new Entities.KidsParticipants();

                    objKidsParticipants.KidsParticipantId = Convert.ToInt64(dr["KidsParticipantId"]);
                    objKidsParticipants.EventId = Convert.ToInt64(dr["EventId"]);
                    objKidsParticipants.FirstName = dr["FirstName"].ToString();
                    objKidsParticipants.LastName = dr["LastName"].ToString();
                    objKidsParticipants.Email = dr["Email"].ToString();
                    objKidsParticipants.Age = (dr["Age"] != DBNull.Value ? dr["Age"].ToString() : null);
                    objKidsParticipants.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objKidsParticipants.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objKidsParticipants.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstKidsParticipants.Add(objKidsParticipants);
                }

            }

            objServices.lstKidsParticipants = lstKidsParticipants;

            return objServices;
        }

        public List<AWS_AssociationToolCore.Entities.KidsRegistrations> GetKidsRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.KidsRegistrations> lstKidsRegistrations = new List<AWS_AssociationToolCore.Entities.KidsRegistrations>();
            DataTable dt = _KidsRegistrations.GetKidsRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _KidsRegistrations.GetKidsRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.KidsRegistrations objKidsRegistrations = new AWS_AssociationToolCore.Entities.KidsRegistrations();

                    objKidsRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objKidsRegistrations.KidsRegistrationsId = Convert.ToInt64(dr["KidsRegistrationsId"].ToString());
                    objKidsRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);                    
                    objKidsRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objKidsRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objKidsRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objKidsRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objKidsRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objKidsRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objKidsRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objKidsRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objKidsRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objKidsRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objKidsRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objKidsRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objKidsRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objKidsRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objKidsRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objKidsRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);
                    
                    lstKidsRegistrations.Add(objKidsRegistrations);
                }
            }
            return lstKidsRegistrations;
        }

        public Int64 DeleteKidsRegistrations(Int64 KidsRegistrationsId)
        {
            Int64 _status = 0;
            if (KidsRegistrationsId != 0)
            {
                _status = _KidsRegistrations.DeleteKidsRegistrations(KidsRegistrationsId);
            }
            return _status;
        }


        #region FE

        public List<AWS_AssociationToolCore.Entities.KidsRegistrations> FEGetKidsRegistrationsList(string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.KidsRegistrations> lstKidsRegistrations = new List<AWS_AssociationToolCore.Entities.KidsRegistrations>();
            DataTable dt = _KidsRegistrations.FEGetKidsRegistrationsList(Email, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _KidsRegistrations.FEGetKidsRegistrationsList(Email, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.KidsRegistrations objKidsRegistrations = new AWS_AssociationToolCore.Entities.KidsRegistrations();

                    objKidsRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objKidsRegistrations.KidsRegistrationsId = Convert.ToInt64(dr["KidsRegistrationsId"].ToString());
                    objKidsRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objKidsRegistrations.RegestrationID = (dr["RegestrationID"] != DBNull.Value ? (dr["RegestrationID"].ToString()) : null);
                    objKidsRegistrations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objKidsRegistrations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objKidsRegistrations.EmailId = (dr["EmailId"] != DBNull.Value ? (dr["EmailId"].ToString()) : null);
                    objKidsRegistrations.DOB = (dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"].ToString()) : DateTime.MinValue);
                    objKidsRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objKidsRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);


                    objKidsRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objKidsRegistrations.PaymentStatus = dr["PaymentStatus"].ToString();
                    objKidsRegistrations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objKidsRegistrations.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0);
                    objKidsRegistrations.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objKidsRegistrations.RegistrationCategoryId = (dr["RegistrationCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["RegistrationCategoryId"]) : 0);
                    objKidsRegistrations.EventName = (dr["EventName"] != DBNull.Value ? (dr["EventName"].ToString()) : null);
                    objKidsRegistrations.CategoryName = (dr["CategoryName"] != DBNull.Value ? (dr["CategoryName"].ToString()) : null);
                    objKidsRegistrations.CompetitionCategory1 = (dr["CompetitionCategory1"] != DBNull.Value ? (dr["CompetitionCategory1"].ToString()) : null);

                    lstKidsRegistrations.Add(objKidsRegistrations);
                }
            }
            return lstKidsRegistrations;
        }

        #endregion

        #region KidsParticipants

        public Int64 InsertBalalaParticipant(Entities.KidsParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _KidsRegistrations.InsertBalalaParticipant(objKidsParticipants);
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(Entities.KidsParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            if (objKidsParticipants != null)
            {
                _status = _KidsRegistrations.UpdateEventParticipant(objKidsParticipants);
            }
            return _status;
        }

        #endregion
    }
}
