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
    public class RedirectionsController : Controller
    {
        BLL.Redirections _Redirections = new BLL.Redirections();
        List<Entities.Redirections> lstRedirections = new List<Entities.Redirections>();
        Entities.Redirections objRedirections = new Entities.Redirections();
        AWS_AssociationToolCore.DAL.Redirections _DRedirections = new AWS_AssociationToolCore.DAL.Redirections();
        #region Redirections

        // [AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        // [AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        public ActionResult RedirectionsList(string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstRedirections = _Redirections.GetRedirectionsListByVariable(Search, Sort, PageNo, Items,  ref Total);
            }
            catch
            {
                Total = -1;
            }

            ViewBag.total = Total;
            ViewBag.PageNo = PageNo;
            ViewBag.items = Items;
            ViewBag.lstRedirections = lstRedirections;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }
        [Authorize]
        public ActionResult AddRedirections()
        {
            return View();
        }

        // [AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [Authorize]
        [HttpPost]
        public IActionResult AddRedirections(Entities.Redirections objRedirections)
        {
            try
            {
                // Get logged-in username from session
                string currentUser = HttpContext.Session.GetString("name") ?? "";

                objRedirections.InsertedBy = currentUser;
                objRedirections.UpdatedBy = currentUser;
                objRedirections.IsActive = true;

                long status = _Redirections.InsertRedirections(objRedirections);

                if (status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                    return RedirectToAction("Index");
                }
                else if (status == 2)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes have been Updated Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("AddRedirections");
                }
            }
            catch (Exception)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed transaction.";
                return RedirectToAction("AddRedirections");
            }
        }
        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [Authorize]
        public ActionResult EditRedirections(Int64 RedirectionId = 0)
        {
            string str = "";
            try
            {
                int status = 0;
                objRedirections = _Redirections.GetRedirectionsById(RedirectionId, ref status);
                if (status != -1 && objRedirections != null)
                {
                    ViewBag.objRedirections = objRedirections;
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed processing your request.</div>";
                    ViewBag.objRedirections = "";
                }

            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
            }
            return View();
        }

        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [Authorize]
        public ActionResult ViewRedirections(Int64 RedirectionId = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.Redirections _objRedirections = _Redirections.GetRedirectionsById(RedirectionId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objRedirections = _objRedirections;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction.";
                    return RedirectToAction("Index", "Redirections");
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed transaction.";
                return RedirectToAction("Index", "Redirections");
            }
            return View();
        }

        // [AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        [Authorize]
        public JsonResult DeleteRedirections(Int64 RedirectionId)
        {
            string str = "";
            try
            {

                Int64 _status = _Redirections.DeleteRedirections(RedirectionId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed deleting Redirections" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction" });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateRedirectionsStatus(Int64 RedirectionId)
        {
            try
            {
                Int64 _status = _Redirections.UpdateRedirectionsStatus(RedirectionId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Status Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "success", message = "Failed updating status" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "success", message = "+ ex.Message +" });
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
