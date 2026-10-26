using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
//using System.Web.Security;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,PageDetails,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class MenuPagesController : Controller
    {
        BLL.MenuPages _MenuPages = new BLL.MenuPages();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        List<Entities.PageDetails> lstPageDetails = new List<Entities.PageDetails>();
        BLL.MenuItems _MenuItems = new BLL.MenuItems();
        List<Entities.MenuItems> lstMenuItems = new List<Entities.MenuItems>();

        #region MenuPages Members

        [Authorize]

        public IActionResult Index(long MenuItemId = 0)
        {
            long UserId = 0;
            long ChapterId = 0;
            bool IsFooterBar = false;
            bool IsMenuBar = false;
            bool IsQuickLinks = false;

            // Get cookies
            if (Request.Cookies.TryGetValue("UserId", out string userIdValue))
                UserId = Convert.ToInt64(userIdValue);

            if (Request.Cookies.TryGetValue("UserRole", out string userRole))
                HttpContext.Session.SetString("userrole", userRole);

            if (Request.Cookies.TryGetValue("chapterid", out string chapterIdValue))
            {
                HttpContext.Session.SetString("chapterid", chapterIdValue);
                ChapterId = Convert.ToInt64(chapterIdValue);
            }

            // Get session flags
            IsFooterBar = HttpContext.Session.GetString("IsFooterBar") == "true";
            IsMenuBar = HttpContext.Session.GetString("IsMenuBar") == "true";
            IsQuickLinks = HttpContext.Session.GetString("IsQuickLinks") == "true";

            List<Entities.MenuPages> lstMenuPages = new List<Entities.MenuPages>();
            List<Entities.MenuItems> lstMenuItems = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems2 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems3 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems4 = new List<Entities.MenuItems>();

            try
            {
                if (MenuItemId != 0)
                {
                    int queryStatus = 0;
                    lstMenuPages = _PageDetails.GetMenuPagesListById(MenuItemId, ref queryStatus);
                    lstMenuItems = _MenuItems.GetMenuItemsAll(ref lstMenuItems2, ref lstMenuItems3, ref lstMenuItems4,
                                                              ChapterId, IsFooterBar, IsMenuBar, IsQuickLinks, ref queryStatus);

                    if (queryStatus != 1)
                    {
                        TempData["message"] = "Failed transaction.";
                        return RedirectToAction("Index", "PageDetails");
                    }
                }
            }
            catch
            {
                TempData["message"] = "Failed transaction.";
            }

            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.lstMenuPages = lstMenuPages;
            ViewBag.lstMenuItems2 = lstMenuItems2;
            ViewBag.lstMenuItems3 = lstMenuItems3;
            ViewBag.lstMenuItems4 = lstMenuItems4;
            ViewBag.MenuItemId = MenuItemId;

            return View();
        }

        [Authorize]

        public ActionResult MenuPagesList(Int64 MenuItemId = 0, string Search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 20)
        {
           
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstPageDetails = _MenuPages.MenuPagesList(MenuItemId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [HttpPost]
        [Authorize]

        public ActionResult AddMenuPages(Entities.MenuPages objMenuPages, Int64 ChapterId = 0, Int64 Mid = 0)
        {
            try
            {
               
                Int64 _status = _MenuPages.InsertMenuPages(objMenuPages);
                if (_status == 1)
                {
                    
                    TempData["messageType"] = "success";
                    TempData["message"] = " Inserted Data successfully";
                    return RedirectToAction("Index", "MenuItems", new { ChapterId = ChapterId, mid = Mid });
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "warning"; // Assuming "info" for update
                    TempData["message"] = " Updated data successfully ";
                    return RedirectToAction("Index", "MenuItems", new { ChapterId = ChapterId, mid = Mid });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "MenuItems", new { ChapterId = ChapterId, mid = Mid });
                }
            }
            catch 
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                return RedirectToAction("Index", "MenuItems", new { ChapterId = ChapterId, mid = Mid });
            }

        }
        [Authorize]

        public ActionResult EditMenuPages(Int64 MenuPagesId = 0)
        {
            string str = "";
            try
            {
               
                int _qstatus = 0;
                Entities.MenuPages _objMenuPages = _MenuPages.GetMenuPagesById(MenuPagesId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objMenuPages });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
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
        [Authorize]

        public JsonResult DeleteMenuPages(Int64 MenuPagesId)
        {
         
            try
            {
               
                Int64 _status = _MenuPages.DeleteMenuPages(MenuPagesId);
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







        #endregion       

    }
}
