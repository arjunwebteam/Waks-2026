using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
   public class CommitteeMembers
    {
        
            DBAccess _dbAccess = new DBAccess();
            SqlParameter[] _sqlP;

            public DataTable GetCommitteeMembersList(ref int status)
            {
                DataTable dt = null;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@QStatus",0)
                };
                    _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                    dt = _dbAccess.GetDataTable("CommitteeMembersGetList", ref _sqlP);
                    status = Convert.ToInt32(_sqlP[0].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public Int64 InsertCommitteeMembers(Entities.CommitteeMembers objCommitteeMembers)
            {
                Int64 _status = 0;
                try
                {
                    _sqlP = new[]
                    {
                    new SqlParameter("@CommitteeMemberId",objCommitteeMembers.CommitteeMemberId),
                    new SqlParameter("@ChapterId",objCommitteeMembers.ChapterId),
                    new SqlParameter("@MemberId",objCommitteeMembers.MemberId),
                    new SqlParameter("@CommitteeCategoryId",objCommitteeMembers.CommitteeCategoryId),
                    new SqlParameter("@DisplayOrder",objCommitteeMembers.DisplayOrder),
                    new SqlParameter("@Designation",(objCommitteeMembers.Designation == null ?DBNull.Value:(object)objCommitteeMembers.Designation)),
                    new SqlParameter("@Description",(objCommitteeMembers.Description == null ?DBNull.Value:(object)objCommitteeMembers.Description)),
                    new SqlParameter("@QStatus",0)
                    };
                    _sqlP[7].Direction = System.Data.ParameterDirection.Output;
                    _dbAccess.SP_ExecuteScalar("CommitteeMembersInsert", ref _sqlP);
                    _status = Convert.ToInt64(_sqlP[7].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _status;
            }

            public DataTable GetCommitteeMembersListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    new SqlParameter("@Total",Total)
                };

                    _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                    dt = _dbAccess.GetDataTable("CommitteeCategoriesGetListByVariable", ref _sqlP);
                    Total = Convert.ToInt32(_sqlP[4].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public DataTable GetCommitteeMemberById(Int64 CommitteeMemberId, ref int status)
            {
                DataTable dt = null;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@CommitteeMemberId",CommitteeMemberId),
                    new SqlParameter("@QStatus",0)
                };
                    _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                    dt = _dbAccess.GetDataTable("CommitteeMembersGetById", ref _sqlP);
                    status = Convert.ToInt32(_sqlP[1].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public DataTable CommitteeMembersList(Int64 CommitteeCategoryId, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
            {
                DataTable dt = null;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@CommitteeCategoryId",CommitteeCategoryId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),                    
                    new SqlParameter("@Total",Total),
                    new SqlParameter("@ChapterId",ChapterId),
                };

                    _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                    dt = _dbAccess.GetDataTable("CommitteeMembersList", ref _sqlP);
                    Total = Convert.ToInt32(_sqlP[5].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public Int64 DeleteCommitteeMember(Int64 CommitteeMemberId)
            {
                Int64 _status = 0;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@CommitteeMemberId",CommitteeMemberId),
                    new SqlParameter("@QStatus",0)
                };
                    _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                    _dbAccess.SP_ExecuteScalar("CommitteeMembersDelete", ref _sqlP);
                    _status = Convert.ToInt64(_sqlP[1].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _status;
            }

            public Int64 UpdateCommitteeMemberDisplayOrder(int DisplayOrder, Int64 CommitteeMemberId)
            {
                Int64 _status = 0;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@CommitteeMemberId",CommitteeMemberId),
                    new SqlParameter("@DisplayOrder",DisplayOrder),
                    new SqlParameter("@QStatus",0)
                };
                    _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                    _dbAccess.SP_ExecuteScalar("CommitteeMembersUpdateDisplayOrder", ref _sqlP);
                    _status = Convert.ToInt64(_sqlP[2].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _status;
            }

        public Int64 CommitteeMembersUpdateStatus(Int64 CommitteeMemberId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CommitteeMemberId",CommitteeMemberId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CommitteeMembersUpdateStatus", ref _sqlP);
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

