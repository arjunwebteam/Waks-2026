using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;

namespace AWS_AssociationToolCore.BLL
{
    public class ExhibitsRegistrations
    {
        AWS_AssociationToolCore.DAL.ExhibitsRegistrations _ExhibitsRegistrations = new AWS_AssociationToolCore.DAL.ExhibitsRegistrations();

        #region Methods

        public Int64 InsertExhibitsRegistrations(Entities.ExhibitsRegistrations objExhibitsRegistrations, ref Int64 ExhibitId)
        {
            Int64 _status = 0;
            if (objExhibitsRegistrations != null)
            {
                _status = _ExhibitsRegistrations.InsertExhibitsRegistrations(objExhibitsRegistrations, ref ExhibitId);

            }
            return _status;
        }

        public Int64 AdminExhibitsRegistrationsUpdate(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            Int64 _status = 0;
            if (objExhibitsRegistrations != null)
            {
                _status = _ExhibitsRegistrations.AdminExhibitsRegistrationsUpdate(objExhibitsRegistrations);

            }
            return _status;
        }

        public Int64 UpdateExhibitsRegistrations(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            Int64 _status = 0;
            if (objExhibitsRegistrations != null)
            {
                _status = _ExhibitsRegistrations.UpdateExhibitsRegistrations(objExhibitsRegistrations);

            }
            return _status;
        }

        public Int64 DeleteExhibitsRegistrations(Int64 ExhibitId)
        {
            Int64 _status = 0;
            _status = _ExhibitsRegistrations.DeleteExhibitsRegistrations(ExhibitId);
            return _status;
        }

        public Int64 UpdateExhibitsRegistrationsStatus(Int64 ExhibitId)
        {
            Int64 _status = 0;
            _status = _ExhibitsRegistrations.UpdateExhibitsRegistrationsStatus(ExhibitId);
            return _status;
        }

        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.ExhibitsRegistrations GetExhibitsRegistrationsById(Int64 ExhibitId, ref int status)
        {
            AWS_AssociationToolCore.Entities.ExhibitsRegistrations objExhibitsRegistrations = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();
            DataTable dt = new DataTable();
            if (ExhibitId != 0)
            {
                dt = _ExhibitsRegistrations.GetExhibitsRegistrationsById(ExhibitId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objExhibitsRegistrations.ExhibitId = Convert.ToInt64(dt.Rows[0]["ExhibitId"].ToString());
                    objExhibitsRegistrations.FirstName = dt.Rows[0]["FirstName"].ToString();
                    objExhibitsRegistrations.LastName = dt.Rows[0]["LastName"].ToString();
                    objExhibitsRegistrations.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : "");
                    objExhibitsRegistrations.Phone = (dt.Rows[0]["Phone"] != DBNull.Value ? dt.Rows[0]["Phone"].ToString() : "");
                    objExhibitsRegistrations.Address1 = (dt.Rows[0]["Address1"] != DBNull.Value ? dt.Rows[0]["Address1"].ToString() : "");
                    objExhibitsRegistrations.Address2 = (dt.Rows[0]["Address2"] != DBNull.Value ? dt.Rows[0]["Address2"].ToString() : "");
                    objExhibitsRegistrations.NoOfBooths = (dt.Rows[0]["NoOfBooths"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["NoOfBooths"]) : 0);
                    objExhibitsRegistrations.SigeOfBooth = (dt.Rows[0]["SigeOfBooth"] != DBNull.Value ? dt.Rows[0]["SigeOfBooth"].ToString() : "");
                    objExhibitsRegistrations.BusinessName = (dt.Rows[0]["BusinessName"] != DBNull.Value ? dt.Rows[0]["BusinessName"].ToString() : "");
                    objExhibitsRegistrations.Category = (dt.Rows[0]["Category"] != DBNull.Value ? dt.Rows[0]["Category"].ToString() : "");
                    objExhibitsRegistrations.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : "");
                    objExhibitsRegistrations.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : "");
                    objExhibitsRegistrations.Zip = (dt.Rows[0]["Zip"] != DBNull.Value ? dt.Rows[0]["Zip"].ToString() : ""); 
                    objExhibitsRegistrations.Zip = (dt.Rows[0]["Zip"] != DBNull.Value ? dt.Rows[0]["Zip"].ToString() : "");
                    objExhibitsRegistrations.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objExhibitsRegistrations.Amount = (dt.Rows[0]["Amount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Amount"]) : 0);
                    objExhibitsRegistrations.AmountPaid = (dt.Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountPaid"]) : 0);
                    objExhibitsRegistrations.AmountDue = (dt.Rows[0]["AmountDue"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountDue"]) : 0);
                    objExhibitsRegistrations.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objExhibitsRegistrations.PaymentDate = (dt.Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["PaymentDate"]) : DateTime.MinValue);
                    objExhibitsRegistrations.Date = (objExhibitsRegistrations.PaymentDate != DateTime.MinValue ? objExhibitsRegistrations.PaymentDate.ToString("MM/dd/yyyy") : "");
                    objExhibitsRegistrations.TransactionId = (dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : "");
                    objExhibitsRegistrations.PaymentStatus = (dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : "");
                    objExhibitsRegistrations.CardNumber = (dt.Rows[0]["CardNumber"] != DBNull.Value ? dt.Rows[0]["CardNumber"].ToString() : "");
                    objExhibitsRegistrations.CSVExpiry = (dt.Rows[0]["CSVExpiry"] != DBNull.Value ? dt.Rows[0]["CSVExpiry"].ToString() : "");
                    objExhibitsRegistrations.PaymentBy = (dt.Rows[0]["PaymentBy"] != DBNull.Value ? dt.Rows[0]["PaymentBy"].ToString() : "");
                    objExhibitsRegistrations.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    objExhibitsRegistrations.PromoCode = (dt.Rows[0]["PromoCode"] != DBNull.Value ? dt.Rows[0]["PromoCode"].ToString() : "");
                    objExhibitsRegistrations.AmountDue = (dt.Rows[0]["AmountDue"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountDue"]) : 0);
                    objExhibitsRegistrations.AmountPaid = (dt.Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountPaid"]) : 0);

                }
            }
            return objExhibitsRegistrations;
        }

        public List<AWS_AssociationToolCore.Entities.ExhibitsRegistrations> GetExhibitsRegistrationsListByVariable(string Search, Int64 PaymentStatusId, Int64 PaymentMethodId, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ExhibitsRegistrations> lstExhibitsRegistrations = new List<AWS_AssociationToolCore.Entities.ExhibitsRegistrations>();
            DataTable dt = _ExhibitsRegistrations.GetExhibitsRegistrationsListByVariable(Search, PaymentStatusId, PaymentMethodId, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ExhibitsRegistrations.GetExhibitsRegistrationsListByVariable(Search, PaymentStatusId, PaymentMethodId, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objExhibitsRegistrations = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objExhibitsRegistrations.RId = Convert.ToInt64(dr["RId"].ToString());
                    objExhibitsRegistrations.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objExhibitsRegistrations.FirstName = dr["FirstName"].ToString();
                    objExhibitsRegistrations.LastName = dr["LastName"].ToString();
                    objExhibitsRegistrations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objExhibitsRegistrations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objExhibitsRegistrations.PaymentDate = (dr["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaymentDate"]) : DateTime.MinValue);
                    objExhibitsRegistrations.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : "");
                    objExhibitsRegistrations.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : "");
                    objExhibitsRegistrations.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : "");
                    objExhibitsRegistrations.Category = (dr["Category"] != DBNull.Value ? dr["Category"].ToString() : "");
                    objExhibitsRegistrations.BusinessName = (dr["BusinessName"] != DBNull.Value ? dr["BusinessName"].ToString() : "");
                    objExhibitsRegistrations.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    objExhibitsRegistrations.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());

