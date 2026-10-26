using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class ExhibitsRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Admin

        public Int64 InsertExhibitsRegistrations(Entities.ExhibitsRegistrations objExhibitsRegistrations, ref Int64 ExhibitId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ExhibitId",0),
                    new SqlParameter("@FirstName",objExhibitsRegistrations.FirstName),
                    new SqlParameter("@LastName",objExhibitsRegistrations.LastName),
                    new SqlParameter("@Address1",(objExhibitsRegistrations.Address1 == null ?DBNull.Value:(object)objExhibitsRegistrations.Address1)),
                    new SqlParameter("@Address2",(objExhibitsRegistrations.Address2 == null ?DBNull.Value:(object)objExhibitsRegistrations.Address2)),
                    new SqlParameter("@Phone",objExhibitsRegistrations.Phone), 
                    new SqlParameter("@Email",objExhibitsRegistrations.Email),
                    new SqlParameter("@City",(objExhibitsRegistrations.City == null ?DBNull.Value:(object)objExhibitsRegistrations.City)),
                    new SqlParameter("@State",(objExhibitsRegistrations.State == null ?DBNull.Value:(object)objExhibitsRegistrations.State)),
                    new SqlParameter("@Zip",(objExhibitsRegistrations.Zip == null ?DBNull.Value:(object)objExhibitsRegistrations.Zip)),
                    new SqlParameter("@NoOfBooths",(objExhibitsRegistrations.NoOfBooths == 0 ?DBNull.Value:(object)objExhibitsRegistrations.NoOfBooths)),
                    new SqlParameter("@SigeOfBooth",(objExhibitsRegistrations.SigeOfBooth == null ?DBNull.Value:(object)objExhibitsRegistrations.SigeOfBooth)),
                    new SqlParameter("@BusinessName",(objExhibitsRegistrations.BusinessName == null ?DBNull.Value:(object)objExhibitsRegistrations.BusinessName)), 
                    new SqlParameter("@Category",(objExhibitsRegistrations.Category == null ?DBNull.Value:(object)objExhibitsRegistrations.Category)), 
                    new SqlParameter("@Amount",(objExhibitsRegistrations.Amount == 0 ?DBNull.Value:(object)objExhibitsRegistrations.Amount)),
                    new SqlParameter("@IsActive",objExhibitsRegistrations.IsActive),
                    new SqlParameter("@OrderNo",(objExhibitsRegistrations.OrderNo == 0 ?DBNull.Value:(object)objExhibitsRegistrations.OrderNo)),  
                    new SqlParameter("@InsertedBy",(objExhibitsRegistrations.InsertedBy == null ?DBNull.Value:(object)objExhibitsRegistrations.InsertedBy)),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objExhibitsRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    //new SqlParameter("@PromoCode",(objExhibitsRegistrations.PromoCode == null ?DBNull.Value:(object)objExhibitsRegistrations.PromoCode)),


                    new SqlParameter("@QStatus",0)
                    };

                _sqlP[0].Direction = _sqlP[21].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ExhibitsRegistrationInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[21].Value);

                ExhibitId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 AdminExhibitsRegistrationsUpdate(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ExhibitId",objExhibitsRegistrations.ExhibitId),
                     
                    new SqlParameter("@AmountPaid",(objExhibitsRegistrations.AmountPaid == 0 ?DBNull.Value:(object)objExhibitsRegistrations.AmountPaid)), 
                    new SqlParameter("@AmountDue",(objExhibitsRegistrations.AmountDue == 0 ?DBNull.Value:(object)objExhibitsRegistrations.AmountDue)), 
                    new SqlParameter("@TransactionId",((objExhibitsRegistrations.TransactionId == "" || objExhibitsRegistrations.TransactionId == null) ?DBNull.Value:(object)objExhibitsRegistrations.TransactionId)), 
                    new SqlParameter("@PaymentStatus",((objExhibitsRegistrations.PaymentStatus == "" || objExhibitsRegistrations.PaymentStatus == null) ?DBNull.Value:(object)objExhibitsRegistrations.PaymentStatus)),
                    new SqlParameter("@PaymentBy",((objExhibitsRegistrations.PaymentBy == "" || objExhibitsRegistrations.PaymentBy == null) ?DBNull.Value:(object)objExhibitsRegistrations.PaymentBy)),
                    new SqlParameter("@PaymentDate",((objExhibitsRegistrations.PaymentDate == DateTime.MinValue || objExhibitsRegistrations.PaymentDate == null) ?DBNull.Value:(object)objExhibitsRegistrations.PaymentDate)), 
                    new SqlParameter("@UpdatedBy",objExhibitsRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };

                _sqlP[9].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("AdminExhibitsRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[9].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateExhibitsRegistrationsStatus(Int64 ExhibitId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@ExhibitId",ExhibitId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ExhibitsRegistrationsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetExhibitsRegistrationsListByVariable(string Search, Int64 PaymentStatusId, Int64 PaymentMethodId, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@PaymentStatusId",PaymentStatusId),
                    new SqlParameter("@PaymentMethodId",PaymentMethodId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total)
                };

                _sqlP[6].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExhibitsRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[6].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetExhibitsRegistrationsById(Int64 ExhibitId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@ExhibitId",ExhibitId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExhibitsRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteExhibitsRegistrations(Int64 ExhibitId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@ExhibitId",ExhibitId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ExhibitsRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateExhibitsRegistrations(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ExhibitId",objExhibitsRegistrations.ExhibitId),
                    new SqlParameter("@Amount",objExhibitsRegistrations.Amount),
                    new SqlParameter("@PaymentBy",(objExhibitsRegistrations.PaymentBy == null ?DBNull.Value:(object)objExhibitsRegistrations.PaymentBy)),
                    new SqlParameter("@TransactionId",(objExhibitsRegistrations.TransactionId == null ?DBNull.Value:(object)objExhibitsRegistrations.TransactionId)),
                    new SqlParameter("@PaymentMethod",(objExhibitsRegistrations.PaymentMethod == null ?DBNull.Value:(object)objExhibitsRegistrations.PaymentMethod)),
                    new SqlParameter("@PaymentStatus",(objExhibitsRegistrations.PaymentStatus == null ?DBNull.Value:(object)objExhibitsRegistrations.PaymentStatus)),
                    new SqlParameter("@PaymentDate",(objExhibitsRegistrations.PaymentDate == DateTime.MinValue ?DBNull.Value:(object)objExhibitsRegistrations.PaymentDate)),
                    new SqlParameter("@CardNumber",(objExhibitsRegistrations.CardNumber == null ?DBNull.Value:(object)objExhibitsRegistrations.CardNumber)),
                    new SqlParameter("@CSVExpiry",(objExhibitsRegistrations.CSVExpiry == null ?DBNull.Value:(object)objExhibitsRegistrations.CSVExpiry)),
                    new SqlParameter("@UpdatedBy",objExhibitsRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };

                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ExhibitsRegistrationsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable ExhibitsRegistrationsExportToExcel(Int64 PaymentStatusId, Int64 PaymentMethodId, string Search, string Sort)
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
                dt = _dbAccess.GetDataTable("ExhibitsRegistrationsExportToExcel", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        #region Front-End

        public DataSet FEExhibitsRegistrationsGetList(ref int QStatus)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[] 
                {              
                    new SqlParameter("@QStatus",QStatus)
                };

                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("FEExhibitsRegistrationsGetList", ref _sqlP);
                QStatus = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion
    }
}
