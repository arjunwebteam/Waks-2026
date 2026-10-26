//using System.Web.Mvc;
//using System.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    [Area("Admin")]

    public class VolunteerCategoriesController : Controller
    {
        BLL.VolunteerCategories _VolunteerCategory = new BLL.VolunteerCategories();
        List<Entities.VolunteerCategories> lstVolunteerCategory = new List<Entities.VolunteerCategories>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();
        BLL.Users _users = new BLL.Users();

        #region VolunteerCategories

        [Authorize]
        public ActionResult Index(Int64 ChapterId = 0, Int64 mid = 0)
        {
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



            List<Entities.Chapters> lstChapters = new();
            List<Entities.PageDetails> lstPageDetails = new();
            List<Entities.MenuItems> lstMenuItems = new();
            Entities.UserRoles objuserroles = new();

            int status = 0;
            int _qstatus = 0;
            int CategoryLevel = 0;

            try
            {

                lstChapters = _Chapters.GetChaptersList(ref status);
             


                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
                if (status != 1)
                {
                    TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                }

                // ✅ Session (Core way)
                HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
                HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
                HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
                HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
                HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

                // ✅ ViewBag (optional)
                ViewBag.IsEdit = objuserroles.IsEdit;
                ViewBag.IsView = objuserroles.IsView;
                ViewBag.IsDelete = objuserroles.IsDelete;
                ViewBag.IsExport = objuserroles.IsExport;
                ViewBag.IsAdd = objuserroles.IsAdd;
                ViewBag.mid = mid;



            }
            catch
            {
                status = -1;
            }

            // ✅ ViewBag assignments
            ViewBag.ChapterId = chapterId;


            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }

        [Authorize]
        public ActionResult VolunteerCategoriesList(Int64 ChapterId = 0, string Search = "", string SortColumn = "OrderNo", string SortOrder = "DESC", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstVolunteerCategory = _VolunteerCategory.GetVolunteerCategoriesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstVolunteerCategory = lstVolunteerCategory;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddVolunteerCategory(Entities.VolunteerCategories objVolunteerCategory, Int64 mid = 0)
        {
            string str = "";
            bool _bool = true;
            try
            {

                Int64 _status = _VolunteerCategory.InsertVolunteerCategories(objVolunteerCategory);
                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Created Successfully.";
                        return RedirectToAction("Index", "VolunteerCategories", new { mid = mid });
                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes has been Updated Successfully.";
                        return RedirectToAction("Index", "VolunteerCategories", new { mid = mid });
                    case 3:
                        TempData["messageType"] = "success";
                        TempData["message"] = "'" + objVolunteerCategory.CategoryName + "' is already existed.";
                        return RedirectToAction("Index", "VolunteerCategories", new { mid = mid });
                    case -1:
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        return RedirectToAction("Index", "VolunteerCategories", new { mid = mid });
                }
            }
            catch
            {
                _bool = false;
            }

            return Json(new { ok = _bool, messageType = "error", message = "Failed transaction." });

        }

        [Authorize]
        public ActionResult EditVolunteerCategory(Int64 VolunteerCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.VolunteerCategories _objVolunteerCategory = _VolunteerCategory.GetVolunteerCategoryById(VolunteerCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objVolunteerCategory });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        public ActionResult ViewVolunteerCategory(Int64 VolunteerCategoryId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.VolunteerCategories _objVolunteerCategory = _VolunteerCategory.GetVolunteerCategoryById(VolunteerCategoryId, ref _qstatus);
                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objVolunteerCategory });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteVolunteerCategory(Int64 VolunteerCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _VolunteerCategory.DeleteVolunteerCategory(VolunteerCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Deleting the  Category" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult VolunteerCategoryStatus(Int64 VolunteerCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _VolunteerCategory.UpdateVolunteerCategoryStatus(VolunteerCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Status Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed transaction" });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed </div>";
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult VolunteerCategoryDisplayOrder(int OrderNo, Int64 VolunteerCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _VolunteerCategory.UpdateVolunteerCategoryDisplayOrder(OrderNo, VolunteerCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating status" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        #endregion


        [Authorize]
        public ActionResult VolunteerCategoriesGetChapterId(Int64 ChapterId = 0)
        {
            int Total = 0;
            string str = "";
            List<Entities.VolunteerCategories> lstVolunteerCategories = new List<Entities.VolunteerCategories>();
            try
            {
                lstVolunteerCategories = _VolunteerCategory.VolunteerCategoriesGetChapterId(ChapterId, ref Total);

                if (lstVolunteerCategories != null)
                {
                    return Json(new { ok = true, data = lstVolunteerCategories });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = true, messageType = "error", message = ex.Message });
            }
        }

        public class AuthorizeAttribute : ActionFilterAttribute
        {
            BLL.Users _user = new BLL.Users();
            BLL.Roles _Roles = new BLL.Roles();
            private object FormsAuthentication;
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string userRole = null;
                int status = 0;

                // ✅ Get ClaimsPrincipal — replaces HttpCookie + FormsAuthentication.Decrypt
                var user = filterContext.HttpContext.User;

                if (user?.Identity != null && user.Identity.IsAuthenticated)
                {
                    // ✅ Read role from Claims — replaces authTicket.UserData
                    userRole = user.FindFirst(ClaimTypes.Role)?.Value;

                    // ✅ Read email from Claims — replaces authTicket.Name
                    string emailFromClaim = user.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(emailFromClaim))
                    {
                        // Get user from DB
                        var objuser = _user.GetAdminUsersGetByEmail(
                            emailFromClaim, ref status);

                        // Inside AuthorizeAttribute, where objuser != null:
                        if (objuser != null)
                        {
                            filterContext.HttpContext.Session.SetString("UserName", objuser.UserName ?? "");
                            filterContext.HttpContext.Session.SetString("UserId", objuser.UserId.ToString());
                            filterContext.HttpContext.Session.SetString("UserEmail", emailFromClaim ?? ""); // ✅ ADD THIS
                            filterContext.HttpContext.Session.SetString("chapterid", objuser.ChapterId.ToString());
                            filterContext.HttpContext.Session.SetString("userrole", userRole ?? "");
                        }
                    }
                }

                // ✅ If no role — redirect to LogOn
                //    replaces new UrlHelper(filterContext.RequestContext)
                if (string.IsNullOrEmpty(userRole))
                {
                    filterContext.Result = new RedirectToActionResult(
                        "LogOn", "Account", new { area = "Admin" });
                    return;
                }

                // ✅ Get all allowed roles from DB
                int roleStatus = 0;
                List<Entities.Roles> lstRoles = _Roles.GetRolesList(ref roleStatus);

                // Build flat list of allowed role names
                List<string> allowedRoles = lstRoles
                    .Select(r => r.RoleName.Trim())
                    .ToList();

                // Split user's roles (comma separated) and check
                var userRoles = userRole
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList();

                bool isAuthorized = userRoles.Any(r => allowedRoles.Contains(r));

                if (!isAuthorized)
                {
                    // ✅ Redirect to Unauthorized — replaces UrlHelper + RedirectResult
                    filterContext.Result = new RedirectToActionResult(
                        "Unauthorized", "Account", new { area = "Admin" });
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}
