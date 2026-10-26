using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class Donors
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 InsertDonors(Entities.Donors objDonors, ref Int64 DonorId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@DonorId",objDonors.DonorId),
                    new SqlParameter("@ChapterId",(objDonors.ChapterId==0?(object)DBNull.Value:objDonors.ChapterId)),
                    new SqlParameter("@MemberId",(objDonors.MemberId==0?DBNull.Value:(object)objDonors.MemberId)),
                    new SqlParameter("@FirstName",objDonors.FirstName),
                    new SqlParameter("@LastName",objDonors.LastName),
                    new SqlParameter("@Email",objDonors.Email),
                    new SqlParameter("@PhoneNo",(objDonors.PhoneNo == null ?DBNull.Value:(object)objDonors.PhoneNo)),
                    new SqlParameter("@Address",(objDonors.Address == null ?DBNull.Value:(object)objDonors.Address)),
                    new SqlParameter("@DonationProgram",(objDonors.DonationProgram == null ?DBNull.Value:(object)objDonors.DonationProgram)),
                    new SqlParameter("@DonationCause",(objDonors.DonationCause == null ?DBNull.Value:(object)objDonors.DonationCause)),
                    new SqlParameter("@IsActive",objDonors.IsActive),
                    new SqlParameter("@Amount",(objDonors.Amount == 0 ?DBNull.Value:(object)objDonors.Amount)),
                    new SqlParameter("@TransactionId",(objDonors.TransactionId == null ?DBNull.Value:(object)objDonors.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objDonors.PaymentStatusId == 0 ?DBNull.Value:(object)objDonors.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objDonors.PaymentMethodId == 0 ?DBNull.Value:(object)objDonors.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objDonors.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objDonors.OrderDate)),
                    new SqlParameter("@PaymentBy",(objDonors.PaymentBy == null ?DBNull.Value:(object)objDonors.PaymentBy)),                    
                    new SqlParameter("@InsertedBy",objDonors.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objDonors.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                   new SqlParameter("@DonorInfoId",(objDonors.DonorInfoId == 0 ?DBNull.Value:(object)objDonors.DonorInfoId)),
                     new SqlParameter("@Country",(objDonors.Country == null ?DBNull.Value:(object)objDonors.Country)),
                    new SqlParameter("@City",(objDonors.City == null ?DBNull.Value:(object)objDonors.City)),
                    new SqlParameter("@State",(objDonors.State == null ?DBNull.Value:(object)objDonors.State)),
                    new SqlParameter("@ZipCode",(objDonors.ZipCode == null ?DBNull.Value:(object)objDonors.ZipCode)),
                    new SqlParameter("@Spouse",(objDonors.Spouse == null ?DBNull.Value:(object)objDonors.Spouse)),
                    new SqlParameter("@FrequencyofDonation",(objDonors.FrequencyofDonation == null ?DBNull.Value:(object)objDonors.FrequencyofDonation)),
                    new SqlParameter("@NoofPayments",(objDonors.NoofPayments == 0 ?DBNull.Value:(object)objDonors.NoofPayments)),
                    new SqlParameter("@TypeofCard",(objDonors.TypeofCard == null ?DBNull.Value:(object)objDonors.TypeofCard)),
                    new SqlParameter("@CardNumber",(objDonors.CardNumber == null ?DBNull.Value:(object)objDonors.CardNumber)),
                    new SqlParameter("@ExipryMonth",(objDonors.ExipryMonth == null ?DBNull.Value:(object)objDonors.ExipryMonth)),
                    new SqlParameter("@ExipryYear",(objDonors.ExipryYear == null ?DBNull.Value:(object)objDonors.ExipryYear)),
                    new SqlParameter("@Cvv",(objDonors.Cvv == null ?DBNull.Value:(object)objDonors.Cvv)),
                    new SqlParameter("@SameBilling",(objDonors.SameBilling == null ?DBNull.Value:(object)objDonors.SameBilling)),
                    new SqlParameter("@BillingAdress",(objDonors.BillingAdress == null ?DBNull.Value:(object)objDonors.BillingAdress)),
                    new SqlParameter("@BillingCountry",(objDonors.BillingCountry == null ?DBNull.Value:(object)objDonors.BillingCountry)),
                    new SqlParameter("@BillingCity",(objDonors.BillingCity == null ?DBNull.Value:(object)objDonors.BillingCity)),
                    new SqlParameter("@BillingState",(objDonors.BillingState == null ?DBNull.Value:(object)objDonors.BillingState)),
                    new SqlParameter("@BillingZipCode",(objDonors.BillingZipCode == null ?DBNull.Value:(object)objDonors.BillingZipCode)),
                    new SqlParameter("@KnowledgeAcurate",(objDonors.KnowledgeAcurate == null ?DBNull.Value:(object)objDonors.KnowledgeAcurate)),
                    new SqlParameter("@Field1",(objDonors.Field1 == null ?DBNull.Value:(object)objDonors.Field1)),
                    new SqlParameter("@Field2",(objDonors.Field2 == null ?DBNull.Value:(object)objDonors.Field2)),
                    new SqlParameter("@Field3",(objDonors.Field3 == null ?DBNull.Value:(object)objDonors.Field3)),
                    
                    };


                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[21].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DonorsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[21].Value);

                DonorId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateDonorsStatus(Int64 DonorId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@DonorId",DonorId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DonorsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateDonors(Entities.Donors objDonors)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@DonorId",objDonors.DonorId),                    
                    new SqlParameter("@Amount",(objDonors.Amount == 0 ?DBNull.Value:(object)objDonors.Amount)),
                    new SqlParameter("@TransactionId",(objDonors.TransactionId == null ?DBNull.Value:(object)objDonors.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objDonors.PaymentStatusId == 0 ?DBNull.Value:(object)objDonors.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objDonors.PaymentMethodId == 0 ?DBNull.Value:(object)objDonors.PaymentMethodId)),
                    new SqlParameter("@PaymentStatus",(objDonors.PaymentStatus == null ?DBNull.Value:(object)objDonors.PaymentStatus)),
                    new SqlParameter("@PaymentMethod",(objDonors.PaymentMethod == null ?DBNull.Value:(object)objDonors.PaymentMethod)),
                    new SqlParameter("@OrderDate",(objDonors.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objDonors.OrderDate)),                   
                    new SqlParameter("@UpdatedBy",objDonors.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DonorsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        
        public DataTable GetDonorsListByVariable(Int64 MemberId,Int64 ChapterId,Int64 PaymentMethod, Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatus",PaymentStatus),
                    new SqlParameter("@PaymentMethod",PaymentMethod),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@MemberId",MemberId)
                };

                _sqlP[6].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("DonorsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[6].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetDonorById(Int64 DonorId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@DonorId",DonorId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("DonorsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

     


        
        public Int64 DeleteDonor(Int64 DonorId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@DonorId",DonorId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DonorsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable DonorRegistrationsExportToExcel(Int64 PaymentStatusId, Int64 PaymentMethodId, string Search, string Sort)
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatusId",PaymentStatusId),
                    new SqlParameter("@PaymentMethodId",PaymentMethodId),                    
                    //new SqlParameter("@StartDate",StartDate),
                    //new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@Total",0)
                };
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("DonorRegistrationsExportToExcel", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #region Front-end

        public DataTable FEDonorsGetList(string Search, string Type, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEDonorsGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }



        public DataTable FEDonorsMemberIdGetList(Int64 MemberId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@MemberId",MemberId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEDonorsMemberIdGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FEGetDonorById(Int64 DonorId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@DonorId",DonorId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEDonorsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public DataSet DonorsGetById(Int64 DonorId, ref int status)
        {
            DataSet ds0 = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@DonorId",DonorId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds0 = _dbAccess.GetDataSet("DonorsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds0;
        }


        #endregion
    }
}
