using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nancy;
using SkiaSharp;
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

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ThemeBanners,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class WebsiteBannersController : Controller
    {
        AWS_AssociationToolCore.BLL.WebsiteBanners _WebsiteBanners = new AWS_AssociationToolCore.BLL.WebsiteBanners();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.Users _users = new BLL.Users();

        [Authorize]
        public ActionResult Index(Int64 ChapterId = 0, Int64 mid = 0)
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

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult BannersList(Int64 ChapterId = 0, string search = "", int PageNo = 1, int PageItems = 25, string SortColumn = "", string SortOrder = "", bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            int Total = 0;
            List<Entities.WebsiteBanners> Bannerslist = new List<Entities.WebsiteBanners>();
            try
            {
                string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");

                Bannerslist = _WebsiteBanners.GetWebsiteBannerListByVariable(ChapterId, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.items = PageItems;
            ViewBag.pageno = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.Bannerslist = Bannerslist;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [HttpPost]
        [Authorize]

        public ActionResult AddWebsiteBanners(Entities.WebsiteBanners objWebsiteBanner, IFormFile BannerUrl, string Mid)
        {
            try
            {
                // ✅ AppInfo for UploadPath
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

                lstLogReport.Add(new Entities.LogReport
                {
                    LogTitle = "Websitebanners",
                    LogDescription = $"Enter into the Websitebanners - ({objWebsiteBanner.UpdatedBy})"
                });
 

                string imageurl = "";

                if (BannerUrl == null || BannerUrl.Length == 0)
                {
                    // ✅ No file uploaded — just save record
                    lstLogReport.Add(new Entities.LogReport
                    {
                        LogTitle = "Websitebanners",
                        LogDescription = $"BannerUrl is null - ({objWebsiteBanner.UpdatedBy})"
                    });

                    objWebsiteBanner.UpdatedBy = this.User.Identity.Name;
                    objWebsiteBanner.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    Int64 _status = _WebsiteBanners.InsertWebsiteBanners(objWebsiteBanner, ref imageurl);

                    if (_status != -1)
                    {
                        TempData["messageType"] = _status == 1 ? "success" : "success";
                        TempData["message"] = _status == 1 ? "Record Inserted Successfully"
                                                               : "Changes has been Updated Successfully";
                    }
                }
                else
                {
                    // ✅ File uploaded — save to NormalImages
                    lstLogReport.Add(new Entities.LogReport
                    {
                        LogTitle = "Websitebanners",
                        LogDescription = $"File uploading - ({objWebsiteBanner.UpdatedBy})"
                    });

                    string fileName = Path.GetFileName(BannerUrl.FileName);
                    string normalDir = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "NormalImages");
                    string thumbDir = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "ThumbImages");
                    string normalPath = Path.Combine(normalDir, fileName);
                    string thumbPath = Path.Combine(thumbDir, fileName);

                    // ✅ Create directories if not exist
                    Directory.CreateDirectory(normalDir);
                    Directory.CreateDirectory(thumbDir);

                    // ✅ Save uploaded file
                    using (var stream = new FileStream(normalPath, FileMode.Create))
                    {
                        BannerUrl.CopyTo(stream);
                    }

                    imageurl = fileName;

                    objWebsiteBanner.UpdatedBy = this.User.Identity.Name;
                    objWebsiteBanner.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    Int64 _status = _WebsiteBanners.InsertWebsiteBanners(objWebsiteBanner, ref imageurl);

                    if (_status != -1)
                    {
                        if (!string.IsNullOrEmpty(imageurl))
                        {
                            string finalNormalPath = Path.Combine(normalDir, imageurl);
                            string finalThumbPath = Path.Combine(thumbDir, imageurl);

                            // ✅ Copy to final normal path (if name changed by DB)
                            if (normalPath != finalNormalPath && System.IO.File.Exists(normalPath))
                            {
                                System.IO.File.Copy(normalPath, finalNormalPath, true);
                                System.IO.File.Delete(normalPath);
                            }

                            // ✅ Copy same image to thumb folder
                            if (System.IO.File.Exists(finalNormalPath))
                                System.IO.File.Copy(finalNormalPath, finalThumbPath, true);
                        }

                        TempData["messageType"] = "success";
                        TempData["message"] = _status == 1 ? "Record Inserted Successfully."
                                                               : "Changes has been Updated Successfully";
                    }
                    else
                    {
                        TempData["messageType"] = "warning";
                        TempData["message"] = "Uploading Failed";
                    }
                }

                return RedirectToAction("Index", "WebsiteBanners", new { mid = Mid });
            }
            catch (Exception ex)
            {
                // ✅ Log the exception
                try
                {
                    List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
                    lstLogReport.Add(new Entities.LogReport
                    {
                        LogTitle = "Websitebanners",
                        LogDescription = $"Exception - ({ex.Message})"
                    });
                }
                catch { }

                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "WebsiteBanners", new { mid = Mid });
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditBanner(Int64 WebsiteBannerId)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.WebsiteBanners objWebsiteBanners = _WebsiteBanners.GetWebsiteBannerById(WebsiteBannerId, ref status);
                if (objWebsiteBanners != null)
                {
                    return Json(new { ok = true, data = objWebsiteBanners });
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
        [Authorize]
      
        public JsonResult DeleteBanner(Int64 WebsiteBannerId, string BannerUrl = "")
        {
            try
            {
                Int64 _status = _WebsiteBanners.DeleteWebsiteBanner(WebsiteBannerId);

                if (_status == 1)
                {
                    // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                    int status = 0;
                    BLL.AppInfo _appinfo = new BLL.AppInfo();
                    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                    if (!string.IsNullOrEmpty(BannerUrl))
                    {
                        string normalPath = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "NormalImages", BannerUrl);
                        string thumbPath = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "ThumbImages", BannerUrl);

                        if (System.IO.File.Exists(normalPath))
                            System.IO.File.Delete(normalPath);

                        if (System.IO.File.Exists(thumbPath))
                            System.IO.File.Delete(thumbPath);
                    }

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
                        message = "Failed Deleting the Record"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = $"Failed transaction: {ex.Message}"
                });
            }
        }
        [HttpPost]
        [Authorize]
        public JsonResult WebsiteBannersStatus(Int64 WebsiteBannerId)
        {
            string str = "";
            try
            {
                Int64 _status = _WebsiteBanners.UpdateWebsiteBannersStatus(WebsiteBannerId);
                if (_status == 1)
                {

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });


                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    // return Json(new { ok = true, data = str });
                }
                else
                {
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
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

                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult WebsiteBannerOrderNo(int OrderNo, Int64 WebsiteBannerId)
        {
            string str = "";
            try
            {
                Int64 _status = _WebsiteBanners.UpdateWebsiteBannersOrderNo(OrderNo, WebsiteBannerId);
                if (_status == 1)
                {
                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
                    //return Json(new { ok = true, data = str });
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Order Successfully"
                    });
                }
                else
                {
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Updating Status"
                    });
                }
            }
            catch (Exception ex)
            {
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
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
        public ActionResult Crop(string CategoryName = "", string filename = "", int top = 0, int left = 0, int bottom = 0, int right = 0)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                string normalPath = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "NormalImages", filename);
                string thumbPath = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "ThumbImages", filename);

                if (System.IO.File.Exists(normalPath))
                {
                    ImageHelper.CropAndResize(normalPath, thumbPath, left, top, right, bottom, 145, 80);
                }

                return RedirectToAction("Index", "WebsiteBanners", new { Type = "close", CategoryName = CategoryName });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "WebsiteBanners", new { Type = "close", CategoryName = CategoryName });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Preview(Int64 WebsiteBannerId = 0, string filename = "", int imgheight = 0, int imgwidth = 0)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                // ✅ Get banner details
                int status = 0;
                Entities.WebsiteBanners objWebsiteBanners = new Entities.WebsiteBanners();

                try
                {
                    objWebsiteBanners = _WebsiteBanners.GetWebsiteBannerById(WebsiteBannerId, ref status);
                }
                catch (Exception ex)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = ex.Message;
                }

                // ✅ Use ImageSharp instead of WebImage
                string imagePath = Path.Combine(objAppInfo.UploadPath, "WebsiteBanners", "NormalImages", objWebsiteBanners.BannerUrl ?? "");

                int height = 0;
                int width = 0;


                if (System.IO.File.Exists(imagePath))
                {
                    using (var codec = SKCodec.Create(imagePath))
                    {
                        if (codec != null)
                        {
                            height = codec.Info.Height;
                            width = codec.Info.Width;
                        }
                    }
                }


                ViewBag.filename = objWebsiteBanners.BannerUrl;
                ViewBag.imgheight = height;
                ViewBag.imgwidth = width;

                return View();
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "WebsiteBanners");
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
