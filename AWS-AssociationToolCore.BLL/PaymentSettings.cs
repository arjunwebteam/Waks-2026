using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
    public class PaymentSettings
    {
        AWS_AssociationToolCore.DAL.PaymentSettings _PaymentSettings = new DAL.PaymentSettings();

        #region Methods

        public Int64 UpdatePaymentSettingsDetails(Entities.PaymentSettings objAppInfo)
        {
            Int64 _status = 0;
            if (objAppInfo != null)
            {
                _status = _PaymentSettings.UpdatePaymentDetails(objAppInfo);
            }
            return _status;
        }

        public Int64 DeletePaymentSettingById(Int64 Id)
        {
            Int64 _status = 0;
            _status = _PaymentSettings.DeletePaymentSettingById(Id);
            return _status;
        }

        public Int64 DeleteAllPaymentSettingsById(string Id)
        {
            Int64 _status = 0;
            _status = _PaymentSettings.DeleteAllPaymentSettingsById(Id);
            return _status;
        }

        public Int64 UpdatePaymentSettingApprovalById(Int64 PaymentSettingId)
        {
            Int64 _status = 0;
            _status = _PaymentSettings.UpdatePaymentSettingApprovalById(PaymentSettingId);
            return _status;
        }


        public Int64 UpdatePaymentMode(Int64 PaymentSettingId, string AccountType, Int64 PaymentMethodId)
        {
            Int64 _status = 0;
            _status = _PaymentSettings.UpdatePaymentMode(PaymentSettingId, AccountType, PaymentMethodId);
            return _status;
        }



        public Int64 UpdatePaymentMethodstatus(Int64 PaymentMethodId)
        {
            Int64 _status = 0;
            _status = _PaymentSettings.UpdatePaymentMethodstatus(PaymentMethodId);
            return _status;
        }

        #endregion

        #region Entity Loading

        public Entities.PaymentSettings GetPaymentSettingsDetails(Int64 PaymentSettingId,string PaymentMethod, ref int Status)
        {
            DataTable dt = _PaymentSettings.GetPaymentSettingsDetails(PaymentSettingId,PaymentMethod, ref Status);
            AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();

            if (Status == 1 && dt.Rows.Count == 1)
            {
                objPaymentSettings.PaymentSettingId = Convert.ToInt64(dt.Rows[0]["PaymentSettingId"].ToString());
                objPaymentSettings.PaymentMethodId = Convert.ToInt64(dt.Rows[0]["PaymentMethodId"].ToString());
                objPaymentSettings.CurrencyCodeId = (dt.Rows[0]["CurrencyCodeId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["CurrencyCodeId"]) : 0);
                objPaymentSettings.PaymentUrl = dt.Rows[0]["PaymentUrl"].ToString();
                //objPaymentSettings.PaymentEmail = dt.Rows[0]["PaymentEmail"].ToString();
                //objPaymentSettings.PaymentPassword = dt.Rows[0]["PaymentPassword"].ToString();
                //objPaymentSettings.CurrencyCode = dt.Rows[0]["CurrencyCode"].ToString();
                objPaymentSettings.SuccessUrl = (dt.Rows[0]["SuccessUrl"] != DBNull.Value ? dt.Rows[0]["SuccessUrl"].ToString() : null); 
                dt.Rows[0]["SuccessUrl"].ToString();
                objPaymentSettings.PaymentMethod = dt.Rows[0]["PaymentMethod"].ToString();
                objPaymentSettings.AccountType = dt.Rows[0]["AccountType"].ToString();
                objPaymentSettings.CancelUrl = (dt.Rows[0]["CancelUrl"] != DBNull.Value ? dt.Rows[0]["CancelUrl"].ToString() : null); dt.Rows[0]["CancelUrl"].ToString();
                objPaymentSettings.NotifyUrl = (dt.Rows[0]["NotifyURL"] != DBNull.Value ? dt.Rows[0]["NotifyURL"].ToString() : null); dt.Rows[0]["NotifyURL"].ToString();
                objPaymentSettings.TokenNo = (dt.Rows[0]["TokenNo"] != DBNull.Value ? dt.Rows[0]["TokenNo"].ToString() : null); dt.Rows[0]["TokenNo"].ToString();
               
               
            }
            return objPaymentSettings;
        }


        public Entities.PaymentSettings PaymentSettingsDetails( string PaymentMethod, ref int Status)
        {
            DataTable dt = _PaymentSettings.PaymentSettingsDetails(PaymentMethod, ref Status);
            AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();

            if (Status == 1 && dt.Rows.Count == 1)
            {
                objPaymentSettings.PaymentSettingId = Convert.ToInt64(dt.Rows[0]["PaymentSettingId"].ToString());
                objPaymentSettings.PaymentMethodId = Convert.ToInt64(dt.Rows[0]["PaymentMethodId"].ToString());
                objPaymentSettings.CurrencyCodeId = (dt.Rows[0]["CurrencyCodeId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["CurrencyCodeId"]) : 0);
                objPaymentSettings.PaymentUrl = dt.Rows[0]["PaymentUrl"].ToString();
                //objPaymentSettings.PaymentEmail = dt.Rows[0]["PaymentEmail"].ToString();
                //objPaymentSettings.PaymentPassword = dt.Rows[0]["PaymentPassword"].ToString();
                //objPaymentSettings.CurrencyCode = dt.Rows[0]["CurrencyCode"].ToString();
                objPaymentSettings.SuccessUrl = (dt.Rows[0]["SuccessUrl"] != DBNull.Value ? dt.Rows[0]["SuccessUrl"].ToString() : null); dt.Rows[0]["SuccessUrl"].ToString();
                objPaymentSettings.PaymentMethod = dt.Rows[0]["PaymentMethod"].ToString();
                objPaymentSettings.AccountType = dt.Rows[0]["AccountType"].ToString();
                objPaymentSettings.CancelUrl = (dt.Rows[0]["CancelUrl"] != DBNull.Value ? dt.Rows[0]["CancelUrl"].ToString() : null); dt.Rows[0]["CancelUrl"].ToString();
                objPaymentSettings.NotifyUrl = (dt.Rows[0]["NotifyURL"] != DBNull.Value ? dt.Rows[0]["NotifyURL"].ToString() : null); dt.Rows[0]["NotifyURL"].ToString();
                objPaymentSettings.TokenNo = (dt.Rows[0]["TokenNo"] != DBNull.Value ? dt.Rows[0]["TokenNo"].ToString() : null); dt.Rows[0]["TokenNo"].ToString();


            }
            return objPaymentSettings;
        }










        public AWS_AssociationToolCore.Entities.PaymentSettings GetPaymentSettingById(Int64 Id, ref int Status)
        {
            AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();
            DataTable dt = new DataTable();
            if (Id != 0)
            {
                dt = _PaymentSettings.GetPaymentSettingById(Id, ref Status);
                if (dt.Rows.Count == 1)
                {
                    objPaymentSettings.PaymentSettingId = Convert.ToInt64(dt.Rows[0]["PaymentSettingId"].ToString());
                    objPaymentSettings.PaymentMethodId = Convert.ToInt64(dt.Rows[0]["PaymentMethodId"].ToString());
                    objPaymentSettings.CurrencyCodeId = Convert.ToInt64(dt.Rows[0]["CurrencyCodeId"].ToString());
                    objPaymentSettings.PaymentUrl = dt.Rows[0]["PaymentUrl"].ToString();
                    objPaymentSettings.PaymentEmail = dt.Rows[0]["PaymentEmail"].ToString();
                    objPaymentSettings.PaymentPassword = dt.Rows[0]["PaymentPassword"].ToString();
                    //objPaymentSettings.CurrencyCode = dt.Rows[0]["CurrencyCode"].ToString();
                    objPaymentSettings.SuccessUrl = dt.Rows[0]["SuccessUrl"].ToString();
                    objPaymentSettings.CancelUrl = dt.Rows[0]["CancelUrl"].ToString();
                    //objPaymentSettings.PaymentMethod = dt.Rows[0]["PaymentMethod"].ToString();
                   objPaymentSettings.AccountType = dt.Rows[0]["AccountType"].ToString();
                    //objPaymentSettings.NotifyUrl = dt.Rows[0]["NotifyUrl"].ToString();
                    //objPaymentSettings.TokenNo = dt.Rows[0]["TokenNo"].ToString();
                }
            }
            return objPaymentSettings;
        }

        public List<AWS_AssociationToolCore.Entities.PaymentSettings> GetPaymentSettingsList(string search,int pageno, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PaymentSettings> lstPaymentSettings = new List<AWS_AssociationToolCore.Entities.PaymentSettings>();
            DataTable dt = _PaymentSettings.GetPaymentSettingsListByVariable(search, pageno, Items, ref Total);
            if (dt.Rows.Count == 0 && pageno != 0)
            {
                dt = _PaymentSettings.GetPaymentSettingsListByVariable(search,pageno - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();

                    objPaymentSettings.PaymentSettingId = Convert.ToInt64(dr["PaymentSettingId"].ToString());
                    objPaymentSettings.RId = Convert.ToInt64(dr["RId"].ToString());
                    objPaymentSettings.PaymentMethodId = Convert.ToInt64(dr["PaymentMethodId"].ToString());
                    objPaymentSettings.CurrencyCodeId = (dr["CurrencyCodeId"] != DBNull.Value ? Convert.ToInt64(dr["CurrencyCodeId"]) : 0);
                    objPaymentSettings.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objPaymentSettings.PaymentUrl = dr["PaymentUrl"].ToString();
                    //objPaymentSettings.PaymentEmail = (dr["PaymentEmail"] != DBNull.Value ? dr["PaymentEmail"].ToString() : null); dr["PaymentEmail"].ToString();
                    //objPaymentSettings.PaymentPassword = (dr["PaymentEmail"] != DBNull.Value ? dr["PaymentEmail"].ToString() : null);
                    objPaymentSettings.CurrencyCode = (dr["CurrencyCode"] != DBNull.Value ? dr["CurrencyCode"].ToString() : null);
                    objPaymentSettings.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objPaymentSettings.AccountType = dr["AccountType"].ToString();
                    objPaymentSettings.SuccessUrl = dr["SuccessUrl"].ToString();
                    objPaymentSettings.Mode = (dr["Mode"] != DBNull.Value ? Convert.ToBoolean(dr["Mode"]) : false);
                    //objPaymentSettings.CancelUrl = dr["CancelUrl"].ToString();
                    objPaymentSettings.NotifyUrl = dr["NotifyUrl"].ToString();
                    objPaymentSettings.TokenNo = dr["TokenNo"].ToString();

                    lstPaymentSettings.Add(objPaymentSettings);
                }
            }
            return lstPaymentSettings;
        }

        public List<AWS_AssociationToolCore.Entities.CurrencyCodes> GetCurrencyCodesList(ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CurrencyCodes> lstCurrencyCodes = new List<AWS_AssociationToolCore.Entities.CurrencyCodes>();
            DataTable dt = _PaymentSettings.CurrencyCodesList(ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CurrencyCodes objCurrencyCodes = new AWS_AssociationToolCore.Entities.CurrencyCodes();
                    objCurrencyCodes.CurrencyCodeId =Convert.ToInt64(dr["CurrencyCodeId"].ToString());
                    objCurrencyCodes.CurrencyCode = dr["CurrencyCode"].ToString();

                    lstCurrencyCodes.Add(objCurrencyCodes);

                }

            }
            return lstCurrencyCodes;
        }

        public List<AWS_AssociationToolCore.Entities.PaymentMethods> GetPaymentMethodsList(ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PaymentMethods> lstPaymentMethods = new List<AWS_AssociationToolCore.Entities.PaymentMethods>();
            DataTable dt = _PaymentSettings.PaymentMethodsList(ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentMethods objPaymentMethods = new AWS_AssociationToolCore.Entities.PaymentMethods();
                    objPaymentMethods.PaymentMethodId = Convert.ToInt64(dr["PaymentMethodId"].ToString());
                    objPaymentMethods.PaymentMethod = dr["PaymentMethod"].ToString();

                    lstPaymentMethods.Add(objPaymentMethods);

                }

            }
            return lstPaymentMethods;
        }

        public Entities.PaymentSettings GetPaymentSettings( ref int Status)
        {
            DataTable dt = _PaymentSettings.GetPaymentSettings( ref Status);
            AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();

            if (Status == 1 && dt.Rows.Count == 1)
            {
                objPaymentSettings.PaymentSettingId = Convert.ToInt64(dt.Rows[0]["PaymentSettingId"].ToString());
                objPaymentSettings.PaymentMethodId = Convert.ToInt64(dt.Rows[0]["PaymentMethodId"].ToString());
                objPaymentSettings.CurrencyCodeId = Convert.ToInt64(dt.Rows[0]["CurrencyCodeId"].ToString());
                objPaymentSettings.PaymentUrl = dt.Rows[0]["PaymentUrl"].ToString();
                objPaymentSettings.PaymentEmail = dt.Rows[0]["PaymentEmail"].ToString();
                objPaymentSettings.PaymentPassword = dt.Rows[0]["PaymentPassword"].ToString();
                objPaymentSettings.CurrencyCode = dt.Rows[0]["CurrencyCode"].ToString();
                objPaymentSettings.SuccessUrl = dt.Rows[0]["SuccessUrl"].ToString();
                objPaymentSettings.PaymentMethod = dt.Rows[0]["PaymentMethod"].ToString();
                objPaymentSettings.AccountType = dt.Rows[0]["AccountType"].ToString();
                objPaymentSettings.CancelUrl = dt.Rows[0]["CancelUrl"].ToString();
                objPaymentSettings.NotifyUrl = dt.Rows[0]["NotifyUrl"].ToString();
                objPaymentSettings.TokenNo = dt.Rows[0]["TokenNo"].ToString();

            }
            return objPaymentSettings;
        }

        public List<AWS_AssociationToolCore.Entities.PaymentStatus> GetPaymentStatusList(ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PaymentStatus> lstPaymentStatus = new List<AWS_AssociationToolCore.Entities.PaymentStatus>();
            DataTable dt = _PaymentSettings.PaymentStatusList(ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentStatus objPaymentStatus = new AWS_AssociationToolCore.Entities.PaymentStatus();
                    objPaymentStatus.PaymentStatusId = Convert.ToInt64(dr["PaymentStatusId"].ToString());
                    objPaymentStatus.PaymentStatus1 = dr["PaymentStatus"].ToString();

                    lstPaymentStatus.Add(objPaymentStatus);

                }

            }
            return lstPaymentStatus;
        }

        #endregion

    }
}
