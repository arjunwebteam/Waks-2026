using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
  public class ServiceDonations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 InsertServiceDonations(Entities.ServiceDonations objServiceDonations, ref Int64 ServiceDonationId,ref string receipturl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ServiceDonationId",objServiceDonations.ServiceDonationId),
                    new SqlParameter("@ReceiptUrl",receipturl),
                    new SqlParameter("@MemberId",(objServiceDonations.MemberId==0?DBNull.Value:(object)objServiceDonations.MemberId)),
                    new SqlParameter("@ServiceId",(objServiceDonations.ServiceId==0?(object)DBNull.Value:objServiceDonations.ServiceId)),
                    new SqlParameter("@DonationCause",(objServiceDonations.DonationCause == null ?DBNull.Value:(object)objServiceDonations.DonationCause)),
                    new SqlParameter("@IsActive",objServiceDonations.IsActive),
                    new SqlParameter("@Amount",(objServiceDonations.Amount == 0 ?DBNull.Value:(object)objServiceDonations.Amount)),
                    new SqlParameter("@TransactionId",(objServiceDonations.TransactionId == null ?DBNull.Value:(object)objServiceDonations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objServiceDonations.PaymentStatusId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objServiceDonations.PaymentMethodId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objServiceDonations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objServiceDonations.OrderDate)),
                    new SqlParameter("@PaymentBy",(objServiceDonations.PaymentBy == null ?DBNull.Value:(object)objServiceDonations.PaymentBy)),
                    new SqlParameter("@UpdatedDate",objServiceDonations.UpdatedDate),
                    new SqlParameter("@UpdatedBy",objServiceDonations.UpdatedBy),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@DonorInfoId",(objServiceDonations.DonorInfoId==0?(object)DBNull.Value:objServiceDonations.DonorInfoId)),
                    new SqlParameter("@ChapterId",(objServiceDonations.ChapterId==0?(object)DBNull.Value:objServiceDonations.ChapterId)),
                   // new SqlParameter("@FirstName",objServiceDonations.FirstName),
                   // new SqlParameter("@LastName",objServiceDonations.LastName),
                   // new SqlParameter("@Email",objServiceDonations.Email),
                    new SqlParameter("@FirstName",(objServiceDonations.FirstName == null ?DBNull.Value:(object)objServiceDonations.FirstName)),
                    new SqlParameter("@LastName",(objServiceDonations.LastName == null ?DBNull.Value:(object)objServiceDonations.LastName)),
                    new SqlParameter("@Email",(objServiceDonations.Email == null ?DBNull.Value:(object)objServiceDonations.Email)),

                    new SqlParameter("@PhoneNo",(objServiceDonations.PhoneNo == null ?DBNull.Value:(object)objServiceDonations.PhoneNo)),
                    new SqlParameter("@Address",(objServiceDonations.Address == null ?DBNull.Value:(object)objServiceDonations.Address)),
                    new SqlParameter("@Country",(objServiceDonations.Country == null ?DBNull.Value:(object)objServiceDonations.Country)),
                    new SqlParameter("@City",(objServiceDonations.City == null ?DBNull.Value:(object)objServiceDonations.City)),
                    new SqlParameter("@State",(objServiceDonations.State == null ?DBNull.Value:(object)objServiceDonations.State)),
                    new SqlParameter("@ZipCode",(objServiceDonations.ZipCode == null ?DBNull.Value:(object)objServiceDonations.ZipCode)),
                    new SqlParameter("@Spouse",(objServiceDonations.Spouse == null ?DBNull.Value:(object)objServiceDonations.Spouse)),
                    new SqlParameter("@FrequencyofDonation",(objServiceDonations.FrequencyofDonation == null ?DBNull.Value:(object)objServiceDonations.FrequencyofDonation)),
                    new SqlParameter("@NoofPayments",(objServiceDonations.NoofPayments == 0 ?DBNull.Value:(object)objServiceDonations.NoofPayments)),
                    new SqlParameter("@TypeofCard",(objServiceDonations.TypeofCard == null ?DBNull.Value:(object)objServiceDonations.TypeofCard)),
                    new SqlParameter("@CardNumber",(objServiceDonations.CardNumber == null ?DBNull.Value:(object)objServiceDonations.CardNumber)),
                    new SqlParameter("@ExipryMonth",(objServiceDonations.ExipryMonth == null ?DBNull.Value:(object)objServiceDonations.ExipryMonth)),
                    new SqlParameter("@ExipryYear",(objServiceDonations.ExipryYear == null ?DBNull.Value:(object)objServiceDonations.ExipryYear)),
                    new SqlParameter("@Cvv",(objServiceDonations.Cvv == null ?DBNull.Value:(object)objServiceDonations.Cvv)),
                    new SqlParameter("@SameBilling",(objServiceDonations.SameBilling == null ?DBNull.Value:(object)objServiceDonations.SameBilling)),
                    new SqlParameter("@BillingAdress",(objServiceDonations.BillingAdress == null ?DBNull.Value:(object)objServiceDonations.BillingAdress)),
                    new SqlParameter("@BillingCountry",(objServiceDonations.BillingCountry == null ?DBNull.Value:(object)objServiceDonations.BillingCountry)),
                    new SqlParameter("@BillingCity",(objServiceDonations.BillingCity == null ?DBNull.Value:(object)objServiceDonations.BillingCity)),
                    new SqlParameter("@BillingState",(objServiceDonations.BillingState == null ?DBNull.Value:(object)objServiceDonations.BillingState)),
                    new SqlParameter("@BillingZipCode",(objServiceDonations.BillingZipCode == null ?DBNull.Value:(object)objServiceDonations.BillingZipCode)),
                    new SqlParameter("@KnowledgeAcurate",(objServiceDonations.KnowledgeAcurate == null ?DBNull.Value:(object)objServiceDonations.KnowledgeAcurate)),
                    new SqlParameter("@Field1",(objServiceDonations.Field1 == null ?DBNull.Value:(object)objServiceDonations.Field1)),
                    new SqlParameter("@Field2",(objServiceDonations.Field2 == null ?DBNull.Value:(object)objServiceDonations.Field2)),
                    new SqlParameter("@Field3",(objServiceDonations.Field3 == null ?DBNull.Value:(object)objServiceDonations.Field3)),

                     new SqlParameter("@Zelle",(objServiceDonations.Zelle  == null?DBNull.Value: (object)objServiceDonations.Zelle)),
                    new SqlParameter("@ZellePartnerName",(objServiceDonations.ZellePartnerName  == null?DBNull.Value: (object)objServiceDonations.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objServiceDonations.ChequeHolderName  == null?DBNull.Value: (object)objServiceDonations.ChequeHolderName)),


                };

                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;


                _sqlP[1].SqlDbType = SqlDbType.NVarChar;
                _sqlP[1].Size = 256;
                _sqlP[1].Direction = System.Data.ParameterDirection.InputOutput;

                

                _sqlP[14].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceDonationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[14].Value);

                ServiceDonationId = Convert.ToInt64(_sqlP[0].Value);
                receipturl = _sqlP[1].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 ServiceDonationsByAdmin(Entities.ServiceDonations objServiceDonations, ref Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ServiceDonationId",objServiceDonations.ServiceDonationId),
                    new SqlParameter("@MemberId",(objServiceDonations.MemberId==0?DBNull.Value:(object)objServiceDonations.MemberId)),
                    new SqlParameter("@ServiceId",(objServiceDonations.ServiceId==0?(object)DBNull.Value:objServiceDonations.ServiceId)),
                    new SqlParameter("@DonationCause",(objServiceDonations.DonationCause == null ?DBNull.Value:(object)objServiceDonations.DonationCause)),
                    new SqlParameter("@IsActive",objServiceDonations.IsActive),
                    new SqlParameter("@Amount",(objServiceDonations.Amount == 0 ?DBNull.Value:(object)objServiceDonations.Amount)),
                    new SqlParameter("@TransactionId",(objServiceDonations.TransactionId == null ?DBNull.Value:(object)objServiceDonations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objServiceDonations.PaymentStatusId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objServiceDonations.PaymentMethodId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objServiceDonations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objServiceDonations.OrderDate)),
                    new SqlParameter("@PaymentBy",(objServiceDonations.PaymentBy == null ?DBNull.Value:(object)objServiceDonations.PaymentBy)),
                    new SqlParameter("@Zelle",(objServiceDonations.Zelle == null ?DBNull.Value:(object)objServiceDonations.Zelle)),
                    new SqlParameter("@ReceiptUrl",(objServiceDonations.ReceiptUrl == null ?DBNull.Value:(object)objServiceDonations.ReceiptUrl)),
                    new SqlParameter("@ZellePartnerName",(objServiceDonations.ZellePartnerName == null ?DBNull.Value:(object)objServiceDonations.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objServiceDonations.ChequeHolderName == null ?DBNull.Value:(object)objServiceDonations.ChequeHolderName)),
                    new SqlParameter("@UpdatedDate",objServiceDonations.UpdatedDate),
                    new SqlParameter("@UpdatedBy",objServiceDonations.UpdatedBy),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@DonorInfoId",(objServiceDonations.DonorInfoId==0?(object)DBNull.Value:objServiceDonations.DonorInfoId)),
                    new SqlParameter("@ChapterId",(objServiceDonations.ChapterId==0?(object)DBNull.Value:objServiceDonations.ChapterId)),
                   // new SqlParameter("@FirstName",objServiceDonations.FirstName),
                   // new SqlParameter("@LastName",objServiceDonations.LastName),
                   // new SqlParameter("@Email",objServiceDonations.Email),
                    new SqlParameter("@FirstName",(objServiceDonations.FirstName == null ?DBNull.Value:(object)objServiceDonations.FirstName)),
                    new SqlParameter("@LastName",(objServiceDonations.LastName == null ?DBNull.Value:(object)objServiceDonations.LastName)),
                    new SqlParameter("@Email",(objServiceDonations.Email == null ?DBNull.Value:(object)objServiceDonations.Email)),

                    new SqlParameter("@PhoneNo",(objServiceDonations.PhoneNo == null ?DBNull.Value:(object)objServiceDonations.PhoneNo)),
                    new SqlParameter("@Address",(objServiceDonations.Address == null ?DBNull.Value:(object)objServiceDonations.Address)),
                    new SqlParameter("@Country",(objServiceDonations.Country == null ?DBNull.Value:(object)objServiceDonations.Country)),
                    new SqlParameter("@City",(objServiceDonations.City == null ?DBNull.Value:(object)objServiceDonations.City)),
                    new SqlParameter("@State",(objServiceDonations.State == null ?DBNull.Value:(object)objServiceDonations.State)),
                    new SqlParameter("@ZipCode",(objServiceDonations.ZipCode == null ?DBNull.Value:(object)objServiceDonations.ZipCode)),
                    new SqlParameter("@Spouse",(objServiceDonations.Spouse == null ?DBNull.Value:(object)objServiceDonations.Spouse)),
                    new SqlParameter("@FrequencyofDonation",(objServiceDonations.FrequencyofDonation == null ?DBNull.Value:(object)objServiceDonations.FrequencyofDonation)),
                    new SqlParameter("@NoofPayments",(objServiceDonations.NoofPayments == 0 ?DBNull.Value:(object)objServiceDonations.NoofPayments)),
                    new SqlParameter("@TypeofCard",(objServiceDonations.TypeofCard == null ?DBNull.Value:(object)objServiceDonations.TypeofCard)),
                    new SqlParameter("@CardNumber",(objServiceDonations.CardNumber == null ?DBNull.Value:(object)objServiceDonations.CardNumber)),
                    new SqlParameter("@ExipryMonth",(objServiceDonations.ExipryMonth == null ?DBNull.Value:(object)objServiceDonations.ExipryMonth)),
                    new SqlParameter("@ExipryYear",(objServiceDonations.ExipryYear == null ?DBNull.Value:(object)objServiceDonations.ExipryYear)),
                    new SqlParameter("@Cvv",(objServiceDonations.Cvv == null ?DBNull.Value:(object)objServiceDonations.Cvv)),
                    new SqlParameter("@SameBilling",(objServiceDonations.SameBilling == null ?DBNull.Value:(object)objServiceDonations.SameBilling)),
                    new SqlParameter("@BillingAdress",(objServiceDonations.BillingAdress == null ?DBNull.Value:(object)objServiceDonations.BillingAdress)),
                    new SqlParameter("@BillingCountry",(objServiceDonations.BillingCountry == null ?DBNull.Value:(object)objServiceDonations.BillingCountry)),
                    new SqlParameter("@BillingCity",(objServiceDonations.BillingCity == null ?DBNull.Value:(object)objServiceDonations.BillingCity)),
                    new SqlParameter("@BillingState",(objServiceDonations.BillingState == null ?DBNull.Value:(object)objServiceDonations.BillingState)),
                    new SqlParameter("@BillingZipCode",(objServiceDonations.BillingZipCode == null ?DBNull.Value:(object)objServiceDonations.BillingZipCode)),
                    new SqlParameter("@KnowledgeAcurate",(objServiceDonations.KnowledgeAcurate == null ?DBNull.Value:(object)objServiceDonations.KnowledgeAcurate)),
                    new SqlParameter("@Field1",(objServiceDonations.Field1 == null ?DBNull.Value:(object)objServiceDonations.Field1)),
                    new SqlParameter("@Field2",(objServiceDonations.Field2 == null ?DBNull.Value:(object)objServiceDonations.Field2)),
                    new SqlParameter("@Field3",(objServiceDonations.Field3 == null ?DBNull.Value:(object)objServiceDonations.Field3)),

                    };

                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[17].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceDonationsInsertByAdmin", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[17].Value);

                ServiceDonationId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertServiceContributions(Entities.ServiceContributions objServiceContributions)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ContributionId",objServiceContributions.ContributionId),
                    new SqlParameter("@ServiceId",(objServiceContributions.ServiceId==0?DBNull.Value:(object)objServiceContributions.ServiceId)),
                    new SqlParameter("@ServiceDonationId",(objServiceContributions.ServiceDonationId==0?(object)DBNull.Value:objServiceContributions.ServiceDonationId)),
                    new SqlParameter("@Percentage",(objServiceContributions.Percentage == null ?DBNull.Value:(object)objServiceContributions.Percentage)), 
                    new SqlParameter("@QStatus",0),
                  new SqlParameter("@CAmount",(objServiceContributions.CAmount==0?DBNull.Value:(object)objServiceContributions.CAmount))
                    };

                
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceContributionsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[4].Value); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }



        public Int64 UpdateServiceDonationsStatus(Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceDonationsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateServiceDonations(Entities.ServiceDonations objServiceDonations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ServiceDonationId",objServiceDonations.ServiceDonationId),
                    new SqlParameter("@Amount",(objServiceDonations.Amount == 0 ?DBNull.Value:(object)objServiceDonations.Amount)),
                    new SqlParameter("@TransactionId",(objServiceDonations.TransactionId == null ?DBNull.Value:(object)objServiceDonations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objServiceDonations.PaymentStatusId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objServiceDonations.PaymentMethodId == 0 ?DBNull.Value:(object)objServiceDonations.PaymentMethodId)),
                    new SqlParameter("@PaymentStatus",(objServiceDonations.PaymentStatus == null ?DBNull.Value:(object)objServiceDonations.PaymentStatus)),
                    new SqlParameter("@PaymentMethod",(objServiceDonations.PaymentMethod == null ?DBNull.Value:(object)objServiceDonations.PaymentMethod)),
                    new SqlParameter("@OrderDate",(objServiceDonations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objServiceDonations.OrderDate)),
                    new SqlParameter("@UpdatedBy",objServiceDonations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",objServiceDonations.UpdatedDate),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceDonationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetServiceDonationsListByVariableId(Int64 ServiceId, Int64 ServiceDonationId, Int64 PaymentMethod, Int64 PaymentStatus, string StartDate, string EndDate, ref decimal SumofDonation, ref Int64 nofofdonors, string Search, string Sort, int PageNo, int Items,Int64 ChapterId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatus",PaymentStatus),
                    new SqlParameter("@PaymentMethod",PaymentMethod),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@StartDate",StartDate),
                    new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@SumofDonation",SumofDonation),
                    new SqlParameter("@nofofdonors",nofofdonors),
                     new SqlParameter("@ChapterId",ChapterId),

                };

                _sqlP[11].SqlDbType = SqlDbType.NVarChar;
                _sqlP[11].Size = 512;
                _sqlP[11].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[12].SqlDbType = SqlDbType.NVarChar;
                _sqlP[12].Size = 512;
                _sqlP[12].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceDonationsGetListByVariableId", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[10].Value);

                SumofDonation = Convert.ToInt64(_sqlP[11].Value);

                nofofdonors = Convert.ToInt64(_sqlP[12].Value);
            }
            catch (Exception ex)
           {
                throw ex;
            }
            return dt;
        }

        public DataTable GetServiceDonationsListByVariable(Int64 ServiceId, Int64 ServiceDonationId, Int64 PaymentMethod, Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                  
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatus",PaymentStatus),
                    new SqlParameter("@PaymentMethod",PaymentMethod),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[8].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceDonationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[8].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet ServiceDonationById(Int64 ServiceDonationId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("ServiceDonationsGetsById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet GetServiceDonationById(Int64 ServiceDonationId, ref int status)
        {
            DataSet ds0 = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds0 = _dbAccess.GetDataSet("ServiceDonationsGetsById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds0;
        }

        public DataSet GetServiceDonationsGetTotalAmount(Int64 ChapterId, ref int status)
        {
            DataSet ds0 = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds0 = _dbAccess.GetDataSet("FEServicesGetTotalAmount", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds0;
        }


        public Int64 DeleteDonor(Int64 ServiceDonationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceDonationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable ServiceDonationsRegistrationsExportToExcel(Int64 ServiceId, Int64 PaymentStatusId, Int64 PaymentMethodId, string StartDate, string EndDate, string Search, string Sort,Int64 chapterId=0)
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatusId",PaymentStatusId),
                    new SqlParameter("@PaymentMethodId",PaymentMethodId),
                    new SqlParameter("@StartDate",StartDate),
                    new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Sort",Sort),
                     new SqlParameter("@chapterId",chapterId),
                   new SqlParameter("@Total",0)
                };
                _sqlP[8].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceDonationsRegistrationsExportToExcel_V1", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[8].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public Int64 ServiceDonationsBulkUpload(string ShipmentsXML, ref string Summery)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new SqlParameter[]
                {
            new SqlParameter("@ShipmentsXML", ShipmentsXML),

            new SqlParameter("@Summery", SqlDbType.NVarChar, 3000000)
            {
                Direction = ParameterDirection.InputOutput,
                Value     = (object)Summery ?? DBNull.Value
            },

            new SqlParameter("@QStatus", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,  // ✅ Must be Output
                Value     = 0
            }
                };

                _dbAccess.SP_ExecuteScalar("ServiceDonationsBulkUpload", ref _sqlP);

                // ✅ Read output values
                Summery = _sqlP[1].Value?.ToString();
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetServiceDonationsList(Int64 ServiceId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                { 
                    new SqlParameter("@ServiceId", ServiceId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceDonationsGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }




        public DataTable GetServiceDonationsLists(string Email,Int64 ChapterId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                     new SqlParameter("@ChapterId", ChapterId),
                     new SqlParameter("@Email",Email),
                      new SqlParameter("@QStatus",0)
                };

                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEDonationOrderListByEmail", ref _sqlP);
                status = Convert.ToInt32(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }













        public DataTable ScholorshipRegistrationsExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ScholorshipRegistrationsExportToExcel", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FivekRegistrationExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportTo5kregistration", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable KidsRegistrationsExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportToKidsRegistrations", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable MembershipChildrenRegistrationExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportTomembership_children", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable PaadutateeyagaRegistrationExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportTopaaduta_teeyaga", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable SambaraluRegistrationExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportTosambaralu_reg", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable SwarabhishekamRegistrationExportToExcel()
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Total",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExportToswarabhishekam", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet GetContributionsById(Int64 ServiceDonationId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceDonationId",ServiceDonationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("GetContributionsById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
         
        public DataTable GetServiceDonarsListByVariable(Int64 ServiceId, Int64 PaymentMethod, Int64 PaymentStatus, string Search, string Sort, int PageNo, int Items, Int64 ChapterId ,ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatus",PaymentStatus),
                    new SqlParameter("@PaymentMethod",PaymentMethod),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total),
                     new SqlParameter("@ChapterId",ChapterId),

                };

                _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("GetServiceDonarsListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[7].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }



        public DataTable FEDonationOrderListByEmail(string Email, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@Email",Email),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEDonationOrderListByEmail", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 BulkUploads(string ShipmentsXML, ref string Summary)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ShipmentsXML",ShipmentsXML),
                    new SqlParameter("@Summary",Summary),
                    new SqlParameter("@QStatus",0)
                };

                _sqlP[1].SqlDbType = SqlDbType.NVarChar;
                _sqlP[1].Size = 3000000;
                _sqlP[1].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DonationBulkImport", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
                Summary = _sqlP[1].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }




    }
}
