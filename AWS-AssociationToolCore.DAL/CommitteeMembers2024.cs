using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
   public class CommitteeMembers2024
    {
        
            DBAccess _dbAccess = new DBAccess();
            SqlParameter[] _sqlP;

            public DataTable GetCommitteeMembers2024List(ref int status)
            {
                DataTable dt = null;
                try
                {
                    _sqlP = new[] 
                {
                    new SqlParameter("@QStatus",0)
                };
                    _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                    dt = _dbAccess.GetDataTable("CommitteeMembers2024GetList", ref _sqlP);
                    status = Convert.ToInt32(_sqlP[0].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public Int64 InsertCommitteeMembers2024(Entities.CommitteeMembers2024 objCommitteeMembers2024)
            {
                Int64 _status = 0;
                try
                {
                    _sqlP = new[]
                    {
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@CommitteeMemberId",objCommitteeMembers2024.CommitteeMemberId),
                    new SqlParameter("@MemberId",objCommitteeMembers2024.MemberId),
                    new SqlParameter("@CommitteeCategoryId",objCommitteeMembers2024.CommitteeCategoryId),
                    new SqlParameter("@DisplayOrder",objCommitteeMembers2024.DisplayOrder),
                    new SqlParameter("@Designation",(objCommitteeMembers2024.Designation == null ?DBNull.Value:(object)objCommitteeMembers2024.Designation)),
                   
                    };
                    _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                    _dbAccess.SP_ExecuteScalar("CommitteeMembers2024Insert", ref _sqlP);
                    _status = Convert.ToInt64(_sqlP[0].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _status;
            }

            public DataTable GetCommitteeMembers2024ListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    dt = _dbAccess.GetDataTable("CommitteeMembers2024GetById", ref _sqlP);
                    status = Convert.ToInt32(_sqlP[1].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return dt;
            }

            public DataTable CommitteeMembers2024List(Int64 CommitteeCategoryId, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    dt = _dbAccess.GetDataTable("CommitteeMembers2024List", ref _sqlP);
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
                    _dbAccess.SP_ExecuteScalar("CommitteeMembers2024Delete", ref _sqlP);
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
                    _dbAccess.SP_ExecuteScalar("CommitteeMembers2024UpdateDisplayOrder", ref _sqlP);
                    _status = Convert.ToInt64(_sqlP[2].Value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _status;
            }

        public Int64 CommitteeMembers2024UpdateStatus(Int64 CommitteeMemberId)
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
                _dbAccess.SP_ExecuteScalar("CommitteeMembers2024UpdateStatus", ref _sqlP);
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

