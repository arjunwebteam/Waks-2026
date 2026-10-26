using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class EventCategories
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 InsertEventCategory(AWS_AssociationToolCore.Entities.EventCategories objEventCategory)
        {

            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventCategoryId",objEventCategory.EventCategoryId),
                    new SqlParameter("@ChapterId",objEventCategory.ChapterId),
                    new SqlParameter("@EventCategory",objEventCategory.EventCategory),
                    new SqlParameter("@IsActive",objEventCategory.IsActive),
                    new SqlParameter("@UpdatedBy",objEventCategory.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCategoriesInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetEventCategoriesListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt1 = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total)
                };

                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt1 = _dbAccess.GetDataTable("EventCategoriesGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt1;
        }

        public DataTable GetEventCategoryById(Int64 EventCategoryId)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@EventCategoryId",EventCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCategoriesGetById", ref _sqlP);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteEventCategoryById(Int64 EventCategoryId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@EventCategoryId",EventCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCategoriesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetEventCategoriesList(ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {             
                    new SqlParameter("@Qstatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCategoriesGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventCategoriesListByChapterId(Int64 ChapterId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@Qstatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCategoriesGetListByChapterId", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
