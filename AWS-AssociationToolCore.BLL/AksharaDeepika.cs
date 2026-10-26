using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
  public  class PatrikaRegistrations
    {
        //Modified
        AWS_AssociationToolCore.DAL.PatrikaRegistrations _PatrikaRegistrations = new AWS_AssociationToolCore.DAL.PatrikaRegistrations();

        #region Methods

        public Int64 InsertPatrikaRegistrations(Entities.PatrikaRegistrations objPatrikaRegistrations, ref string BannerUrl)
        {
            Int64 _status = 0;
            if (objPatrikaRegistrations != null)
            {
                _status = _PatrikaRegistrations.InsertPatrikaRegistrations(objPatrikaRegistrations, ref BannerUrl);

            }
            return _status;
        }

        public Int64 PatrikaRegistrationsDelete(Int64 PatrikaId)
        {
            Int64 _status = 0;
            _status = _PatrikaRegistrations.PatrikaRegistrationsDelete(PatrikaId);
            return _status;
        }

        public Int64 UpdatePatrikaRegistrationsStatus(Int64 PatrikaId)
        {
            Int64 _status = 0;
            _status = _PatrikaRegistrations.UpdatePatrikaRegistrationsStatus(PatrikaId);
            return _status;
        }

        public Int64 UpdatePatrikaRegistrationsOrderNo(int OrderNo, Int64 PatrikaId)
        {
            Int64 _status = 0;
            _status = _PatrikaRegistrations.UpdatePatrikaRegistrationsOrderNo(OrderNo, PatrikaId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> GetPatrikaRegistrationsList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> lstPatrikaRegistrations = new List<Entities.PatrikaRegistrations>();
            DataTable dt = _PatrikaRegistrations.GetPatrikaRegistrationsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PatrikaRegistrations objlstPatrikaRegistrations = new AWS_AssociationToolCore.Entities.PatrikaRegistrations();

                    objlstPatrikaRegistrations.PatrikaId = Convert.ToInt64(dr["PatrikaId"].ToString());
                    objlstPatrikaRegistrations.Title = dr["Title"].ToString();
                    objlstPatrikaRegistrations.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : "");
                   
                    lstPatrikaRegistrations.Add(objlstPatrikaRegistrations);
                }

            }
            return lstPatrikaRegistrations;
        }

        public AWS_AssociationToolCore.Entities.PatrikaRegistrations GetPatrikaRegistrationsById(Int64 PatrikaId, ref int status)
        {
            AWS_AssociationToolCore.Entities.PatrikaRegistrations objPatrikaRegistrations = new AWS_AssociationToolCore.Entities.PatrikaRegistrations();
            DataTable dt = new DataTable();
            if (PatrikaId != 0)
            {
                dt = _PatrikaRegistrations.GetPatrikaRegistrationsById(PatrikaId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objPatrikaRegistrations.PatrikaId = Convert.ToInt64(dt.Rows[0]["PatrikaId"].ToString());
                    objPatrikaRegistrations.Title = dt.Rows[0]["Title"].ToString();
                    objPatrikaRegistrations.BannerUrl = (dt.Rows[0]["BannerUrl"] != DBNull.Value ? dt.Rows[0]["BannerUrl"].ToString() : "");
                    objPatrikaRegistrations.DocumentUrl = (dt.Rows[0]["DocumentUrl"] != DBNull.Value ? dt.Rows[0]["DocumentUrl"].ToString() : "");
                    objPatrikaRegistrations.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["OrderNo"].ToString()) : 0);
                    objPatrikaRegistrations.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                    objPatrikaRegistrations.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : "");
                    objPatrikaRegistrations.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : "");
                    objPatrikaRegistrations.Field3 = (dt.Rows[0]["Field3"] != DBNull.Value ? dt.Rows[0]["Field3"].ToString() : "");
                    objPatrikaRegistrations.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                    objPatrikaRegistrations.InsertedTime = Convert.ToDateTime(dt.Rows[0]["InsertedTime"].ToString());
                    objPatrikaRegistrations.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                    objPatrikaRegistrations.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"].ToString());
                    objPatrikaRegistrations.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);

                }
            }
            return objPatrikaRegistrations;
        }

        public List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> GetPatrikaRegistrationsListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> lstPatrikaRegistrations = new List<AWS_AssociationToolCore.Entities.PatrikaRegistrations>();

            DataTable dt = _PatrikaRegistrations.GetPatrikaRegistrationsListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _PatrikaRegistrations.GetPatrikaRegistrationsListByVariable(ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PatrikaRegistrations objPatrikaRegistrations = new AWS_AssociationToolCore.Entities.PatrikaRegistrations();

                    objPatrikaRegistrations.RId = Convert.ToInt32(dr["RId"].ToString());
                    objPatrikaRegistrations.PatrikaId = Convert.ToInt32(dr["PatrikaId"].ToString());
                    objPatrikaRegistrations.Title = dr["Title"].ToString();
                    objPatrikaRegistrations.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : "");
                    objPatrikaRegistrations.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : "");
                    objPatrikaRegistrations.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objPatrikaRegistrations.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objPatrikaRegistrations.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : "");
                    objPatrikaRegistrations.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : "");
                    objPatrikaRegistrations.Field3 = (dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : "");
                    objPatrikaRegistrations.InsertedBy = dr["InsertedBy"].ToString();
                    objPatrikaRegistrations.InsertedTime = Convert.ToDateTime(dr["InsertedTime"].ToString());
                    objPatrikaRegistrations.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPatrikaRegistrations.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objPatrikaRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objPatrikaRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : "");

                    lstPatrikaRegistrations.Add(objPatrikaRegistrations);
                }
            }
            return lstPatrikaRegistrations;
        }


        public List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> FEGetPatrikaRegistrationsListByVariable(Int64 ChapterId, string Type, Int64 Year, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.PatrikaRegistrations> lstPatrikaRegistrations = new List<AWS_AssociationToolCore.Entities.PatrikaRegistrations>();

            DataTable dt = _PatrikaRegistrations.FEGetPatrikaRegistrationsListByVariable(ChapterId, Type, Year, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _PatrikaRegistrations.FEGetPatrikaRegistrationsListByVariable(ChapterId, Type, Year, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.PatrikaRegistrations objPatrikaRegistrations = new AWS_AssociationToolCore.Entities.PatrikaRegistrations();

                    objPatrikaRegistrations.RId = Convert.ToInt32(dr["RId"].ToString());
                    objPatrikaRegistrations.PatrikaId = Convert.ToInt32(dr["PatrikaId"].ToString());
                    objPatrikaRegistrations.Title = dr["Title"].ToString();
                    objPatrikaRegistrations.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : "");
                    objPatrikaRegistrations.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : "");
                    objPatrikaRegistrations.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objPatrikaRegistrations.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objPatrikaRegistrations.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : "");
                    objPatrikaRegistrations.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : "");
                    objPatrikaRegistrations.Field3 = (dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : "");
                    objPatrikaRegistrations.InsertedBy = dr["InsertedBy"].ToString();
                    objPatrikaRegistrations.InsertedTime = Convert.ToDateTime(dr["InsertedTime"].ToString());
                    objPatrikaRegistrations.UpdatedBy = dr["UpdatedBy"].ToString();
                    objPatrikaRegistrations.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objPatrikaRegistrations.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objPatrikaRegistrations.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : "");

                    lstPatrikaRegistrations.Add(objPatrikaRegistrations);
                }
            }
            return lstPatrikaRegistrations;
        }

   

        #endregion
    }
}
