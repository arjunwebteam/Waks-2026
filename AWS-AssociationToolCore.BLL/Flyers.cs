using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class Flyers
    {
        AWS_AssociationToolCore.DAL.Flyers _Flyers = new AWS_AssociationToolCore.DAL.Flyers();

        #region Methods

        public Int64 InsertFlyers(Entities.Flyers objFlyers, ref string FlyerUrl)
        {
            Int64 _status = 0;
            if (objFlyers != null)
            {
                _status = _Flyers.InsertFlyers(objFlyers, ref FlyerUrl);

            }
            return _status;
        }

        public Int64 DeleteFlyer(Int64 FlyerId)
        {
            Int64 _status = 0;
            _status = _Flyers.DeleteFlyer(FlyerId);
            return _status;
        }

        public Int64 UpdateFlyersStatus(Int64 FlyerId)
        {
            Int64 _status = 0;
            _status = _Flyers.UpdateFlyersStatus(FlyerId);
            return _status;
        }

        public Int64 UpdateFlyersIsPage(Int64 FlyerId)
        {
            Int64 _status = 0;
            _status = _Flyers.UpdateFlyersIsPage(FlyerId);
            return _status;
        }

        #endregion

        #region Entities filling

        public AWS_AssociationToolCore.Entities.Flyers GetFlyerById(Int64 PhotoId, ref int status)
        {
            AWS_AssociationToolCore.Entities.Flyers objFlyers = new AWS_AssociationToolCore.Entities.Flyers();
            DataTable dt = new DataTable();
            if (PhotoId != 0)
            {
                dt = _Flyers.GetFlyerById(PhotoId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objFlyers.FlyerId = Convert.ToInt64(dt.Rows[0]["FlyerId"].ToString());
                    objFlyers.FlyerUrl = (dt.Rows[0]["FlyerUrl"] != DBNull.Value ? dt.Rows[0]["FlyerUrl"].ToString() : "");
                    objFlyers.RedirectUrl = (dt.Rows[0]["RedirectUrl"] != DBNull.Value ? dt.Rows[0]["RedirectUrl"].ToString() : "");
                    objFlyers.PageType = (dt.Rows[0]["PageType"] != DBNull.Value ? dt.Rows[0]["PageType"].ToString() : "");
                    objFlyers.PageContent = (dt.Rows[0]["PageContent"] != DBNull.Value ? dt.Rows[0]["PageContent"].ToString() : "");
                    objFlyers.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                    objFlyers.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objFlyers.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());

                }
            }
            return objFlyers;
        }

        public List<AWS_AssociationToolCore.Entities.Flyers> GetFlyerListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Flyers> lstFlyers = new List<AWS_AssociationToolCore.Entities.Flyers>();

            DataTable dt = _Flyers.GetFlyerListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Flyers.GetFlyerListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Flyers objFlyers = new AWS_AssociationToolCore.Entities.Flyers();

                    objFlyers.RId = Convert.ToInt32(dr["RId"].ToString());
                    objFlyers.FlyerId = Convert.ToInt32(dr["FlyerId"].ToString());
                    objFlyers.FlyerUrl = (dr["FlyerUrl"] != DBNull.Value ? dr["FlyerUrl"].ToString() : "");
                    objFlyers.RedirectUrl = (dr["RedirectUrl"] != DBNull.Value ? dr["RedirectUrl"].ToString() : "");
                    objFlyers.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objFlyers.PageType = (dr["PageType"] != DBNull.Value ? dr["PageType"].ToString() : "");
                    objFlyers.UpdatedBy = dr["UpdatedBy"].ToString();
                    objFlyers.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstFlyers.Add(objFlyers);
                }
            }
            return lstFlyers;
        }

        #endregion
    }
}
