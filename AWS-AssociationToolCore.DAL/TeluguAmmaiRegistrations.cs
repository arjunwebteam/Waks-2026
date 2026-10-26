using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
    public class TeluguAmmaiRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 FETeluguAmmaiRegistrationsInsert(Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations, ref Int64 TeluguAmmaiRegistrationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@TeluguAmmaiRegistrationId",objTeluguAmmaiRegistrations.TeluguAmmaiRegistrationId),
                    new SqlParameter("@ChapterId",(objTeluguAmmaiRegistrations.ChapterId==0?(object)DBNull.Value:objTeluguAmmaiRegistrations.ChapterId)),
                    new SqlParameter("@RegestrationID",(objTeluguAmmaiRegistrations.RegestrationID==null?DBNull.Value:(object)objTeluguAmmaiRegistrations.RegestrationID)),
                    new SqlParameter("@FirstName",objTeluguAmmaiRegistrations.FirstName),
                    new SqlParameter("@LastName",objTeluguAmmaiRegistrations.LastName),
                    new SqlParameter("@EmailId",objTeluguAmmaiRegistrations.EmailId),
                    new SqlParameter("@DOB",(objTeluguAmmaiRegistrations.DOB == DateTime.MinValue ?DBNull.Value:(object)objTeluguAmmaiRegistrations.DOB)),
                    new SqlParameter("@ParentFirstName",(objTeluguAmmaiRegistrations.ParentFirstName == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.ParentFirstName)),
                    new SqlParameter("@ParentLastName",(objTeluguAmmaiRegistrations.ParentLastName == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.ParentLastName)),
                    new SqlParameter("@ParentRelation",(objTeluguAmmaiRegistrations.ParentRelation == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.ParentRelation)),
                    new SqlParameter("@ParentEmail",(objTeluguAmmaiRegistrations.ParentEmail == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.ParentEmail)),
                    new SqlParameter("@SchoolGrade",(objTeluguAmmaiRegistrations.SchoolGrade == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.SchoolGrade)),
                    new SqlParameter("@Gender",(objTeluguAmmaiRegistrations.Gender == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.Gender)),
                    new SqlParameter("@PrimaryPhoneNumber",(objTeluguAmmaiRegistrations.PrimaryPhoneNumber == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PrimaryPhoneNumber)),
                    new SqlParameter("@AlternativePhoneNumber",(objTeluguAmmaiRegistrations.AlternativePhoneNumber == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.AlternativePhoneNumber)),
                    new SqlParameter("@Address1",(objTeluguAmmaiRegistrations.Address1 == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.Address1)),
                    new SqlParameter("@Address2",(objTeluguAmmaiRegistrations.Address2 == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.Address2)),
                    new SqlParameter("@City",(objTeluguAmmaiRegistrations.City == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.City)),
                    new SqlParameter("@State",(objTeluguAmmaiRegistrations.State == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.State)),
                    new SqlParameter("@ZipCode",(objTeluguAmmaiRegistrations.ZipCode == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.ZipCode)),
                    new SqlParameter("@CompetitionCategory1",(objTeluguAmmaiRegistrations.CompetitionCategory1 == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.CompetitionCategory1)),
                    new SqlParameter("@CompetitionCategory2",(objTeluguAmmaiRegistrations.CompetitionCategory2 == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.CompetitionCategory2)),
                    new SqlParameter("@TotalAmount",(objTeluguAmmaiRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.TotalAmount)),
                    new SqlParameter("@IsActive",objTeluguAmmaiRegistrations.IsActive),
                    new SqlParameter("@TransactionId",(objTeluguAmmaiRegistrations.TransactionId == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objTeluguAmmaiRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objTeluguAmmaiRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objTeluguAmmaiRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objTeluguAmmaiRegistrations.OrderDate)),
                    new SqlParameter("@PaymentBy",(objTeluguAmmaiRegistrations.PaymentBy == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentBy)),
                    new SqlParameter("@InsertedBy",objTeluguAmmaiRegistrations.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objTeluguAmmaiRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventId",(objTeluguAmmaiRegistrations.EventId==0?(object)DBNull.Value:objTeluguAmmaiRegistrations.EventId)),
                    new SqlParameter("@RegistrationCategoryId",(objTeluguAmmaiRegistrations.RegistrationCategoryId==0?(object)DBNull.Value:objTeluguAmmaiRegistrations.RegistrationCategoryId)),
                    new SqlParameter("@Age",(objTeluguAmmaiRegistrations.Age==0?(object)DBNull.Value:objTeluguAmmaiRegistrations.Age)),
                    new SqlParameter("@MaritalStatu",(objTeluguAmmaiRegistrations.MaritalStatu==null?(object)DBNull.Value:objTeluguAmmaiRegistrations.MaritalStatu)),
                    new SqlParameter("@District",(objTeluguAmmaiRegistrations.District==null?(object)DBNull.Value:objTeluguAmmaiRegistrations.District)),


                    };


                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[33].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("FETeluguAmmaiRegistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[33].Value);

                TeluguAmmaiRegistrationId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateTeluguAmmaiRegistrations(Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@TeluguAmmaiRegistrationId",objTeluguAmmaiRegistrations.TeluguAmmaiRegistrationId),
                    new SqlParameter("@TotalAmount",(objTeluguAmmaiRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.TotalAmount)),
                    new SqlParameter("@TransactionId",(objTeluguAmmaiRegistrations.TransactionId == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objTeluguAmmaiRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objTeluguAmmaiRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentMethodId)),
                    new SqlParameter("@PaymentStatus",(objTeluguAmmaiRegistrations.PaymentStatus == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentStatus)),
                    new SqlParameter("@PaymentMethod",(objTeluguAmmaiRegistrations.PaymentMethod == null ?DBNull.Value:(object)objTeluguAmmaiRegistrations.PaymentMethod)),
                    new SqlParameter("@OrderDate",(objTeluguAmmaiRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objTeluguAmmaiRegistrations.OrderDate)),
                    new SqlParameter("@UpdatedBy",objTeluguAmmaiRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("TeluguAmmaiRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataSet GetTeluguAmmaiRegistrationsGetById(Int64 TeluguAmmaiRegistrationId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@TeluguAmmaiRegistrationId",TeluguAmmaiRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("TeluguAmmaiRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataTable GetTeluguAmmaiRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("TeluguAmmaiRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteTeluguAmmaiRegistrations(Int64 TeluguAmmaiRegistrationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@TeluguAmmaiRegistrationId",TeluguAmmaiRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("TeluguAmmaiRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public DataTable ExportToTeluguAmmayiRegistration(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("ExporttoTeluguAmmayiFinal", ref _sqlP);
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
                dt = _dbAccess.GetDataTable("FETeluguAmmaiRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetTeluguAmmaiDistrictList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("TeluguAmmaiRegistrationsGetDistrictList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetDistrictList(Int64 eid, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@eid",eid)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("GetDistrictList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        #region Telugu

        public Int64 InsertTeluguAmmaiParticipant(AWS_AssociationToolCore.Entities.TeluguAmmaiParticipants objParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@TeluguAmmaiParticipantId",objParticipants.TeluguAmmaiParticipantId),
                    new SqlParameter("@EventId",objParticipants.EventId),
                    new SqlParameter("@TeluguAmmaiRegistrationId",objParticipants.TeluguAmmaiRegistrationId),
                    new SqlParameter("@FirstName",objParticipants.FirstName),
                    new SqlParameter("@LastName",objParticipants.LastName),
                    new SqlParameter("@Email",(objParticipants.Email==null?(object)DBNull.Value:objParticipants.Email)),
                    new SqlParameter("@Age",(objParticipants.Age==null?(object)DBNull.Value:objParticipants.Age)),
                    new SqlParameter("@Mobile",(objParticipants.Mobile==null?(object)DBNull.Value:objParticipants.Mobile)),
                    new SqlParameter("@IsApproved",objParticipants.IsApproved),
                    new SqlParameter("@UpdatedBy",objParticipants.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("TeluguAmmaiParticipantsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(AWS_AssociationToolCore.Entities.TeluguAmmaiParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@TeluguAmmaiParticipantId",objKidsParticipants.TeluguAmmaiParticipantId),
                    new SqlParameter("@EventId",objKidsParticipants.EventId),
                    new SqlParameter("@TeluguAmmaiRegistrationId",objKidsParticipants.TeluguAmmaiRegistrationId),
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
                _dbAccess.SP_ExecuteScalar("TeluguAmmaiParticipantsUpdate", ref _sqlP);
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
