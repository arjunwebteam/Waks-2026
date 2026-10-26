using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
    public class PerformanceCategory
    {
        AWS_AssociationToolCore.DAL.PerformanceCategory _PerformanceCategory = new AWS_AssociationToolCore.DAL.PerformanceCategory();

        #region Methods

        public Int64 InsertPerformanceCategory(Entities.PerformanceCategory objPerformanceCategory)
        {
            Int64 _status = 0;
            if (objPerformanceCategory != null)
            {
                _status = _PerformanceCategory.InsertPerformanceCategory(objPerformanceCategory);

            }
            return _status;
        }

        public Int64 DeletePerformanceCategory(Int64 PerformanceCategoryId)
        {
            Int64 _status = 0;
            _status = _PerformanceCategory.DeletePerformanceCategory(PerformanceCategoryId);
            return _status;
        }

        public Int64 UpdatePerformanceCategoryStatus(Int64 PerformanceCategoryId)
        {
            Int64 _status = 0;
            _status = _PerformanceCategory.UpdatePerformanceCategoryStatus(PerformanceCategoryId);
            return _status;
        }
        public Int64 UpdatePerformanceCategoryDisplayOrder(int OrderNo, Int64 PerformanceCategoryId)
        {
            Int64 _status = 0;
            _status = _PerformanceCategory.UpdatePerformanceCategoryDisplayOrder(OrderNo, PerformanceCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.PerformanceCategory> GetPerformanceCategoryList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.PerformanceCategory> lstPerformanceCategory = new List<Entities.PerformanceCategory>();
            DataTable dt = _PerformanceCategory.GetPerformanceCategoryList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PerformanceCategory objlstPerformanceCategory = new AWS_AssociationToolCore.Entities.PerformanceCategory();

                    objlstPerformanceCategory.PerformanceCategoryID = Convert.ToInt64(dr["PerformanceCategoryID"].ToString());
                    objlstPerformanceCategory.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : "");
                    objlstPerformanceCategory.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"]) : 0);
                    objlstPerformanceCategory.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"]) : 0);
                    objlstPerformanceCategory.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstPerformanceCategory.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstPerformanceCategory.InsertedBy = dr["InsertedBy"].ToString();
                    objlstPerformanceCategory.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objlstPerformanceCategory.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstPerformanceCategory.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    
                    lstPerformanceCategory.Add(objlstPerformanceCategory);
                }

            }
            return lstPerformanceCategory;
        }

        public AWS_AssociationToolCore.Entities.PerformanceCategory GetPerformanceCategoryById(Int64 PerformanceCategoryID, ref int status)
        {
            AWS_AssociationToolCore.Entities.PerformanceCategory objPerformanceCategory = new AWS_AssociationToolCore.Entities.PerformanceCategory();
            DataTable dt = new DataTable();
            if (PerformanceCategoryID != 0)
            {
                dt = _PerformanceCategory.GetPerformanceCategoryById(PerformanceCategoryID, ref status);
                if (dt.Rows.Count == 1)
                {
                    objPerformanceCategory.PerformanceCategoryID = Convert.ToInt64(dt.Rows[0]["PerformanceCategoryID"].ToString());
                    objPerformanceCategory.CategoryName = (dt.Rows[0]["CategoryName"] != DBNull.Value ? dt.Rows[0]["CategoryName"].ToString() : "");
                    objPerformanceCategory.MemberAmount = (dt.Rows[0]["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["MemberAmount"]) : 0);
                    objPerformanceCategory.NonMemberAmount = (dt.Rows[0]["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["NonMemberAmount"]) : 0);
                    objPerformanceCategory.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["OrderNo"]) : 0);
                    objPerformanceCategory.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objPerformanceCategory.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                    objPerformanceCategory.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                    objPerformanceCategory.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objPerformanceCategory.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                }
            }
            return objPerformanceCategory;
        }

        public List<AWS_AssociationToolCore.Entities.PerformanceCategory> GetPerformanceCategoryListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PerformanceCategory> lstPerformanceCategory = new List<AWS_AssociationToolCore.Entities.PerformanceCategory>();
            DataTable dt = _PerformanceCategory.GetPerformanceCategoryListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _PerformanceCategory.GetPerformanceCategoryListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PerformanceCategory objPerformanceCategory = new AWS_AssociationToolCore.Entities.PerformanceCategory();

                    objPerformanceCategory.RId = Convert.ToInt64(dr["RId"].ToString());

                    objPerformanceCategory.PerformanceCategoryID = Convert.ToInt64(dr["PerformanceCategoryID"].ToString());
                    objPerformanceCategory.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : "");
                    objPerformanceCategory.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"]) : 0);
                    objPerformanceCategory.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"]) : 0);
                    objPerformanceCategory.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objPerformanceCategory.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objPerformanceCategory.InsertedBy = dr["InsertedBy"].ToString();
                    objPerformanceCategory.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objPerformanceCategory.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPerformanceCategory.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());


                    lstPerformanceCategory.Add(objPerformanceCategory);
                }
            }
            return lstPerformanceCategory;
        }

        #endregion

    }
}
