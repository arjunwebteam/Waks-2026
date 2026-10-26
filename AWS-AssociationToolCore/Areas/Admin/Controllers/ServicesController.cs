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
    [Area("Admin")]

    public class ServicesController : Controller
    {
        BLL.Services _Services = new BLL.Services();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Services> lstServices = new List<Entities.Services>();
        List<Entities.ServiceCategories> lstServiceCategories = new List<Entities.ServiceCategories>();
        BLL.ServiceCategories _ServiceCategories = new BLL.ServiceCategories();

        List<Entities.ServiceTypes> lstServiceTypes = new List<Entities.ServiceTypes>();
        BLL.ServiceTypes _ServiceTypes = new BLL.ServiceTypes();
        BLL.Users _users = new BLL.Users();

        #region Services

        [Authorize]
        public IActionResult Index(Int64 mid = 0, Int64 ChapterId = 0)
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
                lstServiceCategories = _ServiceCategories.GetServiceCategoriesList(ref _qstatus);
                lstServiceTypes = _ServiceTypes.GetServiceTypesList(ref _qstatus);


                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
                if (_qstatus != 1)
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
                ViewBag.lstServiceCategories = lstServiceCategories;
                ViewBag.lstServiceTypes = lstServiceTypes;


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
    public ActionResult ServicesList(Int64 ChapterId = 0, string ServiceType = "",string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstServices = _Services.GetServicesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"error closable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServices = lstServices;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult AddService()
        {
            int _qstatus = 0;
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            try
            {

                lstChapters = _Chapters.GetChaptersList(ref _qstatus);
                lstServiceCategories = _ServiceCategories.GetServiceCategoriesList(ref _qstatus);
                lstServiceTypes = _ServiceTypes.GetServiceTypesList(ref _qstatus);

                if (_qstatus != 1)
                {
                    TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Services");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Services");
            }
            ViewBag.lstChapters = lstChapters;
            ViewBag.lstServiceCategories = lstServiceCategories;
            ViewBag.lstServiceTypes = lstServiceTypes;

            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddService(Entities.Services objServices, IFormFile DocumentUrl, Int64 mid = 0)
        {
            try
            {
                var Document = DocumentUrl == null
      ? "N/A"
      : Path.GetFileName(DocumentUrl.FileName);
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);
                //var image = WebImage.GetImageFromRequest();
                //string imageurl = (image != null ? image.ImageFormat : "N/A");

                objServices.UpdatedBy = objServices.ServiceTitle;
                objServices.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                objServices.IsDisplay = true;
                Int64 _status = _Services.InsertServices(objServices, ref Document);

                if (_status == 1)
                {

                    //if (Document != "")
                    //{
                    //    string path = Path.Combine(objappinfo.UploadPath + "\\Services\\Documents\\", Path.GetFileName(Document));
                    //    DocumentUrl.SaveAs(path);
                    //}
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                    //return RedirectToAction("Index", "Services");
                    return RedirectToAction("Index", "Services", new { mid = mid });
                }
                if (_status == 2)
                {
                    //if (Document != "")
                    //{
                    //    string path = Path.Combine(objappinfo.UploadPath + "\\Services\\Documents\\", Path.GetFileName(Document));
                    //    DocumentUrl.SaveAs(path);
                    //}
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";
                    return RedirectToAction("Index", "Services", new { ServiceId = objServices.ServiceId, mid = mid });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    //return RedirectToAction("Index", "Services");
                    return RedirectToAction("Index", "Services", new { mid = mid });

                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                // return RedirectToAction("Index", "Services");
                return RedirectToAction("Index", "Services", new { mid = mid });
            }
        }


        [HttpPost]
    //    public IActionResult AddService(
    //Entities.Services objServices,
    //IFormFile DocumentUrl,    // ✅ replaces HttpPostedFileBase
    //IFormFile ImageFile,      // ✅ replaces WebImage.GetImageFromRequest()
    //Int64 mid = 0)
    //    {
    //        try
    //        {
    //            // ✅ Get upload path — replaces ConfigurationManager.AppSettings
    //            string uploadPath = _configuration["AppSettings:uploadPath"] ?? "";

    //            // ✅ Get document extension — replaces HttpPostedFileBase check
    //            string Document = (DocumentUrl == null || DocumentUrl.Length == 0)
    //                              ? "N/A"
    //                              : Path.GetExtension(DocumentUrl.FileName);

    //            // ✅ Get image extension — replaces WebImage.GetImageFromRequest()
    //            string imageurl = (ImageFile != null && ImageFile.Length > 0)
    //                              ? Path.GetExtension(ImageFile.FileName).TrimStart('.')
    //                              : "N/A";

    //            // ✅ Set properties
    //            objServices.UpdatedBy = HttpContext.User.Identity?.Name ?? "";
    //            objServices.UpdatedDate = AWS_AssociationToolCore.BLL.Common
    //                                        .GetTimeZoneAdjustedDateTime();
    //            objServices.IsDisplay = true;

    //            Int64 _status = _Services.InsertServices(objServices, ref Document);

    //            if (_status == 1)
    //            {
    //                // ✅ Save document — replaces DocumentUrl.SaveAs()
    //                if (Document != "N/A" && DocumentUrl != null)
    //                {
    //                    string docDir = Path.Combine(uploadPath, "Services", "Documents");
    //                    Directory.CreateDirectory(docDir);
    //                    string docPath = Path.Combine(docDir, Path.GetFileName(Document));

    //                    using (var stream = new FileStream(docPath, FileMode.Create))
    //                        DocumentUrl.CopyTo(stream);
    //                }

    //                TempData["messageType"] = "success";
    //                TempData["message"] = "Record Inserted Successfully.";
    //                return RedirectToAction("Index", "Services",
    //                    new { area = "Admin", mid = mid });
    //            }
    //            else if (_status == 2)
    //            {
    //                // ✅ Save document — replaces DocumentUrl.SaveAs()
    //                if (Document != "N/A" && DocumentUrl != null)
    //                {
    //                    string docDir = Path.Combine(uploadPath, "Services", "Documents");
    //                    Directory.CreateDirectory(docDir);
    //                    string docPath = Path.Combine(docDir, Path.GetFileName(Document));

    //                    using (var stream = new FileStream(docPath, FileMode.Create))
    //                        DocumentUrl.CopyTo(stream);
    //                }

    //                TempData["messageType"] = "success";
    //                TempData["message"] = "Changes have been Updated Successfully.";
    //                return RedirectToAction("Index", "Services",
    //                    new { area = "Admin", ServiceId = objServices.ServiceId, mid = mid });
    //            }
    //            else
    //            {
    //                TempData["messageType"] = "error";
    //                TempData["message"] = "Data Error.";
    //                return RedirectToAction("Index", "Services",
    //                    new { area = "Admin", mid = mid });
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            TempData["messageType"] = "error";
    //            TempData["message"] = ex.Message;
    //            return RedirectToAction("Index", "Services",
    //                new { area = "Admin", mid = mid });
    //        }
    //    }
    //    [Authorize]
        public ActionResult EditService(Int64 ServiceId = 0)
        {
            int _qstatus = 0;
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            Entities.Services _objServices = new Entities.Services();


            try
            {
                lstChapters = _Chapters.GetChaptersList(ref _qstatus);
                lstServiceCategories = _ServiceCategories.GetServiceCategoriesList(ref _qstatus);
                lstServiceTypes = _ServiceTypes.GetServiceTypesList(ref _qstatus);
                _objServices = _Services.GetServicesById(ServiceId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objServices = _objServices;
                    ViewBag.lstChapters = lstChapters;
                    ViewBag.lstServiceCategories = lstServiceCategories;
                    ViewBag.lstServiceTypes = lstServiceTypes;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "Services");
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Services");
            }


            return View();
        }


        [Authorize]
        public ActionResult ViewService(Int64 ServiceId = 0)
        {
            int _qstatus = 0;
            Entities.Services _objServices = new Entities.Services();
            try
            {
                _objServices = _Services.GetServicesById(ServiceId, ref _qstatus);
                if (_qstatus != 1)
                {
                    TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Services");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Services");
            }
            ViewBag.objServices = _objServices;
            return View();
        }

        [Authorize]
        public ActionResult EditServicePopup(Int64 ServiceId = 0)
        {

            string str = "";
            Entities.Services _objServices = new Entities.Services();
            try
            {
                int _qstatus = 0;
                _objServices = _Services.GetServicesById(ServiceId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objServices });
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
        [HttpPost]
        public JsonResult DeleteService(Int64 ServiceId)
        {
            try
            {

                Int64 _status = _Services.DeleteServices(ServiceId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Deleting the Donation Cause" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ServicesUpdateStatus(Int64 ServiceId)
        {
            try
            {
                Int64 _status = _Services.ServicesUpdateStatus(ServiceId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Status Updated Successfully" });

                   

                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating user status" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        public JsonResult UpdateServicesOrderNo(int DisplayOrder, Int64 ServiceId)
        {
            try
            {
                Int64 _status = _Services.UpdateServicesOrderNo(DisplayOrder, ServiceId);
                if (_status == 1)
                {                    
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Updated Order Successfully" });
                }
            }
            catch(Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });
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
