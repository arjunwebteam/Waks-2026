using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,DeveloperAdmin,")]
    [Area("Admin")]

    public class PhotosImageSettingsController : Controller
    {
        BLL.PhotosImageSettings _PhotosImageSettings = new BLL.PhotosImageSettings();
        // BLL.Chapters _Chapters = new BLL.Chapters();

        public ActionResult Index()
        {
           
            //List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
          
            try
            {
                int status = 0;                
                Entities.PhotosImageSettings objPhotosImageSettings = _PhotosImageSettings.GetPhotosImageSettingsDetails(ref status);
                if (status != 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.objPhotosImageSettings = objPhotosImageSettings;
               
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            return View();
        }

        public ActionResult UpdatePhotosImageSettings(Entities.PhotosImageSettings objPhotosImageSettings)
        {
            try
            {
                objPhotosImageSettings.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objPhotosImageSettings.InsertedBy = HttpContext.User.Identity.Name.ToString();
                Int64 status = _PhotosImageSettings.UpdatePhotosImageSettingsDetails(objPhotosImageSettings);
                if (status != -1)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";

                }
                else
                {

                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    ;
                }
            }
            catch 
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
            }
            return RedirectToAction("Index", "PhotosImageSettings");
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
