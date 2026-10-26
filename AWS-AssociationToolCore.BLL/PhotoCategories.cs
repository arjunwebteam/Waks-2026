using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
    public class PhotoCategories
    {
        AWS_AssociationToolCore.DAL.PhotoCategories _PhotoCategories = new AWS_AssociationToolCore.DAL.PhotoCategories();

        #region Methods

        public Int64 InsertPhotoCategories(Entities.PhotoCategories objPhotoCategories)
        {
            Int64 _status = 0;
            if (objPhotoCategories != null)
            {
                _status = _PhotoCategories.InsertPhotoCategories(objPhotoCategories);

            }
            return _status;
        }

        public Int64 DeletePhotoCategories(Int64 PhotoCategoryId)
        {
            Int64 _status = 0;
            _status = _PhotoCategories.DeletePhotoCategories(PhotoCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.PhotoCategories> GetPhotoCategoriesList(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            DataTable dt = _PhotoCategories.GetPhotoCategoriesList(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PhotoCategories objlstPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

                    objlstPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                  


                    objlstPhotoCategories.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    objlstPhotoCategories.Year = (dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0);
                    objlstPhotoCategories.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);


                  
                    objlstPhotoCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objlstPhotoCategories.AlbumLink = (dr["AlbumLink"] != DBNull.Value ? dr["AlbumLink"].ToString() : null);

                    lstPhotoCategories.Add(objlstPhotoCategories);
                }

            }
            return lstPhotoCategories;
        }

        public AWS_AssociationToolCore.Entities.PhotoCategories GetPhotoCategoriesById(Int64 PhotoCategoryId, ref int status)
        {
            AWS_AssociationToolCore.Entities.PhotoCategories objPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();
            DataTable dt = new DataTable();
            if (PhotoCategoryId != 0)
            {
                dt = _PhotoCategories.GetPhotoCategoriesById(PhotoCategoryId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dt.Rows[0]["PhotoCategoryId"].ToString());
                    objPhotoCategories.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                    objPhotoCategories.Year =Convert.ToInt64(dt.Rows[0]["Year"].ToString());
                    objPhotoCategories.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objPhotoCategories.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());
                    objPhotoCategories.AlbumLink = dt.Rows[0]["AlbumLink"].ToString();
                    objPhotoCategories.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

                }
            }
            return objPhotoCategories;
        }
        
        public List<AWS_AssociationToolCore.Entities.PhotoCategories> GetPhotoCategoriesListByVariable(string Search,string Year, string Sort, int PageNo, int Items,Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories = new List<AWS_AssociationToolCore.Entities.PhotoCategories>();
            DataTable dt = _PhotoCategories.GetPhotoCategoriesListByVariable(Search, Year, Sort, PageNo, Items, ChapterId,ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _PhotoCategories.GetPhotoCategoriesListByVariable(Search, Year, Sort, PageNo - 1, Items, ChapterId, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PhotoCategories objPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

                    objPhotoCategories.PhotoCategoryId = Convert.ToInt64(dr["PhotoCategoryId"].ToString());
                    objPhotoCategories.CategoryName = dr["CategoryName"].ToString();

                    objPhotoCategories.Year =Convert.ToInt64(dr["Year"].ToString());
                    objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPhotoCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objPhotoCategories.RId = Convert.ToInt64(dr["RId"].ToString());
                    objPhotoCategories.PhotosCount = Convert.ToInt64(dr["PhotosCount"].ToString());

                    lstPhotoCategories.Add(objPhotoCategories);
                }
            }
            return lstPhotoCategories;
        }

        #endregion

        #region Front-end

        public List<AWS_AssociationToolCore.Entities.PhotoCategories> FEGetPhotoCategoriesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            DataSet ds = _PhotoCategories.FEGetPhotoCategoriesList(ref status);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PhotoCategories objPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

                    objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                    objPhotoCategories.CategoryName = dr["CategoryName"].ToString();
                    objPhotoCategories.AlbumLink = (dr["AlbumLink"] != DBNull.Value ? dr["AlbumLink"].ToString() : "");

                    objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPhotoCategories.ImageUrl = dr["ImageUrl"].ToString();
                    objPhotoCategories.PhotoCount = (dr["PhotosCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PhotosCount"]));

                    lstPhotoCategories.Add(objPhotoCategories);
                }

            }
            return lstPhotoCategories;
        }

        #endregion
    }
}
