using AWS_AssociationToolCore.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using static DocumentFormat.OpenXml.Packaging.RelationshipErrorHandler;
namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class GalleryController : Controller
    {
        BLL.VideoCategories _VideoCategories = new BLL.VideoCategories();
        BLL.Videos _Videos = new BLL.Videos();
        BLL.Photos _Photos = new BLL.Photos();
        BLL.News _News = new BLL.News();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();

        public ActionResult Photos(int Year = 0, Int64 ChapterId = 0)
        {
            int status = 0;

            // Read cookie safely
            string cname = string.Empty;

            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is structured (like JSON), parse it accordingly
                cname = cookieValue;
            }


            Entities.InnerPages objmembershipcontent = new Entities.InnerPages();
            int qstatus = 0;
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }




            List<Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();
            List<Entities.PhotoCategories> lstPhotoCategoriess = new List<Entities.PhotoCategories>();

            try
            {
                lstPhotos = _Photos.FEGetPhotoList(ref lstPhotoCategories, Year, ChapterId,ref status);
                lstPhotoCategoriess = _Photos.GetPhotoCategories(ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
          
            ViewBag.lstPhotoCategories = lstPhotoCategories;
            ViewBag.lstPhotos = lstPhotos;
            ViewBag.lstPhotoCategoriess = lstPhotoCategoriess;
            ViewBag.Year = Year;
            ViewBag.cname = cname;
            ViewBag.ChapterId = ChapterId;
            return View();
        }
        public ActionResult PhotosList(string CategoryName)
        {
            int status = 0;
            List<Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();

            // Read cookie safely
            string cname = string.Empty;

            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is structured (like JSON), parse it accordingly
                cname = cookieValue;
            }
            try
            {
                lstPhotos = _Photos.FEGetPhotosListById(CategoryName, ref lstPhotoCategories, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstPhotoCategories = lstPhotoCategories;
            ViewBag.CategoryName = CategoryName;
            ViewBag.lstPhotos = lstPhotos;
            ViewBag.cname = cname;

            return View();
        }


        public ActionResult Videos(Int64 ChapterId=0, string cname = "")
        {
            // Set current year
          

            // Read cookie (ASP.NET Core way)
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is JSON, deserialize (recommended)
                // Example assumes simple value (adjust if needed)
                cname = cookieValue;
            }


            int status = 0;
            Entities.InnerPages objmembershipcontent = new Entities.InnerPages();
            int qstatus = 0;
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            List<Entities.VideoCategories> lstVideoCategories = new List<Entities.VideoCategories>();
            List<Entities.Videos> lstVideos = new List<Entities.Videos>();
            Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();
            try
            {
                lstVideos = _Videos.FEGetVideoList(ChapterId,ref lstVideoCategories, ref status);

                Int64 qmstatus = 0;
                objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl(cname + "/video-gallery", ref qmstatus);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.lstVideos=lstVideos;
            ViewBag.lstVideoCategories = lstVideoCategories;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            return View();
        }

        public ActionResult WebinarVideos(Int64 ChapterId = 0)
        {
            List<Entities.VideoCategories> lstVideoCategories = new List<Entities.VideoCategories>();
            List<Entities.Videos> lstVideos = new List<Entities.Videos>();
            int status = 0;
            try
            {
                lstVideos = _Videos.FEGetWebinarVideoList(ChapterId,ref lstVideoCategories, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstVideos = lstVideos;
            ViewBag.lstVideoCategories = lstVideoCategories;
            ViewBag.ChapterId = ChapterId;

            return View();
        }

        public ActionResult MediaCoverage(string CategoryName = "media-coverage", string cname = "")
        {
            List<Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();
            int status = 0;
            try
            {
                lstPhotos = _Photos.FEGetPhotosListById(CategoryName, ref lstPhotoCategories, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstPhotos = lstPhotos;
            ViewBag.lstPhotoCategories = lstPhotoCategories;
            ViewBag.CategoryName = CategoryName;
            return View();
        }

        public ActionResult Sponsors(Int64 ChapterId = 0, string cname = "")
        {
            int status = 0;

            // Read cookie safely

            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is structured (like JSON), parse it accordingly
                cname = cookieValue;
            }
            List<Entities.SponsorCategories> lstSponsorCategories = new List<Entities.SponsorCategories>();
            List<Entities.Sponsors> lstSponsors = new List<Entities.Sponsors>();
            int qstatus = 0;
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            try
            {
                lstSponsors = _Photos.FEGetSponsorsList(ChapterId,ref lstSponsorCategories, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstSponsorCategories = lstSponsorCategories;
            ViewBag.lstSponsors = lstSponsors;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            return View();
        }


        public ActionResult News(Int64 ChapterId=0)
        {
            int status = 0;

            List<Entities.News> lstNews = new List<Entities.News>();
            try
            {
                lstNews = _News.FEGetNewsList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstNews = lstNews;
            ViewBag.ChapterId = ChapterId;

            return View();
        }


    }
}
