using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
 public   class CommitteeCategoryMaster
    {

        DAL.CommitteeCategoryMaster _CommitteeCategoryMaster = new DAL.CommitteeCategoryMaster();

        public Int64 DeleteCommitteeCategoryMaster(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            if (CommitteeCategoryId != 0)
            {
                _status = _CommitteeCategoryMaster.DeleteCommitteeCategoryMaster(CommitteeCategoryId);
            }
            return _status;
        }

        public Int64 InsertCommitteeCategoryMaster(Entities.CommitteeCategoryMaster objCommitteeCategoryMaster)
        {
            Int64 _status = 0;
            if (objCommitteeCategoryMaster != null)
            {
                _status = _CommitteeCategoryMaster.InsertCommitteeCategoryMaster(objCommitteeCategoryMaster);
            }
            return _status;
        }


        public Int64 DeleteCommitteeMember(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            _status = _CommitteeCategoryMaster.DeleteCommitteeMember(CommitteeCategoryId);
            return _status;
        }


        public Int64 CommitteeMembersUpdateStatus(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            _status = _CommitteeCategoryMaster.CommitteeMembersUpdateStatus(CommitteeCategoryId);
            return _status;
        }

        public List<Entities.CommitteeCategoryMaster> GetCommitteeCategoryMasterAll(Int64 YearId, Int64 ChapterId,ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2, ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3, ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4,  ref int Status)
        {
            DataTable dt = _CommitteeCategoryMaster.GetCommitteeCategoryMaster(YearId, ChapterId,ref Status);
            List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = new List<Entities.CommitteeCategoryMaster>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt32(dr["PageLevel"]) == 1)
                    {
                        Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new Entities.CommitteeCategoryMaster();
                        objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);
                        objCommitteeCategoryMaster.CategoryName = dr["CategoryName"].ToString();
                        objCommitteeCategoryMaster.PageLevel = (dr["PageLevel"] != DBNull.Value ? Convert.ToInt32(dr["PageLevel"]) : 1);
                        objCommitteeCategoryMaster.PageParentId = (dr["PageParentId"] != DBNull.Value ? Convert.ToInt64(dr["PageParentId"]) : 0);
                        objCommitteeCategoryMaster.IdPath = (dr["IdPath"] != DBNull.Value ? dr["IdPath"].ToString() : "0");
                        objCommitteeCategoryMaster.Position = (dr["Position"] != DBNull.Value ? Convert.ToInt32(dr["Position"]) : 0);
                        objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objCommitteeCategoryMaster.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);
                        objCommitteeCategoryMaster.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? dr["InsertedBy"].ToString() : null);
                        objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                        objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                        objCommitteeCategoryMaster.ParentActive = (dr["ParentActive"] != DBNull.Value ? Convert.ToBoolean(dr["ParentActive"]) : false);
                        objCommitteeCategoryMaster.SubMenuItemCount = (dr["SubMenuItemCount"] != DBNull.Value ? Convert.ToInt32(dr["SubMenuItemCount"]) : 0);
                        objCommitteeCategoryMaster.SubCategoryCount = (dr["SubCategoryCount"] != DBNull.Value ? Convert.ToInt32(dr["SubCategoryCount"]) : 0);
                        objCommitteeCategoryMaster.YearId = (dr["YearId"] != DBNull.Value ? Convert.ToInt64(dr["YearId"]) : 0);

                        lstCommitteeCategoryMaster.Add(objCommitteeCategoryMaster);
                    }
                    if (Convert.ToInt32(dr["PageLevel"]) == 2)
                    {
                        Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new Entities.CommitteeCategoryMaster();
                        objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);
                        objCommitteeCategoryMaster.CategoryName = dr["CategoryName"].ToString();
                        objCommitteeCategoryMaster.PageLevel = (dr["PageLevel"] != DBNull.Value ? Convert.ToInt32(dr["PageLevel"]) : 1);
                        objCommitteeCategoryMaster.PageParentId = (dr["PageParentId"] != DBNull.Value ? Convert.ToInt64(dr["PageParentId"]) : 0);
                        objCommitteeCategoryMaster.IdPath = (dr["IdPath"] != DBNull.Value ? dr["IdPath"].ToString() : "0");
                        objCommitteeCategoryMaster.Position = (dr["Position"] != DBNull.Value ? Convert.ToInt32(dr["Position"]) : 0);
                        objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objCommitteeCategoryMaster.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);
                        objCommitteeCategoryMaster.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? dr["InsertedBy"].ToString() : null);
                        objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                        objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                        objCommitteeCategoryMaster.ParentActive = (dr["ParentActive"] != DBNull.Value ? Convert.ToBoolean(dr["ParentActive"]) : false);
                        objCommitteeCategoryMaster.SubMenuItemCount = (dr["SubMenuItemCount"] != DBNull.Value ? Convert.ToInt32(dr["SubMenuItemCount"]) : 0);
                        objCommitteeCategoryMaster.YearId = (dr["YearId"] != DBNull.Value ? Convert.ToInt64(dr["YearId"]) : 0);



                        lstCommitteeCategoryMaster2.Add(objCommitteeCategoryMaster);
                    }
                    if (Convert.ToInt32(dr["PageLevel"]) == 3)
                    {
                        Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new Entities.CommitteeCategoryMaster();
                        objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);
                        objCommitteeCategoryMaster.CategoryName = dr["CategoryName"].ToString();
                        objCommitteeCategoryMaster.PageLevel = (dr["PageLevel"] != DBNull.Value ? Convert.ToInt32(dr["PageLevel"]) : 1);
                        objCommitteeCategoryMaster.PageParentId = (dr["PageParentId"] != DBNull.Value ? Convert.ToInt64(dr["PageParentId"]) : 0);
                        objCommitteeCategoryMaster.IdPath = (dr["IdPath"] != DBNull.Value ? dr["IdPath"].ToString() : "0");
                        objCommitteeCategoryMaster.Position = (dr["Position"] != DBNull.Value ? Convert.ToInt32(dr["Position"]) : 0);
                        objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objCommitteeCategoryMaster.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);
                        objCommitteeCategoryMaster.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? dr["InsertedBy"].ToString() : null);
                        objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                        objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                        objCommitteeCategoryMaster.ParentActive = (dr["ParentActive"] != DBNull.Value ? Convert.ToBoolean(dr["ParentActive"]) : false);
                        objCommitteeCategoryMaster.SubMenuItemCount = (dr["SubMenuItemCount"] != DBNull.Value ? Convert.ToInt32(dr["SubMenuItemCount"]) : 0);
                        objCommitteeCategoryMaster.YearId = (dr["YearId"] != DBNull.Value ? Convert.ToInt64(dr["YearId"]) : 0);

                        lstCommitteeCategoryMaster3.Add(objCommitteeCategoryMaster);
                    }
                    if (Convert.ToInt32(dr["PageLevel"]) == 4)
                    {
                        Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new Entities.CommitteeCategoryMaster();
                        objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);
                        objCommitteeCategoryMaster.CategoryName = dr["CategoryName"].ToString();
                        objCommitteeCategoryMaster.PageLevel = (dr["PageLevel"] != DBNull.Value ? Convert.ToInt32(dr["PageLevel"]) : 1);
                        objCommitteeCategoryMaster.PageParentId = (dr["PageParentId"] != DBNull.Value ? Convert.ToInt64(dr["PageParentId"]) : 0);
                        objCommitteeCategoryMaster.IdPath = (dr["IdPath"] != DBNull.Value ? dr["IdPath"].ToString() : "0");
                        objCommitteeCategoryMaster.Position = (dr["Position"] != DBNull.Value ? Convert.ToInt32(dr["Position"]) : 0);
                        objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objCommitteeCategoryMaster.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);
                        objCommitteeCategoryMaster.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? dr["InsertedBy"].ToString() : null);
                        objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                        objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);
                        objCommitteeCategoryMaster.ParentActive = (dr["ParentActive"] != DBNull.Value ? Convert.ToBoolean(dr["ParentActive"]) : false);
                        objCommitteeCategoryMaster.SubMenuItemCount = (dr["SubMenuItemCount"] != DBNull.Value ? Convert.ToInt32(dr["SubMenuItemCount"]) : 0);
                        objCommitteeCategoryMaster.YearId = (dr["YearId"] != DBNull.Value ? Convert.ToInt64(dr["YearId"]) : 0);

                        lstCommitteeCategoryMaster4.Add(objCommitteeCategoryMaster);
                    }
                }
            }
            return lstCommitteeCategoryMaster;
        }



        public List<Entities.CommitteeCategoryMaster> GetCommitteeCategoryMasterDD(Int64 YearId, Int64 ChapterId, ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2, ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3, ref List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4, ref int Status)
        {
            List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = GetCommitteeCategoryMasterAll(YearId, ChapterId,ref lstCommitteeCategoryMaster2, ref lstCommitteeCategoryMaster3, ref lstCommitteeCategoryMaster4, ref Status);
            return lstCommitteeCategoryMaster;
        }


        //public Entities.CommitteeCategoryMaster GetCommitteeCategoryMasterById(Int64 MenuItemId, ref int Status)
        //{
        //    DataTable dt = null;
        //    Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new Entities.CommitteeCategoryMaster();
         



        //    if (MenuItemId != 0)
        //    {
        //        dt = _CommitteeCategoryMaster.GetCommitteeCategoryMasterById(MenuItemId, ref Status);
        //        if (dt.Rows.Count == 1)
        //        {
        //            objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dt.Rows[0]["CommitteeCategoryId"]);
        //            objCommitteeCategoryMaster.CategoryName = dt.Rows[0]["CategoryName"].ToString();
        //            objCommitteeCategoryMaster.ParentName = (dt.Rows[0]["ParentName"] != DBNull.Value ? dt.Rows[0]["ParentName"].ToString() : null);
        //            objCommitteeCategoryMaster.PageLevel = (dt.Rows[0]["PageLevel"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["PageLevel"]) : 1);
        //            objCommitteeCategoryMaster.PageParentId = (dt.Rows[0]["PageParentId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["PageParentId"]) : 0);
        //            objCommitteeCategoryMaster.IdPath = (dt.Rows[0]["IdPath"] != DBNull.Value ? dt.Rows[0]["IdPath"].ToString() : "0");
        //            objCommitteeCategoryMaster.YearId = (dt.Rows[0]["YearId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["YearId"]) : 0);

        //            objCommitteeCategoryMaster.Position = (dt.Rows[0]["Position"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["Position"]) : 0);
               
        //            objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
        //            objCommitteeCategoryMaster.UpdatedBy = (dt.Rows[0]["UpdatedBy"] != DBNull.Value ? dt.Rows[0]["UpdatedBy"].ToString() : null);
        //            objCommitteeCategoryMaster.InsertedBy = (dt.Rows[0]["InsertedBy"] != DBNull.Value ? dt.Rows[0]["InsertedBy"].ToString() : null);
        //            objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
        //            objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"]);
        //        }






        //        if (dt.Rows.Count == 2)
        //        {
        //            foreach (DataRow dr in dt_ChapterNews.Rows)
        //            {
        //                Entities.ChapterNews objChapterNews = new Entities.ChapterNews();

        //                objChapterNews.ChapterNewsId = Convert.ToInt64(dr["ChapterNewsId"]);
        //                objChapterNews.ChapterId = Convert.ToInt64(dr["ChapterId"]);
        //                objChapterNews.NewsId = Convert.ToInt64(dr["NewsId"]);

        //                lstChapterNews.Add(objChapterNews);
        //            }
        //        }
        //        objNews.lstChapterNews = lstChapterNews;







        //    }
        //    return objCommitteeCategoryMaster;
        //}









        public Entities.CommitteeCategoryMaster GetCommitteeCategoryMasterById(Int64 MenuItemId, ref int Status)
        {

            DataSet ds0 = _CommitteeCategoryMaster.GetCommitteeCategoryMasterById(MenuItemId, ref Status);

          
            DataTable dt_CommitteeCategoryMaster = ds0.Tables[0];
            DataTable dt_ChapterCommittees = ds0.Tables[1];

            AWS_AssociationToolCore.Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = new AWS_AssociationToolCore.Entities.CommitteeCategoryMaster();


            List<Entities.ChapterCommittees> lstChapterCommittees = new List<Entities.ChapterCommittees>();
            if (MenuItemId != 0)
            {
                if (dt_CommitteeCategoryMaster.Rows.Count == 1)
                {
                    objCommitteeCategoryMaster.CommitteeCategoryId = Convert.ToInt64(dt_CommitteeCategoryMaster.Rows[0]["CommitteeCategoryId"]);
                    objCommitteeCategoryMaster.CategoryName = dt_CommitteeCategoryMaster.Rows[0]["CategoryName"].ToString();
                    objCommitteeCategoryMaster.ParentName = (dt_CommitteeCategoryMaster.Rows[0]["ParentName"] != DBNull.Value ? dt_CommitteeCategoryMaster.Rows[0]["ParentName"].ToString() : null);
                    objCommitteeCategoryMaster.PageLevel = (dt_CommitteeCategoryMaster.Rows[0]["PageLevel"] != DBNull.Value ? Convert.ToInt32(dt_CommitteeCategoryMaster.Rows[0]["PageLevel"]) : 1);
                    objCommitteeCategoryMaster.PageParentId = (dt_CommitteeCategoryMaster.Rows[0]["PageParentId"] != DBNull.Value ? Convert.ToInt64(dt_CommitteeCategoryMaster.Rows[0]["PageParentId"]) : 0);
                    objCommitteeCategoryMaster.IdPath = (dt_CommitteeCategoryMaster.Rows[0]["IdPath"] != DBNull.Value ? dt_CommitteeCategoryMaster.Rows[0]["IdPath"].ToString() : "0");
                    objCommitteeCategoryMaster.YearId = (dt_CommitteeCategoryMaster.Rows[0]["YearId"] != DBNull.Value ? Convert.ToInt32(dt_CommitteeCategoryMaster.Rows[0]["YearId"]) : 0);

                    objCommitteeCategoryMaster.Position = (dt_CommitteeCategoryMaster.Rows[0]["Position"] != DBNull.Value ? Convert.ToInt32(dt_CommitteeCategoryMaster.Rows[0]["Position"]) : 0);

                    objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(dt_CommitteeCategoryMaster.Rows[0]["IsActive"]);
                    objCommitteeCategoryMaster.UpdatedBy = (dt_CommitteeCategoryMaster.Rows[0]["UpdatedBy"] != DBNull.Value ? dt_CommitteeCategoryMaster.Rows[0]["UpdatedBy"].ToString() : null);
                    objCommitteeCategoryMaster.InsertedBy = (dt_CommitteeCategoryMaster.Rows[0]["InsertedBy"] != DBNull.Value ? dt_CommitteeCategoryMaster.Rows[0]["InsertedBy"].ToString() : null);
                    objCommitteeCategoryMaster.UpdatedDate = Convert.ToDateTime(dt_CommitteeCategoryMaster.Rows[0]["UpdatedDate"]);
                    objCommitteeCategoryMaster.InsertedDate = Convert.ToDateTime(dt_CommitteeCategoryMaster.Rows[0]["InsertedDate"]);

                }
            }

            if (dt_ChapterCommittees.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_ChapterCommittees.Rows)
                {
                    Entities.ChapterCommittees objChapterCommittees = new Entities.ChapterCommittees();

                    objChapterCommittees.ChapterCommitteeId = Convert.ToInt64(dr["ChapterCommitteeId"]);
                    objChapterCommittees.ChapterId = Convert.ToInt64(dr["ChapterId"]);
                    objChapterCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);

                    lstChapterCommittees.Add(objChapterCommittees);
                }
            }
            objCommitteeCategoryMaster.lstChapterCommittees = lstChapterCommittees;

            return objCommitteeCategoryMaster;
        }


    }
}
