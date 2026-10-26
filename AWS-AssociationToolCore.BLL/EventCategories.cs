using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class EventCategories
    {
        AWS_AssociationToolCore.DAL.EventCategories _EventCategories = new AWS_AssociationToolCore.DAL.EventCategories();

        #region Methods

        public Int64 InsertEventCategory(AWS_AssociationToolCore.Entities.EventCategories objEventCategory)
        {

            Int64 _status = 0;
            if (objEventCategory != null)
            {
                _status = _EventCategories.InsertEventCategory(objEventCategory);
            }
            return _status;
        }

        public Int64 DeleteEventCategory(Int64 EventCategoryId)
        {
            Int64 _status = 0;
            _status = _EventCategories.DeleteEventCategoryById(EventCategoryId);

            return _status;
        }

        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.EventCategories GetEventCategoryById(Int64 Id)
        {
            AWS_AssociationToolCore.Entities.EventCategories objEventCategory = new AWS_AssociationToolCore.Entities.EventCategories();
            DataTable dt = new DataTable();
            if (Id != 0)
            {
                dt = _EventCategories.GetEventCategoryById(Id);
                if (dt.Rows.Count == 1)
                {
                    objEventCategory.EventCategoryId = Convert.ToInt64(dt.Rows[0]["EventCategoryId"]);
                    objEventCategory.EventCategory = dt.Rows[0]["EventCategory"].ToString();
                    objEventCategory.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objEventCategory.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"]);
                    objEventCategory.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                }
            }
            return objEventCategory;
        }

        public List<AWS_AssociationToolCore.Entities.EventCategories> GetEventCategoriesList(ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventCategories> lstEventCategories = new List<AWS_AssociationToolCore.Entities.EventCategories>();
            DataTable dt = _EventCategories.GetEventCategoriesList(ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventCategories objEventCategory = new AWS_AssociationToolCore.Entities.EventCategories();

                    objEventCategory.EventCategoryId = Convert.ToInt64(dr["EventCategoryId"]);
                    objEventCategory.ChapterId = Convert.ToInt64(dr["ChapterId"]);
                    objEventCategory.EventCategory = dr["EventCategory"].ToString();
                    objEventCategory.IsActive =Convert.ToBoolean(dr["IsActive"]);
                    objEventCategory.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEventCategory.UpdatedBy = dr["UpdatedBy"].ToString();
                    lstEventCategories.Add(objEventCategory);
                }

            }
            return lstEventCategories;
        }

        public List<AWS_AssociationToolCore.Entities.EventCategories> GetEventCategoriesListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventCategories> lstEventCategorie = new List<AWS_AssociationToolCore.Entities.EventCategories>();
            DataTable dt = _EventCategories.GetEventCategoriesListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _EventCategories.GetEventCategoriesListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventCategories objEventCategorie = new AWS_AssociationToolCore.Entities.EventCategories();

                    objEventCategorie.EventCategoryId = Convert.ToInt64(dr["EventCategoryId"]);
                    objEventCategorie.EventCategory = dr["EventCategory"].ToString();
                    objEventCategorie.IsActive =Convert.ToBoolean(dr["IsActive"]);
                    objEventCategorie.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEventCategorie.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEventCategorie.RId = Convert.ToInt64(dr["RId"]);
                    objEventCategorie.ChapterId = Convert.ToInt64(dr["ChapterId"]);

                    lstEventCategorie.Add(objEventCategorie);
                }
            }
            return lstEventCategorie;
        }

        public List<AWS_AssociationToolCore.Entities.EventCategories> GetEventCategoriesListByChapterId(Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventCategories> lstEventCategories = new List<AWS_AssociationToolCore.Entities.EventCategories>();
            DataTable dt = _EventCategories.GetEventCategoriesListByChapterId(ChapterId, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventCategories objEventCategory = new AWS_AssociationToolCore.Entities.EventCategories();

                    objEventCategory.EventCategoryId = Convert.ToInt64(dr["EventCategoryId"]);
                    objEventCategory.ChapterId = Convert.ToInt64(dr["ChapterId"]);
                    objEventCategory.EventCategory = dr["EventCategory"].ToString();
                    objEventCategory.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objEventCategory.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEventCategory.UpdatedBy = dr["UpdatedBy"].ToString();
                    lstEventCategories.Add(objEventCategory);
                }

            }
            return lstEventCategories;
        }

        #endregion
    }
}
