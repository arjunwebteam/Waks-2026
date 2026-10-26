using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class DonationCategories
    {
        AWS_AssociationToolCore.DAL.DonationCategories _DonationCategories = new AWS_AssociationToolCore.DAL.DonationCategories();

        #region Methods

        public Int64 InsertDonationCategories(Entities.DonationCategories objDonationCategories)
        {
            Int64 _status = 0;
            if (objDonationCategories != null)
            {
                _status = _DonationCategories.InsertDonationCategories(objDonationCategories);

            }
            return _status;
        }

        public Int64 DeleteDonationCategory(Int64 DonationCategoryId)
        {
            Int64 _status = 0;
            _status = _DonationCategories.DeleteDonationCategory(DonationCategoryId);
            return _status;
        }

        public Int64 UpdateDonationCategoryStatus(Int64 DonationCategoryId)
        {
            Int64 _status = 0;
            _status = _DonationCategories.UpdateDonationCategoryStatus(DonationCategoryId);
            return _status;
        }

        public Int64 UpdateDonationCategoryDisplayOrder(int DisplayOrder, Int64 DonationCategoryId)
        {
            Int64 _status = 0;
            _status = _DonationCategories.UpdateDonationCategoryDisplayOrder(DisplayOrder, DonationCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling
        
        public List<AWS_AssociationToolCore.Entities.DonationCategories> GetDonationCategoriesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            DataTable dt = _DonationCategories.GetDonationCategoriesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.DonationCategories objlstDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();

                    objlstDonationCategories.DonationCategoryId = Convert.ToInt64(dr["DonationCategoryId"].ToString());
                    objlstDonationCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstDonationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstDonationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstDonationCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstDonationCategories.Add(objlstDonationCategories);
                }

            }
            return lstDonationCategories;
        }


        public List<AWS_AssociationToolCore.Entities.DonationCategories> DonationCategoriesGetChapterId(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            DataTable dt = _DonationCategories.DonationCategoriesGetChapterId(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.DonationCategories objlstDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();

                    objlstDonationCategories.DonationCategoryId = Convert.ToInt64(dr["DonationCategoryId"].ToString());
                    objlstDonationCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstDonationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstDonationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstDonationCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstDonationCategories.Add(objlstDonationCategories);
                }

            }
            return lstDonationCategories;
        }


        public AWS_AssociationToolCore.Entities.DonationCategories GetDonationCategoryById(Int64 DonationCategoryId, ref int status)
        {
            AWS_AssociationToolCore.Entities.DonationCategories objDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();
            DataTable dt = new DataTable();
            if (DonationCategoryId != 0)
            {
                dt = _DonationCategories.GetDonationCategoryById(DonationCategoryId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objDonationCategories.DonationCategoryId = Convert.ToInt64(dt.Rows[0]["DonationCategoryId"].ToString());
                    objDonationCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                    objDonationCategories.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["OrderNo"]) : 0);
                    objDonationCategories.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objDonationCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

                }
            }
            return objDonationCategories;
        }

        public List<AWS_AssociationToolCore.Entities.DonationCategories> GetDonationCategoriesListByVariable(Int64 ChapterId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.DonationCategories> lstDonationCategories = new List<AWS_AssociationToolCore.Entities.DonationCategories>();
            DataTable dt = _DonationCategories.GetDonationCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _DonationCategories.GetDonationCategoriesListByVariable(ChapterId,Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.DonationCategories objDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();

                    objDonationCategories.RId = Convert.ToInt64(dr["RId"].ToString());
                    objDonationCategories.DonationCategoryId = Convert.ToInt64(dr["DonationCategoryId"].ToString());
                    objDonationCategories.CategoryName = dr["CategoryName"].ToString();
                    objDonationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objDonationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objDonationCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objDonationCategories.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);

                    lstDonationCategories.Add(objDonationCategories);
                }
            }
            return lstDonationCategories;
        }
        
        #endregion

        #region Front-end

        public List<AWS_AssociationToolCore.Entities.DonationCategories> FEDonationCategoriesGetList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            DataTable dt = _DonationCategories.FEDonationCategoriesGetList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.DonationCategories objlstDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();

                    objlstDonationCategories.DonationCategoryId = Convert.ToInt64(dr["DonationCategoryId"].ToString());
                    objlstDonationCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstDonationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstDonationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);

                    lstDonationCategories.Add(objlstDonationCategories);
                }

            }
            return lstDonationCategories;
        }

        public List<AWS_AssociationToolCore.Entities.DonationCategories> FEDonationCategoriesGetListByChapterId(Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            DataTable dt = _DonationCategories.FEDonationCategoriesGetListByChapterId(ChapterId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.DonationCategories objlstDonationCategories = new AWS_AssociationToolCore.Entities.DonationCategories();

                    objlstDonationCategories.DonationCategoryId = Convert.ToInt64(dr["DonationCategoryId"].ToString());
                    objlstDonationCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstDonationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstDonationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);

                    lstDonationCategories.Add(objlstDonationCategories);
                }

            }
            return lstDonationCategories;
        }

        #endregion
    }
}
