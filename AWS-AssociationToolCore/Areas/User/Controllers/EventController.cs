using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.DAL;
using AWS_AssociationToolCore.Entities;
using AWS_AssociationToolCore.Models;
using Braintree;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Grpc.Core;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Linq;
using NPOI.XWPF.UserModel;
using PaymentProcess;
using QRCoder;
using Stripe;
using Stripe.Checkout;
using System;
using System;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Web;
using Font = System.Drawing.Font;
using Session = Stripe.Checkout.Session;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]

    public class EventController : Controller
    {

        BLL.InnerPages _innerpage = new BLL.InnerPages();
        BLL.Events _Events = new BLL.Events();
        BLL.Members _Members = new BLL.Members();
        Entities.Members _objMember = new Entities.Members();
        BLL.AppInfo _AppInfo = new BLL.AppInfo();
        Entities.Events objEvents = new Entities.Events();
        List<Entities.Events> lstEvents = new List<Entities.Events>();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();
        PayByPayPal objPayByPayPal = new PayByPayPal();
        List<Entities.Events> lstCurrentEvents = new List<Entities.Events>();
        List<Entities.Events> lstUpcomingEvents = new List<Entities.Events>();
        List<Entities.Events> lstPastEvents = new List<Entities.Events>();
        List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
        List<Entities.EventDates> lstEventDates = new List<Entities.EventDates>();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        AWS_AssociationToolCore.BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        Entities.InnerPages objInnerPages = new Entities.InnerPages();
        BLL.EventCompetitions _EventCompetitions = new BLL.EventCompetitions();
        List<Entities.EventCompetitions> lstEventCompetitions = new List<Entities.EventCompetitions>();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        List<Entities.RegistrationCategories> lstEventRegistrationCategories = new List<Entities.RegistrationCategories>();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        Entities.RegistrationCategories objRegistrationCategories = new Entities.RegistrationCategories();
        Entities.Form1 objForm1 = new Entities.Form1();
        BLL.Form1 _Form1 = new BLL.Form1();
        BLL.Users _Users = new BLL.Users();
        BLL.SendMail _sentmail = new BLL.SendMail();
        // ✅ Fix the constructor
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly IBraintreeConfiguration _config;
        private readonly BLL.IWebHostEnvironment _webHostEnvironment;

        public EventController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IBraintreeConfiguration config)
        {
            _env = env;
            _config = config;
        }
        public IActionResult Index(string Type = "upcoming", Int64 ChapterId = 0, string cname = "")
        {
            try
            {
                // ✅ Read cookie only if cname not passed in URL
                if (string.IsNullOrEmpty(cname))
                {
                    if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
                    {
                        cname = cookieValue;
                    }
                }

                Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();
                int qstatus = 0;

                // ✅ Only lookup chapter if cname exists
                if (!string.IsNullOrEmpty(cname))
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                    ChapterId = objChapters?.ChapterId ?? 0;
                }

                // ✅ Always fallback to 1
                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }

                Int64 status = 0;
                objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl(cname + "/" + Type + "-events", ref status);

                ViewBag.objMenuPageDetails = objMenuPageDetails;
                ViewBag.ChapterId = ChapterId;
                ViewBag.Type = Type;
                ViewBag.cname = cname;
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            return View();
        }
        public IActionResult EventsList(Int64 ChapterId = 0, string Type = "upcoming", string search = "", string SortColumn = "StartDate", string SortOrder = "ASC", int PageNo = 1, int PageItems = 10, string cname = "")
        {
            if (Type == "upcoming") SortOrder = "ASC";
            else SortOrder = "DESC";

            string Sort = SortColumn != "" ? SortColumn + " " + SortOrder : "";
            List<Entities.Events> lstEvents = new List<Entities.Events>();
            int Total = 0;
            int qstatus = 0;

            // ✅ Fix: null check before lookup
            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                ChapterId = objChapters?.ChapterId ?? 0;
            }

            if (ChapterId == 0) ChapterId = 1;

            try
            {
                lstEvents = _Events.FEGetEventsList(ChapterId, Type,
                    HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.total = Total;
            ViewBag.items = PageItems;
            ViewBag.pageno = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.Eventslist = lstEvents;
            ViewBag.Type = Type;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            return View();
        }

        public IActionResult EventRegistration(string EventName = "", Int64 MemberId = 0, Int64 eid = 0, string cname = "")
        {
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            int status = 0;
            string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("FormValidationCode", generatedCode, cookieOptions);
            string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;

            try
            {
                if (EventName != "")
                {
                    objEvents = _Events.GetEventByIdFE(eid, EventName, userName, ref lstEventRegistrationTypes, ref lstEventDates, ref lstEventRegistrationCategories, ref status);
                    if (objEvents != null && objEvents.EventId != 0)
                    {
                        string currentUserEmail = "";
                        currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";

                        // Auth check in Core
                        // ✅ Fixed - clean and correct
                        if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(currentUserEmail))
                        {
                            _objMember = _Members.GetMemberFullDetailsByEmail(currentUserEmail, ref status);
                        }
                        lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref status);
                        lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);
                        MemberId = Convert.ToInt64(HttpContext.Session.GetString("MemberID") ?? "0");

                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Not found Event details for selected Event..!!</div>";
                        return RedirectToAction("Error404", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.Memberdetails = _objMember;
            ViewBag.MemberId = MemberId;
            ViewBag.objEvents = objEvents;
            ViewBag.EventName = EventName;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            ViewBag.cname = cname;
            ViewBag.lstEventCompetitions = lstEventCompetitions;
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            ViewBag.ValidationCode = generatedCode;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EventRegistration(IFormFile file2, IFormFile filezelle, Entities.EventUserInfo objEventUserInfo, List<Entities.EventParticipants> lstEventParticipants, List<Entities.EventRegistrationCounts> lstEventRegistrationCounts, string cname = "", string MCaptcha = "", string DiscountPercent = "", string payamount = "", string Notifications = "", string recaptchaToken = "", string Honeypot = "", string ValidationCode = "")
        {
            IBraintreeGateway gateway = _config.GetGateway();

            int status2 = 0;
            Int64 LRstatus = 0;
            /* 
             * Generating IP Address for blocking Bots 
             * This section of code identifies and processes IPs 
             * to prevent bot access to the system.
             */

            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
            Entities.LogReport logs = new Entities.LogReport();

            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            string encodedCname = HttpUtility.UrlEncode(cname);
            objEventUserInfo.DocumentUrl = ip;
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Event Registration",
                LogDescription = "Entered into Event Registration Process",
                LogDate = DateTime.Now,
                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objEventUserInfo.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
            };
            _Users.LogReportInsert(objLogReport, ref LogId);

            /*Getting application settings like email, phone, brevo keys... etc.,*/
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

            Int64 EventUserInfoId = 0;


            try
            {
                if (objEventUserInfo.AmountPaid == 0)
                {
                    ModelState.Remove("CardNumber");
                    ModelState.Remove("CardName");
                    ModelState.Remove("Cvv");
                    ModelState.Remove("CSVYear");
                    ModelState.Remove("CSVMonth");
                    ModelState.Remove("ZellePartnerName");
                    ModelState.Remove("filezelle");
                    ModelState.Remove("Amount");
                }
                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "EventRegistration",
                        LogDescription = "Honeypot Bot activity detected",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("EventRegistration", "Event", new { eid = objEventUserInfo.EventId, EventName = BLL.Common.EncodeURL(objEventUserInfo.EventName), cname = cname });
                }
                // Validate model state


                //If capcha is "Yes", recapcha verification is process, if capcha is "No" without verification we can submit form...
                var capcha = objAppInfo.ShowCapcha;

                // Verify reCAPTCHA token
                bool isHuman = await BLL.Common.VerifyRecaptchaToken(recaptchaToken);

                if (capcha == "No")
                {
                    isHuman = true;
                }

                else if (capcha == "Yes")
                {
                    isHuman = true;
                }
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "capcha",
                    LogDescription = "OFTER capcha",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                // Retrieve the code from the cookie
                string cookieValidationCode = Request.Cookies["FormValidationCode"];
                if (string.IsNullOrEmpty(cookieValidationCode))
                {

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Event Registration",
                        LogDescription = "Failed Retrieve the code from the cookie",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("EventRegistration", "Event");
                }

                if (cookieValidationCode != ValidationCode)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "EventRegistration",
                        LogDescription = "Failed Security validation failed",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("EventRegistration", "Event");
                }
                if (isHuman == true)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "EventRegistration",
                        LogDescription = "After reCAPTCHA Entered to the Process",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    if (objEventUserInfo.PaymentMethod == "PayPal" || objEventUserInfo.PaymentMethod == "Square" || objEventUserInfo.PaymentMethod == "Stripe" || objEventUserInfo.PaymentMethod == "Swire Pay")
                    {
                        objEventUserInfo.IsApproved = true;
                    }
                    else
                    {
                        objEventUserInfo.IsApproved = false;
                    }

                    objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                    objEventUserInfo.InsertedBy = objEventUserInfo.FirstName;
                    objEventUserInfo.OtherPaymentId = payamount.ToString();
                    objEventUserInfo.Gender = DiscountPercent.ToString();
                    objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objEventUserInfo.InsertedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                    string imageurl = "NA";
                    string receipturl = "NA";
                    string SanitizeFileName(string fileName)
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName);
                        string ext = Path.GetExtension(fileName).ToLower(); // ✅ always lowercase .jpg
                        name = name.Replace(" ", "_")
                                   .Replace("(", "")
                                   .Replace(")", "")
                                   .Replace(".", "_")  // ✅ replace dots with underscore
                                   .Trim('_');
                        return name + ext;
                    }

                    // ✅ Step 1: Sanitize and store original names
                    string originalImageFile = "";
                    string originalReceiptFile = "";

                    if (file2 != null && file2.Length > 0)
                    {
                        originalImageFile = SanitizeFileName(file2.FileName);
                        imageurl = originalImageFile;
                        receipturl = imageurl;
                    }

                    if (filezelle != null && filezelle.Length > 0)
                    {
                        originalReceiptFile = SanitizeFileName(filezelle.FileName);
                        receipturl = originalReceiptFile;
                    }

                    // ✅ Save BEFORE DB insert - SP will change these values
                    string originalImageUrl = imageurl;
                    string originalReceiptUrl = receipturl;

                    if (lstEventRegistrationCounts != null && lstEventRegistrationCounts.Count != 0 && lstEventRegistrationCounts[0].EventRegistrationTypeId != 0)
                    {
                        objEventUserInfo.XMLRegistrationsCounts = BLL.Common.CreateXMLForObject(lstEventRegistrationCounts);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "EventRegistration",
                            LogDescription = "Successfully created xml for Registration Tickets",
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                    }
                    EventUserInfoId = objEventUserInfo.EventUserInfoId;

                    Int64 _status = _Events.InsertEventUserInfo(objEventUserInfo, ref EventUserInfoId, ref imageurl, ref receipturl);


                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "EventUserInfo Insert",
                        LogDescription = _status == 1
                                                    ? $"Donation inserted successfully. ServiceDonationId: {EventUserInfoId}"
                                                    : $"Donation insert failed. Status: {_status}",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });




                    if (file2 != null && file2.Length > 0 && imageurl != "NA")
                    {

                        string finalFolder = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages");
                        Directory.CreateDirectory(finalFolder);

                        // imageurl now has SP-generated name e.g. "183-test.about_img_1.jpg"
                        string finalPath = Path.Combine(finalFolder, imageurl);
                        await using var stream1 = new FileStream(finalPath, FileMode.Create);
                        await file2.CopyToAsync(stream1);
                    }

                    if (filezelle != null && filezelle.Length > 0 && receipturl != "NA")
                    {

                        string finalFolder = Path.Combine(objAppInfo.UploadPath, "EventReceiptImages");
                        Directory.CreateDirectory(finalFolder);

                        string finalPath = Path.Combine(finalFolder, receipturl);
                        await using var stream2 = new FileStream(finalPath, FileMode.Create);
                        await filezelle.CopyToAsync(stream2);
                    }
                    Entities.EventOrderDetails objEventOrderDetails = new Entities.EventOrderDetails();


                    if (objAppInfo.IsQRCode == "Yes")
                    {
                        /*Generating QR code for Tickets*/
                        GenerateQRCode(EventUserInfoId);
                    }
                    if (lstEventParticipants != null && lstEventParticipants.Count() != 0)
                    {
                        foreach (Entities.EventParticipants objEventParticipants in lstEventParticipants)
                        {
                            objEventParticipants.EventUserInfoId = EventUserInfoId;
                            objEventParticipants.EventId = objEventUserInfo.EventId;
                            if (objEventParticipants != null && objEventParticipants.FirstName != null)
                            {
                                objEventParticipants.IsApproved = false;
                                objEventParticipants.UpdatedBy = objEventParticipants.FirstName;
                                Int64 estatus = _Events.InsertEventParticipant(objEventParticipants);
                                if (estatus == -1)
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "EventParticipants Failed",
                                        LogDescription = $"Failed inserting EventParticipants  details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    TempData["message"] = "<div class=\"error1 closable\">Failed inserting event registration.</div>";
                                    return RedirectToAction("EventRegistration", "Event", new { eid = objEventUserInfo.EventId, EventName = BLL.Common.EncodeURL(objEventUserInfo.EventName) });
                                }
                            }
                        }
                    }
                    if (objEventUserInfo.AmountPaid != 0)
                    {
                        if (objEventUserInfo.PaymentMethod == "Square")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = "Square Payment started",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            int paystatus = 0;
                            Entities.PaymentSettings objsettings = _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

                            string squareAccessToken = objsettings.SuccessUrl;
                            string squareEnvironment = objsettings.AccountType == "Test" ? "sandbox" : "production";
                            string squareLocationId = objsettings.TokenNo;
                            string squareNonce = Request.Form["squareNonce"].ToString();
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Debug",
                                LogDescription = $"AccountType:{objsettings.AccountType} | " +
                                            $"Environment:{squareEnvironment} | " +
                                            $"TokenFirst15:{squareAccessToken?.Substring(0, Math.Min(15, squareAccessToken?.Length ?? 0))} | " +
                                            $"LocationId:{squareLocationId} | " +
                                            $"Nonce:{squareNonce?.Substring(0, Math.Min(10, squareNonce?.Length ?? 0))}",
                                LogDate = DateTime.Now,
                                InsertedBy = "Debug",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = "Debug",
                                UpdatedDate = DateTime.Now
                            });
                            if (string.IsNullOrWhiteSpace(squareNonce))
                            {
                                TempData["message"] = "<div class=\"error1 closable\">Payment card was not submitted properly. Please try again.</div>";
                                return RedirectToAction("PaymentFailed", "Home");
                            }

                            var squareClient = new Square.SquareClient.Builder()
                                .Environment(squareEnvironment == "production" ? Square.Environment.Production : Square.Environment.Sandbox)
                                .AccessToken(squareAccessToken)
                                .Build();
                            long amountCents = (long)(objEventUserInfo.AmountPaid * 100);
                            var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();
                            var createPaymentBody = new Square.Models.CreatePaymentRequest.Builder(squareNonce, Guid.NewGuid().ToString(), money)
                                .LocationId(squareLocationId)
                                .Note($"Event Registration - {objEventUserInfo.EventCategory} | {objEventUserInfo.FirstName} {objEventUserInfo.LastName}")
                                .Build();

                            try
                            {
                                var squareResponse = await squareClient.PaymentsApi.CreatePaymentAsync(createPaymentBody);
                                if (squareResponse.Payment != null && squareResponse.Payment.Status == "COMPLETED")
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Square Payment",
                                        LogDescription = "Square Payment squareResponse Status  COMPLETED",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    string squareTransactionId = squareResponse.Payment.Id;

                                    // ✅ Session fix
                                    HttpContext.Session.SetString("CTotalAmount", objEventUserInfo.AmountPaid.ToString());

                                    string Username = !string.IsNullOrEmpty(objEventUserInfo.FirstName)
                                        ? objEventUserInfo.FirstName + " " + objEventUserInfo.LastName
                                        : User.Identity?.Name ?? "";


                                    objEventOrderDetails.PaymentMethod = "Square";
                                    objEventOrderDetails.TransactionId = squareTransactionId;
                                    objEventOrderDetails.PaymentStatus = "Completed";
                                    objEventOrderDetails.EventUserInfoId = EventUserInfoId;
                                    objEventUserInfo.PaymentStatusId = 1;
                                    objEventUserInfo.PaymentMethodId = 14;
                                    objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                                    objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;
                                    objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                                    objEventUserInfo.PaymentMethod = "Square";
                                    objEventUserInfo.PaymentStatus = objEventUserInfo.PaymentStatus;
                                    objEventUserInfo.TransactionId = squareTransactionId;
                                    objEventUserInfo.EventUserInfoId = EventUserInfoId;
                                    objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                                    //objEventUserInfo.PaymentDate = DateTime.UtcNow;
                                    objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



                                    objEventUserInfo.Email = objEventUserInfo.Email;
                                    Int64 result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Event Registration",
                                        LogDescription = "Updated EventUserInfo Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    int status = 0;
                                    AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                                    objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
                                    int _qstatus = 0;

                                    List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                                    List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                                    Entities.Events objEvents = new Entities.Events();
                                    List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                                    objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);


                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Get EventUserInfo Details",
                                        LogDescription = "Get By EventUserInfo Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    DateTime startDateTime = objEventUserInfo.StartDate;
                                    DateTime endDateTime = objEventUserInfo.EndDate;


                                    if (objEventUserInfo.StartDate != DateTime.MinValue)
                                    {
                                        startDateTime = objEventUserInfo.StartDate.Date
                                                        .AddHours(objEventUserInfo.StartDate.Hour)
                                                        .AddMinutes(objEventUserInfo.StartDate.Minute);
                                    }




                                    if (objEventUserInfo.EndDate != DateTime.MinValue)
                                    {
                                        endDateTime = objEventUserInfo.EndDate.Date
                                                        .AddHours(objEventUserInfo.EndDate.Hour)
                                                        .AddMinutes(objEventUserInfo.EndDate.Minute);
                                    }



                                    // Step 2: Generate iCalendar content
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("BEGIN:VCALENDAR");
                                    sb.AppendLine("VERSION:2.0");
                                    //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                    sb.AppendLine("PRODID:-//Event Booking//EN");

                                    sb.AppendLine("METHOD:REQUEST");
                                    sb.AppendLine("BEGIN:VEVENT");
                                    sb.AppendLine("UID:" + Guid.NewGuid());
                                    sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
                                    sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
                                    sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
                                    // sb.AppendLine("LOCATION:Temple");
                                    sb.AppendLine("STATUS:CONFIRMED");
                                    sb.AppendLine("END:VEVENT");
                                    sb.AppendLine("END:VCALENDAR");

                                    string icsContent = sb.ToString();

                                    // Step 3: Convert to byte stream
                                    byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                    MemoryStream stream = new MemoryStream(byteArray);
                                    Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");

                                    string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                "<tr>" +
                "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>"+
                
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
                                                       "<td align=\"center\">$" + item.Amount + "</td>"+
                                                    
                                                       "</tr>";

                                            i++;
                                        }
                                    }

                                    tickets += "</table>";

                                    if (objAppInfo.IsQRCode == "Yes")
                                    {

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Entering Is QRCode",
                                            LogDescription = "Get By EventUserInfo Details",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                        GenerateQRCodepdf(EventUserInfoId, objEventUserInfo.EventId, ref lstEventRegTickets);
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Successfully  QRCode Generate",
                                            LogDescription = "Successfully  QRCode Generate",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }
                                    List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

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
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "TicketinG PDF FAILED",
                                                LogDescription = pdfEx.Message,
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

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
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "ticket Count FAILED",
                                                LogDescription = pdfEx.Message,
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });


                                        }
                                    }
                                    //Send Mail to User
                                    string BaseUrl = objAppInfo.UserSiteUrl;
                                    int MailStatus = 0;
                                    Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                    objMailTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref MailStatus);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                        body.Replace("[Email]", objEventUserInfo.Email);
                                        body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                        body.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                                        body.Replace("[PaymentType]", "Square");
                                        body.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                        body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                        body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                                        decimal totalamout = 0;

                                        if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                        {
                                            totalamout = lstEventRegCounts.Sum(x => x.Amount);
                                        }

                                        // If value0, pass default value
                                        body.Replace("[TotalRegistrationAmount]",
                                            totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                                        // Early Bird Amount Calculation
                                        decimal totalEarlyBird = 0;

                                        if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                        {
                                            totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                                        }

                                        // If value is 0, pass default value
                                        body.Replace("[EarlyBirdAmount]",
                                            totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");
                                        body = body.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[Tickets]", tickets);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                                        if (objAppInfo.IsQRCode == "Yes")
                                        {
                                            if (objEventUserInfo.IsQRCode == "Single")
                                            {
                                                string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                                                string RefCode = cleanSiteName + EventUserInfoId;
                                                string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                                var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                                                body = body.Replace("[QRCode]", image1);



                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "QRCode Single",
                                                    LogDescription = "QRCode Single GENERATED",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });
                                            }
                                            else if (objEventUserInfo.IsQRCode == "Multiple")
                                            {
                                                string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(EventUserInfoId.ToString());
                                                var button = "<a href='" + url + "' target='_blank' " +
                                                            "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                                             "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                                             "Event Tickets QR Codes</a>";

                                                body = body.Replace("[QRCode]", button);
                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "QRCode Multiple",
                                                    LogDescription = "QRCode Multiple GENERATED",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });
                                            }
                                        }


                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "User Email Sent",
                                                LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }

                                    }
                                    //Send Mail to Admin
                                    int statusAdmin = 0;
                                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
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
                                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                        body1.Replace("[Email]", objEventUserInfo.Email);

                                        body1.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                                        body1.Replace("[PaymentType]", "Square");
                                        body1.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                        body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                        body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                                        body1 = body1.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[Tickets]", tickets);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                        if (objEvents.ContactEmail != null && objEvents.ContactEmail != "")
                                        {
                                            if (objAppInfo.Email == "Yes")
                                            {
                                                var responsce = _sentmail.SendMailWithAttachmentICO(objEvents.ContactEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                                            }

                                        }
                                        else
                                        {
                                            if (objAppInfo.Email == "Yes")
                                            {
                                                var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                                                // ✅ Check if email was successful or failed
                                                bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                                    LogDescription = isEmailSuccess
                                                        ? $"Confirmation email sent successfully to Admin: {objAppInfo.EventsEmail}"
                                                        : $"Confirmation email FAILED for Admin: {objAppInfo.EventsEmail} | Response: {responsce}",

                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });
                                            }

                                        }

                                    }
                                    return RedirectToAction("ThankYou", "Event", new { id = EventUserInfoId, PaymentMethod = "Square", amt = objEventUserInfo.AmountPaid });


                                }
                                else
                                {
                                    string statusMsg = squareResponse.Payment?.Status ?? "null";

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "squre Payment Failed",
                                        LogDescription = "squre Payment Failed" + statusMsg,
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    return RedirectToAction("PaymentFailed", "Home");
                                }
                            }
                            catch (Square.Exceptions.ApiException sqEx)
                            {
                                string errorCode = sqEx.Errors?.FirstOrDefault()?.Code ?? "UNKNOWN";
                                string errorDetail = sqEx.Errors?.FirstOrDefault()?.Detail ?? sqEx.Message;

                                string userMessage = errorCode switch
                                {
                                    "INVALID_CARD" => "Invalid card number. Please check and try again.",
                                    "CVV_FAILURE" => "Invalid CVV. Please check and try again.",
                                    "ADDRESS_VERIFICATION_FAILURE" => "Invalid zip code. Please check and try again.",
                                    "EXPIRATION_FAILURE" => "Card has expired. Please use a different card.",
                                    "CARD_DECLINED" => "Card was declined. Please use a different card.",
                                    "CARD_DECLINED_CALL_ISSUER" => "Card declined. Please contact your bank.",
                                    "INSUFFICIENT_FUNDS" => "Insufficient funds. Please use a different card.",
                                    "INVALID_EXPIRATION" => "Invalid expiration date. Please check and try again.",
                                    _ => $"Payment failed: {errorDetail}"
                                };

                                TempData["message"] = $"<div class=\"error1 closable\">{userMessage}</div>";

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Charge FAILED",
                                    LogDescription = $"Code:{errorCode} | Detail:{errorDetail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }

                        else if (objEventUserInfo.PaymentMethod == "Credit Card")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Credit Card Payment",
                                LogDescription = "Credit Card Payment started",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            try
                            {
                                // ✅ 1. Get Amount safely
                                decimal amount = objEventUserInfo.AmountPaid;

                                var amountValue = Request.Form["Amount"].FirstOrDefault();

                                if (!string.IsNullOrWhiteSpace(amountValue) &&
                                    decimal.TryParse(amountValue,
                                        System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        out decimal parsedAmount))
                                {
                                    amount = parsedAmount;
                                }

                                // ❗ Validate amount
                                if (amount <= 0)
                                {
                                    TempData["message"] = "Invalid payment amount.";
                                    return RedirectToAction("Index", "Home");
                                }

                                // ✅ 2. Get Braintree Nonce (Core way)
                                string nonce = Request.Form["payment_method_nonce"].FirstOrDefault();

                                if (string.IsNullOrEmpty(nonce))
                                {
                                    TempData["message"] = "Payment authorization failed. Please try again.";
                                    return RedirectToAction("Index", "Home");
                                }

                                // ✅ 3. Create Transaction
                                var request = new TransactionRequest
                                {
                                    Amount = amount,
                                    PaymentMethodNonce = nonce,
                                    Options = new TransactionOptionsRequest
                                    {
                                        SubmitForSettlement = true
                                    }
                                };

                                var result = gateway.Transaction.Sale(request);

                                // ✅ 4. Success
                                if (result.IsSuccess())
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Payment IsSuccess  ",
                                        LogDescription = "credit card Deatils IsSuccess condition ",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    var transaction = result.Target;
                                    objEventOrderDetails.PaymentMethod = "Credit Card";
                                    objEventOrderDetails.TransactionId = transaction.Id;
                                    objEventOrderDetails.PaymentStatus = objEventUserInfo.PaymentStatus;
                                    objEventOrderDetails.EventUserInfoId = EventUserInfoId;
                                    objEventUserInfo.PaymentStatusId = 1;
                                    objEventUserInfo.PaymentMethodId = 13;
                                    objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                                    objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;
                                    objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                                    objEventUserInfo.PaymentMethod = "Credit Card";
                                    objEventUserInfo.PaymentStatus = objEventUserInfo.PaymentStatus;
                                    objEventUserInfo.TransactionId = transaction.Id;
                                    objEventUserInfo.EventUserInfoId = EventUserInfoId;
                                    objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                                    //objEventUserInfo.PaymentDate = DateTime.UtcNow;
                                    objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objEventUserInfo.Email = objEventUserInfo.Email;
                                    Int64 result1 = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Event Registration",
                                        LogDescription = "Updated EventUserInfo Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    if (result1 == 1)
                                    {

                                        int status = 0;
                                        AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                                        objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
                                        int _qstatus = 0;

                                        List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                                        List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                                        Entities.Events objEvents = new Entities.Events();
                                        List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                                        objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);


                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Get EventUserInfo Details",
                                            LogDescription = "Get By EventUserInfo Details",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                        DateTime startDateTime = objEventUserInfo.StartDate;
                                        DateTime endDateTime = objEventUserInfo.EndDate;


                                        if (objEventUserInfo.StartDate != DateTime.MinValue)
                                        {
                                            startDateTime = objEventUserInfo.StartDate.Date
                                                            .AddHours(objEventUserInfo.StartDate.Hour)
                                                            .AddMinutes(objEventUserInfo.StartDate.Minute);
                                        }




                                        if (objEventUserInfo.EndDate != DateTime.MinValue)
                                        {
                                            endDateTime = objEventUserInfo.EndDate.Date
                                                            .AddHours(objEventUserInfo.EndDate.Hour)
                                                            .AddMinutes(objEventUserInfo.EndDate.Minute);
                                        }



                                        // Step 2: Generate iCalendar content
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine("BEGIN:VCALENDAR");
                                        sb.AppendLine("VERSION:2.0");
                                        //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                        sb.AppendLine("PRODID:-//Event Booking//EN");

                                        sb.AppendLine("METHOD:REQUEST");
                                        sb.AppendLine("BEGIN:VEVENT");
                                        sb.AppendLine("UID:" + Guid.NewGuid());
                                        sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                        sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                        sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                        sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
                                        sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
                                        sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
                                        // sb.AppendLine("LOCATION:Temple");
                                        sb.AppendLine("STATUS:CONFIRMED");
                                        sb.AppendLine("END:VEVENT");
                                        sb.AppendLine("END:VCALENDAR");

                                        string icsContent = sb.ToString();

                                        // Step 3: Convert to byte stream
                                        byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                        MemoryStream stream = new MemoryStream(byteArray);
                                        Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");

                                        string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                 "<tr>" +
                 "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                 "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                 "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                 "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>" +
                 "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Early Bird Amount</span></th>" +
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
                                                           "<td align=\"center\">$" + item.EarlyBirdMemberAmount + "</td>" +
                                                           "</tr>";

                                                i++;
                                            }
                                        }

                                        tickets += "</table>";

                                        if (objAppInfo.IsQRCode == "Yes")
                                        {

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Entering Is QRCode",
                                                LogDescription = "Get By EventUserInfo Details",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                            GenerateQRCodepdf(EventUserInfoId, objEventUserInfo.EventId, ref lstEventRegTickets);
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Successfully  QRCode Generate",
                                                LogDescription = "Successfully  QRCode Generate",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                        List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

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
                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "Event ticket  Saved",
                                                    LogDescription = "Event ticket  Saved In folder",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });

                                            }
                                            catch (Exception pdfEx)
                                            {
                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "TicketinG PDF FAILED",
                                                    LogDescription = pdfEx.Message,
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });

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
                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "Event ticket  Genetrated",
                                                    LogDescription = "Event ticket Genetrated",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });


                                            }
                                            catch (Exception pdfEx)
                                            {
                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "ticket Count FAILED",
                                                    LogDescription = pdfEx.Message,
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });


                                            }
                                        }
                                        //Send Mail to User
                                        string BaseUrl = objAppInfo.UserSiteUrl;
                                        int MailStatus = 0;
                                        Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                        objMailTemplates = _MailTemplates.GetMailTemplateById("Event Registrations ", 0, ref MailStatus);
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
                                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                            body.Replace("[Email]", objEventUserInfo.Email);
                                            body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                            body.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                                            body.Replace("[PaymentType]", "Credit Card");
                                            body.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                            body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                            body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                                            decimal totalamout = 0;

                                            if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                            {
                                                totalamout = lstEventRegCounts.Sum(x => x.Amount);
                                            }

                                            // If value0, pass default value
                                            body.Replace("[TotalRegistrationAmount]",
                                                totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                                            // Early Bird Amount Calculation
                                            decimal totalEarlyBird = 0;

                                            if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                            {
                                                totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                                            }

                                            // If value is 0, pass default value
                                            body.Replace("[EarlyBirdAmount]",
                                                totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");
                                            body = body.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body.Replace("[SiteName]", objAppInfo.SiteName);
                                            body.Replace("[SiteName]", objAppInfo.SiteName);
                                            body.Replace("[Tickets]", tickets);
                                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                                            if (objAppInfo.IsQRCode == "Yes")
                                            {
                                                if (objEventUserInfo.IsQRCode == "Single")
                                                {
                                                    string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                                                    string RefCode = cleanSiteName + EventUserInfoId;
                                                    string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                                    var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                                                    body = body.Replace("[QRCode]", image1);
                                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                                    {
                                                        LogId = LogId,
                                                        LogTitle = "QRCode Single",
                                                        LogDescription = "QRCode Single GENERATED",
                                                        LogDate = DateTime.Now,
                                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        InsertedDate = DateTime.Now,
                                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                    });
                                                }
                                                else if (objEventUserInfo.IsQRCode == "Multiple")
                                                {
                                                    string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(EventUserInfoId.ToString());
                                                    var button = "<a href='" + url + "' target='_blank' " +
                                                                "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                                                 "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                                                 "Event Tickets QR Codes</a>";

                                                    body = body.Replace("[QRCode]", button);
                                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                                    {
                                                        LogId = LogId,
                                                        LogTitle = "QRCode Multiple",
                                                        LogDescription = "QRCode Multiple GENERATED",
                                                        LogDate = DateTime.Now,
                                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        InsertedDate = DateTime.Now,
                                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                    });
                                                }
                                            }


                                            if (objAppInfo.Email == "Yes")
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString());

                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "User Email Sent",
                                                    LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });
                                            }

                                        }
                                        //Send Mail to Admin
                                        int statusAdmin = 0;
                                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
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
                                            body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                            body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                            body1.Replace("[Email]", objEventUserInfo.Email);

                                            body1.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                                            body1.Replace("[PaymentType]", "Credit Card");
                                            body1.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                            body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                            body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                                            body1.Replace("[PromoDiscount]", objEventUserInfo.Gender);
                                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                                            body1.Replace("[Tickets]", tickets);
                                            body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                            if (objEvents.ContactEmail != null && objEvents.ContactEmail != "")
                                            {
                                                if (objAppInfo.Email == "Yes")
                                                {
                                                    var responsce = _sentmail.SendMailWithAttachmentICO(objEvents.ContactEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                                                }

                                            }
                                            else
                                            {
                                                if (objAppInfo.Email == "Yes")
                                                {
                                                    var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                                                    // ✅ Check if email was successful or failed
                                                    bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                                    {
                                                        LogId = LogId,
                                                        LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                                        LogDescription = isEmailSuccess
                                                            ? $"Confirmation email sent successfully to Admin: {objAppInfo.EventsEmail}"
                                                            : $"Confirmation email FAILED for Admin: {objAppInfo.EventsEmail} | Response: {responsce}",

                                                        LogDate = DateTime.Now,
                                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        InsertedDate = DateTime.Now,
                                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                    });
                                                }

                                            }

                                            return RedirectToAction("ThankYou", "Event", new { Id = EventUserInfoId, cname = cname, PaymentMethod = "Credit Card" });

                                            //return RedirectToAction("ThankYou", "Event", new { id = EventUserInfoId, cname = cname, PaymentMethod = objEventUserInfo.PaymentMethod, tid = objEventUserInfo.TransactionId });

                                        }
                                    }
                                    else
                                    {
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Payment Failed ",
                                            LogDescription = "credit card Payment Deatails Inserting Failed",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                        string errors = string.Join("\n",
                                        result.Errors.DeepAll().Select(e => $"Error {(int)e.Code}: {e.Message}"));

                                        TempData["message"] = errors;

                                        return RedirectToAction("PaymentFailed", "Home");
                                    }
                                }

                                else
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Payment Failed ",
                                        LogDescription = "credit card Deatils Invalid ",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    string errors = string.Join("\n",
                                    result.Errors.DeepAll().Select(e => $"Error {(int)e.Code}: {e.Message}"));

                                    TempData["message"] = errors;

                                    return RedirectToAction("PaymentFailed", "Home");
                                }
                            }
                            catch (Exception ex)
                            {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Payment Failed ",
                                    LogDescription = "Payment error: " + ex.Message,
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                TempData["message"] = "Payment error: " + ex.Message;
                                return RedirectToAction("PaymentFailed", "Home");

                            }
                        }


                        /*Paypal - Payment process in Client Side*/
                        else if (objEventUserInfo.PaymentMethod == "PayPal")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "PayPal Payment ",
                                LogDescription = "Payment PayPal PaymentMethod Entering",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            decimal paymentAmount = objEventUserInfo.AmountPaid;


                            string Username = "";

                            if (objEventUserInfo.FirstName != null)
                            {
                                Username = objEventUserInfo.FirstName + " " + objEventUserInfo.LastName;
                            }
                            else
                            {
                                Username = HttpContext.User.Identity.Name.ToString();
                            }
                            if (Username != null)
                            {
                                objEventOrderDetails.PaymentMethod = "PayPal";
                                objEventOrderDetails.TransactionId = objEventUserInfo.TransactionId;
                                objEventOrderDetails.PaymentStatus = objEventUserInfo.PaymentStatus;
                                objEventOrderDetails.EventUserInfoId = EventUserInfoId;
                                objEventUserInfo.PaymentStatusId = 1;
                                objEventUserInfo.PaymentMethodId = 1;
                                objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                                objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;
                                objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                                objEventUserInfo.PaymentMethod = "PayPal";
                                objEventUserInfo.PaymentStatus = objEventUserInfo.PaymentStatus;
                                objEventUserInfo.TransactionId = objEventUserInfo.TransactionId;
                                objEventUserInfo.EventUserInfoId = EventUserInfoId;
                                objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                                //objEventUserInfo.PaymentDate = DateTime.UtcNow;
                                objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



                                objEventUserInfo.Email = objEventUserInfo.Email;
                                Int64 result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Event Registration",
                                    LogDescription = "Updated EventUserInfo Details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                int status = 0;
                                AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                                objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
                                int _qstatus = 0;

                                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                                Entities.Events objEvents = new Entities.Events();
                                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);


                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Get EventUserInfo Details",
                                    LogDescription = "Get By EventUserInfo Details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                DateTime startDateTime = objEventUserInfo.StartDate;
                                DateTime endDateTime = objEventUserInfo.EndDate;


                                if (objEventUserInfo.StartDate != DateTime.MinValue)
                                {
                                    startDateTime = objEventUserInfo.StartDate.Date
                                                    .AddHours(objEventUserInfo.StartDate.Hour)
                                                    .AddMinutes(objEventUserInfo.StartDate.Minute);
                                }




                                if (objEventUserInfo.EndDate != DateTime.MinValue)
                                {
                                    endDateTime = objEventUserInfo.EndDate.Date
                                                    .AddHours(objEventUserInfo.EndDate.Hour)
                                                    .AddMinutes(objEventUserInfo.EndDate.Minute);
                                }



                                // Step 2: Generate iCalendar content
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("BEGIN:VCALENDAR");
                                sb.AppendLine("VERSION:2.0");
                                //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                sb.AppendLine("PRODID:-//Event Booking//EN");

                                sb.AppendLine("METHOD:REQUEST");
                                sb.AppendLine("BEGIN:VEVENT");
                                sb.AppendLine("UID:" + Guid.NewGuid());
                                sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
                                sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
                                sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
                                // sb.AppendLine("LOCATION:Temple");
                                sb.AppendLine("STATUS:CONFIRMED");
                                sb.AppendLine("END:VEVENT");
                                sb.AppendLine("END:VCALENDAR");

                                string icsContent = sb.ToString();

                                // Step 3: Convert to byte stream
                                byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                MemoryStream stream = new MemoryStream(byteArray);
                                Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");

                                string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                  "<tr>" +
                  "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                  "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                  "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                  "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>" +
                  "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Early Bird Amount</span></th>" +
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
                                                   "<td align=\"center\">$" + item.EarlyBirdMemberAmount + "</td>" +
                                                   "</tr>";

                                        i++;
                                    }
                                }

                                tickets += "</table>";
                                //tickets = tickets + "</table>";

                                if (objAppInfo.IsQRCode == "Yes")
                                {

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Entering Is QRCode",
                                        LogDescription = "Get By EventUserInfo Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    GenerateQRCodepdf(EventUserInfoId, objEventUserInfo.EventId, ref lstEventRegTickets);
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Successfully  QRCode Generate",
                                        LogDescription = "Successfully  QRCode Generate",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                                List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

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
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "TicketinG PDF FAILED",
                                            LogDescription = pdfEx.Message,
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });

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
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "ticket Count FAILED",
                                            LogDescription = pdfEx.Message,
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });


                                    }
                                }
                                //Send Mail to User
                                string BaseUrl = objAppInfo.UserSiteUrl;
                                int MailStatus = 0;
                                Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                objMailTemplates = _MailTemplates.GetMailTemplateById("Event Registrations ", 0, ref MailStatus);
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
                                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body.Replace("[Email]", objEventUserInfo.Email);
                                    body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body = body.Replace(
                                        "[TransactionId]",
                                        string.IsNullOrWhiteSpace(objEventUserInfo.TransactionId) ? "N/A" : objEventUserInfo.TransactionId
                                    ); body.Replace("[PaymentType]", "PayPal");
                                    body.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                    body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    decimal totalamout = 0;

                                    if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                    {
                                        totalamout = lstEventRegCounts.Sum(x => x.Amount);
                                    }

                                    // If value0, pass default value
                                    body.Replace("[TotalRegistrationAmount]",
                                        totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                                    // Early Bird Amount Calculation
                                    decimal totalEarlyBird = 0;

                                    if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                                    {
                                        totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                                    }

                                    // If value is 0, pass default value
                                    body.Replace("[EarlyBirdAmount]",
                                        totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");
                                    body = body.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender);
                                    body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                                    body.Replace("[SiteName]", objAppInfo.SiteName);
                                    body.Replace("[Tickets]", tickets);
                                    body.Replace("[Year]", DateTime.Now.Year.ToString());

                                    if (objAppInfo.IsQRCode == "Yes")
                                    {
                                        if (objEventUserInfo.IsQRCode == "Single")
                                        {
                                            string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                                            string RefCode = cleanSiteName + EventUserInfoId;
                                            string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                            var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                                            body = body.Replace("[QRCode]", image1);

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "QRCode Single",
                                                LogDescription = "QRCode Single GENERATED",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                        else if (objEventUserInfo.IsQRCode == "Multiple")
                                        {
                                            string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(EventUserInfoId.ToString());
                                            var button = "<a href='" + url + "' target='_blank' " +
                                                        "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                                         "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                                         "Event Tickets QR Codes</a>";

                                            body = body.Replace("[QRCode]", button);
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "QRCode Multiple",
                                                LogDescription = "QRCode Multiple GENERATED",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }


                                    if (objAppInfo.Email == "Yes")
                                    {
                                        var responsce = _sentmail.SendMailSendinbrevowithfrom(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString());

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "User Email Sent",
                                            LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }

                                }
                                //Send Mail to Admin
                                int statusAdmin = 0;
                                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
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
                                    body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body1.Replace("[Email]", objEventUserInfo.Email);

                                    body1.Replace("[TransactionId]", objEventUserInfo.objEventOrderDetails.TransactionId);
                                    body1.Replace("[PaymentType]", "PayPal");
                                    body1.Replace("[PaymentStatus]", objEventUserInfo.objEventOrderDetails.PaymentStatus);
                                    body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                                    body1 = body1.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body1.Replace("[SiteName]", objAppInfo.SiteName);
                                    body1.Replace("[SiteName]", objAppInfo.SiteName);
                                    body1.Replace("[Tickets]", tickets);
                                    body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                    if (objEvents.ContactEmail != null && objEvents.ContactEmail != "")
                                    {
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailWithAttachmentICO(objEvents.ContactEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                                        }

                                    }
                                    else
                                    {
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                                            // ✅ Check if email was successful or failed
                                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                                LogDescription = isEmailSuccess
                                                    ? $"Confirmation email sent successfully to Admin: {objAppInfo.EventsEmail}"
                                                    : $"Confirmation email FAILED for Admin: {objAppInfo.EventsEmail} | Response: {responsce}",

                                                LogDate = DateTime.Now,
                                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }

                                    }

                                    var redirectUrl = Url.Action("ThankYou", "Event", new
                                    {
                                        area = "User",
                                        id = EventUserInfoId,
                                        cname = cname,
                                        PaymentMethod = objEventUserInfo.PaymentMethod,
                                        tid = objEventUserInfo.TransactionId
                                    });
                                    return Json(new { success = true, redirectUrl = redirectUrl });

                                    //return RedirectToAction("ThankYou", "Event", new { id = EventUserInfoId, cname = cname, PaymentMethod = objEventUserInfo.PaymentMethod, tid = objEventUserInfo.TransactionId });

                                }
                            }
                        }
                        /*Cash/Cheque - Payment either a Cash (or) Cheque*/
                        else if (objEventUserInfo.PaymentMethod == "Cash/Cheque")
                        {

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Cash/Cheque Payment ",
                                LogDescription = $"Entered into Cash/Cheque Donation Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            objEventOrderDetails.PaymentMethod = "Cash/Cheque";
                            objEventOrderDetails.TransactionId = "NA";
                            objEventOrderDetails.PaymentStatus = "Pending";
                            objEventOrderDetails.EventUserInfoId = EventUserInfoId;
                            objEventUserInfo.PaymentStatusId = 2;
                            objEventUserInfo.PaymentMethodId = 3;
                            objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                            objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;

                            objEventUserInfo.PaymentStatusId = 2;
                            objEventUserInfo.PaymentMethodId = 3;
                            objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                            objEventUserInfo.PaymentMethod = objEventOrderDetails.PaymentMethod;
                            objEventUserInfo.PaymentStatus = "Pending";
                            objEventUserInfo.TransactionId = "NA";
                            objEventUserInfo.EventUserInfoId = EventUserInfoId;
                            objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                            //objEventUserInfo.PaymentDate = objEventOrderDetails.PaymentDate;
                            objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                            Int64 result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Updated Cash/Cheque PAYMENT DEATILS EventUserInfo",
                                LogDescription = $"Updated Cash/Cheque PAYMENT DEATILS EventUserInfo",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            if (result == 1)
                            {
                                int _qstatus = 0;
                                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                                Entities.Events objEvents = new Entities.Events();
                                // Entities.EventUserInfo objUser = new Entities.EventUserInfo();
                                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);



                                DateTime startDateTime = objEventUserInfo.StartDate;
                                DateTime endDateTime = objEventUserInfo.EndDate;


                                if (objEventUserInfo.StartDate != DateTime.MinValue)
                                {
                                    startDateTime = objEventUserInfo.StartDate.Date
                                                    .AddHours(objEventUserInfo.StartDate.Hour)
                                                    .AddMinutes(objEventUserInfo.StartDate.Minute);
                                }




                                if (objEventUserInfo.EndDate != DateTime.MinValue)
                                {
                                    endDateTime = objEventUserInfo.EndDate.Date
                                                    .AddHours(objEventUserInfo.EndDate.Hour)
                                                    .AddMinutes(objEventUserInfo.EndDate.Minute);
                                }



                                // Step 2: Generate iCalendar content
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("BEGIN:VCALENDAR");
                                sb.AppendLine("VERSION:2.0");
                                //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                sb.AppendLine("PRODID:-//Event Booking//EN");

                                sb.AppendLine("METHOD:REQUEST");
                                sb.AppendLine("BEGIN:VEVENT");
                                sb.AppendLine("UID:" + Guid.NewGuid());
                                sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
                                sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
                                sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
                                // sb.AppendLine("LOCATION:Temple");
                                sb.AppendLine("STATUS:CONFIRMED");
                                sb.AppendLine("END:VEVENT");
                                sb.AppendLine("END:VCALENDAR");

                                string icsContent = sb.ToString();

                                // Step 3: Convert to byte stream
                                byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                MemoryStream stream = new MemoryStream(byteArray);
                                Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");

                                string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Amount</span></th></tr>";

                                if (lstEventRegCounts != null && lstEventRegCounts.Count != 0)
                                {
                                    int i = 1;
                                    foreach (var item in lstEventRegCounts)
                                    {
                                        tickets = tickets + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.RegistrationTitle + "</td><td align=\"center\">" + item.Count + "</td><td align=\"center\">$" + item.Amount + "</td></tr>";
                                        i++;
                                    }
                                }
                                tickets = tickets + "</table>";

                                //Send Mail to User
                                string BaseUrl = objAppInfo.UserSiteUrl;
                                int MailStatus = 0;
                                Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref MailStatus);
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
                                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body.Replace("[Email]", objEventUserInfo.Email);
                                    body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body.Replace("[TransactionId]", "NA");
                                    body.Replace("[PaymentType]", "Cash/Cheque");
                                    body.Replace("[PaymentStatus]", "Pending");
                                    body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                                    body.Replace("[SiteName]", objAppInfo.SiteName);
                                    body.Replace("[Tickets]", tickets);
                                    body.Replace("[Year]", DateTime.Now.Year.ToString());

                                    //string subject = objTemplates.Subject;
                                    //subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                    if (objAppInfo.IsQRCode == "Yes")
                                    {
                                        string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
                                        string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                        var image1 = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
                                        body.Replace("[QRCode]", image1);
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "QRCode",
                                            LogDescription = "IsQRCode Generated",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }
                                    if (objEvents.EmaisToBrevo == "Yes")
                                    {
                                        if (Notifications == "Yes")
                                        {
                                            int[] arr = { 3 };
                                            BLL.Common.AddContactList(objEventUserInfo.Email, objEventUserInfo.FirstName, objEventUserInfo.LastName, arr, objEventUserInfo.Mobile);
                                        }
                                    }
                                    if (objAppInfo.Email == "Yes")
                                    {
                                        var responsce = _sentmail.SendMailWithAttachmentICO(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString(), calendarAttachment);
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "User Email Sent",
                                            LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }

                                }
                                //Send Mail to Admin
                                int statusAdmin = 0;
                                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Event Registration Admin", 0, ref statusAdmin);
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
                                    body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body1.Replace("[Email]", objEventUserInfo.Email);
                                    body1.Replace("[TransactionId]", "NA");
                                    body1.Replace("[PaymentType]", "Cash/Cheque");
                                    body1.Replace("[PaymentStatus]", "Pending");
                                    body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                                    body1.Replace("[PromoDiscount]", objEventUserInfo.Gender);
                                    body1.Replace("[SiteName]", objAppInfo.SiteName);
                                    body1.Replace("[Tickets]", tickets);
                                    body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                    if (objAppInfo.Email == "Yes")
                                    {
                                        _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Admin Email Sent",
                                            LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }
                                }
                                return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", Id = EventUserInfoId, cname = cname, PaymentMethod = "Cash/Cheque" });

                            }
                        }

                        /*Zelle - Saving zelle transaction details*/
                        else if (objEventUserInfo.PaymentMethod == "Zelle")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Zelle Payment ",
                                LogDescription = $"Entered Into Zelle Donation Proces",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            objEventOrderDetails.PaymentMethod = "Zelle";
                            objEventOrderDetails.TransactionId = objEventUserInfo.TransactionId;
                            objEventOrderDetails.PaymentStatus = "Pending";
                            objEventOrderDetails.EventUserInfoId = EventUserInfoId;
                            objEventUserInfo.PaymentStatusId = 2;

                            objEventUserInfo.PaymentMethodId = 6;
                            objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                            objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;

                            objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                            objEventUserInfo.PaymentMethod = objEventOrderDetails.PaymentMethod;
                            objEventUserInfo.PaymentStatus = "Pending";
                            objEventUserInfo.TransactionId = "NA"; ;
                            objEventUserInfo.EventUserInfoId = EventUserInfoId;
                            objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                            //objEventUserInfo.PaymentDate = objEventOrderDetails.PaymentDate;
                            objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                            Int64 result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Updated EventUserInfo Details",
                                LogDescription = $"Updated EventUserInfo Details",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            if (result == 1)
                            {
                                int _qstatus = 0;
                                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                                Entities.Events objEvents = new Entities.Events();
                                //Entities.EventUserInfo objUser = new Entities.EventUserInfo();
                                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);




                                DateTime startDateTime = objEventUserInfo.StartDate;
                                DateTime endDateTime = objEventUserInfo.EndDate;


                                if (objEventUserInfo.StartDate != DateTime.MinValue)
                                {
                                    startDateTime = objEventUserInfo.StartDate.Date
                                                    .AddHours(objEventUserInfo.StartDate.Hour)
                                                    .AddMinutes(objEventUserInfo.StartDate.Minute);
                                }

                                if (objEventUserInfo.EndDate != DateTime.MinValue)
                                {
                                    endDateTime = objEventUserInfo.EndDate.Date
                                                    .AddHours(objEventUserInfo.EndDate.Hour)
                                                    .AddMinutes(objEventUserInfo.EndDate.Minute);
                                }

                                // Step 2: Generate iCalendar content
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("BEGIN:VCALENDAR");
                                sb.AppendLine("VERSION:2.0");
                                //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                sb.AppendLine("PRODID:-//Event Booking//EN");

                                sb.AppendLine("METHOD:REQUEST");
                                sb.AppendLine("BEGIN:VEVENT");
                                sb.AppendLine("UID:" + Guid.NewGuid());
                                sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
                                sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
                                sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
                                // sb.AppendLine("LOCATION:Temple");
                                sb.AppendLine("STATUS:CONFIRMED");
                                sb.AppendLine("END:VEVENT");
                                sb.AppendLine("END:VCALENDAR");

                                string icsContent = sb.ToString();

                                // Step 3: Convert to byte stream
                                byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                MemoryStream stream = new MemoryStream(byteArray);
                                Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");


                                string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Amount</span></th></tr>";

                                if (lstEventRegCounts != null && lstEventRegCounts.Count != 0)
                                {
                                    int i = 1;
                                    foreach (var item in lstEventRegCounts)
                                    {
                                        tickets = tickets + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.RegistrationTitle + "</td><td align=\"center\">" + item.Count + "</td><td align=\"center\">$" + item.Amount + "</td></tr>";
                                        i++;
                                    }
                                }
                                tickets = tickets + "</table>";

                                //Send Mail to User
                                string BaseUrl = objAppInfo.UserSiteUrl;
                                int MailStatus = 0;
                                Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref MailStatus);
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
                                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body.Replace("[Email]", objEventUserInfo.Email);
                                    body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body.Replace("[TransactionId]", "N/A");
                                    body.Replace("[PaymentType]", "Zelle");
                                    body.Replace("[PaymentStatus]", "Pending");
                                    body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                                    body.Replace("[SiteName]", objAppInfo.SiteName);
                                    body.Replace("[Tickets]", tickets);
                                    body.Replace("[Year]", DateTime.Now.Year.ToString());

                                    if (objAppInfo.IsQRCode == "Yes")
                                    {
                                        string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
                                        string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                        var image1 = "<img width=\"113\" height=\"113\" src=\"" + url + "\" />";
                                        body.Replace("[QRCode]", image1);
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "QRCode",
                                            LogDescription = "IsQRCode Generated",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }
                                    if (objEvents.EmaisToBrevo == "Yes")
                                    {
                                        if (Notifications == "Yes")
                                        {
                                            int[] arr = { 3 };
                                            BLL.Common.AddContactList(objEventUserInfo.Email, objEventUserInfo.FirstName, objEventUserInfo.LastName, arr, objEventUserInfo.Mobile);
                                        }
                                    }
                                    if (objAppInfo.Email == "Yes")
                                    {

                                        var responsce = _sentmail.SendMailWithAttachmentICO(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString(), calendarAttachment);
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "User Email Sent",
                                            LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }

                                }
                                //Send Mail to Admin
                                int statusAdmin = 0;
                                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Event Registration Admin", 0, ref statusAdmin);
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
                                    body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                    body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                                    body1.Replace("[Email]", objEventUserInfo.Email);
                                    body1.Replace("[TransactionId]", "N/A");
                                    body1.Replace("[PaymentType]", "Zelle");
                                    body1.Replace("[PaymentStatus]", "Pending");
                                    body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                                    body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());

                                    body1.Replace("[SiteName]", objAppInfo.SiteName);
                                    body1.Replace("[Tickets]", tickets);
                                    body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                    if (objAppInfo.Email == "Yes")
                                    {
                                        _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Admin Email Sent",
                                            LogDescription = $"Admin notification email sent to: {objAppInfo.EventsEmail}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }
                                }


                                return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", Id = EventUserInfoId, cname = cname, PaymentMethod = "Zelle" });
                            }
                        }
                        else if (objEventUserInfo.PaymentMethod == "Stripe")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Stripe Payment ",

                                LogDescription = $"Entered into Stripe Donation Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            Int64 result = 0;
                            objEventOrderDetails.PaymentMethod = "Stripe";
                            objEventOrderDetails.TransactionId = objEventUserInfo.TransactionId;
                            objEventOrderDetails.PaymentStatus = "Pending";
                            objEventOrderDetails.EventUserInfoId = EventUserInfoId;

                            objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                            objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;

                            objEventUserInfo.AmountPaid = objEventUserInfo.AmountPaid;
                            objEventUserInfo.PaymentMethod = objEventOrderDetails.PaymentMethod;
                            objEventUserInfo.PaymentStatus = "Pending";
                            objEventUserInfo.TransactionId = objEventUserInfo.TransactionId;

                            objEventUserInfo.EventUserInfoId = EventUserInfoId;
                            objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                            //objEventUserInfo.PaymentDate = objEventOrderDetails.PaymentDate;
                            objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



                            result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "EventUserInfo Stripe updated Details",
                                LogDescription = $"updated  Stripe EventUserInfo Details",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            int status1 = 0;
                            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status1);



                            // Set your Stripe Secret KeyB
                            StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;
                            string currency = "usd";
                            var priceOptions = new PriceCreateOptions
                            {
                                UnitAmount = (long)(objEventUserInfo.AmountPaid * 100),
                                Currency = currency.ToLower(),
                                ProductData = new PriceProductDataOptions { Name = "Sample Product" }
                            };

                            var priceService = new PriceService();
                            var price = priceService.Create(priceOptions);
                            string scheme = Request.Scheme;
                            var sessionOptions = new SessionCreateOptions
                            {
                                PaymentMethodTypes = new List<string> { "card" },
                                LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
                    },
                                Mode = "payment",
                                SuccessUrl = $"{objAppInfo.BaseUrl}User/Event/ThankYou?session_id={{CHECKOUT_SESSION_ID}}&Id={EventUserInfoId}&PaymentMethod=Stripe&cname={cname}",
                                CancelUrl = Url.Action("ThankYou", "Event", null, scheme)

                            };

                            var sessionService = new SessionService();
                            var session = sessionService.Create(sessionOptions);

                            // Store Session ID in Cookie (Fallback)
                            Response.Cookies.Append("StripeSessionId", session.Id);
                            if (result == 1)
                            {

                                if (!string.IsNullOrEmpty(session.Url))
                                {
                                    // return Json(new { success = true, paymentLinkUrl = session.Url });
                                    return Redirect(session.Url);

                                }
                            }
                        }
                        else if (objEventUserInfo.PaymentMethod == "Swire Pay")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = " Swire Pay payment",
                                LogDescription = $"Entered into Swire Pay Event Registration Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            // Initialize HttpClient and the API request
                            var client1 = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Swire Pay Payment ",
                                LogDescription = $"Entered into Card Payment Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            int status1 = 0;
                            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status1);

                            // Add API key to headers
                            request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);


                            // Prepare JSON content for the API request
                            var redirectUri = Url.Action("ThankYou", "Event", new
                            {
                                Id = EventUserInfoId,  // Ensure MemberId is set
                                PaymentMethod = "Swire Pay",
                                cname = cname
                            }, protocol: Request.Scheme);
                            decimal amount = (int)objEventUserInfo.AmountPaid;
                            decimal amountInINR = (int)objEventUserInfo.AmountPaid;
                            //decimal exchangeRate = 0.012M;  // Example exchange rate
                            decimal amountInUSD = amountInINR * 100;// Assuming this is 50
                            string formattedAmount = $"${amount}"; // This will give you "$50"

                            // Or if you want to append the $ at the end, you can do:
                            string formattedAmountWithPostfix = $"{amount}$";
                            // Prepare JSON content with interpolated values
                            var jsonContent = $@"
                                        {{
                                            ""amount"": ""{amountInUSD}"",
                                            ""currencyCode"": ""USD"",
                                            ""paymentMethodType"": [ ""CARD"" ],
                                            ""redirectUri"": ""{redirectUri}"",
                                            ""description"": ""Event Registration payment for {objEventUserInfo.FirstName + " " + objEventUserInfo.LastName}"",
                                            ""statementDescriptor"": ""Event Registration Payment"",
                                            ""expiresAt"": ""{DateTime.UtcNow.AddDays(7).AddYears(2050 - DateTime.UtcNow.Year):yyyy-MM-ddTHH:mm:ss.fffZ}"",
                                            ""email"": ""{objEventUserInfo.Email}"",
                                            ""phone"": ""{objEventUserInfo.Mobile}"",
                                            ""name"": ""{objEventUserInfo.FirstName + " " + objEventUserInfo.LastName}""
                                        }}";


                            // Set the request content
                            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                            // Send the request to the SwirePay API
                            var response = await client1.SendAsync(request);
                            response.EnsureSuccessStatusCode();  // Throws an exception if status code is not 2xx

                            // Read the response content as JSON
                            var responseBody = await response.Content.ReadAsStringAsync();

                            // Parse the response JSON to get the 'link' field
                            var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                            var paymentLink = jsonData["entity"]?["link"]?.ToString();


                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Swire Pay Payment ",
                                LogDescription = paymentLink,
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });                            // Check if the payment link is available and redirect to it
                            if (!string.IsNullOrEmpty(paymentLink))
                            {
                                return Redirect(paymentLink);  // Redirects the user to the payment link
                            }
                            else
                            {

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Swire Pay Payment ",
                                    LogDescription = "Swire Pay Payment link not found in the response.",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });


                                return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", Id = EventUserInfoId, cname = cname, PaymentMethod = objEventUserInfo.PaymentMethod, tid = objEventOrderDetails.TransactionId });

                            }

                        }



                    }
                    else
                    {
                        /*Free Event Tickets will be processed in below code */
                        int status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);
                        if (objTemplates != null)
                        {
                            StringBuilder body = new StringBuilder();

                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[EventName]", objEventUserInfo.EventName);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));

                            body.Replace("[TransactionId]", " ");
                            body.Replace("[PaymentType]", " ");
                            body.Replace("[PaymentStatus]", " ");
                            body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"));
                            body.Replace("[AmountPaid]", " ");

                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                            if (objAppInfo.Email == "Yes")
                            {
                                _sentmail.SendMailSendinbrevowithfrom(objEventUserInfo.Email, objAppInfo.EventsEmail, objTemplates.Subject, body.ToString());
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "User Email Sent",
                                    LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }

                        }
                        if (objEventUserInfo.ItemName != null && objEventUserInfo.ItemName != "")
                        {
                            Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Cultural Event Registration for Admin", 0, ref status);
                            if (objTemplates1 != null)
                            {
                                StringBuilder body = new StringBuilder();
                                StringBuilder ps = new StringBuilder();
                                StringBuilder subject = new StringBuilder();

                                body.Append(objTemplates1.Description);
                                subject.Append(objTemplates1.Subject);
                                subject.Replace("[EventName]", objEventUserInfo.EventName);
                                body.Replace("[ItemName]", objEventUserInfo.ItemName);
                                body.Replace("[ItemCategory]", objEventUserInfo.ItemCategory);
                                body.Replace("[ItemDuration]", objEventUserInfo.ItemDuration.ToString());
                                body.Replace("[FirstName]", objEventUserInfo.FirstName);
                                body.Replace("[LastName]", objEventUserInfo.LastName);
                                body.Replace("[Email]", objEventUserInfo.Email);
                                body.Replace("[Mobile]", objEventUserInfo.Mobile);
                                body.Replace("[EventName]", objEventUserInfo.EventName);
                                body.Replace("[Description]", objEventUserInfo.ItemDescription);
                                body.Replace("[EventType]", "");
                                body.Replace("[SiteName]", objAppInfo.SiteName);

                                if (lstEventParticipants != null && lstEventParticipants.Count() != 0)
                                {
                                    foreach (Entities.EventParticipants objEventParticipants in lstEventParticipants)
                                    {
                                        ps.Append("<tr height=\"35\"><td align=\"left\" style=\"font-family:Arial, Helvetica, sans-serif; color:#000;\">" + objEventParticipants.FirstName + "</td><td align=\"left\" style=\"font-family:Arial, Helvetica, sans-serif; color:#000;\">" + objEventParticipants.LastName + "</td><td align=\"left\" style=\"font-family:Arial, Helvetica, sans-serif; color:#000;\">" + objEventParticipants.Mobile + "</td></tr>");
                                    }
                                }

                                body.Replace("[Participents]", ps.ToString());
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                body.Replace("[Year]", DateTime.Now.Year.ToString());
                                if (objAppInfo.Email == "Yes")
                                {
                                    _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.EventsEmail, subject.ToString(), body.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Admin notification email sent to: {objAppInfo.EventsEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }

                            }
                        }
                        else
                        {
                            Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Normal Event Registration for Admin", 0, ref status);
                            if (objTemplates1 != null)
                            {
                                StringBuilder body = new StringBuilder();
                                StringBuilder ps = new StringBuilder();
                                StringBuilder subject = new StringBuilder();

                                body.Append(objTemplates1.Description);
                                subject.Append(objTemplates1.Subject);
                                subject.Replace("[EventName]", objEventUserInfo.EventName);
                                body.Replace("[FirstName]", objEventUserInfo.FirstName);
                                body.Replace("[LastName]", objEventUserInfo.LastName);
                                body.Replace("[Email]", objEventUserInfo.Email);
                                body.Replace("[Mobile]", objEventUserInfo.Mobile);
                                body.Replace("[EventName]", objEventUserInfo.EventName);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());
                                if (objAppInfo.Email == "Yes")
                                {
                                    _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.EventsEmail, subject.ToString(), body.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Admin notification email sent to: {objAppInfo.EventsEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }

                            }
                        }
                        return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", cname = objEventUserInfo.EventName });
                    }


                }
                else if (!isHuman)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Captcha Failed - Redirect",
                        LogDescription = "User redirected back due to captcha failure.",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError(string.Empty, " reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("EventRegistration", "Event", new { eid = objEventUserInfo.EventId, EventName = BLL.Common.EncodeURL(objEventUserInfo.EventName), cname = cname });

                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed Transaction.</div>";
                    return RedirectToAction("EventRegistration", "Event", new { eid = objEventUserInfo.EventId, EventName = BLL.Common.EncodeURL(objEventUserInfo.EventName), cname = cname });
                }
            }
            catch (Exception ex)
            {

                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "EventRegistration",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", cname = objEventUserInfo.EventName });
                //return RedirectToAction("EventRegistration", "Event", new { eid = objEventUserInfo.EventId, EventName = BLL.Common.EncodeURL(objEventUserInfo.EventName), cname = cname });
            }


            return RedirectToAction("EventAcknowledgement", "Event", new { area = "User", cname = objEventUserInfo.EventName });
        }



        public async Task<ActionResult> ThankYou(Int64 id = 0, string PaymentMethod = "PayPal", decimal amt = 0, string cname = "", string type = "", string session_id = "", string spPaymentLink = "")
        {
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            Entities.LogReport logs = new Entities.LogReport();

            int status = 0;
            AWS_AssociationToolCore.Entities.AppInfo objAppInfo = new AWS_AssociationToolCore.Entities.AppInfo();
            AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
            objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
            AWS_AssociationToolCore.Entities.EventOrderDetails objEventOrderDetails = new AWS_AssociationToolCore.Entities.EventOrderDetails();

            int _qstatus = 0;

            List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
            List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
            Entities.Events objEvents = new Entities.Events();
            List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
            objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Event Registration",
                LogDescription = "Event Registration ThankYou Page Entering",
                LogDate = DateTime.Now,
                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objEventUserInfo.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);

            DateTime startDateTime = objEventUserInfo.StartDate;
            DateTime endDateTime = objEventUserInfo.EndDate;


            if (objEventUserInfo.StartDate != DateTime.MinValue)
            {
                startDateTime = objEventUserInfo.StartDate.Date
                                .AddHours(objEventUserInfo.StartDate.Hour)
                                .AddMinutes(objEventUserInfo.StartDate.Minute);
            }

            if (objEventUserInfo.EndDate != DateTime.MinValue)
            {
                endDateTime = objEventUserInfo.EndDate.Date
                                .AddHours(objEventUserInfo.EndDate.Hour)
                                .AddMinutes(objEventUserInfo.EndDate.Minute);
            }

            // Step 2: Generate iCalendar content
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
            sb.AppendLine("PRODID:-//Event Booking//EN");

            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("UID:" + Guid.NewGuid());
            sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
            sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
            sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
            sb.AppendLine("SUMMARY:Event - " + objEventUserInfo.EventName);
            sb.AppendLine("DESCRIPTION:" + objEventUserInfo.EventCategory);
            sb.AppendLine("LOCATION:" + objEventUserInfo.Location);
            // sb.AppendLine("LOCATION:Temple");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            string icsContent = sb.ToString();

            // Step 3: Convert to byte stream
            byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
            MemoryStream stream = new MemoryStream(byteArray);
            Attachment calendarAttachment = new Attachment(stream, "Event.ics", "text/calendar");

            if (PaymentMethod != "PayPal")
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = $"PaymentMethod: {PaymentMethod ?? "Unknown"}.",
                    LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                objEventUserInfo.AmountPaid = objEventUserInfo.objEventOrderDetails.AmountPaid;

                ViewBag.objEventUserInfo = objEventUserInfo;

                ViewBag.PayerEmail = objEventUserInfo.Email;
                ViewBag.UserName = objEventUserInfo.FirstName;
                ViewBag.AddressCity = objEventUserInfo.City;
                ViewBag.AddressState = objEventUserInfo.State;
                ViewBag.AddressZip = objEventUserInfo.ZipCode;
                ViewBag.GrossTotal = objEventUserInfo.objEventOrderDetails.AmountPaid;
                ViewBag.PaymentStatus = objEventUserInfo.objEventOrderDetails.PaymentStatus;
                ViewBag.TransactionId = objEventUserInfo.objEventOrderDetails.TransactionId;
                // DateTime Date = ConvertPayPalDateTime(objEventUserInfo.objEventOrderDetails.PaymentDate.ToString());
                ViewBag.PaymentDate = objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy");
                ViewBag.UserPhone = objEventUserInfo.Mobile;
                ViewBag.Date = DateTime.UtcNow;
                ViewBag.cname = cname;
                Int64 LRstatus = 0;


            }
            else
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "EventRegistrations",
                    LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                ViewBag.PaymentStatus = objEventUserInfo.objEventOrderDetails.PaymentStatus;
                ViewBag.TransactionId = objEventUserInfo.objEventOrderDetails.TransactionId;

            }
            if (PaymentMethod == "Stripe")
            {
                int status1 = 0;
                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status1);
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Stripe PaymentMethod",
                    LogDescription = "Event Registration ThankYou Page Stripe PaymentMethod  getting payment details.",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;

                if (string.IsNullOrEmpty(session_id))
                {
                    if (Request.Cookies.TryGetValue("StripeSessionId", out var cookieValue))
                    {
                        session_id = cookieValue;
                    }
                }

                if (string.IsNullOrEmpty(session_id))
                {
                    // Retrieve the latest session from Stripe
                    var sessionService = new SessionService();
                    var sessions = sessionService.List(new SessionListOptions { Limit = 1 });
                    var latestSession = sessions.Data.Count > 0 ? sessions.Data[0] : null;

                    if (latestSession != null)
                    {
                        session_id = latestSession.Id;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Received session_id: " + session_id);
                if (string.IsNullOrEmpty(session_id))
                {
                    throw new Exception("Session ID not found.");
                }

                var sessionServiceInstance = new SessionService();
                var session = sessionServiceInstance.Get(session_id);

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);


                Int64 result = 0;

                objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;

                objEventUserInfo.AmountPaid = objEventUserInfo.objEventOrderDetails.AmountPaid;
                objEventUserInfo.PaymentMethod = objEventOrderDetails.PaymentMethod;
                objEventUserInfo.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                if (session.PaymentStatus == "paid")
                {
                    objEventUserInfo.PaymentStatusId = 1;
                    objEventUserInfo.PaymentMethodId = 11;

                }
                else
                {
                    objEventUserInfo.PaymentStatusId = 2;
                    objEventUserInfo.PaymentMethodId = 11;
                }
                objEventUserInfo.TransactionId = paymentIntent.Id;

                objEventUserInfo.EventUserInfoId = id;
                objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                // objEventUserInfo.PaymentDate = objEventUserInfo.objEventOrderDetails.PaymentDate;

                objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Stripe PaymentMethod",
                    LogDescription = "Updated Transaction details.",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                objEventUserInfo.EventUserInfoId = id;



                string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                    "<tr>" +
                    "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                    "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                    "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                    "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>" +
                    "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Early Bird Amount</span></th>" +
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
                                   "<td align=\"center\">$" + item.EarlyBirdMemberAmount + "</td>" +
                                   "</tr>";

                        i++;
                    }
                }

                tickets += "</table>";

                if (objAppInfo.IsQRCode == "Yes")
                {

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Entering Is QRCode",
                        LogDescription = "Get By EventUserInfo Details",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    GenerateQRCodepdf(id, objEventUserInfo.EventId, ref lstEventRegTickets);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Successfully  QRCode Generate",
                        LogDescription = "Successfully  QRCode Generate",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                }
                List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

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
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "TicketinG PDF FAILED",
                            LogDescription = pdfEx.Message,
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

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
                        pdfFileName = $"EventTickets-Reg-{id}.pdf";
                        pdfFilePath = System.IO.Path.Combine(pdfFolderPath, pdfFileName);

                        // Save PDF to file
                        System.IO.File.WriteAllBytes(pdfFilePath, finalPdf);


                    }
                    catch (Exception pdfEx)
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "ticket Count FAILED",
                            LogDescription = pdfEx.Message,
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                    }
                }
                //Send Mail to User
                string BaseUrl = objAppInfo.UserSiteUrl;
                int MailStatus = 0;
                Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                objMailTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref MailStatus);
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
                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                    body.Replace("[Email]", objEventUserInfo.Email);
                    body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                    body.Replace("[TransactionId]", paymentIntent.Id);
                    body.Replace("[PaymentType]", PaymentMethod);
                    body.Replace("[PaymentStatus]", session.PaymentStatus == "paid" ? "Completed" : "Pending");
                    body.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                    decimal totalamout = 0;

                    if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                    {
                        totalamout = lstEventRegCounts.Sum(x => x.Amount);
                    }

                    // If value0, pass default value
                    body.Replace("[TotalRegistrationAmount]",
                        totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                    // Early Bird Amount Calculation
                    decimal totalEarlyBird = 0;

                    if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                    {
                        totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                    }

                    // If value is 0, pass default value
                    body.Replace("[EarlyBirdAmount]",
                        totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");
                    body = body.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body.Replace("[SiteName]", objAppInfo.SiteName);
                    body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                    body.Replace("[SiteName]", objAppInfo.SiteName);
                    body.Replace("[Tickets]", tickets);
                    body.Replace("[Year]", DateTime.Now.Year.ToString());

                    if (objAppInfo.IsQRCode == "Yes")
                    {
                        if (objEventUserInfo.IsQRCode == "Single")
                        {
                            string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                            string RefCode = cleanSiteName + id;
                            string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                            var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                            body = body.Replace("[QRCode]", image1);

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "QRCode Single",
                                LogDescription = "QRCode Single GENERATED",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                        else if (objEventUserInfo.IsQRCode == "Multiple")
                        {
                            string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(id.ToString());
                            var button = "<a href='" + url + "' target='_blank' " +
                                        "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                         "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                         "Event Tickets QR Codes</a>";

                            body = body.Replace("[QRCode]", button);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "QRCode Multiple",
                                LogDescription = "QRCode Multiple GENERATED",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }

                    if (objAppInfo.Email == "Yes")
                    {
                        var responsce = _sentmail.SendMailWithAttachmentICO(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString(), calendarAttachment);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "User Email Sent",
                            LogDescription = $"Confirmation email sent to user: {objEventUserInfo.Email}",
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "without User Email Sent to procced",
                        LogDescription = "without User Email Sent to procced",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                }
                //Send Mail to Admin
                int statusAdmin = 0;
                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
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
                    body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                    body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                    body1.Replace("[Email]", objEventUserInfo.Email);

                    body1.Replace("[TransactionId]", paymentIntent.Id);
                    body1.Replace("[PaymentType]", PaymentMethod);
                    body1.Replace("[PaymentStatus]", session.PaymentStatus == "paid" ? "Completed" : "Pending");
                    body1.Replace("[PaymentDate]", objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy"));
                    body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                    body1 = body1.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender); body1.Replace("[SiteName]", objAppInfo.SiteName);
                    body1.Replace("[SiteName]", objAppInfo.SiteName);
                    body1.Replace("[Tickets]", tickets);
                    body1.Replace("[Year]", DateTime.Now.Year.ToString());

                    if (objEvents.ContactEmail != null && objEvents.ContactEmail != "")
                    {
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailWithAttachmentICO(objEvents.ContactEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);

                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to Admin: {objEvents.ContactEmail}"
                                    : $"Confirmation email FAILED for user: {objEvents.ContactEmail} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "without User Email Sent to procced",
                            LogDescription = "without User Email Sent to procced",
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                    }
                    else
                    {
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = " admin Email Sent to procced",
                                LogDescription = " admin Email Sent to procced",
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                    }
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Admin Email Sent",
                        LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ViewBag.GrossTotal = objEventUserInfo.objEventOrderDetails.AmountPaid;
                    ViewBag.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                    ViewBag.TransactionId = paymentIntent.Id;
                    DateTime Date = ConvertPayPalDateTime(objEventUserInfo.objEventOrderDetails.PaymentDate.ToString());
                    ViewBag.PaymentDate = objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy");
                    ViewBag.UserPhone = objEventUserInfo.Mobile;
                    ViewBag.Date = DateTime.UtcNow;
                    ViewBag.cname = cname;

                }
            }
            else if (PaymentMethod == "Swire Pay")
            {
                spPaymentLink = Request.Query["sp-payment-link"].ToString();

                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Event Registration ThankYou Page",
                    LogDescription = "Swire Pay Started into ThankYou Page",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                int status2 = 0;
                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);


                spPaymentLink = Request.Query["sp-payment-link"].ToString();

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.swirepay.com/v1/payment-link/" + spPaymentLink + "/verify");
                request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);

                var response = await client.SendAsync(request);
                var responseData1 = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                var responseBody = await response.Content.ReadAsStringAsync();



                var jsonData = JObject.Parse(responseData1);

                // Check if "entity" and "paymentSession" exist in the response JSON
                if (jsonData["entity"]?["paymentSession"] != null)
                {
                    var paymentSession = jsonData["entity"]["paymentSession"];

                    var receiptNumber = paymentSession["receiptNumber"]?.ToString();
                    var paymentType = paymentSession["paymentType"]?.ToString();
                    var status1 = paymentSession["status"]?.ToString();
                    //var status = jsonData["entity"]?["status"]?.ToString();

                    // tid = receiptNumber;

                    // Output the values
                    Console.WriteLine($"Receipt Number: {receiptNumber ?? "Not Available"}");
                    Console.WriteLine($"Payment Type: {paymentType ?? "Not Available"}");



                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Event Registration ThankYou Page",
                        LogDescription = "Response Status is " + status + " into Acknowledgement Page",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    Int64 result = 0;

                    objEventOrderDetails.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                    objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;

                    objEventUserInfo.AmountPaid = objEventUserInfo.objEventOrderDetails.AmountPaid;
                    objEventUserInfo.PaymentMethod = objEventOrderDetails.PaymentMethod;
                    objEventUserInfo.PaymentStatus = status1 == "SUCCEEDED" ? "Completed" : "Pending";
                    if (status1 == "SUCCEEDED")
                    {
                        objEventUserInfo.PaymentStatusId = 1;
                        objEventUserInfo.PaymentMethodId = 12;

                    }
                    else
                    {
                        objEventUserInfo.PaymentStatusId = 2;
                        objEventUserInfo.PaymentMethodId = 12;
                    }
                    objEventUserInfo.TransactionId = receiptNumber;

                    objEventUserInfo.EventUserInfoId = id;
                    objEventUserInfo.UpdatedBy = objEventUserInfo.FirstName;
                    // objEventUserInfo.PaymentDate = objEventUserInfo.objEventOrderDetails.PaymentDate;

                    objEventUserInfo.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objEventUserInfo.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);

                    objEventUserInfo.EventUserInfoId = id;

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Event Registration ThankYou Page",
                        LogDescription = "Updated Transaction details",
                        LogDate = DateTime.Now,
                        InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    string tickets = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                                      "<tr>" +
                                      "<th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th>" +
                                      "<th width=\"38%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Category</span></th>" +
                                      "<th width=\"20%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Count</span></th>" +
                                      "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Registration Amount</span></th>" +
                                      "<th width=\"16%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Early Bird Amount</span></th>" +
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
                                       "<td align=\"center\">$" + item.EarlyBirdMemberAmount + "</td>" +
                                       "</tr>";

                            i++;
                        }
                    }

                    tickets += "</table>";
                    if (objAppInfo.IsQRCode == "Yes")
                    {

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Entering Is QRCode",
                            LogDescription = "Get By EventUserInfo Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        GenerateQRCodepdf(id, objEventUserInfo.EventId, ref lstEventRegTickets);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Successfully  QRCode Generate",
                            LogDescription = "Successfully  QRCode Generate",
                            LogDate = DateTime.Now,
                            InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }
                    List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketViewModels = new List<AWS_AssociationToolCore.Models.TicketPdfViewModel>();

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
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "TicketinG PDF FAILED",
                                LogDescription = pdfEx.Message,
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

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
                            pdfFileName = $"EventTickets-Reg-{id}.pdf";
                            pdfFilePath = System.IO.Path.Combine(pdfFolderPath, pdfFileName);

                            // Save PDF to file
                            System.IO.File.WriteAllBytes(pdfFilePath, finalPdf);


                        }
                        catch (Exception pdfEx)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "ticket Count FAILED",
                                LogDescription = pdfEx.Message,
                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });


                        }
                    }
                    //Send Mail to User
                    string BaseUrl = objAppInfo.UserSiteUrl;
                    int MailStatus = 0;
                    Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                    objMailTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref MailStatus);
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
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                        body.Replace("[Email]", objEventUserInfo.Email);
                        body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                        body.Replace("[TransactionId]", receiptNumber);
                        body.Replace("[PaymentType]", PaymentMethod);
                        body.Replace("[PaymentStatus]", status1 == "SUCCEEDED" ? "Completed" : "Pending");
                        body.Replace("[PaymentDate]", objEventUserInfo.PaymentDate.ToString("MM/dd/yyyy"));
                        body.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[Tickets]", tickets);
                        decimal totalamout = 0;

                        if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                        {
                            totalamout = lstEventRegCounts.Sum(x => x.Amount);
                        }

                        // If value0, pass default value
                        body.Replace("[TotalRegistrationAmount]",
                            totalamout > 0 ? totalamout.ToString("F2") : "0.00");


                        // Early Bird Amount Calculation
                        decimal totalEarlyBird = 0;

                        if (lstEventRegCounts != null && lstEventRegCounts.Count > 0)
                        {
                            totalEarlyBird = lstEventRegCounts.Sum(x => x.EarlyBirdMemberAmount);
                        }

                        // If value is 0, pass default value
                        body.Replace("[EarlyBirdAmount]",
                            totalEarlyBird > 0 ? totalEarlyBird.ToString("F2") : "0.00");
                        body = body.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        if (objAppInfo.IsQRCode == "Yes")
                        {
                            if (objEventUserInfo.IsQRCode == "Single")
                            {
                                string cleanSiteName = objAppInfo.SiteName.Replace(" ", "");
                                string RefCode = cleanSiteName + id;
                                string url = objAppInfo.UserSiteUrl + "Content/QRCodeGeneration/QR-of-" + RefCode + ".jpg";
                                var image1 = "<img width=\"200\" height=\"200\" src=\"" + url + "\" />";
                                body = body.Replace("[QRCode]", image1);

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "QRCode Single",
                                    LogDescription = "QRCode Single GENERATED",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            else if (objEventUserInfo.IsQRCode == "Multiple")
                            {
                                string url = objAppInfo.UserSiteUrl + "eventqr?id1=" + AWS_AssociationToolCore.BLL.Common.EncryptString(id.ToString());
                                var button = "<a href='" + url + "' target='_blank' " +
                                            "style='display:inline-block;padding:10px 20px;background:#007bff;color:#fff;" +
                                             "text-decoration:none;border-radius:5px;font-family:Arial,sans-serif;font-size:14px;'>" +
                                             "Event Tickets QR Codes</a>";

                                body = body.Replace("[QRCode]", button);
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "QRCode Multiple",
                                    LogDescription = "QRCode Multiple GENERATED",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                        }
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailWithAttachmentICO(objEventUserInfo.Email, objAppInfo.EventsEmail, objMailTemplates.Subject, body.ToString(), calendarAttachment);

                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "User Email Sent" : "User Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to user: {objEventUserInfo.Email}"
                                    : $"Confirmation email FAILED for user: {objEventUserInfo.Email} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                    }
                    //Send Mail to Admin
                    int statusAdmin = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Event Registration", 0, ref statusAdmin);
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
                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
                        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));
                        body1.Replace("[Email]", objEventUserInfo.Email);

                        body1.Replace("[TransactionId]", receiptNumber);
                        body1.Replace("[PaymentType]", PaymentMethod);
                        body1.Replace("[PaymentStatus]", status1 == "SUCCEEDED" ? "Completed" : "Pending");
                        body1.Replace("[PaymentDate]", objEventUserInfo.PaymentDate.ToString("MM/dd/yyyy"));
                        body1.Replace("[AmountPaid]", objEventUserInfo.objEventOrderDetails.AmountPaid.ToString());
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
                        body1 = body1.Replace("[PromoDiscount]", string.IsNullOrWhiteSpace(objEventUserInfo.Gender) ? "N/A" : objEventUserInfo.Gender);
                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                        body1.Replace("[Tickets]", tickets);
                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                        if (objEvents.ContactEmail != null && objEvents.ContactEmail != "")
                        {
                            if (objAppInfo.Email == "Yes")
                            {
                                var responsce = _sentmail.SendMailWithAttachmentICO(objEvents.ContactEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "User Email Sent",
                                    LogDescription = $"Confirmation email sent to user: {objEvents.ContactEmail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                            }

                        }
                        else
                        {
                            if (objAppInfo.Email == "Yes")
                            {
                                var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.EventsEmail, objAppInfo.EventsEmail, objTemplatesAdmin.Subject, body1.ToString(), calendarAttachment);
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Admin Email Sent",
                                    LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }

                        }


                        ViewBag.GrossTotal = objEventUserInfo.objEventOrderDetails.AmountPaid;
                        ViewBag.PaymentStatus = status1 == "SUCCEEDED" ? "Completed" : "Pending";
                        ViewBag.TransactionId = receiptNumber;
                        //DateTime Date = ConvertPayPalDateTime(objEventUserInfo.objEventOrderDetails.PaymentDate.ToString());
                        ViewBag.PaymentDate = objEventUserInfo.objEventOrderDetails.PaymentDate.ToString("MM/dd/yyyy");
                        ViewBag.UserPhone = objEventUserInfo.Mobile;
                        ViewBag.Date = DateTime.UtcNow;
                        ViewBag.cname = cname;

                    }
                }

            }
            else
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "EventRegistrationst",
                    LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                    LogDate = DateTime.Now,
                    InsertedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEventUserInfo.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

            }

            ViewBag.objEventUserInfo = objEventUserInfo;
            ViewBag.Date = DateTime.UtcNow;
            ViewBag.cname = cname;
            return View();
        }
        public ActionResult EventQR(string id1 = "", Int64 id = 0, int uid = 0, String MembershipType = "")
        {
            try
            {
                id = Convert.ToInt64(BLL.Common.UnEncryptString(id1.ToString()));

                int _qstatus = 0;
                List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                Entities.Events objEvents = new Entities.Events();


                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventRegCounts = lstEventRegCounts;
                    ViewBag.lstEventsTickets = lstEventsTickets;
                    ViewBag.lstEventRegTickets = lstEventRegTickets;
                    ViewBag.uid = uid;
                    ViewBag.MembershipType = MembershipType;
                    ViewBag.status = _qstatus;
                    if (User.Identity.IsAuthenticated && User.Identity.Name != null && HttpContext.Session.GetString("username") != null)
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
        public ActionResult QRIndividualTicketPreview(Int64 tktid = 0, Int64 euid = 0, Int64 eid = 0, int uid = 0, String MembershipType = "")
        {
            try
            {

                int _qstatus = 0;
                Entities.EventRegTickets objEventRegTickets = new Entities.EventRegTickets();

                objEventRegTickets = _Events.EventUserInfoIndividualGetById(uid, euid, tktid, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objEventRegTickets = objEventRegTickets;

                    ViewBag.uid = uid;
                    ViewBag.MembershipType = MembershipType;
                    ViewBag.status = _qstatus;
                    if (User.Identity.IsAuthenticated && User.Identity.Name != null && HttpContext.Session.GetString("username") != null)
                    {
                        ViewBag.type = "yes";
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = tktid, uid = uid });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = tktid, uid = uid });
            }
            return View();
        }


        [HttpPost]
        public ActionResult InsertEventCheckIn(Entities.EventRegCheckIn objcheckin, int uid = 0)
        {
            try
            {
                objcheckin.CheckInDate = DateTime.UtcNow;
                Int64 status = _Events.EventCheckInInsert(objcheckin);

                if (status == 1 || status == 2)
                {
                    TempData["message"] = "<div class=\"success closable\"> Updated comment successfully.</div>";
                    return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });

                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error-alert closable\">Failed updating registration fields.</div>";
                return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });
            }
            return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });

        }
        public ActionResult PaymentCancelled(Int64 id = 0)
        {
            PayByPayPal.PDTHolder objPDT = new PayByPayPal.PDTHolder();
            int result1 = 0;
            Entities.EventOrderDetails objEventOrderDetails = new Entities.EventOrderDetails();
            objEventUserInfo = _Events.GetEventUserInfoById(id, ref result1);
            string status = "";
            try
            {
                int qstatus = 0;
                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref qstatus);
                objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;
                status = objPayByPayPal.Ipn();

                objPDT = objPayByPayPal.objPDTHolder;
                objEventOrderDetails.PaymentMethod = "PayPal";
                objEventOrderDetails.TransactionId = "N/A";
                objEventOrderDetails.PaymentStatus = "Cancelled";
                objEventOrderDetails.EventId = objEventUserInfo.EventId;
                objEventOrderDetails.UpdatedBy = objEventUserInfo.FirstName;
                objEventOrderDetails.InsertedBy = objEventUserInfo.FirstName;
                objEventOrderDetails.AmountPaid = Convert.ToDecimal(objPDT.GrossTotal);
                Int64 result = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Your payment did not go through please contact board@TeluguAssociationToolweb.org for assistance.";
                throw ex;
            }

            ViewBag.status = status;

            return View();
        }

        // Paypal Acknowledgement
        //public ActionResult PaymentEventAcknowledgement(Int64 id = 0)
        //{
        //    try
        //    {
        //        int status = 0;
        //        Int64 result = 0;
        //        int result1 = 0;
        //        string Username = "";
        //        if (id != 0)
        //        {
        //            Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //            objEventUserInfo = _Events.GetEventUserInfoById(id, ref result1);
        //            PayByPayPal.PDTHolder objPDT = new PayByPayPal.PDTHolder();
        //            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref status);
        //            objPayByPayPal.objPaymentSettings.PDTToken = objPaymentSettings.TokenNo;
        //            objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;

        //            status = objPayByPayPal.Success();
        //            if (status == 0)
        //            {
        //                objPDT = objPayByPayPal.objPDTHolder;
        //                if (objEventUserInfo.FirstName != null && objEventUserInfo.FirstName != "")
        //                {
        //                    Username = objEventUserInfo.FirstName + objEventUserInfo.LastName;
        //                }

        //                if (Username != null)
        //                {
        //                    if (result1 == 1)
        //                    {
        //                        Entities.EventOrderDetails objEventOrderDetails = new Entities.EventOrderDetails();

        //                        objEventOrderDetails.AmountPaid = Convert.ToDecimal(objPDT.GrossTotal);
        //                        objEventOrderDetails.PaymentMethod = "PayPal";
        //                        objEventOrderDetails.TransactionId = objPDT.TransactionId;
        //                        objEventOrderDetails.PaymentStatus = objPDT.PaymentStatus;
        //                        objEventOrderDetails.EventId = objEventUserInfo.EventId;
        //                        objEventOrderDetails.EventUserInfoId = objEventUserInfo.EventUserInfoId;
        //                        objEventOrderDetails.PaymentDate = ConvertPayPalDateTime(objPDT.PaymentDate.ToString());
        //                        objEventOrderDetails.UpdatedBy = Username;
        //                        objEventOrderDetails.InsertedBy = Username;

        //                        result = _Events.InsertEventOrderDetail(objEventOrderDetails);
        //                        if (result == 1)
        //                        {
        //                            ViewBag.PayerEmail = objPDT.PayerEmail;
        //                            ViewBag.UserName = Username;
        //                            ViewBag.ReceiverEmail = objPDT.ReceiverEmail;
        //                            ViewBag.AddressCity = objPDT.AddressCity;
        //                            ViewBag.AddressCountry = objPDT.AddressCountry;
        //                            ViewBag.AddressName = objPDT.AddressName;
        //                            ViewBag.AddressState = objPDT.AddressState;
        //                            ViewBag.AddressStreet = objPDT.AddressStreet;
        //                            ViewBag.AddressZip = objPDT.AddressZip;
        //                            ViewBag.GrossTotal = objPDT.GrossTotal;
        //                            ViewBag.PaymentStatus = objPDT.PaymentStatus;
        //                            ViewBag.TransactionId = objPDT.TransactionId;
        //                            DateTime Date = ConvertPayPalDateTime(objPDT.PaymentDate.ToString());
        //                            ViewBag.PaymentDate = Date.ToString("MM/dd/yyyy");
        //                        }
        //                    }
        //                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Confirm Event Registration", 0, ref status);
        //                    if (objTemplates != null)
        //                    {
        //                        StringBuilder body = new StringBuilder();
        //                        body.Append(objTemplates.Description);
        //                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);

        //                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                        body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                        body.Replace("[TEmailId]", objAppInfo.CompanyEmail);
        //                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
        //                        body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));

        //                        _sentmail.SendMailSendinbrevo(objEventUserInfo.Email, objTemplates.Subject, body.ToString());

        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Event Registrations", 0, ref status);
        //                if (objTemplates != null)
        //                {
        //                    StringBuilder body = new StringBuilder();
        //                    body.Append(objTemplates.Description);
        //                    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);

        //                    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                    body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                    body.Replace("[TEmailId]", objAppInfo.CompanyEmail);
        //                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEventUserInfo.FirstName + " " + objEventUserInfo.LastName));
        //                    body.Replace("[EventName]", BLL.Common.UppercaseFirst(objEventUserInfo.EventName));

        //                    _sentmail.SendMailSendinbrevo(objAppInfo.EventsEmail, objTemplates.Subject, body.ToString());
        //                }
        //                return RedirectToAction("Thankyou", "Event");
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Error404", "Error");
        //    }
        //    return View();
        //}

        public ActionResult EventAcknowledgement(Int64 id = 0, string cname = "")
        {
            int _qstatus = 0;

            List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
            List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
            Entities.Events objEvents = new Entities.Events();
            List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
            objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

            ViewBag.cname = cname;
            ViewBag.objEventUserInfo = objEventUserInfo;
            return View();
        }
        public DateTime ConvertPayPalDateTime(string payPalDateTime)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            // accept a few different date formats because of PST/PDT timezone and slight month difference in sandbox vs. prod.
            string[] dateFormats = { "HH:mm:ss MMM dd, yyyy PST", "HH:mm:ss MMM. dd, yyyy PST", "HH:mm:ss MMM dd, yyyy PDT", "HH:mm:ss MMM. dd, yyyy PDT",
                             "HH:mm:ss dd MMM yyyy PST", "HH:mm:ss dd MMM. yyyy PST", "HH:mm:ss dd MMM yyyy PDT", "HH:mm:ss dd MMM. yyyy PDT"};
            DateTime outputDateTime;

            DateTime.TryParseExact(payPalDateTime, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTime);

            // convert to local timezone
            TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

            outputDateTime = TimeZoneInfo.ConvertTime(outputDateTime, hwZone, TimeZoneInfo.Local);

            return outputDateTime;
        }

        public ActionResult EventDetails(string EventName = "", string Type = "", Int64 eid = 0, string cname = "")
        {
            try
            {
                int status = 0;
                int qstatus = 0;
                Int64 ChapterId = 0;
                // Cookie reading (Core way)
                if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
                {
                    cname = cookieValue;
                }
                if (EventName != "")
                {
                    objEvents = _Events.GetEventById(eid, EventName, ref lstEventRegistrationTypes, ref lstEventDates, ref lstEventRegistrationCategories, ref status);
                }
                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }

                ViewBag.ChapterId = ChapterId;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.objEvents = objEvents;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            ViewBag.lstEventRegistrationCategories = lstEventRegistrationCategories;
            ViewBag.Type = Type;
            ViewBag.cname = cname;
            return View();
        }
        public byte[] GenerateTicketPdf_iText7(string html, string fontDir)
        {
            using (var ms = new MemoryStream())
            {
                var props = new ConverterProperties();

                // ✅ Use default fonts (includes Arial, Helvetica, etc.)
                var fontProvider = new DefaultFontProvider(true, true, true);

                // Only add directory if it exists and has fonts
                if (!string.IsNullOrEmpty(fontDir) && Directory.Exists(fontDir))
                {
                    fontProvider.AddDirectory(fontDir);
                }

                props.SetFontProvider(fontProvider);

                HtmlConverter.ConvertToPdf(
                    new MemoryStream(Encoding.UTF8.GetBytes(html)),
                    ms,
                    props
                );

                return ms.ToArray();
            }
        }
        public void GenerateQRCodeNew(long EventUserInfoId, long EventId, ref List<Entities.EventRegTickets> lstEventRegTickets)
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            //int _qstatus = 0;
            //List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
            //List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
            //List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
            //Entities.Events objEvents = new Entities.Events();

            //objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

            // === Overall Registration QR ===
            string RefCode = objAppInfo.SiteName + EventUserInfoId;
            string overallUrl = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;

            SaveQRCodeImage(overallUrl, objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + RefCode + ".jpg");

            // === Individual Tickets QR ===
            if (lstEventRegTickets != null && lstEventRegTickets.Any())
            {
                foreach (var ticket in lstEventRegTickets)
                {
                    // Payload URL for individual ticket
                    string ticketUrl = objAppInfo.UserSiteUrl + "ticket-preview?tktid=" + ticket.EventRegTicketId + "&euid=" + EventUserInfoId + "&uid=" + objEventUserInfo.EventId;

                    // Unique file name
                    string ticketRefCode = objAppInfo.SiteName + "-Ticket-" + ticket.EventRegTicketId;

                    string savePath = objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + ticketRefCode + ".jpg";

                    SaveQRCodeImage(ticketUrl, savePath);
                }
            }
        }
        public void GenerateQRCodepdf(long EventUserInfoId, long EventId, ref List<Entities.EventRegTickets> lstEventRegTickets)
        {
            int status1 = 0;
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
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
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


        //public void GenerateQRCode(Int64 EventUserInfoId)
        //{
        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

        //    string RefCode = objAppInfo.PresidentPhone + EventUserInfoId;
        //    string code = objAppInfo.UserSiteUrl + "qrcode-preview?id=" + EventUserInfoId;

        //    // ✅ Save path for QR image
        //    string strB = objAppInfo.UploadPath + "QRCodeGeneration\\QR-of-" + RefCode + ".jpg";

        //    // ✅ Generate QR Code bitmap
        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        //    QRCode qrCode = new QRCode(qrCodeData);

        //    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
        //    {
        //        // ✅ Load base ticket image
        //        using (Bitmap bitMapImage = new Bitmap(objAppInfo.UploadPath + "QRCodeGeneration/Qrcodesample.png"))
        //        {
        //            using (Graphics graphicImage = Graphics.FromImage(bitMapImage))
        //            {
        //                graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

        //                // ✅ Resize QR to 140x140 and draw onto base image at position (x, y)
        //                int qrWidth = 140;
        //                int qrHeight = 140;
        //                int posX = 50;  // ← adjust X position on ticket
        //                int posY = 50;  // ← adjust Y position on ticket

        //                graphicImage.DrawImage(qrBitmap, new Rectangle(posX, posY, qrWidth, qrHeight));

        //                // ✅ Optional: draw text on ticket
        //                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        //                StringFormat sf = new StringFormat
        //                {
        //                    LineAlignment = StringAlignment.Center,
        //                    Alignment = StringAlignment.Center
        //                };

        //                // Example: draw EventUserInfoId as text
        //                Font font = new Font("Arial", 12, FontStyle.Bold);
        //                graphicImage.DrawString(
        //                    EventUserInfoId.ToString(),
        //                    font,
        //                    Brushes.Black,
        //                    new RectangleF(posX, posY + qrHeight + 5, qrWidth, 20),
        //                    sf
        //                );
        //                font.Dispose();
        //            }

        //            // ✅ Save final combined image to disk
        //            bitMapImage.Save(strB, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        }
        //    }
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
                    if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && HttpContext.Session.GetString("username") != null)
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

        public byte[] MergePdfs(List<byte[]> pdfFiles)
        {
            using (var ms = new MemoryStream())
            {
                PdfDocument finalPdf = new PdfDocument(new PdfWriter(ms));
                PdfMerger merger = new PdfMerger(finalPdf);

                foreach (var pdf in pdfFiles)
                {
                    using (var readerStream = new MemoryStream(pdf))
                    {
                        PdfDocument tempPdf = new PdfDocument(new PdfReader(readerStream));
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
        public void logreport(string error = "", string pageName = "", string filepath = "")
        {

            if (System.IO.File.Exists(filepath))
            {
                using (StreamWriter stwriter = new StreamWriter(filepath, true))
                {
                    stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                    stwriter.WriteLine("Page :" + pageName);
                    stwriter.WriteLine(error);
                    stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                }
            }
            else
            {
                StreamWriter stwriter = System.IO.File.CreateText(filepath);
                stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                stwriter.WriteLine("Page :" + pageName);
                stwriter.WriteLine(error);
                stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                stwriter.Close();
            }
        }

        #region EventRegistrationForm1
        //public ActionResult EventRegistrationForm1(string EventName = "", Int64 MemberId = 0, Int64 eid = 0, Int64 cid = 0, string cname = "")
        //{
        //    int status = 0;
        //    try
        //    {
        //        if (EventName != "")
        //        {
        //            objEvents = _Events.FEGetEventByCategoryId(eid, cid, EventName, ref lstEventRegistrationTypes, ref status);
        //            if (objEvents != null && objEvents.EventId != 0)
        //            {
        //                if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && HttpContext.Session != null && Session["username"] != null)
        //                {
        //                    _objMember = _Members.GetMemberFullDetailsByEmail(HttpContext.User.Identity.Name.ToString(), ref status);
        //                }
        //                MemberId = Convert.ToInt64((Session["MemberID"] != null ? Session["MemberID"] : 0));
        //            }
        //            else
        //            {
        //                return RedirectToAction("Error404", "Error");
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Error404", "Error");
        //    }

        //    ViewBag.InnerPage = objInnerPages;
        //    ViewBag.Memberdetails = _objMember;
        //    ViewBag.MemberId = MemberId;
        //    ViewBag.objEvents = objEvents;
        //    ViewBag.EventName = EventName;
        //    ViewBag.cid = cid;
        //    ViewBag.cname = cname;
        //    return View();
        //}

        //Cultural admin comments submission
        //[HttpPost]
        //public ActionResult InsertComments(Entities.EventComments objComments, int uid = 0)
        //{
        //    try
        //    {
        //        Int64 status = _Events.InsertEventComments(objComments);

        //        if (status == 1 || status == 2)
        //        {
        //            TempData["message"] = "<div class=\"success closable\"> Updated comment successfully.</div>";
        //            return RedirectToAction("QRCodeView", "Event", new { id = objComments.EventUserInfoId, uid = uid });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error-alert closable\">Failed updating registration fields.</div>";
        //        return RedirectToAction("QRCodeView", "Event", new { id = objComments.EventUserInfoId });
        //    }
        //    return RedirectToAction("QRCodeView", "Event", new { id = objComments.EventUserInfoId });
        //}

        [HttpPost]
        public ActionResult InsertComments(Entities.EventCheckInModel model, int uid = 0)
        {
            try
            {
                // Validate
                if (model == null || string.IsNullOrEmpty(model.Comment))
                {
                    TempData["message"] = "<div class=\"error-alert closable\">Comment is required.</div>";
                    return RedirectToAction("QRCodeView", "Event", new { id = model?.EventUserInfoId });
                }

                // Create EventComments object from model
                Entities.EventComments objComments = new Entities.EventComments();
                objComments.EventUserInfoId = Convert.ToInt64(model.EventUserInfoId);
                objComments.EmployeeCode = model.EmployeeCode;
                objComments.Status = model.Status ?? "Yes";
                objComments.Comment = model.Comment;

                // Save registration check-in counts
                if (model.RegistrationItems != null && model.RegistrationItems.Count > 0)
                {
                    foreach (var item in model.RegistrationItems)
                    {
                        if (item.CheckinCount > 0)
                        {
                            // Update check-in count in database
                            //_Events.UpdateRegistrationCheckInCount(
                            //    model.EventUserInfoId,
                            //    item.RegistrationTitle,
                            //    item.CheckinCount,
                            //    item.TotalCount
                            //);
                        }
                    }
                }

                // Insert the main comment
                Int64 status = _Events.InsertEventComments(objComments);

                if (status == 1 || status == 2)
                {
                    TempData["message"] = "<div class=\"success closable\">Updated comment successfully.</div>";
                    return RedirectToAction("QRCodeView", "Event", new { id = objComments.EventUserInfoId, uid = uid });
                }
                else
                {
                    TempData["message"] = "<div class=\"error-alert closable\">Failed updating registration fields.</div>";
                    return RedirectToAction("QRCodeView", "Event", new { id = objComments.EventUserInfoId });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error-alert closable\">Error: " + ex.Message + "</div>";
                return RedirectToAction("QRCodeView", "Event", new { id = model?.EventUserInfoId });
            }
        }
        [HttpPost]
        public IActionResult InsertCommentsJson([FromBody] Entities.EventCheckInModel model, int uid = 0)
        {
            try
            {
                // Check model
                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Model is null."
                    });
                }

                // Validate comment
                if (string.IsNullOrWhiteSpace(model.Comment))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Comment is required."
                    });
                }

                // Create object
                Entities.EventComments objComments = new Entities.EventComments();

                objComments.EventUserInfoId = Convert.ToInt64(model.EventUserInfoId);
                objComments.EmployeeCode = model.EmployeeCode;
                objComments.Status = model.Status ?? "Yes";
                objComments.Comment = model.Comment;

                // Save registration items
                if (model.RegistrationItems != null &&
                    model.RegistrationItems.Count > 0)
                {
                    foreach (var item in model.RegistrationItems)
                    {
                        if (item.CheckinCount > 0)
                        {
                            Entities.EventComments objRegTicketsstatus =
                                new Entities.EventComments();

                            objRegTicketsstatus.EventUserInfoId =
                                Convert.ToInt64(model.EventUserInfoId);

                            objRegTicketsstatus.EventRegistrationTypeId =
                                item.EventRegistrationTypeId;

                            objRegTicketsstatus.CheckinCount =
                                item.CheckinCount;

                            _Events.UpdateEventRegTicketsstatus(objRegTicketsstatus);
                        }
                    }
                }

                // Insert comment
                Int64 status = _Events.InsertEventComments(objComments);

                if (status == 1 || status == 2)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Check-in updated successfully.",
                        redirectUrl = Url.Action(
                            "QRCodeView",
                            "Event",
                            new
                            {
                                id = objComments.EventUserInfoId,
                                uid = uid
                            })
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update check-in."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        //public ActionResult InsertCommentsJson(Entities.EventCheckInModel model, int uid = 0)
        //{
        //    try
        //    {
        //        // Validate
        //        if (model == null || string.IsNullOrEmpty(model.Comment))
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Comment is required."
        //            });
        //        }

        //        // Create EventComments object
        //        Entities.EventComments objComments = new Entities.EventComments();
        //        objComments.EventUserInfoId = Convert.ToInt64(model.EventUserInfoId);
        //        // objComments.EventId = Convert.ToInt64(model.EventId);
        //        objComments.EmployeeCode = model.EmployeeCode;
        //        objComments.Status = model.Status ?? "Yes";
        //        objComments.Comment = model.Comment;

        //        // Process and save registration check-in counts
        //        if (model.RegistrationItems != null && model.RegistrationItems.Count > 0)
        //        {
        //            foreach (var item in model.RegistrationItems)
        //            {
        //                if (item.CheckinCount > 0)
        //                {
        //                    Entities.EventComments objRegTicketsstatus = new Entities.EventComments();
        //                    objRegTicketsstatus.EventUserInfoId = Convert.ToInt64(model.EventUserInfoId);
        //                    objRegTicketsstatus.EventRegistrationTypeId = item.EventRegistrationTypeId;
        //                    objRegTicketsstatus.CheckinCount = item.CheckinCount;
        //                    Int64 status1 = _Events.UpdateEventRegTicketsstatus(objRegTicketsstatus);
        //                }
        //            }
        //        }

        //        // Insert the comment
        //        Int64 status = _Events.InsertEventComments(objComments);

        //        if (status == 1 || status == 2)
        //        {
        //            return Json(new
        //            {
        //                success = true,
        //                message = "Check-in updated successfully.",
        //                redirectUrl = Url.Action("QRCodeView", "Event", new { id = objComments.EventUserInfoId, uid = uid })
        //            });
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Failed to update check-in."
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Error: " + ex.Message
        //        });
        //    }
        //}
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult checkPromoCode(string PromoCode, decimal Amount = 0)
        {
            BLL.PromoCodes _PromoCodes = new BLL.PromoCodes();
            try
            {
                Int32 _qstatus = 0;
                Entities.PromoCodes _objPromoCodes = _PromoCodes.GetPromocode(PromoCode, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        status = "valid",
                        data = new
                        {
                            DiscountType = _objPromoCodes.DiscountType,  // ✅ explicit PascalCase
                            Discount = _objPromoCodes.Discount
                        }
                    });
                }
                else if (_qstatus == 2)
                {
                    return Json(new
                    {
                        ok = false,
                        status = "expired",
                        message = "Promo code expired."
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        status = "invalid",
                        message = "Invalid promo code."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    status = "error",
                    message = ex.Message
                });
            }
        }

        private ActionResult Json(object value, object allowGet)
        {
            throw new NotImplementedException();
        }
        //public ActionResult EventRegistrationForm1(Entities.Form1 objForm1)
        //{
        //    try
        //    {
        //        objForm1.IsApproved = true;
        //        objForm1.UpdatedBy = objForm1.FirstName;
        //        objForm1.InstertedBy = objForm1.FirstName;
        //        string Username = objForm1.FirstName + "" + objForm1.LastName;
        //        int status2 = 0;
        //        Int64 result = 0;
        //        int status = 0;
        //        Int64 CompetitionRegistrationId = 0;

        //        if (objForm1.AmountPaid != 0)
        //        {
        //            objForm1.PaymentMethodId = 1;
        //            objForm1.PaymentStatusId = 1;
        //        }

        //        objForm1.InsertedDate = DateTime.UtcNow;
        //        objForm1.UpdatedDate = DateTime.UtcNow;
        //        string cname = objForm1.ChapterName;

        //        Int64 _status = _Form1.FEInsertForm1(objForm1, ref CompetitionRegistrationId);
        //        Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

        //        if (_status == 1 || _status == 2)
        //        {
        //            if (objForm1.AmountPaid != 0)
        //            {
        //                if (objForm1.PaymentMethod == "PayPal")
        //                {
        //                    string PayPalRequest = "TRXTYPE=S" //S - sale transaction
        //                    + "&TENDER=C" //C - AuthorizeNet
        //                      + "&FIRSTNAME=" + objForm1.FirstName //card number
        //                      + "&STREET=" + objForm1.Notes //card number
        //                      + "&CITY=" + objForm1.Notes //card number
        //                      + "&STATE=" + objForm1.Notes //card number
        //                      + "&ZIP=" + objForm1.Notes //card number
        //                      + "&LASTNAME=" + objForm1.LastName //card number
        //                    + "&ACCT=" + objForm1.CardNumber //card number
        //                    + "&EXPDATE=" + objForm1.CSVMonth + objForm1.CSVYear.Substring(2, 2)
        //                    + "&CVV2=" + objForm1.Cvv   //card validation value (card security code)               
        //                    + "&AMT=" + objForm1.AmountPaid
        //                    + "&COMMENT1=Donation"
        //                    + "&USER=" + ConfigurationManager.AppSettings["USER"]
        //                    + "&VENDOR=" + ConfigurationManager.AppSettings["VENDOR"]
        //                    + "&PARTNER=" + ConfigurationManager.AppSettings["PARTNER"]
        //                    + "&PWD=" + ConfigurationManager.AppSettings["PWD"];

        //                    // Create an instantce of PayflowNETAPI.
        //                    PayflowNETAPI PayflowNETAPI = new PayflowNETAPI();

        //                    // RequestId is a unique string that is required for each & every transaction. 
        //                    // The merchant can use her/his own algorithm to generate this unique request id or 
        //                    // use the SDK provided API to generate this as shown below (PayflowUtility.RequestId).
        //                    string PayPalResponse = PayflowNETAPI.SubmitTransaction(PayPalRequest, PayflowUtility.RequestId);

        //                    //place data from PayPal into a namevaluecollection
        //                    NameValueCollection RequestCollection = BLL.Common.GetPayPalCollection(PayflowNETAPI.TransactionRequest);
        //                    NameValueCollection reqcollection = RequestCollection;
        //                    string ACCT = reqcollection["ACCT"];
        //                    string EXPDATE = reqcollection["EXPDATE"];

        //                    NameValueCollection ResponseCollection = BLL.Common.GetPayPalCollection(PayPalResponse);
        //                    NameValueCollection collection = ResponseCollection;
        //                    string RESPMSG = collection["RESPMSG"];
        //                    string PNREF = collection["PNREF"];
        //                    if (RESPMSG != "Approved")
        //                    {
        //                        objForm1.AmountPaid = Convert.ToDecimal(objForm1.AmountPaid);
        //                        objForm1.PaymentMethod = "PayPal";
        //                        objForm1.PaymentStatus = "Pending";
        //                        objForm1.EventId = objForm1.EventId;
        //                        objForm1.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        //                        objForm1.UpdatedBy = Username;
        //                        objForm1.InstertedBy = Username;
        //                        objForm1.PaymentStatusId = 1;
        //                        string UDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
        //                        objForm1.InsertedDate = DateTime.UtcNow;
        //                        objForm1.UpdatedDate = DateTime.UtcNow;
        //                    }
        //                    if (RESPMSG == "Approved")
        //                    {
        //                        objForm1.AmountPaid = Convert.ToDecimal(objForm1.AmountPaid);
        //                        objForm1.PaymentMethod = "PayPal";
        //                        objForm1.TransactionId = PNREF;
        //                        objForm1.PaymentStatus = "Completed";
        //                        objForm1.EventId = objForm1.EventId;
        //                        objForm1.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        //                        objForm1.UpdatedBy = Username;
        //                        objForm1.InstertedBy = Username;
        //                        objForm1.PaymentStatusId = 1;
        //                        string UDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
        //                        objForm1.InsertedDate = DateTime.UtcNow;
        //                        objForm1.UpdatedDate = DateTime.UtcNow;
        //                        objForm1.CompetitionRegistrationId = CompetitionRegistrationId;

        //                        result = _Form1.UpdateForm1(objForm1);

        //                        if (result == 1)
        //                        {
        //                            ViewBag.PayerEmail = objForm1.ContactEmail;
        //                            ViewBag.UserName = objForm1.FirstName;
        //                            ViewBag.GrossTotal = objForm1.AmountPaid;
        //                            ViewBag.PaymentStatus = "Completed";
        //                            ViewBag.TransactionId = PNREF;
        //                            DateTime Date = DateTime.UtcNow;
        //                            ViewBag.PaymentDate = Date.ToString("MM/dd/yyyy");
        //                        }

        //                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Form1EventRegistration", 0, ref status);
        //                        if (objTemplates != null)
        //                        {
        //                            StringBuilder body = new StringBuilder();
        //                            body.Append(objTemplates.Description);
        //                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                            body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                            body.Replace("[SiteName]", objAppInfo.SiteName);
        //                            body.Replace("[ApplicationName]", objAppInfo.SiteName);
        //                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objForm1.FirstName + " " + objForm1.LastName));
        //                            body.Replace("[EventName]", BLL.Common.UppercaseFirst(objForm1.EventName));
        //                            body.Replace("[CategoryName]", BLL.Common.UppercaseFirst(objForm1.CategoryName));
        //                            body.Replace("[ContactEmail]", objForm1.ContactEmail);
        //                            body.Replace("[Gender]", objForm1.GradeinSchool);
        //                            body.Replace("[SchoolName]", objForm1.SchoolName);
        //                            body.Replace("[ContactFullName]", objForm1.ContactFullName);
        //                            body.Replace("[ContactPhone]", objForm1.ContactPhone);
        //                            body.Replace("[Notes]", objForm1.Notes);
        //                            body.Replace("[DateofBirth]", objForm1.DateofBirth.ToString("MM/dd/yyyy"));
        //                            body.Replace("[AmountPaid]", objForm1.AmountPaid.ToString());
        //                            body.Replace("[TransactionId]", objForm1.TransactionId);
        //                            body.Replace("[PaymentMethod]", "PayPal");
        //                            body.Replace("[PaymentStatus]", objForm1.PaymentStatus);
        //                            body.Replace("[PaymentDate]", objForm1.PaymentDate.ToString());

        //                            _sentmail.SendMailSendinbrevowithfrom(objForm1.ContactEmail, objAppInfo.EventsEmail, "Thank you for registering the " + objForm1.EventName + " - " + objForm1.CategoryName + ".", body.ToString());
        //                        }

        //                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("AdminForm1Registration", 0, ref status);
        //                        if (objTemplates1 != null)
        //                        {
        //                            StringBuilder body1 = new StringBuilder();
        //                            body1.Append(objTemplates1.Description);
        //                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                            body1.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                            body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                            body1.Replace("[ApplicationName]", objAppInfo.SiteName);
        //                            body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objForm1.FirstName + " " + objForm1.LastName));
        //                            body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objForm1.EventName));
        //                            body1.Replace("[CategoryName]", BLL.Common.UppercaseFirst(objForm1.CategoryName));
        //                            body1.Replace("[ContactEmail]", objForm1.ContactEmail);
        //                            body1.Replace("[Gender]", objForm1.GradeinSchool);
        //                            body1.Replace("[SchoolName]", objForm1.SchoolName);
        //                            body1.Replace("[ContactFullName]", objForm1.ContactFullName);
        //                            body1.Replace("[ContactPhone]", objForm1.ContactPhone);
        //                            body1.Replace("[Notes]", objForm1.Notes);
        //                            body1.Replace("[DateofBirth]", objForm1.DateofBirth.ToString("MM/dd/yyyy"));
        //                            body1.Replace("[AmountPaid]", objForm1.AmountPaid.ToString());
        //                            body1.Replace("[TransactionId]", objForm1.TransactionId);
        //                            body1.Replace("[PaymentMethod]", "PayPal");
        //                            body1.Replace("[PaymentStatus]", objForm1.PaymentStatus);
        //                            body1.Replace("[PaymentDate]", objForm1.PaymentDate.ToString());

        //                            _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.EventsEmail, "You have a new registration for the " + objForm1.EventName + " - " + objForm1.CategoryName + ".", body1.ToString());
        //                        }
        //                    }
        //                    else
        //                    {
        //                        StringBuilder body1 = new StringBuilder();
        //                        body1.Append("<p>Dear Admin, <br /><br />For your convenience, quick reminder email that payment for Event Registration has been failed. <br />");
        //                        body1.Append("<b>User Name:</b> " + objForm1.FirstName + "" + objForm1.LastName + "<br />");
        //                        body1.Append("<b>Email:</b> " + objForm1.ContactEmail + "<br />");
        //                        body1.Append("<b>Phone No:</b> " + objForm1.ContactPhone + "<br />");
        //                        body1.Append("<b>Payment Status:</b> Pending <br />");
        //                        body1.Append("<b>Paypal Response:</b> " + RESPMSG + "<br /><br />");
        //                        body1.Append("Thank You,<br />Admin</p>");

        //                        string tomail = ConfigurationManager.AppSettings["paymentfailemail"];

        //                        _sentmail.SendMailSendinbrevowithfrom(tomail, objAppInfo.EventsEmail, "Quick Remainder Payment failure Email for Event Registration on " + DateTime.Now.ToString("MMM dd - yyyy") + " - Telugu Association Tool", body1.ToString());

        //                        return RedirectToAction("PaymentFailed", "Home");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("FormThankYou", "Event", new { id = CompetitionRegistrationId, cname = objForm1.ChapterName });
        //            }
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"error closable\">Failed Transaction.</div>";
        //            return RedirectToAction("EventRegistrationForm1", "Event", new { eid = objForm1.EventId, EventName = BLL.Common.EncodeURL(objForm1.EventName), cid = objForm1.RegistrationCategoryId });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error closable\">Failed Transaction</div>";
        //        return RedirectToAction("EventRegistrationForm1", "Event", new { eid = objForm1.EventId, EventName = BLL.Common.EncodeURL(objForm1.EventName), cid = objForm1.RegistrationCategoryId });
        //    }
        //    return RedirectToAction("PaymentEventForm1Acknowledgement", "Event", new { id = objForm1.CompetitionRegistrationId, cname = objForm1.ChapterName });
        //}

        //public ActionResult PaymentEventForm1Acknowledgement(Int64 id = 0, string cname = "")
        //{
        //    try
        //    {
        //        Entities.Form1 objForm1 = new Entities.Form1();
        //        int result1 = 0;
        //        objForm1 = _Form1.GetForm1ById(id, ref result1);
        //        ViewBag.objForm1 = objForm1;
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Error404", "Error");
        //    }

        //    ViewBag.cname = cname;
        //    return View();
        //}

        //public ActionResult FormThankYou(Int64 id = 0, string EventName = "", string CategoryName = "", string cname = "")
        //{
        //    ViewBag.EventName = EventName;
        //    ViewBag.CategoryName = CategoryName;
        //    ViewBag.cname = cname;

        //    Entities.Form1 objForm1 = new Entities.Form1();
        //    int result1 = 0;
        //    int status2 = 0;
        //    int status = 0;
        //    objForm1 = _Form1.GetForm1ById(id, ref result1);
        //    ViewBag.objForm1 = objForm1;

        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

        //    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Form1TennisEventRegistration", 0, ref status);
        //    if (objTemplates != null)
        //    {
        //        StringBuilder body = new StringBuilder();
        //        body.Append(objTemplates.Description);
        //        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //        body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //        body.Replace("[SiteName]", objAppInfo.SiteName);
        //        body.Replace("[ApplicationName]", objAppInfo.SiteName);
        //        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objForm1.FirstName + " " + objForm1.LastName));
        //        body.Replace("[EventName]", BLL.Common.UppercaseFirst(objForm1.EventName));
        //        body.Replace("[CategoryName]", BLL.Common.UppercaseFirst(objForm1.CategoryName));
        //        body.Replace("[ContactEmail]", objForm1.ContactEmail);
        //        body.Replace("[Gender]", objForm1.GradeinSchool);
        //        body.Replace("[SchoolName]", objForm1.SchoolName);
        //        body.Replace("[ContactFullName]", objForm1.ContactFullName);
        //        body.Replace("[ContactPhone]", objForm1.ContactPhone);
        //        body.Replace("[Notes]", objForm1.Notes);
        //        body.Replace("[DateofBirth]", objForm1.DateofBirth.ToString("MM/dd/yyyy"));

        //        _sentmail.SendMailSendinbrevowithfrom(objForm1.ContactEmail, objAppInfo.EventsEmail, "Thank you for registering the " + objForm1.EventName + ".", body.ToString());
        //    }

        //    Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("AdminForm1TennisRegistration", 0, ref status);
        //    if (objTemplates1 != null)
        //    {
        //        StringBuilder body1 = new StringBuilder();
        //        body1.Append(objTemplates1.Description);
        //        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //        body1.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
        //        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //        body1.Replace("[SiteName]", objAppInfo.SiteName);
        //        body1.Replace("[ApplicationName]", objAppInfo.SiteName);
        //        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objForm1.FirstName + " " + objForm1.LastName));
        //        body1.Replace("[EventName]", BLL.Common.UppercaseFirst(objForm1.EventName));
        //        body1.Replace("[CategoryName]", BLL.Common.UppercaseFirst(objForm1.CategoryName));
        //        body1.Replace("[ContactEmail]", objForm1.ContactEmail);
        //        body1.Replace("[Gender]", objForm1.GradeinSchool);
        //        body1.Replace("[SchoolName]", objForm1.SchoolName);
        //        body1.Replace("[ContactFullName]", objForm1.ContactFullName);
        //        body1.Replace("[ContactPhone]", objForm1.ContactPhone);
        //        body1.Replace("[Notes]", objForm1.Notes);
        //        body1.Replace("[DateofBirth]", objForm1.DateofBirth.ToString("MM/dd/yyyy"));

        //        _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.EventsEmail, "You have a new registration for the " + objForm1.EventName + ".", body1.ToString());
        //    }

        //    return View();
        //}

        //#endregion

        //#region QRCode
        //public void GenerateQRCodeNew(long EventUserInfoId, ref List<Entities.EventRegTickets> lstEventRegTickets)
        //{
        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

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
        //            string ticketUrl = objAppInfo.UserSiteUrl + "ticket-preview?tktid=" + ticket.EventRegTicketId + "&euid=" + EventUserInfoId + "&uid=" + objEventUserInfo.EventId;

        //            // Unique file name
        //            string ticketRefCode = objAppInfo.SiteName + "-Ticket-" + ticket.EventRegTicketId;

        //            string savePath = objAppInfo.UploadPath + "\\QRCodeGeneration\\QR-of-" + ticketRefCode + ".jpg";

        //            SaveQRCodeImage(ticketUrl, savePath);
        //        }
        //    }
        //}
        //private void SaveQRCodeImage(string payload, string savePath)
        //{
        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        //    QRCode qrCode = new QRCode(qrCodeData);

        //    using (Bitmap bitMap = qrCode.GetGraphic(20))
        //    {
        //        // Save QR as image file
        //        bitMap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
        //    }
        //}
        //public void GenerateQRCode(Int64 EventUserInfoId)
        //{

        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
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

        //public ActionResult QRCodeView(Int64 id = 0, int uid = 0, String MembershipType = "")
        //{
        //    try
        //    {

        //        int _qstatus = 0;
        //        List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
        //        List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
        //        Entities.Events objEvents = new Entities.Events();
        //        List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
        //        objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

        //        if (_qstatus == 1)
        //        {
        //            ViewBag.objEventUserInfo = objEventUserInfo;
        //            ViewBag.lstEventRegCounts = lstEventRegCounts;
        //            ViewBag.lstEventsTickets = lstEventsTickets;
        //            ViewBag.uid = uid;
        //            ViewBag.MembershipType = MembershipType;
        //            ViewBag.status = _qstatus;
        //            if (User.Identity.IsAuthenticated && User.Identity.Name != null && HttpContext.Session.GetString("username") != null)
        //            {
        //                ViewBag.type = "yes";
        //            }
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //            return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //        return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
        //    }
        //    return View();
        //}

        //#endregion

        //#region PromoCode

        //[HttpPost]
        //public ActionResult checkPromoCode(string PromoCode, decimal Amount = 0)
        //{
        //    string str = "";
        //    BLL.PromoCodes _PromoCodes = new BLL.PromoCodes();

        //    try
        //    {
        //        Int32 _qstatus = 0;
        //        Entities.PromoCodes _objPromoCodes = _PromoCodes.GetPromocode(PromoCode, ref _qstatus);
        //        if (_qstatus == 1)
        //        {
        //            return Json(new { ok = true, data = _objPromoCodes });
        //        }
        //        else
        //        {
        //            str = "<div class=\"success closable\">Failed Transaction</div>";
        //            return Json(new { ok = false, data = str });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        str = "<div class=\"error1 closable\">" + ex.Message + "</div>";
        //        return Json(new { ok = false, data = str });
        //    }
        //}

        //#endregion

        //#region FormSubmissions

        //[HttpPost]
        //public async Task<JsonResult> InsertPaymentAsync(Entities.EventUserInfo payment, List<Entities.EventParticipants> lstEventParticipants, List<Entities.EventRegistrationCounts> lstEventRegistrationCounts, string payamount = "")
        //{
        //    //logreport("Sports Registration: Entered into Sports Registration - " + payment.FirstName + " - " + payment.Email, Request.Path);

        //    Int64 _status = 0;
        //    Int64 LRstatus = 0;
        //    Int64 EventUserInfoId = 0;
        //    List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
        //    Entities.LogReport logs = new Entities.LogReport();
        //    int status = 0;
        //    AWS_AssociationToolCore.Entities.AppInfo objAppInfo = new AWS_AssociationToolCore.Entities.AppInfo();
        //    AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
        //    objAppInfo = _AppInfo.GetAppInfoDetails(ref status);
        //    try
        //    {

        //        // Honeypot validation
        //        if (!string.IsNullOrWhiteSpace(payment.Honeypot))
        //        {
        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Events";
        //            logs.LogDescription = "Honeypot Bot activity detected -(" + objEventUserInfo.FirstName + " " + objEventUserInfo.LastName + " - " + EventUserInfoId + "  " + DateTime.UtcNow + ")";
        //            lstLogReport.Add(logs);
        //            LRstatus = _Users.InsertLogReport(lstLogReport);

        //            return Json(new { ok = false, data = "Honeypot Bot activity detected" });
        //        }
        //        string YoursecretKey = objAppInfo.CapchaSecreatKey;
        //        //  objAppInfo.CapchaSecreatKey; //Put Your Secret Key .

        //        //If capcha is "Yes", recapcha verification is process, if capcha is "No" without verification we can submit form...
        //        var capcha = objAppInfo.ShowCapcha;
        //        //objAppInfo.ShowCapcha;


        //        string captchaResponse = payment.recaptchaToken;
        //        string clientHost = Request.Url.Host;

        //        //Validate CAPTCHA
        //        bool isHuman = await BLL.Common.ValidateCaptchaAsync(captchaResponse, clientHost);

        //        if (capcha == "No")
        //        {
        //            isHuman = true;
        //        }


        //        if (!isHuman)
        //        {
        //            /* 
        //             * If the IsHuman boolean value is false, 
        //             * the form submission process will be stopped.
        //             */
        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Events";
        //            logs.LogDescription = "reCAPTCHA validation failed. Please try again -(" + payment.FirstName + " " + payment.LastName + " - " + EventUserInfoId + "  " + DateTime.UtcNow + ")";
        //            lstLogReport.Add(logs);
        //            LRstatus = _Users.InsertLogReport(lstLogReport);


        //            ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");

        //            return Json(new { ok = false, data = "reCAPTCHA validation failed. Please try again." });
        //        }


        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        payment.UpdatedBy = payment.FirstName;
        //        payment.InsertedBy = payment.FirstName;
        //        payment.IsApproved = true;
        //        payment.OtherPaymentId = payamount.ToString();
        //        //objEventUserInfo.Gender = DiscountPercent.ToString();
        //        payment.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //        payment.InsertedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //        //payment.IsActive = false;
        //        EventUserInfoId = payment.EventUserInfoId;
        //        string imageurl = "NA";
        //        string receipturl = "NA";
        //        if (lstEventRegistrationCounts != null && lstEventRegistrationCounts.Count != 0 && lstEventRegistrationCounts[0].EventRegistrationTypeId != 0)
        //        {
        //            payment.XMLRegistrationsCounts = BLL.Common.CreateXMLForObject(lstEventRegistrationCounts);
        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Events";
        //            logs.LogDescription = "Successfully created xml for Registration Tickets(" + payment.FirstName + " " + payment.LastName + " - " + EventUserInfoId + "  " + DateTime.UtcNow + ")";
        //            lstLogReport.Add(logs);
        //        }
        //        //_status = _PremierLeagueRegistrations.InsertPremierLeagueRegistrations(payment, ref PremierLeagueRegistrationId);
        //        _status = _Events.InsertEventUserInfo(payment, ref EventUserInfoId, ref imageurl, ref receipturl);

        //        logs = new Entities.LogReport();
        //        logs.LogTitle = "Events";
        //        logs.LogDescription = "Inserted basic info Successfuly(" + payment.FirstName + " " + payment.LastName + " - " + EventUserInfoId + "  " + DateTime.UtcNow + ")";
        //        lstLogReport.Add(logs);
        //        LRstatus = _Users.InsertLogReport(lstLogReport);

        //        //logreport("Sports Registration: Inserted basic info Successfuly (" + payment.FirstName + " - " + payment.Email + ")", Request.Path);
        //        if (EventUserInfoId != 0)
        //        {
        //            return Json(new { success = EventUserInfoId > 0, paymentId = EventUserInfoId });
        //        }
        //        else
        //        {
        //            return Json(new { ok = false, data = "Data inserted Failed" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { ok = false, data = ex.Message });
        //    }

        //}

        //[HttpPost]
        //public ActionResult PayPalSaveLog(string message)
        //{
        //    //try
        //    //{
        //    //    string folderPath = Server.MapPath("~/Content/Log/PayPalSaveLog/");
        //    //    if (!Directory.Exists(folderPath))
        //    //    {
        //    //        Directory.CreateDirectory(folderPath);
        //    //    }

        //    //    string fileName = "paypal_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
        //    //    string fullPath = Path.Combine(folderPath, fileName);

        //    //    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
        //    //    System.IO.File.AppendAllText(fullPath, logEntry);

        //    //    return Json(new { success = true });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Fallback log in case main logging fails
        //    //    System.IO.File.AppendAllText(Server.MapPath("~/Content/Log/PayPalSaveLog/error_fallback.log"),
        //    //        $"{DateTime.Now} - Failed to log PayPal: {ex.Message}{Environment.NewLine}");

        //    //    return Json(new { success = false, error = ex.Message });
        //    //}
        //}
        //#endregion
        //#region GenerateTickets

        //private string LoadTicketTemplate()
        //{
        //    string templatePath = Server.MapPath("~/Content/Templates/ticket.html");
        //    return System.IO.File.ReadAllText(templatePath);
        //}


        //public byte[] GenerateTicketPdf_iText7(string html, string fontDir)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var props = new ConverterProperties();

        //        // Setup fonts
        //        var fontProvider = new DefaultFontProvider(false, false, false);
        //        if (!string.IsNullOrEmpty(fontDir))
        //            fontProvider.AddDirectory(fontDir);   // e.g. "C:\\MyApp\\fonts"
        //        props.SetFontProvider(fontProvider);

        //        // Convert HTML directly to the output stream (ms)
        //        HtmlConverter.ConvertToPdf(
        //            new MemoryStream(Encoding.UTF8.GetBytes(html)), // html source
        //            ms,                                             // PDF output
        //            props                                           // converter options
        //        );

        //        return ms.ToArray();
        //    }
        //}


        //public byte[] MergePdfs(List<byte[]> pdfFiles)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        PdfDocument finalPdf = new PdfDocument(new PdfWriter(ms));
        //        PdfMerger merger = new PdfMerger(finalPdf);

        //        foreach (var pdf in pdfFiles)
        //        {
        //            using (var readerStream = new MemoryStream(pdf))
        //            {
        //                PdfDocument tempPdf = new PdfDocument(new PdfReader(readerStream));
        //                merger.Merge(tempPdf, 1, tempPdf.GetNumberOfPages());
        //                tempPdf.Close();
        //            }
        //        }

        //        finalPdf.Close();
        //        return ms.ToArray();
        //    }
        //}


        //private string GetDaySuffix(int day)
        //{
        //    if (day >= 11 && day <= 13) return "TH";
        //    switch (day % 10)
        //    {
        //        case 1: return "ST";
        //        case 2: return "ND";
        //        case 3: return "RD";
        //        default: return "TH";
        //    }
        //}
        //public ActionResult EventQR(string id1 = "", Int64 id = 0, int uid = 0, String MembershipType = "")
        //{
        //    try
        //    {
        //        id = Convert.ToInt64(BLL.Common.UnEncryptString(id1.ToString()));

        //        int _qstatus = 0;
        //        List<Entities.EventRegistrationCounts> lstEventRegCounts = new List<Entities.EventRegistrationCounts>();
        //        List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
        //        List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
        //        Entities.Events objEvents = new Entities.Events();


        //        objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(id, 0, ref objEvents, ref lstEventRegCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);

        //        if (_qstatus == 1)
        //        {
        //            ViewBag.objEventUserInfo = objEventUserInfo;
        //            ViewBag.lstEventRegCounts = lstEventRegCounts;
        //            ViewBag.lstEventsTickets = lstEventsTickets;
        //            ViewBag.lstEventRegTickets = lstEventRegTickets;
        //            ViewBag.uid = uid;
        //            ViewBag.MembershipType = MembershipType;
        //            ViewBag.status = _qstatus;
        //            if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && Session["username"] != null)
        //            {
        //                ViewBag.type = "yes";
        //            }
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //            return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //        return RedirectToAction("QRCodeView", "Event", new { id = id, uid = uid });
        //    }
        //    return View();
        //}

        //public ActionResult QRIndividualTicketPreview(Int64 tktid = 0, Int64 euid = 0, Int64 eid = 0, int uid = 0, String MembershipType = "")
        //{
        //    try
        //    {

        //        int _qstatus = 0;
        //        Entities.EventRegTickets objEventRegTickets = new Entities.EventRegTickets();

        //        objEventRegTickets = _Events.EventUserInfoIndividualGetById(uid, euid, tktid, ref _qstatus);

        //        if (_qstatus == 1)
        //        {
        //            ViewBag.objEventRegTickets = objEventRegTickets;

        //            ViewBag.uid = uid;
        //            ViewBag.MembershipType = MembershipType;
        //            ViewBag.status = _qstatus;
        //            if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && Session["username"] != null)
        //            {
        //                ViewBag.type = "yes";
        //            }
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //            return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = tktid, uid = uid });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
        //        return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = tktid, uid = uid });
        //    }
        //    return View();
        //}


        //[HttpPost]
        //public ActionResult InsertEventCheckIn(Entities.EventRegCheckIn objcheckin, int uid = 0)
        //{
        //    try
        //    {
        //        objcheckin.CheckInDate = DateTime.UtcNow;
        //        Int64 status = _Events.EventCheckInInsert(objcheckin);

        //        if (status == 1 || status == 2)
        //        {
        //            TempData["message"] = "<div class=\"success closable\"> Updated comment successfully.</div>";
        //            return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });

        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error-alert closable\">Failed updating registration fields.</div>";
        //        return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });
        //    }
        //    return RedirectToAction("QRIndividualTicketPreview", "Event", new { tktid = objcheckin.EventRegTicketId, euid = objcheckin.EventUserInfoId });

        //}
        #endregion

    }
}


