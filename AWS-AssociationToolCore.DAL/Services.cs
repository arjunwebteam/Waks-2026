using AWS_AssociationToolCore.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
   public class Services
    {

        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Method

        public Int64 DeleteServices(Int64 ServiceId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServicesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 ServicesUpdateStatus(Int64 ServiceId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServicesUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateServicesOrderNo(int DisplayOrder, Int64 ServiceId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@DisplayOrder",DisplayOrder),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServiceUpdateOrderNo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }




        public Int64 InsertServices(Entities.Services objServices, ref string DocumentUrl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@ServiceId",objServices.ServiceId),
                    new SqlParameter("@ChapterId",(objServices.ChapterId!= 0?(object)objServices.ChapterId:DBNull.Value)),
                    new SqlParameter("@CategoryId",(objServices.CategoryId!= 0?(object)objServices.CategoryId:DBNull.Value)),
                    new SqlParameter("@ServiceTitle",(objServices.ServiceTitle!= null?(object)objServices.ServiceTitle:DBNull.Value)),
                    new SqlParameter("@ServiceType",(objServices.ServiceType!= null?(object)objServices.ServiceType:DBNull.Value)),
                    new SqlParameter("@OrganisedBy",(objServices.OrganisedBy!= null?(object)objServices.OrganisedBy:DBNull.Value)),
                    new SqlParameter("@Purpose",(objServices.Purpose!= null?(object)objServices.Purpose:DBNull.Value)),
                    new SqlParameter("@Description",(objServices.Description!= null?(object)objServices.Description:DBNull.Value)),
                    new SqlParameter("@ShortDescription",(objServices.ShortDescription!= null?(object)objServices.ShortDescription:DBNull.Value)),
                    new SqlParameter("@DocumentUrl",DocumentUrl),
                    new SqlParameter("@ExpiryDate",(objServices.ExpiryDate==DateTime.MinValue?(object)DBNull.Value:objServices.ExpiryDate)),
                    new SqlParameter("@EstimationAmount",(objServices.EstimationAmount!= 0?(object)objServices.EstimationAmount:DBNull.Value)),
                    new SqlParameter("@ServiceStatus",(objServices.ServiceStatus!= null?(object)objServices.ServiceStatus:DBNull.Value)),
                    new SqlParameter("@IsDisplay",objServices.IsDisplay),
                    new SqlParameter("@Field1",(objServices.Field1!= null?(object)objServices.Field1:DBNull.Value)),
                    new SqlParameter("@Field2",(objServices.Field2!= null?(object)objServices.Field2:DBNull.Value)),
                    new SqlParameter("@UpdatedDate",objServices.UpdatedDate),
                    new SqlParameter("@UpdatedBy",objServices.UpdatedBy),
                     new SqlParameter("@DisplayOrder",(objServices.DisplayOrder!= 0?(object)objServices.DisplayOrder:DBNull.Value)),
                    new SqlParameter("@QStatus",0),
                   };

                _sqlP[9].SqlDbType = SqlDbType.NVarChar;
                _sqlP[9].Size = 512;
                _sqlP[9].Direction = System.Data.ParameterDirection.InputOutput;

              
                _sqlP[19].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("ServicesInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[19].Value);

                DocumentUrl = _sqlP[9].Value.ToString();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }



      

        #endregion

        #region Admin Section

        public DataTable GetServicesById(Int64 ServiceId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServicesGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetServicesList(ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServicesGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet FEGetServicesById(Int64 ServiceId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("FEServicesGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataSet FEGetServicesDataById(Int64 ServiceId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ServiceId",ServiceId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("FEGetServicesDataById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataTable GetServicesListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    //new SqlParameter("@ChapterId",ChapterId),
                    //new SqlParameter("@CategoryId",CategoryId),
                    //new SqlParameter("@ServiceType",ServiceType),
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    //new SqlParameter("@MemberId",MemberId),
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ServicesGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public DataTable FEServicesGetListByChapterId(Int64 ChapterId, ref int status)
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
                dt = _dbAccess.GetDataTable("FEServicesGetListByChapterId", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FEGetServicesListByVariable(Int64 MemberId, Int64 ChapterId, Int64 CategoryId, string ServiceType, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@CategoryId",CategoryId),
                    new SqlParameter("@ServiceType",ServiceType),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@MemberId",MemberId),



                };
                _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEServicesGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[7].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FEGetServicesListByVariableByYear(string Year, Int64 MemberId, Int64 ChapterId, Int64 CategoryId, string ServiceType, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@CategoryId",CategoryId),
                    new SqlParameter("@ServiceType",ServiceType),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@MemberId",MemberId),
                    new SqlParameter("@Year",Year),
                     
                };
                _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEServicesGetListByVariableByYear", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[7].Value);
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
