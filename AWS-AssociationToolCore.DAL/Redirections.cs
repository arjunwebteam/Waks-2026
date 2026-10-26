using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
  public  class Redirections
    {

        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Method

        public Int64 DeleteRedirections(Int64 RedirectionId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@RedirectionId",RedirectionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("RedirectionsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertRedirections(Entities.Redirections objRedirections)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@RedirectionId",objRedirections.RedirectionId),
                    new SqlParameter("@FromUrl",(objRedirections.FromUrl!= null?(object)objRedirections.FromUrl:DBNull.Value.ToString())),
                    new SqlParameter("@ToUrl",(objRedirections.ToUrl!= null?(object)objRedirections.ToUrl:DBNull.Value.ToString())),
                    new SqlParameter("@Target",(objRedirections.Target!= null?(object)objRedirections.Target:DBNull.Value.ToString())),
                    new SqlParameter("@IsActive",objRedirections.IsActive),
                    new SqlParameter("@Field1",(objRedirections.Field1!= null?(object)objRedirections.Field1:DBNull.Value.ToString())),
                    new SqlParameter("@Field2",(objRedirections.Field2!= null?(object)objRedirections.Field2:DBNull.Value.ToString())),
                    new SqlParameter("@InsertedBy",(objRedirections.InsertedBy!= null?(object)objRedirections.InsertedBy:DBNull.Value.ToString())),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",(objRedirections.UpdatedBy!= null?(object)objRedirections.UpdatedBy:DBNull.Value.ToString())),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };
                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("RedirectionsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        #region Entites Filling

        public DataTable GetRedirectionsById(Int64 RedirectionId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@RedirectionId",RedirectionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RedirectionsGetListById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetRedirectionsByurl(string Url, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Url",Url),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RedirectionsGetListByUrl", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetRedirectionsListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                  
                    new SqlParameter("@Total",0)
                };
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RedirectionsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable RedirectionsList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RedirectionsGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

     
        public Int64 UpdateRedirectionsStatus(Int64 RedirectionId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@RedirectionId",RedirectionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("RedirectionsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        //public DataTable ExportToRedirections(string Search, string Sort)
        //{
        //    DataTable dt = null;
        //    Int32 Total = 0;
        //    try
        //    {
        //        _sqlP = new[]
        //        {
        //            new SqlParameter("@Search",Search),
        //            new SqlParameter("@Sort",Sort),
        //            new SqlParameter("@Total",0)
        //        };
        //        _sqlP[2].Direction = System.Data.ParameterDirection.Output;
        //        dt = _dbAccess.GetDataTable("RedirectionsExporttoExcel", ref _sqlP);
        //        Total = Convert.ToInt32(_sqlP[2].Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return dt;
        //}

        #endregion  
   

    }
}
