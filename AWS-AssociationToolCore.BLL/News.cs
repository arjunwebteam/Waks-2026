using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class News
    {
        //string logourl = System.Configuration.ConfigurationManager.AppSettings["adminimgurl"] + "/news/";
        AWS_AssociationToolCore.DAL.News _News = new AWS_AssociationToolCore.DAL.News();

        #region Methods

       public Int64 InsertNews(Entities.News objNews, ref string ImageUrl)
        {
            Int64 _status = 0;
            if (objNews != null )
            {
                _status = _News.InsertNews(objNews, ref ImageUrl);

            }
            return _status;
        }

       public Int64 DeleteNews(Int64 NewsId)
        {
            Int64 _status = 0;
            _status = _News.DeleteNews(NewsId);
            return _status;
        }

       public Int64 UpdateNewsStatus(Int64 NewsId)
       {
           Int64 _status = 0;
           _status = _News.UpdateNewsStatus(NewsId);
           return _status;
       }

       public Int64 UpdateNewsDisplayOrder(int OrderNo, Int64 NewsId)
       {
           Int64 _status = 0;
           _status = _News.UpdateNewsDisplayOrder(OrderNo, NewsId);
           return _status;
       }

        #endregion

        #region Entities filling

       public List<AWS_AssociationToolCore.Entities.News> GetNewsList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.News> lstNews = new List<Entities.News>();
            DataTable dt = _News.GetNewsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.News objlstNews = new AWS_AssociationToolCore.Entities.News();

                    objlstNews.NewsId = Convert.ToInt64(dr["NewsId"].ToString());
                    objlstNews.Title = dr["Title"].ToString();
                    objlstNews.NewsText = dr["NewsText"].ToString();
                    objlstNews.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
                    objlstNews.PostDate = Convert.ToDateTime(dr["PostDate"].ToString());
                    objlstNews.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objlstNews.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objlstNews.ExpiryDate = Convert.ToDateTime(dr["ExpiryDate"].ToString());

                    lstNews.Add(objlstNews);
                }

            }
            return lstNews;
        }

       public AWS_AssociationToolCore.Entities.News GetNewsById(Int64 NewsId, ref int status)
        {
            
            DataSet ds0 = _News.GetNewsById(NewsId, ref status);
            DataTable dt_News = ds0.Tables[0];
            DataTable dt_ChapterNews = ds0.Tables[1];

            AWS_AssociationToolCore.Entities.News objNews = new AWS_AssociationToolCore.Entities.News();
            List<Entities.ChapterNews> lstChapterNews = new List<Entities.ChapterNews>();

            if (dt_News.Rows.Count == 1)
                {
                    objNews.NewsId = Convert.ToInt64(dt_News.Rows[0]["NewsId"].ToString());
                    objNews.Title = dt_News.Rows[0]["Title"].ToString();
                    objNews.NewsText = dt_News.Rows[0]["NewsText"].ToString();
                    objNews.ImageUrl = (dt_News.Rows[0]["ImageUrl"] != DBNull.Value ? dt_News.Rows[0]["ImageUrl"].ToString() : "");
                    objNews.PostDate =Convert.ToDateTime(dt_News.Rows[0]["PostDate"].ToString());
                    objNews.OrderNo = (dt_News.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt64(dt_News.Rows[0]["OrderNo"]) : 0);
                    objNews.IsActive = Convert.ToBoolean(dt_News.Rows[0]["IsActive"]);
                    objNews.UpdatedBy = dt_News.Rows[0]["UpdatedBy"].ToString();
                    objNews.UpdatedTime = Convert.ToDateTime(dt_News.Rows[0]["UpdatedTime"].ToString());
                    objNews.ChapterId = (dt_News.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt_News.Rows[0]["ChapterId"]) : 0);
                    objNews.ExpiryDate = (dt_News.Rows[0]["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dt_News.Rows[0]["ExpiryDate"].ToString()) : DateTime.MinValue);

            }

            if (dt_ChapterNews.Rows.Count != 0)
            {
                foreach (DataRow dr in dt_ChapterNews.Rows)
                {
                    Entities.ChapterNews objChapterNews = new Entities.ChapterNews();

                    objChapterNews.ChapterNewsId = Convert.ToInt64(dr["ChapterNewsId"]);
                    objChapterNews.ChapterId = Convert.ToInt64(dr["ChapterId"]);
                    objChapterNews.NewsId = Convert.ToInt64(dr["NewsId"]);

                    lstChapterNews.Add(objChapterNews);
                }
            }
            objNews.lstChapterNews = lstChapterNews;

            return objNews;
        }

        public List<AWS_AssociationToolCore.Entities.News> GetNewsListByVariable(Int64 ChapterId,Int64 MemberId,string Type, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.News> lstNews = new List<AWS_AssociationToolCore.Entities.News>();
            DataTable dt = _News.GetNewsListByVariable(ChapterId,MemberId, Type, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _News.GetNewsListByVariable(ChapterId,MemberId, Search, Type, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.News objNews = new AWS_AssociationToolCore.Entities.News();

                    objNews.RId = Convert.ToInt64(dr["RId"].ToString());
                    objNews.NewsId = Convert.ToInt64(dr["NewsId"].ToString());
                    objNews.Title = dr["Title"].ToString();
                    objNews.NewsText = dr["NewsText"].ToString();
                    objNews.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
                    objNews.PostDate = Convert.ToDateTime(dr["PostDate"].ToString());
                    objNews.OrderNo = (dr["OrderNo"] != DBNull.Value ?Convert.ToInt64(dr["OrderNo"]) : 0);
                    objNews.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objNews.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objNews.ExpiryDate = (dr["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ExpiryDate"].ToString()) : DateTime.MinValue);


                    lstNews.Add(objNews);
                }
            }
            return lstNews;
        }

        public List<AWS_AssociationToolCore.Entities.News> GetAPINewsListByVariable(Int64 ChapterId, Int64 MemberId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.News> lstNews = new List<AWS_AssociationToolCore.Entities.News>();
            DataTable dt = _News.GetAPINewsListByVariable(ChapterId, MemberId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _News.GetAPINewsListByVariable(ChapterId, MemberId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.News objNews = new AWS_AssociationToolCore.Entities.News();

                    objNews.RId = Convert.ToInt64(dr["RId"].ToString());
                    objNews.NewsId = Convert.ToInt64(dr["NewsId"].ToString());
                    objNews.Title = dr["Title"].ToString();
                    objNews.NewsText = dr["NewsText"].ToString();
                    objNews.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
                    objNews.PostDate = Convert.ToDateTime(dr["PostDate"].ToString());
                    objNews.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objNews.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());
                    objNews.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objNews.ExpiryDate = Convert.ToDateTime(dr["ExpiryDate"].ToString());


                    lstNews.Add(objNews);
                }
            }
            return lstNews;
        }

        #endregion

        #region Front-end

        public List<AWS_AssociationToolCore.Entities.News> FEGetNewsList(Int64 ChapterId,ref int status)
        {
            List<AWS_AssociationToolCore.Entities.News> lstNews = new List<Entities.News>();
            DataTable dt = _News.FEGetNewsList(ChapterId,ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.News objlstNews = new AWS_AssociationToolCore.Entities.News();

                    objlstNews.NewsId = Convert.ToInt64(dr["NewsId"].ToString());
                    objlstNews.Title = dr["Title"].ToString();
                    objlstNews.NewsText = dr["NewsText"].ToString();
                    objlstNews.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : "");
                    objlstNews.PostDate = Convert.ToDateTime(dr["PostDate"].ToString());
                    objlstNews.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objlstNews.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objlstNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objlstNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"].ToString());

                    lstNews.Add(objlstNews);
                }

            }
            return lstNews;
        }

        public List<Entities.News> FENewsGetListByVariable(Int64 NewsId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<Entities.News> lstNews = new List<Entities.News>();
            DataTable dt = _News.FENewsGetListByVariable(NewsId, Search, Sort, PageNo, Items, ref Total);
            //if (dt.Rows.Count == 0 && PageNo != 0)
            //{
            //    dt = _News.GetNewsList(NewsId, Search, Sort,  (PageNo - 1), Items, ref Total);
            //}
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.News objNews = new Entities.News();
                    objNews.NewsId = Convert.ToInt64(dr["NewsId"]);
                    objNews.Title = dr["Title"].ToString();
                    objNews.NewsText = dr["NewsText"].ToString();
                    objNews.ImageUrl = (dr["ImageUrl"] == DBNull.Value ? "" : dr["ImageUrl"].ToString());
                    objNews.PostDate = Convert.ToDateTime(dr["PostDate"]);
                    objNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objNews.RId = Convert.ToInt64(dr["RId"]);

                    lstNews.Add(objNews);
                }
            }
            return lstNews;
        }

        public List<Entities.News> FENewsGetListByVariableByChapterId(Int64 NewsId, string Type, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<Entities.News> lstNews = new List<Entities.News>();
            DataTable dt = _News.FENewsGetListByVariableByChapterId(NewsId, Type, ChapterId, Search, Sort, PageNo, Items, ref Total);
            //if (dt.Rows.Count == 0 && PageNo != 0)
            //{
            //    dt = _News.GetNewsList(NewsId, Search, Sort,  (PageNo - 1), Items, ref Total);
            //}
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.News objNews = new Entities.News();
                    objNews.NewsId = Convert.ToInt64(dr["NewsId"]);
                    objNews.Title = dr["Title"].ToString();
                    objNews.NewsText = dr["NewsText"].ToString();
                    objNews.ImageUrl = (dr["ImageUrl"] == DBNull.Value ? "" : dr["ImageUrl"].ToString());
                    objNews.PostDate = Convert.ToDateTime(dr["PostDate"]);
                    objNews.UpdatedBy = dr["UpdatedBy"].ToString();
                    objNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objNews.RId = Convert.ToInt64(dr["RId"]); 

                    lstNews.Add(objNews);
                }
            }
            return lstNews;
        }

        #endregion


        //#region API
        //public List<Entities.News> APIFENewsGetListByVariable(Int64 NewsId, string Search, string Sort, int PageNo, int Items, ref int Total)
        //{
        //    List<Entities.News> lstNews = new List<Entities.News>();
        //    DataTable dt = _News.FENewsGetListByVariable(NewsId, Search, Sort, PageNo, Items, ref Total);
        //    //if (dt.Rows.Count == 0 && PageNo != 0)
        //    //{
        //    //    dt = _News.GetNewsList(NewsId, Search, Sort,  (PageNo - 1), Items, ref Total);
        //    //}
        //    if (dt.Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            Entities.News objNews = new Entities.News();
        //            objNews.NewsId = Convert.ToInt64(dr["NewsId"]);
        //            objNews.Title = dr["Title"].ToString();
        //            objNews.NewsText = dr["NewsText"].ToString();
        //            objNews.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? logourl + dr["ImageUrl"].ToString() : "");
        //            //objNews.ImageUrl = (dr["ImageUrl"] == DBNull.Value ? "" : dr["ImageUrl"].ToString());
        //            objNews.PostDate = Convert.ToDateTime(dr["PostDate"]);
        //            objNews.UpdatedBy = dr["UpdatedBy"].ToString();
        //            objNews.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
        //            objNews.RId = Convert.ToInt64(dr["RId"]);

        //            lstNews.Add(objNews);
        //        }
        //    }
        //    return lstNews;
        //}
        //#endregion
    }
}
