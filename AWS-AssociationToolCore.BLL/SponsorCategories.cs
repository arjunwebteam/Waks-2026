using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class SponsorCategories
    {
        AWS_AssociationToolCore.DAL.SponsorCategories _SponsorCategories = new AWS_AssociationToolCore.DAL.SponsorCategories();

        #region Methods

        public Int64 InsertSponsorCategories(Entities.SponsorCategories objSponsorCategories)
        {
            Int64 _status = 0;
            if (objSponsorCategories != null)
            {
                _status = _SponsorCategories.InsertSponsorCategories(objSponsorCategories);

            }
            return _status;
        }

        public Int64 DeleteSponsorCategory(Int64 SponsorCategoryId)
        {
            Int64 _status = 0;
            _status = _SponsorCategories.DeleteSponsorCategory(SponsorCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.SponsorCategories> GetSponsorCategoriesList(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.SponsorCategories> lstSponsorCategories = new List<Entities.SponsorCategories>();
            DataTable dt = _SponsorCategories.GetSponsorCategoriesList(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.SponsorCategories objlstSponsorCategories = new AWS_AssociationToolCore.Entities.SponsorCategories();

                    objlstSponsorCategories.SponsorCategoryId = Convert.ToInt64(dr["SponsorCategoryId"].ToString());
                    objlstSponsorCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstSponsorCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstSponsorCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objlstSponsorCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstSponsorCategories.Add(objlstSponsorCategories);
                }

            }
            return lstSponsorCategories;
        }

        public AWS_AssociationToolCore.Entities.SponsorCategories GetSponsorCategoryById(Int64 SponsorCategoryId, ref int status)
        {
            AWS_AssociationToolCore.Entities.SponsorCategories objSponsorCategories = new AWS_AssociationToolCore.Entities.SponsorCategories();
            DataTable dt = new DataTable();
            if (SponsorCategoryId != 0)
            {
                dt = _SponsorCategories.GetSponsorCategoryById(SponsorCategoryId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objSponsorCategories.SponsorCategoryId = Convert.ToInt64(dt.Rows[0]["SponsorCategoryId"].ToString());
                    objSponsorCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                    objSponsorCategories.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objSponsorCategories.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());
                    objSponsorCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                    
                    objSponsorCategories.IsActive = (dt.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["IsActive"]) : false);
                }
            }
            return objSponsorCategories;
        }

        public List<AWS_AssociationToolCore.Entities.SponsorCategories> GetSponsorCategoriesListByVariable(Int64 ChapterId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.SponsorCategories> lstSponsorCategories = new List<AWS_AssociationToolCore.Entities.SponsorCategories>();
            DataTable dt = _SponsorCategories.GetSponsorCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _SponsorCategories.GetSponsorCategoriesListByVariable(ChapterId,Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.SponsorCategories objSponsorCategories = new AWS_AssociationToolCore.Entities.SponsorCategories();

                    objSponsorCategories.RId = Convert.ToInt32(dr["RId"].ToString());
                    objSponsorCategories.SponsorCategoryId = Convert.ToInt32(dr["SponsorCategoryId"].ToString());
                    objSponsorCategories.CategoryName = dr["CategoryName"].ToString();
                    objSponsorCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objSponsorCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objSponsorCategories.SponsorsCount = Convert.ToInt32(dr["SponsorsCount"].ToString());
                    objSponsorCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objSponsorCategories.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);           
                    objSponsorCategories.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objSponsorCategories.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"].ToString()) : 0);
                    lstSponsorCategories.Add(objSponsorCategories);
                }
            }
            return lstSponsorCategories;
        }

        public List<AWS_AssociationToolCore.Entities.SponsorCategories> GetSponsorCategoriesListByChapterId(Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.SponsorCategories> lstSponsorCategories = new List<Entities.SponsorCategories>();
            DataTable dt = _SponsorCategories.GetSponsorCategoriesListByChapterId(ChapterId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.SponsorCategories objlstSponsorCategories = new AWS_AssociationToolCore.Entities.SponsorCategories();

                    objlstSponsorCategories.SponsorCategoryId = Convert.ToInt64(dr["SponsorCategoryId"].ToString());
                    objlstSponsorCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstSponsorCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstSponsorCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objlstSponsorCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objlstSponsorCategories.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objlstSponsorCategories.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);

                    lstSponsorCategories.Add(objlstSponsorCategories);
                }

            }
            return lstSponsorCategories;
        }
        public Int64 UpdateSponsorcatrgoryStatus(Int64 SponsorCategoryId)
        {
            Int64 _status = 0;
            _status = _SponsorCategories.UpdateSponsorcatrgoryStatus(SponsorCategoryId);
            return _status;
        }

        public Int64 UpdateSponsorsCategoryDisplayOrder(int DisplayOrder, Int64 SponsorCategoryId)
        {
            Int64 _status = 0;
            _status = _SponsorCategories.UpdateSponsorsCategoryDisplayOrder(DisplayOrder, SponsorCategoryId);
            return _status;
        }


        #endregion
    }
}
