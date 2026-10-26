using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.Entities;
using Braintree;
using DocumentFormat.OpenXml.EMMA;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json.Linq;
using PaymentProcess;
using PayPal.Api;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System;

using System.Collections.Specialized;

using System.Data;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using static AWS_AssociationToolCore.Areas.User.Controllers.MembersController;
using Session = Stripe.Checkout.Session;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]

    public class ServicesController : Controller
    {
        //test
        BLL.Services _Services = new BLL.Services();
        List<Entities.Services> lstServices = new List<Entities.Services>();
        List<Entities.ServiceCategories> lstServiceCategories = new List<Entities.ServiceCategories>();
        BLL.ServiceCategories _ServiceCategories = new BLL.ServiceCategories();
        BLL.Members _Members = new BLL.Members();
        Entities.Members _objMember = new Entities.Members();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();
        PayByPayPal objPayByPayPal = new PayByPayPal();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        BLL.ServiceDonations _ServiceDonations = new BLL.ServiceDonations();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        Entities.Services _objServices = new Entities.Services();
        List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();
        List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        Entities.ServiceContributions _ServiceContributions = new Entities.ServiceContributions();
        Entities.Members objMembers = new Entities.Members();
        BLL.Users _Users = new BLL.Users();
        BLL.SendMail _sentmail = new BLL.SendMail();
        private object _environment;

        private readonly IBraintreeConfiguration _config;
        public ServicesController(IBraintreeConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(
            long ServiceCategoryId = 0,
            long MemberId = 0,
            long ChapterId = 0,
            string Type = "",
            long ServiceId = 0,
            string cname = "",
            long ServiceDonationId = 0)
        {
            List<Entities.PaymentMethods> lstpaymentmethods = new List<Entities.PaymentMethods>();
            string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);

            try
            {
                // Cookie reading (Core way)
                if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
                {
                    cname = cookieValue;
                }

                int status = 0;
                int QStatus = 0;
                int _qstatus = 0;
                int qstatus = 0;

                if (!string.IsNullOrEmpty(cname))
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters.ChapterId;
                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }

                lstServices = _Services.FEServicesGetListByChapterId(ChapterId, ref status);
                lstpaymentmethods = _paymentSettings.GetPaymentMethodsList(ref status);

                // Session in Core (use string, int etc.)
                HttpContext.Session.SetInt32("ChapterId", (int)ChapterId);

                _objServices = _Services.FEGetServicesDataById(ServiceId, ref lstServiceImages, ref status);
                string currentUserEmail = "";
                currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";

                // Auth check in Core
                // ✅ Fixed - clean and correct
                if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(currentUserEmail))
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(currentUserEmail, ref _qstatus);
                }
                HttpContext.Session.SetInt32("MemberId", (int)_objMember.MemberId);

                // Cookie writing (Core way)
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps // IsSecureConnection replaced by IsHttps
                };
                Response.Cookies.Append("FormValidationCode", generatedCode, cookieOptions);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.Memberdetails = _objMember;
            ViewBag.ServiceCategoryId = ServiceCategoryId;
            ViewBag.ChapterId = ChapterId;
            ViewBag.Type = Type;
            ViewBag.ServiceId = ServiceId;
            ViewBag.MemberId = _objMember.MemberId;
            ViewBag.objServices = _objServices;
            ViewBag.lstServiceImages = lstServiceImages;
            ViewBag.lstServices = lstServices;
            ViewBag.cname = cname;
            ViewBag.ServiceDonationId = ServiceDonationId;
            ViewBag.lstpaymentmethods = lstpaymentmethods;
            ViewBag.ValidationCode = generatedCode;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceDonation(IFormFile file1,IFormFile filezelle,Entities.ServiceDonations objServiceDonations,List<Entities.ServiceContributions> lstServiceContributions,string cname = "",long ServiceDonationId = 0,string recaptchaToken = "",string Honeypot = "",string ValidationCode = "",long DMemberId = 0)
        {
            IBraintreeGateway gateway = _config.GetGateway();

            long LRstatus = 0;
            Entities.ServiceDonations objServiceDonationss = new Entities.ServiceDonations();
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
            Entities.LogReport logs = new Entities.LogReport();

            // ✅ FIX 1: Core way to get IP Address
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            objServiceDonations.Field2 = ip;
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Service Donation",
                LogDescription = $"Entered into Service Donation Process",
                LogDate = DateTime.Now,
                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objServiceDonations.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
            };
            _Users.LogReportInsert(objLogReport, ref LogId);

            try
            {
                int status2 = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donations",
                        LogDescription = "Honeypot Bot activity detected",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("Index", "Services");
                }

                if (Request.Form["ClientValidationStatus"] == "true")
                    ModelState.Clear();
                var capcha = objAppInfo.ShowCapcha;

                // ✅ FIX 2: Core way to get form value and host
                string captchaResponse = Request.Cookies["g-recaptcha-response"];
                //string captchaResponse = Request.Form["g-recaptcha-response"];
                string clientHost = Request.Host.Host;

                bool isHuman = await BLL.Common.ValidateCaptchaAsync(captchaResponse, clientHost);

                if (capcha == "No")
                {
                    isHuman = true;  // Skip CAPTCHA check
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
                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()


                };


                _Users.LogSubReportInsert(LogSubReport);
                // ✅ FIX 3: Core cookie reading
                string cookieValidationCode = Request.Cookies["FormValidationCode"];
                if (string.IsNullOrEmpty(cookieValidationCode))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donations",
                        LogDescription = "Failed Retrieve the code from the cookie.",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
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
                        LogTitle = "Donations",
                        LogDescription = "Failed Security validation failed..",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("Index", "Services");
                }

                if (isHuman = true)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donations",
                        LogDescription = "After reCAPTCHA Entered to the Process",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    if (objServiceDonations.PaymentMethod == "PayPal")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donations",
                            LogDescription = "Client side PayPal Payment Process is Completed(" +
                          DateTime.UtcNow + ")&PaymentStatus:" + objServiceDonations.PaymentStatus +
                            "&TransactionId:" + objServiceDonations.TransactionId,
                            LogDate = DateTime.Now,
                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    }

                    decimal Percentage = 0;
                    decimal IndAmount = objServiceDonations.Amount;
                    decimal TotalAmount = objServiceDonations.Amount;
                    objServiceDonations.UpdatedBy = objServiceDonations.FirstName;

                    if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                    {
                      
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donations",
                            LogDescription = "Contributions Amount Splitting",
                            LogDate = DateTime.Now,
                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        long CCount = lstServiceContributions.Count(x => x.ServiceId != 0);
                        if (CCount != 0)
                        {
                            objServiceDonations.Amount = 0;
                        }
                    }

                    string imageurl = "NA";
                    string receipturl = "NA";

                    string SanitizeFileName(string fileName)
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName);
                        string ext = Path.GetExtension(fileName).ToLower();
                        name = name.Replace(" ", "_")
                                   .Replace("(", "")
                                   .Replace(")", "")
                                   .Replace(".", "_")  // ✅ dots in name → underscore
                                   .Trim('_');
                        return name + ext;
                    }

                    // ✅ Sanitize and save to temp
                    if (file1 != null && file1.Length > 0)
                    {
                        imageurl = SanitizeFileName(file1.FileName);
                        receipturl = imageurl;
                        string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", imageurl);
                        Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                        await using var stream1 = new FileStream(tempPath, FileMode.Create);
                        await file1.CopyToAsync(stream1);
                    }

                    if (filezelle != null && filezelle.Length > 0)
                    {
                        receipturl = SanitizeFileName(filezelle.FileName);
                        string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", receipturl);
                        Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                        await using var stream2 = new FileStream(tempPath, FileMode.Create);
                        await filezelle.CopyToAsync(stream2);
                    }

                    // ✅ Save original names BEFORE SP changes them
                    string originalImageUrl = imageurl;   // "no-image.png"
                    string originalReceiptUrl = receipturl; // "no-image.png" or zelle filename


                    objServiceDonations.MemberId = DMemberId;
                    objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    long status1 = _ServiceDonations.InsertServiceDonations(objServiceDonations, ref ServiceDonationId, ref receipturl);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation Insert",
                        LogDescription = status1 == 1
                                                    ? $"Donation inserted successfully. ServiceDonationId: {ServiceDonationId}"
                                                    : $"Donation insert failed. Status: {status1}",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    if (ServiceDonationId != 0)
                    {
                      

                        // ✅ file1: tempSrc=original name, dest=SP generated name
                        if (file1 != null && file1.Length > 0
                            && !string.IsNullOrEmpty(receipturl) && receipturl != "NA")
                        {
                            // ✅ Move temp files to final folder
                            string finalFolder = Path.Combine(objAppInfo.UploadPath, "ServiceDonationReceiptImages");
                            Directory.CreateDirectory(finalFolder);


                            string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalImageUrl);
                            string dest = Path.Combine(finalFolder, receipturl); // SP name
                            if (System.IO.File.Exists(tempSrc))
                                System.IO.File.Move(tempSrc, dest, overwrite: true);
                        }

                        // ✅ filezelle: tempSrc=original zelle name, dest=SP generated name  
                        if (filezelle != null && filezelle.Length > 0
                            && !string.IsNullOrEmpty(receipturl) && receipturl != "NA")
                        {
                            // ✅ Move temp files to final folder
                            string finalFolder = Path.Combine(objAppInfo.UploadPath, "ServiceDonationReceiptImages");
                            Directory.CreateDirectory(finalFolder);

                            string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalReceiptUrl);
                            string dest = Path.Combine(finalFolder, receipturl); // SP name
                            if (System.IO.File.Exists(tempSrc))
                                System.IO.File.Move(tempSrc, dest, overwrite: true);
                        }
                        int status = 0;

                        // ✅ Contributions processing
                        if (lstServiceContributions != null && lstServiceContributions.Count() != 0)
                        {
                            long CCount1 = lstServiceContributions.Count(x => x.ServiceId != 0);
                            long gencount = lstServiceContributions.Count(x => x.ServiceId == 10);
                            decimal contriAmount = 0;
                            decimal percent = 0;

                            if (CCount1 != 0)
                            {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Donations",
                                    LogDescription = "Entered Into Contributions Insertion Process",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                             
                                if (gencount != 0)
                                {
                                    var contributions_list = lstServiceContributions.FindAll(x => x.ServiceId != 0 && x.ServiceId != 10);

                                    foreach (Entities.ServiceContributions objServiceContributions in contributions_list)
                                    {
                                        IndAmount = (TotalAmount / 100) * Convert.ToDecimal(objServiceContributions.Percentage);
                                        objServiceContributions.CAmount = IndAmount;
                                        objServiceContributions.ServiceDonationId = ServiceDonationId;
                                        contriAmount = contriAmount + IndAmount;
                                        percent = percent + Convert.ToDecimal(objServiceContributions.Percentage);
                                        long estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);


                                        if (estatus == -1)
                                        {
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Donations",
                                                LogDescription = "Failed Multiple Contributions Inserted Successfully",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                            TempData["message"] = "<div class=\"error1 closable\">Failed inserting member details.</div>";
                                            return RedirectToAction("Acknowledgement", "Services");
                                        }
                                    }

                                    Entities.ServiceContributions objGenContributions = new Entities.ServiceContributions();
                                    IndAmount = TotalAmount - contriAmount;
                                    objGenContributions.Percentage = (100 - percent).ToString();
                                    objGenContributions.CAmount = IndAmount;
                                    objGenContributions.ServiceDonationId = ServiceDonationId;
                                    objGenContributions.ServiceId = 10;
                                    long estatus1 = _ServiceDonations.InsertServiceContributions(objGenContributions);

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Donations",
                                        LogDescription = "Contributions Inserted Successfully",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                               
                                }
                                else
                                {
                                    int z = 1;
                                    var contributions_list = lstServiceContributions.FindAll(x => x.ServiceId != 0);

                                    foreach (Entities.ServiceContributions objServiceContributions in contributions_list)
                                    {
                                        long estatus = 0;
                                        if (z != lstServiceContributions.Count(x => x.ServiceId != 0))
                                        {
                                            IndAmount = (TotalAmount / 100) * Convert.ToDecimal(objServiceContributions.Percentage);
                                            objServiceContributions.CAmount = IndAmount;
                                            objServiceContributions.ServiceDonationId = ServiceDonationId;
                                            contriAmount = contriAmount + IndAmount;
                                            percent = percent + Convert.ToDecimal(objServiceContributions.Percentage);
                                            estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);
                                        }
                                        else
                                        {
                                            IndAmount = (TotalAmount - contriAmount);
                                            objServiceContributions.CAmount = IndAmount;
                                            objServiceContributions.ServiceDonationId = ServiceDonationId;
                                            objServiceContributions.Percentage = (100 - percent).ToString();
                                            estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Donations",
                                                LogDescription = "Contributions Inserted Successfully",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }

                                        if (estatus == -1)
                                        {
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Donations",
                                                LogDescription = "Failed Inserting Contributions details..!!",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                         
                                            TempData["message"] = "<div class=\"error1 closable\">Failed inserting member details.</div>";
                                            return RedirectToAction("Acknowledgement", "Services");
                                        }

                                        z++;
                                    }
                                }
                            }
                        }
                        if (objServiceDonations.PaymentMethod == "Square")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = "Square Payment started",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
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
                            long amountCents = (long)(objServiceDonations.Amount * 100);
                            var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();
                            var createPaymentBody = new Square.Models.CreatePaymentRequest.Builder(squareNonce, Guid.NewGuid().ToString(), money)
                                .LocationId(squareLocationId)
                                .Note($"Service Registration - {objServiceDonations.DonationCause} | {objServiceDonations.FirstName} {objServiceDonations.LastName}")
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
                                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    string squareTransactionId = squareResponse.Payment.Id;

                                    // ✅ Session fix
                                    HttpContext.Session.SetString("CTotalAmount", objServiceDonations.Amount.ToString());
                                

                                    string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                                        ? objMembers.FirstName + " " + objMembers.LastName
                                        : User.Identity?.Name ?? "";

                            
                                    objServiceDonations.Amount = objServiceDonations.Amount;
                                    objServiceDonations.PaymentMethod = "Square";
                                    objServiceDonations.TransactionId = squareTransactionId;
                                    objServiceDonations.PaymentStatus = "Completed";
                                    objServiceDonations.ServiceDonationId = ServiceDonationId;
                                    objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objServiceDonations.UpdatedBy = Username;
                                    int test = 0;
                                    Int64 result5 = _ServiceDonations.UpdateServiceDonations(objServiceDonations);

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Donations",
                                        LogDescription = "Updated Service Donations Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    objServiceDonationss = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                                    if (result5 == 1)
                                    {

                                          int _status = 0;
                                           Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                                      if (objTemplates != null)
                                      {
                                        StringBuilder body = new StringBuilder();
                                        body.Append(objTemplates.Description);
                                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                        body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonationss.FirstName + " " + objServiceDonationss.LastName));
                                        body.Replace("[FirstName]", objServiceDonationss.FirstName);
                                        body.Replace("[LastName]", objServiceDonationss.LastName);
                                        body.Replace("[Email]", objServiceDonationss.Email);
                                        body.Replace("[PhoneNo]", objServiceDonationss.PhoneNo);
                                        body.Replace("[Address]", (objServiceDonationss.Address == "" ? "N/A" : objServiceDonationss.Address));
                                        body.Replace("[DonationCause]", objServiceDonationss.DonationCauses);
                                        body.Replace("[DonationAmount]", TotalAmount.ToString());
                                            body.Replace("[TransactionId]", objServiceDonationss.TransactionId);
                                        body.Replace("[PaymentStatus]", objServiceDonationss.PaymentStatus);
                                        body.Replace("[PaymentType]", "Square");
                                        body.Replace("[PaymentDate]", objServiceDonationss.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplates.Subject;
                                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevo(objServiceDonationss.Email, subject, body.ToString());

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "User Email Sent",
                                                LogDescription = $"Confirmation email sent to user: {objServiceDonationss.Email}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }

                                          Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                                         if (objTemplates1 != null)
                                    {
                                        StringBuilder body1 = new StringBuilder();
                                        body1.Append(objTemplates1.Description);
                                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                        body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonationss.FirstName + " " + objServiceDonationss.LastName));
                                        body1.Replace("[FirstName]", objServiceDonationss.FirstName);
                                        body1.Replace("[LastName]", objServiceDonationss.LastName);
                                        body1.Replace("[Email]", objServiceDonationss.Email);
                                        body1.Replace("[PhoneNo]", objServiceDonationss.PhoneNo);
                                        body1.Replace("[Address]", (objServiceDonationss.Address == "" ? "N/A" : objServiceDonationss.Address));
                                        body1.Replace("[DonationCause]", objServiceDonationss.DonationCauses);
                                        body1.Replace("[DonationProgram]", objServiceDonationss.DonationCauses);
                                        body1.Replace("[DonationAmount]", TotalAmount.ToString());
                                            body1.Replace("[TransactionId]", objServiceDonationss.TransactionId);
                                        body1.Replace("[PaymentStatus]", objServiceDonationss.PaymentStatus);
                                        body1.Replace("[PaymentType]", "Square");
                                        body1.Replace("[PaymentDate]", objServiceDonationss.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplates.Subject;
                                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.DonationEmail, subject, body1.ToString());
                                            // ✅ Check if email was successful or failed
                                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                                LogDescription = isEmailSuccess
                                                    ? $"Confirmation email sent successfully to Admin: {objAppInfo.DonationEmail}"
                                                    : $"Confirmation email FAILED for Admin: {objAppInfo.DonationEmail} | Response: {responsce}",

                                                LogDate = DateTime.Now,
                                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }


                                        return RedirectToAction("Acknowledgement", "Services", new { ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonationss.PaymentMethod });
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
                                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    return RedirectToAction("PaymentFailed", "Home");
                                }
                            }
                            catch (Exception ex)
                            {
                                StringBuilder body1 = new StringBuilder();
                                body1.Append("<p>Dear Admin, <br /><br />For your convenience, quick reminder email that payment for exhibit Registration has been failed. <br />");
                                body1.Append("<b>User Name:</b> " + objMembers.FirstName + " " + objMembers.LastName + "<br />");
                                body1.Append("<b>Email:</b> " + objMembers.Email + "<br />");
                                body1.Append("<b>Payment Status:</b> Pending <br />");
                                body1.Append("<b>Paypal Response:</b> " + ex.Message + "<br /><br />");
                                body1.Append("Thank You,<br />Admin</p>");
                                string tomail = objsettings.PaymentPassword;

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "squre Payment Failed",
                                    LogDescription = "squre Payment Failed" + ex.Message,
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });


                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }

                        else if (objServiceDonations.PaymentMethod == "Credit Card")
                        {
                            try
                            {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Credit Card Payment",
                                    LogDescription = "Credit Card Payment started",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                // ✅ 1. Get Amount safely
                                decimal amount = objServiceDonations.Amount;

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
                                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    var transaction = result.Target;
                                    long result1 = 0;
                                    int test = 0;
                                    objServiceDonations.TransactionId = transaction.Id;
                                    objServiceDonations.PaymentStatus = "Completed";
                                    objServiceDonations.PaymentMethod = "Credit Card";
                                    objServiceDonations.UpdatedBy = $"{objServiceDonations.FirstName} {objServiceDonations.LastName}";
                                    objServiceDonations.Amount = amount;
                                    objServiceDonations.ServiceDonationId = ServiceDonationId;
                                    objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                    result1 = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Credit Card Payment",
                                        LogDescription = "Credit Card MemberOrder Inserted ",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    if (result1 == 1)
                                    {
                                        objServiceDonationss = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                                        int _status = 0;
                                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                                        if (objTemplates != null)
                                        {
                                            StringBuilder body = new StringBuilder();
                                            body.Append(objTemplates.Description);
                                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                            body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                            body.Replace("[FirstName]", objServiceDonations.FirstName);
                                            body.Replace("[LastName]", objServiceDonations.LastName);
                                            body.Replace("[Email]", objServiceDonations.Email);
                                            body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                            body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                                            body.Replace("[DonationCause]", objServiceDonations.DonationCauses);

                                            body.Replace("[DonationAmount]", TotalAmount.ToString());
                                            body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                                            body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                            body.Replace("[PaymentType]", "Credit Card");
                                            body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                            body.Replace("[SiteName]", objAppInfo.SiteName);
                                            body.Replace("[ApplicationName]", objAppInfo.SiteName);

                                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                                            string subject = objTemplates.Subject;
                                            subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                            if (objAppInfo.Email == "Yes")
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.CompanyEmail, subject, body.ToString());

                                            }
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "User Email Sent",
                                                LogDescription = $"Confirmation email sent to user: {objMembers.Email}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                                        if (objTemplates1 != null)
                                        {
                                            StringBuilder body1 = new StringBuilder();
                                            body1.Append(objTemplates1.Description);
                                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                            body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                            body1.Replace("[FirstName]", objServiceDonations.FirstName);
                                            body1.Replace("[LastName]", objServiceDonations.LastName);
                                            body1.Replace("[Email]", objServiceDonations.Email);
                                            body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                            body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                                            body1.Replace("[DonationProgram]", objServiceDonations.DonationCause);
                                            body1.Replace("[DonationCause]", objServiceDonations.DonationCauses);
                                            body1.Replace("[DonationAmount]", TotalAmount.ToString());
                                            body1.Replace("[TransactionId]", objServiceDonations.TransactionId);
                                            body1.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                            body1.Replace("[PaymentType]", "Credit Card");
                                            body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                                            body1.Replace("[ApplicationName]", objAppInfo.SiteName);

                                            body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                            string subject = objTemplates.Subject;
                                            subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                            if (objAppInfo.Email == "Yes")
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.DonationEmail, objAppInfo.DonationEmail, subject, body1.ToString());

                                            }
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Admin Email Sent",
                                                LogDescription = $"Confirmation email sent to Admin: {objAppInfo.MemberEmail}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }



                                        return RedirectToAction("Acknowledgement", "Services", new { ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonationss.PaymentMethod });
                                    }
                                    else
                                    {
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Payment Failed ",
                                            LogDescription = "credit card Payment Deatails Inserting Failed",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                        string errors = string.Join("\n",
                                        result.Errors.DeepAll().Select(e => $"Error {(int)e.Code}: {e.Message}"));

                                        TempData["message"] = errors;

                                        return RedirectToAction("PaymentFailed", "Home");
                                    }

                                }

                                // ❌ Validation errors
                                else
                                {
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Payment Failed ",
                                        LogDescription = "credit card Deatils Invalid ",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
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
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                TempData["message"] = "Payment error: " + ex.Message;
                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }
                        // ==================== PayPal ====================
                        else if (objServiceDonations.PaymentMethod == "PayPal")
                        {
                            decimal paymentAmount = objServiceDonations.PaymentAmount;


                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "PayPal Payment",
                                LogDescription = "Payment PayPal PaymentMethod Entering",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            string Username = "";
                            if (objServiceDonations.FirstName != null)
                            {
                                Username = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
                            }
                            else
                            {
                                // ✅ FIX 5: Core Identity Name
                                Username = User.Identity?.Name ?? "";
                            }

                            if (!string.IsNullOrEmpty(Username))
                            {
                                long result = 0;
                                int test = 0;

                                objServiceDonations.Amount = Convert.ToDecimal(paymentAmount);
                                objServiceDonations.PaymentMethod = "PayPal";
                                objServiceDonations.TransactionId = objServiceDonations.TransactionId;
                                objServiceDonations.PaymentStatus = "Completed";
                                objServiceDonations.ServiceDonationId = ServiceDonationId;
                                objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objServiceDonations.UpdatedBy = Username;

                                result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                                objServiceDonationss = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Donations",
                                    LogDescription = "Updated Service Donations Details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                            }

                            int _status = 0;
                            Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                            if (objTemplates != null)
                            {
                                StringBuilder body = new StringBuilder();
                                body.Append(objTemplates.Description);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonationss.FirstName + " " + objServiceDonationss.LastName));
                                body.Replace("[FirstName]", objServiceDonationss.FirstName);
                                body.Replace("[LastName]", objServiceDonationss.LastName);
                                body.Replace("[Email]", objServiceDonationss.Email);
                                body.Replace("[PhoneNo]", objServiceDonationss.PhoneNo);
                                body.Replace("[Address]", (objServiceDonationss.Address == "" ? "N/A" : objServiceDonationss.Address));
                                body.Replace("[DonationCause]", objServiceDonationss.DonationCauses);
                                body.Replace("[DonationAmount]", paymentAmount.ToString());
                                body.Replace("[TransactionId]", objServiceDonationss.TransactionId);
                                body.Replace("[PaymentStatus]", objServiceDonationss.PaymentStatus);
                                body.Replace("[PaymentType]", "PayPal");
                                body.Replace("[PaymentDate]", objServiceDonationss.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevo(objServiceDonationss.Email, subject, body.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objServiceDonationss.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                            if (objTemplates1 != null)
                            {
                                StringBuilder body1 = new StringBuilder();
                                body1.Append(objTemplates1.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonationss.FirstName + " " + objServiceDonationss.LastName));
                                body1.Replace("[FirstName]", objServiceDonationss.FirstName);
                                body1.Replace("[LastName]", objServiceDonationss.LastName);
                                body1.Replace("[Email]", objServiceDonationss.Email);
                                body1.Replace("[PhoneNo]", objServiceDonationss.PhoneNo);
                                body1.Replace("[Address]", (objServiceDonationss.Address == "" ? "N/A" : objServiceDonationss.Address));
                                body1.Replace("[DonationCause]", objServiceDonationss.DonationCauses);
                                body1.Replace("[DonationProgram]", objServiceDonationss.DonationCauses);
                                body1.Replace("[DonationAmount]", paymentAmount.ToString());
                                body1.Replace("[TransactionId]", objServiceDonationss.TransactionId);
                                body1.Replace("[PaymentStatus]", objServiceDonationss.PaymentStatus);
                                body1.Replace("[PaymentType]", "PayPal");
                                body1.Replace("[PaymentDate]", objServiceDonationss.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.DonationEmail, subject, body1.ToString());
                                    // ✅ Check if email was successful or failed
                                    bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                        LogDescription = isEmailSuccess
                                            ? $"Confirmation email sent successfully to Admin: {objAppInfo.DonationEmail}"
                                            : $"Confirmation email FAILED for Admin: {objAppInfo.DonationEmail} | Response: {responsce}",

                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            var redirectUrl = Url.Action("Acknowledgement", "Services", new
                            {
                                area = "User",
                                ServiceDonationId = ServiceDonationId,
                                cname = cname,
                                PaymentMethod = objServiceDonations.PaymentMethod,
                                tid = objServiceDonations.TransactionId
                            });
                            return Json(new { success = true, redirectUrl = redirectUrl });
                            //return RedirectToAction("Acknowledgement", "Services", new { ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonations.PaymentMethod, tid = objServiceDonations.TransactionId });
                        }

                        // ==================== Cash/Cheque ====================
                        else if (objServiceDonations.PaymentMethod == "Cash/Cheque")
                        {  // ── Step 5: Log Admin Email Sent ──
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "PaymentMethod Cash/Cheque",
                                LogDescription = $"Entered into Cash/Cheque Donation Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            int test = 0;


                            // ✅ FIX 6: Core Session
                            HttpContext.Session.SetString("CTotalAmount", TotalAmount.ToString());

                            objServiceDonations.Amount = IndAmount;
                            objServiceDonations.PaymentMethod = "Cash/Cheque";
                            objServiceDonations.TransactionId = "N/A";
                            objServiceDonations.PaymentStatus = "Pending";
                            objServiceDonations.ServiceDonationId = ServiceDonationId;
                            objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;

                            long result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                            objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "PaymentMethod Cash/Cheque Entered",
                                LogDescription = $"Updated Cash/Cheque Service Donations",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });


                            int _qstatus = 0;
                            string contributions = "";
                            lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);

                            if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                            {
                                contributions = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";
                                int i = 1;
                                foreach (var item in lstServiceContributions)
                                {
                                    contributions = contributions + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.ServiceTitle + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                                    i++;
                                }
                                contributions = contributions + "</table>";
                            }

                            int _status = 0;
                            Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                            if (objTemplates != null)
                            {
                                StringBuilder body = new StringBuilder();
                                body.Append(objTemplates.Description);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                body.Replace("[FirstName]", objServiceDonations.FirstName);
                                body.Replace("[LastName]", objServiceDonations.LastName);
                                body.Replace("[Email]", objServiceDonations.Email);
                                body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                                body.Replace("[DonationCause]", objServiceDonations.DonationCauses);
                                body.Replace("[DonationAmount]", TotalAmount.ToString());
                                body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                                body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                body.Replace("[PaymentType]", "Cash/Cheque");
                                body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body.Replace("[Contributions]", contributions);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.DonationEmail, subject, body.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objServiceDonations.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                            if (objTemplates1 != null)
                            {
                                StringBuilder body1 = new StringBuilder();
                                body1.Append(objTemplates1.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                body1.Replace("[FirstName]", objServiceDonations.FirstName);
                                body1.Replace("[LastName]", objServiceDonations.LastName);
                                body1.Replace("[Email]", objServiceDonations.Email);
                                body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                                body1.Replace("[DonationCause]", objServiceDonations.DonationCauses);
                                body1.Replace("[DonationProgram]", objServiceDonations.DonationCauses);
                                body1.Replace("[DonationAmount]", TotalAmount.ToString());
                                body1.Replace("[TransactionId]", objServiceDonations.TransactionId);
                                body1.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                body1.Replace("[PaymentType]", "Cash/Cheque");
                                body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body1.Replace("[Contributions]", contributions);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.DonationEmail, objAppInfo.DonationEmail, subject, body1.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Confirmation email sent to Admin: {objAppInfo.DonationEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            return RedirectToAction("Acknowledgement", "Services", new { area = "User", ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonations.PaymentMethod, tid = objServiceDonations.TransactionId });
                        }

                        // ==================== Zelle ====================
                        else if (objServiceDonations.PaymentMethod == "Zelle")
                        {
                            int test = 0;

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Payment Zelle",
                                LogDescription = $"Entered Into Zelle Donation Proces",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            // ✅ FIX 7: Core Session
                            HttpContext.Session.SetString("CTotalAmount", TotalAmount.ToString());

                            objServiceDonations.Amount = IndAmount;
                            objServiceDonations.PaymentMethod = "Zelle";
                            objServiceDonations.TransactionId = objServiceDonations.TransactionId;
                            objServiceDonations.PaymentStatus = "Pending";
                            objServiceDonations.ServiceDonationId = ServiceDonationId;
                            objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
                            objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                            long result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                            objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Donations",
                                LogDescription = $"Updated Service Donations Details",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            int _qstatus = 0;
                            string contributions = "";
                            lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Donations",
                                LogDescription = $"Get Service Donations Details info By Id",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });






                            if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                            {
                                contributions = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";
                                int i = 1;
                                foreach (var item in lstServiceContributions)
                                {
                                    contributions = contributions + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.ServiceTitle + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                                    i++;
                                }
                                contributions = contributions + "</table>";
                            }

                            int _status = 0;
                            Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                            if (objTemplates != null)
                            {
                                StringBuilder body = new StringBuilder();
                                body.Append(objTemplates.Description);
                                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                body.Replace("[FirstName]", objServiceDonations.FirstName);
                                body.Replace("[LastName]", objServiceDonations.LastName);
                                body.Replace("[Email]", objServiceDonations.Email);
                                body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                                body.Replace("[DonationCause]", objServiceDonations.DonationCauses);
                                body.Replace("[DonationAmount]", TotalAmount.ToString());
                                body.Replace("[TransactionId]", "N/A");
                                body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                body.Replace("[PaymentType]", "Zelle");
                                body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body.Replace("[SiteName]", objAppInfo.SiteName);
                                body.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body.Replace("[Contributions]", contributions);
                                body.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.DonationEmail, subject, body.ToString());
                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "User Email Sent",
                                        LogDescription = $"Confirmation email sent to user: {objServiceDonations.Email}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                            if (objTemplates1 != null)
                            {
                                StringBuilder body1 = new StringBuilder();
                                body1.Append(objTemplates1.Description);
                                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                                body1.Replace("[FirstName]", objServiceDonations.FirstName);
                                body1.Replace("[LastName]", objServiceDonations.LastName);
                                body1.Replace("[Email]", objServiceDonations.Email);
                                body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                                body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                                body1.Replace("[DonationCause]", objServiceDonations.DonationCauses);
                                body1.Replace("[DonationProgram]", objServiceDonations.DonationCause);
                                body1.Replace("[DonationAmount]", TotalAmount.ToString());
                                body1.Replace("[TransactionId]", "N/A");
                                body1.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                                body1.Replace("[PaymentType]", "Zelle");
                                body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                body1.Replace("[SiteName]", objAppInfo.SiteName);
                                body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                                body1.Replace("[Contributions]", contributions);
                                body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                string subject = objTemplates.Subject;
                                subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                if (objAppInfo.Email == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.DonationEmail, objAppInfo.DonationEmail, subject, body1.ToString());

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Admin Email Sent",
                                        LogDescription = $"Admin notification email sent to: {objAppInfo.DonationEmail}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            return RedirectToAction("Acknowledgement", "Services", new { area = "User", ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonations.PaymentMethod, tid = objServiceDonations.TransactionId });
                        }

                        // ==================== Stripe ====================
                        else if (objServiceDonations.PaymentMethod == "Stripe")
                        {

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Payment Stripe ",
                                LogDescription = $"Entered into Stripe Donation Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            objServiceDonations.Amount = IndAmount;
                            objServiceDonations.PaymentMethod = "Stripe";
                            objServiceDonations.TransactionId = objServiceDonations.TransactionId;
                            objServiceDonations.PaymentStatus = "Pending";
                            objServiceDonations.ServiceDonationId = ServiceDonationId;
                            objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                            objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;

                            long result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "MembershipOrders Stripe updated",
                                LogDescription = $"updated  Stripe MembershipOrders",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);

                            StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;
                            string currency = "usd";

                            var priceOptions = new PriceCreateOptions
                            {
                                UnitAmount = (long)(IndAmount * 100),
                                Currency = currency.ToLower(),
                                ProductData = new PriceProductDataOptions { Name = "Sample Product" }
                            };
                            var priceService = new PriceService();
                            var price = priceService.Create(priceOptions);

                            // ✅ FIX 8: Core URL scheme (replaces Request.Url.Scheme)
                            string scheme = Request.Scheme;

                            var sessionOptions = new SessionCreateOptions
                            {
                                PaymentMethodTypes = new List<string> { "card" },
                                LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
                        },
                                Mode = "payment",
                                SuccessUrl = $"{objAppInfo.BaseUrl}User/Services/Acknowledgement?session_id={{CHECKOUT_SESSION_ID}}&ServiceDonationId={ServiceDonationId}&PaymentMethod=Stripe&cname={cname}",
                                // ✅ FIX 9: Core Url.Action with scheme
                                CancelUrl = Url.Action("Acknowledgement", "Services", null, scheme)
                            };

                            var sessionService = new SessionService();
                            var session = sessionService.Create(sessionOptions);

                            // ✅ FIX 10: Core cookie append (replaces Response.Cookies.Add(new HttpCookie()))
                            Response.Cookies.Append("StripeSessionId", session.Id, new CookieOptions { HttpOnly = true });

                            if (result == 1)
                            {
                                if (!string.IsNullOrEmpty(session.Url))
                                {
                                    return Redirect(session.Url);
                                }
                            }
                        }

                        // ==================== Swire Pay ====================
                        else if (objServiceDonations.PaymentMethod == "Swire Pay")
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Payment Swire Pay ",
                                LogDescription = $"Entered into Swire Pay Donation Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });


                            var client1 = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");



                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "PaymentMethod Swire Pay Entered",
                                LogDescription = $"Entered into Card Payment Process",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });



                            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);
                            request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);

                            // ✅ FIX 11: Core protocol (replaces Request.Url.Scheme)
                            var redirectUri = Url.Action("Acknowledgement", "Services", new
                            {
                                area = "User",
                                ServiceDonationId = ServiceDonationId,
                                PaymentMethod = "Swire Pay",
                                cname = cname
                            }, protocol: Request.Scheme);

                            decimal amountInUSD = IndAmount * 100;

                            var jsonContent = $@"
                    {{
                        ""amount"": ""{amountInUSD}"",
                        ""currencyCode"": ""USD"",
                        ""paymentMethodType"": [ ""CARD"" ],
                        ""redirectUri"": ""{redirectUri}"",
                        ""description"": ""Service Donation payment for {objServiceDonations.FirstName}"",
                        ""statementDescriptor"": ""Donation Payment"",
                        ""expiresAt"": ""{DateTime.UtcNow.AddDays(7):yyyy-MM-ddTHH:mm:ss.fffZ}"",
                        ""email"": ""{objServiceDonations.Email}"",
                        ""phone"": ""{objServiceDonations.PhoneNo}"",
                        ""name"": ""{objServiceDonations.FirstName}""
                    }}";

                            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                            var response = await client1.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var responseBody = await response.Content.ReadAsStringAsync();
                            var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                            var paymentLink = jsonData["entity"]?["link"]?.ToString();




                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Donations",
                                LogDescription = paymentLink,
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            if (!string.IsNullOrEmpty(paymentLink))
                            {
                                return Redirect(paymentLink);
                            }
                            else
                            {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Donations",
                                    LogDescription = "Swire Pay Payment link not found in the response.",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });


                                return RedirectToAction("Acknowledgement", "Services", new { area = "User", ServiceDonationId = ServiceDonationId, cname = cname, PaymentMethod = objServiceDonations.PaymentMethod, tid = objServiceDonations.TransactionId });
                            }
                        }
                    }
                }
                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Captcha Failed - Redirect",
                        LogDescription = "User redirected back due to captcha failure.",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    TempData["message"] = "<div class=\"error1 closable\">Google reCaptcha not validated Successfully</div>";
                    return RedirectToAction("Index", "Services");
                }
            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Donations",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objServiceDonationss.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objServiceDonationss.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
                return RedirectToAction("Index", "Services");
            }

            return View();
        }


        // Acknowledgement for PayPal
        public async Task<ActionResult> Acknowledgement(Int64 ServiceDonationId = 0, string PaymentMethod = "PayPal", string cname = "", string tid = "", string pstatus = "", string session_id = "", string spPaymentLink = "")
        {
            Int64 LRstatus = 0;
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
            Int32 test1 = 0;


            Entities.LogReport logs = new Entities.LogReport();
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            List<Entities.ServiceContributions> lstServiceContributions = new List<Entities.ServiceContributions>();
            objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test1);

            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Service Donation",
                LogDescription = "Donation Acknowledgement Page Entering",
                LogDate = DateTime.Now,
                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objServiceDonations.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);

            int _qstatus = 0;
            try
            {
                
                int status = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);
               
               
                 if (PaymentMethod == "Stripe")
                {
                    int status2 = 0;
                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation Acknowledgement Stripe PaymentMethod Entering",
                        LogDescription = "Donation Acknowledgement Stripe PaymentMethod  getting payment details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;

                    if (string.IsNullOrEmpty(session_id))
                    {
                        // Retrieve from Cookies if missing in query string
                        if (Request.Cookies["StripeSessionId"] != null)
                        {
                            session_id = Request.Cookies["StripeSessionId"]; // ✅ No .Value needed
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

                    Int32 test = 0;


                    objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                    var sessionServiceInstance = new SessionService();
                    var session = sessionServiceInstance.Get(session_id);

                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);


                    objServiceDonations.Amount = objServiceDonations.Amount;
                    objServiceDonations.PaymentMethod = PaymentMethod;
                    objServiceDonations.TransactionId = paymentIntent.Id;
                    objServiceDonations.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                    objServiceDonations.ServiceDonationId = ServiceDonationId;
                    // objServiceDonations.OrderDate = DateTime.UtcNow;
                    objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;

                    Int64 result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation Acknowledgement Page",
                        LogDescription = "Updated Transaction details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });


                    string contributions = "";

                    lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);



                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation Acknowledgement Page",
                        LogDescription = "Get Service Donations info By Id(" + objServiceDonations.FirstName + " " + objServiceDonations.LastName + " - " + ServiceDonationId + " - " + DateTime.UtcNow + " )",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                   
                    if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                    {
                        contributions = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";

                        int i = 1;
                        foreach (var item in lstServiceContributions)
                        {
                            contributions = contributions + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.ServiceTitle + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                            i++;
                        }
                        contributions = contributions + "</table>";
                    }

                    int _status = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                        body.Replace("[FirstName]", objServiceDonations.FirstName);
                        body.Replace("[LastName]", objServiceDonations.LastName);
                        body.Replace("[Email]", objServiceDonations.Email);
                        body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                        body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                        body.Replace("[DonationCause]", objServiceDonations.DonationCauses);

                        body.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                        body.Replace("[TransactionId]", paymentIntent.Id);
                        body.Replace("[PaymentStatus]", session.PaymentStatus == "paid" ? "Completed" : "Pending");
                        body.Replace("[PaymentType]", PaymentMethod);
                        body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body.Replace("[Contributions]", contributions);

                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.DonationEmail, subject, body.ToString());

                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "User Email Sent" : "User Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to user: {objServiceDonations.Email}"
                                    : $"Confirmation email FAILED for user: {objServiceDonations.Email} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }
                    Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                    if (objTemplates1 != null)
                    {
                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplates1.Description);
                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                        body1.Replace("[FirstName]", objServiceDonations.FirstName);
                        body1.Replace("[LastName]", objServiceDonations.LastName);
                        body1.Replace("[Email]", objServiceDonations.Email);
                        body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                        body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                        body1.Replace("[DonationCause]", objServiceDonations.DonationCauses);

                        body1.Replace("[DonationProgram]", objServiceDonations.DonationCause);
                        body1.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                        body1.Replace("[TransactionId]", paymentIntent.Id);
                        body1.Replace("[PaymentStatus]", session.PaymentStatus == "paid" ? "Completed" : "Pending");
                        body1.Replace("[PaymentType]", PaymentMethod);
                        body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                        body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body1.Replace("[Contributions]", contributions);

                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);

                        if (objAppInfo.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.DonationEmail, objAppInfo.DonationEmail, subject, body1.ToString());

                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to Admin: {objAppInfo.DonationEmail}"
                                    : $"Confirmation email FAILED for Admin: {objAppInfo.DonationEmail} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }

                    ViewBag.PayerEmail = objServiceDonations.Email;
                    ViewBag.UserName = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
                    ViewBag.DonationProgram = objServiceDonations.DonationCause;

                    ViewBag.GrossTotal = objServiceDonations.Amount;
                    ViewBag.ServiceDonationId = ServiceDonationId;
                    DateTime paydate = DateTime.UtcNow;
                    DateTime Date = paydate = DateTime.UtcNow;
                    ViewBag.PaymentDate = objServiceDonations.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt");

                    ViewBag.objServiceDonations = objServiceDonations;
                    ViewBag.lstServiceContributions = lstServiceContributions;


                }
                else if (PaymentMethod == "Swire Pay")
                {
                    spPaymentLink = Request.Query["sp-payment-link"].ToString();

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation",
                        LogDescription = "Swire Pay Started into Acknowledgement Page(" + objServiceDonations.FirstName + " " + objServiceDonations.LastName + " - " + ServiceDonationId + " - " + DateTime.UtcNow + " )",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                   
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.swirepay.com/v1/payment-link/" + spPaymentLink + "/verify");

                    int status2 = 0;
                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

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
                        pstatus = paymentType;
                        tid = receiptNumber;

                        // Output the values
                        Console.WriteLine($"Receipt Number: {receiptNumber ?? "Not Available"}");
                        Console.WriteLine($"Payment Type: {paymentType ?? "Not Available"}");


                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donation",
                            LogDescription = "Response Status is " + status + " into Acknowledgement Page", 
                            LogDate = DateTime.Now,
                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        Int32 test = 0;

                        objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);


                        objServiceDonations.Amount = objServiceDonations.Amount;
                        objServiceDonations.PaymentMethod = PaymentMethod;
                        objServiceDonations.TransactionId = receiptNumber;
                        objServiceDonations.PaymentStatus = status1 == "SUCCEEDED" ? "Completed" : "Pending";
                        objServiceDonations.ServiceDonationId = ServiceDonationId;
                        //objServiceDonations.OrderDate = DateTime.UtcNow;

                        objServiceDonations.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;

                        Int64 result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donation",
                            LogDescription = "Updated Transaction details",
                            LogDate = DateTime.Now,
                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });



                        //Int32 test = 0;


                        //  objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref test);

      
                        string contributions = "";

                        lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donation",
                            LogDescription = "Get Service Donations info By Id",
                            LogDate = DateTime.Now,
                            InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });



                        if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                        {
                            contributions = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";

                            int i = 1;
                            foreach (var item in lstServiceContributions)
                            {
                                contributions = contributions + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.ServiceTitle + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                                i++;
                            }
                            contributions = contributions + "</table>";
                        }

                        int _status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                        if (objTemplates != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                            body.Replace("[FirstName]", objServiceDonations.FirstName);
                            body.Replace("[LastName]", objServiceDonations.LastName);
                            body.Replace("[Email]", objServiceDonations.Email);
                            body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                            body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                            body.Replace("[DonationCause]", objServiceDonations.DonationCauses);

                            body.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                            body.Replace("[TransactionId]", receiptNumber);
                            body.Replace("[PaymentStatus]", status1 == "SUCCEEDED" ? "Completed" : "Pending");
                            body.Replace("[PaymentType]", PaymentMethod);
                            body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ApplicationName]", objAppInfo.SiteName);
                            body.Replace("[Contributions]", contributions);

                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplates.Subject;
                            subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.DonationEmail, subject, body.ToString());

                                // ✅ Check if email was successful or failed
                                bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = isEmailSuccess ? "User Email Sent" : "User Email Sent Failed",
                                    LogDescription = isEmailSuccess
                                        ? $"Confirmation email sent successfully to user: {objServiceDonations.Email}"
                                        : $"Confirmation email FAILED for user: {objServiceDonations.Email} | Response: {responsce}",

                                    LogDate = DateTime.Now,
                                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "without User Email Sent to procced",
                                LogDescription = "without User Email Sent to procced",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                        if (objTemplates1 != null)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplates1.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                            body1.Replace("[FirstName]", objServiceDonations.FirstName);
                            body1.Replace("[LastName]", objServiceDonations.LastName);
                            body1.Replace("[Email]", objServiceDonations.Email);
                            body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                            body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));

                            body1.Replace("[DonationCause]", objServiceDonations.DonationCauses);

                            body1.Replace("[DonationProgram]", objServiceDonations.DonationCause);
                            body1.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                            body1.Replace("[TransactionId]", receiptNumber);
                            body1.Replace("[PaymentStatus]", status1 == "SUCCEEDED" ? "Completed" : "Pending");
                            body1.Replace("[PaymentType]", PaymentMethod);
                            body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                            body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                            body1.Replace("[Contributions]", contributions);

                            body1.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplates.Subject;
                            subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);

                            if (objAppInfo.Email == "Yes")
                            {
                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.DonationEmail, objAppInfo.DonationEmail, subject, body1.ToString());

                                // ✅ Check if email was successful or failed
                                bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                    LogDescription = isEmailSuccess
                                        ? $"Confirmation email sent successfully to Admin: {objAppInfo.DonationEmail}"
                                        : $"Confirmation email FAILED for Admin: {objAppInfo.DonationEmail} | Response: {responsce}",

                                    LogDate = DateTime.Now,
                                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "without Admin Email Sent to procced",
                                LogDescription = "without Admin Email Sent to procced",
                                LogDate = DateTime.Now,
                                InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                    }
                    //var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);

                    ViewBag.PayerEmail = objServiceDonations.Email;
                    ViewBag.UserName = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
                    ViewBag.DonationProgram = objServiceDonations.DonationCause;

                    ViewBag.GrossTotal = objServiceDonations.Amount;
                    ViewBag.ServiceDonationId = ServiceDonationId;
                    DateTime paydate = DateTime.UtcNow;
                    DateTime Date = paydate = DateTime.UtcNow;
                    ViewBag.PaymentDate = Date.ToString("MM/dd/yyyy hh:mm:ss tt");

                    ViewBag.objServiceDonations = objServiceDonations;
                    ViewBag.lstServiceContributions = lstServiceContributions;

                }

                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Donation Acknowledgement",
                        LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                        LogDate = DateTime.Now,
                        InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    // Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
                    // List<Entities.ServiceContributions> lstServiceContributions = new List<Entities.ServiceContributions>();

                    int result1 = 0;

                    objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref result1);



                    lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);

                   

                    ViewBag.PayerEmail = objServiceDonations.Email;
                    ViewBag.UserName = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
                    ViewBag.DonationProgram = objServiceDonations.DonationCause;

                    ViewBag.GrossTotal = objServiceDonations.Amount;
                    ViewBag.ServiceDonationId = ServiceDonationId;
                    //DateTime Date = paydate = DateTime.UtcNow;
                    //ViewBag.PaymentDate = objServiceDonations.OrderDate;
         
                    ViewBag.objServiceDonations = objServiceDonations;
                    ViewBag.lstServiceContributions = lstServiceContributions;


                }

            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Donation Acknowledgement",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objServiceDonations.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objServiceDonations.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            
           
            ViewBag.cname = cname;
            ViewBag.objServiceDonations = objServiceDonations;
            ViewBag.lstServiceContributions = lstServiceContributions;
            return View();
        }

        //Paypal IPN


        [HttpPost]
        public async Task<JsonResult> InsertPaymentAsync(Entities.ServiceDonations payment)
        {
            Int64 LRstatus = 0;
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            Entities.LogReport logs = new Entities.LogReport();
            //logreport("Donation Registration: Entered into Sports Registration - " + payment.FirstName + " - " + payment.Email, Request.Path);


            logs = new Entities.LogReport();
            logs.LogTitle = "Donations"; logs.LogDescription = "Donation Registration: Entered into Donation Registration"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
            lstLogReport.Add(logs);
            LRstatus = _Users.InsertLogReport(lstLogReport);

            Int64 _status = 0;
            Int64 ServiceDonationId = 0;
            Int64 MemberId = 0;
            int status2 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
            try
            {

                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(payment.Honeypot))
                {
                   // logreport("Donation Registration: Honeypot Bot activity detected - " + payment.FirstName + " - " + payment.Email, Request.Path);


                    logs = new Entities.LogReport();
                    logs.LogTitle = "Donations"; logs.LogDescription = "Donation Registration: Honeypot Bot activity detected -"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
                    lstLogReport.Add(logs);
                    LRstatus = _Users.InsertLogReport(lstLogReport);

                    return Json(new { ok = false, data = "Honeypot Bot activity detected" });
                }
                string YoursecretKey = objAppInfo.CapchaSecreatKey;
                //  objAppInfo.CapchaSecreatKey; //Put Your Secret Key .

                //If capcha is "Yes", recapcha verification is process, if capcha is "No" without verification we can submit form...
                var capcha = objAppInfo.ShowCapcha;
                //objAppInfo.ShowCapcha;


                string captchaResponse = payment.recaptchaToken;
                string clientHost = HttpContext.Request.Host.Host;
                //Validate CAPTCHA
                bool isHuman = await BLL.Common.ValidateCaptchaAsync(captchaResponse, clientHost);

                if (capcha == "No")
                {
                    isHuman = true;
                }


                if (!isHuman)
                {
                    /* 
                     * If the IsHuman boolean value is false, 
                     * the form submission process will be stopped.
                     */
                    // logreport("Donation Registration: reCAPTCHA validation failed. Please try again - " + payment.FirstName + " - " + payment.Email, Request.Path);

                    logs = new Entities.LogReport();
                    logs.LogTitle = "Donations"; logs.LogDescription = "Donation Registration: reCAPTCHA validation failed. Please try again -"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
                    lstLogReport.Add(logs);
                    LRstatus = _Users.InsertLogReport(lstLogReport);



                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");

                    return Json(new { ok = false, data = "reCAPTCHA validation failed. Please try again." });
                }


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                decimal Percentage = 0;
                decimal IndAmount = payment.Amount;
                decimal TotalAmount = payment.Amount;
                payment.UpdatedBy = payment.FirstName;
                string receipturl = "NA";

                payment.MemberId = payment.MemberId;
                payment.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                ServiceDonationId = payment.ServiceDonationId;
               

                _status = _ServiceDonations.InsertServiceDonations(payment, ref ServiceDonationId, ref receipturl);

                // logreport("Donation Registration: Inserted basic info Successfuly (" + payment.FirstName + " - " + payment.Email + ")", Request.Path);

                logs = new Entities.LogReport();
                logs.LogTitle = "Donations"; logs.LogDescription = "Donation Registration: Inserted basic info Successfuly"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = payment.FirstName + " " + payment.LastName;
                lstLogReport.Add(logs);
                LRstatus = _Users.InsertLogReport(lstLogReport);


                if (ServiceDonationId != 0)
                {
                    return Json(new { success = ServiceDonationId > 0, paymentId = ServiceDonationId });
                }
                else
                {
                    return Json(new { ok = false, data = "Data inserted Failed" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, data = ex.Message });
            }

        }

        public ActionResult DonationIPN(Int64 ServiceDonationId = 0)
        {
            int result1 = 0;
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            PayByPayPal.PDTHolder objPDT = new PayByPayPal.PDTHolder();
            Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
            objServiceDonations = _ServiceDonations.ServiceDonationsGetById(ServiceDonationId, ref result1);
            string status = "";
            try
            {
                int qstatus = 0;
                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref qstatus);
                objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;
                status = null /*objPayByPayPal.ipn()*/;

                // For Testing the Action
                //BLL.Common.SendMailwithfrom("y.sekhar@innovateindia.in", ConfigurationManager.AppSettings["adminemailid"].ToString(), "testIPN", status.ToString());

                if (status == "VERIFIED")
                {
                    objPDT = objPayByPayPal.objPDTHolder;

                    objServiceDonations.Amount = Convert.ToDecimal(objPDT.GrossTotal);
                    objServiceDonations.PaymentMethod = "PayPal";
                    objServiceDonations.TransactionId = objPDT.TransactionId;
                    objServiceDonations.PaymentStatus = objPDT.PaymentStatus;
                    objServiceDonations.ServiceDonationId = objServiceDonations.ServiceDonationId;
                    //objServiceDonations.OrderDate = ConvertPayPalDateTime(objPDT.PaymentDate.ToString());
                    objServiceDonations.UpdatedBy = objServiceDonations.FirstName;
                    Int64 result = 0;
                    result = _ServiceDonations.UpdateServiceDonations(objServiceDonations);
                }
            }
            catch
            {
                TempData["message"] = "Your payment did not go through please contact admin for assistance.";
            }

            ViewBag.status = status;
            return View();
        }

        
        public DateTime ConvertPayPalDateTime(string payPalDateTime)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            // accept a few different date formats because of PST/PDT timezone and slight month difference in sandbox vs. prod.
            string[] dateFormats = { "HH:mm:ss MMM dd, yyyy EST", "HH:mm:ss MMM. dd, yyyy EST", "HH:mm:ss MMM dd, yyyy EDT", "HH:mm:ss MMM. dd, yyyy EDT",
                             "HH:mm:ss dd MMM yyyy EST", "HH:mm:ss dd MMM. yyyy EST", "HH:mm:ss dd MMM yyyy EDT", "HH:mm:ss dd MMM. yyyy EDT"};
            DateTime outputDateTime;

            DateTime.TryParseExact(payPalDateTime, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTime);

            // convert to local timezone
            TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            outputDateTime = TimeZoneInfo.ConvertTime(outputDateTime, hwZone, TimeZoneInfo.Local);

            return outputDateTime;
        }
        
        public ActionResult ServicesIndex(Int64 MemberId = 0, Int64 ChapterId = 0, Int64 ServiceCategoryId = 0, string ServiceType = "", string cname = "")
        {
            int qstatus = 0;
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
            ViewBag.ServiceCategoryId = ServiceCategoryId;
            ViewBag.ServiceType = ServiceType;
            ViewBag.MemberId = MemberId;
            ViewBag.cname = cname;
            return View();
        }

        public ActionResult FEServicesList(Int64 MemberId = 0, Int64 ChapterId = 0, string cname = "", Int64 CategoryId = 0, string ServiceType = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 50)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");

            int Total = 0;
            try
            {
                lstServices = _Services.FEGetServicesListByVariable(MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);

            }
            catch(Exception ex)
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServices = lstServices;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.ChapterId = ChapterId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.ServiceType = ServiceType;
            ViewBag.MemberId = MemberId;
            ViewBag.cname = cname;
            return View();

        }
        [HttpGet]
        public ActionResult ServicesDetails(Int64 sid = 0, string SName = "", string cname = "")
        {
            try
            {
                int status = 0;
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
                }

                if (sid != 0)
                {
                    _objServices = _Services.FEGetServicesById(sid, ref lstServiceImages, ref lstServiceDonations, ref status);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.objServices = _objServices;
            ViewBag.lstServiceImages = lstServiceImages;
            ViewBag.lstServiceDonations = lstServiceDonations;
            ViewBag.ChapterName = cname;
            return View();
        }

        [HttpPost]
        public JsonResult ServicesGetListByChapterId()
        {
            long chapterId = Convert.ToInt64(HttpContext.Session.GetString("ChapterId"));
            int status = 0;
            try
            {
                lstServices = _Services.FEServicesGetListByChapterId(chapterId, ref status);
                bool data = (lstServices != null && lstServices.Count != 0 ? false : true);

                return Json(new { ok = true, data = lstServices, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">" + ex.Message + "</div>" });
            }
        }

        public ActionResult FEServicesListByYear(string Year = "", Int64 MemberId = 0, Int64 ChapterId = 0, string cname = "", Int64 CategoryId = 0, string ServiceType = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 200)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int qstatus = 0; 
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            } 
            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            int Total = 0;
            try
            {
                lstServices = _Services.FEGetServicesListByVariableByYear(Year, MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServices = lstServices;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.ChapterId = ChapterId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.ServiceType = ServiceType;
            ViewBag.MemberId = MemberId;
            ViewBag.Year = Year;
            ViewBag.cname = cname;
            return View();

        }

        public ActionResult ServicesDonationsList(Int64 ServiceId = 0, string cname = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 50)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");

            int qstatus = 0;
            Int64 ChapterId = 0;
            int Total = 0;
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            lstServiceDonations = _ServiceDonations.GetServiceDonationsList(ServiceId, Search, Sort, PageNo, Items, ref Total);

            ViewBag.lstServiceDonations = lstServiceDonations;
            ViewBag.cname = cname;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.total = Total;
            return View();
        }

        public ActionResult AddDonation(Int64 ServiceCategoryId = 0, Int64 MemberId = 0, Int64 ChapterId = 0, string Type = "", Int64 ServiceId = 0, string cname = "", Int64 ServiceDonationId = 0)
        {

            try
            {
                int status = 0;
                int QStatus = 0;
                int _qstatus = 0;

                int qstatus = 0;
                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;

                    lstServices = _Services.FEServicesGetListByChapterId(ChapterId, ref status);

                    long chapterId = Convert.ToInt64(HttpContext.Session.GetString("ChapterId"));

                    _objServices = _Services.FEGetServicesDataById(ServiceId, ref lstServiceImages, ref status);

                    if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null)
                    {
                        _objMember = _Members.GetMemberFullDetailsByEmail(HttpContext.User.Identity.Name.ToString(), ref _qstatus);
                    }

                    HttpContext.Session.SetString("MemberId", _objMember.MemberId.ToString());
                    objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.Memberdetails = _objMember;
            ViewBag.ServiceCategoryId = ServiceCategoryId;
            ViewBag.ChapterId = ChapterId;
            ViewBag.Type = Type;
            ViewBag.ServiceId = ServiceId;
            ViewBag.MemberId = _objMember.MemberId;
            ViewBag.objServices = _objServices;
            ViewBag.lstServiceImages = lstServiceImages;
            ViewBag.lstServices = lstServices;
            ViewBag.cname = cname;
            ViewBag.ServiceDonationId = ServiceDonationId;
            ViewBag.objMembers = objMembers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertDonation(
    IFormFile file,
    IFormFile filezelle,
    Entities.ServiceDonations objServiceDonations,
    List<Entities.ServiceContributions> lstServiceContributions,
    string cname = "",
    long ServiceDonationId = 0)
        {
            try
            {
                decimal totalAmount = objServiceDonations.Amount;
                decimal individualAmount = totalAmount;

                objServiceDonations.UpdatedBy = objServiceDonations.FirstName;

                int status2 = 0;
                var appInfo = _appinfo.GetAppInfoDetails(ref status2);

                // Reset amount if contributions exist
                if (lstServiceContributions?.Any(x => x.ServiceId != 0) == true)
                {
                    objServiceDonations.Amount = 0;
                }
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
                string receiptUrl = "NA";

                // File upload helper
                async Task<string> SaveFileAsync(IFormFile uploadedFile)
                {
                    if (uploadedFile == null || uploadedFile.Length == 0)
                        return null;

                    var fileName = Path.GetFileName(uploadedFile.FileName);
                    var folderPath = Path.Combine(objAppInfo.UploadPath, "uploads", "temp");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }

                    return fileName;
                }

                // Save files
                var file1 = await SaveFileAsync(file);
                var file2 = await SaveFileAsync(filezelle);

                if (!string.IsNullOrEmpty(file1)) receiptUrl = file1;
                if (!string.IsNullOrEmpty(file2)) receiptUrl = file2;

                // Insert donation
                long insertStatus = _ServiceDonations.InsertServiceDonations(
                    objServiceDonations,
                    ref ServiceDonationId,
                    ref receiptUrl
                );

                if (ServiceDonationId != 0)
                {
                    if (lstServiceContributions?.Any(x => x.ServiceId != 0) == true)
                    {
                        decimal contributedAmount = 0;
                        decimal percentSum = 0;

                        var validContributions = lstServiceContributions
                            .Where(x => x.ServiceId != 0)
                            .ToList();

                        bool hasGeneral = validContributions.Any(x => x.ServiceId == 10);

                        if (hasGeneral)
                        {
                            var specific = validContributions.Where(x => x.ServiceId != 10);

                            foreach (var item in specific)
                            {
                                individualAmount = (totalAmount / 100) * Convert.ToDecimal(item.Percentage);

                                item.CAmount = individualAmount;
                                item.ServiceDonationId = ServiceDonationId;

                                contributedAmount += individualAmount;
                                percentSum += Convert.ToDecimal(item.Percentage);

                                var res = _ServiceDonations.InsertServiceContributions(item);
                                if (res == -1)
                                    return ErrorRedirect();
                            }

                            // General contribution
                            var general = new Entities.ServiceContributions
                            {
                                ServiceId = 10,
                                ServiceDonationId = ServiceDonationId,
                                CAmount = totalAmount - contributedAmount,
                                Percentage = (100 - percentSum).ToString()
                            };

                            _ServiceDonations.InsertServiceContributions(general);
                        }
                        else
                        {
                            int index = 1;
                            int totalCount = validContributions.Count;

                            foreach (var item in validContributions)
                            {
                                long res;

                                if (index != totalCount)
                                {
                                    individualAmount = (totalAmount / 100) * Convert.ToDecimal(item.Percentage);
                                    contributedAmount += individualAmount;
                                    percentSum += Convert.ToDecimal(item.Percentage);
                                }
                                else
                                {
                                    individualAmount = totalAmount - contributedAmount;
                                    item.Percentage = (100 - percentSum).ToString();
                                }

                                item.CAmount = individualAmount;
                                item.ServiceDonationId = ServiceDonationId;

                                res = _ServiceDonations.InsertServiceContributions(item);

                                if (res == -1)
                                    return ErrorRedirect();

                                index++;
                            }
                        }
                    }

                    return RedirectToAction("ThankYou", "Services",
                        new { ServiceDonationId, cname });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='error1 closable'>{ex.Message}</div>";
                return RedirectToAction("Index", "Services");
            }

            return View();
        }

        private IActionResult ErrorRedirect()
        {
            throw new NotImplementedException();
        }

        public ActionResult ThankYou(string cname = "", string type = "")
        {
            ViewBag.cname = cname;
            ViewBag.type = type;
            return View();
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


        public ActionResult Print(Int64 Id = 0, string amt = "")
        {
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            List<Entities.ServiceContributions> lstServiceContributions = new List<Entities.ServiceContributions>();
            int status2 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

            int result1 = 0;
            objServiceDonations = _ServiceDonations.ServiceDonationsGetById(Id, ref result1);

            int _qstatus = 0;
            string contributions = "";

            lstServiceContributions = _ServiceDonations.GetContributionsById(Id, ref _qstatus); 

            if (lstServiceContributions != null && lstServiceContributions.Count != 0)
            {
                contributions = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";

                int i = 1;
                foreach (var item in lstServiceContributions)
                {
                    if (item.Amount != 0)
                    {
                        contributions = contributions + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + (item.ServiceTitle == null ? objServiceDonations.DonationCause : item.ServiceTitle) + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                        i++;
                    }
                }
                contributions = contributions + "</table>";
            }

            int _status = 0;
            Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
            if (objTemplates != null)
            {
                StringBuilder body = new StringBuilder();
                body.Append(objTemplates.Description);
                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                body.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                body.Replace("[FirstName]", objServiceDonations.FirstName);
                body.Replace("[LastName]", objServiceDonations.LastName);
                body.Replace("[Email]", objServiceDonations.Email);
                body.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                body.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                if (objServiceDonations.DonationCause == "General Donation")
                {
                    body.Replace("[DonationCause]", "");
                }
                else
                {
                    body.Replace("[DonationCause]", "Cause : <span style=\"color:#0490bf;\">" + objServiceDonations.DonationCause + "</span>");
                }

                body.Replace("[DonationAmount]", amt);
                body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                body.Replace("[PaymentType]", "Paypal");
                body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                body.Replace("[GUrl]",objAppInfo.GooglePlusUrl);
                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                body.Replace("[SiteName]", objAppInfo.SiteName);
                body.Replace("[ApplicationName]", objAppInfo.SiteName);
                body.Replace("[Contributions]", contributions);

                objTemplates.Description = body.ToString();
            }

            ViewBag.objTemplates = objTemplates;
            return View();
        }
        public ActionResult PaymentCancelled(Int64 id = 0)
        {
            string filename = "Log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            var relativePath = "~/Content/logfiles/" + filename;
            //var filepath = HttpContext.Server.MapPath(relativePath);

            //logreport("Payment failed", Request.Path, filepath);
            int result1 = 0;
            Entities.Members objMembers = new Entities.Members();
            PayByPayPal.PDTHolder objPDT = new PayByPayPal.PDTHolder();
            Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
            objMembers = _Members.GetMemberFullDetailsById(id, ref result1);
            string status = "";
            try
            {
                int qstatus = 0;
                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref qstatus);
                objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;

                objPDT = objPayByPayPal.objPDTHolder;

                objMembershipOrders.PaymentStatus = "Cancelled";
                objMembershipOrders.PaymentMethod = "PayPal";
                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                objMembershipOrders.UpdatedBy = objMembers.FirstName;
                objMembershipOrders.MemberId = objMembers.MemberId;
                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                objMembershipOrders.Amount = objMembers.MemberAmount;

                Int64 result = _Members.InsertMemberOrder(objMembershipOrders);

            }
            catch
            {
                TempData["message"] = "Your payment did not go through please contact  board@teluguassociationtoolweb.org for assistance.";

            }

            ViewBag.status = status;

            return View();
        }



        //public ActionResult PayPalSaveLog(string message)
        //{
        //    try
        //    {
        //        Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
        //        string folderPath = objAppInfo.BaseUrl("~/Content/Log/PayPalSaveLog/");
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



    }
}
