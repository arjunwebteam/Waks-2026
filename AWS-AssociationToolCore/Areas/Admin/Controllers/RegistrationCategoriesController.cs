using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Administrator,DeveloperAdmin,")]
    public class RegistrationCategoriesController : Controller
    {
        BLL.RegistrationCategories _lstRegistrationCategories = new BLL.RegistrationCategories();
        List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();

        #region RegistrationCategories

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult RegistrationCategoriesList(string Search = "", string Location = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstRegistrationCategories = _lstRegistrationCategories.GetRegistrationCategoriesListByVariable(Search, Location, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstRegistrationCategories = lstRegistrationCategories;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddRegistrationCategories(Entities.RegistrationCategories objRegistrationCategories)
        {
            try
            {
                // Get logged-in username from session
                string currentUser = HttpContext.Session.GetString("name") ?? "";

        
                objRegistrationCategories.UpdatedBy = currentUser; objRegistrationCategories.IsActive = true;
                Int64 _status = _lstRegistrationCategories.InsertRegistrationCategories(objRegistrationCategories);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted RegistrationCategoriesdetails successfully.</div>";
                        return RedirectToAction("Index", "RegistrationCategories");
                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated RegistrationCategoriesdetails successfully.</div>";
                        return RedirectToAction("Index", "RegistrationCategories");
                    case 3:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\"> '" + objRegistrationCategories.CategoryName + "' is already existed.</div>";
                        return RedirectToAction("Index", "RegistrationCategories");
                    case -1:
                    default:
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                        return RedirectToAction("Index", "RegistrationCategories");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return RedirectToAction("Index", "RegistrationCategories");
            }

        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult CreateRegistrationCategories()
        {
            try
            {
                int status = 0;
                List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();
                if (status == 1)
                {
                    ViewBag.lstRegistrationCategories = lstRegistrationCategories;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult CreateRegistrationCategories(Entities.RegistrationCategories objRegistrationCategories)
        {
            try
            {
                Int64 _status = 0;
                objRegistrationCategories.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objRegistrationCategories.InstertedBy = HttpContext.User.Identity.Name.ToString();
                objRegistrationCategories.IsActive = true;
                _status = _lstRegistrationCategories.InsertRegistrationCategories(objRegistrationCategories);
                if (_status == 1 || _status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" + (_status == 2 ? "Updated " : "Inserted ") + " Registration Categories details successfully.</div>";
                    if (_status == 1)
                    {
                        return RedirectToAction("Index", "RegistrationCategories");
                    }
                    else
                    {
                        return RedirectToAction("EditRegistrationCategories", "RegistrationCategories", new { RegistrationCategoryId = objRegistrationCategories.RegistrationCategoryId });
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed " + (_status == 2 ? "updating " : "inserting ") + " Registration Categories details.</div>";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return View();
            }
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult EditRegistrationCategories(Int64 RegistrationCategoryId)
        {
            try
            {
                int _status = 0;
                int _list = 0;
                Entities.RegistrationCategories objRegistrationCategories = _lstRegistrationCategories.GetRegistrationCategoriesById(RegistrationCategoryId, ref _status);

                if (_status == 1)
                {
                    ViewBag.objRegistrationCategories = objRegistrationCategories;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "AdCategoriesMaster");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "RegistrationCategories");
            }
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        //public ActionResult EditRegistrationCategories(Entities.RegistrationCategories objRegistrationCategories)
        //{
        //    try
        //    {
        //        Int64 _status = 0;
        //        objRegistrationCategories.IsActive = Convert.ToBoolean(ConfigurationManager.AppSettings["masterstatus"]);

        //        _status = _lstRegistrationCategories.InsertRegistrationCategories(objRegistrationCategories);
        //        if (_status == 1 || _status == 2)
        //        {
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" + (_status == 2 ? "Updated " : "Inserted ") + " Registration Categories details successfully.</div>";
        //            return RedirectToAction("Index", "RegistrationCategories");
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed " + (_status == 2 ? "updating " : "inserting ") + " Registration Categories  details.</div>";
        //            return View();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
        //        return View();
        //    }
        //}

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewRegistrationCategories(Int64 RegistrationCategoryId)
        {
            try
            {
                int _status = 0;
                Entities.RegistrationCategories objRegistrationCategories = _lstRegistrationCategories.GetRegistrationCategoriesById(RegistrationCategoryId, ref _status);
                if (_status == 1)
                {
                    ViewBag.objRegistrationCategories = objRegistrationCategories;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "RegistrationCategories");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "RegistrationCategories");
            }
        }


        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteRegistrationCategories(Int64 RegistrationCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _lstRegistrationCategories.DeleteRegistrationCategories(RegistrationCategoryId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Deleted RegistrationCategoriessuccessfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        public JsonResult RegistrationCategoriesStatus(Int64 RegistrationCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _lstRegistrationCategories.UpdateRegistrationCategoriesStatusById(RegistrationCategoryId);
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


        [HttpPost]
        public JsonResult UpdateRegistrationCategoriesOrderNo(int DisplayOrder, Int64 RegistrationCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _lstRegistrationCategories.UpdateRegistrationCategoriesOrderNo(DisplayOrder, RegistrationCategoryId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Order No successfully</div>";
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
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return Json(new { ok = false, data = str });
            }
        }

        #endregion

    }
}
