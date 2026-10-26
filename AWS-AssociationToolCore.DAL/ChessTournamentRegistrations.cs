using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
    public class ChessTournamentRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 FEChessTournamentRegistrationsInsert(Entities.ChessTournamentRegistrations objChessTournamentRegistrations, ref Int64 ChessTournamentRegistrationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ChessTournamentRegistrationId",objChessTournamentRegistrations.ChessTournamentRegistrationId),
                    new SqlParameter("@ChapterId",(objChessTournamentRegistrations.ChapterId==0?(object)DBNull.Value:objChessTournamentRegistrations.ChapterId)),
                    new SqlParameter("@RegestrationID",(objChessTournamentRegistrations.RegestrationID==null?DBNull.Value:(object)objChessTournamentRegistrations.RegestrationID)),
                    new SqlParameter("@FirstName",objChessTournamentRegistrations.FirstName),
                    new SqlParameter("@LastName",objChessTournamentRegistrations.LastName),
                    new SqlParameter("@EmailId",objChessTournamentRegistrations.EmailId),
                    new SqlParameter("@DOB",(objChessTournamentRegistrations.DOB == DateTime.MinValue ?DBNull.Value:(object)objChessTournamentRegistrations.DOB)),
                    new SqlParameter("@ParentFirstName",(objChessTournamentRegistrations.ParentFirstName == null ?DBNull.Value:(object)objChessTournamentRegistrations.ParentFirstName)),
                    new SqlParameter("@ParentLastName",(objChessTournamentRegistrations.ParentLastName == null ?DBNull.Value:(object)objChessTournamentRegistrations.ParentLastName)),
                    new SqlParameter("@ParentRelation",(objChessTournamentRegistrations.ParentRelation == null ?DBNull.Value:(object)objChessTournamentRegistrations.ParentRelation)),
                    new SqlParameter("@ParentEmail",(objChessTournamentRegistrations.ParentEmail == null ?DBNull.Value:(object)objChessTournamentRegistrations.ParentEmail)),
                    new SqlParameter("@SchoolGrade",(objChessTournamentRegistrations.SchoolGrade == null ?DBNull.Value:(object)objChessTournamentRegistrations.SchoolGrade)),
                    new SqlParameter("@Gender",(objChessTournamentRegistrations.Gender == null ?DBNull.Value:(object)objChessTournamentRegistrations.Gender)),
                    new SqlParameter("@PrimaryPhoneNumber",(objChessTournamentRegistrations.PrimaryPhoneNumber == null ?DBNull.Value:(object)objChessTournamentRegistrations.PrimaryPhoneNumber)),
                    new SqlParameter("@AlternativePhoneNumber",(objChessTournamentRegistrations.AlternativePhoneNumber == null ?DBNull.Value:(object)objChessTournamentRegistrations.AlternativePhoneNumber)),
                    new SqlParameter("@Address1",(objChessTournamentRegistrations.Address1 == null ?DBNull.Value:(object)objChessTournamentRegistrations.Address1)),
                    new SqlParameter("@Address2",(objChessTournamentRegistrations.Address2 == null ?DBNull.Value:(object)objChessTournamentRegistrations.Address2)),
                    new SqlParameter("@City",(objChessTournamentRegistrations.City == null ?DBNull.Value:(object)objChessTournamentRegistrations.City)),
                    new SqlParameter("@State",(objChessTournamentRegistrations.State == null ?DBNull.Value:(object)objChessTournamentRegistrations.State)),
                    new SqlParameter("@ZipCode",(objChessTournamentRegistrations.ZipCode == null ?DBNull.Value:(object)objChessTournamentRegistrations.ZipCode)),
                    new SqlParameter("@CompetitionCategory1",(objChessTournamentRegistrations.CompetitionCategory1 == null ?DBNull.Value:(object)objChessTournamentRegistrations.CompetitionCategory1)),
                    new SqlParameter("@CompetitionCategory2",(objChessTournamentRegistrations.CompetitionCategory2 == null ?DBNull.Value:(object)objChessTournamentRegistrations.CompetitionCategory2)),
                    new SqlParameter("@TotalAmount",(objChessTournamentRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.TotalAmount)),
                    new SqlParameter("@IsActive",objChessTournamentRegistrations.IsActive),
                    new SqlParameter("@TransactionId",(objChessTournamentRegistrations.TransactionId == null ?DBNull.Value:(object)objChessTournamentRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objChessTournamentRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objChessTournamentRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentMethodId)),
                    new SqlParameter("@OrderDate",(objChessTournamentRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objChessTournamentRegistrations.OrderDate)),
                    new SqlParameter("@PaymentBy",(objChessTournamentRegistrations.PaymentBy == null ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentBy)),
                    new SqlParameter("@InsertedBy",objChessTournamentRegistrations.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objChessTournamentRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventId",(objChessTournamentRegistrations.EventId==0?(object)DBNull.Value:objChessTournamentRegistrations.EventId)),
                    new SqlParameter("@RegistrationCategoryId",(objChessTournamentRegistrations.RegistrationCategoryId==0?(object)DBNull.Value:objChessTournamentRegistrations.RegistrationCategoryId)),
                    new SqlParameter("@Age",(objChessTournamentRegistrations.Age==0?(object)DBNull.Value:objChessTournamentRegistrations.Age)),
                    new SqlParameter("@MaritalStatu",(objChessTournamentRegistrations.MaritalStatu==null?(object)DBNull.Value:objChessTournamentRegistrations.MaritalStatu)),
                    new SqlParameter("@District",(objChessTournamentRegistrations.District==null?(object)DBNull.Value:objChessTournamentRegistrations.District)),


                    };


                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[33].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("FEChessTournamentRegistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[33].Value);

                ChessTournamentRegistrationId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateChessTournamentRegistrations(Entities.ChessTournamentRegistrations objChessTournamentRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ChessTournamentRegistrationId",objChessTournamentRegistrations.ChessTournamentRegistrationId),
                    new SqlParameter("@TotalAmount",(objChessTournamentRegistrations.TotalAmount == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.TotalAmount)),
                    new SqlParameter("@TransactionId",(objChessTournamentRegistrations.TransactionId == null ?DBNull.Value:(object)objChessTournamentRegistrations.TransactionId)),
                    new SqlParameter("@PaymentStatusId",(objChessTournamentRegistrations.PaymentStatusId == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objChessTournamentRegistrations.PaymentMethodId == 0 ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentMethodId)),
                    new SqlParameter("@PaymentStatus",(objChessTournamentRegistrations.PaymentStatus == null ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentStatus)),
                    new SqlParameter("@PaymentMethod",(objChessTournamentRegistrations.PaymentMethod == null ?DBNull.Value:(object)objChessTournamentRegistrations.PaymentMethod)),
                    new SqlParameter("@OrderDate",(objChessTournamentRegistrations.OrderDate == DateTime.MinValue ?DBNull.Value:(object)objChessTournamentRegistrations.OrderDate)),
                    new SqlParameter("@UpdatedBy",objChessTournamentRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ChessTournamentRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataSet GetChessTournamentRegistrationsGetById(Int64 ChessTournamentRegistrationId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChessTournamentRegistrationId",ChessTournamentRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("ChessTournamentRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataTable GetChessTournamentRegistrationsListByVariableId(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("ChessTournamentRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteChessTournamentRegistrations(Int64 ChessTournamentRegistrationId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChessTournamentRegistrationId",ChessTournamentRegistrationId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ChessTournamentRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public DataTable ExportToChessTournamentRegistration(Int64 EventId, Int64 RegistrationCategoryId, Int64 ChapterId, Int64 PaymentStatusId, string CompetitionCategory1, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("ExporttoChessTournamentFinal", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #region FE

        public DataTable FEGetChessTournamentList(string Email, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("FEChessTournamentRegistrationsGetListByVariable", ref _sqlP);
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
                dt = _dbAccess.GetDataTable("ChessTournamentRegistrationsGetDistrictList", ref _sqlP);
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
                dt = _dbAccess.GetDataTable("GetChessDistrictList", ref _sqlP);
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

        public Int64 InsertChessTournamentParticipants(AWS_AssociationToolCore.Entities.ChessTournamentParticipants objParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ChessTournamentParticipantId",objParticipants.ChessTournamentParticipantId),
                    new SqlParameter("@EventId",objParticipants.EventId),
                    new SqlParameter("@ChessTournamentRegistrationId",objParticipants.ChessTournamentRegistrationId),
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
                _dbAccess.SP_ExecuteScalar("ChessTournamentParticipantsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(AWS_AssociationToolCore.Entities.ChessTournamentParticipants objKidsParticipants)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ChessTournamentParticipantId",objKidsParticipants.ChessTournamentParticipantId),
                    new SqlParameter("@EventId",objKidsParticipants.EventId),
                    new SqlParameter("@ChessTournamentRegistrationId",objKidsParticipants.ChessTournamentRegistrationId),
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
                _dbAccess.SP_ExecuteScalar("ChessTournamentParticipantsUpdate", ref _sqlP);
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
