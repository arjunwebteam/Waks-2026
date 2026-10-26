using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class PaymentMethods
    {
        AWS_AssociationToolCore.DAL.PaymentMethods _PaymentMethods = new AWS_AssociationToolCore.DAL.PaymentMethods();

        #region Admin

       
        public Int64 UpdatePaymentMethodstatus(Int64 PaymentMethodId)
        {
            Int64 _status = 0;
            _status = _PaymentMethods.UpdatePaymentMethodstatus(PaymentMethodId);
            return _status;
        }

      
        public List<AWS_AssociationToolCore.Entities.PaymentMethods> GetPaymentMethodsListByVariable( string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PaymentMethods> lstPaymentMethods = new List<AWS_AssociationToolCore.Entities.PaymentMethods>();
            DataTable dt = _PaymentMethods.GetPaymentMethodsListByVariable( Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _PaymentMethods.GetPaymentMethodsListByVariable( Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentMethods objPaymentMethods = new AWS_AssociationToolCore.Entities.PaymentMethods();

                    objPaymentMethods.RId = Convert.ToInt64(dr["RId"].ToString());
                    objPaymentMethods.PaymentMethodId = Convert.ToInt64(dr["PaymentMethodId"].ToString());
                    objPaymentMethods.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objPaymentMethods.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                  

                    lstPaymentMethods.Add(objPaymentMethods);
                }
            }
            return lstPaymentMethods;
        }


        public List<AWS_AssociationToolCore.Entities.PaymentMethods> GetPaymentmethodsList(ref int status)
        {
            //List<AWS_AssociationToolCore.Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            List<AWS_AssociationToolCore.Entities.PaymentMethods> lstPaymentMethods = new List<AWS_AssociationToolCore.Entities.PaymentMethods>();


            DataTable dt = _PaymentMethods.GetPaymentmethodsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentMethods objPaymentMethods = new AWS_AssociationToolCore.Entities.PaymentMethods();

                    //objPaymentMethods.RId = Convert.ToInt64(dr["RId"].ToString());
                    objPaymentMethods.PaymentMethodId = Convert.ToInt64(dr["PaymentMethodId"].ToString());
                    objPaymentMethods.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objPaymentMethods.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);

                    //objPaymentMethods.PaymentMode = (dr["PaymentMode"] != DBNull.Value ? Convert.ToBoolean(dr["PaymentMode"]) : false);


                    lstPaymentMethods.Add(objPaymentMethods);
                }

            }
            return lstPaymentMethods;
        }

        #endregion


    }
}
