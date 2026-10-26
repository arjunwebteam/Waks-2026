using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class PromoCodesController : Controller
    {
        BLL.PromoCodes _PromoCodes = new BLL.PromoCodes();
        List<Entities.PromoCodes> lstPromoCodes = new List<Entities.PromoCodes>();
        BLL.Users _users = new BLL.Users();

        #region PromoCodes

        [Authorize]
        public ActionResult Index(Int64 mid = 0)
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

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            return View();

        }



        [Authorize]
        public ActionResult PromoCodesList(string StartDate = "", string EndDate = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstPromoCodes = _PromoCodes.GetPromoCodesList(StartDate, EndDate, Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                Total = -1;
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstPromoCodes = lstPromoCodes;
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
        public ActionResult AddPromoCodes(Int64 mid = 0)
        {
            ViewBag.mid = mid;
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddPromoCodes(Entities.PromoCodes objPromoCodes, Int64 mid = 0)
        {
            try
            {
                objPromoCodes.UpdatedBy = "Admin";
                objPromoCodes.InsertedBy = "Admin";
                //objPromoCodes.InsertedDate = DateTime.UtcNow;
                // objPromoCodes.UpdatedDate = DateTime.UtcNow;
                objPromoCodes.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objPromoCodes.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                objPromoCodes.IsActive = true;
                Int64 _status = _PromoCodes.InsertPromoCodes(objPromoCodes);
                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = "Record Inserted Successfully";

                        return RedirectToAction("Index", "PromoCodes", new { mid = mid });
                    case 2:
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = "Updated PromoCodes successfully";

                        return RedirectToAction("Index", "PromoCodes", new { mid = mid });

                    case -1:
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Data Error";
                        return RedirectToAction("Index", "PromoCodes", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                return RedirectToAction("Index", "PromoCodes", new { mid = mid });
            }

        }

        [Authorize]
        public ActionResult EditPromoCodes(Int64 PromoCodeId = 0, Int64 mid = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.PromoCodes _objPromoCodes = _PromoCodes.GetPromoCodesById(PromoCodeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objPromoCodes = _objPromoCodes;

                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "PromoCodes", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                return RedirectToAction("Index", "PromoCodes", new { mid = mid });
            }
            ViewBag.mid = mid;
            return View();
        }

        public ActionResult ViewPromoCodes(Int64 PromoCodeId = 0, Int64 mid = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.PromoCodes _objPromoCodes = _PromoCodes.GetPromoCodesById(PromoCodeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objPromoCodes = _objPromoCodes;

                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "PromoCodes", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                return RedirectToAction("Index", "PromoCodes", new { mid = mid });
            }
            return View();
        }


        [Authorize]
        [HttpPost]
        public JsonResult DeletePromoCodes(Int64 PromoCodeId)
        {

            try
            {

                Int64 _status = _PromoCodes.DeletePromoCodes(PromoCodeId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the PromoCode"
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



        [HttpPost]
        public JsonResult PromoCodesStatus(Int64 PromoCodeId)
        {
            string str = "";
            try
            {
                Int64 _status = _PromoCodes.UpdatePromoCodesStatus(PromoCodeId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Status Updated Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",

                    });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
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
