using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class ServiceDonations
    {
        AWS_AssociationToolCore.DAL.ServiceDonations _ServiceDonations = new AWS_AssociationToolCore.DAL.ServiceDonations();

        #region Methods

        public Int64 InsertServiceDonations(Entities.ServiceDonations objServiceDonations, ref Int64 ServiceDonationId, ref string receipturl)
        {
            Int64 _status = 0;
            if (objServiceDonations != null)
            {
                _status = _ServiceDonations.InsertServiceDonations(objServiceDonations, ref ServiceDonationId,ref receipturl);

            }
            return _status;
        }

        public Int64 InsertServiceDonationsByAdmin(Entities.ServiceDonations objServiceDonations, ref Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            if (objServiceDonations != null)
            {
                _status = _ServiceDonations.ServiceDonationsByAdmin(objServiceDonations, ref ServiceDonationId);

            }
            return _status;
        }

        public Int64 InsertServiceContributions(Entities.ServiceContributions objServiceContributions)
        {
            Int64 _status = 0;
            if (objServiceContributions != null)
            {
                _status = _ServiceDonations.InsertServiceContributions(objServiceContributions);
            }
            return _status;
        }

        public Int64 UpdateServiceDonations(Entities.ServiceDonations objServiceDonations)
        {
            Int64 _status = 0;
            if (objServiceDonations != null)
            {
                _status = _ServiceDonations.UpdateServiceDonations(objServiceDonations);

            }
            return _status;
        }

        public Int64 DeleteDonor(Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            _status = _ServiceDonations.DeleteDonor(ServiceDonationId);
            return _status;
        }

        public Int64 UpdateServiceDonationsStatus(Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            _status = _ServiceDonations.UpdateServiceDonationsStatus(ServiceDonationId);
            return _status;
        }

        public Int64 ServiceDonationsBulkUpload(string ShipmentsXML, ref string Summery)
        {
            Int64 _status = 0;
            _status = _ServiceDonations.ServiceDonationsBulkUpload(ShipmentsXML, ref Summery);
            return _status;
        }

        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.ServiceDonations GetServiceDonationsById(Int64 ServiceDonationId, ref int status)
        {
            DataSet ds0 = _ServiceDonations.ServiceDonationById(ServiceDonationId, ref status);

            DataTable dt = ds0.Tables[0];
            DataTable dt_ContibutionInfo = ds0.Tables[1]; 

            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            List<AWS_AssociationToolCore.Entities.ServiceContributions> lstServiceContributions = new List<AWS_AssociationToolCore.Entities.ServiceContributions>();
              
                if (dt.Rows.Count == 1)
                {
                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dt.Rows[0]["ServiceDonationId"].ToString());
                    objServiceDonations.ServiceId = (dt.Rows[0]["ServiceId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ServiceId"]) : 0);
                    objServiceDonations.MemberId = (dt.Rows[0]["MemberId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["MemberId"]) : 0);
                    objServiceDonations.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objServiceDonations.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                    objServiceDonations.DonationCauses = dt.Rows[0]["DonationCauses"] != DBNull.Value ? dt.Rows[0]["DonationCauses"].ToString() : null;
                    objServiceDonations.TransactionId = dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null;
                    objServiceDonations.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                    objServiceDonations.PaymentMethod = dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null;
                    objServiceDonations.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                    objServiceDonations.PaymentStatus = dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null;
                    objServiceDonations.OrderDate = (dt.Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["OrderDate"]) : DateTime.MinValue);
                    objServiceDonations.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value ? dt.Rows[0]["PaymentBy"].ToString() : null);
                    objServiceDonations.UpdatedBy = (dt.Rows[0]["UpdatedBy"] != DBNull.Value ? dt.Rows[0]["UpdatedBy"].ToString() : null);
                    objServiceDonations.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    objServiceDonations.FirstName = (dt.Rows[0]["FirstName"] != DBNull.Value ? dt.Rows[0]["FirstName"].ToString() : null);
                    objServiceDonations.LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString() : null);
                    objServiceDonations.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : null);
                    objServiceDonations.Field3 = (dt.Rows[0]["Field3"] != DBNull.Value ? dt.Rows[0]["Field3"].ToString() : null);
                    objServiceDonations.FrequencyofDonation = (dt.Rows[0]["FrequencyofDonation"] != DBNull.Value ? dt.Rows[0]["FrequencyofDonation"].ToString() : null);
                    objServiceDonations.NoofPayments = (dt.Rows[0]["NoofPayments"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["NoofPayments"]) : 0);
                    objServiceDonations.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : null);
                    objServiceDonations.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : null);
                    objServiceDonations.PhoneNo = (dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null);
                    objServiceDonations.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                    objServiceDonations.Country = (dt.Rows[0]["Country"] != DBNull.Value ? dt.Rows[0]["Country"].ToString() : null);
                    objServiceDonations.ZipCode = (dt.Rows[0]["ZipCode"] != DBNull.Value ? dt.Rows[0]["ZipCode"].ToString() : null);
                     objServiceDonations.ReceiptUrl = dt.Rows[0]["ReceiptUrl"] != DBNull.Value ? dt.Rows[0]["ReceiptUrl"].ToString() : null;
                    objServiceDonations.ChequeHolderName = dt.Rows[0]["ChequeHolderName"] != DBNull.Value ? dt.Rows[0]["ChequeHolderName"].ToString() : null;
                     objServiceDonations.ZellePartnerName = dt.Rows[0]["ZellePartnerName"] != DBNull.Value ? dt.Rows[0]["ZellePartnerName"].ToString() : null;
                objServiceDonations.Zelle = dt.Rows[0]["Zelle"] != DBNull.Value ? dt.Rows[0]["Zelle"].ToString() : null;


                //objServiceDonations.d = dt.Rows[0]["ServiceTitle"] != DBNull.Value ? dt.Rows[0]["ServiceTitle"].ToString() : null;
            }

            if (dt_ContibutionInfo.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_ContibutionInfo.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceContributions objServiceContributions = new AWS_AssociationToolCore.Entities.ServiceContributions();

                    objServiceContributions.ContributionId = (dr["ContributionId"] != DBNull.Value ? Convert.ToInt64(dr["ContributionId"]) : 0);
                    objServiceContributions.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceId"]) : 0);
                    objServiceContributions.ServiceDonationId = (dr["ServiceDonationId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceDonationId"]) : 0);
                    objServiceContributions.Percentage = (dr["Percentage"] != DBNull.Value ? (dr["Percentage"].ToString()) : null);
                    objServiceContributions.ServiceTitle = (dr["ServiceTitle"] != DBNull.Value ? (dr["ServiceTitle"].ToString()) : null);


                    lstServiceContributions.Add(objServiceContributions);
                }

                objServiceDonations.lstServiceContributions = lstServiceContributions;
            }
            return objServiceDonations;
        }


        public Entities.ServiceDonations ServiceDonationsGetById(Int64 ServiceDonationId, ref int status)
        {
            DataSet ds0 = _ServiceDonations.GetServiceDonationById(ServiceDonationId, ref status);

            DataTable dt = ds0.Tables[0];
            DataTable dt_DonorInfo = ds0.Tables[1];

            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();

            if (dt.Rows.Count == 1)
            {

                objServiceDonations.ServiceDonationId = Convert.ToInt64(dt.Rows[0]["ServiceDonationId"].ToString());
                objServiceDonations.MemberId = (dt.Rows[0]["MemberId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["MemberId"]) : 0);
                objServiceDonations.ServiceId = (dt.Rows[0]["ServiceId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ServiceId"]) : 0);
                objServiceDonations.DonationCause = dt.Rows[0]["DonationCause"] != DBNull.Value ? dt.Rows[0]["DonationCause"].ToString() : null;
                objServiceDonations.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                objServiceDonations.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                objServiceDonations.TransactionId = dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null;
                objServiceDonations.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                objServiceDonations.PaymentMethod = dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null;
                objServiceDonations.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                objServiceDonations.PaymentStatus = dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null;
                objServiceDonations.OrderDate = (dt.Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["OrderDate"]) : DateTime.MinValue);
                objServiceDonations.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value ? dt.Rows[0]["PaymentBy"].ToString() : null);
                objServiceDonations.DonationCauses = (dt.Rows[0]["DonationCauses"] != DBNull.Value ? dt.Rows[0]["DonationCauses"].ToString() : null);
                objServiceDonations.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : null);
                objServiceDonations.FirstName = dt.Rows[0]["FirstName"] != DBNull.Value ? dt.Rows[0]["FirstName"].ToString() : null;
                objServiceDonations.LastName = dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString() : null;
                objServiceDonations.Address = dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null;
                objServiceDonations.PhoneNo = dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null;

            }

            if (dt_DonorInfo.Rows.Count == 1)
            {
                objServiceDonations.ChapterId = (dt_DonorInfo.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt_DonorInfo.Rows[0]["ChapterId"]) : 0);
                objServiceDonations.FirstName = dt_DonorInfo.Rows[0]["FirstName"] != DBNull.Value ? dt_DonorInfo.Rows[0]["FirstName"].ToString() : null;
                objServiceDonations.Email = dt_DonorInfo.Rows[0]["Email"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Email"].ToString() : null;
                objServiceDonations.LastName = dt_DonorInfo.Rows[0]["LastName"] != DBNull.Value ? dt_DonorInfo.Rows[0]["LastName"].ToString() : null;
                objServiceDonations.Address = dt_DonorInfo.Rows[0]["Address"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Address"].ToString() : null;
                objServiceDonations.PhoneNo = dt_DonorInfo.Rows[0]["PhoneNo"] != DBNull.Value ? dt_DonorInfo.Rows[0]["PhoneNo"].ToString() : null;
                objServiceDonations.Country = (dt_DonorInfo.Rows[0]["Country"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Country"].ToString() : null);
                objServiceDonations.City = (dt_DonorInfo.Rows[0]["City"] != DBNull.Value ? dt_DonorInfo.Rows[0]["City"].ToString() : null);
                objServiceDonations.State = (dt_DonorInfo.Rows[0]["State"] != DBNull.Value ? dt_DonorInfo.Rows[0]["State"].ToString() : null);
                objServiceDonations.ZipCode = (dt_DonorInfo.Rows[0]["ZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ZipCode"].ToString() : null);
                objServiceDonations.Spouse = (dt_DonorInfo.Rows[0]["Spouse"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Spouse"].ToString() : null);
                objServiceDonations.NoofPayments = (dt_DonorInfo.Rows[0]["NoofPayments"] != DBNull.Value ? Convert.ToInt64(dt_DonorInfo.Rows[0]["NoofPayments"]) : 0);
                objServiceDonations.ZipCode = (dt_DonorInfo.Rows[0]["ZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ZipCode"].ToString() : null);
                objServiceDonations.CardNumber = (dt_DonorInfo.Rows[0]["CardNumber"] != DBNull.Value ? dt_DonorInfo.Rows[0]["CardNumber"].ToString() : null);
                objServiceDonations.ExipryMonth = (dt_DonorInfo.Rows[0]["ExipryMonth"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ExipryMonth"].ToString() : null);
                objServiceDonations.ExipryYear = (dt_DonorInfo.Rows[0]["ExipryYear"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ExipryYear"].ToString() : null);
                objServiceDonations.Cvv = (dt_DonorInfo.Rows[0]["Cvv"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Cvv"].ToString() : null);
                objServiceDonations.SameBilling = (dt_DonorInfo.Rows[0]["SameBilling"] != DBNull.Value ? dt_DonorInfo.Rows[0]["SameBilling"].ToString() : null);
                objServiceDonations.BillingAdress = (dt_DonorInfo.Rows[0]["BillingAdress"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingAdress"].ToString() : null);
                objServiceDonations.BillingCountry = (dt_DonorInfo.Rows[0]["BillingCountry"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingCountry"].ToString() : null);
                objServiceDonations.BillingState = (dt_DonorInfo.Rows[0]["BillingState"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingState"].ToString() : null);
                objServiceDonations.BillingCity = (dt_DonorInfo.Rows[0]["BillingCity"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingCity"].ToString() : null);
                objServiceDonations.BillingZipCode = (dt_DonorInfo.Rows[0]["BillingZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingZipCode"].ToString() : null);
                objServiceDonations.Field1 = (dt_DonorInfo.Rows[0]["Field1"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field1"].ToString() : null);
                objServiceDonations.Field2 = (dt_DonorInfo.Rows[0]["Field2"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field2"].ToString() : null);
                objServiceDonations.Field3 = (dt_DonorInfo.Rows[0]["Field3"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field3"].ToString() : null);
                objServiceDonations.FrequencyofDonation = (dt_DonorInfo.Rows[0]["FrequencyofDonation"] != DBNull.Value ? dt_DonorInfo.Rows[0]["FrequencyofDonation"].ToString() : null);
                objServiceDonations.TypeofCard = (dt_DonorInfo.Rows[0]["TypeofCard"] != DBNull.Value ? dt_DonorInfo.Rows[0]["TypeofCard"].ToString() : null);
                objServiceDonations.KnowledgeAcurate = (dt_DonorInfo.Rows[0]["KnowledgeAcurate"] != DBNull.Value ? dt_DonorInfo.Rows[0]["KnowledgeAcurate"].ToString() : null);

            }



            return objServiceDonations;
        }

        public Entities.ServiceDonations GetServiceDonationsGetTotalAmount(Int64 ChapterId, ref List<Entities.Members> lstCommitteeMembers, ref int status)
        {
            DataSet ds0 = _ServiceDonations.GetServiceDonationsGetTotalAmount(ChapterId, ref status);

            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            DataTable dt = ds0.Tables[0];
            DataTable dt_MembersInfo = ds0.Tables[1];

            if (dt.Rows.Count == 1)
            {
                objServiceDonations.YearTotalAmount = (dt.Rows[0]["YearTotalAmount"] != DBNull.Value ? dt.Rows[0]["YearTotalAmount"].ToString() : null);
                objServiceDonations.TotalAmount = (dt.Rows[0]["TotalAmount"] != DBNull.Value ? dt.Rows[0]["TotalAmount"].ToString() : null);

            }

            if (dt_MembersInfo.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_MembersInfo.Rows)
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

            return objServiceDonations;
        }

        public List<AWS_AssociationToolCore.Entities.ServiceDonations> GetServiceDonationsListByVariableId(Int64 ServiceId, Int64 ServiceDonationId, Int64 PaymentMethod, Int64 PaymentStatus, string StartDate, string EndDate, ref decimal SumofDonation, ref Int64 nofofdonors, string Search, string Sort, int PageNo, int Items,Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<AWS_AssociationToolCore.Entities.ServiceDonations>();
            DataTable dt = _ServiceDonations.GetServiceDonationsListByVariableId(ServiceId, ServiceDonationId, PaymentMethod, PaymentStatus, StartDate, EndDate, ref SumofDonation, ref nofofdonors, Search, Sort, PageNo, Items, ChapterId,ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ServiceDonations.GetServiceDonationsListByVariableId(ServiceId, ServiceDonationId, PaymentMethod, PaymentStatus, StartDate, EndDate, ref SumofDonation, ref nofofdonors, Search, Sort, PageNo - 1, Items, ChapterId, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    objServiceDonations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt32(dr["ServiceId"]) : 0);
                    objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt32(dr["MemberId"]) : 0);
                    objServiceDonations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objServiceDonations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objServiceDonations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceDonations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objServiceDonations.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? (dr["PaymentMethod"].ToString()) : null);
                    objServiceDonations.PaymentStatus = dr["PaymentStatus"].ToString();
                    //objServiceDonations.CardNumber = (dr["CardNumber"] != DBNull.Value ? (dr["CardNumber"].ToString()) : null);
                    // objServiceDonations.CSVExpiry = (dr["CSVExpiry"] != DBNull.Value ? (dr["CSVExpiry"].ToString()) : null);
                    objServiceDonations.DonationCause = (dr["DonationCause"] != DBNull.Value ? (dr["DonationCause"].ToString()) : null);
                    objServiceDonations.Zelle = (dr["Zelle"] != DBNull.Value ? (dr["Zelle"].ToString()) : null);
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objServiceDonations.Email = (dr["Email"] != DBNull.Value ? (dr["Email"].ToString()) : null);
                    objServiceDonations.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? (dr["PhoneNo"].ToString()) : null);
                    lstServiceDonations.Add(objServiceDonations);
                }
            }
            return lstServiceDonations;
        }

        public List<AWS_AssociationToolCore.Entities.ServiceDonations> GetServiceDonationsListByVariable(Int64 ServiceId, Int64 ServiceDonationId, Int64 PaymentMethod, Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<AWS_AssociationToolCore.Entities.ServiceDonations>();
            DataTable dt = _ServiceDonations.GetServiceDonationsListByVariable(ServiceId, ServiceDonationId, PaymentMethod, PaymentStatus, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ServiceDonations.GetServiceDonationsListByVariable(ServiceId, ServiceDonationId, PaymentMethod, PaymentStatus, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    objServiceDonations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt32(dr["ServiceId"]) : 0);
                    objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt32(dr["MemberId"]) : 0);
                    objServiceDonations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objServiceDonations.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objServiceDonations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceDonations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objServiceDonations.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? (dr["PaymentMethod"].ToString()) : null);
                    objServiceDonations.PaymentStatus = dr["PaymentStatus"].ToString();
                    //objServiceDonations.CardNumber = (dr["CardNumber"] != DBNull.Value ? (dr["CardNumber"].ToString()) : null);
                   // objServiceDonations.CSVExpiry = (dr["CSVExpiry"] != DBNull.Value ? (dr["CSVExpiry"].ToString()) : null);
                    objServiceDonations.DonationCause = (dr["DonationCause"] != DBNull.Value ? (dr["DonationCause"].ToString()) : null);
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objServiceDonations.Email = (dr["Email"] != DBNull.Value ? (dr["Email"].ToString()) : null);

                    lstServiceDonations.Add(objServiceDonations);
                }
            }
            return lstServiceDonations;
        }


        public List<AWS_AssociationToolCore.Entities.ServiceDonations> GetServiceDonationsList(Int64 ServiceId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<AWS_AssociationToolCore.Entities.ServiceDonations>(); 
            DataTable dt = _ServiceDonations.GetServiceDonationsList(ServiceId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ServiceDonations.GetServiceDonationsList(ServiceId, Search, Sort, PageNo - 1, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    objServiceDonations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt64(dr["MemberId"]) : 0);
                    objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceId"]) : 0);
                    objServiceDonations.DonationCause = dr["DonationCause"] != DBNull.Value ? dr["DonationCause"].ToString() : null;
                    objServiceDonations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceDonations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objServiceDonations.TransactionId = dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null;
                    objServiceDonations.PaymentMethodId = (dr["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentMethodId"]) : 0);
                    objServiceDonations.PaymentStatusId = (dr["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentStatusId"]) : 0);
                    objServiceDonations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"]) : DateTime.MinValue);
                    objServiceDonations.UpdatedDate = (dr["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedDate"]) : DateTime.MinValue);
                    objServiceDonations.PaymentBy = (dr["PaymentBy"] != DBNull.Value ? dr["PaymentBy"].ToString() : null);
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : "Anonymous");
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : "");
                    objServiceDonations.Field3 = dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null;

                    lstServiceDonations.Add(objServiceDonations);
                }
            }
            return lstServiceDonations;
        }








        public List<AWS_AssociationToolCore.Entities.ServiceContributions> GetContributionsById(Int64 ServiceDonationId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.ServiceContributions> lstServiceContributions = new List<AWS_AssociationToolCore.Entities.ServiceContributions>();

            DataSet ds0 = _ServiceDonations.GetContributionsById(ServiceDonationId, ref status);
             
            DataTable dt_ContibutionInfo = ds0.Tables[0];
             
            if (dt_ContibutionInfo.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_ContibutionInfo.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceContributions objServiceContributions = new AWS_AssociationToolCore.Entities.ServiceContributions();

                    objServiceContributions.ContributionId = (dr["ContributionId"] != DBNull.Value ? Convert.ToInt64(dr["ContributionId"]) : 0);
                    objServiceContributions.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceId"]) : 0);
                    objServiceContributions.ServiceDonationId = (dr["ServiceDonationId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceDonationId"]) : 0);
                    objServiceContributions.Percentage = (dr["Percentage"] != DBNull.Value ? (dr["Percentage"].ToString()) : null);
                    objServiceContributions.ServiceTitle = (dr["ServiceTitle"] != DBNull.Value ? (dr["ServiceTitle"].ToString()) : null);
                    objServiceContributions.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);


                    lstServiceContributions.Add(objServiceContributions);
                } 
            }
            return lstServiceContributions;
        }

        public List<AWS_AssociationToolCore.Entities.ServiceDonations> GetServiceDonarsListByVariable(Int64 ServiceId, Int64 PaymentMethod, Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items,Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<AWS_AssociationToolCore.Entities.ServiceDonations>();
            DataTable dt = _ServiceDonations.GetServiceDonarsListByVariable(ServiceId, PaymentMethod, PaymentStatus, Search, Sort, PageNo, Items, ChapterId, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ServiceDonations.GetServiceDonarsListByVariable(ServiceId, PaymentMethod, PaymentStatus, Search, Sort, PageNo - 1, Items, ChapterId, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    objServiceDonations.RId = Convert.ToInt64(dr["RId"].ToString());
                    //objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    //objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt32(dr["ServiceId"]) : 0);
                    //objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt32(dr["MemberId"]) : 0); 
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objServiceDonations.Email = (dr["Email"] != DBNull.Value ? (dr["Email"].ToString()) : null);
                    objServiceDonations.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? (dr["PhoneNo"].ToString()) : null);
                    objServiceDonations.ContributionsCount = (dr["ContributionsCount"] != DBNull.Value ? Convert.ToInt32(dr["ContributionsCount"]) : 0);
                    objServiceDonations.LastOrderDate = (dr["LastOrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["LastOrderDate"]) : DateTime.MinValue);

                    lstServiceDonations.Add(objServiceDonations);
                }
            }
            return lstServiceDonations;
        }




        public List<AWS_AssociationToolCore.Entities.ServiceDonations> GetServiceDonationsLists(string Email,Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<AWS_AssociationToolCore.Entities.ServiceDonations>();
            DataTable dt = _ServiceDonations.GetServiceDonationsLists(Email, ChapterId, ref status);
           

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    
                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt64(dr["MemberId"]) : 0);
                    objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceId"]) : 0);
                    objServiceDonations.DonationCause = dr["DonationCause"] != DBNull.Value ? dr["DonationCause"].ToString() : null;
                    objServiceDonations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceDonations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objServiceDonations.TransactionId = dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null;

                    objServiceDonations.PaymentMethod = dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null;
                    objServiceDonations.PaymentStatus = dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null;

                   
                    objServiceDonations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"]) : DateTime.MinValue);
                    objServiceDonations.UpdatedDate = (dr["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedDate"]) : DateTime.MinValue);
                    objServiceDonations.PaymentBy = (dr["PaymentBy"] != DBNull.Value ? dr["PaymentBy"].ToString() : null);
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : "Anonymous");
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : "");
                    objServiceDonations.Field3 = dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null;
                    objServiceDonations.DonationCause = dr["DonationCause"] != DBNull.Value ? dr["DonationCause"].ToString() : null;
                    lstServiceDonations.Add(objServiceDonations);
                }
            }
            return lstServiceDonations;
        }






        public Int64 BulkUploads(string ShipmentsXML, ref string Summary)
        {
            Int64 _status = 0;
            _status = _ServiceDonations.BulkUploads(ShipmentsXML, ref Summary);
            return _status;
        }








        #endregion
    }
}
