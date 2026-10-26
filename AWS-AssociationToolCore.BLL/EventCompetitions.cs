using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class EventCompetitions
    {
        DAL.EventCompetitions _EventCompetitions = new DAL.EventCompetitions();

        #region Methods

        public Int64 DeleteEventCompetitions(Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            if (EventCompetitionId != 0)
            {
                _status = _EventCompetitions.DeleteEventCompetitions(EventCompetitionId);
            }
            return _status;
        }

        public Int64 EventCompetitionsInsert(Entities.EventCompetitions objMembershipType)
        {
            Int64 _status = 0;
            if (objMembershipType != null)
            {
                _status = _EventCompetitions.EventCompetitionsInsert(objMembershipType);
            }
            return _status;
        }

        public Int64 UpdateEventCompetitionDisplayOrder(int DisplayOrder, Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            _status = _EventCompetitions.UpdateEventCompetitionDisplayOrder(DisplayOrder, EventCompetitionId);
            return _status;
        }
        public Int64 UpdateEventCompetitionstatus(Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            _status = _EventCompetitions.UpdateEventCompetitionstatus(EventCompetitionId);
            return _status;
        }

        #endregion

        #region Entity Loading

        public List<Entities.EventCompetitions> GetEventCompetitionsListByVariable(Int64 EventId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _EventCompetitions.GetEventCompetitionsListByVariable(EventId, Search, Sort, PageNo, Items, ref Total);
            List<Entities.EventCompetitions> lstMembershipType = new List<Entities.EventCompetitions>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _EventCompetitions.GetEventCompetitionsListByVariable(EventId, Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.EventCompetitions objMembershipType = new Entities.EventCompetitions();

                    objMembershipType.RId = Convert.ToInt64(dr["Rid"]);
                    objMembershipType.CompetitionName = dr["CompetitionName"].ToString();
                    objMembershipType.EventCompetitionId = Convert.ToInt64(dr["EventCompetitionId"]);
                    objMembershipType.Price = Convert.ToDecimal(dr["Price"]);
                    objMembershipType.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt64(dr["DisplayOrder"]) : 0);
                    objMembershipType.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objMembershipType.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objMembershipType.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    objMembershipType.Field3 = (dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null);
                    objMembershipType.InsertedBy = dr["InsertedBy"].ToString();
                    objMembershipType.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objMembershipType.UpdatedBy = dr["UpdatedBy"].ToString();
                    objMembershipType.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    objMembershipType.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"]) : 0);
                    objMembershipType.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objMembershipType.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);


                    lstMembershipType.Add(objMembershipType);
                }
            }
            return lstMembershipType;
        }

        public Entities.EventCompetitions GetEventCompetitionById(Int64 EventCompetitionId, ref int status)
        {
            DataTable dt = _EventCompetitions.GetEventCompetitionById(EventCompetitionId, ref status);
            Entities.EventCompetitions objMembershipType = new Entities.EventCompetitions();

            if (dt.Rows.Count == 1)
            {
                objMembershipType.EventCompetitionId = Convert.ToInt64(dt.Rows[0]["EventCompetitionId"]);
                objMembershipType.CompetitionName = dt.Rows[0]["CompetitionName"].ToString();
                objMembershipType.Price = Convert.ToDecimal(dt.Rows[0]["Price"]);
                objMembershipType.DisplayOrder = (dt.Rows[0]["DisplayOrder"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["DisplayOrder"]) : 0);
                objMembershipType.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                objMembershipType.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                objMembershipType.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
                objMembershipType.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objMembershipType.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());
                objMembershipType.NonMemberAmount = (dt.Rows[0]["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["NonMemberAmount"]) : 0);
                objMembershipType.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);
               
            }

            return objMembershipType;
        }

        public List<Entities.EventCompetitions> GetEventCompetitionsList(ref int status)
        {
            DataTable dt = _EventCompetitions.GetEventCompetitionsList(ref status);
            List<Entities.EventCompetitions> lstMembershipType = new List<Entities.EventCompetitions>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.EventCompetitions objMembershipType = new Entities.EventCompetitions();

                    objMembershipType.EventCompetitionId = Convert.ToInt64(dr["EventCompetitionId"]);
                    objMembershipType.CompetitionName = dr["CompetitionName"].ToString();
                    objMembershipType.EventCompetitionId = Convert.ToInt64(dr["EventCompetitionId"]);
                    objMembershipType.Price = Convert.ToDecimal(dr["Price"]);
                    objMembershipType.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt64(dr["DisplayOrder"]) : 0);
                    objMembershipType.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objMembershipType.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objMembershipType.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    objMembershipType.Field3 = (dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null);
                    objMembershipType.InsertedBy = dr["InsertedBy"].ToString();
                    objMembershipType.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objMembershipType.UpdatedBy = dr["UpdatedBy"].ToString();
                    objMembershipType.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    objMembershipType.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"]) : 0);
                    objMembershipType.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);

                    lstMembershipType.Add(objMembershipType);
                }
            }

            return lstMembershipType;
        }


        public List<Entities.EventCompetitions> FEGetEventCompetitionsList(Int64 EventId, ref int status)
        {
            DataTable dt = _EventCompetitions.FEGetEventCompetitionsList(EventId, ref status);
            List<Entities.EventCompetitions> lstMembershipType = new List<Entities.EventCompetitions>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.EventCompetitions objMembershipType = new Entities.EventCompetitions();

                    objMembershipType.EventCompetitionId = Convert.ToInt64(dr["EventCompetitionId"]);
                    objMembershipType.CompetitionName = dr["CompetitionName"].ToString();
                    objMembershipType.EventCompetitionId = Convert.ToInt64(dr["EventCompetitionId"]);
                    objMembershipType.Price = Convert.ToDecimal(dr["Price"]);
                    objMembershipType.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt64(dr["DisplayOrder"]) : 0);
                    objMembershipType.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objMembershipType.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objMembershipType.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    objMembershipType.Field3 = (dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null);
                    objMembershipType.InsertedBy = dr["InsertedBy"].ToString();
                    objMembershipType.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
                    objMembershipType.UpdatedBy = dr["UpdatedBy"].ToString();
                    objMembershipType.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
                    objMembershipType.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"]) : 0);
                    objMembershipType.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);

                    lstMembershipType.Add(objMembershipType);
                }
            }

            return lstMembershipType;
        }

        public Entities.EventCompetitions FEEventCompetitionsGetById(Int64 EventCompetitionId, Int64 MemberId, ref int status)
        {
            DataTable dt = _EventCompetitions.FEGetEventCompetitionById(EventCompetitionId, MemberId, ref status);
            Entities.EventCompetitions objMembershipType = new Entities.EventCompetitions();

            if (dt.Rows.Count == 1)
            {
                objMembershipType.EventCompetitionId = Convert.ToInt64(dt.Rows[0]["EventCompetitionId"]);
                objMembershipType.CompetitionName = dt.Rows[0]["CompetitionName"].ToString();                
                objMembershipType.Price = (dt.Rows[0]["Price"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Price"]) : 0);
                objMembershipType.EventId = (dt.Rows[0]["EventId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["EventId"]) : 0);

            }

            return objMembershipType;
        }


        #endregion
    }
}
