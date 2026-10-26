using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class ServiceCategories
    {
        DAL.ServiceCategories _ServiceCategory = new DAL.ServiceCategories();

        #region Methods

        public Int64 DeleteServiceCategory(Int64 ServiceCategoryId)
        {
            Int64 _status = 0;
            if (ServiceCategoryId != 0)
            {
                _status = _ServiceCategory.DeleteServiceCategory(ServiceCategoryId);
            }
            return _status;
        }

        public Int64 UpdateServiceCategoryStatus(Int64 ServiceCategoryId)
        {
            Int64 _status = 0;
            if (ServiceCategoryId != 0)
            {
                _status = _ServiceCategory.UpdateServiceCategoryStatus(ServiceCategoryId);
            }
            return _status;
        }

        public Int64 InsertServiceCategory(Entities.ServiceCategories objServiceCategories)
        {
            Int64 _status = 0;
            if (objServiceCategories != null)
            {
                _status = _ServiceCategory.InsertServiceCategory(objServiceCategories);
            }
            return _status;
        }

        #endregion

        #region Entity Loading

        public List<Entities.ServiceCategories> GetServiceCategoriesListByVariable(Int64 ChapterId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _ServiceCategory.GetServiceCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
            List<Entities.ServiceCategories> lstProjectCategory = new List<Entities.ServiceCategories>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _ServiceCategory.GetServiceCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceCategories objServiceCategories = new Entities.ServiceCategories();

                    objServiceCategories.RId = Convert.ToInt64(dr["Rid"]);
                    objServiceCategories.ServiceCategoryId = Convert.ToInt64(dr["ServiceCategoryId"]);
                    objServiceCategories.CategoryName = dr["CategoryName"].ToString();
                    objServiceCategories.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objServiceCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServiceCategories.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    objServiceCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objServiceCategories.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);

                    lstProjectCategory.Add(objServiceCategories);
                }
            }
            return lstProjectCategory;
        }

        public Entities.ServiceCategories GetServiceCategoryById(Int64 ServiceCategoryId, ref int status)
        {
            DataTable dt = _ServiceCategory.GetServiceCategoryById(ServiceCategoryId, ref status);
            Entities.ServiceCategories objServiceCategories = new Entities.ServiceCategories();

            if (dt.Rows.Count == 1)
            {
                objServiceCategories.ServiceCategoryId = Convert.ToInt64(dt.Rows[0]["ServiceCategoryId"]);
                objServiceCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                objServiceCategories.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objServiceCategories.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
                objServiceCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

            }

            return objServiceCategories;
        }

        public List<Entities.ServiceCategories> GetServiceCategoriesList(ref int status)
        {
            DataTable dt = _ServiceCategory.GetServiceCategoriesList(ref status);
            List<Entities.ServiceCategories> lstProjectCategory = new List<Entities.ServiceCategories>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceCategories objServiceCategories = new Entities.ServiceCategories();

                    objServiceCategories.ServiceCategoryId = Convert.ToInt64(dr["ServiceCategoryId"]);
                    objServiceCategories.CategoryName = dr["CategoryName"].ToString();
                    objServiceCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstProjectCategory.Add(objServiceCategories);
                }
            }

            return lstProjectCategory;
        }

        #endregion

        #region FrontEnd
        public List<Entities.ServiceCategories> FEServiceCategoriesGetListByChapterId(Int64 ChapterId,ref int status)
        {
            DataTable dt = _ServiceCategory.FEServiceCategoriesGetListByChapterId(ChapterId,ref status);
            List<Entities.ServiceCategories> lstProjectCategory = new List<Entities.ServiceCategories>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceCategories objServiceCategories = new Entities.ServiceCategories();

                    objServiceCategories.ServiceCategoryId = Convert.ToInt64(dr["ServiceCategoryId"]);
                    objServiceCategories.CategoryName = dr["CategoryName"].ToString();
                    objServiceCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstProjectCategory.Add(objServiceCategories);
                }
            }

            return lstProjectCategory;
        }

        #endregion

        #region API
        public Entities.ServiceCategories ServiceCategoriesGetByChapterId(Int64 ChapterId, ref int status)
        {
            DataTable dt = _ServiceCategory.ServiceCategoriesGetByChapterId(ChapterId, ref status);
            Entities.ServiceCategories objServiceCategories = new Entities.ServiceCategories();

            if (dt.Rows.Count == 1)
            {
                objServiceCategories.ServiceCategoryId = Convert.ToInt64(dt.Rows[0]["ServiceCategoryId"]);
                objServiceCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                objServiceCategories.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objServiceCategories.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
                objServiceCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

            }

            return objServiceCategories;
        }

        #endregion
    }
}
