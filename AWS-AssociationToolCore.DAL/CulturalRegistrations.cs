using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
    public class CulturalRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;


        public Int64 InsertCulturalRegistrations(Entities.CulturalRegistrations objCulturalRegistrations, ref Int64 CulturalRegistrationId,ref string receipturl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@CulturalRegistrationId",objCulturalRegistrations.CulturalRegistrationId),
                    new SqlParameter("@ChapterId",objCulturalRegistrations.ChapterId),
                    new SqlParameter("@FirstName",objCulturalRegistrations.FirstName),
                    new SqlParameter("@LastName",objCulturalRegistrations.LastName),
                    new SqlParameter("@Address1",(objCulturalRegistrations.Address1 == null ?DBNull.Value:(object)objCulturalRegistrations.Address1)),
                    new SqlParameter("@Address2",(objCulturalRegistrations.Address2 == null ?DBNull.Value:(object)objCulturalRegistrations.Address2)),
                    new SqlParameter("@City",(objCulturalRegistrations.City == null ?DBNull.Value:(object)objCulturalRegistrations.City)),
                    new SqlParameter("@State",(objCulturalRegistrations.State == null ?DBNull.Value:(object)objCulturalRegistrations.State)),
                    new SqlParameter("@ZipCode",(objCulturalRegistrations.ZipCode == null ?DBNull.Value:(object)objCulturalRegistrations.ZipCode)),
                    new SqlParameter("@Email",(objCulturalRegistrations.Email == null ?DBNull.Value:(object)objCulturalRegistrations.Email)),
                    new SqlParameter("@PhoneNo",(objCulturalRegistrations.PhoneNo == null ?DBNull.Value:(object)objCulturalRegistrations.PhoneNo)),
                    new SqlParameter("@AAALifetimeMember",(objCulturalRegistrations.AAALifetimeMember == null ?DBNull.Value:(object)objCulturalRegistrations.AAALifetimeMember)),
                    new SqlParameter("@CoordinatorName",(objCulturalRegistrations.CoordinatorName == null ?DBNull.Value:(object)objCulturalRegistrations.CoordinatorName)),
                    new SqlParameter("@ContactPhone",(objCulturalRegistrations.ContactPhone == null ?DBNull.Value:(object)objCulturalRegistrations.ContactPhone)),
                    new SqlParameter("@ContactEmail",(objCulturalRegistrations.ContactEmail == null ?DBNull.Value:(object)objCulturalRegistrations.ContactEmail)),
                    new SqlParameter("@OrganizationName",(objCulturalRegistrations.OrganizationName == null ?DBNull.Value:(object)objCulturalRegistrations.OrganizationName)),
                    new SqlParameter("@PerformanceCategory",(objCulturalRegistrations.PerformanceCategory == null ?DBNull.Value:(object)objCulturalRegistrations.PerformanceCategory)),
                    new SqlParameter("@SpecifyCategory",(objCulturalRegistrations.SpecifyCategory == null ?DBNull.Value:(object)objCulturalRegistrations.SpecifyCategory)),
                    new SqlParameter("@NameofthePerformance",(objCulturalRegistrations.NameofthePerformance == null ?DBNull.Value:(object)objCulturalRegistrations.NameofthePerformance)),
                    new SqlParameter("@PerformanceDuration",(objCulturalRegistrations.PerformanceDuration == null ?DBNull.Value:(object)objCulturalRegistrations.PerformanceDuration)),
                    new SqlParameter("@NumberofParticipants",(objCulturalRegistrations.NumberofParticipants == null ?DBNull.Value:(object)objCulturalRegistrations.NumberofParticipants)),
                    new SqlParameter("@ParticipantAgeGroup",(objCulturalRegistrations.ParticipantAgeGroup == null ?DBNull.Value:(object)objCulturalRegistrations.ParticipantAgeGroup)),
                    new SqlParameter("@SpecialRequirements",(objCulturalRegistrations.SpecialRequirements == null ?DBNull.Value:(object)objCulturalRegistrations.SpecialRequirements)),
                    new SqlParameter("@BriefDescriptionofthePerformance",(objCulturalRegistrations.BriefDescriptionofthePerformance == null ?DBNull.Value:(object)objCulturalRegistrations.BriefDescriptionofthePerformance)),
                    new SqlParameter("@PaymentStatusId",(objCulturalRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objCulturalRegistrations.PaymentStatusId)),
                    //new SqlParameter("@PaymentDate",(objCulturalRegistrations.PaymentDate == null ?DBNull.Value:(object)objCulturalRegistrations.PaymentDate)),
                    new SqlParameter("@PaymentDate",DateTime.UtcNow),
                    new SqlParameter("@PaymentMethodId",(objCulturalRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objCulturalRegistrations.PaymentMethodId)),
                    new SqlParameter("@OtherPaymentId",(objCulturalRegistrations.OtherPaymentId == null ?DBNull.Value:(object)objCulturalRegistrations.OtherPaymentId)),
                    new SqlParameter("@TransactionId",(objCulturalRegistrations.TransactionId == null ?DBNull.Value:(object)objCulturalRegistrations.TransactionId)),
                    new SqlParameter("@AmountPaid",(objCulturalRegistrations.AmountPaid == 0 ?DBNull.Value:(object)objCulturalRegistrations.AmountPaid)),
                    new SqlParameter("@Zelle",(objCulturalRegistrations.Zelle == null ?DBNull.Value:(object)objCulturalRegistrations.Zelle)),
                    //new SqlParameter("@ReceiptUrl",(objCulturalRegistrations.ReceiptUrl == null ?DBNull.Value:(object)objCulturalRegistrations.ReceiptUrl)),
                    new SqlParameter("@ReceiptUrl",receipturl),
                    new SqlParameter("@ZellePartnerName",(objCulturalRegistrations.ZellePartnerName == null ?DBNull.Value:(object)objCulturalRegistrations.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objCulturalRegistrations.ChequeHolderName == null ?DBNull.Value:(object)objCulturalRegistrations.ChequeHolderName)),
                    new SqlParameter("@AAAInstagram",(objCulturalRegistrations.AAAInstagram == null ?DBNull.Value:(object)objCulturalRegistrations.AAAInstagram)),
                    new SqlParameter("@IsActive",(objCulturalRegistrations.IsActive == false ?DBNull.Value:(object)objCulturalRegistrations.IsActive)),
                    new SqlParameter("@EventId",(objCulturalRegistrations.EventId == 0 ?DBNull.Value:(object)objCulturalRegistrations.EventId)),
                    new SqlParameter("@InsertedBy",objCulturalRegistrations.InsertedBy),
                    new SqlParameter("@InsertedDate",objCulturalRegistrations.InsertedDate),
                    new SqlParameter("@UpdatedBy",objCulturalRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",objCulturalRegistrations.UpdatedDate),
                    new SqlParameter("@QStatus",0)
                    };

               
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;


                _sqlP[31].SqlDbType = SqlDbType.NVarChar;
                _sqlP[31].Size = 256;
                _sqlP[31].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[41].Direction = System.Data.ParameterDirection.Output;

                _dbAccess.SP_ExecuteScalar("CulturalRegistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[41].Value);
                CulturalRegistrationId = Convert.ToInt64(_sqlP[0].Value);
                receipturl = _sqlP[31].Value.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetCulturalRegistrationsListByVariable(string State, Int64 EventId, string Search, string Sort, int PageNo, int Items, Int64 ChapterId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
            new SqlParameter("@State",     State),
            new SqlParameter("@EventId",   EventId),   // ✅ added
            new SqlParameter("@Search",    Search),
            new SqlParameter("@Sort",      Sort),
            new SqlParameter("@PageNo",    PageNo),
            new SqlParameter("@Items",     Items),
            new SqlParameter("@ChapterId", ChapterId),
            new SqlParameter("@Total",     Total)      // ✅ index 7 now
        };
                _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CulturalRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[7].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetCulturalRegistrationsById(Int64 CulturalRegistrationId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CulturalRegistrationId",CulturalRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CulturalRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteCulturalRegistrations(Int64 CulturalRegistrationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CulturalRegistrationId",CulturalRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CulturalRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        public DataTable CulturalRegistrationsExportToExcel( string Search, string Sort,Int64 ChapterId=0)
        {
            DataTable dt = null;
            int Total = 0;
            try
            {
                _sqlP = new[]
                {

                    
                    new SqlParameter("@Search",Search),
                     new SqlParameter("@Sort",Sort),
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CulturalRegistrationsExportToExcel", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 InsertCulturalParticipants(AWS_AssociationToolCore.Entities.CulturalParticipants objCulturalParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ParticipantID",objCulturalParticipants.ParticipantID),
                    new SqlParameter("@CulturalRegistrationId",(objCulturalParticipants.CulturalRegistrationId==0?(object)DBNull.Value:objCulturalParticipants.CulturalRegistrationId)),
                    new SqlParameter("@FirstName",(objCulturalParticipants.FirstName==null?(object)DBNull.Value:objCulturalParticipants.FirstName)),
                    new SqlParameter("@LastName",(objCulturalParticipants.LastName==null?(object)DBNull.Value:objCulturalParticipants.LastName)),
                    new SqlParameter("@Email",(objCulturalParticipants.Email==null?(object)DBNull.Value:objCulturalParticipants.Email)),
                    new SqlParameter("@PhoneNumber",(objCulturalParticipants.PhoneNumber==null?(object)DBNull.Value:objCulturalParticipants.PhoneNumber)),
                    new SqlParameter("@Age",(objCulturalParticipants.Age==0?(object)DBNull.Value:objCulturalParticipants.Age)),
                    new SqlParameter("@Gender",(objCulturalParticipants.Gender==null?(object)DBNull.Value:objCulturalParticipants.Gender)),
                    new SqlParameter("@Address",(objCulturalParticipants.Address==null?(object)DBNull.Value:objCulturalParticipants.Address)),
                    new SqlParameter("@InsertedBy",(objCulturalParticipants.InsertedBy==null?(object)DBNull.Value:objCulturalParticipants.InsertedBy)),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",(objCulturalParticipants.UpdatedBy==null?(object)DBNull.Value:objCulturalParticipants.UpdatedBy)),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };
                _sqlP[13].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CulturalParticipantsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[13].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateCulturalRegistrationsPaymentInfo(AWS_AssociationToolCore.Entities.CulturalRegistrations objCulturalRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@CulturalRegistrationId",objCulturalRegistrations.CulturalRegistrationId),
                    new SqlParameter("@PaymentStatusId",(objCulturalRegistrations.PaymentStatusId==0?(object)DBNull.Value:objCulturalRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objCulturalRegistrations.PaymentMethodId==0?(object)DBNull.Value:objCulturalRegistrations.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objCulturalRegistrations.TransactionId==null?(object)DBNull.Value:objCulturalRegistrations.TransactionId )),
                    //new SqlParameter("@AmountPaid",(objCulturalRegistrations.AmountPaid==0?(object)DBNull.Value:objCulturalRegistrations.AmountPaid )),
                    new SqlParameter("@PaymentDate",objCulturalRegistrations.PaymentDate),
                    new SqlParameter("@QStatus",0),
                    };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("UpdateCulturalRegistrationsPaymentInfo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public Int64 UpdateCulturalRegistrations(Entities.CulturalRegistrations objCulturalRegistrations, ref string receipturl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@CulturalRegistrationId",objCulturalRegistrations.CulturalRegistrationId),
                    new SqlParameter("@FirstName",objCulturalRegistrations.FirstName),
                    new SqlParameter("@LastName",objCulturalRegistrations.LastName),
                    new SqlParameter("@Address1",(objCulturalRegistrations.Address1 == null ?DBNull.Value:(object)objCulturalRegistrations.Address1)),
                    new SqlParameter("@Address2",(objCulturalRegistrations.Address2 == null ?DBNull.Value:(object)objCulturalRegistrations.Address2)),

                    new SqlParameter("@City",(objCulturalRegistrations.City == null ?DBNull.Value:(object)objCulturalRegistrations.City)),
                    new SqlParameter("@State",(objCulturalRegistrations.State == null ?DBNull.Value:(object)objCulturalRegistrations.State)),
                    new SqlParameter("@ZipCode",(objCulturalRegistrations.ZipCode == null ?DBNull.Value:(object)objCulturalRegistrations.ZipCode)),
                    new SqlParameter("@Email",(objCulturalRegistrations.Email == null ?DBNull.Value:(object)objCulturalRegistrations.Email)),
                    new SqlParameter("@PhoneNo",(objCulturalRegistrations.PhoneNo == null ?DBNull.Value:(object)objCulturalRegistrations.PhoneNo)),

                    new SqlParameter("@PaymentStatusId",(objCulturalRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objCulturalRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objCulturalRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objCulturalRegistrations.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objCulturalRegistrations.TransactionId == null ?DBNull.Value:(object)objCulturalRegistrations.TransactionId)),
                    new SqlParameter("@AmountPaid",(objCulturalRegistrations.AmountPaid == 0 ?DBNull.Value:(object)objCulturalRegistrations.AmountPaid)),
                    new SqlParameter("@UpdatedBy",objCulturalRegistrations.UpdatedBy),

                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@ReceiptUrl",receipturl),
                    new SqlParameter("@ZellePartnerName",(objCulturalRegistrations.ZellePartnerName  == null?DBNull.Value: (object)objCulturalRegistrations.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objCulturalRegistrations.ChequeHolderName  == null?DBNull.Value: (object)objCulturalRegistrations.ChequeHolderName)),
                     new SqlParameter("@Zelle",(objCulturalRegistrations.Zelle  == null?DBNull.Value: (object)objCulturalRegistrations.Zelle)),

                    new SqlParameter("@EventId",(objCulturalRegistrations.EventId == 0 ?DBNull.Value:(object)objCulturalRegistrations.EventId)),
                    new SqlParameter("@QStatus",0),
                    };

                _sqlP[16].SqlDbType = SqlDbType.NVarChar;
                _sqlP[16].Size = 256;
                _sqlP[16].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[21].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CulturalRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[21].Value);

                receipturl = _sqlP[16].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataSet GetCulturalRegistrationsFullDetailsById(Int64 CulturalRegistrationId, ref int Status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CulturalRegistrationId",CulturalRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("CulturalRegistrationsFullDetailsGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

    }
}
