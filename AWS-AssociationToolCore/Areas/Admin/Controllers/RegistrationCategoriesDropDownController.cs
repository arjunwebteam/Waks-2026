using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,DeveloperAdmin,")]
    public class RegistrationCategoriesDropDownController : Controller
    {
        BLL.RegistrationCategoriesDropDown _DonationCategory = new BLL.RegistrationCategoriesDropDown();
        List<Entities.RegistrationCategoriesDropDown> lstRegistrationCategoriesDropDown = new List<Entities.RegistrationCategoriesDropDown>();

        #region RegistrationCategoriesDropDown

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult RegistrationCategoriesDropDownList(string Search = "", string SortColumn = "OrderNo", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstRegistrationCategoriesDropDown = _DonationCategory.GetRegistrationCategoriesDropDownListByVariable(Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstRegistrationCategoriesDropDown = lstRegistrationCategoriesDropDown;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();

            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddRegistrationCategories(Entities.RegistrationCategoriesDropDown objRegistrationCategoriesDropDown)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objRegistrationCategoriesDropDown.InsertedBy = HttpContext.User.Identity.Name.ToString();
                objRegistrationCategoriesDropDown.InsertedTime = DateTime.UtcNow;
                objRegistrationCategoriesDropDown.IsActive = false;


                Int64 _status = _DonationCategory.InsertRegistrationCategoriesDropDown(objRegistrationCategoriesDropDown);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Registration Category details successfully.</div>";
                        return RedirectToAction("Index", "RegistrationCategoriesDropDown");
                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Registration Category details successfully.</div>";
                        return RedirectToAction("Index", "RegistrationCategoriesDropDown");
                    case 3:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\"> '" + objRegistrationCategoriesDropDown.CategoryName + "' is already existed.</div>";
                        return RedirectToAction("Index", "RegistrationCategoriesDropDown");
                    case -1:
                    default:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                        return RedirectToAction("Index", "RegistrationCategoriesDropDown");
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                _bool = false;
            }

            return Json(new { ok = _bool, data = str });

        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult EditRegistrationCategories(Int64 RegistrationCategoriesCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.RegistrationCategoriesDropDown _objDonationCategory = _DonationCategory.GetRegistrationCategoriesById(RegistrationCategoriesCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objDonationCategory });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewRegistrationCategories(Int64 RegistrationCategoriesCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.RegistrationCategoriesDropDown _objDonationCategory = _DonationCategory.GetRegistrationCategoriesById(RegistrationCategoriesCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objDonationCategory });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteRegistrationCategories(Int64 RegistrationCategoriesCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _DonationCategory.DeleteRegistrationCategoriesCategory(RegistrationCategoriesCategoryId);
                if (_status == 1)
                {

                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting Donation Category</div>";
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
        [Models.SessionClass.SessionExpireFilter]
        public JsonResult RegistrationCategoriesStatus(Int64 RegistrationCategoriesCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _DonationCategory.UpdateRegistrationCategoriesStatus(RegistrationCategoriesCategoryId);
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
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        public JsonResult RegistrationCategoriesDisplayOrder(int DisplayOrder, Int64 RegistrationCategoriesCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _DonationCategory.UpdateRegistrationCategoriesDisplayOrder(DisplayOrder, RegistrationCategoriesCategoryId);
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
