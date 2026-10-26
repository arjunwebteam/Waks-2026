using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,PaymentMethods,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class PaymentMethodsController : Controller
    {
        BLL.PaymentMethods _PaymentMethods = new BLL.PaymentMethods();

        #region PaymentMethods

        [Authorize]
        public ActionResult Index()
        {
            
          
          
             return View();
        }


        [Authorize]
        public ActionResult PaymentMethodsList(string Search = "", string SortColumn = "PaymentMethodId", string SortOrder = "Desc", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.PaymentMethods> lstPaymentMethods = new List<Entities.PaymentMethods>();

            try
            {
                lstPaymentMethods = _PaymentMethods.GetPaymentMethodsListByVariable(Search, Sort, PageNo, Items, ref Total);

            }
            catch 
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstPaymentMethods = lstPaymentMethods;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }















        [HttpPost]
        [Authorize]
        public JsonResult PaymentMethodstatus(Int64 PaymentMethodId)
        {
            string str = "";
            try
            {
                Int64 _status = _PaymentMethods.UpdatePaymentMethodstatus(PaymentMethodId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });

                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Failed updating status"
                    });
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
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

        #endregion

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
