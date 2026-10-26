using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
  public  class ServiceImages
    {
        DAL.ServiceImages _ServiceImages = new DAL.ServiceImages();

        #region Methods

        public Int64 DeleteServiceImages(Int64 ServiceImageId)
        {
            Int64 _status = 0;
            if (ServiceImageId != 0)
            {
                _status = _ServiceImages.DeleteServiceImages(ServiceImageId);
            }
            return _status;
        }

        public Int64 InsertServiceImages(Entities.ServiceImages objServiceImages, ref string Imageurl)
        {
            Int64 _status = 0;
            if (objServiceImages != null)
            {
                _status = _ServiceImages.InsertServiceImages(objServiceImages, ref Imageurl);
            }
            return _status;
        }

        public Int64 ImageDefaultImage(Int64 ServiceImageId, Int64 ServiceId)
        {
            Int64 _status = 0;
            _status = _ServiceImages.ProjectDefaultImage(ServiceImageId, ServiceId);
            return _status;
        }

        #endregion

        #region Entity Loading

        public Entities.ServiceImages GetServiceImagesById(Int64 ServiceImagesId, ref int status)
        {
            DataTable dt = _ServiceImages.GetServiceImagesById(ServiceImagesId, ref status);
            Entities.ServiceImages objServiceImages = new Entities.ServiceImages();
            if (dt.Rows.Count == 1)
            {
                objServiceImages.ServiceImageId = Convert.ToInt64(dt.Rows[0]["ServiceImageId"]);
                objServiceImages.ServiceId = Convert.ToInt64(dt.Rows[0]["ServiceId"].ToString());
                objServiceImages.ServiceTitle = dt.Rows[0]["ServiceTitle"].ToString();
                objServiceImages.ImageDescription = dt.Rows[0]["ImageDescription"].ToString();
                objServiceImages.ImageUrl = dt.Rows[0]["ImageUrl"].ToString();
                objServiceImages.IsDefault = Convert.ToBoolean(dt.Rows[0]["IsDefault"].ToString());
            }
            return objServiceImages;
        }

        public List<Entities.ServiceImages> ServiceImagesGetListByVariable(Int64 ServiceId, ref int Total)
        {
            DataTable dt = _ServiceImages.ServiceImagesGetListByVariable(ServiceId, ref Total);
            List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceImages objServiceImages = new Entities.ServiceImages();

                    objServiceImages.ServiceImageId = Convert.ToInt64(dr["ServiceImageId"]);
                    objServiceImages.ServiceId = Convert.ToInt64(dr["ServiceId"].ToString());
                    objServiceImages.ImageUrl = dr["ImageUrl"].ToString();
                    objServiceImages.IsDefault = Convert.ToBoolean(dr["IsDefault"].ToString());
                    lstServiceImages.Add(objServiceImages);
                }
            }
            return lstServiceImages;
        }


        public List<Entities.ServiceImages> GetServiceImagesList(ref int Total)
        {
            List<Entities.ServiceImages> lstContactUs = new List<Entities.ServiceImages>();
            DataTable dt = _ServiceImages.GetServiceImagesList(ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceImages objServiceImages = new Entities.ServiceImages();
                    objServiceImages.RId = Convert.ToInt64(dr["Rid"]);
                    objServiceImages.ServiceImageId = Convert.ToInt64(dr["ServiceImageId"]);
                    objServiceImages.ServiceId = Convert.ToInt64(dr["ServiceId"].ToString());
                    objServiceImages.ImageUrl = dr["ImageUrl"].ToString();
                    objServiceImages.IsDefault = Convert.ToBoolean(dr["IsDefault"].ToString());
                    lstContactUs.Add(objServiceImages);

                }
            }
            return lstContactUs;
        }


        public List<Entities.ServiceImages> GetServiceImagesListByVariableee(Int64 ServiceId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _ServiceImages.GetServiceImagesListByVariableee(ServiceId,Search, Sort, PageNo, Items, ref Total);
            List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _ServiceImages.GetServiceImagesListByVariableee(ServiceId,Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.ServiceImages objServiceImages = new Entities.ServiceImages();

                    objServiceImages.RId = Convert.ToInt64(dr["Rid"]);
                    objServiceImages.ServiceImageId = Convert.ToInt64(dr["ServiceImageId"]);
                    objServiceImages.ServiceId = Convert.ToInt64(dr["ServiceId"].ToString());
                    objServiceImages.ImageUrl = dr["ImageUrl"].ToString();
                    objServiceImages.IsDefault = Convert.ToBoolean(dr["IsDefault"].ToString());
                    objServiceImages.ImageDescription = (dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : null);
                    objServiceImages.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServiceImages.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    lstServiceImages.Add(objServiceImages);
                }
            }
            return lstServiceImages;
        }


        #endregion
    }
}
