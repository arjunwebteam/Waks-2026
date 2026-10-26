using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
  public  class Photos
    {
        AWS_AssociationToolCore.DAL.Photos _Photos = new AWS_AssociationToolCore.DAL.Photos();
        //string logourl = System.Configuration.ConfigurationManager.AppSettings["uploadPath"] + "photogallery/thumb/";

        #region Methods

        public Int64 InsertPhotos(Entities.Photos objPhotos, ref string imageurl)
        {
            Int64 _status = 0;
            if (objPhotos != null)
            {
                _status = _Photos.InsertPhotos(objPhotos,ref imageurl);

            }
            return _status;
        }

        public Int64 DeletePhotos(Int64 PhotoId)
        {
            Int64 _status = 0;
            _status = _Photos.DeletePhotos(PhotoId);
            return _status;
        }

        public Int64 DeleteAllPhotos(string PhotoIds,ref string ImageUrl)
        {
            Int64 _status = 0;
            _status = _Photos.DeleteAllPhotos(PhotoIds,ref ImageUrl);
            return _status;
        }

        public Int64 PhotosDefaultPhoto(Int64 PhotoId, Int64 PhotoCategoryId)
        {
            Int64 _status = 0;
            _status = _Photos.PhotosDefaultPhoto(PhotoId, PhotoCategoryId);
            return _status;
        }

        public Int64 UpdatePhotosDisplayOrder(int DisplayOrder, Int64 PhotoId)
        {
            Int64 _status = 0;
            _status = _Photos.UpdatePhotosDisplayOrder(DisplayOrder, PhotoId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.Photos> GetPhotosList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Photos> lstPhotos = new List<Entities.Photos>();
            DataTable dt = _Photos.GetPhotosList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Photos objlstPhotos = new AWS_AssociationToolCore.Entities.Photos();
                    objlstPhotos.PhotoId = Convert.ToInt32(dr["PhotoId"].ToString());
                    objlstPhotos.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                    objlstPhotos.ImageUrl = dr["ImageUrl"].ToString();
                    objlstPhotos.ImageDescription = dr["ImageDescription"].ToString();
                    objlstPhotos.AlbumLink = dr["AlbumLink"].ToString();
                    objlstPhotos.DefaultImage =Convert.ToBoolean(dr["DefaultImage"]);
                    objlstPhotos.DisplayOrder =Convert.ToInt32(dr["DisplayOrder"].ToString());
                    objlstPhotos.IsActive =Convert.ToBoolean(dr["State"]);
                    objlstPhotos.IsHome =Convert.ToBoolean(dr["IsHome"].ToString());
                    objlstPhotos.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstPhotos.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    lstPhotos.Add(objlstPhotos);
                }

            }
            return lstPhotos;
        }

        public AWS_AssociationToolCore.Entities.Photos GetPhotosById(Int64 PhotoId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Photos objPhotos = new AWS_AssociationToolCore.Entities.Photos();
            DataTable dt = new DataTable();
            if (PhotoId != 0)
            {
                dt = _Photos.GetPhotosById(PhotoId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objPhotos.PhotoId = Convert.ToInt32(dt.Rows[0]["PhotoId"].ToString());
                    objPhotos.PhotoCategoryId = Convert.ToInt32(dt.Rows[0]["PhotoCategoryId"].ToString());
                    objPhotos.ImageUrl = dt.Rows[0]["ImageUrl"].ToString();
                    objPhotos.ImageDescription = dt.Rows[0]["CategoryName"].ToString();
                    objPhotos.CategoryName = dt.Rows[0]["ImageDescription"].ToString();
                    objPhotos.AlbumLink = dt.Rows[0]["AlbumLink"].ToString();
                    objPhotos.IsHome =Convert.ToBoolean(dt.Rows[0]["IsHome"].ToString());
                    objPhotos.DefaultImage =Convert.ToBoolean( dt.Rows[0]["DefaultImage"].ToString());
                    objPhotos.DisplayOrder =Convert.ToInt32(dt.Rows[0]["DisplayOrder"].ToString());
                    objPhotos.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objPhotos.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());

                }
            }
            return objPhotos;
        }

        public List<AWS_AssociationToolCore.Entities.Photos> GetPhotosListByVariable(Int64 PhotoCategoryId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Photos> lstPhotos = new List<AWS_AssociationToolCore.Entities.Photos>();
            DataTable dt = _Photos.GetPhotosListByVariable(PhotoCategoryId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Photos.GetPhotosListByVariable(PhotoCategoryId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Photos objPhotos = new AWS_AssociationToolCore.Entities.Photos();

                    objPhotos.PhotoId = Convert.ToInt64(dr["PhotoId"].ToString());
                    objPhotos.PhotoCategoryId = Convert.ToInt64(dr["PhotoCategoryId"].ToString());
                    objPhotos.ImageUrl = dr["ImageUrl"].ToString();
                    objPhotos.ImageDescription = dr["ImageDescription"].ToString();
                    objPhotos.AlbumLink = dr["AlbumLink"].ToString();
                    objPhotos.DefaultImage = Convert.ToBoolean(dr["DefaultImage"].ToString());
                    objPhotos.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"].ToString());
                    objPhotos.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPhotos.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                   

                    lstPhotos.Add(objPhotos);
                }
            }
            return lstPhotos;
        }

        #endregion

        #region Front-end

        public List<AWS_AssociationToolCore.Entities.Photos> FEGetPhotosListById(string PhotoCategoryName, ref List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Photos> lstPhotos = new List<Entities.Photos>();
            DataSet ds = _Photos.FEGetPhotosListById(PhotoCategoryName, ref status);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Photos objPhotos = new AWS_AssociationToolCore.Entities.Photos();

                    objPhotos.PhotoId = Convert.ToInt32(dr["PhotoId"].ToString());
                    objPhotos.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                    objPhotos.CategoryName = dr["CategoryName"].ToString();
                    objPhotos.ImageUrl = dr["ImageUrl"].ToString();
                    objPhotos.ImageDescription = dr["ImageDescription"].ToString();
                    objPhotos.AlbumLink = dr["AlbumLink"].ToString();
                    objPhotos.DefaultImage = Convert.ToBoolean(dr["DefaultImage"]);
                    objPhotos.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"].ToString());
                    objPhotos.IsActive = Convert.ToBoolean(dr["IsActive"]);

                    objPhotos.UpdatedBy = dr["UpdatedBy"].ToString();

                    lstPhotos.Add(objPhotos);
                }

            }
            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.PhotoCategories objPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

                    objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                    objPhotoCategories.CategoryName = dr["CategoryName"].ToString();
                    objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPhotoCategories.ImageUrl = dr["ImageUrl"].ToString();
                    objPhotoCategories.PhotoCount = (dr["PhotosCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PhotosCount"]));

                    lstPhotoCategories.Add(objPhotoCategories);
                }

            }

            return lstPhotos;
        }


        public List<Entities.Photos> FEGetPhotoList(ref List<Entities.PhotoCategories> lstPhotoCategories, int Year,Int64 ChapterId, ref int status)
        {
            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();

            DataSet ds = _Photos.FEGetPhotoList(Year, ChapterId, ref status);
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Entities.Photos objlstPhotos = new Entities.Photos();

                    objlstPhotos.PhotoId = Convert.ToInt64(dr["PhotoId"].ToString());
                    objlstPhotos.PhotoCategoryId = Convert.ToInt64(dr["PhotoCategoryId"].ToString());
                    objlstPhotos.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
                    objlstPhotos.ImageDescription = (dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : "");
                    objlstPhotos.AlbumLink = (dr["AlbumLink"] != DBNull.Value ? dr["AlbumLink"].ToString() : "");
                    objlstPhotos.DefaultImage = Convert.ToBoolean(dr["DefaultImage"]);
                    objlstPhotos.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"].ToString());
                    objlstPhotos.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstPhotos.IsHome = Convert.ToBoolean(dr["IsHome"].ToString());
                    objlstPhotos.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstPhotos.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());


                    lstPhotos.Add(objlstPhotos);
                }
            }
            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.PhotoCategories objPhotoCategories = new Entities.PhotoCategories();

                    objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
                    objPhotoCategories.CategoryName = dr["CategoryName"].ToString();
                    objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPhotoCategories.Year = Convert.ToInt32(dr["Year"].ToString());
                    objPhotoCategories.PhotosCount = (dr["PhotosCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PhotosCount"]));
                    objPhotoCategories.AlbumLink = (dr["AlbumLink"] != DBNull.Value ? dr["AlbumLink"].ToString() : "");

                    lstPhotoCategories.Add(objPhotoCategories);
                }
            }
            return lstPhotos;
        }

        public List<Entities.Sponsors> FEGetSponsorsList(Int64 ChapterId, ref List<Entities.SponsorCategories> lstSponsorCategories, ref int status)
        {
            List<Entities.Sponsors> lstSponsors = new List<Entities.Sponsors>();

            DataSet ds = _Photos.FEGetSponsorsList(ChapterId, ref status);
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Entities.Sponsors objSponsors = new Entities.Sponsors();

                    objSponsors.SponsorId = Convert.ToInt64(dr["SponsorId"].ToString());
                    objSponsors.SponsorCategoryId = Convert.ToInt64(dr["SponsorCategoryId"].ToString());
                    objSponsors.LogoUrl = (dr["LogoUrl"] != DBNull.Value ? dr["LogoUrl"].ToString() : "");
                    objSponsors.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : "");
                    objSponsors.RedirectUrl = (dr["RedirectUrl"] != DBNull.Value ? dr["RedirectUrl"].ToString() : "");
                    objSponsors.Target = (dr["Target"] != DBNull.Value ? dr["Target"].ToString() : "");

                    lstSponsors.Add(objSponsors);
                }
            }
            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.SponsorCategories objSponsorCategories = new Entities.SponsorCategories();

                    objSponsorCategories.SponsorCategoryId = Convert.ToInt32(dr["SponsorCategoryId"].ToString());
                    objSponsorCategories.CategoryName = dr["CategoryName"].ToString();
                    objSponsorCategories.SponsorsCount = (dr["SponsorsCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SponsorsCount"]));

                    lstSponsorCategories.Add(objSponsorCategories);
                }
            }
            return lstSponsors;
        }



        public List<AWS_AssociationToolCore.Entities.PhotoCategories> GetPhotoCategories(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            DataTable dt = _Photos.GetPhotoCategories(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PhotoCategories objlstPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

                    objlstPhotoCategories.Year = Convert.ToInt32(dr["Year"].ToString());

                    lstPhotoCategories.Add(objlstPhotoCategories);
                }

            }
            return lstPhotoCategories;
        }





        #endregion
        //#region API
        //public List<AWS_AssociationToolCore.Entities.Photos> APIFEGetPhotosListById(string PhotoCategoryName, ref List<AWS_AssociationToolCore.Entities.PhotoCategories> lstPhotoCategories, ref int status)
        //{
        //    List<AWS_AssociationToolCore.Entities.Photos> lstPhotos = new List<Entities.Photos>();
        //    DataSet ds = _Photos.FEGetPhotosListById(PhotoCategoryName, ref status);
        //    DataTable dt = ds.Tables[0];

        //    if (dt.Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            AWS_AssociationToolCore.Entities.Photos objPhotos = new AWS_AssociationToolCore.Entities.Photos();

        //            objPhotos.PhotoId = Convert.ToInt32(dr["PhotoId"].ToString());
        //            objPhotos.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
        //            objPhotos.CategoryName = dr["CategoryName"].ToString();
        //            objPhotos.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? logourl + dr["ImageUrl"].ToString() : "");
        //            //objPhotos.ImageUrl = dr["ImageUrl"].ToString();
        //            objPhotos.ImageDescription = dr["ImageDescription"].ToString();
        //            objPhotos.AlbumLink = dr["AlbumLink"].ToString();
        //            objPhotos.DefaultImage = Convert.ToBoolean(dr["DefaultImage"]);
        //            objPhotos.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"].ToString());
        //            objPhotos.IsActive = Convert.ToBoolean(dr["IsActive"]);

        //            objPhotos.UpdatedBy = dr["UpdatedBy"].ToString();

        //            lstPhotos.Add(objPhotos);
        //        }

        //    }
        //    if (ds.Tables[1].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[1].Rows)
        //        {
        //            AWS_AssociationToolCore.Entities.PhotoCategories objPhotoCategories = new AWS_AssociationToolCore.Entities.PhotoCategories();

        //            objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
        //            objPhotoCategories.CategoryName = dr["CategoryName"].ToString();
        //            objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
        //            objPhotoCategories.ImageUrl = dr["ImageUrl"].ToString();
        //            objPhotoCategories.PhotoCount = (dr["PhotosCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PhotosCount"]));

        //            lstPhotoCategories.Add(objPhotoCategories);
        //        }

        //    }

        //    return lstPhotos;
        //}

        //public List<Entities.Photos> APIFEGetPhotoList(ref List<Entities.PhotoCategories> lstPhotoCategories, int Year,Int64 ChapterId,  ref int status)
        //{
        //    List<Entities.Photos> lstPhotos = new List<Entities.Photos>();

        //    DataSet ds = _Photos.FEGetPhotoList(Year,ChapterId, ref status);
        //    if (ds.Tables[0].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            Entities.Photos objlstPhotos = new Entities.Photos();

        //            objlstPhotos.PhotoId = Convert.ToInt64(dr["PhotoId"].ToString());
        //            objlstPhotos.PhotoCategoryId = Convert.ToInt64(dr["PhotoCategoryId"].ToString());
        //            objlstPhotos.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? logourl + dr["ImageUrl"].ToString() : "");
        //            //objlstPhotos.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
        //            objlstPhotos.ImageDescription = (dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : "");
        //            objlstPhotos.AlbumLink = (dr["AlbumLink"] != DBNull.Value ? dr["AlbumLink"].ToString() : "");
        //            objlstPhotos.DefaultImage = Convert.ToBoolean(dr["DefaultImage"]);
        //            objlstPhotos.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"].ToString());
        //            objlstPhotos.IsActive = Convert.ToBoolean(dr["IsActive"]);
        //            objlstPhotos.IsHome = Convert.ToBoolean(dr["IsHome"].ToString());
        //            objlstPhotos.UpdatedBy = dr["UpdatedBy"].ToString();
        //            objlstPhotos.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());


        //            lstPhotos.Add(objlstPhotos);
        //        }
        //    }
        //    if (ds.Tables[1].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[1].Rows)
        //        {
        //            Entities.PhotoCategories objPhotoCategories = new Entities.PhotoCategories();

        //            objPhotoCategories.PhotoCategoryId = Convert.ToInt32(dr["PhotoCategoryId"].ToString());
        //            objPhotoCategories.CategoryName = dr["CategoryName"].ToString();
        //            objPhotoCategories.UpdatedBy = dr["UpdatedBy"].ToString();
        //            objPhotoCategories.Year = Convert.ToInt32(dr["Year"].ToString());
        //            objPhotoCategories.PhotosCount = (dr["PhotosCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PhotosCount"]));

        //            lstPhotoCategories.Add(objPhotoCategories);
        //        }
        //    }
        //    return lstPhotos;
        //}
        //#endregion
    }
}
