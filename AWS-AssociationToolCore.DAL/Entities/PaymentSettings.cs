using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
   public class PaymentSettings
    {
        private Int64 _RId;

        public Int64 RId
        {
            get { return _RId; }
            set { _RId = value; }
        }
        private bool _IsActive;

        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }
        private Int64 _PaymentSettingId;

        public Int64 PaymentSettingId
        {
            get { return _PaymentSettingId; }
            set { _PaymentSettingId = value; }
        }
        private string _PaymentMethod;

        public string PaymentMethod
        {
            get { return _PaymentMethod; }
            set { _PaymentMethod = value; }
        }

        private Int64 _PaymentMethodId;

        public Int64 PaymentMethodId
        {
            get { return _PaymentMethodId; }
            set { _PaymentMethodId = value; }
        }
        private Int64 _CurrencyCodeId;

        public Int64 CurrencyCodeId
        {
            get { return _CurrencyCodeId; }
            set { _CurrencyCodeId = value; }
        }
        private string _AccountType;

        public string AccountType
        {
            get { return _AccountType; }
            set { _AccountType = value; }
        }
        private string _PaymentUrl;

        public string PaymentUrl
        {
            get { return _PaymentUrl; }
            set { _PaymentUrl = value; }
        }
        private string _Cmd;

        public string Cmd
        {
            get { return _Cmd; }
            set { _Cmd = value; }
        }
        private string _PaymentEmail;

        public string PaymentEmail
        {
            get { return _PaymentEmail; }
            set { _PaymentEmail = value; }
        }
        private string _PaymentPassword;

        public string PaymentPassword
        {
            get { return _PaymentPassword; }
            set { _PaymentPassword = value; }
        }
        private string _CurrencyCode;

        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set { _CurrencyCode = value; }
        }
        private string _SuccessUrl;

        public string SuccessUrl
        {
            get { return _SuccessUrl; }
            set { _SuccessUrl = value; }
        }
        private string _CancelUrl;

        public string CancelUrl
        {
            get { return _CancelUrl; }
            set { _CancelUrl = value; }
        }
        private string _NotifyUrl;

        public string NotifyUrl
        {
            get { return _NotifyUrl; }
            set { _NotifyUrl = value; }
        }
        private string _TokenNo;

        public string TokenNo
        {
            get { return _TokenNo; }
            set { _TokenNo = value; }
        }

        public bool Mode { get; set; }
        public string APIUser { get; internal set; }
    }

}

//dsfdsf