using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
  public  class EventCompetitions
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Method

        public Int64 DeleteEventCompetitions(Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventCompetitionId",EventCompetitionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCompetitionsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 EventCompetitionsInsert(Entities.EventCompetitions objMembershipType)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventCompetitionId",objMembershipType.EventCompetitionId),
                    new SqlParameter("@CompetitionName",objMembershipType.CompetitionName),
                    new SqlParameter("@Price",objMembershipType.Price),
                    new SqlParameter("@DisplayOrder",(objMembershipType.DisplayOrder==0?(object)DBNull.Value:objMembershipType.DisplayOrder)),
                    new SqlParameter("@IsActive",objMembershipType.IsActive),
                    new SqlParameter("@Field1",(objMembershipType.Field1 !=null ? (object)objMembershipType.Field1:DBNull.Value)),
                    new SqlParameter("@Field2",(objMembershipType.Field2 !=null ? (object)objMembershipType.Field2:DBNull.Value)),
                    new SqlParameter("@Field3",(objMembershipType.Field3 !=null ? (object)objMembershipType.Field3:DBNull.Value)),
                    new SqlParameter("@InsertedBy",objMembershipType.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objMembershipType.UpdatedBy),
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@NonMemberAmount",(objMembershipType.NonMemberAmount==0?(object)DBNull.Value:objMembershipType.NonMemberAmount)),
                    new SqlParameter("@EventId",(objMembershipType.EventId==0?(object)DBNull.Value:objMembershipType.EventId)),
                    };
                _sqlP[12].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCompetitionsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[12].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public Int64 UpdateEventCompetitionDisplayOrder(int DisplayOrder, Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventCompetitionId",EventCompetitionId),
                    new SqlParameter("@DisplayOrder",DisplayOrder),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCompetitionsUpdateDisplayOrder", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventCompetitionstatus(Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventCompetitionId",EventCompetitionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCompetitionsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        #region Admin Section

        public DataTable GetEventCompetitionById(Int64 EventCompetitionId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventCompetitionId",EventCompetitionId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCompetitionsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventCompetitionsListByVariable(Int64 EventId, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    new SqlParameter("@Total",0),
                    new SqlParameter("@EventId",EventId)
                };
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCompetitionsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventCompetitionsList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventCompetitionsGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FEGetEventCompetitionsList(Int64 EventId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventId",EventId)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEEventCompetitionsGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion 

        #region FE

        public DataTable FEGetEventCompetitionById(Int64 EventCompetitionId, Int64 MemberId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventCompetitionId",EventCompetitionId),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@MemberId",MemberId)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEEventCompetitionsGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
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
