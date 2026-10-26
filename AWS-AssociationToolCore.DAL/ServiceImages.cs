using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
namespace AWS_AssociationToolCore.DAL
{
   public class ServiceImages
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Method

        public Int64 DeleteServiceImages(Int64 ServiceImageId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceImageId",ServiceImageId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceImagesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertServiceImages(Entities.ServiceImages objServiceImages, ref string Imageurl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                        new SqlParameter("@ServiceImageId",objServiceImages.ServiceImageId),
                        new SqlParameter("@ServiceId",objServiceImages.ServiceId),
                        new SqlParameter("@ImageUrl",Imageurl),
                        new SqlParameter("@IsDefault",objServiceImages.IsDefault),
                        new SqlParameter("@ImageDescription",(objServiceImages.ImageDescription != null ? (object)objServiceImages.ImageDescription : DBNull.Value.ToString())),
                        new SqlParameter("@UpdatedBy",objServiceImages.UpdatedBy),
                        new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                        new SqlParameter("@QStatus",0)
                    };
                _sqlP[2].SqlDbType = SqlDbType.NVarChar;
                _sqlP[2].Size = 256;
                _sqlP[2].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceImagesInsert", ref _sqlP);
                Imageurl = _sqlP[2].Value.ToString();
                _status = Convert.ToInt64(_sqlP[7].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

      



        #endregion

        #region Admin Section

        public DataTable GetServiceImagesById(Int64 ServiceImageId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceImageId",ServiceImageId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceImagesGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ServiceImagesGetListByVariable(Int64 ServiceId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@Total",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceImagesGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetServiceImagesList(ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };

                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceImagesList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 ProjectDefaultImage(Int64 ServiceImageId, Int64 ServiceId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceImageId",ServiceImageId),
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ProjectDefaultImage", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public DataTable GetServiceImagesListByVariableee(Int64 ServiceId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServiceImagesGetListByVariableee", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
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
