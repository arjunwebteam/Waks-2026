using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Chapters,SubAdmin,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class ServiceCategoriesController : Controller
    {
        BLL.ServiceCategories _ServiceCategories = new BLL.ServiceCategories();
        List<Entities.ServiceCategories> lstServiceCategories = new List<Entities.ServiceCategories>();
        BLL.Chapters _Chapters = new BLL.Chapters();

        #region ServiceCategories

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            int Total = 0;
            ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);

            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ServiceCategoriesList(Int64 ChapterId=0,string Search = "", string SortColumn = "IC.UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstServiceCategories = _ServiceCategories.GetServiceCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"error closable\">Failed transaction.</div>";
            }
            
            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServiceCategories = lstServiceCategories;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddServiceCategory(Entities.ServiceCategories objProjectCategory)
        {

            string str = "";
            bool _bool = true;
            try
            {
                objProjectCategory.IconUrl = null;
                objProjectCategory.IsActive = true;
                objProjectCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _ServiceCategories.InsertServiceCategory(objProjectCategory);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Category details successfully.</div>";
                        return RedirectToAction("Index", "ServiceCategories");

                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Category details successfully.</div>";
                        return RedirectToAction("Index", "ServiceCategories");

                    case -1:
                    default:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                        return RedirectToAction("Index", "ServiceCategories");
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed transaction.</div>";
                _bool = false;
            }

            return RedirectToAction("Index", "ServiceCategories");

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditServiceCategory(Int64 ServiceCategoryId = 0)
        {

            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.ServiceCategories _objServiceCategories = _ServiceCategories.GetServiceCategoryById(ServiceCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objServiceCategories });
                }
                else
                {
                    str = "<div class=\"success closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteServiceCategory(Int64 ServiceCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceCategories.DeleteServiceCategory(ServiceCategoryId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult UpdateServiceCategoryStatus(Int64 ServiceCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceCategories.UpdateServiceCategoryStatus(ServiceCategoryId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
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
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                return Json(new { ok = false, data = str });
            }
        }

        #endregion

    }
}
