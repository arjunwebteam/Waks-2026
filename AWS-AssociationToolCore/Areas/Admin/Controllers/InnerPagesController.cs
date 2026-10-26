using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
//using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
      [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,InnerPages,SubAdmin,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class InnerPagesController : Controller
    {
        BLL.InnerPages _innerpages = new BLL.InnerPages();
        List<Entities.InnerPages> lstInnerPages = new List<Entities.InnerPages>();
        BLL.InnerPageCategories _InnerPageCategory = new BLL.InnerPageCategories();
        List<Entities.InnerPageCategories> lstInnerPageCategory = new List<Entities.InnerPageCategories>();

        #region InnerPages

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public IActionResult Index(long InnerPageCategoryId = 0)
        {
            try
            {
                // Get user role from session
                ViewBag.userrole = HttpContext.Session.GetString("userrole") ?? "";

                int qStatus = 0;

                // Initialize lists
                var lstInnerPageCategories = new List<Entities.InnerPageCategories>();
                var lstInnerPageCategories2 = new List<Entities.InnerPageCategories>();
                var lstInnerPageCategories3 = new List<Entities.InnerPageCategories>();
                var lstInnerPageCategories4 = new List<Entities.InnerPageCategories>();

                int status = 0;

                // Fetch all inner page categories
                lstInnerPageCategories = _InnerPageCategory.GetInnerPageCategoriesAll(
                    ref lstInnerPageCategories2,
                    ref lstInnerPageCategories3,
                    ref lstInnerPageCategories4,
                    ref status
                );

                if (status == 1)
                {
                    ViewBag.lstInnerPageCategories = lstInnerPageCategories;
                    ViewBag.lstInnerPageCategories2 = lstInnerPageCategories2;
                    ViewBag.lstInnerPageCategories3 = lstInnerPageCategories3;
                    ViewBag.lstInnerPageCategories4 = lstInnerPageCategories4;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "InnerPages");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">Failed transaction. {ex.Message}</div>";
            }

            ViewBag.InnerPageCategoryId = InnerPageCategoryId;
            return View();
        }
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult DetailsList(Int64 InnerPageCategoryId = 0, string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstInnerPages = _innerpages.GetInnerPagesListByVariable(InnerPageCategoryId,Search, Sort, PageNo, Items, ref Total);
                
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstInnerPages = lstInnerPages;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult AddDetails(String Heading = "", Int64 InnerPageCategoryId = 0)
        {
            try
            {

                List<Entities.InnerPageCategories> lstInnerPageCategories2 = new List<Entities.InnerPageCategories>();
                List<Entities.InnerPageCategories> lstInnerPageCategories3 = new List<Entities.InnerPageCategories>();
                List<Entities.InnerPageCategories> lstInnerPageCategories4 = new List<Entities.InnerPageCategories>();
                int status = 0;
                List<Entities.InnerPageCategories> lstInnerPageCategories = _InnerPageCategory.GetInnerPageCategoriesAll(ref lstInnerPageCategories2, ref lstInnerPageCategories3, ref lstInnerPageCategories4, ref status);
                if (status == 1)
                {
                    ViewBag.lstInnerPageCategories = lstInnerPageCategories;
                    ViewBag.lstInnerPageCategories2 = lstInnerPageCategories2;
                    ViewBag.lstInnerPageCategories3 = lstInnerPageCategories3;
                    ViewBag.lstInnerPageCategories4 = lstInnerPageCategories4;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "InnerPages");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "InnerPages");
            }
            ViewBag.Heading = Heading;
            ViewBag.InnerPageCategoryId = InnerPageCategoryId;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddDetails(Entities.InnerPages objInnerPages)
        {
            try
            {               
                objInnerPages.InsertedBy = HttpContext.User.Identity.Name.ToString();
                objInnerPages.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objInnerPages.IsActive = true;
                Int64 _status = _innerpages.InsertInnerPages(objInnerPages);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "InnerPages");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("EditDetails", "InnerPages", new { InnerPageId = objInnerPages.InnerPageId });
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("AddDetails", "InnerPages");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("AddDetails", "InnerPages");
            }

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditDetails(Int64 InnerPageId = 0)
        {
            try
            {

                List<Entities.InnerPageCategories> lstInnerPageCategories2 = new List<Entities.InnerPageCategories>();
                List<Entities.InnerPageCategories> lstInnerPageCategories3 = new List<Entities.InnerPageCategories>();
                List<Entities.InnerPageCategories> lstInnerPageCategories4 = new List<Entities.InnerPageCategories>();
                int status = 0;
                List<Entities.InnerPageCategories> lstInnerPageCategories = _InnerPageCategory.GetInnerPageCategoriesAll(ref lstInnerPageCategories2, ref lstInnerPageCategories3, ref lstInnerPageCategories4, ref status);
                if (status == 1)
                {
                    ViewBag.lstInnerPageCategories = lstInnerPageCategories;
                    ViewBag.lstInnerPageCategories2 = lstInnerPageCategories2;
                    ViewBag.lstInnerPageCategories3 = lstInnerPageCategories3;
                    ViewBag.lstInnerPageCategories4 = lstInnerPageCategories4;
                }

                int _qstatus = 0;
                Entities.InnerPages _objInnerPages = _innerpages.GetInnerPagesById(InnerPageId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objDetails = _objInnerPages;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\" alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "InnerPages");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "InnerPages");
            }
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewDetails(Int64 InnerPageId = 0)
        {
            try
            {
               
                int _qstatus = 0;
                Entities.InnerPages _objInnerPages = _innerpages.GetInnerPagesById(InnerPageId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objDetails = _objInnerPages;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "InnerPages");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "InnerPages");
            }
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteDetails(Int64 InnerPageId)
        {
            string str = "";
            try
            {
               
                Int64 _status = _innerpages.DeleteInnerPages(InnerPageId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting page</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch 
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        public JsonResult InnerPageDisplayOrder(int DisplayOrder, Int64 InnerPageId)
        {
            string str = "";
            try
            {
                Int64 _status = _innerpages.UpdateInnerPagesDisplayOrder(DisplayOrder, InnerPageId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch 
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }


        [HttpPost]
        public JsonResult InnerPageStatus(Int64 InnerPageId)
        {
            string str = "";
            try
            {
                Int64 _status = _innerpages.UpdateInnerPagesStatus(InnerPageId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated status successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
            }
        }


        #endregion

    }
}
