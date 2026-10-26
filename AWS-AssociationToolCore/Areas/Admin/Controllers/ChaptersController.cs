//using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChaptersController : Controller
    {
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.InnerPageCategories _InnerPageCategory = new BLL.InnerPageCategories();
        List<Entities.InnerPageCategories> lstInnerPageCategory = new List<Entities.InnerPageCategories>();
        BLL.Users _users = new BLL.Users();
        #region Chapters

        [Authorize]
        public IActionResult Index(long ChapterId = 0, long mid = 0)
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
            int CategoryLevel = 0;

            try
            {
                //lstMenuItems = _MenuItems.GetMenuItemsByLevel(CategoryLevel, ref status);
                //lstPageDetails = _PageDetails.GetPageDetailsList(ref status);
                lstChapters = _Chapters.GetChaptersList(ref status);

                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);

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
            //ViewBag.IsFooterBar = IsFooterBar;
            //ViewBag.IsMenuBar = IsMenuBar;
            //ViewBag.IsQuickLinks = IsQuickLinks;

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }
        [Authorize]
        public ActionResult ChaptersList(string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 mid = 0, Int64 ChapterId = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstChapters = _Chapters.GetChaptersListByVariable(Search, Sort, PageNo, Items, ref Total, ChapterId);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstChapters = lstChapters;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult AddChapter(Int64 mid = 0)
        {
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            int status = 0; 
            try
            {
                lstChapters = _Chapters.GetChaptersList(ref status);

                Entities.Chapters _objChapters = _Chapters.GetChapters(ref status);
                ViewBag.objChapters = _objChapters;
            }
            catch
            {
                status = -1;
            }

            ViewBag.lstChapters = lstChapters;
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        [HttpPost]

        public ActionResult AddChapter(Entities.Chapters objChapters, Int64 mid = 0)
        {
            try
            {
                objChapters.UpdatedBy = HttpContext.User.Identity.Name.ToString();
               // objChapters.UpdatedDate = DateTime.UtcNow;
                objChapters.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                objChapters.IsActive = true;
                Int64 _status = _Chapters.InsertChapters(objChapters);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Inserted Record Successfully";
                    //return RedirectToAction("Index", "Chapters");
                    return RedirectToAction("Index", "Chapters", new { mid = mid });
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";
                    return RedirectToAction("Index", "Chapters", new { mid = mid });
                   // return RedirectToAction("Index", "Chapters");
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed uploading page.";
                   // return RedirectToAction("Index", "Chapters");
                    return RedirectToAction("Index", "Chapters", new { mid = mid });
                }
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                //return RedirectToAction("Index", "Chapters");
                return RedirectToAction("Index", "Chapters", new { mid = mid });
            }

        }

        [Authorize]
        public ActionResult EditChapter(Int64 ChapterId = 0, Int64 mid = 0)
        {
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            try
            { 
                int _qstatus = 0;
                Int32 status = 0;
                Entities.Chapters _objChapters = _Chapters.GetChaptersById(ChapterId, ref _qstatus);
                lstChapters = _Chapters.GetChaptersList(ref status);

                if (_qstatus == 1)
                {
                    ViewBag.objChapters = _objChapters;
                    ViewBag.lstChapters = lstChapters;
                    ViewBag.status = _qstatus;
                    ViewBag.mid = mid;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction.";
                   // return RedirectToAction("Index", "Chapters");
                    return RedirectToAction("Index", "Chapters", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed transaction.";
                //return RedirectToAction("Index", "Chapters");
                return RedirectToAction("Index", "Chapters", new {  mid = mid });
            }
            return View();
        }

        [Authorize]
        public ActionResult ViewChapter(Int64 ChapterId = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.Chapters _objChapters = _Chapters.GetChaptersById(ChapterId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objChapters = _objChapters;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Chapters");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Chapters");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteChapter(Int64 ChapterId)
        {
            try
            {

                Int64 _status = _Chapters.DeleteChapter(ChapterId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the Record"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ChapterDisplayOrder(int DisplayOrder, Int64 ChapterId)
        {
            string str = "";
            try
            {
                Int64 _status = _Chapters.UpdateChaptersDisplayOrder(DisplayOrder, ChapterId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Order Successfully"
                    });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Order updating user status"
                    });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public JsonResult Chapterstatus(Int64 ChapterId)
        {
           
            try
            {
                Int64 _status = _Chapters.UpdateChaptersStatus(ChapterId);
                if (_status == 1)
                {
                    
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });
                }
                else
                {
                    
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating status"
                    });
                }
            }
            catch (Exception ex)
            {
                
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult CheckChapterNameAvailability(string cname)
        {
            int status = 0;
            try
            {
                Entities.Chapters objChapters = _Chapters.GetChaptersListByName(cname, ref status);
                bool data = (objChapters.ChapterId == 0 ? true : false);
                return Json(new { ok = true, data = data });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error closable\">Failed transaction.</div>" });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult CheckExistChapterNameAvailability(Int64 ChapterId, string cname)
        {
            int status = 0;
            try
            {
                Entities.Chapters objChapters = _Chapters.GetChaptersListByName(cname, ref status);
                bool data = (objChapters.ChapterId == ChapterId || objChapters.ChapterId == 0 ? true : false);
                return Json(new { ok = true, data = data });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error closable\">Failed transaction.</div>" });
            }
        }

        #endregion


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
