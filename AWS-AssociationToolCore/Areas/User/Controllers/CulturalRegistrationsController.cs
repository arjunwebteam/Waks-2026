using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.DAL;
using AWS_AssociationToolCore.Entities;
using AWS_AssociationToolCore.Models;
using Braintree;
using DocumentFormat.OpenXml.EMMA;
using Grpc.Core;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json.Linq;
using PaymentProcess;
using QRCoder;
using Stripe;
using Stripe.Checkout;
using System;
using System;
using System;
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
using Session = Stripe.Checkout.Session;
namespace AWS_AssociationToolCore.Areas.User.Controllers
{

    [Area("User")]
    public class CulturalRegistrationsController : Controller
    {
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        AWS_AssociationToolCore.BLL.CulturalRegistrations _Cultural = new BLL.CulturalRegistrations();
        AWS_AssociationToolCore.BLL.PerformanceCategory _PerformanceCategory = new BLL.PerformanceCategory();
        BLL.SendMail _sentmail = new BLL.SendMail();
        Entities.CulturalRegistrations objCulturalRegistrations = new Entities.CulturalRegistrations();
        Entities.CulturalParticipants objCulturalParticipants = new Entities.CulturalParticipants();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();
        PayByPayPal objPayByPayPal = new PayByPayPal();
        BLL.Users _Users = new BLL.Users();
        BLL.Members _Members = new BLL.Members();
        Entities.Members _objMember = new Entities.Members();
        BLL.ServiceDonations _ServiceDonations = new BLL.ServiceDonations();
        BLL.Chapters _Chapters = new BLL.Chapters();
        Entities.Chapters objChapters = new Entities.Chapters();

