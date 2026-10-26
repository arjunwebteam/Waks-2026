using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
      //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Members,ChapterAdmin,SiteAdmin,Administrator,Treasurer,DeveloperAdmin,")]
    public class MembersOrdersController : Controller
    {
        BLL.Members _Members = new BLL.Members();
        List<Entities.Members> lstMembers = new List<Entities.Members>();
        Entities.Members objMembers = new Entities.Members();
        BLL.MembershipTypes _MembershipType = new BLL.MembershipTypes();
        List<Entities.MembershipTypes> lstMembershipType = new List<Entities.MembershipTypes>();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();
        BLL.Users _users = new BLL.Users();

        //[Authorize]
        public IActionResult Index(long MembershipTypeId = 0, long mid = 0)
        {
            int status = 0;

            // Get UserId from session
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long UserId))
            {
                // Redirect to login if user not found in session
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            // Get user roles
            var objuserroles = _users.GetRoleDetialsById(UserId, mid, ref status);

            // Store permissions in session
            HttpContext.Session.SetInt32("IsEdit", objuserroles.IsEdit ? 1 : 0);
            HttpContext.Session.SetInt32("IsView", objuserroles.IsView ? 1 : 0);
            HttpContext.Session.SetInt32("IsDelete", objuserroles.IsDelete ? 1 : 0);
            HttpContext.Session.SetInt32("IsExport", objuserroles.IsExport ? 1 : 0);
            HttpContext.Session.SetInt32("IsAdd", objuserroles.IsAdd ? 1 : 0);

            // Pass permissions to ViewBag
            ViewBag.IsEdit = objuserroles.IsEdit;
            ViewBag.IsView = objuserroles.IsView;
            ViewBag.IsDelete = objuserroles.IsDelete;
            ViewBag.IsExport = objuserroles.IsExport;
            ViewBag.IsAdd = objuserroles.IsAdd;
            ViewBag.mid = mid;

            return View();
        }

        public ActionResult MembersOrdersList(string Search = "", string SortColumn = "InsertedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {
                lstMembers = _Members.GetMembersOrderDetailsListByVariable(Search, Sort, PageNo, Items, ref Total);               
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstMembers = lstMembers;
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

        public ActionResult EditMemberOrder(Int64 MemberOrderId = 0, Int64 mid = 0)
        {
            try
            {
                int qstatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref qstatus);

                if (qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.status = qstatus;
                }
                int _qstatus = 0;
                Entities.MembershipOrders _objMemberOrder = _Members.GetMemberOrderById(MemberOrderId, ref _qstatus);

                if (_qstatus == 1 && qstatus == 1)
                {
                    ViewBag.objMemberOrder = _objMemberOrder;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "MembersOrders", new { mid = mid });
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "MembersOrders", new { mid = mid });
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateMemberOrder(Entities.MembershipOrders objMemberOrder, Int64 mid = 0)
        {
            try
            {
                objMemberOrder.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _Members.InsertMemberOrder(objMemberOrder);
                if (_status != -1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
                    return RedirectToAction("EditMemberOrder", "MembersOrders", new { MemberOrderId = objMemberOrder.MembershipOrderId, mid = mid });

                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction.</div>";
                    return RedirectToAction("EditMemberOrder", "MembersOrders", new { MemberOrderId = objMemberOrder.MembershipOrderId, mid = mid });
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("EditMemberOrder", "MembersOrders", new { MemberOrderId = objMemberOrder.MembershipOrderId, mid = mid });
            }

        }

        public ActionResult ViewMemberOrder(Int64 MemberOrderId = 0)
        {
            try
            {
                int _qstatus = 0;
                Entities.Members _objMembers = _Members.GetMemberFullDetailsById(MemberOrderId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objMemberDetails = _objMembers;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Members");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Members");
            }
            return View();
        }

        [HttpPost]
        public JsonResult DeleteMemberOrder(Int64 MemberOrderId)
        {
            string str = "";
            try
            {
                Int64 _status = _Members.DeleteMemberOrder(MemberOrderId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting order</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch 
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        public class AuthorizeAttribute : ActionFilterAttribute
        {
            BLL.Users _user = new BLL.Users();
            BLL.Roles _Roles = new BLL.Roles();
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                string userRole = null;
                int status = 0;

                var objUser = new AWS_AssociationToolCore.Entities.Users();
                var httpContext = context.HttpContext;

                // Get the authentication cookie
                if (httpContext.Request.Cookies.TryGetValue("UserCookie", out var userCookie))
                {
                    // Decrypt / parse cookie value
                    // Replace this with your logic: either JWT decode or your custom encrypted string
                    var authData = ParseAuthCookie(userCookie);
                    //if (authData != null)
                    //{
                    objUser = _user.GetAdminUsersGetByEmail(authData.Email, ref status);
                    userRole = authData.Roles;

                    if (objUser != null)
                    {
                        httpContext.Response.Cookies.Append("UserName", objUser.UserName);
                        httpContext.Response.Cookies.Append("UserId", objUser.UserId.ToString());
                        httpContext.Response.Cookies.Append("chapterid", objUser.ChapterId.ToString());
                        httpContext.Response.Cookies.Append("UserRole", userRole ?? string.Empty);
                    }
                    //}
                }

                if (string.IsNullOrEmpty(userRole) || objUser == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
                    return;
                }

                // Get allowed roles
                var allowedRoles = _Roles.GetRolesList(ref status)?
                    .SelectMany(r => r.RoleName.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(r => r.Trim())
                    .ToList() ?? new List<string>();

                // Check if user role is allowed
                var userRoles = userRole.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(r => r.Trim());

                if (!userRoles.Any(r => allowedRoles.Contains(r)))
                {
                    context.Result = new RedirectToActionResult("Unauthorized", "Account", new { area = "Admin" });
                    return;
                }

                base.OnActionExecuting(context);
            }

            // This is a helper to parse your cookie in Core.
            // Replace with your encryption/decryption or JWT parsing logic
            private (string Email, string Roles) ParseAuthCookie(string cookieValue)
            {
                try
                {
                    // Example: if your cookie is "email|roles"
                    var parts = cookieValue.Split('|');
                    if (parts.Length == 2)
                    {
                        return (parts[0], parts[1]);
                    }
                }
                catch
                {
                    // Log exception
                }
                return default;
            }
        }
    }
}
