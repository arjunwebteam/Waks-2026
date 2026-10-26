using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
    public class CulturalRegistrations
    {
        AWS_AssociationToolCore.DAL.CulturalRegistrations _CulturalRegistrations = new AWS_AssociationToolCore.DAL.CulturalRegistrations();

        public Int64 InsertCulturalRegistrations(Entities.CulturalRegistrations objCulturalRegistrations, ref Int64 CulturalRegistrationId, ref string receipturl)
        {
            Int64 _status = 0;
            if (objCulturalRegistrations != null)
            {
                _status = _CulturalRegistrations.InsertCulturalRegistrations(objCulturalRegistrations, ref CulturalRegistrationId,ref receipturl);

            }
            return _status;
        }

        public Int64 DeleteCulturalRegistrations(Int64 CulturalRegistrationId)
        {
            Int64 _status = 0;
            _status = _CulturalRegistrations.DeleteCulturalRegistrations(CulturalRegistrationId);
            return _status;
        }

        public AWS_AssociationToolCore.Entities.CulturalRegistrations GetCulturalRegistrationsById(Int64 CulturalRegistrationId, ref int status)
        {
            AWS_AssociationToolCore.Entities.CulturalRegistrations objCulturalRegistrations = new AWS_AssociationToolCore.Entities.CulturalRegistrations();
            DataTable dt = new DataTable();
            if (CulturalRegistrationId != 0)
            {
                dt = _CulturalRegistrations.GetCulturalRegistrationsById(CulturalRegistrationId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objCulturalRegistrations.CulturalRegistrationId = Convert.ToInt64(dt.Rows[0]["CulturalRegistrationId"].ToString());
                    objCulturalRegistrations.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objCulturalRegistrations.LastName = dt.Rows[0]["LastName"].ToString();
                    objCulturalRegistrations.Address1 = (dt.Rows[0]["Address1"] != DBNull.Value ? dt.Rows[0]["Address1"].ToString() : null);
                    objCulturalRegistrations.Address2 = (dt.Rows[0]["Address2"] != DBNull.Value ? dt.Rows[0]["Address2"].ToString() : null);
                    objCulturalRegistrations.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                    objCulturalRegistrations.ZipCode = (dt.Rows[0]["ZipCode"] != DBNull.Value ? dt.Rows[0]["ZipCode"].ToString() : null);
                    objCulturalRegistrations.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : null);
                    objCulturalRegistrations.PhoneNo = (dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null);
                    objCulturalRegistrations.AAALifetimeMember = (dt.Rows[0]["AAALifetimeMember"] != DBNull.Value ? dt.Rows[0]["AAALifetimeMember"].ToString() : null);
                    objCulturalRegistrations.CoordinatorName = (dt.Rows[0]["CoordinatorName"] != DBNull.Value ? dt.Rows[0]["CoordinatorName"].ToString() : null);
                    objCulturalRegistrations.ContactPhone = (dt.Rows[0]["ContactPhone"] != DBNull.Value ? dt.Rows[0]["ContactPhone"].ToString() : null);
                    objCulturalRegistrations.ContactEmail = (dt.Rows[0]["ContactEmail"] != DBNull.Value ? dt.Rows[0]["ContactEmail"].ToString() : null);
                    objCulturalRegistrations.OrganizationName = (dt.Rows[0]["OrganizationName"] != DBNull.Value ? dt.Rows[0]["OrganizationName"].ToString() : null);
                    objCulturalRegistrations.PerformanceCategory = (dt.Rows[0]["PerformanceCategory"] != DBNull.Value ? dt.Rows[0]["PerformanceCategory"].ToString() : null);
                    objCulturalRegistrations.SpecifyCategory = (dt.Rows[0]["SpecifyCategory"] != DBNull.Value ? dt.Rows[0]["SpecifyCategory"].ToString() : null);
                    objCulturalRegistrations.NameofthePerformance = (dt.Rows[0]["NameofthePerformance"] != DBNull.Value ? dt.Rows[0]["NameofthePerformance"].ToString() : null);
                    objCulturalRegistrations.PerformanceDuration = (dt.Rows[0]["PerformanceDuration"] != DBNull.Value ? dt.Rows[0]["PerformanceDuration"].ToString() : null);
                    objCulturalRegistrations.NumberofParticipants = (dt.Rows[0]["NumberofParticipants"] != DBNull.Value ? dt.Rows[0]["NumberofParticipants"].ToString() : null);
                    objCulturalRegistrations.ParticipantAgeGroup = (dt.Rows[0]["ParticipantAgeGroup"] != DBNull.Value ? dt.Rows[0]["ParticipantAgeGroup"].ToString() : null);
                    objCulturalRegistrations.SpecialRequirements = (dt.Rows[0]["SpecialRequirements"] != DBNull.Value ? dt.Rows[0]["SpecialRequirements"].ToString() : null);
                    objCulturalRegistrations.BriefDescriptionofthePerformance = (dt.Rows[0]["BriefDescriptionofthePerformance"] != DBNull.Value ? dt.Rows[0]["BriefDescriptionofthePerformance"].ToString() : null);
                    objCulturalRegistrations.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                    objCulturalRegistrations.PaymentDate = (dt.Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["PaymentDate"]) : DateTime.MinValue);
                    objCulturalRegistrations.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                    objCulturalRegistrations.OtherPaymentId = (dt.Rows[0]["OtherPaymentId"] != DBNull.Value ? dt.Rows[0]["OtherPaymentId"].ToString() : null);
                    objCulturalRegistrations.TransactionId = (dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null);
                    objCulturalRegistrations.AmountPaid = (dt.Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountPaid"]) : 0);
                    objCulturalRegistrations.Zelle = (dt.Rows[0]["Zelle"] != DBNull.Value ? dt.Rows[0]["Zelle"].ToString() : null);
                    objCulturalRegistrations.ReceiptUrl = (dt.Rows[0]["ReceiptUrl"] != DBNull.Value ? dt.Rows[0]["ReceiptUrl"].ToString() : null);
                    objCulturalRegistrations.ZellePartnerName = (dt.Rows[0]["ZellePartnerName"] != DBNull.Value ? dt.Rows[0]["ZellePartnerName"].ToString() : null);
                    objCulturalRegistrations.ChequeHolderName = (dt.Rows[0]["ChequeHolderName"] != DBNull.Value ? dt.Rows[0]["ChequeHolderName"].ToString() : null);
                    objCulturalRegistrations.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objCulturalRegistrations.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                    objCulturalRegistrations.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    objCulturalRegistrations.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                    objCulturalRegistrations.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    objCulturalRegistrations.AAAInstagram = (dt.Rows[0]["AAAInstagram"] != DBNull.Value ? dt.Rows[0]["AAAInstagram"].ToString() : null);
                    objCulturalRegistrations.EventName = (dt.Rows[0]["EventName"] != DBNull.Value ? dt.Rows[0]["EventName"].ToString() : null);
                    objCulturalRegistrations.PaymentStatus = (dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null);
                    objCulturalRegistrations.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);

                }

            }
            return objCulturalRegistrations;
        }


        public List<AWS_AssociationToolCore.Entities.CulturalRegistrations> GetCulturalRegistrationsListByVariable( string State, Int64 EventId, string Search, string Sort, int PageNo, int Items,Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CulturalRegistrations> lstCulturalRegistrations = new List<AWS_AssociationToolCore.Entities.CulturalRegistrations>();

            DataTable dt = _CulturalRegistrations.GetCulturalRegistrationsListByVariable(State, EventId,Search, Sort, PageNo, Items, ChapterId, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CulturalRegistrations.GetCulturalRegistrationsListByVariable(State, EventId,Search, Sort, PageNo - 1, Items, ChapterId,ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CulturalRegistrations objCulturalRegistrations = new AWS_AssociationToolCore.Entities.CulturalRegistrations();

                    objCulturalRegistrations.RId = Convert.ToInt32(dr["RId"].ToString());
                    objCulturalRegistrations.CulturalRegistrationId = Convert.ToInt32(dr["CulturalRegistrationId"].ToString());
                    objCulturalRegistrations.FirstName = dr["FirstName"].ToString();
                    objCulturalRegistrations.LastName = dr["LastName"].ToString();
                    objCulturalRegistrations.Address1 = (dr["Address1"] != DBNull.Value ? dr["Address1"].ToString() : null);
                    objCulturalRegistrations.Address2 = (dr["Address2"] != DBNull.Value ? dr["Address2"].ToString() : null);
                    objCulturalRegistrations.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objCulturalRegistrations.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objCulturalRegistrations.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objCulturalRegistrations.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCulturalRegistrations.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
                    objCulturalRegistrations.AAALifetimeMember = (dr["AAALifetimeMember"] != DBNull.Value ? dr["AAALifetimeMember"].ToString() : null);
                    objCulturalRegistrations.CoordinatorName = (dr["CoordinatorName"] != DBNull.Value ? dr["CoordinatorName"].ToString() : null);
                    objCulturalRegistrations.ContactPhone = (dr["ContactPhone"] != DBNull.Value ? dr["ContactPhone"].ToString() : null);
                    objCulturalRegistrations.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objCulturalRegistrations.OrganizationName = (dr["OrganizationName"] != DBNull.Value ? dr["OrganizationName"].ToString() : null);
                    objCulturalRegistrations.PerformanceCategory = (dr["PerformanceCategory"] != DBNull.Value ? dr["PerformanceCategory"].ToString() : null);
                    objCulturalRegistrations.SpecifyCategory = (dr["SpecifyCategory"] != DBNull.Value ? dr["SpecifyCategory"].ToString() : null);
                    objCulturalRegistrations.NameofthePerformance = (dr["NameofthePerformance"] != DBNull.Value ? dr["NameofthePerformance"].ToString() : null);
                    objCulturalRegistrations.PerformanceDuration = (dr["PerformanceDuration"] != DBNull.Value ? dr["PerformanceDuration"].ToString() : null);
                    objCulturalRegistrations.NumberofParticipants = (dr["NumberofParticipants"] != DBNull.Value ? dr["NumberofParticipants"].ToString() : null);
                    objCulturalRegistrations.ParticipantAgeGroup = (dr["ParticipantAgeGroup"] != DBNull.Value ? dr["ParticipantAgeGroup"].ToString() : null);
                    objCulturalRegistrations.SpecialRequirements = (dr["SpecialRequirements"] != DBNull.Value ? dr["SpecialRequirements"].ToString() : null);
                    objCulturalRegistrations.BriefDescriptionofthePerformance = (dr["BriefDescriptionofthePerformance"] != DBNull.Value ? dr["BriefDescriptionofthePerformance"].ToString() : null);
                    objCulturalRegistrations.PaymentStatusId = (dr["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentStatusId"]) : 0);
                    objCulturalRegistrations.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);
                    objCulturalRegistrations.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objCulturalRegistrations.PaymentDate = (dr["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaymentDate"]) : DateTime.MinValue);
                    objCulturalRegistrations.PaymentMethodId = (dr["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentMethodId"]) : 0);
                    objCulturalRegistrations.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objCulturalRegistrations.OtherPaymentId = (dr["OtherPaymentId"] != DBNull.Value ? dr["OtherPaymentId"].ToString() : null);
                    objCulturalRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null);
                    objCulturalRegistrations.AmountPaid = (dr["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dr["AmountPaid"]) : 0);
                    objCulturalRegistrations.Zelle = (dr["Zelle"] != DBNull.Value ? dr["Zelle"].ToString() : null);
                    objCulturalRegistrations.ReceiptUrl = (dr["ReceiptUrl"] != DBNull.Value ? dr["ReceiptUrl"].ToString() : null);
                    objCulturalRegistrations.ZellePartnerName = (dr["ZellePartnerName"] != DBNull.Value ? dr["ZellePartnerName"].ToString() : null);
                    objCulturalRegistrations.ChequeHolderName = (dr["ChequeHolderName"] != DBNull.Value ? dr["ChequeHolderName"].ToString() : null);
                    objCulturalRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objCulturalRegistrations.InsertedBy = dr["InsertedBy"].ToString();
                    objCulturalRegistrations.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objCulturalRegistrations.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCulturalRegistrations.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    lstCulturalRegistrations.Add(objCulturalRegistrations);
                }
            }
            return lstCulturalRegistrations;
        }

        public Int64 InsertCulturalParticipants(Entities.CulturalParticipants objCulturalParticipants)
        {
            Int64 _status = 0;
            if (objCulturalParticipants != null)
            {
                _status = _CulturalRegistrations.InsertCulturalParticipants(objCulturalParticipants);
            }
            return _status;
        }

        public Int64 UpdateCulturalRegistrationsPaymentInfo(Entities.CulturalRegistrations objCulturalRegistrations)
        {
            Int64 _status = 0;
            if (objCulturalRegistrations != null)
            {
                _status = _CulturalRegistrations.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
            }
            return _status;
        }





        public Int64 UpdateCulturalRegistrations(Entities.CulturalRegistrations objCulturalRegistrations, ref string receipturl)
        {
            Int64 _status = 0;
            if (objCulturalRegistrations != null)
            {
                _status = _CulturalRegistrations.UpdateCulturalRegistrations(objCulturalRegistrations, ref receipturl);
            }
            return _status;
        }

        public Entities.CulturalRegistrations GetCulturalRegistrationsFullDetailsById(Int64 CulturalRegistrationId, ref int status)
        {
            DataSet ds = _CulturalRegistrations.GetCulturalRegistrationsFullDetailsById(CulturalRegistrationId, ref status);
            Entities.CulturalRegistrations objCulturalRegistrations = new Entities.CulturalRegistrations();
            List<Entities.CulturalParticipants> lstCulturalParticipants = new List<Entities.CulturalParticipants>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objCulturalRegistrations.CulturalRegistrationId = Convert.ToInt64(ds.Tables[0].Rows[0]["CulturalRegistrationId"].ToString());
                objCulturalRegistrations.FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
                objCulturalRegistrations.LastName = ds.Tables[0].Rows[0]["LastName"].ToString();
                objCulturalRegistrations.Address1 = (ds.Tables[0].Rows[0]["Address1"] != DBNull.Value ? ds.Tables[0].Rows[0]["Address1"].ToString() : null);
                objCulturalRegistrations.Address2 = (ds.Tables[0].Rows[0]["Address2"] != DBNull.Value ? ds.Tables[0].Rows[0]["Address2"].ToString() : null);
                objCulturalRegistrations.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objCulturalRegistrations.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objCulturalRegistrations.Email = (ds.Tables[0].Rows[0]["Email"] != DBNull.Value ? ds.Tables[0].Rows[0]["Email"].ToString() : null);
                objCulturalRegistrations.PhoneNo = (ds.Tables[0].Rows[0]["PhoneNo"] != DBNull.Value ? ds.Tables[0].Rows[0]["PhoneNo"].ToString() : null);
                objCulturalRegistrations.AAALifetimeMember = (ds.Tables[0].Rows[0]["AAALifetimeMember"] != DBNull.Value ? ds.Tables[0].Rows[0]["AAALifetimeMember"].ToString() : null);
                objCulturalRegistrations.CoordinatorName = (ds.Tables[0].Rows[0]["CoordinatorName"] != DBNull.Value ? ds.Tables[0].Rows[0]["CoordinatorName"].ToString() : null);
                objCulturalRegistrations.ContactPhone = (ds.Tables[0].Rows[0]["ContactPhone"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactPhone"].ToString() : null);
                objCulturalRegistrations.ContactEmail = (ds.Tables[0].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactEmail"].ToString() : null);
                objCulturalRegistrations.OrganizationName = (ds.Tables[0].Rows[0]["OrganizationName"] != DBNull.Value ? ds.Tables[0].Rows[0]["OrganizationName"].ToString() : null);
                objCulturalRegistrations.PerformanceCategory = (ds.Tables[0].Rows[0]["PerformanceCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["PerformanceCategory"].ToString() : null);
                objCulturalRegistrations.SpecifyCategory = (ds.Tables[0].Rows[0]["SpecifyCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["SpecifyCategory"].ToString() : null);
                objCulturalRegistrations.NameofthePerformance = (ds.Tables[0].Rows[0]["NameofthePerformance"] != DBNull.Value ? ds.Tables[0].Rows[0]["NameofthePerformance"].ToString() : null);
                objCulturalRegistrations.PerformanceDuration = (ds.Tables[0].Rows[0]["PerformanceDuration"] != DBNull.Value ? ds.Tables[0].Rows[0]["PerformanceDuration"].ToString() : null);
                objCulturalRegistrations.NumberofParticipants = (ds.Tables[0].Rows[0]["NumberofParticipants"] != DBNull.Value ? ds.Tables[0].Rows[0]["NumberofParticipants"].ToString() : null);
                objCulturalRegistrations.ParticipantAgeGroup = (ds.Tables[0].Rows[0]["ParticipantAgeGroup"] != DBNull.Value ? ds.Tables[0].Rows[0]["ParticipantAgeGroup"].ToString() : null);
                objCulturalRegistrations.SpecialRequirements = (ds.Tables[0].Rows[0]["SpecialRequirements"] != DBNull.Value ? ds.Tables[0].Rows[0]["SpecialRequirements"].ToString() : null);
                objCulturalRegistrations.BriefDescriptionofthePerformance = (ds.Tables[0].Rows[0]["BriefDescriptionofthePerformance"] != DBNull.Value ? ds.Tables[0].Rows[0]["BriefDescriptionofthePerformance"].ToString() : null);
                objCulturalRegistrations.PaymentStatusId = (ds.Tables[0].Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["PaymentStatusId"]) : 0);
                objCulturalRegistrations.PaymentDate = (ds.Tables[0].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["PaymentDate"]) : DateTime.MinValue);
                objCulturalRegistrations.PaymentMethodId = (ds.Tables[0].Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["PaymentMethodId"]) : 0);
                objCulturalRegistrations.OtherPaymentId = (ds.Tables[0].Rows[0]["OtherPaymentId"] != DBNull.Value ? ds.Tables[0].Rows[0]["OtherPaymentId"].ToString() : null);
                objCulturalRegistrations.TransactionId = (ds.Tables[0].Rows[0]["TransactionId"] != DBNull.Value ? ds.Tables[0].Rows[0]["TransactionId"].ToString() : null);
                objCulturalRegistrations.AmountPaid = (ds.Tables[0].Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["AmountPaid"]) : 0);
                objCulturalRegistrations.Zelle = (ds.Tables[0].Rows[0]["Zelle"] != DBNull.Value ? ds.Tables[0].Rows[0]["Zelle"].ToString() : null);
                objCulturalRegistrations.ReceiptUrl = (ds.Tables[0].Rows[0]["ReceiptUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["ReceiptUrl"].ToString() : null);
                objCulturalRegistrations.ZellePartnerName = (ds.Tables[0].Rows[0]["ZellePartnerName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZellePartnerName"].ToString() : null);
                objCulturalRegistrations.ChequeHolderName = (ds.Tables[0].Rows[0]["ChequeHolderName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChequeHolderName"].ToString() : null);
                objCulturalRegistrations.IsActive = (ds.Tables[0].Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]) : false);
                objCulturalRegistrations.InsertedBy = ds.Tables[0].Rows[0]["InsertedBy"].ToString();
                objCulturalRegistrations.UpdatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedDate"].ToString());
                objCulturalRegistrations.AAAInstagram = (ds.Tables[0].Rows[0]["AAAInstagram"] != DBNull.Value ? ds.Tables[0].Rows[0]["AAAInstagram"].ToString() : null);
                objCulturalRegistrations.EventName = (ds.Tables[0].Rows[0]["EventName"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventName"].ToString() : null);
                objCulturalRegistrations.PaymentStatus = (ds.Tables[0].Rows[0]["PaymentStatus"] != DBNull.Value ? ds.Tables[0].Rows[0]["PaymentStatus"].ToString() : null);
                objCulturalRegistrations.EventId = (ds.Tables[0].Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]) : 0);
                objCulturalRegistrations.State = (ds.Tables[0].Rows[0]["State"] != DBNull.Value ? ds.Tables[0].Rows[0]["State"].ToString() : null);
                objCulturalRegistrations.UpdatedBy = (ds.Tables[0].Rows[0]["UpdatedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["UpdatedBy"].ToString() : null);

            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.CulturalParticipants objCulturalParticipants = new Entities.CulturalParticipants();

                    objCulturalParticipants.ParticipantID = Convert.ToInt64(dr["ParticipantID"]);
                    objCulturalParticipants.CulturalRegistrationId = Convert.ToInt64(dr["CulturalRegistrationId"]);
                    objCulturalParticipants.FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null);
                    objCulturalParticipants.LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null);
                    objCulturalParticipants.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCulturalParticipants.Age = (dr["Age"] != DBNull.Value ? Convert.ToInt64(dr["Age"]) : 0);
                    objCulturalParticipants.PhoneNumber = (dr["PhoneNumber"] != DBNull.Value ? dr["PhoneNumber"].ToString() : null);
                    objCulturalParticipants.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);
                    objCulturalParticipants.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);

                    lstCulturalParticipants.Add(objCulturalParticipants);
                }

            }
            return objCulturalRegistrations;
        }

    }
}
