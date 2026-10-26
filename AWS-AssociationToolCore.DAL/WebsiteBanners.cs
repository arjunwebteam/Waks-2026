using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
  public  class WebsiteBanners
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public DataTable GetWebsiteBannersList(ref int qstatus)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("WebsiteBannersGetList", ref _sqlP);
                qstatus = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdateWebsiteBannersStatus(Int64 WebsiteBannerId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@WebsiteBannerId",WebsiteBannerId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("WebsiteBannersUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertWebsiteBanners(Entities.WebsiteBanners objWebsiteBanners, ref string BannerUrl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@WebsiteBannerId",objWebsiteBanners.WebsiteBannerId),
                    new SqlParameter("@ChapterId",(objWebsiteBanners.ChapterId==0?(object)DBNull.Value:objWebsiteBanners.ChapterId)),
                    new SqlParameter("@BannerTitle",objWebsiteBanners.BannerTitle),
                    new SqlParameter("@BannerUrl",BannerUrl),
                    new SqlParameter("@RedirectUrl",(objWebsiteBanners.RedirectUrl == null ?DBNull.Value:(object)objWebsiteBanners.RedirectUrl.Trim())),
                    new SqlParameter("@OrderNo",(objWebsiteBanners.OrderNo == 0 ?DBNull.Value:(object)objWebsiteBanners.OrderNo)),
                    new SqlParameter("@Target",(objWebsiteBanners.Target == null ?DBNull.Value:(object)objWebsiteBanners.Target.Trim())),
                    new SqlParameter("@IsActive",objWebsiteBanners.IsActive),
                    new SqlParameter("@UpdatedBy",objWebsiteBanners.UpdatedBy),
                    new SqlParameter("@UpdatedTime",objWebsiteBanners.UpdatedTime),
                    new SqlParameter("@QStatus",0)
                    };
                _sqlP[3].SqlDbType = SqlDbType.NVarChar;
                _sqlP[3].Size = 512;
                _sqlP[3].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[10].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("WebsiteBannersInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[10].Value);

                BannerUrl = _sqlP[3].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetWebsiteBannerListByVariable(Int64 ChapterId,string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {   new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("WebsiteBannersGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetWebsiteBannerById(Int64 WebsiteBannerId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@WebsiteBannerId",WebsiteBannerId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("WebsiteBannersGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteWebsiteBanner(Int64 WebsiteBannerId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@WebsiteBannerId",WebsiteBannerId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("WebsiteBannersDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateWebsiteBannersOrderNo(int OrderNo, Int64 WebsiteBannerId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@WebsiteBannerId",WebsiteBannerId),
                    new SqlParameter("@OrderNo",OrderNo),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("WebsiteBannersUpdateOrderNo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetWebsiteBannersListByChapterId(Int64 ChapterId, ref int qstatus)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("WebsiteBannersGetListByChapterId", ref _sqlP);
                qstatus = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
