using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,Helpline,DeveloperAdmin,")]
    public class ServiceTypesController : Controller
    {
        BLL.ServiceTypes _ServiceTypes = new BLL.ServiceTypes();
        List<Entities.ServiceTypes> lstServiceTypes = new List<Entities.ServiceTypes>();
        BLL.Chapters _Chapters = new BLL.Chapters();

        #region ServiceTypes

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            int Total = 0;
            ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);

            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ServiceTypesList(string Search = "", string SortColumn = "S.UpdateDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstServiceTypes = _ServiceTypes.GetServiceTypesListByVariable(Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"error closable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServiceTypes = lstServiceTypes;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddServiceType(Entities.ServiceTypes objProjectCategory)
        {

            string str = "";
            bool _bool = true;
            try
            {
                
                objProjectCategory.IsActive = true;
                objProjectCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objProjectCategory.UpdateDate = DateTime.UtcNow;
                 Int64 _status = _ServiceTypes.InsertServiceTypes(objProjectCategory);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Service Type successfully.</div>";
                        return RedirectToAction("Index", "ServiceTypes");

                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Service Type successfully.</div>";
                        return RedirectToAction("Index", "ServiceTypes");

                    case -1:
                    default:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                        return RedirectToAction("Index", "ServiceTypes");
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed transaction.</div>";
                _bool = false;
            }

            return RedirectToAction("Index", "ServiceTypes");

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditServiceType(Int64 ServiceTypeId = 0)
        {

            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.ServiceTypes _objServiceTypes = _ServiceTypes.GetServiceTypeById(ServiceTypeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objServiceTypes });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteServiceType(Int64 ServiceTypeId)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceTypes.DeleteServiceType(ServiceTypeId);
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
                str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult UpdateServiceTypeStatus(Int64 ServiceTypeId)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceTypes.UpdateServiceTypeStatus(ServiceTypeId);
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


        [HttpPost]
        public JsonResult UpdateServiceTypesDisplayOrder(int OrderNo, Int64 ServiceTypeId)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceTypes.UpdateServiceTypesDisplayOrder(OrderNo, ServiceTypeId);
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
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }


        #endregion

    }
}
