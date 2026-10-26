using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class KidsRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 FEKidsRegistrationsInsert(Entities.KidsRegistrations objKidsRegistrations, ref Int64 KidsRegistrationsId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@KidsRegistrationsId",objKidsRegistrations.KidsRegistrationsId),
                    new SqlParameter("@ChapterId",(objKidsRegistrations.ChapterId==0?(object)DBNull.Value:objKidsRegistrations.ChapterId)),
                    new SqlParameter("@RegestrationID",(objKidsRegistrations.RegestrationID==null?DBNull.Value:(object)objKidsRegistrations.RegestrationID)),
                    new SqlParameter("@FirstName",objKidsRegistrations.FirstName),
                    new SqlParameter("@LastName",objKidsRegistrations.LastName),
                    new SqlParameter("@EmailId",objKidsRegistrations.EmailId),
                    new SqlParameter("@DOB",(objKidsRegistrations.DOB == DateTime.MinValue ?DBNull.Value:(object)objKidsRegistrations.DOB)),
                    new SqlParameter("@ParentFirstName",(objKidsRegistrations.ParentFirstName == null ?DBNull.Value:(object)objKidsRegistrations.ParentFirstName)),
                    new SqlParameter("@ParentLastName",(objKidsRegistrations.ParentLastName == null ?DBNull.Value:(object)objKidsRegistrations.ParentLastName)),
                    new SqlParameter("@ParentRelation",(objKidsRegistrations.ParentRelation == null ?DBNull.Value:(object)objKidsRegistrations.ParentRelation)),
                    new SqlParameter("@ParentEmail",(objKidsRegistrations.ParentEmail == null ?DBNull.Value:(object)objKidsRegistrations.ParentEmail)),
                    new SqlParameter("@SchoolGrade",(objKidsRegistrations.SchoolGrade == null ?DBNull.Value:(object)objKidsRegistrations.SchoolGrade)),
                    new SqlParameter("@Gender",(objKidsRegistrations.Gender == null ?DBNull.Value:(object)objKidsRegistrations.Gender)),
                    new SqlParameter("@PrimaryPhoneNumber",(objKidsRegistrations.PrimaryPhoneNumber == null ?DBNull.Value:(object)objKidsRegistrations.PrimaryPhoneNumber)),
                    new SqlParameter("@AlternativePhoneNumber",(objKidsRegistrations.AlternativePhoneNumber == null ?DBNull.Value:(object)objKidsRegistrations.AlternativePhoneNumber)),
                    new SqlParameter("@Address1",(objKidsRegistrations.Address1 == null ?DBNull.Value:(object)objKidsRegistrations.Address1)),
                    new SqlParameter("@Address2",(objKidsRegistrations.Address2 == null ?DBNull.Value:(object)objKidsRegistrations.Address2)),
                    new SqlParameter("@City",(objKidsRegistrations.City == null ?DBNull.Value:(object)objKidsRegistrations.City)),
                    new SqlParameter("@State",(objKidsRegistrations.State == null ?DBNull.Value:(object)objKidsRegistrations.State)),
                    new SqlParameter("@ZipCode",(objKidsRegistrations.ZipCode == null ?DBNull.Value:(object)objKidsRegistrations.ZipCode)),
                    new SqlParameter("@CompetitionCategory1",(objKidsRegistrations.CompetitionCategory1 == null ?DBNull.Value:(object)objKidsRegistrations.CompetitionCategory1)),
                    new SqlParameter("@CompetitionCategory2",(objKidsRegistrations.CompetitionCategory2 == null ?DBNull.Value:(object)objKidsRegistrations.CompetitionCategory2)),
                    new SqlParameter("@TotalAmount",(objKidsRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objKidsRegistrations.TotalAmount)),
                    new SqlParameter("@IsActive",objKidsRegistrations.IsActive),                    
                    new SqlParameter("@TransactionId",(objKidsRegistrations.TransactionId == null ?DBNull.Value:(object)objKidsRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objKidsRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objKidsRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objKidsRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objKidsRegistrations.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objKidsRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objKidsRegistrations.OrderDate)),
                    new SqlParameter("@PaymentBy",(objKidsRegistrations.PaymentBy == null ?DBNull.Value:(object)objKidsRegistrations.PaymentBy)),                    
                    new SqlParameter("@InsertedBy",objKidsRegistrations.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objKidsRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventId",(objKidsRegistrations.EventId==0?(object)DBNull.Value:objKidsRegistrations.EventId)),
                    new SqlParameter("@RegistrationCategoryId",(objKidsRegistrations.RegistrationCategoryId==0?(object)DBNull.Value:objKidsRegistrations.RegistrationCategoryId)),                    
                  
                    
                    };


                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[33].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("FEKidsRegistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[33].Value);

                KidsRegistrationsId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateKidsRegistrations(Entities.KidsRegistrations objKidsRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@KidsRegistrationsId",objKidsRegistrations.KidsRegistrationsId),
                    new SqlParameter("@TotalAmount",(objKidsRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objKidsRegistrations.TotalAmount)),
                    new SqlParameter("@TransactionId",(objKidsRegistrations.TransactionId == null ?DBNull.Value:(object)objKidsRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objKidsRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objKidsRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objKidsRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objKidsRegistrations.PaymentMethodId)),
                    new SqlParameter("@PaymentStatus",(objKidsRegistrations.PaymentStatus == null ?DBNull.Value:(object)objKidsRegistrations.PaymentStatus)),
                    new SqlParameter("@PaymentMethod",(objKidsRegistrations.PaymentMethod == null ?DBNull.Value:(object)objKidsRegistrations.PaymentMethod)),
                    new SqlParameter("@OrderDate",(objKidsRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objKidsRegistrations.OrderDate)),
                    new SqlParameter("@UpdatedBy",objKidsRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("KidsRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataSet GetKidsRegistrationsGetById(Int64 KidsRegistrationsId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@KidsRegistrationsId",KidsRegistrationsId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("KidsRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataTable GetKidsRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@ChapterId",ChapterId),                    
                    new SqlParameter("@Search",Search),                    
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),                    
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@RegistrationCategoryId",RegistrationCategoryId),
                    new SqlParameter("@PaymentStatusId",PaymentStatusId),
                    new SqlParameter("@CompetitionCategory1",CompetitionCategory1)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("KidsRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteKidsRegistrations(Int64 KidsRegistrationsId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@KidsRegistrationsId",KidsRegistrationsId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("KidsRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public DataTable ExportToKidsRegistrations(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@ChapterId",ChapterId),                    
                    new SqlParameter("@Search",Search),                    
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),                    
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@RegistrationCategoryId",RegistrationCategoryId),
                     new SqlParameter("@PaymentStatusId",PaymentStatusId),
                    new SqlParameter("@CompetitionCategory1",CompetitionCategory1)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExporttoKidsRegistrationsFinal", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #region FE

        public DataTable FEGetKidsRegistrationsList(string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                     new SqlParameter("@Search",Search),
                    new SqlParameter("@Email",Email),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEKidsRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        #region KidsParticipants

        public Int64 InsertBalalaParticipant(AWS_AssociationToolCore.Entities.KidsParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@KidsParticipantId",objKidsParticipants.KidsParticipantId),
                    new SqlParameter("@EventId",objKidsParticipants.EventId),
                    new SqlParameter("@KidsRegistrationsId",objKidsParticipants.KidsRegistrationsId ),
                    new SqlParameter("@FirstName",objKidsParticipants.FirstName),
                    new SqlParameter("@LastName",objKidsParticipants.LastName),
                    new SqlParameter("@Email",(objKidsParticipants.Email==null?(object)DBNull.Value:objKidsParticipants.Email)),
                    new SqlParameter("@Age",(objKidsParticipants.Age==null?(object)DBNull.Value:objKidsParticipants.Age)),
                    new SqlParameter("@Mobile",(objKidsParticipants.Mobile==null?(object)DBNull.Value:objKidsParticipants.Mobile)),
                    new SqlParameter("@IsApproved",objKidsParticipants.IsApproved),
                    new SqlParameter("@UpdatedBy",objKidsParticipants.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("KidsParticipantsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(AWS_AssociationToolCore.Entities.KidsParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@KidsParticipantId",objKidsParticipants.KidsParticipantId),
                    new SqlParameter("@EventId",objKidsParticipants.EventId),
                    new SqlParameter("@KidsRegistrationsId",objKidsParticipants.KidsRegistrationsId),
                    new SqlParameter("@FirstName",objKidsParticipants.FirstName),
                    new SqlParameter("@LastName",objKidsParticipants.LastName),
                    new SqlParameter("@Email",objKidsParticipants.Email),
                    new SqlParameter("@Age",(objKidsParticipants.Age==null?(object)DBNull.Value:objKidsParticipants.Age)),
                    new SqlParameter("@Mobile",(objKidsParticipants.Mobile==null?(object)DBNull.Value:objKidsParticipants.Mobile)),
                    new SqlParameter("@IsApproved",objKidsParticipants.IsApproved),
                    new SqlParameter("@UpdatedBy",objKidsParticipants.UpdatedBy),
                    new SqlParameter("@UpdatedTime",objKidsParticipants.UpdatedTime),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("KidsParticipantsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

    }
}
