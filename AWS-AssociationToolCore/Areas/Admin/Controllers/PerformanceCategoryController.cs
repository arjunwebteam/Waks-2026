using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ThemePerformanceCategorys,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class PerformanceCategoryController : Controller
    {
        AWS_AssociationToolCore.BLL.PerformanceCategory _PerformanceCategory = new AWS_AssociationToolCore.BLL.PerformanceCategory();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.Users _users = new BLL.Users();

        [Authorize]
        public ActionResult Index(long ChapterId = 0, long mid = 0)
        {
            int status = 0;
            int Total = 0;
            Entities.UserRoles objuserroles = new Entities.UserRoles();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();

            // ✅ Get UserId from Claims — replaces Request.Cookies["UserId"]
            Int64 UserId = Convert.ToInt64(
                User.FindFirst("UserId")?.Value ?? "0");

            // ✅ Get UserRole from Claims — replaces Request.Cookies["UserRole"]
            string userRole = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value ?? "";

            // ✅ Get ChapterId from Claims — replaces Request.Cookies["chapterid"]
            if (ChapterId == 0)
                ChapterId = Convert.ToInt64(
                    User.FindFirst("ChapterId")?.Value ?? "0");

            // ✅ Set Session — replaces Session["key"] = value
            HttpContext.Session.SetString("userrole", userRole);
            HttpContext.Session.SetString("chapterid", ChapterId.ToString());

            // Get data
            lstChapters = _Chapters.GetChaptersList(ref Total);
            objuserroles = _users.GetRoleDetialsById(UserId, mid, ref status);

            // ✅ Store permissions in Session
            HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

            // ✅ Pass directly to ViewBag
            ViewBag.IsEdit = objuserroles.IsEdit;
            ViewBag.IsView = objuserroles.IsView;
            ViewBag.IsDelete = objuserroles.IsDelete;
            ViewBag.IsExport = objuserroles.IsExport;
            ViewBag.IsAdd = objuserroles.IsAdd;

            ViewBag.lstChapters = lstChapters;
            ViewBag.ChapterId = ChapterId;
            ViewBag.mid = mid;

            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult PerformanceCategorysList(string search = "", int PageNo = 1, int PageItems = 25, string SortColumn = "", string SortOrder = "", Int64 Mid = 0)
        {
            int Total = 0;
            List<Entities.PerformanceCategory> PerformanceCategorylist = new List<Entities.PerformanceCategory>();
            try
            {
                string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");

                PerformanceCategorylist = _PerformanceCategory.GetPerformanceCategoryListByVariable(search, Sort, PageNo, PageItems, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.items = PageItems;
            ViewBag.pageno = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.PerformanceCategorylist = PerformanceCategorylist;
            ViewBag.Mid = Mid;

            return View();
        }

        [HttpPost]
        public ActionResult AddPerformanceCategory(Entities.PerformanceCategory objPerformanceCategory)
        {
            try
            {
                objPerformanceCategory.UpdatedBy = this.User.Identity.Name;
                objPerformanceCategory.InsertedBy = this.User.Identity.Name;
                Int64 _status = _PerformanceCategory.InsertPerformanceCategory(objPerformanceCategory);
                   
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                }
                else if (_status == 2)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";
                }
                return RedirectToAction("Index", "PerformanceCategory");
            }
            catch (Exception EX)
            {
                TempData["messageType"] = "error";
                TempData["message"] = EX.Message;
                return RedirectToAction("Index", "PerformanceCategory");
            }
        }

        [HttpPost]
        public ActionResult EditPerformanceCategory(Int64 PerformanceCategoryId)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.PerformanceCategory objPerformanceCategory = _PerformanceCategory.GetPerformanceCategoryById(PerformanceCategoryId, ref status);
                if (objPerformanceCategory != null)
                {
                    return Json(new { ok = true, data = objPerformanceCategory });
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

        [HttpPost]
        public JsonResult DeletePerformanceCategory(Int64 PerformanceCategoryId)
        {
            try
            {
                Int64 _status = _PerformanceCategory.DeletePerformanceCategory(PerformanceCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Deleting the Record" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [HttpPost]
        public JsonResult PerformanceCategoryStatus(Int64 PerformanceCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _PerformanceCategory.UpdatePerformanceCategoryStatus(PerformanceCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Status Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating status" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PerformanceCategoryOrderNo(int OrderNo, Int64 PerformanceCategoryId)
        {
            try
            {
                Int64 _status = _PerformanceCategory.UpdatePerformanceCategoryDisplayOrder(OrderNo, PerformanceCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Updating Status" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });
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
