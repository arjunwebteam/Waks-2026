using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class PageDetailsController : Controller  // ✅ Fix 1: BaseController → Controller
    {
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        BLL.Redirections _Redirections = new BLL.Redirections();
        Entities.Redirections objRedirections = new Entities.Redirections();

        public ActionResult Index()
        {
            return View();
        }

        //[Authorize]
        public ActionResult GetPageDetails(string PageUrl = "", Int64 mrid = 0, Int64 ChapterId = 0, string Type = "")
        {
            if (Type == "API")
            {
                // ✅ Fix 2: RedirectToActionResult → RedirectToAction
                return RedirectToAction("APIPageDetails", "WebAPI", new { APIPageUrl = PageUrl });
            }

            // ✅ Fix 3: Request.Url.AbsoluteUri → ASP.NET Core way
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            int status1 = 0;

            objRedirections = _Redirections.GetRedirectionsByurl(url, ref status1);

            if (objRedirections.ToUrl != null && objRedirections.ToUrl != "")
            {
                return new RedirectResult(objRedirections.ToUrl);
            }

            Int64 status = 0;
            string IDPath = "";
            string ChapterName = "";

            long userId = 0;

            var userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                long.TryParse(userIdStr, out userId);
            }

            if (Request.Cookies.TryGetValue("UserRole", out string userRoleValue))
            {
                HttpContext.Session.SetString("userrole", userRoleValue);
            }
            long chapterId = 0;

            var chapterValue = HttpContext.Session.GetString("chapterid");

            if (!string.IsNullOrEmpty(chapterValue))
            {
                long.TryParse(chapterValue, out chapterId);
            }

            Entities.PageDetails objPageDetails = new Entities.PageDetails();
            Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();

            try
            {
                objPageDetails = _PageDetails.GetPageDetailsListById(chapterId, mrid, ref IDPath, "", PageUrl, ref status);
                objMenuPageDetails = _PageDetails.GetBreadCrumbListById(chapterId, mrid, ref status);

                if (objPageDetails == null || objPageDetails.PageDetailId == 0)
                {
                    return RedirectToAction("UpdateSoon", "PageDetails", new { cname = ChapterName });
                }
            }
            catch
            {
                status = -1;
            }
            HttpContext.Session.SetString("mrid", mrid.ToString());
            HttpContext.Session.SetString("PageUrl", PageUrl ?? "");
            ViewBag.cname = ChapterName;
            ViewBag.MenuItemId = mrid;
            ViewBag.objPageDetails = objPageDetails;
            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.IDPath = IDPath;
            ViewBag.PageUrl = PageUrl;
            ViewBag.ChapterId = ChapterId;

            return View();
        }

        public ActionResult UpdateSoon(string cname = "")
        {
            ViewBag.cname = cname;
            return View();
        }

    }
}