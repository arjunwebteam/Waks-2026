using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
  public  class PatrikaRegistrations
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public DataTable GetPatrikaRegistrationsList(ref int qstatus)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PatrikaRegistrationsGetList", ref _sqlP);
                qstatus = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdatePatrikaRegistrationsStatus(Int64 PatrikaId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PatrikaId",PatrikaId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PatrikaRegistrationsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertPatrikaRegistrations(Entities.PatrikaRegistrations objPatrikaRegistrations, ref string BannerUrl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@PatrikaId",objPatrikaRegistrations.PatrikaId),
                    new SqlParameter("@ChapterId",(objPatrikaRegistrations.ChapterId==0?(object)DBNull.Value:objPatrikaRegistrations.ChapterId)),
                    new SqlParameter("@Title",objPatrikaRegistrations.Title),
                    new SqlParameter("@BannerUrl",BannerUrl),
                    new SqlParameter("@DocumentUrl",(objPatrikaRegistrations.DocumentUrl == null ?DBNull.Value:(object)objPatrikaRegistrations.DocumentUrl.Trim())),
                    new SqlParameter("@OrderNo",(objPatrikaRegistrations.OrderNo == 0 ?DBNull.Value:(object)objPatrikaRegistrations.OrderNo)),
                    new SqlParameter("@IsActive",objPatrikaRegistrations.IsActive),
                    new SqlParameter("@Field1",(objPatrikaRegistrations.Field1 == null ?DBNull.Value:(object)objPatrikaRegistrations.Field1.Trim())),
                    new SqlParameter("@Field2",(objPatrikaRegistrations.Field2 == null ?DBNull.Value:(object)objPatrikaRegistrations.Field2.Trim())),
                    new SqlParameter("@Field3",(objPatrikaRegistrations.Field3 == null ?DBNull.Value:(object)objPatrikaRegistrations.Field3.Trim())),
                    new SqlParameter("@InsertedBy",objPatrikaRegistrations.InsertedBy),
                    new SqlParameter("@InsertedTime",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objPatrikaRegistrations.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };
                _sqlP[3].SqlDbType = SqlDbType.NVarChar;
                _sqlP[3].Size = 512;
                _sqlP[3].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[14].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PatrikaRegistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[14].Value);

                BannerUrl = _sqlP[3].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetPatrikaRegistrationsListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                dt = _dbAccess.GetDataTable("PatrikaRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public DataTable FEGetPatrikaRegistrationsListByVariable(Int64 ChapterId, string Type, Int64 Year, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Year",Year),
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEPatrikaRegistrationsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetPatrikaRegistrationsById(Int64 PatrikaId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PatrikaId",PatrikaId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("PatrikaRegistrationsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 PatrikaRegistrationsDelete(Int64 PatrikaId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PatrikaId",PatrikaId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PatrikaRegistrationsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdatePatrikaRegistrationsOrderNo(int OrderNo, Int64 PatrikaId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@PatrikaId",PatrikaId),
                    new SqlParameter("@OrderNo",OrderNo),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("PatrikaRegistrationsUpdateOrderNo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

   

    }
}
