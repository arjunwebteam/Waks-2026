using AWS_AssociationToolCore.Areas.Admin.Models;
using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.DAL;
using AWS_AssociationToolCore.Entities;
using AWS_AssociationToolCore.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Grpc.Core;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml;
using OfficeOpenXml;
using QRCoder;
//using QRCoder.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Font = System.Drawing.Font;
using Roles = AWS_AssociationToolCore.Entities.Roles;
using Users = AWS_AssociationToolCore.BLL.Users;




namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{

    [Area("Admin")]

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Events,SubAdmin,ChapterAdmin,SiteAdmin,Administrator,Treasurer,ProgramsManager,DeveloperAdmin,")]
    public class EventRegistrationsController : Controller
    {
        BLL.Events _Events = new BLL.Events();
        DAL.Events _DEvents = new DAL.Events();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        List<Entities.EventUserInfo> lstEventUserInfo = new List<Entities.EventUserInfo>();
        List<Entities.EventRegistrationCounts> lstRegistrationCount = new List<Entities.EventRegistrationCounts>();
        Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
        BLL.Members _Members = new BLL.Members();
        Entities.Members objMembers = new Entities.Members();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();
        AWS_AssociationToolCore.BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();

        BLL.AppInfo _AppInfo = new BLL.AppInfo();
        List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
        BLL.EventCompetitions _EventCompetitions = new BLL.EventCompetitions();
        List<Entities.EventCompetitions> lstEventCompetitions = new List<Entities.EventCompetitions>();
        BLL.Users _users = new BLL.Users();
        BLL.SendMail _sendmail = new BLL.SendMail();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        // ✅ Fix the constructor
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public EventRegistrationsController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _env = env;
            // ... other injections
        }
        [Authorize]
        public IActionResult Index(long eventId = 0, string eventName = "", long mid = 0, long ChapterId = 0)
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
            int qStatus = 0;
            int CategoryLevel = 0;

            try
            {
                //lstMenuItems = _MenuItems.GetMenuItemsByLevel(CategoryLevel, ref status);
                //lstPageDetails = _PageDetails.GetPageDetailsList(ref status);
                lstChapters = _Chapters.GetChaptersList(ref status);
                var lstEventRegistrationTypes = _Events.GetEventRegistrationTypesList(eventId, ref qStatus);
            
                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);

                // ✅ Session (Core way)
                HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
                HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
                HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
                HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
                HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

                // ✅ ViewBag (optional)
                ViewBag.EventId = eventId;
                ViewBag.EventName = eventName;
                ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
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
            //ViewBag.IsFooterBar = IsFooterBar;
            //ViewBag.IsMenuBar = IsMenuBar;
            //ViewBag.IsQuickLinks = IsQuickLinks;

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
           

            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult EventUsersList(Int64 EventId = 0, string EventCategory = "", string Search = "", string SortColumn = "InsertedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {
                lstEventUserInfo = _Events.GetEventUserInfoListByVariable(EventId, EventCategory, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstEventUserInfo = lstEventUserInfo;
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

        public ActionResult Registrations()
        {
            int Status = 0;
            List<Entities.Events> lstEvents = _Events.RegistrationGetEventsList(ref Status);
            ViewBag.lstEvents = lstEvents;
            return View();
        }
        public ActionResult AddRegistrations(Int64 EventId = 0, string EventName = "", Int64 mid = 0, Int64 ChapterId = 0)
        {
            int qstatus = 0;
            int Status = 0; int status = 0;
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            List<Entities.Events> lstEvents = new List<Entities.Events>();
            BLL.Events _Events = new BLL.Events();
            try
            {
                lstEventRegistrationTypes = _Events.GetEventRegistrationTypesList(EventId, ref qstatus);

                objMembers = _Members.AddMembershipRequirement(ref qstatus);
                lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref status);

                lstEvents = _Events.RegistrationGetEventsList(ref Status);

                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstEvents = lstEvents;
            ViewBag.EventId = EventId;
            ViewBag.EventName = EventName;

            ViewBag.objMembers = objMembers;
            ViewBag.lstEventCompetitions = lstEventCompetitions;
            ViewBag.status = qstatus;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;

            ViewBag.lstEventCompetitions = lstEventCompetitions;
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            ViewBag.mid = mid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRegistrations(
    IFormFile file,
    IFormFile filezelle,
    Entities.EventUserInfo objEventUserInfo,
    List<Entities.EventParticipants> lstEventParticipants,
    List<Entities.EventRegistrationCounts> lstEventRegistrationCounts,
    string paymentType = "",
    string type = "",
    long mid = 0)
        {
            Entities.Events objEvents = new Entities.Events();
            var lstEventsTickets = new List<Entities.EventsTickets>();
            var objEventUserInfo1 = new Entities.EventUserInfo();

            int result1 = 0;
            int status = 0;

            try
            {
                var objAppInfo = _AppInfo.GetAppInfoDetails(ref status);

                string receipturl = "NA";

                // ✅ File Upload (Core way)
                if (file != null && file.Length > 0)
                {
                    var tempPath = Path.Combine(objAppInfo.UploadPath, "temp", file.FileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    receipturl = Path.GetExtension(file.FileName);
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    var tempPath = Path.Combine(objAppInfo.UploadPath, "temp", filezelle.FileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await filezelle.CopyToAsync(stream);
                    }

                    receipturl = Path.GetExtension(filezelle.FileName);
                }

                // ✅ XML conversion
                if (lstEventRegistrationCounts != null &&
                    lstEventRegistrationCounts.Count > 0 &&
                    lstEventRegistrationCounts[0].EventRegistrationTypeId != 0)
                {
                    objEventUserInfo.XMLRegistrationsCounts =
                        BLL.Common.CreateXMLForObject(lstEventRegistrationCounts);
                }

                // ✅ User + Date info
                var userName = User.Identity?.Name ?? "System";

                objEventUserInfo.UpdatedBy = userName;
                objEventUserInfo.InsertedBy = userName;
                objEventUserInfo.UpdatedTime = BLL.Common.GetTimeZoneAdjustedDateTime();
                objEventUserInfo.InsertedTime = BLL.Common.GetTimeZoneAdjustedDateTime();
                objEventUserInfo.InsertedDate = BLL.Common.GetTimeZoneAdjustedDateTime();

                long _status = _Events.InsertEventUserInfo(objEventUserInfo, ref receipturl);

                long eventId = objEventUserInfo.EventId;

                var lstEventRegTickets = new List<Entities.EventRegTickets>();

                objEventUserInfo1 = _Events.GetEventUserInfoFullDetailsById(
                    objEventUserInfo.EventUserInfoId,
                    eventId,
                    ref objEvents,
                    ref lstEventRegistrationCounts,
                    ref lstEventsTickets,
                    ref lstEventRegTickets,
                    ref result1
                );

                // ✅ Save final file
                if (file != null)
                {
                    var finalPath = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages", objEventUserInfo1.ReceiptUrl);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                if (filezelle != null)
                {
                    var finalPath = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages", objEventUserInfo1.ReceiptUrl);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await filezelle.CopyToAsync(stream);
                    }
                }

                // ✅ SUCCESS / UPDATE LOGIC
                if (_status != -1)
                {
                    if (_status == 1)
                    {
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully.</div>";
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = "Record Inserted Successfully";
                    }
                    else if (_status == 2)
                    {
                        if (paymentType == "Completed")
                        {
                            // var attachment = GetAttachment(objEventUserInfo1, objEvents, lstEventsTickets);

                            Entities.MailTemplates objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);
                            if (objMailTemplates != null)
                            {
                                StringBuilder body = new StringBuilder();

                                body.Append(objMailTemplates.Description);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[EUrl]", objAppInfo.SupportEmail);
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[InUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo1.FirstName + " " + objEventUserInfo1.LastName));
                                body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo1.EventName));
                                body.Replace("[TransactionId]", objEventUserInfo1.objEventOrderDetails.TransactionId);
                                body.Replace("[PaymentType]", objEventUserInfo1.objEventOrderDetails.PaymentMethod);
                                body.Replace("[PaymentStatus]", objEventUserInfo1.objEventOrderDetails.PaymentStatus);
                                body.Replace("[PaymentDate]", DateTime.UtcNow.ToString());
                                body.Replace("[AmountPaid]", objEventUserInfo1.objEventOrderDetails.AmountPaid.ToString());
                                body.Replace("[SiteName]", objAppInfo.SiteName);

                                //BLL.Common.SendMailwithAttachment(objEventUserInfo.Email, objMailTemplates.Subject, body.ToString(), attachment);
                                _sendmail.SendMailwithAttachment(objEventUserInfo1.Email, objMailTemplates.Subject, body.ToString(), "tickets-" + BLL.Common.EncodeURL(objEventUserInfo1.EventName) + "-" + objEventUserInfo1.EventUserInfoId + ".pdf");
                            }

                            int statusAdmin = 0;
                            Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
                            if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                            {

                                StringBuilder body1 = new StringBuilder();
                                body1.Append(objTemplatesAdmin.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[EUrl]", objAppInfo.SupportEmail);
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[InUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo1.FirstName + " " + objEventUserInfo1.LastName));
                                body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo1.EventName));
                                body1.Replace("[Email]", objEventUserInfo1.Email);
                                body1.Replace("[TransactionId]", objEventUserInfo1.objEventOrderDetails.TransactionId);
                                body1.Replace("[PaymentType]", objEventUserInfo1.objEventOrderDetails.PaymentMethod);
                                body1.Replace("[PaymentStatus]", objEventUserInfo1.objEventOrderDetails.PaymentStatus);
                                body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString());
                                body1.Replace("[AmountPaid]", objEventUserInfo1.objEventOrderDetails.AmountPaid.ToString());

                                _sendmail.SendMailwithCC(objAppInfo.CompanyEmail, objAppInfo.CompanyEmail, objAppInfo.CompanyEmail, "Registered Event Details - " + objAppInfo.SiteName, body1.ToString());
                            }

                        }
                        //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                        TempData["messageType"] = "success"; // Assuming "info" for update
                        TempData["message"] = "Changes has been Updated Successfully";
                    }

                    // ✅ Redirects
                    if (type == "Ticket Master")
                    {
                        return RedirectToAction("TicketMaster", "EventRegistrations", new { eid = objEvents.EventId, ename = objEvents.EventName, tid = objEventUserInfo1.objEventOrderDetails.TransactionId });

                    }
                    else
                    {
                        return RedirectToAction("Index", "EventRegistrations", new { EventId = objEventUserInfo.EventId, ename = objEvents.EventName, mid = mid });

                    }

                }

                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";

                    if (type == "Ticket Master")
                    {
                        return RedirectToAction("TicketMaster", "EventRegistrations", new { eid = objEvents.EventId, ename = objEvents.EventName, tid = objEventUserInfo1.TransactionId });
                    }
                    else
                    {
                        return RedirectToAction("EditEventUser", "EventRegistrations", new { EventUserInfoId = objEventUserInfo.EventUserInfoId, mid = mid });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed processing your request.";

                if (type == "Ticket Master")
                {
                    return RedirectToAction("TicketMaster", "EventRegistrations", new { eid = objEvents.EventId, ename = objEvents.EventName, tid = objEventUserInfo1.TransactionId });
                }
                else
                {
                    return RedirectToAction("AddRegistrations", "EventRegistrations", new { eid = objEvents.EventId, ename = objEvents.EventName, mid = mid });
                }
            }

        }

        public ActionResult RegistrationsEventUsersList(Int64 EventId = 0, string EventCategory = "Cultural Events", string Search = "", string SortColumn = "InsertedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {
                lstEventUserInfo = _Events.RegistrationGetEventUserInfoListByVariable(EventId, EventCategory, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstEventUserInfo = lstEventUserInfo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }


        public ActionResult EditEventUser(Int64 EventUserInfoId = 0, Int64 mid = 0)
        {
            Int64 EventId = 0;
            try
            {
                int qstatus = 0;
                int status = 0;
                objMembers = _Members.AddMembershipRequirement(ref qstatus);
                lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref status);

                if (qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.lstEventCompetitions = lstEventCompetitions;
                    ViewBag.status = qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("Index", "EventRegistrations");
                }

                int _qstatus = 0;
                List<Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                Entities.Events objEvents = new Entities.Events();
                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();

                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, EventId, ref objEvents, ref lstEventRegistrationCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);
                if (_qstatus == 1)
                {
                    ViewBag.lstEventRegistrationCounts = lstEventRegistrationCounts;
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventRegTickets = lstEventRegTickets;
                    ViewBag.status = _qstatus;
                    ViewBag.mid = mid;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("Index", "EventRegistrations");
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed processing your request.";
                return RedirectToAction("Index", "EventRegistrations");
            }
            return View();
        }
        public ActionResult EventRegistration(string EventName = "", Int64 MemberId = 0, Int64 eid = 0, string cname = "")
        {
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            int status = 0;
            try
            {
                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.MemberId = MemberId;

            ViewBag.EventName = EventName;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            ViewBag.cname = cname;
            ViewBag.lstEventCompetitions = lstEventCompetitions;
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            return View();
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdateEventUser(
    IFormFile file,
    IFormFile filezelle,
    Entities.EventUserInfo objEventUserInfo,
    string paymentType = "",
    string type = "",
    long mid = 0)
        {
            var objEvents = new Entities.Events();
            var lstEventsTickets = new List<Entities.EventsTickets>();
            var lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
            var objEventUserInfo1 = new Entities.EventUserInfo();

            int result1 = 0;
            int status = 0;

            try
            {
                var objAppInfo = _AppInfo.GetAppInfoDetails(ref status);

                string receipturl = "NA";

                // ✅ Upload temp files
                if (file != null && file.Length > 0)
                {
                    var tempPath = Path.Combine(objAppInfo.UploadPath, "temp", file.FileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    receipturl = Path.GetExtension(file.FileName);
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    var tempPath = Path.Combine(objAppInfo.UploadPath, "temp", filezelle.FileName);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await filezelle.CopyToAsync(stream);
                    }

                    receipturl = Path.GetExtension(filezelle.FileName);
                }

                // ✅ User info
                var userName = User.Identity?.Name ?? "System";

                objEventUserInfo.UpdatedBy = userName;
                objEventUserInfo.InsertedBy = userName;
                long _status = _Events.UpdateEventUserInfo(objEventUserInfo, ref receipturl);

                var lstEventRegTickets = new List<Entities.EventRegTickets>();

                long eventId = objEventUserInfo.EventId;

                objEventUserInfo1 = _Events.GetEventUserInfoFullDetailsById(
                    objEventUserInfo.EventUserInfoId,
                    eventId,
                    ref objEvents,
                    ref lstEventRegistrationCounts,
                    ref lstEventsTickets,
                    ref lstEventRegTickets,
                    ref result1
                );

                // ✅ Save final files
                if (file != null)
                {
                    var finalPath = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages", objEventUserInfo1.ReceiptUrl);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                if (filezelle != null)
                {
                    var finalPath = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages", objEventUserInfo1.ReceiptUrl);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await filezelle.CopyToAsync(stream);
                    }
                }

                // ✅ Success handling
                if (_status != -1)
                {
                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully";
                    }
                    else if (_status == 2)
                    {
                        if (paymentType == "Completed")
                        {
                            int mailStatus = 0;

                            var template = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref mailStatus);

                            if (template != null)
                            {
                                var body = new StringBuilder(template.Description);

                                body.Replace("[USERNAME]", objEventUserInfo1.FirstName + " " + objEventUserInfo1.LastName);
                                body.Replace("[EventName]", objEventUserInfo1.EventName);
                                body.Replace("[TransactionId]", objEventUserInfo1.objEventOrderDetails.TransactionId);
                                body.Replace("[PaymentType]", objEventUserInfo1.objEventOrderDetails.PaymentMethod);
                                body.Replace("[PaymentStatus]", objEventUserInfo1.objEventOrderDetails.PaymentStatus);
                                body.Replace("[AmountPaid]", objEventUserInfo1.objEventOrderDetails.AmountPaid.ToString());

                                _sendmail.SendMailwithAttachment(
                                   objEventUserInfo1.Email,
                                   template.Subject,
                                   body.ToString(),
                                   $"tickets-{objEventUserInfo1.EventUserInfoId}.pdf"
                               );
                            }

                            int statusAdmin = 0;
                            var adminTemplate = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);

                            if (adminTemplate != null && adminTemplate.MailTemplateId != 0)
                            {
                                var body1 = new StringBuilder(adminTemplate.Description);

                                body1.Replace("[USERNAME]", objEventUserInfo1.FirstName + " " + objEventUserInfo1.LastName);
                                body1.Replace("[EventName]", objEventUserInfo1.EventName);
                                body1.Replace("[Email]", objEventUserInfo1.Email);
                                body1.Replace("[TransactionId]", objEventUserInfo1.objEventOrderDetails.TransactionId);

                               // _sendmail.SendMailwithCC(
                               //    objAppInfo.CompanyEmail,
                               //    objAppInfo.CompanyEmail,
                               //    objAppInfo.CompanyEmail,
                               //    "Registered Event Details",
                               //    body1.ToString()
                               //);
                            }
                        }

                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes updated successfully";
                    }

                    // ✅ Redirects
                    if (type == "Ticket Master")
                    {
                        return RedirectToAction("TicketMaster", "EventRegistrations",
                            new
                            {
                                eid = objEvents.EventId,
                                ename = objEvents.EventName,
                                tid = objEventUserInfo1.objEventOrderDetails.TransactionId
                            });
                    }

                    return RedirectToAction("Index", "EventRegistrations",
                        new { EventId = objEventUserInfo.EventId, mid });
                }

                // ❌ Failure
                TempData["messageType"] = "error";
                TempData["message"] = "Failed processing your request.";

                return RedirectToAction("EditEventUser", "EventRegistrations",
                    new { EventUserInfoId = objEventUserInfo.EventUserInfoId, mid });
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed processing your request.";

                return RedirectToAction("EditEventUser", "EventRegistrations",
                    new { EventUserInfoId = objEventUserInfo.EventUserInfoId, mid });
            }
        }
        public ActionResult ViewEventUser(Int64 EventUserInfoId = 0)
        {
            Int64 EventId = 0;
            try
            {

                int _qstatus = 0;
                int status = 0;
                List<Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                Entities.Events objEvents = new Entities.Events();
                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();

                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, EventId, ref objEvents, ref lstEventRegistrationCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);
                lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref status);

                if (_qstatus == 1)
                {
                    ViewBag.lstEventRegistrationCounts = lstEventRegistrationCounts;
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventCompetitions = lstEventCompetitions;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("Index", "EventRegistrations");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "EventRegistrations");
            }
            return View();
        }


        [HttpPost]
        public JsonResult DeleteEventUser(Int64 EventUserInfoId)
        {

            try
            {

                Int64 _status = _Events.DeleteEventUserInfoById(EventUserInfoId);
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

        public JsonResult EventUserInfoStatus(Int64 EventUserInfoId)
        {
            string str = "";
            try
            {
                Int64 _status = _Events.UpdateEventUserInfoStatus(EventUserInfoId);
                int status = 0;
                Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref status);
                if (_status == 1)
                {
                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);
                    if (objTemplates != null)
                    {
                        Entities.EventUserInfo objEventUserInfo = _Events.GetEventUserInfoById(EventUserInfoId, ref status);
                        StringBuilder body = new StringBuilder();
                        if (objEventUserInfo.IsApproved == true)
                        {
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objappinfo.BaseUrl);
                            body.Replace("[FBUrl]", objappinfo.FacebookUrl);
                            body.Replace("[TWUrl]", objappinfo.TwitterUrl);
                            body.Replace("[YUrl]", objappinfo.YoutubeUrl);
                            body.Replace("[TPhoneNo]", objappinfo.CompanyPhone);
                            body.Replace("[TEmailId]", objappinfo.CompanyEmail);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + "" + objEventUserInfo.LastName));
                            body.Replace("[EVENTNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                            // _sentmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body.ToString());
                        }
                    }
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
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

        #region export to excel

        [HttpGet]
        #endregion

        #region EventTicketMaster

        [HttpGet]
        public IActionResult TicketMaster(long eid = 0, string ename = "", string tid = "")
        {
            // Check session for username
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            {
                // Get current absolute URL
                var returnUrl = UriHelper.BuildAbsolute(
                    HttpContext.Request.Scheme,
                    HttpContext.Request.Host,
                    HttpContext.Request.Path,
                    HttpContext.Request.QueryString.HasValue ? HttpContext.Request.QueryString.Value : string.Empty
                );

                // Redirect to login page with returnUrl
                return Redirect($"/admin-login.html?returnurl={BLL.Common.Encode(returnUrl)}");
            }

            ViewBag.ename = ename;
            ViewBag.eid = eid;
            ViewBag.tid = tid;

            return View();
        }
        public ActionResult EventTicketMasterList(Int64 EventId = 0, string Mobile = "", string Search = "", string SortColumn = "EventRegCountId", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            int QStatus = 0;
            Int64 EventUserInfoId = 0;
            Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();

            Entities.Members objMembers = new Entities.Members();

            List<Entities.PaymentStatus> lstPaymentStatus = new List<Entities.PaymentStatus>();
            try
            {

                lstRegistrationCount = _Events.EventTicketMasterGetListByVariable(EventId, Mobile, Search, Sort, PageNo, Items, ref Total);

                int _qstatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);

                objEventUserInfo = _Events.GetEventUserInfoFullDetailsByMobile(EventUserInfoId, EventId, Mobile, Search, ref QStatus);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstRegistrationCount = lstRegistrationCount;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.objEventUserInfo = objEventUserInfo;
            ViewBag.objMembers = objMembers;
            return View();
        }

        [HttpPost]
        public JsonResult EventTicketMasterVisitCountUpdate(int VisitCount, Int64 EventRegCountId)
        {
            string str = "";
            try
            {
                Int64 _status = _Events.EventTicketMasterVisitCountUpdate(VisitCount, EventRegCountId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Visit Count Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating Visit Count</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        #endregion



        [Authorize]
        public JsonResult ResendQRCode(Int64 EventUserInfoId, string type = "")
        {

            string str = "";
            string QrUri = "";
            int _status = 0;
            int status = 0;
            //string RefCode = "TATH" + EventUserInfoId;
            Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
            //string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
            try
            {

                if (EventUserInfoId != 0)
                {

                    str = "<div class=\"alert alert-success alert-dismissable\">QR Code sent successfully to Registered Email-Id</div>";
                    if (type == "Event")
                    {
                        int _qstatus = 0;
                        List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                        Entities.Events objEvents = new Entities.Events();
                        Entities.EventUserInfo objUser = new Entities.EventUserInfo();
                        List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref status);

                        Entities.EventUserInfo objEventUserInfo = _Events.GetEventUserInfoById(EventUserInfoId, ref _status);
                        List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();

                        //Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
                        objUser = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);



                        string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                                      "<tr>" +
                                      "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                                      "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                                      "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                                      "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>" +
                                      
                                      "</tr>";
                        if (lstEventRegCounts != null && lstEventRegCounts.Count != 0)
                        {
                            int i = 1;

                            foreach (var item in lstEventRegCounts)
                            {
                                tickets += "<tr>" +
                                           "<td align=\"center\">" + i + "</td>" +
                                           "<td align=\"center\">" + item.RegistrationTitle + "</td>" +
                                           "<td align=\"center\">" + item.Count + "</td>" +
                                           "<td align=\"center\">$" + item.Amount + "</td>" +
                                         
                                           "</tr>";

                                i++;
                            }
                        }

                        tickets += "</table>";


                        // ============================================================
                        // PDF TICKET GENERATION - START
                        // ============================================================

                        // Step 1: Generate QR Codes for all tickets

                        // GenerateQRCode(EventUserInfoId, EventId, ref lstEventRegTickets);


                        if (objAppInfo.IsQRCode == "Yes")
                        {
                            GenerateQRCodepdf(EventUserInfoId, objEventUserInfo.EventId, ref lstEventRegTickets);
                        }

                        // Step 2: Create list to hold individual ticket PDFs
                        List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

                        // Step 3: Generate PDF for each ticket
                        foreach (var ticket in lstEventRegTickets)
                        {
                            try
                            {
                                // Prepare batch/category info
                                string categoryInfo = "";
                                if (!string.IsNullOrEmpty(ticket.RegistrationTitle))
                                {
                                    // You can customize this based on your batch logic
                                    // Example: "Batch 3 at 08:00 PM – 8:20 PM (1 Slot Per Family)"
                                    categoryInfo = ticket.RegistrationTitle;
                                }

                                // Create view model with ticket data
                                var ticketViewModel = new AWS_AssociationToolCore.Models.TicketPdfViewModel
                                {
                                    // Event Information
                                    EventName = objEventUserInfo.EventName,
                                    EventId = objEventUserInfo.EventId,
                                    Location = objEventUserInfo.Location,
                                    StartDate = objEventUserInfo.StartDate,
                                    EventDescription = "Event Registration",  // Or use your field
                                    LogoUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/donation-middle-img.png",  // Your logo URL
                                    CalendarUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/calendar.png",  // Your Calendar URL
                                    LocationUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/location.png",  // Your Location URL
                                    HOLDERUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/user.png",  // Your HOLDER URL
                                    TicketDetailsUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/voucher.png",  // Your TicketDetails URL
                                    routeUrl = $"{objAppInfo.UserSiteUrl}Content/User/images/route.png",  // Your route URL

                                    // Ticket Information
                                    TicketId = ticket.TicketId,
                                    TicketNumber = ticket.EventRegTicketId.ToString(),
                                    RegistrationTitle = ticket.RegistrationTitle,
                                    Category = categoryInfo,  // This will show in the batch info section

                                    // User Information
                                    SponsorName = $"{objEventUserInfo.FirstName} {objEventUserInfo.LastName}",
                                    Email = objEventUserInfo.Email,
                                    Phone = objEventUserInfo.Mobile,

                                    // Payment Information
                                    TransactionId = objEventUserInfo.TransactionId ?? "N/A",
                                    AmountPaid = objEventUserInfo.AmountPaid,
                                    PaymentDate = objEventUserInfo.PaymentDate,
                                    PaymentMethod = objEventUserInfo.PaymentMethod,
                                    PaymentStatus = objEventUserInfo.PaymentStatus ?? "Paid",

                                    // QR Code URL - matches your format
                                    QRCodeUrl = $"{objAppInfo.UserSiteUrl}Content/QRCodeGeneration/QR-of-Aws Association-Ticket-{ticket.EventRegTicketId}.jpg",

                                    // Organization Information
                                    SiteName = objAppInfo.SiteName,
                                    CompanyAddress = objAppInfo.CompanyAddress,
                                    CompanyPhone = objAppInfo.CompanyPhone,
                                    CompanyEmail = objAppInfo.CompanyEmail
                                };

                                ticketViewModels.Add(ticketViewModel);
                                // Log success for this ticket

                            }
                            catch (Exception pdfEx)
                            {
                                // Log error but continue with other tickets

                            }
                        }

                        // Step 4: Merge all ticket PDFs into one document
                        byte[] finalPdf = null;
                        string pdfFileName = "";
                        string pdfFilePath = "";
                        if (ticketViewModels.Count > 0)
                        {
                            try
                            {
                                // Generate PDF with 3 tickets per page
                                finalPdf = AWS_AssociationToolCore.Helpers.PdfGenerationHelper.GeneratePdfBytesFromViewMultipleTickets(
                                    this,
                                    "TicketPdfTemplate",  // New template
                                    ticketViewModels, _env
                                );

                                // Step 5: Save merged PDF to server folder
                                string pdfFolderPath = objAppInfo.UploadPath + "/GeneratedTickets";

                                // Create folder if doesn't exist
                                if (!Directory.Exists(pdfFolderPath))
                                {
                                    Directory.CreateDirectory(pdfFolderPath);
                                }

                                // Generate filename
                                pdfFileName = $"EventTickets-Reg-{EventUserInfoId}.pdf";
                                pdfFilePath = System.IO.Path.Combine(pdfFolderPath, pdfFileName);

                                // Save PDF to file
                                System.IO.File.WriteAllBytes(pdfFilePath, finalPdf);


                            }
                            catch (Exception pdfEx)
                            {
                                // Log PDF generation error

                            }
                        }

                        // ============================================================
                        // PDF TICKET GENERATION - END
                        // ============================================================





                        //// Load template
                        //string htmlTemplate = LoadTicketTemplate();

                        //List<byte[]> ticketPdfs = new List<byte[]>();
                        //int ticketNo = 1;

                        //DateTime eventDate = objEvents.StartDate;

                        //// Extract parts
                        //string month = eventDate.ToString("MMM").ToUpper();   // JAN
                        //string year = eventDate.Year.ToString();              // 2025
                        //string day = eventDate.Day.ToString();                // 19
                        //string suffix = GetDaySuffix(eventDate.Day);          // TH
                        //string time = eventDate.ToString("hh:mm tt");


                        //foreach (var participant in lstEventRegTickets)
                        //{
                        //    string ticketRefCode = objAppInfo.SiteName + "-Ticket-" + participant.EventRegTicketId;
                        //    string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + ticketRefCode + ".jpg";

                        //    // Build the <img> tag
                        //    var image1 = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";

                        //    string filledHtml = htmlTemplate
                        //        .Replace("[SiteName]", objAppInfo.SiteName)
                        //        .Replace("[CompanyAddress]", objAppInfo.CompanyAddress)
                        //        .Replace("[EventName]", objEvents.EventName)
                        //        .Replace("[Location]", objEvents.Location)
                        //        .Replace("[MONTH]", month)
                        //        .Replace("[YEAR]", year)
                        //        .Replace("[DAY]", day)
                        //        .Replace("[DAY_SUFFIX]", suffix)
                        //        .Replace("[TIME]", time)
                        //        .Replace("[SPONSOR_NAME]", objEventUserInfo.FirstName + " " + objEventUserInfo.LastName)
                        //        .Replace("[CATEGORY]", participant.RegistrationTitle) // or dynamic value
                        //        .Replace("[TICKET_NUMBER]", participant.TicketId)
                        //        .Replace("[QRCode]", image1);


                        //    string fontDir = Server.MapPath("~/fonts");  // if MVC

                        //    byte[] pdfBytes = GenerateTicketPdf_iText7(filledHtml, fontDir);


                        //    ticketPdfs.Add(pdfBytes);
                        //    ticketNo++;
                        //}

                        //// Merge PDFs
                        //byte[] finalPdf = MergePdfs(ticketPdfs);


                        if (objTemplates != null)
                        {

                            StringBuilder body1 = new StringBuilder();
                            if (objEventUserInfo.IsApproved == true)
                            {
                                body1.Append(objTemplates.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmailId]", objAppInfo.CompanyEmail);
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + "" + objEventUserInfo.LastName));
                                body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                body1 = body1.Replace(
     "[TransactionId]",
     string.IsNullOrWhiteSpace(objEventUserInfo.TransactionId) ? "N/A" : objEventUserInfo.TransactionId
 );
                                body1.Replace("[PaymentType]", objEventUserInfo.PaymentMethod);
                                body1.Replace("[PaymentStatus]", objEventUserInfo.PaymentStatus);
                                // ✅ Safe PaymentDate replacement
                                string paymentDate = objEventUserInfo.PaymentDate == DateTime.MinValue
                         ? "N/A"
                         : objEventUserInfo.PaymentDate.ToString("MM/dd/yyyy");

                                body1 = body1.Replace("[PaymentDate]", paymentDate);
                                decimal totalamout = 0;

                                if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                {
                                    totalamout = lstEventRegCounts.Sum(x => x.Amount);
                                }

                                // If value0, pass default value
                                body1.Replace("[TotalRegistrationAmount]",
                                    totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                                // Early Bird Amount Calculation
                                decimal totalEarlyBird = 0;

                                if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                {
                                    totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                                }

                                // If value is 0, pass default value
                                body1.Replace("[EarlyBirdAmount]",
                                    totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");

                                body1.Replace("[AmountPaid]", objEventUserInfo.AmountPaid.ToString());
                                body1.Replace("[Email]", objEventUserInfo.Email);
                                body1 = body1.Replace("[PromoDiscount]",string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A - " : objEventUserInfo.Gender);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                body1.Replace("[Tickets]", tickets);
                                //string url = ConfigurationManager.AppSettings["usersiteurl"].ToString() + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                //var image = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
                                //body1.Replace("[QRCode]", image);


                                //string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                //                 var image = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
                                //                     body1.Replace("[QRCode]", image);




                                //_sendmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body1.ToString());

                                if (objAppInfo.IsQRCode == "Yes")
                                {
                                    if (objUser.IsQRCode == "Single")
                                    {


                                        string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                                        string RefCode = cleanSiteName + EventUserInfoId;
                                        string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                            var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                                            body1 = body1.Replace("[QRCode]", image1);  // ✅ Assigned back
                                        
                                        //// Instead of phone+id, use something shorter
                                        //string RefCode = "EVT" + EventUserInfoId;
                                        //string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";

                                        ////string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
                                        ////string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                        //var image1 = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
                                        //body1.Replace("[QRCode]", image1);
                                    }
                                    else if (objUser.IsQRCode == "Multiple")
                                    {
                                        string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(EventUserInfoId.ToString());
                                        var button = "<a href='" + url + "' target='_blank' " +
                                                     "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                                     "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                                     "Event Tickets QR Codes</a>";

                                        body1 = body1.Replace("[QRCode]", button);
                                    }
                                }


                                _sendmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body1.ToString());
                            }
                        }
                    }
                    // return Json(new { ok = true, data = str });
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "QR Code sent successfully to Registered Email-Id"
                    });
                }
                else
                {

                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed sending QR Code"
                    });

                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed sending QR Code</div>";
                    //return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed"
                });
                //str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                //return Json(new { ok = true, data = str });
            }
        }
        public void GenerateQRCodepdf(long EventUserInfoId, long EventId, ref List<Entities.EventRegTickets> lstEventRegTickets)
        {
            int status1 = 0;
            AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();

            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            // ✅ Clean SiteName - remove spaces
            string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");

            // === Overall Registration QR ===
            string RefCode = cleanSiteName + EventUserInfoId;
            string overallUrl = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;
            SaveQRCodeImage(overallUrl, objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + RefCode + ".jpg");

            // === Individual Tickets QR ===
            if (lstEventRegTickets != null && lstEventRegTickets.Any())
            {
                foreach (var ticket in lstEventRegTickets)
                {
                    string ticketUrl = objAppInfo.UserSiteUrl + "ticket-preview?tktid=" + ticket.EventRegTicketId + "&euid=" + EventUserInfoId + "&uid=" + objEventUserInfo.EventId;

                    // ✅ Clean SiteName in ticket RefCode too
                    string ticketRefCode = cleanSiteName + "-Ticket-" + ticket.EventRegTicketId;
                    string savePath = objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + ticketRefCode + ".jpg";
                    SaveQRCodeImage(ticketUrl, savePath);
                }
            }
        }

        private string LoadTicketTemplate()
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);
            string templatePath = objAppInfo.UploadPath + "/Templates/ticket.html";  // ✅ removed stray 'D'
            return System.IO.File.ReadAllText(templatePath);
        }

        private void SaveQRCodeImage(string payload, string savePath)
        {
            try
            {
                // ✅ Ensure directory exists before saving
                string directory = Path.GetDirectoryName(savePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // ✅ Generate QR Code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    // ✅ Determine format from file extension
                    System.Drawing.Imaging.ImageFormat format = Path.GetExtension(savePath).ToLower() switch
                    {
                        ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                        ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                        _ => System.Drawing.Imaging.ImageFormat.Png  // default
                    };

                    bitMap.Save(savePath, format);
                }
            }
            catch (Exception ex)
            {
                // ✅ Log or rethrow as needed
                throw new Exception($"Failed to save QR code image to '{savePath}': {ex.Message}", ex);
            }
        }
        public void GenerateQRCode(Int64 EventUserInfoId)
        {
            int status1 = 0;
            BLL.AppInfo _appinfo = new BLL.AppInfo();

            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
            string code = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;
            string strB = objAppInfo.UploadPath + "/QRCodeGeneration/QR-of-" + RefCode + ".jpg";

            string samplePath = objAppInfo.UploadPath + "/QRCodeGeneration/Qrcodesample.png";

            if (!System.IO.File.Exists(samplePath))
            {
                // File not found - just save QR directly without background
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    qrBitmap.Save(strB, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                return;
            }

            // Original code below - only runs if file exists
            QRCodeGenerator qrGenerator2 = new QRCodeGenerator();
            QRCodeData qrCodeData2 = qrGenerator2.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode2 = new QRCode(qrCodeData2);

            using (Bitmap qrBitmap = qrCode2.GetGraphic(20))
            {
                using (Bitmap originalImage = new Bitmap(samplePath))
                {
                    // ✅ Fix: Convert indexed pixel format to Format32bppArgb
                    using (Bitmap bitMapImage = new Bitmap(originalImage.Width, originalImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphicImage = Graphics.FromImage(bitMapImage))
                        {
                            // ✅ Draw original image onto new bitmap first
                            graphicImage.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);

                            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

                            int qrWidth = 140;
                            int qrHeight = 140;
                            int posX = 50;
                            int posY = 50;

                            graphicImage.DrawImage(qrBitmap, new Rectangle(posX, posY, qrWidth, qrHeight));

                            Font font = new Font("Arial", 12, FontStyle.Bold);
                            StringFormat sf = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            };

                            graphicImage.DrawString(
                                EventUserInfoId.ToString(),
                                font,
                                Brushes.Black,
                                new RectangleF(posX, posY + qrHeight + 5, qrWidth, 20),
                                sf
                            );

                            font.Dispose();
                        }

                        bitMapImage.Save(strB, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
        }

        //#region Sending_Ticket

        //[HttpPost]
        //[Authorize]
        //public JsonResult SendTickets(Int64 EventUserInfoId)
        //{
        //    string str = "";
        //    int result1 = 0;
        //    int status1 = 0;
        //    Entities.Events objEvents = new Entities.Events();
        //    List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
        //    List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
        //    List<Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
        //    Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
        //    try
        //    {
        //        AWS_AssociationToolCore.Entities.AppInfo objAppInfo = new AWS_AssociationToolCore.Entities.AppInfo();
        //        AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
        //        objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);

        //        Int64 EventId = objEventUserInfo.EventId;
        //        objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, EventId, ref objEvents, ref lstEventRegistrationCounts, ref lstEventsTickets, ref result1);

        //        if (EventUserInfoId != 0)
        //        {
        //            var attachment = GetAttachment(objEventUserInfo, objEvents, lstEventsTickets);
        //            int status = 0;
        //            Entities.MailTemplates objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);
        //            if (objMailTemplates != null)
        //            {
        //                StringBuilder body = new StringBuilder();

        //                body.Append(objMailTemplates.Description);
        //                body.Replace("[usersiteurl]", ConfigurationManager.AppSettings["baseurl"].ToString());
        //                body.Replace("[EUrl]", objAppInfo.SupportEmail);
        //                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                body.Replace("[InUrl]", objAppInfo.TwitterUrl);
        //                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
        //                body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
        //                body.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
        //                body.Replace("[PaymentType]", objEventUserInfo.objEventOrderDetails.PaymentMethod);
        //                body.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
        //                body.Replace("[PaymentDate]", DateTime.UtcNow.ToString());
        //                body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
        //                body.Replace("[SiteName]", objAppInfo.SiteName);

        //                var sts = BLL.Common.SendMailwithAttachment(objEventUserInfo.Email, objMailTemplates.Subject, body.ToString(), "tickets-" + BLL.Common.EncodeURL(objEventUserInfo.EventName) + "-" + objEventUserInfo.EventUserInfoId + ".pdf");
        //            }

        //            str = "<div class=\"alert alert-success alert-dismissable\">Ticket(s) sent successfully</div>";
        //            return Json(new { ok = true, data = str });
        //        }
        //        else
        //        {
        //            str = "<div class=\"alert alert-danger alert-dismissable\">Failed Ticket(s) sening</div>";
        //            return Json(new { ok = false, data = str });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //        return Json(new { ok = false, data = str });
        //    }
        //}

        //private Attachment GetAttachment(Entities.EventUserInfo objEventUserInfo, Entities.Events objEvents, List<Entities.EventsTickets> lstEventsTickets)
        //{

        //    //objEvents = _Events.FEGetEventById(EventId, "", ref lstEventRegistrationTypes, ref status);

        //    var file = ConvertHtmlToPDF(objEventUserInfo, objEvents, lstEventsTickets);

        //    FileStream file1 = new FileStream(ConfigurationManager.AppSettings["uploadPath"] + "\\events\\TicketImage\\tickets-" + BLL.Common.EncodeURL(objEventUserInfo.EventName) + "-" + objEventUserInfo.EventUserInfoId + ".pdf", FileMode.Create, FileAccess.Write);
        //    file.WriteTo(file1);
        //    file1.Close();

        //    file.Seek(0, SeekOrigin.Begin);
        //    Attachment attachment = new Attachment(file, "tickets-" + BLL.Common.EncodeURL(objEventUserInfo.EventName) + "-" + objEventUserInfo.EventUserInfoId + ".pdf", "application/pdf");
        //    ContentDisposition disposition = attachment.ContentDisposition;
        //    disposition.CreationDate = System.DateTime.Now;
        //    disposition.ModificationDate = System.DateTime.Now;
        //    disposition.DispositionType = DispositionTypeNames.Attachment;
        //    return attachment;
        //}

        //private MemoryStream ConvertHtmlToPDF(Entities.EventUserInfo objEventUserInfo, Entities.Events objEvents, List<Entities.EventsTickets> lstEventsTickets)
        //{
        //    //Create a MemoryStream which will hold the data
        //    var ms = new MemoryStream();
        //    int status = 0;
        //    string pdffilepath = "";
        //    try
        //    {
        //        Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //        logreport("Started ticket generation", Request.Path, objEventUserInfo.FirstName);
        //        //Create an iTextSharp Document
        //        using (var doc = new iTextSharp.text.Document())
        //        {
        //            //Create a pdf writer that will hold the instance of PDF abstraction which is doc and the memory stream
        //            var writer = PdfWriter.GetInstance(doc, ms);

        //            //Open the document for writing
        //            doc.Open();
        //            int i = 1;



        //            #region Ticket Image Generation
        //            System.Drawing.Image orgImg;

        //            if (objEvents.TopLine != null && objEvents.TopLine != "")
        //            {
        //                string filePath = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\events\\TicketImage\\", objEvents.TopLine);
        //                orgImg = System.Drawing.Image.FromFile(filePath);
        //            }
        //            else
        //            {
        //                string filePath = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\events\\banners\\", "no-image.png");
        //                orgImg = System.Drawing.Image.FromFile(filePath);
        //            }

        //            string filePath1 = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\Tickets\\", "event-img-bg.png");
        //            System.Drawing.Image orgImg1 = System.Drawing.Image.FromFile(filePath1);

        //            //Load the Image to be written on.
        //            Bitmap bitMapImage = new System.Drawing.Bitmap(Server.MapPath("~/Content/Tickets/ticketsample.jpg"));
        //            Graphics graphicImage = Graphics.FromImage(bitMapImage);

        //            // Pasting Image
        //            graphicImage.DrawImage(orgImg, 11, 69, 233, 233);

        //            //Smooth graphics is nice.
        //            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

        //            PrivateFontCollection pfcoll = new PrivateFontCollection();
        //            //put a font file under a Fonts directory within your application root
        //            pfcoll.AddFontFile(Server.MapPath("~/Content/User/font/FREE3OF9.TTF"));
        //            FontFamily ff = pfcoll.Families[0];

        //            // Barcode
        //            //graphicImage.DrawString(ConfigurationManager.AppSettings["baseurl"].ToString() + "ticket-master.html?eid=" + objEvents.EventId + "&ename=" + objEvents.EventName + "&ph=" + objEventUserInfo.Mobile, new Font(ff, 35), Brushes.Black, new Point(302, 116));

        //            //QRCode

        //            pdffilepath = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString() + "ticket-master.html?eid=" + objEvents.EventId + "&ename=" + objEvents.EventName + "&tid=" + objEventUserInfo.objEventOrderDetails.TransactionId;

        //            string code = pdffilepath;
        //            QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        //            System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
        //            imgBarCode.Height = 140;
        //            imgBarCode.Width = 140;
        //            string strB = System.Configuration.ConfigurationManager.AppSettings["uploadPath"] + "\\EventProfile\\Profiledocs\\QR-of-" + objEventUserInfo.EventUserInfoId + ".jpg";
        //            using (Bitmap bitMap = qrCode.GetGraphic(20))
        //            {
        //                using (MemoryStream msa = new MemoryStream())
        //                {
        //                    bitMap.Save(msa, System.Drawing.Imaging.ImageFormat.Png);
        //                    byte[] byteImage = msa.ToArray();
        //                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
        //                    bitMap.Save(strB);
        //                }
        //            }

        //            string QRCode = System.Configuration.ConfigurationManager.AppSettings["uploadPath"] + "\\EventProfile\\Profiledocs\\QR-of-" + objEventUserInfo.EventUserInfoId + ".jpg";

        //            System.Drawing.Image QRImage = System.Drawing.Image.FromFile(QRCode);
        //            graphicImage.DrawImage(QRImage, 296, 63, 110, 110);

        //            // Barcode
        //            //graphicImage.DrawString(ConfigurationManager.AppSettings["baseurl"].ToString() + "Admin/EventRegistrations/TicketMaster?eid=" + objEvents.EventId + "&ename=" + objEvents.EventName + "&ph=" + objEventUserInfo.Mobile, new Font("Arial", 12), Brushes.Black, new Point(317, 150));


        //            // Ticket Id
        //            graphicImage.DrawString("Ticket Id: " + objEventUserInfo.objEventOrderDetails.TransactionId, new Font("Arial", 16, FontStyle.Bold), Brushes.White, new Point(318, 24));

        //            // Event Name
        //            graphicImage.DrawString(objEvents.EventName, new Font("Arial", 18, FontStyle.Bold), Brushes.Yellow, new Point(11, 24));





        //            // Price
        //            if (objEventUserInfo.TotalAmount > 0)
        //            {
        //                string pricepath = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\Tickets\\", "price-img.png");
        //                System.Drawing.Image priceImg2 = System.Drawing.Image.FromFile(pricepath);
        //                graphicImage.DrawImage(priceImg2, 677, 182, 111, 111);
        //                graphicImage.DrawString("$" + Math.Round((objEventUserInfo.TotalAmount), 0).ToString(), new Font("Arial", 20, FontStyle.Bold), Brushes.White, new Point(707, 238));
        //            }

        //            // Write Phone
        //            //graphicImage.DrawString("+1 " + objAppInfo.CompanyPhone, new Font("Arial", 14, FontStyle.Bold), Brushes.White, new Point(233, 412));

        //            // Write Email
        //            //graphicImage.DrawString(objAppInfo.CompanyEmail, new Font("Arial", 12, FontStyle.Bold), Brushes.White, new Point(483, 412));

        //            // Start Date End Date
        //            graphicImage.DrawString(@objEvents.StartDate.ToString("dddd") + " , " + @objEvents.StartDate.ToString("MMM dd, yyyy"), new Font("Arial", 12, FontStyle.Bold), Brushes.White, new Point(280, 215));

        //            //// Address
        //            if (objEvents.Location != "" && objEvents.Location != null)
        //            {
        //                graphicImage.DrawString(objEvents.Location, new Font("Arial", 10, FontStyle.Bold), Brushes.White, new Point(276, 289));
        //                // City, State, Country, Zip
        //                graphicImage.DrawString(objEvents.City + (objEvents.City != null && objEvents.City != "" ? ", " : "") + objEvents.StateName + (objEvents.StateName != null && objEvents.StateName != "" ? ", " : "") + objEvents.ZipCode, new Font("Arial", 12, FontStyle.Bold), Brushes.White, new Point(280, 311));
        //            }
        //            else
        //            {
        //                graphicImage.DrawString(objEvents.City + (objEvents.City != null && objEvents.City != "" ? ", " : "") + objEvents.StateName + (objEvents.StateName != null && objEvents.StateName != "" ? ", " : "") + objEvents.ZipCode, new Font("Arial", 12, FontStyle.Bold), Brushes.White, new Point(280, 289));
        //            }

        //            //Ticket Type with Count

        //            graphicImage.DrawString(objEventUserInfo.TicketTypesWithCount, new Font("Arial", 14, FontStyle.Bold), Brushes.White, new Point(22, 365));

        //            // Short Description
        //            //graphicImage.DrawString(objEvents.EventName + " held at " + objEvents.Location + ", " + objEvents.City + ", " + objEvents.StateName, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(81, 395));



        //            // Pasting Image
        //            graphicImage.DrawImage(orgImg1, 0, 60, 260, 260);

        //            //if (objEventUserInfo.MemberId != null && objEventUserInfo.MemberId != 0)
        //            //{
        //            //    // Member ID
        //            //    graphicImage.DrawString(item.RegistrationTitle.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.White, new Point(264, 58));
        //            //    graphicImage.DrawString("Member #" + " " + objEventUserInfo.EventUserInfoId.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, new Point(264, 85));
        //            //}
        //            //else
        //            //{
        //            //    graphicImage.DrawString((item.RegistrationTitle.Length > 33 ? item.RegistrationTitle.Substring(0, 28) + "..." : item.RegistrationTitle.ToString()), new Font("Arial", 16, FontStyle.Bold), Brushes.White, new Point(264, 58));
        //            //}

        //            // Image Saving
        //            //Response.ContentType = "image/jpeg";
        //            bitMapImage.Save(Server.MapPath("~/Content/Tickets/" + objEventUserInfo.EventUserInfoId + "-" + objEventUserInfo.FirstName + "-" + objEventUserInfo.LastName + ".jpg"));

        //            //Clean house.
        //            graphicImage.Dispose();
        //            bitMapImage.Dispose();
        //            #endregion

        //            #region Pdf Witer

        //            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(" ");
        //            string imageURL = System.Configuration.ConfigurationManager.AppSettings["baseurl"] + "Content/Tickets/" + objEventUserInfo.EventUserInfoId + "-" + objEventUserInfo.FirstName + "-" + objEventUserInfo.LastName + ".jpg";
        //            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
        //            //Resize image depend upon your need
        //            jpg.ScaleToFit(500f, 280f);
        //            //Give some space after the image
        //            jpg.SpacingAfter = 10f;

        //            jpg.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
        //            doc.Add(paragraph);
        //            doc.Add(jpg);
        //            doc.Add(paragraph);
        //            if (i == 1)
        //            {
        //                i++;
        //            }
        //            else
        //            {
        //                doc.NewPage();
        //                i = 1;
        //            }
        //            #endregion


        //            writer.CloseStream = false;

        //            //close the document
        //            doc.Close();
        //            //return the stream 
        //            logreport("Ticket generation Completed", Request.Path, objEventUserInfo.FirstName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logreport(ex.Message, Request.Path, objEventUserInfo.FirstName);
        //    }
        //    return ms;
        //}

        //private static ImageCodecInfo GetEncoder(ImageFormat format)
        //{
        //    var codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (var codec in codecs)
        //    {
        //        if (codec.FormatID == format.Guid)
        //        {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}

        //public Byte[] getBarcodeImage(string barcode, string file)
        //{
        //    try
        //    {
        //        BarCode39 _barcode = new BarCode39();
        //        int barSize = 16;
        //        string fontFile = Server.MapPath("~/Content/font/FREE3OF9.TTF");
        //        return (_barcode.Code39(barcode, barSize, true, file, fontFile));
        //    }
        //    catch (Exception ex)
        //    {
        //        //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);
        //    }
        //    return null;
        //}

        //public JsonResult GetAmountEncode(decimal Amount = 0)
        //{
        //    string EAmount = "";
        //    if (Amount != 0)
        //    {
        //        EAmount = BLL.Common.Encrypt(Amount.ToString());
        //    }
        //    return Json(new { ok = true, data = EAmount });
        //}


        //public void logreport(string error = "", string pageName = "", string fileName = "")
        //{
        //    fileName = "Log_" + DateTime.Now.ToString("dd-MM-yyyy") + BLL.Common.EncodeURL(fileName) + ".txt";
        //    var relativePath = "~/Content/logfiles/" + fileName;
        //    var filepath = HttpContext.Server.MapPath(relativePath);

        //    if (System.IO.File.Exists(filepath))
        //    {
        //        using (StreamWriter stwriter = new StreamWriter(filepath, true))
        //        {
        //            stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
        //            stwriter.WriteLine("Page :" + pageName);
        //            stwriter.WriteLine(error);
        //            stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
        //        }
        //    }
        //    else
        //    {
        //        StreamWriter stwriter = System.IO.File.CreateText(filepath);
        //        stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
        //        stwriter.WriteLine("Page :" + pageName);
        //        stwriter.WriteLine(error);
        //        stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
        //        stwriter.Close();
        //    }
        //}

        ////#endregion

        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        //public JsonResult ResendQRCode(Int64 EventUserInfoId, string type = "")
        //{

        //    string str = "";
        //    string QrUri = "";
        //    int _status = 0;
        //    int status = 0;
        //    try
        //    {
        //        Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //        string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
        //        if (EventUserInfoId != 0)
        //        {

        //           // str = "<div class=\"alert alert-success alert-dismissable\">QR Code sent successfully to Registered Email-Id</div>";

        //            if (type == "Event")
        //            {
        //                Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);

        //                Entities.EventUserInfo objEventUserInfo = _Events.GetEventUserInfoById(EventUserInfoId, ref _status);

        //                if (objTemplates != null)
        //                {

        //                    StringBuilder body1 = new StringBuilder();
        //                    if (objEventUserInfo.IsApproved == true)
        //                    {
        //                        body1.Append(objTemplates.Description);
        //                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                        body1.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                        body1.Replace("[TEmailId]", objAppInfo.CompanyEmail);
        //                        body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + "" + objEventUserInfo.LastName));
        //                        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
        //                        string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
        //                        var image = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
        //                        body1.Replace("[QRCode]", image);
        //                        _sendmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body1.ToString());
        //                    }
        //                }
        //            }
        //            return Json(new
        //            {
        //                ok = true,
        //                messageType = "success",
        //                message = "QR Code sent successfully to Registered Email-Id"
        //            });
        //        }
        //        else
        //        {
        //           // str = "<div class=\"alert alert-danger alert-dismissable\">Failed sending QR Code</div>";

        //            return Json(new
        //            {
        //                ok = false,
        //                messageType = "error",
        //                message = "Failed sending QR Code"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            ok = false,
        //            messageType = "error",
        //            message = "Failed transaction."
        //        });
        //    }
        //}

        //public JsonResult ResendQRCode(Int64 EventUserInfoId, string type = "")
        //{

        //    string str = "";
        //    string QrUri = "";
        //    int _status = 0;
        //    int status = 0;
        //    //string RefCode = "TATH" + EventUserInfoId;
        //    Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //    string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
        //    try
        //    {

        //        if (EventUserInfoId != 0)
        //        {

        //            str = "<div class=\"alert alert-success alert-dismissable\">QR Code sent successfully to Registered Email-Id</div>";
        //            if (type == "Event")
        //            {
        //                int _qstatus = 0;
        //                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
        //                Entities.Events objEvents = new Entities.Events();
        //                Entities.EventUserInfo objUser = new Entities.EventUserInfo();
        //                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
        //                Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref status);

        //                Entities.EventUserInfo objEventUserInfo = _Events.GetEventUserInfoById(EventUserInfoId, ref _status);
        //                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();

        //                //Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //                objUser = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets,ref lstEventRegTickets, ref _qstatus);



        //                string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Amount</span></th></tr>";

        //                if (lstEventRegCounts != null && lstEventRegCounts.Count != 0)
        //                {
        //                    int i = 1;
        //                    foreach (var item in lstEventRegCounts)
        //                    {
        //                        tickets = tickets + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.RegistrationTitle + "</td><td align=\"center\">" + item.Count + "</td><td align=\"center\">$" + item.Amount + "</td></tr>";
        //                        i++;
        //                    }
        //                }
        //                tickets = tickets + "</table>";


        //                // Load template
        //                string htmlTemplate = LoadTicketTemplate();

        //                List<byte[]> ticketPdfs = new List<byte[]>();
        //                int ticketNo = 1;

        //                DateTime eventDate = objEvents.StartDate;

        //                // Extract parts
        //                string month = eventDate.ToString("MMM").ToUpper();   // JAN
        //                string year = eventDate.Year.ToString();              // 2025
        //                string day = eventDate.Day.ToString();                // 19
        //                string suffix = GetDaySuffix(eventDate.Day);          // TH
        //                string time = eventDate.ToString("hh:mm tt");


        //                foreach (var participant in lstEventRegTickets)
        //                {
        //                    string ticketRefCode = objAppInfo.SiteName + "-Ticket-" + participant.EventRegTicketId;
        //                    string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + ticketRefCode + ".jpg";

        //                    // Build the <img> tag
        //                    var image1 = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";

        //                    string filledHtml = htmlTemplate
        //                        .Replace("[SiteName]", objAppInfo.SiteName)
        //                        .Replace("[CompanyAddress]", objAppInfo.CompanyAddress)
        //                        .Replace("[EventName]", objEvents.EventName)
        //                        .Replace("[Location]", objEvents.Location)
        //                        .Replace("[MONTH]", month)
        //                        .Replace("[YEAR]", year)
        //                        .Replace("[DAY]", day)
        //                        .Replace("[DAY_SUFFIX]", suffix)
        //                        .Replace("[TIME]", time)
        //                        .Replace("[SPONSOR_NAME]", objEventUserInfo.FirstName + " " + objEventUserInfo.LastName)
        //                        .Replace("[CATEGORY]", participant.RegistrationTitle) // or dynamic value
        //                        .Replace("[TICKET_NUMBER]", participant.TicketId)
        //                        .Replace("[QRCode]", image1);


        //                    string fontDir = Server.MapPath("~/fonts");  // if MVC

        //                    byte[] pdfBytes = GenerateTicketPdf_iText7(filledHtml, fontDir);


        //                    ticketPdfs.Add(pdfBytes);
        //                    ticketNo++;
        //                }

        //                // Merge PDFs
        //                byte[] finalPdf = MergePdfs(ticketPdfs);


        //                if (objTemplates != null)
        //                {

        //                    StringBuilder body1 = new StringBuilder();
        //                    if (objEventUserInfo.IsApproved == true)
        //                    {
        //                        body1.Append(objTemplates.Description);
        //                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                        body1.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                        body1.Replace("[TEmailId]", objAppInfo.CompanyEmail);
        //                        body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + "" + objEventUserInfo.LastName));
        //                        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
        //                        body1.Replace("[TransactionId]", objEventUserInfo.TransactionId);
        //                        body1.Replace("[PaymentType]", objEventUserInfo.PaymentMethod);
        //                        body1.Replace("[PaymentStatus]", objEventUserInfo.PaymentStatus);
        //                        body1.Replace("[PaymentDate]", objEventUserInfo.PaymentDate.ToString("MM/dd/yyyy"));
        //                        body1.Replace("[AmountPaid]", objEventUserInfo.AmountPaid.ToString());
        //                        body1.Replace("[Email]", objEventUserInfo.Email);
        //                        body1.Replace("[Tickets]", tickets);
        //                         //string url = ConfigurationManager.AppSettings["usersiteurl"].ToString() + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
        //                        //var image = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
        //                        //body1.Replace("[QRCode]", image);


        //                        string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
        //                                         var image = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
        //                                             body1.Replace("[QRCode]", image);




        //                        _sendmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body1.ToString());



        //                       // BLL.Common.SendMail(objEventUserInfo.Email, objTemplates.Subject, body1.ToString());
        //                    }
        //                }
        //            }
        //           // return Json(new { ok = true, data = str });
        //            return Json(new
        //            {
        //                ok = true,
        //                messageType = "success",
        //                message = "QR Code sent successfully to Registered Email-Id"
        //            });
        //        }
        //        else
        //        {

        //            return Json(new
        //            {
        //                ok = false,
        //                messageType = "error",
        //                message = "Failed sending QR Code"
        //            });

        //            //str = "<div class=\"alert alert-danger alert-dismissable\">Failed sending QR Code</div>";
        //            //return Json(new { ok = false, data = str });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            ok = false,
        //            messageType = "error",
        //            message = "Failed"
        //        });
        //        //str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
        //        //return Json(new { ok = true, data = str });
        //    }
        //}
        [HttpPost]
        public JsonResult AllocateSeat(long eventticketId, string TicketId, Int64 EventUserInfoId, Int64 EventId, string seatNo, bool notify, string email)
        {
            try
            {
                string InsertedBy = HttpContext.User.Identity.Name.ToString();
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = new Entities.AppInfo();
                int status = 0;
                int _qstatus = 0;
                objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                Entities.EventUserInfo objeventuserinfo = new Entities.EventUserInfo();

                List<Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                List<Entities.EventRegTickets> lstEventsRegTickets = new List<Entities.EventRegTickets>();
                Entities.Events objEvents = new Entities.Events();
                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, EventId, ref objEvents, ref lstEventRegistrationCounts, ref lstEventsTickets, ref lstEventsRegTickets, ref _qstatus);


                // 1. Update seat allocation in DB
                // Assuming you added column SeatNo in EventRegTickets
                _Events.UpdateSeatAllocation(eventticketId, seatNo, InsertedBy, notify);

                // 2. Send notification email if chosen
                if (notify && !string.IsNullOrEmpty(email))
                {
                    int statusAdmin = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Your Event Seat Allocation", 0, ref statusAdmin);
                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                    {

                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplatesAdmin.Description);
                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body1.Replace("[EUrl]", objAppInfo.SupportEmail);
                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body1.Replace("[InUrl]", objAppInfo.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                        body1.Replace("[Email]", objEventUserInfo.Email);
                        body1.Replace("[seatNo]", seatNo);
                        body1.Replace("[TicketId]", TicketId);
                        body1.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                        body1.Replace("[PaymentType]", objEventUserInfo.objEventOrderDetails.PaymentMethod);
                        body1.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                        body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString());
                        body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());

                        //_sendmail.SendMailwithCC(objAppInfo.CompanyEmail, objAppInfo.CompanyEmail, objAppInfo.CompanyEmail, "Registered Event Details - " + objAppInfo.SiteName, body1.ToString());
                    }


                    string subject = "Your Event Seat Allocation";
                    string body = $"Dear Attendee,<br/><br/>Your seat has been allocated as <strong>{seatNo}</strong>.<br/><br/>Thank you.";

                    //EmailHelper.SendEmail(email, subject, body);
                }

                return Json(new { success = true, message = "Seat allocated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        #region QRCode
        //public void GenerateQRCodeNew(long EventUserInfoId, ref List<Entities.EventRegTickets> lstEventRegTickets)
        //{
        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);

        //    //int _qstatus = 0;
        //    //List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
        //    //List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
        //    //List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
        //    //Entities.Events objEvents = new Entities.Events();

        //    //objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

        //    // === Overall Registration QR ===
        //    string RefCode = objAppInfo.SiteName + EventUserInfoId;
        //    string overallUrl = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;

        //    SaveQRCodeImage(overallUrl, objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + RefCode + ".jpg");

        //    // === Individual Tickets QR ===
        //    if (lstEventRegTickets != null && lstEventRegTickets.Any())
        //    {
        //        foreach (var ticket in lstEventRegTickets)
        //        {
        //            // Payload URL for individual ticket
        //            string ticketUrl = objAppInfo.UserSiteUrl + "ticket-preview?tktid=" + ticket.EventRegTicketId + "&euid=" + objEventUserInfo.EventUserInfoId + "&uid=" + objEventUserInfo.EventId;

        //            // Unique file name
        //            string ticketRefCode = objAppInfo.SiteName + "-Ticket-" + ticket.EventRegTicketId;

        //            string savePath = objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + ticketRefCode + ".jpg";

        //            SaveQRCodeImage(ticketUrl, savePath);
        //        }
        //    }
        //}
        //private void SaveQRCodeImage(string payload, string savePath)
        //{
        //    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        //    using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
        //    using (QRCode qrCode = new QRCode(qrCodeData))
        //    using (Bitmap bitMap = qrCode.GetGraphic(20))
        //    {
        //        // Save QR as PNG
        //        bitMap.Save(savePath, ImageFormat.Png);
        //    }
        //}
        //public void GenerateQRCode(Int64 EventUserInfoId)
        //{

        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);
        //    string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;

        //    string EmployeeRefUrl = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;

        //    string WebUri = EmployeeRefUrl;

        //    string UriPayload = WebUri.ToString();

        //    QRCodeGenerator QrGenerator = new QRCodeGenerator();
        //    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(UriPayload, QRCodeGenerator.ECCLevel.Q);

        //    #region QRCode Generation Code


        //    string pdffilepath = "";

        //    #region Barcode

        //    #endregion

        //    #region Ticket Image Generation

        //    //Load the Image to be written on.
        //    Bitmap bitMapImage = new System.Drawing.Bitmap(Server.MapPath("~/Content/QRCodeGeneration/Qrcodesample.png"));
        //    Graphics graphicImage = Graphics.FromImage(bitMapImage);


        //    //Smooth graphics is nice.
        //    graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
        //    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        //    TextInfo textInfo = cultureInfo.TextInfo;

        //    StringFormat sf = new StringFormat();
        //    sf.LineAlignment = StringAlignment.Center;
        //    sf.Alignment = StringAlignment.Center;

        //    pdffilepath = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;

        //    string code = pdffilepath;


        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        //    QRCode qrCode = new QRCode(qrCodeData);
        //    System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();


        //    imgBarCode.Height = 140;
        //    imgBarCode.Width = 140;

        //    string strB = objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + RefCode + ".jpg";

        //    strB = strB.Replace("https://", "");
        //    strB = strB.Replace("/", "");


        //    using (Bitmap bitMap = qrCode.GetGraphic(20))
        //    {
        //        using (MemoryStream msa = new MemoryStream())
        //        {
        //            bitMap.Save(msa, System.Drawing.Imaging.ImageFormat.Png);
        //            byte[] byteImage = msa.ToArray();
        //            imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
        //            bitMap.Save(strB);
        //        }
        //    }


        //    //Clean house.
        //    graphicImage.Dispose();
        //    bitMapImage.Dispose();

        //    #endregion

        //    #endregion
        //}

        public ActionResult QRCodeView(Int64 id = 0, int uid = 0, String MembershipType = "")
        {
            try
            {

                int _qstatus = 0;
                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                Entities.Events objEvents = new Entities.Events();
                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventRegCounts = lstEventRegCounts;
                    ViewBag.lstEventsTickets = lstEventsTickets;
                    ViewBag.uid = uid;
                    ViewBag.MembershipType = MembershipType;
                    ViewBag.status = _qstatus;
                    var username = HttpContext.Session.GetString("username");
                    if (this.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(this.User.Identity.Name) && !string.IsNullOrEmpty(username))
                    {
                        ViewBag.type = "yes";
                    }

                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
            }
            return View();
        }

        #endregion
        #region GenerateTickets

        //private string LoadTicketTemplate()
        //{
        //    // Map the path relative to wwwroot or content root
        //    string templatePath = Path.Combine(_env.WebRootPath, "Content", "Templates", "ticket.html");
        //    // OR use ContentRootPath if file is outside wwwroot
        //    // string templatePath = Path.Combine(_env.ContentRootPath, "Content", "Templates", "ticket.html");

        //    return System.IO.File.ReadAllText(templatePath);
        //}


        public byte[] GenerateTicketPdf_iText7(string html, string fontDir)
        {
            using (var ms = new MemoryStream())
            {
                var props = new ConverterProperties();

                // Setup fonts
                var fontProvider = new DefaultFontProvider(false, false, false);
                if (!string.IsNullOrEmpty(fontDir))
                    fontProvider.AddDirectory(fontDir);   // e.g. "C:\\MyApp\\fonts"
                props.SetFontProvider(fontProvider);

                // Convert HTML directly to the output stream (ms)
                HtmlConverter.ConvertToPdf(
                    new MemoryStream(Encoding.UTF8.GetBytes(html)), // html source
                    ms,                                             // PDF output
                    props                                           // converter options
                );

                return ms.ToArray();
            }
        }


        public byte[] MergePdfs(List<byte[]> pdfFiles)
        {
            using (var ms = new MemoryStream())
            {
                iText.Kernel.Pdf.PdfDocument finalPdf = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfWriter(ms));
                PdfMerger merger = new PdfMerger(finalPdf);

                foreach (var pdf in pdfFiles)
                {
                    using (var readerStream = new MemoryStream(pdf))
                    {
                        iText.Kernel.Pdf.PdfDocument tempPdf = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(readerStream));
                        merger.Merge(tempPdf, 1, tempPdf.GetNumberOfPages());
                        tempPdf.Close();
                    }
                }

                finalPdf.Close();
                return ms.ToArray();
            }
        }


        private string GetDaySuffix(int day)
        {
            if (day >= 11 && day <= 13) return "TH";
            switch (day % 10)
            {
                case 1: return "ST";
                case 2: return "ND";
                case 3: return "RD";
                default: return "TH";
            }
        }
        public IActionResult EventQR(string id1 = "", long id = 0, int uid = 0, string MembershipType = "")
        {
            try
            {
                id = Convert.ToInt64(BLL.Common.UnEncryptString(id1));

                int _qstatus = 0;
                var lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                var lstEventRegTickets = new List<Entities.EventRegTickets>();
                var lstEventsTickets = new List<Entities.EventsTickets>();
                var objEvents = new Entities.Events();

                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(
                    id, 0,
                    ref objEvents,
                    ref lstEventRegCounts,
                    ref lstEventsTickets,
                    ref lstEventRegTickets,
                    ref _qstatus
                );

                if (_qstatus == 1)
                {
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventRegCounts = lstEventRegCounts;
                    ViewBag.lstEventsTickets = lstEventsTickets;
                    ViewBag.lstEventRegTickets = lstEventRegTickets;
                    ViewBag.uid = uid;
                    ViewBag.MembershipType = MembershipType;
                    ViewBag.status = _qstatus;

                    // ASP.NET Core session check
                    var username = HttpContext.Session.GetString("username");
                    if (this.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(this.User.Identity.Name) && !string.IsNullOrEmpty(username))
                    {
                        ViewBag.type = "yes";
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
            }

            return View();
        }

        public IActionResult EventuserExporttoExcel(
            long EventId = 0,
            string Search = "",
            string SortColumn = "EUI.EventUserInfoId",
            string SortOrder = "ASC")
        {
            try
            {
                string Sort = !string.IsNullOrEmpty(SortColumn) ? $"{SortColumn} {SortOrder}" : "";

                // Get data from DB
                DataTable dtUni = _DEvents.ExportToEventUserInfoList(EventId, Search, Sort);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("EventUsers-Export");

                    // Load DataTable headers
                    for (int col = 0; col < dtUni.Columns.Count; col++)
                    {
                        worksheet.Cells[1, col + 1].Value = dtUni.Columns[col].ColumnName;

                        // Style header row
                        worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, col + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, col + 1].Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.Color.FromArgb(173, 216, 230));
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
                    string fileName = $"EventUsers-Export-{DateTime.UtcNow:MM-dd-yyyy}.xlsx";

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index");
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
