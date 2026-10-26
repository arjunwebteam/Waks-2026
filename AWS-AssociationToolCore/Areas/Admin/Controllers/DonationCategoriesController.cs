using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class DonationCategoriesController : Controller
    {
        BLL.DonationCategories _DonationCategory = new BLL.DonationCategories();
        List<Entities.DonationCategories> lstDonationCategory = new List<Entities.DonationCategories>();
        BLL.Chapters _Chapters = new BLL.Chapters();

        #region DonationCategories

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            int Total = 0;
            ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);

            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult DonationCategoriesList(Int64 ChapterId=0,string Search = "", string SortColumn = "OrderNo", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstDonationCategory = _DonationCategory.GetDonationCategoriesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);

            }
            catch(Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">"+ ex.Message+ "</div>";
            }
            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstDonationCategory = lstDonationCategory;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddDonationCategory(Entities.DonationCategories objDonationCategory)
        {
            string str = "";
            bool _bool = true;
            try
            {

                Int64 _status = _DonationCategory.InsertDonationCategories(objDonationCategory);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Category details successfully.</div>";
                        return RedirectToAction("Index", "DonationCategories");
                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Category details successfully.</div>";
                        return RedirectToAction("Index", "DonationCategories");
                    case 3:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\"> '" + objDonationCategory.CategoryName + "' is already existed.</div>";
                        return RedirectToAction("Index", "DonationCategories");
                    case -1:
                    default:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                        return RedirectToAction("Index", "DonationCategories");
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                _bool = false;
            }

            return Json(new { ok = _bool, data = str });

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditDonationCategory(Int64 DonationCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.DonationCategories _objDonationCategory = _DonationCategory.GetDonationCategoryById(DonationCategoryId, ref _qstatus);

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

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewDonationCategory(Int64 DonationCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.DonationCategories _objDonationCategory = _DonationCategory.GetDonationCategoryById(DonationCategoryId, ref _qstatus);

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


     


        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteDonationCategory(Int64 DonationCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _DonationCategory.DeleteDonationCategory(DonationCategoryId);
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
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult DonationCategoryStatus(Int64 DonationCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _DonationCategory.UpdateDonationCategoryStatus(DonationCategoryId);
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
        public JsonResult DonationCategoryDisplayOrder(int DisplayOrder, Int64 DonationCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _DonationCategory.UpdateDonationCategoryDisplayOrder(DisplayOrder, DonationCategoryId);
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


        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult DonationCategoriesGetChapterId(Int64 ChapterId =0)
        {
            int Total = 0;
            string str = "";
            List<Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            try
            {
                lstDonationCategories = _DonationCategory.DonationCategoriesGetChapterId(ChapterId, ref Total);

                if (lstDonationCategories != null)
                {
                    return Json(new { ok = true, data = lstDonationCategories });
                }
                else
                {
                    str = "<div class=\"success closable\">Failed</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"error closable\">" + ex.Message + "</div>";
                return Json(new { ok = true, data = str });
            }
        }

        #endregion  

    }
}