        private readonly IBraintreeConfiguration _config;
        public CulturalRegistrationsController(IBraintreeConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(Int64 EventId = 0, Int64 MemberId = 0, string cname = "", string EventName = "")
        {
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            List<Entities.PaymentStatus> lstpaymentstatus = new List<Entities.PaymentStatus>();
            List<Entities.PerformanceCategory> lstPerformanceCategory = new List<Entities.PerformanceCategory>();
            int status = 0;
            string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);

            Response.Cookies.Append("FormValidationCode", generatedCode, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps
            });
            if (string.IsNullOrEmpty(cname))
            {
                cname = "global";
            }
            try
            {
                int _qstatus = 0;
                Int64 CulturalRegistrationId = 0;
                objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                lstPerformanceCategory = _PerformanceCategory.GetPerformanceCategoryList(ref status);
                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);
                lstpaymentstatus = _paymentSettings.GetPaymentStatusList(ref status);
                if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && HttpContext.Session != null && HttpContext.Session.GetString("ChapterName") != null)
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(HttpContext.User.Identity.Name.ToString(), ref status);
                }
                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);

                MemberId = Convert.ToInt64(HttpContext.Session.GetString("MemberID") ?? "0");
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.Memberdetails = _objMember;
            ViewBag.MemberId = MemberId;
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            ViewBag.lstpaymentstatus = lstpaymentstatus;
            ViewBag.lstPerformanceCategory = lstPerformanceCategory;
            ViewBag.EventId = EventId;
            ViewBag.ValidationCode = generatedCode;
            ViewBag.cname = cname;
            ViewBag.EventName = EventName;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCulturalRegistration(IFormFile file, IFormFile filezelle, Entities.CulturalRegistrations objCulturalRegistrations, List<Entities.CulturalParticipants> lstCulturalParticipants, string cname = "", string MCaptcha = "", string Honeypot = "", string ValidationCode = "", string recaptchaToken = ""/*, string Secure = ""*/)
        {
            try
            {
                IBraintreeGateway gateway = _config.GetGateway();

                List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

                Entities.LogReport logs = new Entities.LogReport();
                Int64 LRstatus = 0;
                int status = 0;
                Int64 LogId = 0;
                var objLogReport = new Entities.ApplicationLogs
                {
                    LogTitle = "Cultural Registration",
                    LogDescription = $"Entered into Cultural Registration Process",
                    LogDate = DateTime.Now,
                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                };
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);
                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "CulturalRegistration",
                        LogDescription = "Honeypot Bot activity detected",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("Index", "CulturalRegistrations");
                }
                if (Request.Form["ClientValidationStatus"] == "true")
                {
                    // Skip ModelState validation for the payment gateway fields
                    ModelState.Clear();
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
                var LogSubReport = new Entities.Logdetails
                {

                    LogId = LogId,
                    LogTitle = "Captcha Validation",
                    LogDescription = isHuman
                   ? $"Captcha passed. Mode: {capcha}"
                   : $"Captcha failed. Mode: {capcha}",

                    LogDate = DateTime.Now,
                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()


                };


                _Users.LogSubReportInsert(LogSubReport);
                if (!isHuman)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "culturalRegistrations Captcha Failed - Redirect",
                        LogDescription = "User redirected back due to captcha failure.",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });


                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("Index", "CulturalRegistrations");
                }
                // Retrieve the code from the cookie
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "culturalRegistrations",
                        LogDescription = "Honeypot Bot activity detected",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError(string.Empty, "Kumar2 reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("EventRegistration", "Event", new { eid = objCulturalRegistrations.EventId, EventName = BLL.Common.EncodeURL(objCulturalRegistrations.EventName), cname = cname });
                }

                // Retrieve the code from the cookie
                string cookieValidationCode = Request.Cookies["FormValidationCode"];
                if (string.IsNullOrEmpty(cookieValidationCode))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "culturalRegistrations",
                        LogDescription = "Failed Retrieve the code from the cookie",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("Index", "Services");
                }

                if (cookieValidationCode != ValidationCode)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "culturalRegistrations",
                        LogDescription = "Failed Security validation failed",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("Index", "Services");
                }


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

                if (file != null && file.Length > 0)
                {
                    originalImageFile = SanitizeFileName(file.FileName);
                    receipturl = originalImageFile;
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    originalReceiptFile = SanitizeFileName(filezelle.FileName);
                    receipturl = originalReceiptFile;
                }

                // ✅ Save BEFORE DB insert - SP will change these values
                string originalImageUrl = imageurl;
                string originalReceiptUrl = receipturl;

                Int64 ChapterId = 0;

                int qstatus = 0;
                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                    cname = "global";
                }

        
                //if (ModelState.IsValid) //Condition put for message show based on True or false.  
                //{
                objCulturalRegistrations.InsertedBy = objCulturalRegistrations.FirstName;
                    objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;

                    //List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
                    //Entities.LogReport logs = new Entities.LogReport();

                    Int64 CulturalRegistrationId = 0;
                    objCulturalRegistrations.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objCulturalRegistrations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objCulturalRegistrations.ChapterId = ChapterId;
                    Int64 _status = _Cultural.InsertCulturalRegistrations(objCulturalRegistrations, ref CulturalRegistrationId, ref receipturl);
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "EventUserInfo Insert",
                    LogDescription = _status == 1
                                                 ? $"CulturalRegistrations inserted successfully. CulturalRegistrationId: {CulturalRegistrationId}"
                                                 : $"CulturalRegistrations insert failed. Status: {_status}",
                    LogDate = DateTime.Now,
                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                string finalFolder = Path.Combine(objAppInfo.UploadPath, "CulturalRegistration");
                Directory.CreateDirectory(finalFolder);

                if (file != null)
                {
                    // imageurl now has SP-generated name e.g. "183-test.about_img_1.jpg"
                    string finalPath = Path.Combine(finalFolder, receipturl);
                    await using var stream1 = new FileStream(finalPath, FileMode.Create);
                    await file.CopyToAsync(stream1);
                }

                if (filezelle != null)
                {
                    string finalPath = Path.Combine(finalFolder, receipturl);
                    await using var stream2 = new FileStream(finalPath, FileMode.Create);
                    await filezelle.CopyToAsync(stream2);
                }
              


                if (lstCulturalParticipants != null && lstCulturalParticipants.Count() != 0)
                    {
                        foreach (Entities.CulturalParticipants objCulturalParticipants in lstCulturalParticipants)
                        {
                            if (objCulturalParticipants != null && objCulturalParticipants.FirstName != null)
                            {
                                objCulturalParticipants.CulturalRegistrationId = CulturalRegistrationId;
                                Int64 estatus = _Cultural.InsertCulturalParticipants(objCulturalParticipants);
                                if (estatus == -1)
                                {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "CulturalParticipants Failed",
                                    LogDescription = $"Failed inserting CulturalParticipants  details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objCulturalParticipants.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objCulturalParticipants.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                TempData["message"] = "<div class=\"error1 closable\">Failed uploading Participant Details.</div>";
                                    return RedirectToAction("Index", "CulturalRegistrations");
                                }
                            }
                        }
                    }



                    if (objCulturalRegistrations.AmountPaid != 0)
                    {
                    if (objCulturalRegistrations.PaymentMethod == "Square")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Square Payment",
                            LogDescription = "Square Payment started",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

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
                        long amountCents = (long)(objCulturalRegistrations.AmountPaid * 100);
                        var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();
                        var createPaymentBody = new Square.Models.CreatePaymentRequest.Builder(squareNonce, Guid.NewGuid().ToString(), money)
                            .LocationId(squareLocationId)
                            .Note($"Cultural Registrations  - {objCulturalRegistrations.PerformanceCategory} | {objCulturalRegistrations.FirstName} {objCulturalRegistrations.LastName}")
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
                                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                string squareTransactionId = squareResponse.Payment.Id;

                                // ✅ Session fix
                                HttpContext.Session.SetString("CTotalAmount", objCulturalRegistrations.AmountPaid.ToString());


                                string Username = !string.IsNullOrEmpty(objCulturalRegistrations.FirstName)
                                    ? objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName
                                    : User.Identity?.Name ?? "";


                                objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                                objCulturalRegistrations.PaymentMethod = "Square";
                                objCulturalRegistrations.TransactionId = squareTransactionId;
                                objCulturalRegistrations.PaymentStatus = "Completed";
                                objCulturalRegistrations.PaymentStatusId = 1;
                                objCulturalRegistrations.PaymentMethodId = 14;
                                objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                                objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objCulturalRegistrations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objCulturalRegistrations.UpdatedBy = Username;
                                int test = 0;

                                Int64 result1 = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                                if (result1 == 1)
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Event CulturalRegistrations",
                                        LogDescription = "Updated CulturalRegistrations Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    int status1 = 0;
                                    AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                                    objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);
                                    int _qstatus = 0;


                                    objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Get CulturalRegistrations Details",
                                        LogDescription = "Get By CulturalRegistrations Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                                    if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                                    {
                                        int i = 1;
                                        foreach (var item in lstCulturalParticipants)
                                        {
                                            if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                                            {

                                                Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                                            }
                                            else
                                            {
                                                Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                                            }

                                            i++;
                                        }

                                    }
                                    Participants = Participants + "</table>";
                                    //Send Mail to User
                                    string BaseUrl = objAppInfo.UserSiteUrl;
                                    int MailStatus = 0;
                                    Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                    objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                        body.Replace("[Email]", objCulturalRegistrations.Email);
                                        body.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                        body.Replace("[PaymentType]", "Square");
                                        body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                        body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[Participants]", Participants);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());
                                        body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                        body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                        body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                        body.Replace("[EventName]", objCulturalRegistrations.EventName);
                                        body.Replace("[PaymentStatus]", "Completed");

                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "User Email Sent",
                                                LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }
                                    //Send Mail to Admin
                                    int statusAdmin = 0;
                                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                        body1.Replace("[Email]", objCulturalRegistrations.Email);
                                        body1.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                        body1.Replace("[PaymentType]", "Square");
                                        body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                        body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[Participants]", Participants);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                        body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                        body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                        body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                        body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                                        body1.Replace("[PaymentStatus]", "Completed");
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Admin Email Sent",
                                                LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }


                                    return RedirectToAction("Acknowledgement", "CulturalRegistrations", new { CulturalRegistrationId = CulturalRegistrationId, cname = cname, PaymentMethod = "Square" });
                                }
                                else
                                {

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "squre Payment Failed",
                                        LogDescription = "squre Payment Failed",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    return RedirectToAction("PaymentFailed", "Home");
                                }
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
                                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }
                        catch (Square.Exceptions.ApiException sqEx)
                        {
                            string errDetail = string.Join(" | ", sqEx.Errors?.Select(e => $"{e.Code}: {e.Detail}") ?? new[] { sqEx.Message });
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = $"Customer/Card creation failed: {errDetail} | MemberId:{objCulturalRegistrations.CulturalRegistrationId}",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            return RedirectToAction("PaymentFailed", "Home");
                        }
                    }

                    else if (objCulturalRegistrations.PaymentMethod == "Credit Card")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Credit Card Payment",
                            LogDescription = "Credit Card Payment started",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        try
                        {
                            Entities.MembershipOrders objCulturalRegistrationshipOrders = new Entities.MembershipOrders();

                            // ✅ 1. Get Amount safely
                            decimal amount = objCulturalRegistrations.AmountPaid;

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
                                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                var transaction = result.Target;
                                objCulturalRegistrations.TransactionId = transaction.Id;
                                objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;

                                objCulturalRegistrations.PaymentStatusId = 1;
                                objCulturalRegistrations.PaymentMethodId = 13;
                                objCulturalRegistrations.PaymentMethod = "Credit Card";
                                // objCulturalRegistrations.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                                objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                                objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                                objCulturalRegistrations.PaymentStatus = "Completed";
                                objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                                Int64 result1 = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                                if (result1 == 1)
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Event CulturalRegistrations",
                                        LogDescription = "Updated CulturalRegistrations Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    int status1 = 0;
                                    AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                                    objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);
                                    int _qstatus = 0;


                                    objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Get CulturalRegistrations Details",
                                        LogDescription = "Get By CulturalRegistrations Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                                    if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                                    {
                                        int i = 1;
                                        foreach (var item in lstCulturalParticipants)
                                        {
                                            if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                                            {

                                                Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                                            }
                                            else
                                            {
                                                Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                                            }

                                            i++;
                                        }

                                    }
                                    Participants = Participants + "</table>";
                                    //Send Mail to User
                                    string BaseUrl = objAppInfo.UserSiteUrl;
                                    int MailStatus = 0;
                                    Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                                    objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                        body.Replace("[Email]", objCulturalRegistrations.Email);
                                        body.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                        body.Replace("[PaymentType]", "Credit Card");
                                        body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                        body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[Participants]", Participants);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());
                                        body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                        body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                        body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                        body.Replace("[EventName]", objCulturalRegistrations.EventName);
                                        body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);

                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "User Email Sent",
                                                LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }
                                    //Send Mail to Admin
                                    int statusAdmin = 0;
                                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                        body1.Replace("[Email]", objCulturalRegistrations.Email);
                                        body1.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                        body1.Replace("[PaymentType]", "Credit Card");
                                        body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                        body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[Participants]", Participants);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                        body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                        body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                        body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                        body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                                        body1.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Admin Email Sent",
                                                LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }


                                    return RedirectToAction("Acknowledgement", "CulturalRegistrations", new { CulturalRegistrationId = CulturalRegistrationId, cname = cname, PaymentMethod = "Credit Card" });
                                }
                                else
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Payment Failed ",
                                        LogDescription = "credit card Payment Deatails Inserting Failed",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
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
                                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
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
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            TempData["message"] = "Payment error: " + ex.Message;
                            return RedirectToAction("PaymentFailed", "Home");
                        }
                    }

                    else if (objCulturalRegistrations.PaymentMethod == "PayPal")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Event CulturalRegistrations",
                            LogDescription = "CulturalRegistrations Payment PayPal PaymentMethod Entering",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        decimal paymentAmount = objCulturalRegistrations.AmountPaid;



                        string Username = "";

                        if (objCulturalRegistrations.FirstName != null)
                        {
                            Username = objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName;
                        }
                        else
                        {
                            Username = HttpContext.User.Identity.Name.ToString();
                        }
                        if (Username != null)
                        {
                            objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                            objCulturalRegistrations.PaymentMethod = "PayPal";
                            objCulturalRegistrations.TransactionId = objCulturalRegistrations.TransactionId;
                            objCulturalRegistrations.PaymentStatusId = 1;
                            objCulturalRegistrations.PaymentMethodId = 1;
                            objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                            //objCulturalRegistrations.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                            objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                            objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                            objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                            objCulturalRegistrations.Email = objCulturalRegistrations.Email;
                            objCulturalRegistrations.PaymentStatus = "Completed";

                            Int64 result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Event CulturalRegistrations",
                                LogDescription = "Updated CulturalRegistrations Details",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            int status1 = 0;
                            AWS_AssociationToolCore.BLL.AppInfo _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
                            objAppInfo = _AppInfo.GetAppInfoDetails(ref status1);
                            int _qstatus = 0;


                            objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Get CulturalRegistrations Details",
                                LogDescription = "Get By CulturalRegistrations Details",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                            if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                            {
                                int i = 1;
                                foreach (var item in lstCulturalParticipants)
                                {
                                    if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                                    {

                                        Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                                    }
                                    else
                                    {
                                        Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                                    }

                                    i++;
                                }

                            }
                            Participants = Participants + "</table>";
                            //Send Mail to User
                            string BaseUrl = objAppInfo.UserSiteUrl;
                            int MailStatus = 0;
                            Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                            objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body.Replace("[Email]", objCulturalRegistrations.Email);
                                body.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                body.Replace("[PaymentType]", "PayPal");
                                body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[Participants]", Participants);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());
                                body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);

                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }
                            //Send Mail to Admin
                            int statusAdmin = 0;
                            Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body1.Replace("[Email]", objCulturalRegistrations.Email);
                                body1.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                                body1.Replace("[PaymentType]", "PayPal");
                                body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[Participants]", Participants);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body1.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            var redirectUrl = Url.Action("Acknowledgement", "CulturalRegistrations", new
                            {
                                area = "User",
                                CulturalRegistrationId = CulturalRegistrationId,
                                cname = cname,
                                PaymentMethod = "PayPal"
                            });
                            return Json(new { success = true, redirectUrl = redirectUrl });
                        }
                    }
                    /*Cash/Cheque - Payment either a Cash (or) Cheque*/
                    else if (objCulturalRegistrations.PaymentMethod == "Cash/Cheque")
                    {

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PaymentMethod Cash/Cheque Entered",
                            LogDescription = $"Entered into Cash/Cheque Donation Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        objCulturalRegistrations.PaymentMethod = "Cash/Cheque";
                        objCulturalRegistrations.TransactionId = "NA";
                        objCulturalRegistrations.PaymentStatus = "Pending";
                        objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                        objCulturalRegistrations.PaymentStatusId = 2;
                        objCulturalRegistrations.PaymentMethodId = 3;
                        //objCulturalRegistrations.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                        objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                        objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                        objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                        Int64 result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Updated Cash/Cheque PAYMENT DEATILS CulturalRegistrations",
                            LogDescription = $"Updated Cash/Cheque PAYMENT DEATILS CulturalRegistrations",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        if (result == 1)
                        {
                            int _qstatus = 0;

                            objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                            string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                            if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                            {
                                int i = 1;
                                foreach (var item in lstCulturalParticipants)
                                {
                                    if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                                    {

                                        Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                                    }
                                    else
                                    {
                                        Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                                    }

                                    i++;
                                }

                            }
                            Participants = Participants + "</table>";
                            //Send Mail to User
                            string BaseUrl = objAppInfo.UserSiteUrl;
                            int MailStatus = 0;
                            Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                            objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body.Replace("[Email]", objCulturalRegistrations.Email);
                                body.Replace("[TransactionId]", "N/A");
                                body.Replace("[PaymentType]", "Cash/Cheque");
                                body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[Participants]", Participants);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());
                                body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }
                            //Send Mail to Admin
                            int statusAdmin = 0;
                            Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body1.Replace("[Email]", objCulturalRegistrations.Email);
                                body1.Replace("[TransactionId]", "N/A");
                                body1.Replace("[PaymentType]", "Cash/Cheque");
                                body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[Participants]", Participants);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body1.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);


                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            return RedirectToAction("ThankYou", "CulturalRegistrations", new { area = "User", Id = CulturalRegistrationId, cname = cname, PaymentMethod = "Cash/Cheque" });
                        }
                    }
                    /*Zelle - Saving zelle transaction details*/
                    else if (objCulturalRegistrations.PaymentMethod == "Zelle")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "CulturalRegistrations",
                            LogDescription = $"Entered Into Zelle CulturalRegistrations",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        objCulturalRegistrations.PaymentMethod = "Zelle";
                        objCulturalRegistrations.PaymentStatus = "Pending";
                        objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                        objCulturalRegistrations.PaymentStatusId = 2;
                        objCulturalRegistrations.PaymentMethodId = 6;
                        //objCulturalRegistrations.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                        objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                        objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                        objCulturalRegistrations.TransactionId = objCulturalRegistrations.TransactionId;
                        // objCulturalRegistrations.PaymentDate = objCulturalRegistrations.PaymentDate;
                        objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        Int64 result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Updated CulturalRegistrations Details",
                            LogDescription = $"Updated EventUserInfo Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        if (result == 1)
                        {
                            int _qstatus = 0;

                            objCulturalRegistrations = _Cultural.GetCulturalRegistrationsById(CulturalRegistrationId, ref _qstatus);
                            string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                            if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                            {
                                int i = 1;
                                foreach (var item in lstCulturalParticipants)
                                {
                                    if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                                    {

                                        Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                                    }
                                    else
                                    {
                                        Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                                    }

                                    i++;
                                }

                            }
                            Participants = Participants + "</table>";
                            //Send Mail to User
                            string BaseUrl = objAppInfo.UserSiteUrl;
                            int MailStatus = 0;
                            Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                            objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body.Replace("[Email]", objCulturalRegistrations.Email);
                                body.Replace("[TransactionId]", "N/A");
                                body.Replace("[PaymentType]", "Zelle");
                                body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[Participants]", Participants);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());
                                body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }
                            //Send Mail to Admin
                            int statusAdmin = 0;
                            Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                                body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                                body1.Replace("[Email]", objCulturalRegistrations.Email);
                                body1.Replace("[TransactionId]", "N/A");
                                body1.Replace("[PaymentType]", "Zelle");
                                body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                                body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[Participants]", Participants);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());
                                body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                                body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                                body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                                body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                                body1.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);

                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Admin",
                                        LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            return RedirectToAction("ThankYou", "CulturalRegistrations", new { area = "User", Id = CulturalRegistrationId, cname = cname, PaymentMethod = "Zelle" });
                        }
                    }
                    else if (objCulturalRegistrations.PaymentMethod == "Stripe")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PaymentMethod Stripe Entered",
                            LogDescription = $"Entered into Stripe Donation Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        Int64 result = 0;
                        objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                        objCulturalRegistrations.PaymentMethod = "PayPal";
                        objCulturalRegistrations.TransactionId = objCulturalRegistrations.TransactionId;
                        objCulturalRegistrations.PaymentStatusId = 1;
                        objCulturalRegistrations.PaymentMethodId = 11;
                        // objCulturalRegistrations.PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                        objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                        objCulturalRegistrations.AmountPaid = objCulturalRegistrations.AmountPaid;
                        objCulturalRegistrations.UpdatedBy = objCulturalRegistrations.FirstName;
                        objCulturalRegistrations.Email = objCulturalRegistrations.Email;
                        objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "objCulturalRegistrations Stripe updated Details",
                            LogDescription = $"updated  Stripe objCulturalRegistrations Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int status1 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status1);



                        // Set your Stripe Secret KeyB
                        StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;
                        string currency = "usd";
                        var priceOptions = new PriceCreateOptions
                        {
                            UnitAmount = (long)(objCulturalRegistrations.AmountPaid * 100),
                            Currency = currency.ToLower(),
                            ProductData = new PriceProductDataOptions { Name = "Cultural Registration" }
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
                            SuccessUrl = $"{objAppInfo.BaseUrl}User/CulturalRegistrations/Acknowledgement?session_id={{CHECKOUT_SESSION_ID}}&CulturalRegistrationId={CulturalRegistrationId}&PaymentMethod=Stripe&cname={cname}",
                            CancelUrl = Url.Action("Acknowledgement", "CulturalRegistrations", null, scheme)
                        };


                        var sessionService = new SessionService();
                        var session = sessionService.Create(sessionOptions);

                        // Store Session ID in Cookie (Fallback)
                        Response.Cookies.Append("StripeSessionId", session.Id);

                        if (result == 1)
                        {
                            LRstatus = _Users.InsertLogReport(lstLogReport);

                            if (!string.IsNullOrEmpty(session.Url))
                            {
                                // return Json(new { success = true, paymentLinkUrl = session.Url });
                                return Redirect(session.Url);

                            }
                        }


                    }
                    else if (objCulturalRegistrations.PaymentMethod == "Swire Pay")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PaymentMethod Swire Pay Entered",
                            LogDescription = $"Entered into Swire Pay Event Registration Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        // Initialize HttpClient and the API request
                        var client1 = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PaymentMethod Swire Pay Entered",
                            LogDescription = $"Entered into Card Payment Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int status1 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status1);

                        // Add API key to headers
                        request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);


                        // Prepare JSON content for the API request
                        var redirectUri = Url.Action("Acknowledgement", "CulturalRegistrations", new
                        {
                            CulturalRegistrationId = CulturalRegistrationId,  // Ensure MemberId is set
                            PaymentMethod = "Swire Pay",
                            cname = cname
                        }, protocol: Request.Scheme);
                        decimal amount = (int)objCulturalRegistrations.AmountPaid;
                        decimal amountInINR = (int)objCulturalRegistrations.AmountPaid;
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
                                            ""description"": ""Event Registration payment for {objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName}"",
                                            ""statementDescriptor"": ""Event Registration Payment"",
                                            ""expiresAt"": ""{DateTime.UtcNow.AddDays(7).AddYears(2050 - DateTime.UtcNow.Year):yyyy-MM-ddTHH:mm:ss.fffZ}"",
                                            ""email"": ""{objCulturalRegistrations.Email}"",
                                            ""phone"": ""{objCulturalRegistrations.PhoneNo}"",
                                            ""name"": ""{objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName}""
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
                            LogTitle = "Cultural Registrations",
                            LogDescription = paymentLink,
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        // Check if the payment link is available and redirect to it
                        if (!string.IsNullOrEmpty(paymentLink))
                        {
                            return Redirect(paymentLink);  // Redirects the user to the payment link
                        }
                        else
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Cultural Registrations",
                                LogDescription = "Swire Pay Payment link not found in the response.",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            return RedirectToAction("Acknowledgement", "CulturalRegistrations", new { CulturalRegistrationId = CulturalRegistrationId, cname = cname, PaymentMethod = objCulturalRegistrations.PaymentMethod, tid = objCulturalRegistrations.TransactionId });

                        }

                    }
                }
                    
                    if (_status == 1 || _status == 2)
                    {
                        int status1 = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("User Cultural Registration", 0, ref status1);
                        if (objTemplates != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Email]", objCulturalRegistrations.Email);

                            _sentmail.SendMailSendinbrevo(objCulturalRegistrations.Email, "Thanks For Becoming A Cultural Registration - " + objAppInfo.SiteName, body.ToString());
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "User Email Sent",
                            LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }

                        int status2 = 0;
                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Cultural Registration For Admin", 0, ref status2);
                        if (objTemplates1 != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates1.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName));
                            body.Replace("[LastName]", BLL.Common.UppercaseFirst(objCulturalRegistrations.LastName));
                            body.Replace("[Email]", objCulturalRegistrations.Email);
                            body.Replace("[PhoneNo]", objCulturalRegistrations.PhoneNo);
                            body.Replace("[Address]", objCulturalRegistrations.Address1);
                            body.Replace("[City]", objCulturalRegistrations.City);
                            body.Replace("[State]", objCulturalRegistrations.State);
                            body.Replace("[Zip]", objCulturalRegistrations.ZipCode);
                            body.Replace("[AAALifetimeMember]", objCulturalRegistrations.AAALifetimeMember);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);

                            _sentmail.SendMailSendinbrevo(objAppInfo.VolunteerEmail, "You have a new Cultural Registration from " + objAppInfo.SiteName, body.ToString());
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Admin Email Sent",
                            LogDescription = $"Confirmation email sent to Admin: {objAppInfo.VolunteerEmail}",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }
                        return RedirectToAction("ThankYou", "CulturalRegistrations");

                    }

                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                        return RedirectToAction("Index", "CulturalRegistrations");
                    }
                //}
                //else
                //{
                //    TempData["message"] = "<div class=\"error1 closable\">Google reCaptcha not validated Successfully</div>";
                //    return RedirectToAction("Index", "CulturalRegistrations");
                //}

                //else
                //{
                //    TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                //    return RedirectToAction("AddCulturalRegistration", "CulturalRegistrations");
                //}
            }
            catch(Exception ex)
            {
                
                TempData["message"] = ex.Message;
                TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
                return RedirectToAction("Index", "CulturalRegistrations");
            }
        }
        

        public async Task<ActionResult> Acknowledgement(Int64 CulturalRegistrationId = 0, string cname = "", string PaymentMethod = "PayPal", string tid = "", string pstatus = "", string session_id = "", string spPaymentLink = "")
        {
            int status1 = 0;
            string Username = "";
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            Entities.LogReport logs = new Entities.LogReport();

            DateTime paydate = DateTime.UtcNow;

            Entities.CulturalRegistrations objCulturalRegistrations = new Entities.CulturalRegistrations();
           
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = " Cultural Registrations",
                LogDescription = " CulturalRegistrations Acknowledgement Page Entering",
                LogDate = DateTime.Now,
                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objCulturalRegistrations.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            try
            {
                if (PaymentMethod == "Stripe")
                {
                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status1);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "CulturalRegistrations Acknowledgement Page Stripe PaymentMethod Entering",
                        LogDescription = "CulturalRegistrations Acknowledgement Page Stripe PaymentMethod  getting payment details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
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

                    objCulturalRegistrations.PaymentMethod = PaymentMethod;
                    objCulturalRegistrations.TransactionId = paymentIntent.Id;
                    objCulturalRegistrations.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                    objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                    objCulturalRegistrations.UpdatedBy = Username;
                    objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    Int64 result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "CulturalRegistrations ThankYou Page",
                        LogDescription = "Updated Transaction details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                }
               else if (PaymentMethod == "Swire Pay")
                {
                    spPaymentLink = Request.Query["sp-payment-link"].ToString();
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "CulturalRegistrations ThankYou Page",
                        LogDescription = "Swire Pay Started into ThankYou Page",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    spPaymentLink = Request.Query["sp-payment-link"].ToString();
                    var client = new HttpClient();
                    int status2 = 0;

                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

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
                        var status = paymentSession["status"]?.ToString();
                        //var status = jsonData["entity"]?["status"]?.ToString();
                        pstatus = paymentType;
                        tid = receiptNumber;

                        // Output the values
                        Console.WriteLine($"Receipt Number: {receiptNumber ?? "Not Available"}");
                        Console.WriteLine($"Payment Type: {paymentType ?? "Not Available"}");



                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "CulturalRegistrations ThankYou Page",
                            LogDescription = "Response Status is " + status + " into Acknowledgement Page",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        objCulturalRegistrations.PaymentMethod = PaymentMethod;
                        objCulturalRegistrations.TransactionId = receiptNumber;
                        objCulturalRegistrations.PaymentStatus = status == "SUCCEEDED" ? "Completed" : "Pending";
                        objCulturalRegistrations.CulturalRegistrationId = CulturalRegistrationId;
                        objCulturalRegistrations.UpdatedBy = Username;
                        objCulturalRegistrations.PaymentDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        Int64 result = _Cultural.UpdateCulturalRegistrationsPaymentInfo(objCulturalRegistrations);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "CulturalRegistrations ThankYou Page",
                            LogDescription = "Updated Transaction details",
                            LogDate = DateTime.Now,
                            InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }
                    //var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                }
                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Cultural Registrations",
                        LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                        LogDate = DateTime.Now,
                        InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                }

                    int result1 = 0;
                List<Entities.CulturalParticipants> lstCulturalParticipants = new List<Entities.CulturalParticipants>();
                objCulturalRegistrations = _Cultural.GetCulturalRegistrationsFullDetailsById(CulturalRegistrationId, ref result1);

                if (PaymentMethod == "Stripe" || PaymentMethod == "Swire Pay")
                {
                    string Participants = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">First Name</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Last Name</span></th><th width=\"19%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Email</span></th></tr>";

                    if (lstCulturalParticipants != null && lstCulturalParticipants.Count != 0)
                    {
                        int i = 1;
                        foreach (var item in lstCulturalParticipants)
                        {
                            if ((item.FirstName != null && item.FirstName != "") || (item.LastName != null && item.LastName != "") || (item.Email != null && item.Email != ""))
                            {

                                Participants = Participants + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.FirstName + "</td><td align=\"center\">" + item.LastName + "</td><td align=\"center\">" + item.Email + "</td></tr>";
                            }
                            else
                            {
                                Participants = "<tr><td colspan='4' align='center'>No Participants Available</td></tr>";
                            }

                            i++;
                        }

                    }
                    Participants = Participants + "</table>";
                    //Send Mail to User
                    string BaseUrl = objAppInfo.UserSiteUrl;
                    int MailStatus = 0;
                    objCulturalRegistrations.PaymentStatus = "Completed";

                    Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();
                    objMailTemplates = _MailTemplates.GetMailTemplateById("Confirm Cultural Event Registration", 0, ref MailStatus);
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
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                        body.Replace("[Email]", objCulturalRegistrations.Email);
                        body.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                        body.Replace("[PaymentType]", PaymentMethod);
                        body.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                        body.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[Participants]", Participants);
                        body.Replace("[Year]", DateTime.Now.Year.ToString());
                        body.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                        body.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                        body.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                        body.Replace("[EventName]", objCulturalRegistrations.EventName);
                        body.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objCulturalRegistrations.Email, objAppInfo.CompanyEmail, objMailTemplates.Subject, body.ToString());
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "User Email Sent",
                                LogDescription = $"Confirmation email sent to user: {objCulturalRegistrations.Email}",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }
                    //Send Mail to Admin
                    int statusAdmin = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Cultural Event Registration", 0, ref statusAdmin);
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
                        body1.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objCulturalRegistrations.FirstName + " " + objCulturalRegistrations.LastName));
                        body1.Replace("[Email]", objCulturalRegistrations.Email);
                        body1.Replace("[TransactionId]", objCulturalRegistrations.TransactionId);
                        body1.Replace("[PaymentType]", PaymentMethod);
                        body1.Replace("[PaymentDate]", objCulturalRegistrations.PaymentDate.ToString("MM/dd/yyyy"));
                        body1.Replace("[AmountPaid]", objCulturalRegistrations.AmountPaid.ToString());
                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                        body1.Replace("[Participants]", Participants);
                        body1.Replace("[Year]", DateTime.Now.Year.ToString());
                        body1.Replace("[CoordinatorName]", objCulturalRegistrations.CoordinatorName);
                        body1.Replace("[NameofthePerformance]", objCulturalRegistrations.NameofthePerformance);
                        body1.Replace("[NumberofParticipants]", objCulturalRegistrations.NumberofParticipants);
                        body1.Replace("[EventName]", objCulturalRegistrations.EventName);
                        body1.Replace("[PaymentStatus]", objCulturalRegistrations.PaymentStatus);
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.EventsEmail, objAppInfo.CompanyEmail, objTemplatesAdmin.Subject, body1.ToString());

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Admin Email Sent",
                                LogDescription = $"Confirmation email sent to Admin: {objAppInfo.EventsEmail}",
                                LogDate = DateTime.Now,
                                InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }
                }

                    Int64 LRstatus = 0;

                ViewBag.objCulturalRegistrations = objCulturalRegistrations;

            }
            catch (Exception ex)
            {

                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "CulturalRegistrations",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objCulturalRegistrations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
            return View();
        }





        //[HttpPost]
        //public async Task<JsonResult> InsertPaymentAsync(Entities.CulturalRegistrations payment, List<Entities.CulturalParticipants> lstCulturalParticipants)
        //{
        //    Int64 LRstatus = 0;
        //    List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

        //    Entities.LogReport logs = new Entities.LogReport();
        //    //logreport("Donation Registration: Entered into Sports Registration - " + payment.FirstName + " - " + payment.Email, Request.Path);


        //    logs = new Entities.LogReport();
        //    logs.LogTitle = "CulturalRegistrations"; logs.LogDescription = "Cultural Registration: Entered into Cultural Registration"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
        //    lstLogReport.Add(logs);
        //    LRstatus = _Users.InsertLogReport(lstLogReport);

        //    Int64 _status = 0;
        //    //Int64 CulturalRegistrationId = 0;
        //    //Int64 MemberId = 0;
        //    int status2 = 0;
        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
        //    try
        //    {

        //        // Honeypot validation
        //        if (!string.IsNullOrWhiteSpace(payment.Honeypot))
        //        {
        //            // logreport("Donation Registration: Honeypot Bot activity detected - " + payment.FirstName + " - " + payment.Email, Request.Path);


        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Cultural"; logs.LogDescription = "Cultural Registration: Honeypot Bot activity detected -"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
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
        //            // logreport("Donation Registration: reCAPTCHA validation failed. Please try again - " + payment.FirstName + " - " + payment.Email, Request.Path);

        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Cultural"; logs.LogDescription = "Cultural Registration: reCAPTCHA validation failed. Please try again -"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
        //            lstLogReport.Add(logs);
        //            LRstatus = _Users.InsertLogReport(lstLogReport);



        //            ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");

        //            return Json(new { ok = false, data = "reCAPTCHA validation failed. Please try again." });
        //        }


        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        payment.InsertedBy = payment.FirstName;
        //        payment.UpdatedBy = payment.FirstName;
        //        string receipturl = "NA";



        //        Int64 CulturalRegistrationId = 0;
        //        payment.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //        payment.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();




        //        if (lstCulturalParticipants != null && lstCulturalParticipants.Count() != 0)
        //        {
        //            foreach (Entities.CulturalParticipants objCulturalParticipants in lstCulturalParticipants)
        //            {
        //                if (objCulturalParticipants != null && objCulturalParticipants.FirstName != null)
        //                {
        //                    objCulturalParticipants.CulturalRegistrationId = CulturalRegistrationId;
        //                    Int64 estatus = _Cultural.InsertCulturalParticipants(objCulturalParticipants);
        //                    if (estatus == -1)
        //                    {
        //                        TempData["message"] = "<div class=\"error1 closable\">Failed uploading Participant Details.</div>";
        //                        //return RedirectToAction("Index", "CulturalRegistrations");
        //                    }
        //                }
        //            }
        //        }


        //        _status = _Cultural.InsertCulturalRegistrations(payment, ref CulturalRegistrationId, ref receipturl);

        //        // _status = _ServiceDonations.InsertServiceDonations(payment, ref ServiceDonationId, ref receipturl);

        //        // logreport("Donation Registration: Inserted basic info Successfuly (" + payment.FirstName + " - " + payment.Email + ")", Request.Path);

        //        logs = new Entities.LogReport();
        //        logs.LogTitle = "Donations"; logs.LogDescription = "Donation Registration: Inserted basic info Successfuly"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
        //        lstLogReport.Add(logs);
        //        LRstatus = _Users.InsertLogReport(lstLogReport);


        //        if (CulturalRegistrationId != 0)
        //        {
        //            return Json(new { success = CulturalRegistrationId > 0, paymentId = CulturalRegistrationId });
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
        //    try
        //    {
        //        string folderPath = Server.MapPath("~/Content/Log/PayPalSaveLog/");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        string fileName = "paypal_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
        //        string fullPath = Path.Combine(folderPath, fileName);

        //        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
        //        System.IO.File.AppendAllText(fullPath, logEntry);

        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Fallback log in case main logging fails
        //        System.IO.File.AppendAllText(Server.MapPath("~/Content/Log/PayPalSaveLog/error_fallback.log"),
        //            $"{DateTime.Now} - Failed to log PayPal: {ex.Message}{Environment.NewLine}");

        //        return Json(new { success = false, error = ex.Message });
        //    }
        //}











        public ActionResult ThankYou()
        {
            return View();
        }
    }
}