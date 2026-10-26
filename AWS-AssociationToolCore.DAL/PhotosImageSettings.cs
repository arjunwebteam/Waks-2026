using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
    public class PhotosImageSettings
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Admin

        public Int64 UpdatePhotosImageSettingsDetails(Entities.PhotosImageSettings objPhotosImageSettings)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@PhotosSettingsid",objPhotosImageSettings.PhotosSettingsid),
                    new SqlParameter("@Eventsize",(objPhotosImageSettings.Eventsize == null ?DBNull.Value:(object)objPhotosImageSettings.Eventsize)),
                    new SqlParameter("@Photosize",(objPhotosImageSettings.Photosize == null ?DBNull.Value:(object)objPhotosImageSettings.Photosize)),
                    new SqlParameter("@Sponsorsize",(objPhotosImageSettings.Sponsorsize == null ?DBNull.Value:(object)objPhotosImageSettings.Sponsorsize)),
                    new SqlParameter("@Bannersize",(objPhotosImageSettings.Bannersize == null ?DBNull.Value:(object)objPhotosImageSettings.Bannersize)),
                    new SqlParameter("@Newssize",(objPhotosImageSettings.Newssize == null ?DBNull.Value:(object)objPhotosImageSettings.Newssize)),
                    new SqlParameter("@committeesize",(objPhotosImageSettings.committeesize == null ?DBNull.Value:(object)objPhotosImageSettings.committeesize)),
                    new SqlParameter("@Field1",(objPhotosImageSettings.Field1 == null ?DBNull.Value:(object)objPhotosImageSettings.Field1)),
                    new SqlParameter("@Field2",(objPhotosImageSettings.Field2 == null ?DBNull.Value:(object)objPhotosImageSettings.Field2)),
                    new SqlParameter("@Field3",(objPhotosImageSettings.Field3 == null ?DBNull.Value:(object)objPhotosImageSettings.Field3)),
                    new SqlParameter("@InsertedBy",objPhotosImageSettings.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objPhotosImageSettings.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@IsCultural",(objPhotosImageSettings.IsCultural == null ?DBNull.Value:(object)objPhotosImageSettings.IsCultural)),
                    new SqlParameter("@IsSports",(objPhotosImageSettings.IsSports == null ?DBNull.Value:(object)objPhotosImageSettings.IsSports)),
                    new SqlParameter("@TimeZone",(objPhotosImageSettings.TimeZone == null ?DBNull.Value:(object)objPhotosImageSettings.TimeZone)),
                    new SqlParameter("@IsHelpDocument",(objPhotosImageSettings.IsHelpDocument == null ?DBNull.Value:(object)objPhotosImageSettings.IsHelpDocument)),
                    new SqlParameter("@IsFeaturesDocument",(objPhotosImageSettings.IsFeaturesDocument == null ?DBNull.Value:(object)objPhotosImageSettings.IsFeaturesDocument))



                    };
                _sqlP[14].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PhotosImageSettings", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[14].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetPhotosImageSettingsDetails(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PhotosImageSettingsGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        #endregion
    }
}