                    lstExhibitsRegistrations.Add(objExhibitsRegistrations);
                }
            }
            return lstExhibitsRegistrations;
        }

        #endregion

        #region Front-End

        public void FEExhibitsRegistrationsGetList(ref List<Entities.ExhibitsRegistrations> OtherExhibitsRegistrations, 
            ref List<Entities.ExhibitsRegistrations> SilverExhibitsRegistrations,
            ref List<Entities.ExhibitsRegistrations> GoldExhibitsRegistrations,
            ref List<Entities.ExhibitsRegistrations> PlatinumExhibitsRegistrations,
            ref List<Entities.ExhibitsRegistrations> DiamondExhibitsRegistrations,
            ref List<Entities.ExhibitsRegistrations> BenefactorExhibitsRegistrations, ref int Total)
        {
            DataSet ds = _ExhibitsRegistrations.FEExhibitsRegistrationsGetList(ref Total);

            if (ds.Tables[0].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    OtherExhibitsRegistrations.Add(objDonars);
                }
            }

            if (ds.Tables[1].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    SilverExhibitsRegistrations.Add(objDonars);
                }
            }
            if (ds.Tables[2].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    GoldExhibitsRegistrations.Add(objDonars);
                }
            }
            if (ds.Tables[3].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    PlatinumExhibitsRegistrations.Add(objDonars);
                }
            }
            if (ds.Tables[4].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    DiamondExhibitsRegistrations.Add(objDonars);
                }
            }
            if (ds.Tables[5].Rows.Count != 0)
            {

                foreach (DataRow dr in ds.Tables[5].Rows)
                {
                    AWS_AssociationToolCore.Entities.ExhibitsRegistrations objDonars = new AWS_AssociationToolCore.Entities.ExhibitsRegistrations();

                    objDonars.ExhibitId = Convert.ToInt64(dr["ExhibitId"].ToString());
                    objDonars.FirstName = dr["FirstName"].ToString();
                    objDonars.LastName = dr["LastName"].ToString();
                    objDonars.PaymentDate = Convert.ToDateTime(dr["PaymentDate"]);
                    objDonars.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objDonars.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objDonars.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objDonars.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                    BenefactorExhibitsRegistrations.Add(objDonars);
                }
            } 
        }

        #endregion 
    }
}
