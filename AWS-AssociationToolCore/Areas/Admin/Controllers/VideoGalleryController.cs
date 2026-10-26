using Microsoft.AspNetCore.Http;
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
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Videos,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    [Area("Admin")]

    public class VideoGalleryController : Controller
    {
        AWS_AssociationToolCore.BLL.VideoCategories _VideoCategory = new AWS_AssociationToolCore.BLL.VideoCategories();
        List<Entities.VideoCategories> lstVideoCategories = new List<Entities.VideoCategories>();
        AWS_AssociationToolCore.BLL.Videos _Video = new AWS_AssociationToolCore.BLL.Videos();
        List<Entities.Videos> lstVideos = new List<Entities.Videos>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.Users _users = new BLL.Users();

        #region Video Category

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
                if (status != 1)
                {
                    TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                }

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
        public ActionResult VideosCategoryList(Int64 ChapterId = 0, string Search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstVideoCategories = _VideoCategory.GetVideoCategoriesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstVideoCategories = lstVideoCategories;
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
        public ActionResult AddVideoCategory(Entities.VideoCategories objVideoCategory, Int64 mid = 0)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objVideoCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();

                objVideoCategory.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                Int64 _status = _VideoCategory.InsertVideoCategories(objVideoCategory);
                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = "Record Inserted Successfully";

                        _bool = true;
                        break;
                    case 2:
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = " Changes has been Updated Successfully";

                        _bool = true;
                        break;
                    case -1:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Data Error";
                        _bool = false;
                        break;
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Data Error";
                        _bool = false;
                        break;
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                _bool = false;
            }
            return RedirectToAction("Index", "VideoGallery", new { mid = mid });
        }

        [Authorize]
        public ActionResult EditVideoCategory(Int64 VideoCategoryId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.VideoCategories objVideoCategory = _VideoCategory.GetVideoCategoryById(VideoCategoryId, ref _qstatus);

                if (objVideoCategory != null)
                {
                    return Json(new { ok = true, data = objVideoCategory });
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
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteVideoCategory(Int64 VideoCategoryId)
        {

            try
            {
                Int64 _status = _VideoCategory.DeleteVideoCategory(VideoCategoryId);
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
                        message = "Failed Deleting the Category"
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

        #endregion

        #region Videos

        [HttpGet]
        [Authorize]
        public ActionResult Videos(Int64 VideoCategoryId = 0, Int64 mid = 0, Int64 ChapterId = 0)
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
            int Total = 0;
            int _qstatus = 0;
            int CategoryLevel = 0;

            try
            {

                //lstChapters = _Chapters.GetChaptersList(ref status);


                ViewBag.VideoCategoryId = VideoCategoryId;
                ViewBag.catlist = _VideoCategory.GetVideoCategoriesList(chapterId, ref Total);
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

           
    
            ViewBag.mid = mid;

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult VideosList(Int64 VideoCategoryId = 0, string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            int Total = 0;
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            List<Entities.Videos> lstVideos = new List<Entities.Videos>();
            try
            {
                lstVideos = _Video.GetVideosListByVariable(VideoCategoryId, Search, Sort, PageNo, Items, ref Total);
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.VideoCategoryId = VideoCategoryId;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.total = Total;
            ViewBag.lstVideosLst = lstVideos;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        public ActionResult AddVideo(Int64 VideoCategoryId = 0, Int64 ChapterId = 0)
        {
            long chapterId = 0;

            var chapterValue = HttpContext.Session.GetString("chapterid");

            ChapterId = Convert.ToInt64(chapterValue);
            int _qstatus = 0;
            try
            {
                List<Entities.VideoCategories> lstVideoCategories = _VideoCategory.GetVideoCategoriesList(ChapterId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.lstVideoCategories = lstVideoCategories;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "VideoGallery");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.VideoCategoryId = VideoCategoryId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddVideo(Entities.Videos objVideo, Int64 mid = 0)
        {
            try
            {
                objVideo.UpdatedBy = HttpContext.User.Identity.Name.ToString();

                objVideo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                Int64 status = _Video.InsertVideos(objVideo);

                if (status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                    return RedirectToAction("Videos", "VideoGallery", new { VideoCategoryId = objVideo.VideoCategoryId, mid = mid });
                }
                if (status == 2)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";
                    return RedirectToAction("Videos", "VideoGallery", new { VideoCategoryId = objVideo.VideoCategoryId, mid = mid });
                }
                else
                {
                    TempData["messageType"] = "warning";
                    TempData["message"] = "Failed uploading page.";

                    return RedirectToAction("AddVideo", "VideoGallery", new { VideoCategoryId = objVideo.VideoCategoryId, mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "An error occurred.";
                //return RedirectToAction("AddVideo", "VideoGallery");
                return RedirectToAction("AddVideo", "VideoGallery", new { VideoCategoryId = objVideo.VideoCategoryId, mid = mid });

            }
        }


        [Authorize]
        [HttpPost]
        public JsonResult VideoDelete(Int64 VideoId)
        {
            string str = "";

            try
            {
                Int64 _status = _Video.DeleteVideo(VideoId);
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

        [Authorize]
        [HttpPost]


        public IActionResult VideoDeleteAll(string VideoIds, string ImageUrl = "")
        {
            try
            {
                Int64 _status = _Video.DeleteAllVideos(VideoIds);

                if (_status == 1)
                {
                    // ✅ Use AppInfo.UploadPath - same pattern as your project
                    int status = 0;
                    var _appinfo = new BLL.AppInfo();
                    var objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                    string uploadPath = objAppInfo.UploadPath; // ✅ from DB

                    if (!string.IsNullOrEmpty(ImageUrl))
                    {
                        string[] urls = ImageUrl.Split(',');
                        foreach (var item in urls)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                string normalVideoPath = Path.Combine(uploadPath, "Videogallery", "normalVideo", item.Trim());
                                string thumbPath = Path.Combine(uploadPath, "Videogallery", "thumb", item.Trim());

                                if (System.IO.File.Exists(normalVideoPath))
                                    System.IO.File.Delete(normalVideoPath);

                                if (System.IO.File.Exists(thumbPath))
                                    System.IO.File.Delete(thumbPath);
                            }
                        }
                    }

                    return Json(new { ok = true, messageType = "success", message = "Records Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Deleting the Record" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = $"Failed transaction: {ex.Message}" });
            }
        }
        [Authorize]
        public ActionResult EditVideoPopup(Int64 VideoId = 0)
        {

            string str = "";
            Entities.Videos objVideos = new Entities.Videos();
            try
            {
                int _qstatus = 0;
                objVideos = _Video.GetVideoById(VideoId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = objVideos });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        public ActionResult VideoEdit(Int64 VideoId, Int64 ChapterId = 0)
        {
            try
            {
                int _qstatus = 0;
                Entities.Videos objVideos = _Video.GetVideoById(VideoId, ref _qstatus);
                int qstatus = 0;
                List<Entities.VideoCategories> lstVideoCategories = _VideoCategory.GetVideoCategoriesList(ChapterId, ref qstatus);

                if (_qstatus == 1 && qstatus == 1)
                {
                    ViewBag.objVideos = objVideos;
                    ViewBag.lstVideoCategories = lstVideoCategories;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Videos", "VideoGallery");
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Data Error";
                return RedirectToAction("Videos", "VideoGallery");
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult VideoStatus(Int64 VideoId)
        {

            try
            {
                Int64 _status = _Video.UpdateWebsiteBannersStatus(VideoId);
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
                        message = "Failed updating user status"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",

                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult VideoDisplayOrder(int DisplayOrder, Int64 VideoId)
        {

            try
            {
                Int64 _status = _Video.UpdateVideosDisplayOrder(DisplayOrder, VideoId);
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
                        message = "Failed Updated Order"
                    });

                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",

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
