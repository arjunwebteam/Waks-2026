using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{


    [Area("Admin")]
    public class MembershipTypesController : Controller
    {
        BLL.MembershipTypes _MembershipTypes = new BLL.MembershipTypes();
        List<Entities.MembershipTypes> lstMembershipTypes = new List<Entities.MembershipTypes>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();
        BLL.Users _users = new BLL.Users();

        #region MembershipTypes

        [Authorize]
        public IActionResult Index(long ChapterId = 0, bool IsFooterBar = false, bool IsMenuBar = false, bool IsQuickLinks = false, long mid = 0)
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
            ViewBag.IsFooterBar = IsFooterBar;
            ViewBag.IsMenuBar = IsMenuBar;
            ViewBag.IsQuickLinks = IsQuickLinks;

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }

        [Authorize]
        public ActionResult MembershipTypesList(Int64 ChapterId=0, string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstMembershipTypes = _MembershipTypes.GetMembershipTypesListByVariable(ChapterId,Search, Sort, PageNo, Items, ref Total);
                
            }
            catch 
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstMembershipTypes = lstMembershipTypes;
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
        public ActionResult AddMembershipTypes(Entities.MembershipTypes objMembershipTypes, Int64 mid = 0)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objMembershipTypes.UpdatedBy = this.User.Identity.Name;
                objMembershipTypes.IsActive = true;
                objMembershipTypes.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                Int64 _status = _MembershipTypes.InsertMembershipType(objMembershipTypes);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully.";
                }
                else if (_status == 2)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";


                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                }
                return RedirectToAction("Index", "MembershipTypes", new { mid = mid });

            }
              
            
            catch (Exception EX)
            {
                TempData["messageType"] = "error";
                TempData["message"] = EX.Message;
                return RedirectToAction("Index", "MembershipTypes", new { mid = mid });
            }

        }

        [Authorize]
        public ActionResult EditMembershipTypes(Int64 MembershipTypeId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.MembershipTypes _objMembershipTypes = _MembershipTypes.GetMembershipTypeById(MembershipTypeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objMembershipTypes });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Transation"
                    });
                }
            }
            catch 
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed Transation"
                });
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteMembershipTypes(Int64 MembershipTypeId)
        {
            string str = "";
            try
            {
                Int64 _status = _MembershipTypes.DeleteMembershipType(MembershipTypeId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Recored Deleted successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the Membership Type"
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
        public JsonResult UpdateDisplayOrder(int DisplayOrder, Int64 MembershipTypeId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _MembershipTypes.UpdateMembershipTypeDisplayOrder(DisplayOrder, MembershipTypeId);
            if (_status == 1)
            {
                return Json(new
                {
                    ok = true,
                    messageType = "success",
                    message = "Updated Order Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Order No updating user status"
                });
            }
            
        }

        [Authorize]
        public JsonResult UpdateMembershipTypeStatus(Int64 MembershipTypeId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _MembershipTypes.UpdateMembershipTypeStatus(MembershipTypeId);
            if (_status == 1)
            {
                return Json(new
                {
                    ok = true,
                    messageType = "success",
                    message = "Status Updated Successfully"
                });
                //str = "<div class=\"alert alert-success alert-dismissable\">Update Status Successfully</div>";
                //return Json(new { ok = true, data = str });
            }
            else
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed updating user status"
                });
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
