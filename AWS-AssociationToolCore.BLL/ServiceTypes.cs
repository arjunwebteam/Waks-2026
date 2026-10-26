using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
  public  class ServiceTypes
    {
        AWS_AssociationToolCore.DAL.ServiceTypes _ServiceTypes = new AWS_AssociationToolCore.DAL.ServiceTypes();

        #region Methods

        public Int64 InsertServiceTypes(Entities.ServiceTypes objServiceTypes)
        {
            Int64 _status = 0;
            if (objServiceTypes != null)
            {
                _status = _ServiceTypes.InsertServiceType(objServiceTypes);

            }
            return _status;
        }

        public Int64 DeleteServiceType(Int64 ServiceTypeId)
        {
            Int64 _status = 0;
            _status = _ServiceTypes.DeleteServiceType(ServiceTypeId);
            return _status;
        }


        public Int64 UpdateServiceTypeStatus(Int64 ServiceTypeId)
        {
            Int64 _status = 0;
            _status = _ServiceTypes.UpdateServiceTypeStatus(ServiceTypeId);
            return _status;
        }


        public Int64 UpdateServiceTypesDisplayOrder(int DisplayOrder, Int64 ServiceTypeId)
        {
            Int64 _status = 0;
            _status = _ServiceTypes.UpdateServiceTypesDisplayOrder(DisplayOrder, ServiceTypeId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.ServiceTypes> GetServiceTypesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.ServiceTypes> lstServiceTypes = new List<Entities.ServiceTypes>();
            DataTable dt = _ServiceTypes.GetServiceTypesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceTypes objlstServiceTypes = new AWS_AssociationToolCore.Entities.ServiceTypes();

                    objlstServiceTypes.ServiceTypeId = Convert.ToInt64(dr["ServiceTypeId"].ToString());
                    objlstServiceTypes.ServiceType = (dr["ServiceType"] != DBNull.Value ? dr["ServiceType"].ToString() : "");
                    objlstServiceTypes.Descriptione = (dr["Descriptione"] != DBNull.Value ? dr["Descriptione"].ToString() : "");
                    objlstServiceTypes.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objlstServiceTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstServiceTypes.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstServiceTypes.UpdateDate = Convert.ToDateTime(dr["UpdateDate"].ToString());
                  
                    lstServiceTypes.Add(objlstServiceTypes);
                }

 

            }
            return lstServiceTypes;
        }

        public AWS_AssociationToolCore.Entities.ServiceTypes GetServiceTypeById(Int64 ServiceTypeId, ref int status)
        {
            AWS_AssociationToolCore.Entities.ServiceTypes objServiceTypes = new AWS_AssociationToolCore.Entities.ServiceTypes();
            DataTable dt = new DataTable();
            if (ServiceTypeId != 0)
            {
                dt = _ServiceTypes.GetServiceTypeById(ServiceTypeId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objServiceTypes.ServiceTypeId = Convert.ToInt64(dt.Rows[0]["ServiceTypeId"].ToString());
                    objServiceTypes.ServiceType = (dt.Rows[0]["ServiceType"] != DBNull.Value ? dt.Rows[0]["ServiceType"].ToString() : null);
                    objServiceTypes.Descriptione = (dt.Rows[0]["Descriptione"] != DBNull.Value ? dt.Rows[0]["Descriptione"].ToString() : null);
                    objServiceTypes.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                    objServiceTypes.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["OrderNo"]) : 0);
                    objServiceTypes.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objServiceTypes.UpdateDate = Convert.ToDateTime(dt.Rows[0]["UpdateDate"].ToString());

                     
                }
            }
            return objServiceTypes;
        }

        public List<AWS_AssociationToolCore.Entities.ServiceTypes> GetServiceTypesListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.ServiceTypes> lstServiceTypes = new List<AWS_AssociationToolCore.Entities.ServiceTypes>();
            DataTable dt = _ServiceTypes.GetServiceTypesListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _ServiceTypes.GetServiceTypesListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceTypes objServiceTypes = new AWS_AssociationToolCore.Entities.ServiceTypes();

                    objServiceTypes.RId = Convert.ToInt32(dr["RId"].ToString());
                    objServiceTypes.ServiceTypeId = Convert.ToInt64(dr["ServiceTypeId"].ToString());
                    objServiceTypes.ServiceType = (dr["ServiceType"] != DBNull.Value ? dr["ServiceType"].ToString() : "");
                    objServiceTypes.Descriptione = (dr["Descriptione"] != DBNull.Value ? dr["Descriptione"].ToString() : "");
                    objServiceTypes.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objServiceTypes.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServiceTypes.UpdateDate = Convert.ToDateTime(dr["UpdateDate"].ToString());

                    lstServiceTypes.Add(objServiceTypes);
                }
            }
            return lstServiceTypes;
        }

        #endregion
    }
}
