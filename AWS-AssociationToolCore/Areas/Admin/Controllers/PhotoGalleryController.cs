using AWS_AssociationToolCore.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Photos,Media,DeveloperAdmin,")]

    [Area("Admin")]

    public class PhotoGalleryController : Controller
    {
        AWS_AssociationToolCore.BLL.PhotoCategories _PhotoCategory = new AWS_AssociationToolCore.BLL.PhotoCategories();
        List<Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();
        AWS_AssociationToolCore.BLL.Photos _Photo = new AWS_AssociationToolCore.BLL.Photos();
        List<Entities.Photos> lstPhotos = new List<Entities.Photos>();
        BLL.Users _users = new BLL.Users();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        private object _env;

        #region Photos Category

        [Authorize]
        public IActionResult Index(long mid = 0, long ChapterId = 0)
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
                var lstPhotoCategories = _PhotoCategory.GetPhotoCategoriesList(chapterId, ref status);
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
                ViewBag.lstPhotoCategories = lstPhotoCategories;
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
        public ActionResult PhotosCategoryList(string Search = "", string Year = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0,Int64 ChapterId=0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstPhotoCategories = _PhotoCategory.GetPhotoCategoriesListByVariable(Search, Year, Sort, PageNo, Items, ChapterId, ref Total);               
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstPhotoCategories = lstPhotoCategories;
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
        public ActionResult AddPhotoCategory(Entities.PhotoCategories objPhotoCategory, Int64 mid = 0)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objPhotoCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objPhotoCategory.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                Int64 _status = _PhotoCategory.InsertPhotoCategories(objPhotoCategory);
                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully.";
                        _bool = true;
                        break;
                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes has been Updated Successfully.";
                        _bool = true;
                        break;
                    case -1:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        TempData["message"] = "<div class=\"error-alert closable\"></div>";
                        _bool = false;
                        break;
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        _bool = false;
                        break;
                }
            }
            catch 
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed transaction.";
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                _bool = false;
            }
            return RedirectToAction("Index", "PhotoGallery", new { mid = mid });
        }

        [Authorize]
        public ActionResult EditPhotoCategory(Int64 PhotoCategoryId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.PhotoCategories objPhotoCategory = _PhotoCategory.GetPhotoCategoriesById(PhotoCategoryId, ref _qstatus);

                if (objPhotoCategory != null)
                {
                    return Json(new { ok = true, data = objPhotoCategory });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
                }
            }
            catch 
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeletePhotoCategory(Int64 PhotoCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _PhotoCategory.DeletePhotoCategories(PhotoCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully." });
                }
                else
                {
                    return Json(new { ok = false, messageType = "success", message = "Failed deleting Record." });
                }
            }
            catch 
            {
                return Json(new { ok = false, messageType = "success", message = "Failed transaction." });
            }
        }

        #endregion       

        #region Photos

        [HttpGet]
        [Authorize]
        public IActionResult Photos(long PhotoCategoryId = 0, long mid = 0, long ChapterId = 0)
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

            int status = 0;
            var objuserroles = new Entities.UserRoles();
            List<Entities.PhotoCategories> lstPhotoCategories = new List<Entities.PhotoCategories>();

            long chapterId = 0; // ✅ Moved OUTSIDE try block

            try
            {
                int Total = 0;
                var chapterValue = HttpContext.Session.GetString("chapterid");
                if (!string.IsNullOrEmpty(chapterValue))
                {
                    long.TryParse(chapterValue, out chapterId);
                }
                lstPhotoCategories = _PhotoCategory.GetPhotoCategoriesList(chapterId, ref Total);
                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed transaction.";
            }

            ViewBag.PhotoCategoryId = PhotoCategoryId;
            ViewBag.catlist = lstPhotoCategories;

            // Store permissions in session
            HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

            // Pass permissions to ViewBag
            ViewBag.IsEdit = objuserroles.IsEdit;
            ViewBag.IsView = objuserroles.IsView;
            ViewBag.IsDelete = objuserroles.IsDelete;
            ViewBag.IsExport = objuserroles.IsExport;
            ViewBag.IsAdd = objuserroles.IsAdd;
            ViewBag.mid = mid;
            ViewBag.ChapterId = chapterId; // ✅ Now accessible here

            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult PhotosList(Int64 PhotoCategoryId = 0, string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0,Int64 ChapterId=0)
        {
            int Total = 0;
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            List<Entities.Photos> lstPhotos =new List<Entities.Photos>();
            try
            {
                lstPhotos = _Photo.GetPhotosListByVariable(PhotoCategoryId, Search, Sort, PageNo, Items, ref Total);
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.lstPhotosLst = lstPhotos;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.PhotoCategoryId = PhotoCategoryId;
            ViewBag.total = Total;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.mid = Mid;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        [Authorize]
        public ActionResult AddPhoto(Int64 PhotoCategoryId = 0, Int64 mid = 0,Int64 ChapterId=0)
        {
            int _qstatus = 0;
            try
            {
                List<Entities.PhotoCategories> lstPhotoCategories = _PhotoCategory.GetPhotoCategoriesList(ChapterId,ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.lstPhotoCategories = lstPhotoCategories;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully.";

                   


                    return RedirectToAction("Index", "PhotoGallery");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.PhotoCategoryId = PhotoCategoryId;
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        [HttpPost]

        public IActionResult AddPhoto(Entities.Photos objPhoto, List<IFormFile> file1, long PhotoId = 0, string Thumbimage = "", long mid = 0)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                foreach (var file in file1)
                {
                    if (file != null && file.Length > 0)
                    {
                        string imageUrl = Path.GetFileName(file.FileName);
                        Image image = null;

                        // Decode base64 thumbnail if provided
                        if (!string.IsNullOrEmpty(Thumbimage))
                        {
                            string base64String = Thumbimage.Contains(",") ? Thumbimage.Split(',')[1] : Thumbimage;
                            byte[] bytes = Convert.FromBase64String(base64String);
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                image = Image.FromStream(ms);
                            }
                        }

                        // Save uploaded file temporarily
                        string tempPath = Path.Combine(objappinfo.UploadPath, "temp", imageUrl);
                        Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                        using (var stream = new FileStream(tempPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        objPhoto.UpdatedBy = HttpContext.User.Identity.Name;
                        objPhoto.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        long status = _Photo.InsertPhotos(objPhoto, ref imageUrl);

                        // Save normal and thumb images
                        if ((status == 1 || status == 2) && image != null)
                        {
                            string normalPath = Path.Combine(objappinfo.UploadPath, "photogallery", "normalphoto", imageUrl);
                            string thumbPath = Path.Combine(objappinfo.UploadPath, "photogallery", "thumbphoto", imageUrl);
                            Directory.CreateDirectory(Path.GetDirectoryName(normalPath));
                            Directory.CreateDirectory(Path.GetDirectoryName(thumbPath));

                            file.CopyTo(new FileStream(normalPath, FileMode.Create));
                            image.Save(thumbPath);
                        }

                        // Set TempData message
                        if (status == 1)
                        {
                            TempData["messageType"] = "success";
                            TempData["message"] = "Added Records Successfully.";
                        }
                        else if (status == 2)
                        {
                            TempData["messageType"] = "warning";
                            TempData["message"] = "Changes have been Updated Successfully.";
                        }
                        else
                        {
                            TempData["messageType"] = "error";
                            TempData["message"] = "Failed processing your request.";
                        }
                    }
                    else if (PhotoId != 0)
                    {
                        // Handle update without file
                        string imageUrl = "NA";
                        objPhoto.UpdatedBy = HttpContext.User.Identity.Name;
                        long status = _Photo.InsertPhotos(objPhoto, ref imageUrl);

                        if (status == 1)
                        {
                            TempData["messageType"] = "success";
                            TempData["message"] = "Record Inserted Successfully.";
                        }
                        else if (status == 2)
                        {
                            TempData["messageType"] = "warning";
                            TempData["message"] = "Changes have been Updated Successfully.";
                        }
                        else
                        {
                            TempData["messageType"] = "error";
                            TempData["message"] = "Failed uploading image.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = $"Failed transaction: {ex.Message}";
            }

            return RedirectToAction("Photos", "PhotoGallery", new { PhotoCategoryId = objPhoto.PhotoCategoryId, mid = mid });
        }
        [Authorize]
        [HttpPost]
        public JsonResult PhotoDelete(long PhotoId, string ImageFileName)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);
                long status = _Photo.DeletePhotos(PhotoId);

                if (status == 1)
                {
                    // Combine paths safely using WebRootPath
                    string normalPath = Path.Combine(objappinfo.UploadPath ?? "", "photogallery", "normalphoto", ImageFileName);
                    string thumbPath = Path.Combine(objappinfo.UploadPath ?? "", "photogallery", "thumbphoto", ImageFileName);

                    // Delete files if they exist
                    if (System.IO.File.Exists(normalPath))
                        System.IO.File.Delete(normalPath);

                    if (System.IO.File.Exists(thumbPath))
                        System.IO.File.Delete(thumbPath);

                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "warning", message = "Failed deleting photo." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = $"Failed transaction: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpPost]
        //public JsonResult PhotoDeleteAll(string PhotoIds)
        //{
        //    try
        //    {
        //        string imageUrls = "";
        //        long status = _Photo.DeleteAllPhotos(PhotoIds, ref imageUrls);

        //        if (status == 1)
        //        {
        //            string[] urls = imageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //            foreach (var item in urls)
        //            {
        //                string normalPath = Path.Combine(_env.WebRootPath, "photogallery", "normalphoto", item);
        //                string thumbPath = Path.Combine(_env.WebRootPath, "photogallery", "thumbphoto", item);

        //                if (System.IO.File.Exists(normalPath))
        //                    System.IO.File.Delete(normalPath);

        //                if (System.IO.File.Exists(thumbPath))
        //                    System.IO.File.Delete(thumbPath);
        //            }

        //            return Json(new { ok = true, messageType = "success", message = "Records Deleted Successfully" });
        //        }
        //        else
        //        {
        //            return Json(new { ok = false, messageType = "error", message = "Failed deleting photos." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { ok = false, messageType = "error", message = $"Failed transaction: {ex.Message}" });
        //    }
        //}

        [HttpGet]
        [Authorize]
        public IActionResult PhotoEdit(Int64 PhotoId, Int64 mid = 0, Int64 ChapterId = 0, Int64 PhotoCategoryId = 0)
        {
            try
            {
                int _qstatus = 0;
                Entities.Photos objPhotos = _Photo.GetPhotosById(PhotoId, ref _qstatus);

                int qstatus = 0;
                List<Entities.PhotoCategories> lstPhotoCategories =
                    _PhotoCategory.GetPhotoCategoriesList(ChapterId, ref qstatus);

                if (_qstatus == 1 && qstatus == 1)
                {
                    ViewBag.objPhotos = objPhotos;
                    ViewBag.lstPhotoCategories = lstPhotoCategories;
                    ViewBag.mid = mid;
                    ViewBag.ChapterId = ChapterId;        // ✅ Was missing
                    ViewBag.PhotoCategoryId = PhotoCategoryId;
                    ViewBag.status = _qstatus;

                    // ✅ Permissions from session
                    ViewBag.IsEdit = Convert.ToBoolean(HttpContext.Session.GetString("IsEdit"));
                    ViewBag.IsView = Convert.ToBoolean(HttpContext.Session.GetString("IsView"));
                    ViewBag.IsDelete = Convert.ToBoolean(HttpContext.Session.GetString("IsDelete"));
                    ViewBag.IsExport = Convert.ToBoolean(HttpContext.Session.GetString("IsExport"));
                    ViewBag.IsAdd = Convert.ToBoolean(HttpContext.Session.GetString("IsAdd"));
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction.";
                    return RedirectToAction("Photos", "PhotoGallery");
                }
            }
            catch (Exception ex)  // ✅ Catch exception to see actual error
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;  // ✅ Show real error for debugging
                return RedirectToAction("Photos", "PhotoGallery");
            }

            return View();
        }
        [Authorize]
        public JsonResult PhotosDisplayOrder(int DisplayOrder, Int64 PhotoId)
        {
            string str = "";
            try
            {
                Int64 _status = _Photo.UpdatePhotosDisplayOrder(DisplayOrder, PhotoId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\"></div>";
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\"></div>";
                    return Json(new { ok = false, messageType = "warning", message = "Failed updating status" });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\"></div>";
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
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
