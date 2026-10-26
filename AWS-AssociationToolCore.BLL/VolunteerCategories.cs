using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
   public class VolunteerCategories
    {
        AWS_AssociationToolCore.DAL.VolunteerCategories _VolunteerCategories = new AWS_AssociationToolCore.DAL.VolunteerCategories();

        #region Methods

        public Int64 InsertVolunteerCategories(Entities.VolunteerCategories objVolunteerCategories)
        {
            Int64 _status = 0;
            if (objVolunteerCategories != null)
            {
                _status = _VolunteerCategories.InsertVolunteerCategories(objVolunteerCategories);

            }
            return _status;
        }

        public Int64 DeleteVolunteerCategory(Int64 VolunteerCategoryId)
        {
            Int64 _status = 0;
            _status = _VolunteerCategories.DeleteVolunteerCategory(VolunteerCategoryId);
            return _status;
        }

        public Int64 UpdateVolunteerCategoryStatus(Int64 VolunteerCategoryId)
        {
            Int64 _status = 0;
            _status = _VolunteerCategories.UpdateVolunteerCategoryStatus(VolunteerCategoryId);
            return _status;
        }

        public Int64 UpdateVolunteerCategoryDisplayOrder(int DisplayOrder, Int64 VolunteerCategoryId)
        {
            Int64 _status = 0;
            _status = _VolunteerCategories.UpdateVolunteerCategoryDisplayOrder(DisplayOrder, VolunteerCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.VolunteerCategories> GetVolunteerCategoriesList(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.VolunteerCategories> lstVolunteerCategories = new List<Entities.VolunteerCategories>();
            DataTable dt = _VolunteerCategories.GetVolunteerCategoriesList(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.VolunteerCategories objlstVolunteerCategories = new AWS_AssociationToolCore.Entities.VolunteerCategories();

                    objlstVolunteerCategories.VolunteerCategoryId = Convert.ToInt64(dr["VolunteerCategoryId"].ToString());
                    objlstVolunteerCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstVolunteerCategories.Type = (dr["Type"] != DBNull.Value ? dr["Type"].ToString() : "");
                    objlstVolunteerCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstVolunteerCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstVolunteerCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objlstVolunteerCategories.VolunteerHours = (dr["VolunteerHours"] != DBNull.Value ? Convert.ToInt32(dr["VolunteerHours"]) : 0);
               


                    lstVolunteerCategories.Add(objlstVolunteerCategories);
                }

            }
            return lstVolunteerCategories;
        }

        public List<AWS_AssociationToolCore.Entities.VolunteerCategories> GetVolunteerCategoriesListByChapterId(Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.VolunteerCategories> lstVolunteerCategories = new List<Entities.VolunteerCategories>();
            DataTable dt = _VolunteerCategories.GetVolunteerCategoriesListByChapterId(ChapterId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.VolunteerCategories objlstVolunteerCategories = new AWS_AssociationToolCore.Entities.VolunteerCategories();

                    objlstVolunteerCategories.VolunteerCategoryId = Convert.ToInt64(dr["VolunteerCategoryId"].ToString());
                    objlstVolunteerCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstVolunteerCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstVolunteerCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstVolunteerCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objlstVolunteerCategories.Type = (dr["Type"] != DBNull.Value
                        ? Convert.ToString(dr["Type"])
                        : string.Empty);


                    lstVolunteerCategories.Add(objlstVolunteerCategories);
                }

            }
            return lstVolunteerCategories;
        }

        public List<AWS_AssociationToolCore.Entities.VolunteerCategories> VolunteerCategoriesGetChapterId(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.VolunteerCategories> lstVolunteerCategories = new List<Entities.VolunteerCategories>();
            DataTable dt = _VolunteerCategories.VolunteerCategoriesGetChapterId(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.VolunteerCategories objlstVolunteerCategories = new AWS_AssociationToolCore.Entities.VolunteerCategories();

                    objlstVolunteerCategories.VolunteerCategoryId = Convert.ToInt64(dr["VolunteerCategoryId"].ToString());
                    objlstVolunteerCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstVolunteerCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objlstVolunteerCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstVolunteerCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);



                    lstVolunteerCategories.Add(objlstVolunteerCategories);
                }

            }
            return lstVolunteerCategories;
        }



        public AWS_AssociationToolCore.Entities.VolunteerCategories GetVolunteerCategoryById(Int64 VolunteerCategoryId, ref int status)
        {
            AWS_AssociationToolCore.Entities.VolunteerCategories objVolunteerCategories = new AWS_AssociationToolCore.Entities.VolunteerCategories();
            DataTable dt = new DataTable();
            if (VolunteerCategoryId != 0)
            {
                dt = _VolunteerCategories.GetVolunteerCategoryById(VolunteerCategoryId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objVolunteerCategories.VolunteerCategoryId = Convert.ToInt64(dt.Rows[0]["VolunteerCategoryId"].ToString());
                    objVolunteerCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                    objVolunteerCategories.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["OrderNo"]) : 0);
                    objVolunteerCategories.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objVolunteerCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["ChapterId"]) : 0);
                    objVolunteerCategories.VolunteerHours = (dt.Rows[0]["VolunteerHours"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["VolunteerHours"]) : 0);
                    objVolunteerCategories.Type = (dt.Rows[0]["Type"] != DBNull.Value
                        ? Convert.ToString(dt.Rows[0]["Type"])
                        : string.Empty);
                }
            }
            return objVolunteerCategories;
        }

        public List<AWS_AssociationToolCore.Entities.VolunteerCategories> GetVolunteerCategoriesListByVariable(Int64 ChapterId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.VolunteerCategories> lstVolunteerCategories = new List<AWS_AssociationToolCore.Entities.VolunteerCategories>();
            DataTable dt = _VolunteerCategories.GetVolunteerCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _VolunteerCategories.GetVolunteerCategoriesListByVariable(ChapterId,Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.VolunteerCategories objVolunteerCategories = new AWS_AssociationToolCore.Entities.VolunteerCategories();

                    objVolunteerCategories.RId = Convert.ToInt64(dr["RId"].ToString());
                    objVolunteerCategories.VolunteerCategoryId = Convert.ToInt64(dr["VolunteerCategoryId"].ToString());
                    objVolunteerCategories.CategoryName = dr["CategoryName"].ToString();
                    objVolunteerCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
                    objVolunteerCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objVolunteerCategories.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objVolunteerCategories.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);
                    objVolunteerCategories.Type = (dr["Type"] != DBNull.Value ? dr["Type"].ToString() : null);
                    objVolunteerCategories.VolunteerHours = (dr["VolunteerHours"] != DBNull.Value ? Convert.ToInt32(dr["VolunteerHours"]) : 0);

                    lstVolunteerCategories.Add(objVolunteerCategories);
                }
            }
            return lstVolunteerCategories;
        }

        #endregion

    }
}
