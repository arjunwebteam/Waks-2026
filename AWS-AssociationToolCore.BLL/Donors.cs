using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class Donors
    {
        AWS_AssociationToolCore.DAL.Donors _Donors = new AWS_AssociationToolCore.DAL.Donors();

        #region Methods

        public Int64 InsertDonors(Entities.Donors objDonors, ref Int64 DonorId)
        {
            Int64 _status = 0;
            if (objDonors != null)
            {
                _status = _Donors.InsertDonors(objDonors, ref DonorId);

            }
            return _status;
        }

        public Int64 UpdateDonors(Entities.Donors objDonors)
        {
            Int64 _status = 0;
            if (objDonors != null)
            {
                _status = _Donors.UpdateDonors(objDonors);

            }
            return _status;
        }

        public Int64 DeleteDonor(Int64 DonorId)
        {
            Int64 _status = 0;
            _status = _Donors.DeleteDonor(DonorId);
            return _status;
        }

        public Int64 UpdateDonorsStatus(Int64 DonorId)
        {
            Int64 _status = 0;
            _status = _Donors.UpdateDonorsStatus(DonorId);
            return _status;
        }

        #endregion

        #region Entities filling
        
        public AWS_AssociationToolCore.Entities.Donors GetDonorsById(Int64 DonorId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Donors objDonors = new AWS_AssociationToolCore.Entities.Donors();
            DataTable dt = new DataTable();
            if (DonorId != 0)
            {
                dt = _Donors.GetDonorById(DonorId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objDonors.DonorId = Convert.ToInt64(dt.Rows[0]["DonorId"].ToString());
                    objDonors.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objDonors.Email = dt.Rows[0]["Email"].ToString();
                    objDonors.LastName = dt.Rows[0]["LastName"].ToString();
                    objDonors.Address = dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null;
                    objDonors.PhoneNo = dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null;
                    objDonors.DonationProgram = dt.Rows[0]["DonationProgram"] != DBNull.Value ?dt.Rows[0]["DonationProgram"].ToString():null;
                    objDonors.DonationCause = dt.Rows[0]["DonationCause"] != DBNull.Value ? dt.Rows[0]["DonationCause"].ToString() : null;
                    objDonors.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ?Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objDonors.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                    objDonors.TransactionId = dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null;
                    objDonors.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                    objDonors.PaymentMethod = dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null;
                    objDonors.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                    objDonors.PaymentStatus = dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null;
                    objDonors.OrderDate = (dt.Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["OrderDate"]) : DateTime.MinValue);
                    objDonors.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value? dt.Rows[0]["PaymentBy"].ToString():null);
                    objDonors.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    objDonors.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

                }
            }
            return objDonors;
        }

        public List<AWS_AssociationToolCore.Entities.Donors> GetDonorsListByVariable(Int64 MemberId,Int64 ChapterId, Int64 PaymentMethod,Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Donors> lstDonors = new List<AWS_AssociationToolCore.Entities.Donors>();
            DataTable dt = _Donors.GetDonorsListByVariable(MemberId,ChapterId, PaymentMethod, PaymentStatus, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Donors.GetDonorsListByVariable(MemberId,ChapterId, PaymentMethod, PaymentStatus, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Donors objDonors = new AWS_AssociationToolCore.Entities.Donors();

                    objDonors.RId = Convert.ToInt64(dr["RId"].ToString());
                    objDonors.DonorId = Convert.ToInt64(dr["DonorId"].ToString());
                    objDonors.FirstName = dr["FirstName"].ToString();
                    objDonors.LastName = dr["LastName"].ToString();
                    objDonors.Email = dr["Email"].ToString();
                    objDonors.PhoneNo = dr["PhoneNo"].ToString();
                    objDonors.DonationProgram = (dr["DonationProgram"] != DBNull.Value ? (dr["DonationProgram"].ToString()) : null);
                    objDonors.OrderDate = (dr["OrderDate"] != DBNull.Value ?Convert.ToDateTime(dr["OrderDate"].ToString()) : DateTime.MinValue);
                    objDonors.TransactionId = (dr["TransactionId"] != DBNull.Value ? (dr["TransactionId"].ToString()) : null);
                    objDonors.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonors.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonors.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? (dr["PaymentMethod"].ToString()) : null);
                    objDonors.PaymentStatus = dr["PaymentStatus"].ToString();
                    objDonors.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objDonors.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objDonors.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);

                    lstDonors.Add(objDonors);
                }
            }
            return lstDonors;
        }
        
        #endregion

        #region

        public List<AWS_AssociationToolCore.Entities.Donors> FEDonorsGetList(string Search, string Type, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Donors> lstDonors = new List<Entities.Donors>();

            DataTable dt = _Donors.FEDonorsGetList(Search, Type, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Donors.FEDonorsGetList(Search, Type, Sort, PageNo - 1, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Donors objDonors = new AWS_AssociationToolCore.Entities.Donors();

                    objDonors.RId = Convert.ToInt64(dr["RId"].ToString());

                    objDonors.DonorId = Convert.ToInt64(dr["DonorId"].ToString());
                    objDonors.FirstName = dr["FirstName"].ToString();
                    objDonors.LastName = dr["LastName"].ToString();
                    objDonors.Email = dr["Email"].ToString();
                    objDonors.PhoneNo = dr["PhoneNo"].ToString();
                     
                    objDonors.DonationProgram = dr["DonationProgram"].ToString();
                    
                   
                    objDonors.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    
                    objDonors.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());

                    lstDonors.Add(objDonors);
                }

            }
            return lstDonors;
        }


        public List<AWS_AssociationToolCore.Entities.ServiceDonations> FEDonorsMemberIdGetList(Int64 MemberId,string Search,string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();

            DataTable dt = _Donors.FEDonorsMemberIdGetList(MemberId,Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Donors.FEDonorsMemberIdGetList(MemberId,Search, Sort, PageNo - 1, Items, ref Total);
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
                    
                    objServiceDonations.DonationCause = (dr["DonationCause"] != DBNull.Value ? (dr["DonationCause"].ToString()) : null);
                    objServiceDonations.FirstName = (dr["FirstName"] != DBNull.Value ? (dr["FirstName"].ToString()) : null);
                    objServiceDonations.LastName = (dr["LastName"] != DBNull.Value ? (dr["LastName"].ToString()) : null);
                    objServiceDonations.Email = (dr["Email"] != DBNull.Value ? (dr["Email"].ToString()) : null);
                    objServiceDonations.IsDisplay = (dr["IsDisplay"] != DBNull.Value ? Convert.ToBoolean(dr["IsDisplay"]) : false);

                    lstServiceDonations.Add(objServiceDonations);
                }

            }
            return lstServiceDonations;
        }

        public AWS_AssociationToolCore.Entities.Donors FEGetDonorsById(Int64 DonorId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Donors objDonors = new AWS_AssociationToolCore.Entities.Donors();
            DataTable dt = new DataTable();
            if (DonorId != 0)
            {
                dt = _Donors.FEGetDonorById(DonorId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objDonors.DonorId = Convert.ToInt64(dt.Rows[0]["DonorId"].ToString());
                    objDonors.DonationCategoryId = (dt.Rows[0]["DonationCategoryId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["DonationCategoryId"]) : 0);
                    objDonors.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                    //objDonors.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);
                    objDonors.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objDonors.Email = dt.Rows[0]["Email"].ToString();
                    objDonors.LastName = dt.Rows[0]["LastName"].ToString();
                    objDonors.Address = dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null;
                    objDonors.PhoneNo = dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null;
                    objDonors.DonationProgram = dt.Rows[0]["DonationProgram"] != DBNull.Value ? dt.Rows[0]["DonationProgram"].ToString() : null;
                    objDonors.DonationCause = dt.Rows[0]["DonationCause"] != DBNull.Value ? dt.Rows[0]["DonationCause"].ToString() : null;
                    objDonors.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objDonors.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                    objDonors.TransactionId = dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null;
                    objDonors.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                    objDonors.PaymentMethod = dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null;
                    objDonors.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                    objDonors.PaymentStatus = dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null;
                    objDonors.OrderDate = (dt.Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["OrderDate"]) : DateTime.MinValue);
                    objDonors.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value ? dt.Rows[0]["PaymentBy"].ToString() : null);
                    //objDonors.FirstDistrictPreference = (dt.Rows[0]["FirstDistrictPreference"] != DBNull.Value ? dt.Rows[0]["FirstDistrictPreference"].ToString() : null);
                    //objDonors.SecondDistrictPreference = (dt.Rows[0]["SecondDistrictPreference"] != DBNull.Value ? dt.Rows[0]["SecondDistrictPreference"].ToString() : null);
                    //objDonors.ThirdDistrictPreference = (dt.Rows[0]["ThirdDistrictPreference"] != DBNull.Value ? dt.Rows[0]["ThirdDistrictPreference"].ToString() : null);
                    //objDonors.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    //objDonors.CategoryName = dt.Rows[0]["CategoryName"] != DBNull.Value ? dt.Rows[0]["CategoryName"].ToString() : null;
                    //objDonors.EventName = dt.Rows[0]["EventName"] != DBNull.Value ? dt.Rows[0]["EventName"].ToString() : null;
                    //objDonors.Heading = dt.Rows[0]["Heading"] != DBNull.Value ? dt.Rows[0]["Heading"].ToString() : null;

                }
            }
            return objDonors;
        }

        #endregion


        public Entities.Donors DonorsGetById(Int64 DonorId, ref int status)
        {
            DataSet ds0 = _Donors.DonorsGetById(DonorId, ref status);

            DataTable dt = ds0.Tables[0];
            DataTable dt_DonorInfo = ds0.Tables[1];


            Entities.Donors objDonors = new Entities.Donors();
           
            if (dt.Rows.Count == 1)
            {

                objDonors.DonorId = Convert.ToInt64(dt.Rows[0]["DonorId"].ToString());
                objDonors.FirstName = dt.Rows[0]["FirstName"].ToString();
                objDonors.Email = dt.Rows[0]["Email"].ToString();
                objDonors.LastName = dt.Rows[0]["LastName"].ToString();
                objDonors.Address = dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null;
                objDonors.PhoneNo = dt.Rows[0]["PhoneNo"] != DBNull.Value ? dt.Rows[0]["PhoneNo"].ToString() : null;
                objDonors.DonationProgram = dt.Rows[0]["DonationProgram"] != DBNull.Value ? dt.Rows[0]["DonationProgram"].ToString() : null;
                objDonors.DonationCause = dt.Rows[0]["DonationCause"] != DBNull.Value ? dt.Rows[0]["DonationCause"].ToString() : null;
                objDonors.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                objDonors.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                objDonors.TransactionId = dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null;
                objDonors.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentMethodId"]) : 0);
                objDonors.PaymentMethod = dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null;
                objDonors.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PaymentStatusId"]) : 0);
                objDonors.PaymentStatus = dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null;
                objDonors.OrderDate = (dt.Rows[0]["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["OrderDate"]) : DateTime.MinValue);
                objDonors.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value ? dt.Rows[0]["PaymentBy"].ToString() : null);
                objDonors.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                objDonors.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                objDonors.ChapterName = dt.Rows[0]["ChapterName"] != DBNull.Value ? dt.Rows[0]["ChapterName"].ToString() : null;

                

            }


            if (dt_DonorInfo.Rows.Count == 1)
            {
                 
                objDonors.Country = (dt_DonorInfo.Rows[0]["Country"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Country"].ToString() : null);
                objDonors.City = (dt_DonorInfo.Rows[0]["City"] != DBNull.Value ? dt_DonorInfo.Rows[0]["City"].ToString() : null);
                objDonors.State = (dt_DonorInfo.Rows[0]["State"] != DBNull.Value ? dt_DonorInfo.Rows[0]["State"].ToString() : null);
                objDonors.ZipCode = (dt_DonorInfo.Rows[0]["ZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ZipCode"].ToString() : null);
                objDonors.Spouse = (dt_DonorInfo.Rows[0]["Spouse"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Spouse"].ToString() : null);
                objDonors.NoofPayments = (dt_DonorInfo.Rows[0]["NoofPayments"] != DBNull.Value ? Convert.ToInt64(dt_DonorInfo.Rows[0]["NoofPayments"]) : 0);
                objDonors.ZipCode = (dt_DonorInfo.Rows[0]["ZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ZipCode"].ToString() : null);

                objDonors.CardNumber = (dt_DonorInfo.Rows[0]["CardNumber"] != DBNull.Value ? dt_DonorInfo.Rows[0]["CardNumber"].ToString() : null);
                objDonors.ExipryMonth = (dt_DonorInfo.Rows[0]["ExipryMonth"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ExipryMonth"].ToString() : null);
                objDonors.ExipryYear = (dt_DonorInfo.Rows[0]["ExipryYear"] != DBNull.Value ? dt_DonorInfo.Rows[0]["ExipryYear"].ToString() : null);
                objDonors.Cvv = (dt_DonorInfo.Rows[0]["Cvv"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Cvv"].ToString() : null);
                objDonors.SameBilling = (dt_DonorInfo.Rows[0]["SameBilling"] != DBNull.Value ? dt_DonorInfo.Rows[0]["SameBilling"].ToString() : null);
                objDonors.BillingAdress = (dt_DonorInfo.Rows[0]["BillingAdress"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingAdress"].ToString() : null);
                objDonors.BillingCountry = (dt_DonorInfo.Rows[0]["BillingCountry"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingCountry"].ToString() : null);
                objDonors.BillingState = (dt_DonorInfo.Rows[0]["BillingState"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingState"].ToString() : null);
                objDonors.BillingCity = (dt_DonorInfo.Rows[0]["BillingCity"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingCity"].ToString() : null);
                objDonors.BillingZipCode = (dt_DonorInfo.Rows[0]["BillingZipCode"] != DBNull.Value ? dt_DonorInfo.Rows[0]["BillingZipCode"].ToString() : null);

                objDonors.Field1 = (dt_DonorInfo.Rows[0]["Field1"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field1"].ToString() : null);
                objDonors.Field2 = (dt_DonorInfo.Rows[0]["Field2"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field2"].ToString() : null);
                objDonors.Field3 = (dt_DonorInfo.Rows[0]["Field3"] != DBNull.Value ? dt_DonorInfo.Rows[0]["Field3"].ToString() : null);
                objDonors.FrequencyofDonation = (dt_DonorInfo.Rows[0]["FrequencyofDonation"] != DBNull.Value ? dt_DonorInfo.Rows[0]["FrequencyofDonation"].ToString() : null);
                objDonors.TypeofCard = (dt_DonorInfo.Rows[0]["TypeofCard"] != DBNull.Value ? dt_DonorInfo.Rows[0]["TypeofCard"].ToString() : null);
                objDonors.KnowledgeAcurate = (dt_DonorInfo.Rows[0]["KnowledgeAcurate"] != DBNull.Value ? dt_DonorInfo.Rows[0]["KnowledgeAcurate"].ToString() : null);

                
            }



            return objDonors;
        }
















    }
}
