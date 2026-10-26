using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppApplicationSettingsController : Controller
    {
        BLL.AppInfo _appinfo = new BLL.AppInfo();
        BLL.Chapters _Chapters = new BLL.Chapters();

        [Authorize]
        public ActionResult Index()
        {
            //int Total = 0;
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            //lstChapters = _Chapters.GetChaptersList(ref Total);
            try
            {
                int status = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);
                if (status != 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.objAppInfo = objAppInfo;
                //  ViewBag.lstChapters = lstChapters;
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
            }
            return View();
        }

        [Authorize]
        [HttpPost]
    
        public ActionResult UpdateAppInfos(Entities.AppInfo objAppInfo, IFormFile LayoutLogo, IFormFile faviconlogo, IFormFile Loginlogo)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfoPath = _appinfoHelper.GetAppInfoDetails(ref status1);
                string uploadPath = objAppInfoPath.UploadPath;

                objAppInfo.UpdatedBy = HttpContext.User.Identity.Name;
                objAppInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                string imageurl = "NA";
                string imageurl1 = "NA";
                string imageurl2 = "NA";
                string layoutFile = "";
                string faviconFile = "";
                string loginFile = "";

                // ✅ Save LayoutLogo
                if (LayoutLogo != null && LayoutLogo.Length > 0)
                {
                    layoutFile = Path.GetFileName(LayoutLogo.FileName);
                    string layoutNormalDir = Path.Combine(uploadPath, "LayoutLogo", "NormalImages");
                    string layoutThumbDir = Path.Combine(uploadPath, "LayoutLogo", "ThumbImages");
                    Directory.CreateDirectory(layoutNormalDir);
                    Directory.CreateDirectory(layoutThumbDir);

                    string layoutTempPath = Path.Combine(layoutNormalDir, layoutFile);
                    using (var stream = new FileStream(layoutTempPath, FileMode.Create))
                    {
                        LayoutLogo.CopyTo(stream);
                    }
                    imageurl = layoutFile;
                }

                // ✅ Save faviconlogo
                if (faviconlogo != null && faviconlogo.Length > 0)
                {
                    faviconFile = Path.GetFileName(faviconlogo.FileName);
                    string faviconNormalDir = Path.Combine(uploadPath, "faviconlogo", "NormalImages");
                    string faviconThumbDir = Path.Combine(uploadPath, "faviconlogo", "ThumbImages");
                    Directory.CreateDirectory(faviconNormalDir);
                    Directory.CreateDirectory(faviconThumbDir);

                    string faviconTempPath = Path.Combine(faviconNormalDir, faviconFile);
                    using (var stream = new FileStream(faviconTempPath, FileMode.Create))
                    {
                        faviconlogo.CopyTo(stream);
                    }
                    imageurl1 = faviconFile;
                }

                // ✅ Save Loginlogo
                if (Loginlogo != null && Loginlogo.Length > 0)
                {
                    loginFile = Path.GetFileName(Loginlogo.FileName);
                    string loginNormalDir = Path.Combine(uploadPath, "Loginlogo", "NormalImages");
                    string loginThumbDir = Path.Combine(uploadPath, "Loginlogo", "ThumbImages");
                    Directory.CreateDirectory(loginNormalDir);
                    Directory.CreateDirectory(loginThumbDir);

                    string loginTempPath = Path.Combine(loginNormalDir, loginFile);
                    using (var stream = new FileStream(loginTempPath, FileMode.Create))
                    {
                        Loginlogo.CopyTo(stream);
                    }
                    imageurl2 = loginFile;
                }

                // ✅ Update DB — imageurl values may change after this call
                Int64 status = _appinfo.UpdateAppInfoDetails(objAppInfo, ref imageurl, ref imageurl1, ref imageurl2);

                // ✅ Copy LayoutLogo to final path after DB update
                if (!string.IsNullOrEmpty(layoutFile) && imageurl != "NA" && imageurl != "")
                {
                    string tempPath = Path.Combine(uploadPath, "LayoutLogo", "NormalImages", layoutFile);
                    string normalPath = Path.Combine(uploadPath, "LayoutLogo", "NormalImages", imageurl);
                    string thumbPath = Path.Combine(uploadPath, "LayoutLogo", "ThumbImages", imageurl);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                // ✅ Copy faviconlogo to final path after DB update
                if (!string.IsNullOrEmpty(faviconFile) && imageurl1 != "NA" && imageurl1 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "faviconlogo", "NormalImages", faviconFile);
                    string normalPath = Path.Combine(uploadPath, "faviconlogo", "NormalImages", imageurl1);
                    string thumbPath = Path.Combine(uploadPath, "faviconlogo", "ThumbImages", imageurl1);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                // ✅ Copy Loginlogo to final path after DB update
                if (!string.IsNullOrEmpty(loginFile) && imageurl2 != "NA" && imageurl2 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "Loginlogo", "NormalImages", loginFile);
                    string normalPath = Path.Combine(uploadPath, "Loginlogo", "NormalImages", imageurl2);
                    string thumbPath = Path.Combine(uploadPath, "Loginlogo", "ThumbImages", imageurl2);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                if (status != -1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
            }

            return RedirectToAction("Index", "AppApplicationSettings");
        }

        [Authorize]
        [HttpPost]
        [Authorize]
        public ActionResult UpdateAppInfo(Entities.AppInfo objAppInfo, IFormFile LayoutLogo, IFormFile faviconlogo, IFormFile Loginlogo)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfoPath = _appinfoHelper.GetAppInfoDetails(ref status1);
                string uploadPath = objAppInfoPath.UploadPath;

                objAppInfo.UpdatedBy = HttpContext.User.Identity.Name;
                objAppInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                string imageurl = "NA";
                string imageurl1 = "NA";
                string imageurl2 = "NA";
                string layoutFile = "";
                string faviconFile = "";
                string loginFile = "";

                // ✅ Save LayoutLogo
                if (LayoutLogo != null && LayoutLogo.Length > 0)
                {
                    layoutFile = Path.GetFileName(LayoutLogo.FileName);
                    string layoutNormalDir = Path.Combine(uploadPath, "LayoutLogo", "NormalImages");
                    string layoutThumbDir = Path.Combine(uploadPath, "LayoutLogo", "ThumbImages");
                    Directory.CreateDirectory(layoutNormalDir);
                    Directory.CreateDirectory(layoutThumbDir);

                    string layoutTempPath = Path.Combine(layoutNormalDir, layoutFile);
                    using (var stream = new FileStream(layoutTempPath, FileMode.Create))
                    {
                        LayoutLogo.CopyTo(stream);
                    }
                    imageurl = layoutFile;
                }

                // ✅ Save faviconlogo
                if (faviconlogo != null && faviconlogo.Length > 0)
                {
                    faviconFile = Path.GetFileName(faviconlogo.FileName);
                    string faviconNormalDir = Path.Combine(uploadPath, "faviconlogo", "NormalImages");
                    string faviconThumbDir = Path.Combine(uploadPath, "faviconlogo", "ThumbImages");
                    Directory.CreateDirectory(faviconNormalDir);
                    Directory.CreateDirectory(faviconThumbDir);

                    string faviconTempPath = Path.Combine(faviconNormalDir, faviconFile);
                    using (var stream = new FileStream(faviconTempPath, FileMode.Create))
                    {
                        faviconlogo.CopyTo(stream);
                    }
                    imageurl1 = faviconFile;
                }

                // ✅ Save Loginlogo
                if (Loginlogo != null && Loginlogo.Length > 0)
                {
                    loginFile = Path.GetFileName(Loginlogo.FileName);
                    string loginNormalDir = Path.Combine(uploadPath, "Loginlogo", "NormalImages");
                    string loginThumbDir = Path.Combine(uploadPath, "Loginlogo", "ThumbImages");
                    Directory.CreateDirectory(loginNormalDir);
                    Directory.CreateDirectory(loginThumbDir);

                    string loginTempPath = Path.Combine(loginNormalDir, loginFile);
                    using (var stream = new FileStream(loginTempPath, FileMode.Create))
                    {
                        Loginlogo.CopyTo(stream);
                    }
                    imageurl2 = loginFile;
                }

                // ✅ Update DB — imageurl values may change after this call
                Int64 status = _appinfo.UpdateAppInfoDetails(objAppInfo, ref imageurl, ref imageurl1, ref imageurl2);

                // ✅ Copy LayoutLogo to final path after DB update
                if (!string.IsNullOrEmpty(layoutFile) && imageurl != "NA" && imageurl != "")
                {
                    string tempPath = Path.Combine(uploadPath, "LayoutLogo", "NormalImages", layoutFile);
                    string normalPath = Path.Combine(uploadPath, "LayoutLogo", "NormalImages", imageurl);
                    string thumbPath = Path.Combine(uploadPath, "LayoutLogo", "ThumbImages", imageurl);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                // ✅ Copy faviconlogo to final path after DB update
                if (!string.IsNullOrEmpty(faviconFile) && imageurl1 != "NA" && imageurl1 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "faviconlogo", "NormalImages", faviconFile);
                    string normalPath = Path.Combine(uploadPath, "faviconlogo", "NormalImages", imageurl1);
                    string thumbPath = Path.Combine(uploadPath, "faviconlogo", "ThumbImages", imageurl1);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                // ✅ Copy Loginlogo to final path after DB update
                if (!string.IsNullOrEmpty(loginFile) && imageurl2 != "NA" && imageurl2 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "Loginlogo", "NormalImages", loginFile);
                    string normalPath = Path.Combine(uploadPath, "Loginlogo", "NormalImages", imageurl2);
                    string thumbPath = Path.Combine(uploadPath, "Loginlogo", "ThumbImages", imageurl2);

                    if (System.IO.File.Exists(tempPath) && tempPath != normalPath)
                    {
                        System.IO.File.Copy(tempPath, normalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Copy(normalPath, thumbPath, true);
                }

                if (status != -1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes have been updated successfully";
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data error";
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
            }

            return RedirectToAction("Index", "AppApplicationSettings");
        }



        public ActionResult APPIndex()
        {
            //int Total = 0;
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            //lstChapters = _Chapters.GetChaptersList(ref Total);
            try
            {
                int status = 0;
                Entities.MobileAppInfo objMobileAppInfo = _appinfo.AppGetAppInfoDetails(ref status);



                if (status != 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.objMobileAppInfo = objMobileAppInfo;
                //  ViewBag.lstChapters = lstChapters;
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
            }
            return View();
        }



        [HttpPost]
        [Authorize]
        public ActionResult APPUpdateAppInfo(Entities.MobileAppInfo objAppInfo,
            IFormFile SplashMiddle,
            IFormFile SplashBottom,
            IFormFile HomeTopHeader,
            IFormFile Customloader,
            IFormFile OtherclasssHeader)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfoPath = _appinfoHelper.GetAppInfoDetails(ref status1);
                string uploadPath = objAppInfoPath.UploadPath;

                string imageurl = "NA"; string splashMiddleFile = "";
                string imageurl1 = "NA"; string splashBottomFile = "";
                string imageurl2 = "NA"; string homeTopHeaderFile = "";
                string imageurl3 = "NA"; string customloaderFile = "";
                string imageurl4 = "NA"; string otherclasssHeaderFile = "";

                // ✅ Save SplashMiddle
                if (SplashMiddle != null && SplashMiddle.Length > 0)
                {
                    splashMiddleFile = Path.GetFileName(SplashMiddle.FileName);
                    string normalDir = Path.Combine(uploadPath, "SplashMiddle", "normalphoto");
                    Directory.CreateDirectory(normalDir);

                    string tempPath = Path.Combine(normalDir, splashMiddleFile);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        SplashMiddle.CopyTo(stream);
                    }
                    imageurl = splashMiddleFile;
                }

                // ✅ Save SplashBottom
                if (SplashBottom != null && SplashBottom.Length > 0)
                {
                    splashBottomFile = Path.GetFileName(SplashBottom.FileName);
                    string normalDir = Path.Combine(uploadPath, "SplashBottom", "normalphoto");
                    Directory.CreateDirectory(normalDir);

                    string tempPath = Path.Combine(normalDir, splashBottomFile);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        SplashBottom.CopyTo(stream);
                    }
                    imageurl1 = splashBottomFile;
                }

                // ✅ Save HomeTopHeader
                if (HomeTopHeader != null && HomeTopHeader.Length > 0)
                {
                    homeTopHeaderFile = Path.GetFileName(HomeTopHeader.FileName);
                    string normalDir = Path.Combine(uploadPath, "HomeTopHeader", "normalphoto");
                    Directory.CreateDirectory(normalDir);

                    string tempPath = Path.Combine(normalDir, homeTopHeaderFile);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        HomeTopHeader.CopyTo(stream);
                    }
                    imageurl2 = homeTopHeaderFile;
                }

                // ✅ Save Customloader
                if (Customloader != null && Customloader.Length > 0)
                {
                    customloaderFile = Path.GetFileName(Customloader.FileName);
                    string normalDir = Path.Combine(uploadPath, "Customloader", "normalphoto");
                    Directory.CreateDirectory(normalDir);

                    string tempPath = Path.Combine(normalDir, customloaderFile);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        Customloader.CopyTo(stream);
                    }
                    imageurl3 = customloaderFile;
                }

                // ✅ Save OtherclasssHeader
                if (OtherclasssHeader != null && OtherclasssHeader.Length > 0)
                {
                    otherclasssHeaderFile = Path.GetFileName(OtherclasssHeader.FileName);
                    string normalDir = Path.Combine(uploadPath, "OtherclasssHeader", "normalphoto");
                    Directory.CreateDirectory(normalDir);

                    string tempPath = Path.Combine(normalDir, otherclasssHeaderFile);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        OtherclasssHeader.CopyTo(stream);
                    }
                    imageurl4 = otherclasssHeaderFile;
                }

                // ✅ Update DB — imageurl values may change after this call
                Int64 status = _appinfo.APPUpdateAppInfoDetails(
                    objAppInfo,
                    ref imageurl,
                    ref imageurl1,
                    ref imageurl2,
                    ref imageurl3,
                    ref imageurl4
                );

                // ✅ Copy SplashMiddle to final path after DB update
                if (!string.IsNullOrEmpty(splashMiddleFile) && imageurl != "NA" && imageurl != "")
                {
                    string tempPath = Path.Combine(uploadPath, "SplashMiddle", "normalphoto", splashMiddleFile);
                    string finalPath = Path.Combine(uploadPath, "SplashMiddle", "normalphoto", imageurl);

                    if (System.IO.File.Exists(tempPath) && tempPath != finalPath)
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                // ✅ Copy SplashBottom to final path after DB update
                if (!string.IsNullOrEmpty(splashBottomFile) && imageurl1 != "NA" && imageurl1 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "SplashBottom", "normalphoto", splashBottomFile);
                    string finalPath = Path.Combine(uploadPath, "SplashBottom", "normalphoto", imageurl1);

                    if (System.IO.File.Exists(tempPath) && tempPath != finalPath)
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                // ✅ Copy HomeTopHeader to final path after DB update
                if (!string.IsNullOrEmpty(homeTopHeaderFile) && imageurl2 != "NA" && imageurl2 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "HomeTopHeader", "normalphoto", homeTopHeaderFile);
                    string finalPath = Path.Combine(uploadPath, "HomeTopHeader", "normalphoto", imageurl2);

                    if (System.IO.File.Exists(tempPath) && tempPath != finalPath)
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                // ✅ Copy Customloader to final path after DB update
                if (!string.IsNullOrEmpty(customloaderFile) && imageurl3 != "NA" && imageurl3 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "Customloader", "normalphoto", customloaderFile);
                    string finalPath = Path.Combine(uploadPath, "Customloader", "normalphoto", imageurl3);

                    if (System.IO.File.Exists(tempPath) && tempPath != finalPath)
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                // ✅ Copy OtherclasssHeader to final path after DB update
                if (!string.IsNullOrEmpty(otherclasssHeaderFile) && imageurl4 != "NA" && imageurl4 != "")
                {
                    string tempPath = Path.Combine(uploadPath, "OtherclasssHeader", "normalphoto", otherclasssHeaderFile);
                    string finalPath = Path.Combine(uploadPath, "OtherclasssHeader", "normalphoto", imageurl4);

                    if (System.IO.File.Exists(tempPath) && tempPath != finalPath)
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                if (status != -1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes have been updated successfully";
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data error";
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error: " + ex.ToString());
            }

            return RedirectToAction("APPIndex", "AppApplicationSettings");
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
