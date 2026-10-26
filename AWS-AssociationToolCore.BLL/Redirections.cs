using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
   public class Redirections
    {

        DAL.Redirections _Redirections = new DAL.Redirections();

        #region Methods

        public Int64 DeleteRedirections(Int64 RedirectionId)
        {
            Int64 _status = 0;
            if (RedirectionId != 0)
            {
                _status = _Redirections.DeleteRedirections(RedirectionId);
            }
            return _status;
        }

        public Int64 InsertRedirections(Entities.Redirections objRedirections)
        {
            Int64 _status = 0;
            if (objRedirections != null)
            {
                _status = _Redirections.InsertRedirections(objRedirections);
            }
            return _status;
        }

        //public Int64 UpdateRedirectionsDisplayOrder(int DisplayOrder, Int64 RedirectionId)
        //{
        //    Int64 _status = 0;
        //    _status = _Redirections.UpdateRedirectionsDisplayOrder(DisplayOrder, RedirectionId);
        //    return _status;
        //}

        public Int64 UpdateRedirectionsStatus(Int64 RedirectionId)
        {
            Int64 _status = 0;
            _status = _Redirections.UpdateRedirectionsStatus(RedirectionId);
            return _status;
        }

        #endregion

        #region Entity Loading

        public List<Entities.Redirections> GetRedirectionsListByVariable(string Search, string Sort, int PageNo, int Items,  ref int Total)
        {
            DataTable dt = _Redirections.GetRedirectionsListByVariable(Search, Sort, PageNo, Items,  ref Total);
            List<Entities.Redirections> lstRedirections = new List<Entities.Redirections>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _Redirections.GetRedirectionsListByVariable(Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Redirections objRedirections = new Entities.Redirections();

                    objRedirections.RId = Convert.ToInt64(dr["Rid"]);
                    objRedirections.RedirectionId = Convert.ToInt64(dr["RedirectionId"]);
                    objRedirections.FromUrl = (dr["FromUrl"] != null ? dr["FromUrl"].ToString() : "");
                    objRedirections.ToUrl = (dr["ToUrl"] != null ? dr["ToUrl"].ToString() : "");
                    objRedirections.Target = (dr["Target"] != null ? dr["Target"].ToString() : "");
                    objRedirections.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"].ToString()) : false);
                    objRedirections.Field1 = (dr["Field1"] != null ? dr["Field1"].ToString() : "");
                    objRedirections.Field2 = (dr["Field2"] != null ? dr["Field2"].ToString() : "");
                    objRedirections.InsertedBy = (dr["InsertedBy"] != null ? dr["InsertedBy"].ToString() : "");
                    objRedirections.InsertedDate = (dr["InsertedDate"] != null ? Convert.ToDateTime(dr["InsertedDate"]) : DateTime.MinValue);
                    objRedirections.UpdatedBy = (dr["UpdatedBy"] != null ? dr["UpdatedBy"].ToString() : "");
                    objRedirections.UpdatedDate = (dr["UpdatedDate"] != null ? Convert.ToDateTime(dr["UpdatedDate"]) : DateTime.UtcNow);

                     lstRedirections.Add(objRedirections);
                }
            }
            return lstRedirections;
        }

        public Entities.Redirections GetRedirectionsById(Int64 RedirectionId, ref int status)
        {
            DataTable dt = _Redirections.GetRedirectionsById(RedirectionId, ref status);
            Entities.Redirections objRedirections = new Entities.Redirections();

            if (dt.Rows.Count == 1)
            {

                objRedirections.RedirectionId = Convert.ToInt64(dt.Rows[0]["RedirectionId"]);
                objRedirections.FromUrl = (dt.Rows[0]["FromUrl"] != DBNull.Value ? dt.Rows[0]["FromUrl"].ToString() : "");
                objRedirections.ToUrl = (dt.Rows[0]["ToUrl"] != DBNull.Value ? dt.Rows[0]["ToUrl"].ToString() : "");
                objRedirections.Target = (dt.Rows[0]["Target"] != DBNull.Value ? dt.Rows[0]["Target"].ToString() : "");
                objRedirections.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                objRedirections.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : "");
                objRedirections.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : "");
                objRedirections.InsertedBy = (dt.Rows[0]["InsertedBy"] != DBNull.Value ? dt.Rows[0]["InsertedBy"].ToString() : "");
                objRedirections.InsertedDate = (dt.Rows[0]["InsertedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["InsertedDate"]) : DateTime.MinValue);
                objRedirections.UpdatedBy = (dt.Rows[0]["UpdatedBy"] != DBNull.Value ? dt.Rows[0]["UpdatedBy"].ToString() : "");
                objRedirections.UpdatedDate = (dt.Rows[0]["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]) : DateTime.UtcNow);
             
            }

            return objRedirections;
        }

        public Entities.Redirections GetRedirectionsByurl(string Url, ref int status)
        {
            DataTable dt = _Redirections.GetRedirectionsByurl(Url, ref status);
            Entities.Redirections objRedirections = new Entities.Redirections();

            if (dt.Rows.Count == 1)
            {

                objRedirections.RedirectionId = Convert.ToInt64(dt.Rows[0]["RedirectionId"]);
                objRedirections.FromUrl = (dt.Rows[0]["FromUrl"] != DBNull.Value ? dt.Rows[0]["FromUrl"].ToString() : "");
                objRedirections.ToUrl = (dt.Rows[0]["ToUrl"] != DBNull.Value ? dt.Rows[0]["ToUrl"].ToString() : "");
                objRedirections.Target = (dt.Rows[0]["Target"] != DBNull.Value ? dt.Rows[0]["Target"].ToString() : "");
                objRedirections.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                objRedirections.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : "");
                objRedirections.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : "");
                objRedirections.InsertedBy = (dt.Rows[0]["InsertedBy"] != DBNull.Value ? dt.Rows[0]["InsertedBy"].ToString() : "");
                objRedirections.InsertedDate = (dt.Rows[0]["InsertedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["InsertedDate"]) : DateTime.MinValue);
                objRedirections.UpdatedBy = (dt.Rows[0]["UpdatedBy"] != DBNull.Value ? dt.Rows[0]["UpdatedBy"].ToString() : "");
                objRedirections.UpdatedDate = (dt.Rows[0]["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]) : DateTime.UtcNow);

            }

            return objRedirections;
        }

        public List<Entities.Redirections> RedirectionsList(ref int status)
        {

            List<Entities.Redirections> lstRedirections = new List<Entities.Redirections>();
            DataTable dt = _Redirections.RedirectionsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Redirections objRedirections = new Entities.Redirections();


                    objRedirections.RedirectionId = Convert.ToInt64(dr["RedirectionId"]);
                    objRedirections.FromUrl = (dr["FromUrl"] != null ? dr["FromUrl"].ToString() : "");
                    objRedirections.ToUrl = (dr["ToUrl"] != null ? dr["ToUrl"].ToString() : "");
                    objRedirections.Target = (dr["Target"] != null ? dr["Target"].ToString() : "");
                    objRedirections.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"].ToString()) : false);
                    objRedirections.Field1 = (dr["Field1"] != null ? dr["Field1"].ToString() : "");
                    objRedirections.Field2 = (dr["Field2"] != null ? dr["Field2"].ToString() : "");
                    objRedirections.InsertedBy = (dr["InsertedBy"] != null ? dr["InsertedBy"].ToString() : "");
                    objRedirections.InsertedDate = (dr["InsertedDate"] != null ? Convert.ToDateTime(dr["InsertedDate"]) : DateTime.MinValue);
                    objRedirections.UpdatedBy = (dr["UpdatedBy"] != null ? dr["UpdatedBy"].ToString() : "");
                    objRedirections.UpdatedDate = (dr["UpdatedDate"] != null ? Convert.ToDateTime(dr["UpdatedDate"]) : DateTime.UtcNow);

                    lstRedirections.Add(objRedirections);
                }
            }
            return lstRedirections;
        }

        #endregion

    }
}
