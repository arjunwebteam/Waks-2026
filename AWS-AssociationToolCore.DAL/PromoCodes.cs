using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
    public class PromoCodes
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Method

        public Int64 DeletePromoCodes(Int64 PromoCodeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCodeId",PromoCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PromoCodesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertPromoCodes(Entities.PromoCodes objPromoCodes)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@PromoCodeId",objPromoCodes.PromoCodeId),
                    new SqlParameter("@PromoCode",objPromoCodes.PromoCode),
                    new SqlParameter("@Message",(objPromoCodes.Message != null ? (object)objPromoCodes.Message:DBNull.Value)),
                  new SqlParameter("@StartDate",(objPromoCodes.StartDate==DateTime.MinValue?(object)DBNull.Value:objPromoCodes.StartDate)),
                    new SqlParameter("@EndDate",(objPromoCodes.EndDate==DateTime.MinValue?(object)DBNull.Value:objPromoCodes.EndDate)),
                    new SqlParameter("@NoOfUsers",(objPromoCodes.NoOfUsers != 0 ? (object)objPromoCodes.NoOfUsers:DBNull.Value)),
                    new SqlParameter("@MinimumOrderAmount",(objPromoCodes.MinimumOrderAmount != 0 ? (object)objPromoCodes.MinimumOrderAmount:DBNull.Value)),
                    new SqlParameter("@Discount",(objPromoCodes.Discount != 0 ? (object)objPromoCodes.Discount:DBNull.Value)),
                    new SqlParameter("@DiscountType",(objPromoCodes.DiscountType != null ? (object)objPromoCodes.DiscountType:DBNull.Value)),
                    new SqlParameter("@MaxDiscountAmount",(objPromoCodes.MaxDiscountAmount != 0 ? (object)objPromoCodes.MaxDiscountAmount:DBNull.Value)),
                    new SqlParameter("@RepeatUsage",(objPromoCodes.RepeatUsage != null ? (object)objPromoCodes.RepeatUsage:DBNull.Value)),
                    new SqlParameter("@NoOfRepeatUsage",(objPromoCodes.NoOfRepeatUsage != 0 ? (object)objPromoCodes.NoOfRepeatUsage:DBNull.Value)),
                    new SqlParameter("@Status",(objPromoCodes.Status != null ? (object)objPromoCodes.Status:DBNull.Value)),
                    new SqlParameter("@IsActive",objPromoCodes.IsActive),
                    new SqlParameter("@ServiceId",(objPromoCodes.ServiceId != 0 ? (object)objPromoCodes.ServiceId:DBNull.Value)),
                    new SqlParameter("@Field1",(objPromoCodes.Field1 != null ? (object)objPromoCodes.Field1:DBNull.Value)),
                    new SqlParameter("@Field2",(objPromoCodes.Field2 != null ? (object)objPromoCodes.Field2:DBNull.Value)),
                    new SqlParameter("@Field3",(objPromoCodes.Field3 != null ? (object)objPromoCodes.Field3:DBNull.Value)),
                    new SqlParameter("@InsertedBy",objPromoCodes.InsertedBy),
                    new SqlParameter("@InsertedDate",objPromoCodes.InsertedDate),
                    new SqlParameter("@UpdatedBy",objPromoCodes.UpdatedBy),
                    new SqlParameter("@UpdatedDate",objPromoCodes.UpdatedDate)
                    };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PromoCodesInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetCodeByPromoCodesTitle(string PromoCode, ref int _QStatus)
        {

            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCode", PromoCode),
                    new SqlParameter("@QStatus", _QStatus)
                };

                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CouponsGetByPromoCode", ref _sqlP);
                _QStatus = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdatePromoCodeStatus(Int64 PromoCodeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCodeId",PromoCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PromoCodeStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        #region Entity

        public DataTable GetPromoCodeById(Int64 PromoCodeId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCodeId",PromoCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PromoCodesGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetPromoCodes(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PromoCodesGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetPromoCodesList(string StartDate, string EndDate, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                { 
                   
                    new SqlParameter("@StartDate",StartDate),
                    new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[6].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PromoCodesGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[6].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        public DataTable ValidateCoupon(string PromoCode, decimal Amount, Int64 ServiceId, Int64 CustomerId, ref int _QStatus)
        {

            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCode", PromoCode),
                    new SqlParameter("@Amount", Amount),
                    new SqlParameter("@ServiceId", ServiceId),
                    new SqlParameter("@CustomerId", CustomerId),
                    new SqlParameter("@QStatus", _QStatus)
                };

                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEPromocodeValidate", ref _sqlP);
                _QStatus = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }



        public DataTable GetPromocode(string PromoCode,ref int _QStatus)
        {

            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCode", PromoCode),
                    new SqlParameter("@QStatus", _QStatus)
                };

                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEGetPromocodeValidate", ref _sqlP);
                _QStatus = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ValidateCouponAPI(string PromoCode, decimal Amount, Int64 ServiceId, Int64 CustomerId, ref int _QStatus)
        {

            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PromoCode", PromoCode),
                    new SqlParameter("@Amount", Amount),
                    new SqlParameter("@ServiceId", ServiceId),
                    new SqlParameter("@CustomerId", CustomerId),
                    new SqlParameter("@QStatus", _QStatus)
                };

                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PromocodeValidateAPI", ref _sqlP);
                _QStatus = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
