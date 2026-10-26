using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
  public  class CommitteeCategoryMaster
    {

        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 DeleteCommitteeCategoryMaster(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CommitteeCategoryMasterDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public DataTable GetCommitteeCategoryMaster(Int64 YearId, Int64 ChapterId,ref int Status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@YearId",YearId),
                      new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CommitteeCategoryMasterGetList", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
      
        //public DataTable GetCommitteeCategoryMasterById(Int64 CommitteeCategoryId, ref int Status)
        //{
        //    DataTable dt = null;
        //    try
        //    {
        //        _sqlP = new[]
        //        {
        //            new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
        //            new SqlParameter("@QStatus",0)
        //        };
        //        _sqlP[1].Direction = System.Data.ParameterDirection.Output;
        //        dt = _dbAccess.GetDataTable("CommitteeCategoryMasterGetById", ref _sqlP);
        //        Status = Convert.ToInt32(_sqlP[1].Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return dt;
        //}



        public DataSet GetCommitteeCategoryMasterById(Int64 CommitteeCategoryId, ref int status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                  new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("CommitteeCategoryMasterGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }


        public Int64 InsertCommitteeCategoryMaster(Entities.CommitteeCategoryMaster objCommitteeCategoryMaster)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                     new SqlParameter("@QStatus",0),
                    new SqlParameter("@CommitteeCategoryId",objCommitteeCategoryMaster.CommitteeCategoryId),
                    new SqlParameter("@CategoryName",objCommitteeCategoryMaster.CategoryName),
                    new SqlParameter("@YearId",objCommitteeCategoryMaster.YearId),
                    new SqlParameter("@PageParentId",(objCommitteeCategoryMaster.PageParentId == 0 ?DBNull.Value:(object)objCommitteeCategoryMaster.PageParentId)),
                    new SqlParameter("@Position",(objCommitteeCategoryMaster.Position == 0 ?DBNull.Value:(object)objCommitteeCategoryMaster.Position)),
                    new SqlParameter("@InsertedBy",objCommitteeCategoryMaster.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.Now),
                    new SqlParameter("@IsActive",objCommitteeCategoryMaster.IsActive),
                    new SqlParameter("@UpdatedBy",objCommitteeCategoryMaster.UpdatedBy),
                    new SqlParameter("@UpdatedDate",objCommitteeCategoryMaster.UpdatedDate),
                    new SqlParameter("@ChapterIds",(objCommitteeCategoryMaster.ChapterIds == null ?DBNull.Value:(object)objCommitteeCategoryMaster.ChapterIds)),

                    };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CommitteeCategoryMasterInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public Int64 DeleteCommitteeMember(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
            {
                    new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CommitteeCategoryMasterDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public Int64 CommitteeMembersUpdateStatus(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CommitteeCategoryMasterUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }





    }
}
