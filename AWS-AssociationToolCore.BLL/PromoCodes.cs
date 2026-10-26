using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
   public class PromoCodes
    {
        DAL.PromoCodes _PromoCodes = new DAL.PromoCodes();

        #region Methods

        public Int64 DeletePromoCodes(Int64 PromoCodeId)
        {
            Int64 _status = 0;
            if (PromoCodeId != 0)
            {
                _status = _PromoCodes.DeletePromoCodes(PromoCodeId);
            }
            return _status;
        }

        public Int64 InsertPromoCodes(Entities.PromoCodes objPromoCodes)
        {
            Int64 _status = 0;
            if (objPromoCodes != null)
            {
                _status = _PromoCodes.InsertPromoCodes(objPromoCodes);
            }
            return _status;
        }

        public Int64 UpdatePromoCodesStatus(Int64 PromoCodeId)
        {
            Int64 _status = 0;
            _status = _PromoCodes.UpdatePromoCodeStatus(PromoCodeId);
            return _status;
        }

        #endregion

        #region Entity Loading

        public List<Entities.PromoCodes> GetPromoCodesList(string StartDate, string EndDate, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _PromoCodes.GetPromoCodesList(StartDate, EndDate, Search, Sort, PageNo, Items, ref Total);
            List<Entities.PromoCodes> lstPromoCodes = new List<Entities.PromoCodes>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.PromoCodes objPromoCodes = new Entities.PromoCodes();
                    objPromoCodes.RId = Convert.ToInt64(dr["Rid"]);
                    objPromoCodes.PromoCodeId = Convert.ToInt64(dr["PromoCodeId"]);
                    objPromoCodes.PromoCode = (dr["PromoCode"] == DBNull.Value) ? "" : dr["PromoCode"].ToString();
                    objPromoCodes.Message = (dr["Message"] == DBNull.Value) ? "" : dr["Message"].ToString();
                    objPromoCodes.StartDate = (dr["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["StartDate"].ToString());
                    objPromoCodes.EndDate = (dr["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["EndDate"].ToString());
                    objPromoCodes.NoOfUsers = (dr["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dr["NoOfUsers"].ToString());
                    objPromoCodes.MinimumOrderAmount = (dr["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["MinimumOrderAmount"].ToString());
                    objPromoCodes.Discount = (dr["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["Discount"].ToString());
                    objPromoCodes.DiscountType = (dr["DiscountType"] == DBNull.Value) ? "" : dr["DiscountType"].ToString();
                    objPromoCodes.MaxDiscountAmount = (dr["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["MaxDiscountAmount"].ToString());
                    objPromoCodes.RepeatUsage = (dr["RepeatUsage"] == DBNull.Value) ? "" : dr["RepeatUsage"].ToString();
                    objPromoCodes.NoOfRepeatUsage = (dr["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dr["NoOfRepeatUsage"].ToString());
                    objPromoCodes.Status = (dr["Status"] == DBNull.Value) ? "" : dr["Status"].ToString();
                    objPromoCodes.IsActive = (dr["IsActive"] == DBNull.Value) ? false : Convert.ToBoolean(dr["IsActive"].ToString());
                    objPromoCodes.ServiceId = (dr["ServiceId"] == DBNull.Value) ? 0 : Convert.ToInt64(dr["ServiceId"].ToString());
                    objPromoCodes.Field1 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field1"].ToString();
                    objPromoCodes.Field2 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field2"].ToString();
                    objPromoCodes.Field3 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field3"].ToString();
                    objPromoCodes.InsertedBy = (dr["InsertedBy"] == DBNull.Value) ? "" : dr["InsertedBy"].ToString();
                    objPromoCodes.InsertedDate = (dr["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objPromoCodes.UpdatedBy = (dr["UpdatedBy"] == DBNull.Value) ? "" : dr["UpdatedBy"].ToString();
                    objPromoCodes.UpdatedDate = (dr["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    lstPromoCodes.Add(objPromoCodes);
                }
            }
            return lstPromoCodes;
        }

        public Entities.PromoCodes GetPromoCodesById(Int64 PromoCodeId, ref int status)
        {
            DataTable dt = _PromoCodes.GetPromoCodeById(PromoCodeId, ref status);

            Entities.PromoCodes objPromoCodes = new Entities.PromoCodes();

            if (dt.Rows.Count == 1)
            {  
                objPromoCodes.PromoCodeId = Convert.ToInt64(dt.Rows[0]["PromoCodeId"]);
                objPromoCodes.PromoCode = (dt.Rows[0]["PromoCode"] == DBNull.Value) ? "" : dt.Rows[0]["PromoCode"].ToString();
                objPromoCodes.Message = (dt.Rows[0]["Message"] == DBNull.Value) ? "" : dt.Rows[0]["Message"].ToString();
                objPromoCodes.StartDate = (dt.Rows[0]["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                objPromoCodes.EndDate = (dt.Rows[0]["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());
                objPromoCodes.NoOfUsers = (dt.Rows[0]["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfUsers"].ToString());
                objPromoCodes.MinimumOrderAmount = (dt.Rows[0]["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MinimumOrderAmount"].ToString());
                objPromoCodes.Discount = (dt.Rows[0]["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["Discount"].ToString());
                objPromoCodes.DiscountType = (dt.Rows[0]["DiscountType"] == DBNull.Value) ? "" : dt.Rows[0]["DiscountType"].ToString();
                objPromoCodes.MaxDiscountAmount = (dt.Rows[0]["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MaxDiscountAmount"].ToString());
                objPromoCodes.RepeatUsage = (dt.Rows[0]["RepeatUsage"] == DBNull.Value) ? "" : dt.Rows[0]["RepeatUsage"].ToString();
                objPromoCodes.NoOfRepeatUsage = (dt.Rows[0]["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfRepeatUsage"].ToString());
                objPromoCodes.Status = (dt.Rows[0]["Status"] == DBNull.Value) ? "" : dt.Rows[0]["Status"].ToString();
                objPromoCodes.IsActive = (dt.Rows[0]["IsActive"] == DBNull.Value) ? false : Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                objPromoCodes.ServiceId = (dt.Rows[0]["ServiceId"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["ServiceId"].ToString());
                objPromoCodes.Field1 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field1"].ToString();
                objPromoCodes.Field2 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field2"].ToString();
                objPromoCodes.Field3 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field3"].ToString();
                objPromoCodes.InsertedBy = (dt.Rows[0]["InsertedBy"] == DBNull.Value) ? "" : dt.Rows[0]["InsertedBy"].ToString();
                objPromoCodes.InsertedDate = (dt.Rows[0]["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                objPromoCodes.UpdatedBy = (dt.Rows[0]["UpdatedBy"] == DBNull.Value) ? "" : dt.Rows[0]["UpdatedBy"].ToString();
                objPromoCodes.UpdatedDate = (dt.Rows[0]["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                //objPromoCodes.ServiceName = (dt.Rows[0]["ServiceName"] == DBNull.Value) ? "" : dt.Rows[0]["ServiceName"].ToString();
            }
            return objPromoCodes;
        }

        public List<Entities.PromoCodes> GetPromoCodes(ref int status)
        {
            DataTable dt = _PromoCodes.GetPromoCodes(ref status);

            List<Entities.PromoCodes> lstPromoCodes = new List<Entities.PromoCodes>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.PromoCodes objPromoCodes = new Entities.PromoCodes();

                    objPromoCodes.PromoCodeId = Convert.ToInt64(dr["PromoCodeId"]);
                    objPromoCodes.PromoCode = (dr["PromoCode"] == DBNull.Value) ? "" : dr["PromoCode"].ToString();
                    objPromoCodes.Message = (dr["Message"] == DBNull.Value) ? "" : dr["Message"].ToString();
                    objPromoCodes.StartDate = (dr["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["StartDate"].ToString());
                    objPromoCodes.EndDate = (dr["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["EndDate"].ToString());
                    objPromoCodes.NoOfUsers = (dr["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dr["NoOfUsers"].ToString());
                    objPromoCodes.MinimumOrderAmount = (dr["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["MinimumOrderAmount"].ToString());
                    objPromoCodes.Discount = (dr["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["Discount"].ToString());
                    objPromoCodes.DiscountType = (dr["DiscountType"] == DBNull.Value) ? "" : dr["DiscountType"].ToString();
                    objPromoCodes.MaxDiscountAmount = (dr["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["MaxDiscountAmount"].ToString());
                    objPromoCodes.RepeatUsage = (dr["RepeatUsage"] == DBNull.Value) ? "" : dr["RepeatUsage"].ToString();
                    objPromoCodes.NoOfRepeatUsage = (dr["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dr["NoOfRepeatUsage"].ToString());
                    objPromoCodes.Status = (dr["Status"] == DBNull.Value) ? "" : dr["Status"].ToString();
                    objPromoCodes.Field1 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field1"].ToString();
                    objPromoCodes.Field2 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field2"].ToString();
                    objPromoCodes.Field3 = (dr["Field1"] == DBNull.Value) ? "" : dr["Field3"].ToString();
                    objPromoCodes.InsertedBy = (dr["InsertedBy"] == DBNull.Value) ? "" : dr["InsertedBy"].ToString();
                    objPromoCodes.InsertedDate = (dr["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objPromoCodes.UpdatedBy = (dr["UpdatedBy"] == DBNull.Value) ? "" : dr["UpdatedBy"].ToString();
                    objPromoCodes.UpdatedDate = (dr["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    lstPromoCodes.Add(objPromoCodes);
                }
            }
            return lstPromoCodes;
        }

        public Entities.PromoCodes GetCouponsByCouponTitle(string PromoCode, ref int _QStatus)
        {
            Entities.PromoCodes _objPromoCodes = new Entities.PromoCodes();
            DataTable dt = new DataTable();

            if (PromoCode != null && PromoCode.Trim() != "")
            {
                dt = _PromoCodes.GetCodeByPromoCodesTitle(PromoCode, ref _QStatus);
                if (dt.Rows.Count == 1)
                {
                    _objPromoCodes.PromoCodeId = Convert.ToInt64(dt.Rows[0]["PromoCodeId"]);
                    _objPromoCodes.PromoCode = (dt.Rows[0]["PromoCode"] == DBNull.Value) ? "" : dt.Rows[0]["PromoCode"].ToString();
                    _objPromoCodes.Message = (dt.Rows[0]["Message"] == DBNull.Value) ? "" : dt.Rows[0]["Message"].ToString();
                    _objPromoCodes.StartDate = (dt.Rows[0]["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                    _objPromoCodes.EndDate = (dt.Rows[0]["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());
                    _objPromoCodes.NoOfUsers = (dt.Rows[0]["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfUsers"].ToString());
                    _objPromoCodes.MinimumOrderAmount = (dt.Rows[0]["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MinimumOrderAmount"].ToString());
                    _objPromoCodes.Discount = (dt.Rows[0]["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["Discount"].ToString());
                    _objPromoCodes.DiscountType = (dt.Rows[0]["DiscountType"] == DBNull.Value) ? "" : dt.Rows[0]["DiscountType"].ToString();
                    _objPromoCodes.MaxDiscountAmount = (dt.Rows[0]["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MaxDiscountAmount"].ToString());
                    _objPromoCodes.RepeatUsage = (dt.Rows[0]["RepeatUsage"] == DBNull.Value) ? "" : dt.Rows[0]["RepeatUsage"].ToString();
                    _objPromoCodes.NoOfRepeatUsage = (dt.Rows[0]["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfRepeatUsage"].ToString());
                    _objPromoCodes.Status = (dt.Rows[0]["Status"] == DBNull.Value) ? "" : dt.Rows[0]["Status"].ToString();
                    _objPromoCodes.Field1 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field1"].ToString();
                    _objPromoCodes.Field2 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field2"].ToString();
                    _objPromoCodes.Field3 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field3"].ToString();
                    _objPromoCodes.InsertedBy = (dt.Rows[0]["InsertedBy"] == DBNull.Value) ? "" : dt.Rows[0]["InsertedBy"].ToString();
                    _objPromoCodes.InsertedDate = (dt.Rows[0]["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    _objPromoCodes.UpdatedBy = (dt.Rows[0]["UpdatedBy"] == DBNull.Value) ? "" : dt.Rows[0]["UpdatedBy"].ToString();
                    _objPromoCodes.UpdatedDate = (dt.Rows[0]["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                }
            }
            return _objPromoCodes;
        }


        public Entities.PromoCodes ValidateCoupon(string PromoCode, decimal Amount, Int64 ServiceId, Int64 CustomerId, ref int _QStatus)
        {
            Entities.PromoCodes _objPromoCodes = new Entities.PromoCodes();
            DataTable dt = new DataTable();

            if (PromoCode != null && PromoCode.Trim() != "")
            {
                dt = _PromoCodes.ValidateCoupon(PromoCode, Amount, ServiceId, CustomerId, ref _QStatus);
                if (dt.Rows.Count == 1)
                {
                    _objPromoCodes.PromoCodeId = Convert.ToInt64(dt.Rows[0]["PromoCodeId"]);
                    _objPromoCodes.PromoCode = (dt.Rows[0]["PromoCode"] == DBNull.Value) ? "" : dt.Rows[0]["PromoCode"].ToString();
                    _objPromoCodes.Message = (dt.Rows[0]["Message"] == DBNull.Value) ? "" : dt.Rows[0]["Message"].ToString();
                    _objPromoCodes.StartDate = (dt.Rows[0]["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                    _objPromoCodes.EndDate = (dt.Rows[0]["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());
                    _objPromoCodes.NoOfUsers = (dt.Rows[0]["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfUsers"].ToString());
                    _objPromoCodes.MinimumOrderAmount = (dt.Rows[0]["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MinimumOrderAmount"].ToString());
                    _objPromoCodes.Discount = (dt.Rows[0]["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["Discount"].ToString());
                    _objPromoCodes.DiscountType = (dt.Rows[0]["DiscountType"] == DBNull.Value) ? "" : dt.Rows[0]["DiscountType"].ToString();
                    _objPromoCodes.MaxDiscountAmount = (dt.Rows[0]["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MaxDiscountAmount"].ToString());
                    //   _objPromoCodes.AfterDiscountTotal = Amount - _objPromoCodes.MaxDiscountAmount;
                    _objPromoCodes.RepeatUsage = (dt.Rows[0]["RepeatUsage"] == DBNull.Value) ? "" : dt.Rows[0]["RepeatUsage"].ToString();
                    _objPromoCodes.NoOfRepeatUsage = (dt.Rows[0]["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfRepeatUsage"].ToString());
                    _objPromoCodes.Status = (dt.Rows[0]["Status"] == DBNull.Value) ? "" : dt.Rows[0]["Status"].ToString();
                    _objPromoCodes.Field1 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field1"].ToString();
                    _objPromoCodes.Field2 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field2"].ToString();
                    _objPromoCodes.Field3 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field3"].ToString();
                    _objPromoCodes.InsertedBy = (dt.Rows[0]["InsertedBy"] == DBNull.Value) ? "" : dt.Rows[0]["InsertedBy"].ToString();
                    _objPromoCodes.InsertedDate = (dt.Rows[0]["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    _objPromoCodes.UpdatedBy = (dt.Rows[0]["UpdatedBy"] == DBNull.Value) ? "" : dt.Rows[0]["UpdatedBy"].ToString();
                    _objPromoCodes.UpdatedDate = (dt.Rows[0]["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                }
            }
            return _objPromoCodes;
        }


        public Entities.PromoCodes GetPromocode(string PromoCode, ref int _QStatus)
        {
            Entities.PromoCodes _objPromoCodes = new Entities.PromoCodes();
            DataTable dt = new DataTable();

            if (PromoCode != null && PromoCode.Trim() != "")
            {
                dt = _PromoCodes.GetPromocode(PromoCode, ref _QStatus);
                if (dt.Rows.Count == 1)
                {
                    _objPromoCodes.PromoCodeId = Convert.ToInt64(dt.Rows[0]["PromoCodeId"]);
                    _objPromoCodes.PromoCode = (dt.Rows[0]["PromoCode"] == DBNull.Value) ? "" : dt.Rows[0]["PromoCode"].ToString();
                    _objPromoCodes.Message = (dt.Rows[0]["Message"] == DBNull.Value) ? "" : dt.Rows[0]["Message"].ToString();
                    _objPromoCodes.StartDate = (dt.Rows[0]["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                    _objPromoCodes.EndDate = (dt.Rows[0]["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());
                    _objPromoCodes.NoOfUsers = (dt.Rows[0]["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfUsers"].ToString());
                    _objPromoCodes.MinimumOrderAmount = (dt.Rows[0]["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MinimumOrderAmount"].ToString());
                    _objPromoCodes.Discount = (dt.Rows[0]["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["Discount"].ToString());
                    _objPromoCodes.DiscountType = (dt.Rows[0]["DiscountType"] == DBNull.Value) ? "" : dt.Rows[0]["DiscountType"].ToString();
                    _objPromoCodes.MaxDiscountAmount = (dt.Rows[0]["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MaxDiscountAmount"].ToString());
                    //   _objPromoCodes.AfterDiscountTotal = Amount - _objPromoCodes.MaxDiscountAmount;
                    _objPromoCodes.RepeatUsage = (dt.Rows[0]["RepeatUsage"] == DBNull.Value) ? "" : dt.Rows[0]["RepeatUsage"].ToString();
                    _objPromoCodes.NoOfRepeatUsage = (dt.Rows[0]["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfRepeatUsage"].ToString());
                    _objPromoCodes.Status = (dt.Rows[0]["Status"] == DBNull.Value) ? "" : dt.Rows[0]["Status"].ToString();
                    _objPromoCodes.Field1 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field1"].ToString();
                    _objPromoCodes.Field2 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field2"].ToString();
                    _objPromoCodes.Field3 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field3"].ToString();
                    _objPromoCodes.InsertedBy = (dt.Rows[0]["InsertedBy"] == DBNull.Value) ? "" : dt.Rows[0]["InsertedBy"].ToString();
                    _objPromoCodes.InsertedDate = (dt.Rows[0]["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    _objPromoCodes.UpdatedBy = (dt.Rows[0]["UpdatedBy"] == DBNull.Value) ? "" : dt.Rows[0]["UpdatedBy"].ToString();
                    _objPromoCodes.UpdatedDate = (dt.Rows[0]["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                }
            }
            return _objPromoCodes;
        }


        public Entities.PromoCodes ValidateCouponAPI(string PromoCode, decimal Amount, Int64 ServiceId, Int64 CustomerId, ref int _QStatus)
        {
            Entities.PromoCodes _objPromoCodes = new Entities.PromoCodes();
            DataTable dt = new DataTable();

            if (PromoCode != null && PromoCode.Trim() != "")
            {
                dt = _PromoCodes.ValidateCouponAPI(PromoCode, Amount, ServiceId, CustomerId, ref _QStatus);
                if (dt.Rows.Count == 1)
                {
                    _objPromoCodes.PromoCodeId = Convert.ToInt64(dt.Rows[0]["PromoCodeId"]);
                    _objPromoCodes.PromoCode = (dt.Rows[0]["PromoCode"] == DBNull.Value) ? "" : dt.Rows[0]["PromoCode"].ToString();
                    _objPromoCodes.Message = (dt.Rows[0]["Message"] == DBNull.Value) ? "" : dt.Rows[0]["Message"].ToString();
                    _objPromoCodes.StartDate = (dt.Rows[0]["StartDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                    _objPromoCodes.EndDate = (dt.Rows[0]["EndDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());
                    _objPromoCodes.NoOfUsers = (dt.Rows[0]["NoOfUsers"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfUsers"].ToString());
                    _objPromoCodes.MinimumOrderAmount = (dt.Rows[0]["MinimumOrderAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MinimumOrderAmount"].ToString());
                    _objPromoCodes.Discount = (dt.Rows[0]["Discount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["Discount"].ToString());
                    _objPromoCodes.DiscountType = (dt.Rows[0]["DiscountType"] == DBNull.Value) ? "" : dt.Rows[0]["DiscountType"].ToString();
                    _objPromoCodes.MaxDiscountAmount = (dt.Rows[0]["MaxDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["MaxDiscountAmount"].ToString());
                    _objPromoCodes.RepeatUsage = (dt.Rows[0]["RepeatUsage"] == DBNull.Value) ? "" : dt.Rows[0]["RepeatUsage"].ToString();
                    _objPromoCodes.NoOfRepeatUsage = (dt.Rows[0]["NoOfRepeatUsage"] == DBNull.Value) ? 0 : Convert.ToInt64(dt.Rows[0]["NoOfRepeatUsage"].ToString());
                    _objPromoCodes.Status = (dt.Rows[0]["Status"] == DBNull.Value) ? "" : dt.Rows[0]["Status"].ToString();
                    _objPromoCodes.Field1 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field1"].ToString();
                    _objPromoCodes.Field2 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field2"].ToString();
                    _objPromoCodes.Field3 = (dt.Rows[0]["Field1"] == DBNull.Value) ? "" : dt.Rows[0]["Field3"].ToString();
                    _objPromoCodes.InsertedBy = (dt.Rows[0]["InsertedBy"] == DBNull.Value) ? "" : dt.Rows[0]["InsertedBy"].ToString();
                    _objPromoCodes.InsertedDate = (dt.Rows[0]["InsertedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    _objPromoCodes.UpdatedBy = (dt.Rows[0]["UpdatedBy"] == DBNull.Value) ? "" : dt.Rows[0]["UpdatedBy"].ToString();
                    _objPromoCodes.UpdatedDate = (dt.Rows[0]["UpdatedDate"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                    _objPromoCodes.TotalDiscountAmount = (dt.Rows[0]["TotalDiscountAmount"] == DBNull.Value) ? 0 : Convert.ToDecimal(dt.Rows[0]["TotalDiscountAmount"].ToString());

                    
                }
            }
            return _objPromoCodes;
        }


        #endregion
    }
}
