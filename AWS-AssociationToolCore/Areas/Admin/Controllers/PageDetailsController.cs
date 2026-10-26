//using System.Web.Helpers;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
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
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,PageDetails,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class PageDetailsController : Controller
    {
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        List<Entities.PageDetails> lstPageDetails = new List<Entities.PageDetails>();
        BLL.MenuItems _MenuItems = new BLL.MenuItems();
        List<Entities.MenuItems> lstMenuItems = new List<Entities.MenuItems>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();
        BLL.Users _users = new BLL.Users();
        private string _uploadRootPath;

        #region PageDetails
        [Authorize]
        public IActionResult Index(long mid = 0, long ChapterId = 0)
        {
            long UserId = 0;
            string userRole = "";
            string chapterIdFromCookie = "";

            // 1️⃣ Get UserId from cookies or session
            if (Request.Cookies.TryGetValue("UserId", out string userIdCookie))
            {
                long.TryParse(userIdCookie, out UserId);
            }

            // 2️⃣ Get UserRole from cookies
            if (Request.Cookies.TryGetValue("UserRole", out string userRoleCookie))
            {
                userRole = userRoleCookie;
                HttpContext.Session.SetString("userrole", userRole);
            }

            // 3️⃣ Get ChapterId from cookies
            if (Request.Cookies.TryGetValue("chapterid", out string chapterCookie))
            {
                chapterIdFromCookie = chapterCookie;
                HttpContext.Session.SetString("chapterid", chapterIdFromCookie);
                long.TryParse(chapterCookie, out ChapterId);
            }

            // 4️⃣ Initialize variables
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            Entities.UserRoles objUserRoles = new Entities.UserRoles();
            int status = 0;

            try
            {
                // Get list of chapters
                lstChapters = _Chapters.GetChaptersList(ref status);

                // Get role details for user
                objUserRoles = _users.GetRoleDetialsById(UserId, mid, ref status);
            }
            catch
            {
                status = -1;
            }

            // 5️⃣ Save permissions in session
            HttpContext.Session.SetString("IsEdit", objUserRoles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objUserRoles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objUserRoles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objUserRoles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objUserRoles.IsAdd.ToString());

            // 6️⃣ Pass data to ViewBag
            ViewBag.IsEdit = objUserRoles.IsEdit;
            ViewBag.IsView = objUserRoles.IsView;
            ViewBag.IsDelete = objUserRoles.IsDelete;
            ViewBag.IsExport = objUserRoles.IsExport;
            ViewBag.IsAdd = objUserRoles.IsAdd;
            ViewBag.mid = mid;
            ViewBag.ChapterId = ChapterId;
            ViewBag.lstChapters = lstChapters;

            return View();
        }

        public ActionResult PageDetailsList(Int64 ChapterId = 0, string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstPageDetails = _PageDetails.GetPageDetailsListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
               
            }
            catch
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstPageDetails = lstPageDetails;
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

        public ActionResult AddPageDetails(Int64 ChapterId = 0, Int64 mid = 0)
        {
            List<Entities.MenuItems> lstMenuItems2 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems3 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems4 = new List<Entities.MenuItems>();
            int status = 0;
            List<Entities.Chapters> lstChapters = _Chapters.GetChaptersList(ref status);
            List<Entities.MenuItems> lstExistingMenuItems = _MenuItems.ExistingMenuItemsGetList(ref status);
            List<Entities.MenuItems> lstMenuItems = _MenuItems.GetMenuItemsDD(ChapterId, ref lstMenuItems2, ref lstMenuItems3, ref lstMenuItems4, ref status);
            if (status == 1)
            {

                ViewBag.lstChapters = lstChapters;
                ViewBag.lstMenuItems = lstMenuItems;
                ViewBag.lstMenuItems2 = lstMenuItems2;
                ViewBag.lstMenuItems3 = lstMenuItems3;
                ViewBag.lstMenuItems4 = lstMenuItems4;

            }
            ViewBag.mid = mid;
            ViewBag.lstExistingMenuItems = lstExistingMenuItems;
            ViewBag.ChapterId = ChapterId;
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AddPageDetails(Entities.PageDetails objPageDetails, IFormFile file, long mid = 0)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string folderPath = Path.Combine(objappinfo.UploadPath, "PageDocuments");
                Directory.CreateDirectory(folderPath);

                string docUrl = "NA";      // ✅ tell SP "no new file" by default
                string tempFileName = "";
                string tempPath = "";
                bool newFileUploaded = false;

                if (file != null && file.Length > 0)
                {
                    tempFileName = Path.GetFileName(file.FileName);
                    tempPath = Path.Combine(folderPath, tempFileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    docUrl = tempFileName;   // input to SP — SP will build the final unique name from this
                    newFileUploaded = true;
                }

                // Set metadata
                string currentUser = User.Identity?.Name ?? "System";
                objPageDetails.InsertedBy = currentUser;
                objPageDetails.UpdatedBy = currentUser;
                objPageDetails.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objPageDetails.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objPageDetails.IsActive = true;

                long status = _PageDetails.InsertPageDetails(objPageDetails, ref docUrl);

                // ✅ Rename physical file to match the SP-computed name actually saved in DB
                if (newFileUploaded && !string.IsNullOrEmpty(docUrl))
                {
                    string finalPath = Path.Combine(folderPath, docUrl);

                    if (!string.Equals(tempPath, finalPath, StringComparison.OrdinalIgnoreCase)
                        && System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Copy(tempPath, finalPath, true);
                        System.IO.File.Delete(tempPath);
                    }
                }

                switch (status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully.";
                        break;
                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes have been Updated Successfully.";
                        break;
                    default:
                        // clean up orphaned upload if SP failed
                        if (newFileUploaded && System.IO.File.Exists(tempPath))
                        {
                            System.IO.File.Delete(tempPath);
                        }
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        break;
                }

                string redirectController = objPageDetails.AddPage switch
                {
                    "Only Page" => "PageDetails",
                    "Existing Menu" => "MenuItems",
                    _ => "MenuItems"
                };

                return RedirectToAction("Index", "PageDetails", new { area = "Admin", mid });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "PageDetails", new { area = "Admin", mid });
            }
        }

        //public IActionResult AddPageDetails(Entities.PageDetails objPageDetails, IFormFile file, long mid = 0)
        //{
        //    try
        //    {
        //        int status1 = 0;
        //        BLL.AppInfo _appinfo = new BLL.AppInfo();
        //        Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status1);

        //        string fileUrl = "";

        //        if (file != null && file.Length > 0)
        //        {
        //            // Generate unique filename
        //            fileUrl = $"{BLL.Common.EncodeURL(objPageDetails.Heading)}" +
        //                      $"-{DateTime.Now:ddMMyyyyHHmmss}" +
        //                      $"{Path.GetExtension(file.FileName)}";

        //            objPageDetails.DocumentUrl = fileUrl;
        //        }

        //        // Set metadata
        //        string currentUser = User.Identity?.Name ?? "System";
        //        objPageDetails.InsertedBy = currentUser;
        //        objPageDetails.UpdatedBy = currentUser;
        //        objPageDetails.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //        objPageDetails.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //        objPageDetails.IsActive = true;

        //        long status = _PageDetails.InsertPageDetails(objPageDetails);

        //        // ✅ Save file — replaces file.SaveAs() which is HttpPostedFileBase only
        //        if (!string.IsNullOrEmpty(fileUrl) && file != null)
        //        {
        //            string folderPath = Path.Combine(objappinfo.UploadPath, "PageDocuments");

        //            // ✅ Create folder if it doesn't exist
        //            Directory.CreateDirectory(folderPath);

        //            string fullPath = Path.Combine(folderPath, fileUrl);

        //            // ✅ CopyTo() — correct way for IFormFile
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //        }

        //        // Set TempData based on status
        //        switch (status)
        //        {
        //            case 1:
        //                TempData["messageType"] = "success";
        //                TempData["message"] = "Record Inserted Successfully.";
        //                break;
        //            case 2:
        //                TempData["messageType"] = "success";
        //                TempData["message"] = "Changes have been Updated Successfully.";
        //                break;
        //            default:
        //                TempData["messageType"] = "error";
        //                TempData["message"] = "Failed processing your request.";
        //                break;
        //        }

        //        // Determine redirect based on AddPage property
        //        string redirectController = objPageDetails.AddPage switch
        //        {
        //            "Only Page" => "PageDetails",
        //            "Existing Menu" => "MenuItems",
        //            _ => "MenuItems"
        //        };

        //        return RedirectToAction("Index", redirectController,
        //            new { area = "Admin", mid });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["messageType"] = "error";
        //        TempData["message"] = ex.Message;
        //        return RedirectToAction("AddPageDetails", "PageDetails",
        //            new { area = "Admin", mid });
        //    }
        //}



        public ActionResult EditPageDetails( Int64 PageDetailId = 0, Int64 MenuItemId = 0, Int64 mid = 0,Int64 ChapterId=0)
        {
            Entities.PageDetails _objPageDetails = new Entities.PageDetails();
            Entities.MenuPages objMenuPages = new Entities.MenuPages();
            Entities.MenuItems objMenuItems = new Entities.MenuItems();
            List<Entities.MenuItems> lstMenuItems2 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems3 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems4 = new List<Entities.MenuItems>();
            int status = 0;
            List<Entities.Chapters> lstChapters = _Chapters.GetChaptersList(ref status);
            List<Entities.MenuItems> lstExistingMenuItems = _MenuItems.ExistingMenuItemsGetbyId(PageDetailId, ref status);
            List<Entities.MenuItems> lstMenuItems = _MenuItems.GetMenuItemsDD(1, ref lstMenuItems2, ref lstMenuItems3, ref lstMenuItems4, ref status);
            if (status == 1)
            {

                ViewBag.lstChapters = lstChapters;
                ViewBag.lstMenuItems = lstMenuItems;
                ViewBag.lstMenuItems2 = lstMenuItems2;
                ViewBag.lstMenuItems3 = lstMenuItems3;
                ViewBag.lstMenuItems4 = lstMenuItems4;

            }
            try
            {

                int _qstatus = 0;
                // Entities.PageDetails _objPageDetails = _PageDetails.GetPageDetailsById(PageDetailId, ref _qstatus);
                _objPageDetails = _PageDetails.MenuPagesDetailsGetById(ref objMenuItems, ref objMenuPages, PageDetailId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objPageDetails = _objPageDetails;
                    ViewBag.objMenuItems = objMenuItems;
                    ViewBag.objMenuPages = objMenuPages;
                    ViewBag.MenuItemId = MenuItemId;
                    ViewBag.status = _qstatus;
                    ViewBag.ChapterId = ChapterId;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction.";
                    return RedirectToAction("Index", "PageDetails", new { mid = mid });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "PageDetails", new { mid = mid });
            }
            ViewBag.mid = mid;
            ViewBag.lstExistingMenuItems = lstExistingMenuItems;

            return View();
        }

        public ActionResult ViewPageDetails(Int64 PageDetailId = 0, Int64 mid = 0)
        {
            try
            {
               
                int _qstatus = 0;
                Entities.PageDetails _objPageDetails = _PageDetails.GetPageDetailsById(PageDetailId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objDetails = _objPageDetails;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "PageDetails", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Data Error";
                return RedirectToAction("Index", "PageDetails", new { mid = mid });
            }
            return View();
        }

        [HttpPost]
        public JsonResult DeletePageDetails(Int64 PageDetailId)
        {
            string str = "";
            try
            {
               
                Int64 _status = _PageDetails.DeletePageDetails(PageDetailId);
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

        [HttpPost]
        public JsonResult PageDetailstatus(Int64 PageDetailId)
        {
            string str = "";
            try
            {
                Int64 _status = _PageDetails.UpdatePageDetailsStatus(PageDetailId);
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
        public ActionResult PagedetailsRemoveDocumentUrl(Int64 PageDetailId=0, Int64 mid = 0)
        {
            try
            {
             
                Int64 status = _PageDetails.PagedetailsRemoveDocumentUrl(PageDetailId);
                if (status != -1)
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "success",
                        message = "Remove DocumentUrl Successfully"
                    });
                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Remove DocumentUrl"
                    });
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed updating payment settings.</div>";
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction"
                });
            }
           // return RedirectToAction("EditPageDetails", "PageDetails", new { PageDetailId= PageDetailId, mid = mid });
        }

        [HttpPost]
        public JsonResult CheckPageHeadingAvailability(long PageDetailId, string Heading)
        {
            int status = 0;
            try
            {
                Entities.PageDetails objPageDetails = _PageDetails.PageDetailsGetByHeading(Heading, ref status);

                bool data = (objPageDetails == null ||
                             objPageDetails.PageDetailId == PageDetailId ||
                             objPageDetails.PageDetailId == 0);

                return Json(new { ok = true, data = data });
            }
            catch
            {
                return Json(new { ok = false, message = "Failed transaction." });
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
