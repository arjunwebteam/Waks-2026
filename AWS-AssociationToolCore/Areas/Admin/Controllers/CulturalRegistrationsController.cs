using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml;            // ← keep only ONE
using OfficeOpenXml.Style;      // ← keep only ONE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CulturalRegistrationsController : Controller
    {
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        BLL.CulturalRegistrations _CulturalRegistrations = new BLL.CulturalRegistrations();
        DAL.CulturalRegistrations _DCulturalRegistrations = new DAL.CulturalRegistrations();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.Users _users = new BLL.Users();
        BLL.SendMail _sendmail = new BLL.SendMail();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();


        #region CulturalRegistrations


        [Authorize]

        public IActionResult Index(Int64 ChapterId = 0, long eventId = 0, Int64 mid = 0)
        {
            int status = 0;
            int Total = 0;
            int _sstatus = 0;
            Entities.UserRoles objuserroles = new Entities.UserRoles();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            List<Entities.Events> lstEvents = new List<Entities.Events>();
            AWS_AssociationToolCore.BLL.Events _Events = new AWS_AssociationToolCore.BLL.Events();

            // ✅ Get UserId from Claims — replaces Request.Cookies["UserId"]
            Int64 UserId = Convert.ToInt64(
                User.FindFirst("UserId")?.Value ?? "0");

            // ✅ Get UserRole from Claims — replaces Request.Cookies["UserRole"]
            string userRole = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value ?? "";

            // ✅ Get ChapterId from Claims — replaces Request.Cookies["chapterid"]
            if (ChapterId == 0)
                ChapterId = Convert.ToInt64(
                    User.FindFirst("ChapterId")?.Value ?? "0");

            // ✅ Set Session — replaces Session["key"] = value
            HttpContext.Session.SetString("userrole", userRole);
            HttpContext.Session.SetString("chapterid", ChapterId.ToString());

            // Get data
            lstChapters = _Chapters.GetChaptersList(ref Total);
            objuserroles = _users.GetRoleDetialsById(UserId, mid, ref status);
            lstEvents = _Events.GetEventsList(ref _sstatus);
            // ✅ Store permissions in Session
            HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

            // ✅ Pass directly to ViewBag
            ViewBag.IsEdit = objuserroles.IsEdit;
            ViewBag.IsView = objuserroles.IsView;
            ViewBag.IsDelete = objuserroles.IsDelete;
            ViewBag.IsExport = objuserroles.IsExport;
            ViewBag.IsAdd = objuserroles.IsAdd;
            ViewBag.EventId = eventId;
            ViewBag.lstEvents = lstEvents;
            ViewBag.lstChapters = lstChapters;
            ViewBag.ChapterId = ChapterId;
            ViewBag.mid = mid;

            return View();
        }
        //[Authorize]
        [HttpPost]
        public ActionResult AddCulturalRegistrations(Entities.CulturalRegistrations objCulturalRegistrations, ref Int64 CulturalRegistrationId)
        {
            try
            {
                string receipturl = "NA";
                objCulturalRegistrations.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objCulturalRegistrations.InsertedBy = HttpContext.User.Identity.Name.ToString();
                objCulturalRegistrations.IsActive = false;
                Int64 _status = _CulturalRegistrations.InsertCulturalRegistrations(objCulturalRegistrations, ref CulturalRegistrationId, ref receipturl);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "CulturalRegistrations");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "CulturalRegistrations", new { CulturalRegistrationId = objCulturalRegistrations.CulturalRegistrationId });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("AddCulturalRegistrations", "CulturalRegistrations");
            }
            return View();
        }


        public ActionResult CulturalRegistrationsList(Int64 EventId = 0, string State = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0, Int64 ChapterId = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.CulturalRegistrations> lstCulturalRegistrations = new List<Entities.CulturalRegistrations>();

            try
            {
                lstCulturalRegistrations = _CulturalRegistrations.GetCulturalRegistrationsListByVariable(State, EventId, Search, Sort, PageNo, Items, ChapterId, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstCulturalRegistrations = lstCulturalRegistrations;
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

        public ActionResult EditCulturalRegistrations(Int64 CulturalRegistrationId = 0, Int64 mid = 0)
        {
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            List<Entities.PaymentStatus> lstpaymentstatus = new List<Entities.PaymentStatus>();
            try
            {
                int _qstatus = 0;
                int status = 0;
                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);
                lstpaymentstatus = _paymentSettings.GetPaymentStatusList(ref status);
                Entities.CulturalRegistrations _objCulturalRegistrations = _CulturalRegistrations.GetCulturalRegistrationsFullDetailsById(CulturalRegistrationId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objCulturalRegistrations = _objCulturalRegistrations;
                    ViewBag.status = _qstatus;
                    ViewBag.mid = mid;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "CulturalRegistrations");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "CulturalRegistrations");
            }
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            ViewBag.lstpaymentstatus = lstpaymentstatus;
            return View();
        }
        //[Authorize]
        [HttpPost]
        [HttpPost]
        // ✅ Removed [ValidateInput(false)] — not in Core
        // ✅ Use [RequestFormLimits] if needed for large forms
        public IActionResult UpdateCulturalRegistrations(
    IFormFile file,                                          // ✅ replaces HttpPostedFileBase
    IFormFile filezelle,                                     // ✅ replaces HttpPostedFileBase
    Entities.CulturalRegistrations objCulturalRegistrations,
    string PaymentType = "",
    Int64 mid = 0)
        {
            int status = 0;

            try
            {
                // Get app info
                AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                AWS_AssociationToolCore.Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);

                string receipturl = "NA";
                string uploadPath = objAppInfo.UploadPath;

                // ──────────────────────────────────────────
                // file upload — replaces file.SaveAs + WebImage
                // ──────────────────────────────────────────
                if (file != null && file.Length > 0)
                {
                    string tempDir = Path.Combine(uploadPath, "temp");
                    Directory.CreateDirectory(tempDir);

                    string tempPath = Path.Combine(tempDir, file.FileName);

                    // ✅ Save file — replaces file.SaveAs()
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                        file.CopyTo(stream);

                    // ✅ Get extension — replaces WebImage.ImageFormat
                    receipturl = Path.GetExtension(file.FileName).TrimStart('.');
                }

                // ──────────────────────────────────────────
                // filezelle upload
                // ──────────────────────────────────────────
                if (filezelle != null && filezelle.Length > 0)
                {
                    string tempDir = Path.Combine(uploadPath, "temp");
                    Directory.CreateDirectory(tempDir);

                    string tempPath = Path.Combine(tempDir, filezelle.FileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                        filezelle.CopyTo(stream);

                    receipturl = Path.GetExtension(filezelle.FileName).TrimStart('.');
                }

                // ✅ Get logged-in user — replaces Identity.Name.ToString()
                objCulturalRegistrations.UpdatedBy = HttpContext.User.Identity?.Name ?? "";
                objCulturalRegistrations.InsertedBy = HttpContext.User.Identity?.Name ?? "";

                Int64 _status = _CulturalRegistrations
                    .UpdateCulturalRegistrations(objCulturalRegistrations, ref receipturl);

                Int64 EventId = objCulturalRegistrations.EventId;

                // Reload registration after update
                objCulturalRegistrations = _CulturalRegistrations
                    .GetCulturalRegistrationsById(
                        objCulturalRegistrations.CulturalRegistrationId, ref status);

                // ──────────────────────────────────────────
                // Move file to final location
                // ──────────────────────────────────────────
                string receiptDir = Path.Combine(uploadPath, "EventReceiptImages");
                Directory.CreateDirectory(receiptDir);

                if (file != null && file.Length > 0)
                {
                    string finalPath = Path.Combine(
                        receiptDir, objCulturalRegistrations.ReceiptUrl);

                    // ✅ replaces file.SaveAs(finalPath)
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                        file.CopyTo(stream);
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    string finalPath = Path.Combine(
                        receiptDir, objCulturalRegistrations.ReceiptUrl);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                        filezelle.CopyTo(stream);
                }

                // ──────────────────────────────────────────
                // Result handling
                // ──────────────────────────────────────────
                if (_status != -1)
                {
                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully.";
                    }
                    else if (_status == 2)
                    {
                        if (PaymentType == "Completed")
                        {
                            // ── User confirmation email ──
                            Entities.MailTemplates objMailTemplates = _MailTemplates
                                .GetMailTemplateById("Confirm Event Registration", 0, ref status);

                            if (objMailTemplates != null)
                            {
                                StringBuilder body = new StringBuilder();
                                body.Append(objMailTemplates.Description);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(
                                    objCulturalRegistrations.FirstName + " "
                                    + objCulturalRegistrations.LastName));
                                body.Replace("[Email]", objCulturalRegistrations.Email);
                                body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                body.Replace("[PaymentType]", objCulturalRegistrations.PaymentMethod);
                                body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                                body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate
                                                                            .ToString("MM/dd/yyyy"));
                                body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body.Replace("[SiteName]", objAppInfo.SiteName);

                                // ✅ Send mail with PDF attachment
                                _sendmail.SendMailwithAttachment(
                                    objCulturalRegistrations.Email,
                                    objMailTemplates.Subject,
                                    body.ToString(),
                                    objCulturalRegistrations.CulturalRegistrationId + ".pdf");
                            }

                            // ── Admin notification email ──
                            int statusAdmin = 0;
                            Entities.MailTemplates objTemplatesAdmin = _MailTemplates
                                .GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);

                            if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                            {
                                StringBuilder body1 = new StringBuilder();
                                body1.Append(objTemplatesAdmin.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[INUrl]", objAppInfo.Topline);
                                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(
                                    objCulturalRegistrations.FirstName + " "
                                    + objCulturalRegistrations.LastName));
                                body1.Replace("[Email]", objCulturalRegistrations.Email);
                                body1.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                body1.Replace("[PaymentType]", "Credit Card");
                                body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate
                                                                    .ToString("MM/dd/yyyy"));
                                body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body1.Replace("[SiteName]", objAppInfo.SiteName);

                                _sendmail.SendMailwithCC(
                                    objAppInfo.CompanyEmail,
                                    objAppInfo.CompanyEmail,
                                    objAppInfo.CompanyEmail,
                                    "Registered Cultural Event Details - " + objAppInfo.SiteName,
                                    body1.ToString());
                            }
                        }

                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes have been Updated Successfully.";
                        return RedirectToAction("Index", "CulturalRegistrations",
                            new { area = "Admin", mid = mid });
                    }
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("Index", "CulturalRegistrations",
                        new { area = "Admin", mid = mid });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed processing your request.";
                //logreport(ex.Message);  // log error
                return RedirectToAction("Index", "CulturalRegistrations",
                    new { area = "Admin", mid = mid });
            }

            return View();
        }
        #region export to excel



        public IActionResult CulturalRegistrationsExportToExcel(string search = "",string sortColumn = "UpdatedDate",string sortOrder = "DESC", long chapterId = 0)
        {
            try
            {
                string sort = !string.IsNullOrEmpty(sortColumn)
                                ? $"{sortColumn} {sortOrder}"
                                : "";

                DataTable dtUni = _DCulturalRegistrations
                                    .CulturalRegistrationsExportToExcel(search, sort, chapterId);

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("CulturalRegistration-Export");

                    // Load DataTable headers
                    for (int col = 0; col < dtUni.Columns.Count; col++)
                    {
                        worksheet.Cells[1, col + 1].Value = dtUni.Columns[col].ColumnName;

                        // Style header row
                        worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, col + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, col + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 216, 230));
                    }

                    // Load DataTable rows
                    for (int row = 0; row < dtUni.Rows.Count; row++)
                    {
                        for (int col = 0; col < dtUni.Columns.Count; col++)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = dtUni.Rows[row][col];
                        }
                    }

                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    var content = package.GetAsByteArray();
                    string fileName = $"CulturalRegistration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                // Log the actual error
                TempData["message"] = $"Export failed: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        #endregion


        [HttpPost]
        public JsonResult DeleteCulturalRegistrations(Int64 CulturalRegistrationId)
        {
            //string str = "";
            try
            {

                Int64 _status = _CulturalRegistrations.DeleteCulturalRegistrations(CulturalRegistrationId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                    // return Json(new { ok = true, data = str });
                }
                else
                {
                    return Json(new { ok = false, messageType = "warning", message = "Failed deleting page" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
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