using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class NewsLetter
    {
        AWS_AssociationToolCore.DAL.NewsLetter _NewsLetter = new AWS_AssociationToolCore.DAL.NewsLetter();

        #region Methods

        public Int64 InsertNewsLetter(AWS_AssociationToolCore.Entities.NewsLetter objNewsLetter)
        {

            Int64 _status = 0;
            if (objNewsLetter != null)
            {
                _status = _NewsLetter.InsertNewsLetter(objNewsLetter);
            }
            return _status;
        }

        public Int64 DeleteNewsLetter(Int64 LetterId)
        {
            Int64 _status = 0;

            _status = _NewsLetter.DeleteNewsLetterById(LetterId);
            
            return _status;
        }

        public Int64 UpdateNewsLettertatusById(Int64 LetterId)
        {
            Int64 _status = 0;

            _status = _NewsLetter.UpdateNewsLetterById(LetterId);

            return _status;
        }

        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.NewsLetter GetNewsLetterById(Int64 Id)
        {
            AWS_AssociationToolCore.Entities.NewsLetter objNewsLetter = new AWS_AssociationToolCore.Entities.NewsLetter();

            DataTable dt = new DataTable();
            if (Id != 0)
            {
                dt = _NewsLetter.GetNewsLetterById(Id);
                if (dt.Rows.Count == 1)
                {
                    objNewsLetter.LetterId = Convert.ToInt64(dt.Rows[0]["LetterId"].ToString());
                    objNewsLetter.EmailId = dt.Rows[0]["EmailId"].ToString();
                    objNewsLetter.RegisteredDate = Convert.ToDateTime(dt.Rows[0]["RegisteredDate"]);
                    objNewsLetter.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                }
            }
            return objNewsLetter;
        }

        public AWS_AssociationToolCore.Entities.NewsLetter GetNewsLetterByEmail(string Email,ref int status)
        {
            AWS_AssociationToolCore.Entities.NewsLetter objNewsLetter = new AWS_AssociationToolCore.Entities.NewsLetter();

            DataTable dt = new DataTable();
            if (Email != "")
            {
                dt = _NewsLetter.GetNewsLetterByEmail(Email,ref status);
                if (dt.Rows.Count == 1)
                {
                    objNewsLetter.LetterId = Convert.ToInt64(dt.Rows[0]["LetterId"].ToString());
                    objNewsLetter.EmailId = dt.Rows[0]["EmailId"].ToString();
                    objNewsLetter.RegisteredDate = Convert.ToDateTime(dt.Rows[0]["RegisteredDate"]);
                    objNewsLetter.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                }
            }
            return objNewsLetter;
        }

        public List<AWS_AssociationToolCore.Entities.NewsLetter> GetNewsLetterListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.NewsLetter> lstNewsLetter = new List<AWS_AssociationToolCore.Entities.NewsLetter>();

            DataTable dt = _NewsLetter.GetNewsLetterListByVariable(Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _NewsLetter.GetNewsLetterListByVariable( Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.NewsLetter objNewsLetter = new AWS_AssociationToolCore.Entities.NewsLetter();

                    objNewsLetter.LetterId = Convert.ToInt64(dr["LetterId"]);
                    objNewsLetter.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objNewsLetter.RegisteredDate = Convert.ToDateTime(dr["RegisteredDate"]);
                    objNewsLetter.RId = Convert.ToInt64(dr["RId"]);
                    objNewsLetter.EmailId = dr["EmailId"].ToString();

                    lstNewsLetter.Add(objNewsLetter);
                }
            }
            return lstNewsLetter;
        }

        #endregion
    }
}
