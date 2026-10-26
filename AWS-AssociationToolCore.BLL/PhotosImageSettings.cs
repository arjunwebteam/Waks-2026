using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Xml;

namespace AWS_AssociationToolCore.BLL
{
    public class PhotosImageSettings
    {
        AWS_AssociationToolCore.DAL.PhotosImageSettings _PhotosImageSettings = new AWS_AssociationToolCore.DAL.PhotosImageSettings();


        #region Admin

       

        public Int64 UpdatePhotosImageSettingsDetails(Entities.PhotosImageSettings objPhotosImageSettings)
        {
            Int64 _status = 0;
            _status = _PhotosImageSettings.UpdatePhotosImageSettingsDetails(objPhotosImageSettings);

            return _status;
        }



        public Entities.PhotosImageSettings GetPhotosImageSettingsDetails(ref int Status)
        {
            DataTable dt = _PhotosImageSettings.GetPhotosImageSettingsDetails(ref Status);
            Entities.PhotosImageSettings objPhotosImageSettings = new Entities.PhotosImageSettings();

            if (Status == 1 && dt.Rows.Count == 1)
            {
                objPhotosImageSettings.PhotosSettingsid = Convert.ToInt64(dt.Rows[0]["PhotosSettingsid"]);
                objPhotosImageSettings.Eventsize = (dt.Rows[0]["Eventsize"] != DBNull.Value ? dt.Rows[0]["Eventsize"].ToString() : null);
                objPhotosImageSettings.Photosize = (dt.Rows[0]["Photosize"] != DBNull.Value ? dt.Rows[0]["Photosize"].ToString() : null);
                objPhotosImageSettings.Sponsorsize = (dt.Rows[0]["Sponsorsize"] != DBNull.Value ? dt.Rows[0]["Sponsorsize"].ToString() : null);
                objPhotosImageSettings.Bannersize = (dt.Rows[0]["Bannersize"] != DBNull.Value ? dt.Rows[0]["Bannersize"].ToString() : null);
                objPhotosImageSettings.Newssize = (dt.Rows[0]["Newssize"] != DBNull.Value ? dt.Rows[0]["Newssize"].ToString() : null);
                objPhotosImageSettings.committeesize = (dt.Rows[0]["committeesize"] != DBNull.Value ? dt.Rows[0]["committeesize"].ToString() : null);
                objPhotosImageSettings.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : null);
                objPhotosImageSettings.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : null);
                objPhotosImageSettings.Field3 = (dt.Rows[0]["Field3"] != DBNull.Value ? dt.Rows[0]["Field3"].ToString() : null);
                objPhotosImageSettings.IsCultural = (dt.Rows[0]["IsCultural"] != DBNull.Value ? dt.Rows[0]["IsCultural"].ToString() : null);
                objPhotosImageSettings.IsSports = (dt.Rows[0]["IsSports"] != DBNull.Value ? dt.Rows[0]["IsSports"].ToString() : null);
                objPhotosImageSettings.TimeZone = (dt.Rows[0]["TimeZone"] != DBNull.Value ? dt.Rows[0]["TimeZone"].ToString() : null);
                objPhotosImageSettings.IsHelpDocument = (dt.Rows[0]["IsHelpDocument"] != DBNull.Value ? dt.Rows[0]["IsHelpDocument"].ToString() : null);
                objPhotosImageSettings.IsFeaturesDocument = (dt.Rows[0]["IsFeaturesDocument"] != DBNull.Value ? dt.Rows[0]["IsFeaturesDocument"].ToString() : null);

            }
            return objPhotosImageSettings;
        }


        #endregion
    }


}