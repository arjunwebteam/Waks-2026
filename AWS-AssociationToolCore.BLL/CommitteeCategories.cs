using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AWS_AssociationToolCore.DAL;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
    public class CommitteeCategories
    {
        AWS_AssociationToolCore.DAL.CommitteeCategories _CommitteeCategories = new AWS_AssociationToolCore.DAL.CommitteeCategories();        

        #region Methods

        public Int64 InsertCommitteeCategories(Entities.CommitteeCategories objCommitteeCategories)
        {
            Int64 _status = 0;
            if (objCommitteeCategories != null )
            {
                _status = _CommitteeCategories.InsertCommitteeCategories(objCommitteeCategories);

            }
            return _status;
        }

        public Int64 DeleteCommitteeCategory(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            _status = _CommitteeCategories.DeleteCommitteeCategory(CommitteeCategoryId);
            return _status;
        }

        public Int64 UpdateCommitteeCategoryStatus(Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            _status = _CommitteeCategories.UpdateCommitteeCategoryStatus(CommitteeCategoryId);
            return _status;
        }

        public Int64 UpdateCommitteeCategoriesDisplayOrder(int DisplayOrder, Int64 CommitteeCategoryId)
        {
            Int64 _status = 0;
            _status = _CommitteeCategories.UpdateCommitteeCategoriesDisplayOrder(DisplayOrder, CommitteeCategoryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.CommitteeCategories> GetCommitteeCategoriesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            DataTable dt = _CommitteeCategories.GetCommitteeCategoriesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    //objlstCommitteeCategories.Type = dr["Type"].ToString();
                    //objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                    objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //objlstCommitteeCategories.Year = dr["Year"].ToString();
                    //objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    //objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }

            }
            return lstCommitteeCategories;
        } 

        public List<AWS_AssociationToolCore.Entities.CommitteeCategories> GetPastCommitteeCategoriesList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            DataTable dt = _CommitteeCategories.GetPastCommitteeCategoriesList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstCommitteeCategories.Type = dr["Type"].ToString();
                    objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                    objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstCommitteeCategories.Year = dr["Year"].ToString();
                    objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }

            }
            return lstCommitteeCategories;
        }

        public AWS_AssociationToolCore.Entities.CommitteeCategories GetCommitteeCategoryById(Int64 CommitteeCategoryId, ref int status)
        {  
                DataSet ds0 = _CommitteeCategories.GetCommitteeCategoryById(CommitteeCategoryId, ref status);
                DataTable dt_Members = ds0.Tables[0];
                DataTable dt_ChapterCommittees = ds0.Tables[1];
                DataTable dt_Chapters = ds0.Tables[2];

                Entities.CommitteeCategories objCommitteeCategories = new Entities.CommitteeCategories();
                List<Entities.ChapterCommittees> lstChapterCommittees = new List<Entities.ChapterCommittees>();
                List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();

                if (dt_Members.Rows.Count == 1)
                {
                    objCommitteeCategories.CommitteeCategoryId = Convert.ToInt32(dt_Members.Rows[0]["CommitteeCategoryId"].ToString());
                    objCommitteeCategories.CategoryName = dt_Members.Rows[0]["CategoryName"].ToString();
                    objCommitteeCategories.Type = dt_Members.Rows[0]["Type"].ToString();
                    objCommitteeCategories.DisplayOrder = Convert.ToInt32(dt_Members.Rows[0]["DisplayOrder"]);
                    objCommitteeCategories.IsActive = Convert.ToBoolean(dt_Members.Rows[0]["IsActive"]);
                    objCommitteeCategories.Year = dt_Members.Rows[0]["Year"].ToString();
                    objCommitteeCategories.UpdatedBy = dt_Members.Rows[0]["UpdatedBy"].ToString();
                    objCommitteeCategories.UpdatedTime = Convert.ToDateTime(dt_Members.Rows[0]["UpdatedTime"].ToString());
                  
                }

            if (dt_ChapterCommittees.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_ChapterCommittees.Rows)
                {
                    Entities.ChapterCommittees objChapterCommittees = new Entities.ChapterCommittees();

                    objChapterCommittees.ChapterCommitteeId = Convert.ToInt64(dr["ChapterCommitteeId"]);
                    objChapterCommittees.ChapterId = Convert.ToInt64(dr["ChapterId"]);
                    objChapterCommittees.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"]);
                    objChapterCommittees.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objChapterCommittees.OrderNo = Convert.ToInt32(dr["OrderNo"]);
                    objChapterCommittees.UpdatedBy = dr["UpdatedBy"].ToString();
                    objChapterCommittees.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstChapterCommittees.Add(objChapterCommittees);
                }
            }
            objCommitteeCategories.lstChapterCommittees = lstChapterCommittees;

                if (dt_Chapters.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt_Chapters.Rows)
                    {
                        Entities.Chapters objChapters = new Entities.Chapters();

                        objChapters.ChapterId = Convert.ToInt64(dr["ChapterId"].ToString());
                        objChapters.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);
                        objChapters.ShortDescription = (dr["ShortDescription"] != DBNull.Value ? (dr["ShortDescription"].ToString()) : null);
                        objChapters.ShortName = (dr["ShortName"] != DBNull.Value ? (dr["ShortName"].ToString()) : null);
                        objChapters.Description = (dr["Description"] != DBNull.Value ? (dr["Description"].ToString()) : null);
                        objChapters.Address = (dr["Address"] != DBNull.Value ? (dr["Address"].ToString()) : null);
                        objChapters.City = (dr["City"] != DBNull.Value ? (dr["City"].ToString()) : null);
                        objChapters.State = (dr["State"] != DBNull.Value ? (dr["State"].ToString()) : null);
                        objChapters.ZipCode = (dr["ZipCode"] != DBNull.Value ? (dr["ZipCode"].ToString()) : null);
                        objChapters.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                        objChapters.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                        objChapters.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? (dr["UpdatedBy"].ToString()) : null);
                        objChapters.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());

                        lstChapters.Add(objChapters); 
                    }
                }
            objCommitteeCategories.lstChapters = lstChapters;
            return objCommitteeCategories;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeCategories> GetCommitteeCategoriesListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories = new List<AWS_AssociationToolCore.Entities.CommitteeCategories>();
            DataTable dt = _CommitteeCategories.GetCommitteeCategoriesListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CommitteeCategories.GetCommitteeCategoriesListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objCommitteeCategories.RId = Convert.ToInt64(dr["RId"].ToString());
                    objCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    objCommitteeCategories.Type = dr["Type"].ToString();
                    objCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                    objCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objCommitteeCategories.Year = dr["Year"].ToString();
                    objCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objCommitteeCategories.CommitteeMemberCount = Convert.ToInt64(dr["CommitteeMemberCount"].ToString());

                    lstCommitteeCategories.Add(objCommitteeCategories);
                }
            }
            return lstCommitteeCategories;
        }

        public List<AWS_AssociationToolCore.Entities.CommitteeCategories> GetCommitteeCategoriesListByChapterId(Int64 ChapterId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            DataTable dt = _CommitteeCategories.GetCommitteeCategoriesListByChapterId(ChapterId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CommitteeCategories objlstCommitteeCategories = new AWS_AssociationToolCore.Entities.CommitteeCategories();

                    objlstCommitteeCategories.CommitteeCategoryId = Convert.ToInt64(dr["CommitteeCategoryId"].ToString());
                    objlstCommitteeCategories.CategoryName = dr["CategoryName"].ToString();
                    objlstCommitteeCategories.Type = dr["Type"].ToString();
                    objlstCommitteeCategories.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                    objlstCommitteeCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstCommitteeCategories.Year = dr["Year"].ToString();
                    objlstCommitteeCategories.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstCommitteeCategories.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstCommitteeCategories.Add(objlstCommitteeCategories);
                }

            }
            return lstCommitteeCategories;
        }
        #endregion
    }
}
