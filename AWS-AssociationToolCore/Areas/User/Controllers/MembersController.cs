using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.Entities;
using Braintree;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.XWPF.UserModel;
using OpenXmlPowerTools;
using PaymentProcess;
using Square;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class MembersController : Controller
    {
        BLL.InnerPages _InnerPages = new BLL.InnerPages();
        BLL.Members _Members = new BLL.Members();
        BLL.Volunteers _Volunteers = new BLL.Volunteers();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        List<Entities.Members> lstMembers = new List<Entities.Members>();
        Entities.Members objMembers = new Entities.Members();
        BLL.MembershipTypes _MembershipType = new BLL.MembershipTypes();
        List<Entities.MembershipTypes> lstMembershipType = new List<Entities.MembershipTypes>();
        List<Entities.Volunteers> lstVolunteers = new List<Entities.Volunteers>();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();
        PayByPayPal objPayByPayPal = new PayByPayPal();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        Entities.Members objMemberDetails = new Entities.Members();
        BLL.Events _Events = new BLL.Events();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.ServiceDonations _ServiceDonations = new BLL.ServiceDonations();
        BLL.KidsRegistrations _BalalaSambaralu = new BLL.KidsRegistrations();
        BLL.Users _Users = new BLL.Users();
        RecurringPaymentController _RecurringPaymentController = new RecurringPaymentController();
        BLL.SendMail _sentmail = new BLL.SendMail();
        Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
        List<Entities.EventCompetitions> lstEventCompetitions = new List<Entities.EventCompetitions>();
        BLL.EventCompetitions _EventCompetitions = new BLL.EventCompetitions();
        AWS_AssociationToolCore.BLL.ServiceDonations _Donor = new AWS_AssociationToolCore.BLL.ServiceDonations();
        private string PayPalResponse;

        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IBraintreeConfiguration _config;

        private readonly IDataProtectionProvider _dataProtectionProvider;

        private readonly IMemoryCache _cache;

        public MembersController(IBraintreeConfiguration config, IDataProtectionProvider dataProtectionProvider, IMemoryCache cache)
        {
            _config = config;
            _dataProtectionProvider = dataProtectionProvider;
            _cache = cache;
        }

        #region Membership
        //Seema
        //final
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult AddMember(Entities.Members objMembernew, long memberId = 0, long chapterId = 0, string cname = "")
        {
            long actualChildrenId = 0;

            if (!string.IsNullOrEmpty(objMembernew.ChildrenId))
            {
                try
                {
                    actualChildrenId = Convert.ToInt64(AWS_AssociationToolCore.BLL.Common.Decrypt(objMembernew.ChildrenId));
                }
                catch
                {
                    actualChildrenId = 0; // tampered/garbage value — fail safe instead of throwing
                }
            }
            Entities.InnerPages objmembershipcontent = new Entities.InnerPages();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            int qstatus = 0;

            // ✅ Read cookie safely (ASP.NET Core way)
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }

            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            int status1 = 0;
            lstChapters = _Chapters.GetChaptersList(ref status1);

            if (objChapters != null)
            {
                chapterId = objChapters.ChapterId;
            }

            if (chapterId == 0)
            {
                chapterId = 1;
            }

            string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);

            try
            {
                // ✅ Set cookie in ASP.NET Core
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("FormValidationCode", generatedCode, cookieOptions);

                int _qstatus = 0;
                int status = 0;

                objMembers = _Members.AddMembershipRequirementByChapterId(chapterId, ref _qstatus);

                objMemberDetails = _Members.FEMemberGetFullDetails(memberId, ref status);

                if (_qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Members");
                }

                if (status == 1)
                {
                    ViewBag.objMemberDetails = objMemberDetails;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error1 closable\">Failed Transaction: {ex.Message}</div>";
                return RedirectToAction("Index", "Members");
            }

            ViewBag.objmembershipcontent = objmembershipcontent;
            ViewBag.ChapterId = chapterId;
            ViewBag.cname = cname;
            ViewBag.actualChildrenId = actualChildrenId;
            ViewBag.MemberId = memberId;
            ViewBag.lstChapters = lstChapters;
            ViewBag.ValidationCode = generatedCode;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddMember(
    Entities.Members objMembers,
    IFormFile file2,
    IFormFile filezelle,
    List<Entities.ChildrenInfo> lstChildrenInfo,
    Entities.MembershipTypes objMembershipType,
    string Phc1 = "",
    string Phc2 = "",
    string Phc3 = "",
    string MCaptcha = "",
    string cname = "",
    string PhoneNo = "",
    string Notifications = "",
    string recaptchaToken = "",
    string Honeypot = "",
    string ValidationCode = "")
        {
            IBraintreeGateway gateway = _config.GetGateway();

            Int64 LRstatus = 0;

            // IP Address (Core)
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            objMembers.WebsiteAddress = ip;

            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
            Entities.LogReport logs = new Entities.LogReport();
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Membership Registration",
                LogDescription = $"Membership Form Submission",
                LogDate = DateTime.Now,
                InsertedBy = objMembers.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objMembers.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            // Honeypot validation
            if (!string.IsNullOrWhiteSpace(Honeypot))
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = "Honeypot Bot activity detected",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                ModelState.AddModelError("", "Bot activity detected.");
                return RedirectToAction("AddMember", "Members", new { cname = cname });
            }

            if (Request.Form["ClientValidationStatus"] == "true")
                ModelState.Clear();

            // Cookie validation (Core)
            if (!Request.Cookies.TryGetValue("FormValidationCode", out string cookieValue) || string.IsNullOrEmpty(cookieValue))
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = "Failed Retrieve the code from the cookie.",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                ModelState.AddModelError("", "Security validation failed.");
                return RedirectToAction("AddMember", "Members", new { cname = cname });
            }

            if (cookieValue != ValidationCode)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = "Failed Security validation",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                ModelState.AddModelError("", "Security validation failed.");
                return RedirectToAction("AddMember", "Members", new { cname = cname });
            }

            try
            {


                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = "After reCAPTCHA Entered to the Process",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                string imageurl = "NA";
                string receipturl = "NA";
                string SanitizeFileName(string fileName)
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);

                    // Remove spaces and special chars
                    name = name.Replace(" ", "_")
                               .Replace("(", "")
                               .Replace(")", "")
                               .Trim('.');

                    return name + ext;
                }

                // Upload to temp with sanitized name
                if (file2 != null && file2.Length > 0)
                {
                    imageurl = SanitizeFileName(file2.FileName);
                    receipturl = imageurl;
                    string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", imageurl);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                    await using var stream1 = new FileStream(tempPath, FileMode.Create);
                    await file2.CopyToAsync(stream1);
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    receipturl = SanitizeFileName(filezelle.FileName);
                    string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", receipturl);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                    await using var stream2 = new FileStream(tempPath, FileMode.Create);
                    await filezelle.CopyToAsync(stream2);
                }

                // ✅ Save original names before DB insert
                string originalImageUrl = imageurl;
                string originalReceiptUrl = receipturl;




                if (objMembers.PaymentMethod == "Cash/Cheque" || objMembers.PaymentMethod == "Bank/Transfer")
                {
                    objMembers.IsApproved = false;
                }
                else
                {
                    objMembers.IsApproved = true;
                }
                objMembers.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objMembers.InsertedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objMembers.IsLockedOut = false;

                string Password = objMembers.Password;
                objMembers.Password = BLL.Password.ComputeHash(Password, "SHA512", null);
                objMembers.IsActivated = false;
                objMembers.RegistrationGUID = AWS_AssociationToolCore.BLL.Common.generateGUID();
                objMembers.UserName = objMembers.FirstName + " " + objMembers.LastName;
                objMembers.DateActivated = DateTime.UtcNow;
                objMembers.LastPasswordChangedDate = DateTime.MinValue;
                objMembers.Fax = "No";
                string RecurringType = objMembers.RecurringType;
                objMembers.SpouseOccupation = objMembers.ChildrenId;


                Int64 MemberId = 0;
                Int64 _status = 0;
                objMembers.objMembershipOrder.UpdatedBy = User.Identity?.Name ?? "";

                _status = _Members.FEInsertMembers(objMembers, ref MemberId, ref imageurl, ref receipturl);

                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = status1 == 1
                                                    ? $"Members inserted successfully. ServiceDonationId: {MemberId}"
                                                    : $"Members insert failed. Status: {status1}",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });



                // ✅ Move using ORIGINAL temp name → rename to DB name
                if (file2 != null && file2.Length > 0 && imageurl != "NA")
                {
                    string finalFolder = Path.Combine(objAppInfo.UploadPath, "MemberReceiptImages");
                    Directory.CreateDirectory(finalFolder);

                    string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalImageUrl);
                    string dest = Path.Combine(finalFolder, imageurl); // imageurl now has MemberId prefix
                    if (System.IO.File.Exists(tempSrc))
                        System.IO.File.Move(tempSrc, dest, overwrite: true);
                }

                if (filezelle != null && filezelle.Length > 0 && receipturl != "NA")
                {
                    string finalFolder = Path.Combine(objAppInfo.UploadPath, "MemberReceiptImages");
                    Directory.CreateDirectory(finalFolder);

                    string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalReceiptUrl);
                    string dest = Path.Combine(finalFolder, receipturl); // receipturl now has MemberId prefix
                    if (System.IO.File.Exists(tempSrc))
                        System.IO.File.Move(tempSrc, dest, overwrite: true);
                }




                // Children info
                if (lstChildrenInfo != null && lstChildrenInfo.Count() != 0)
                {
                    foreach (Entities.ChildrenInfo objChildrenInfo in lstChildrenInfo)
                    {
                        objChildrenInfo.MemberId = MemberId;
                        if (objChildrenInfo != null && objChildrenInfo.FirstName != null)
                        {
                            Int64 estatus = _Members.InsertChildrenInfo(objChildrenInfo);
                            if (estatus == -1)
                            {
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Members ChildrenInfo Failed",
                                    LogDescription = $"Failed inserting ChildrenInfo member details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                TempData["message"] = "<div class=\"error1 closable\">Failed inserting member details.</div>";
                                return RedirectToAction("AddMember", "Members", new { cname = cname });
                            }
                        }
                    }
                }

                if (objMembers.MemberAmount != 0)
                {
                    // Session (Core)
                    HttpContext.Session.SetString("TotalAmount", objMembers.MemberAmount.ToString());

                    if (objMembers.PaymentMethod == "Square")
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
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        int paystatus = 0;
                        Entities.PaymentSettings objsettings = _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

                        string squareAccessToken = objsettings.SuccessUrl;
                        string squareEnvironment = objsettings.AccountType == "Test" ? "sandbox" : "production";
                        string squareLocationId = objsettings.TokenNo;
                        string squareNonce = Request.Form["squareNonce"].ToString();
                        // ADD THIS RIGHT AFTER getting objsettings
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

                        //  ── ADDED: Recurring detection (same pattern as ServiceDonation) ────────
                        bool isRecurring = !string.IsNullOrEmpty(objMembers.RecurringType)
                                           && objMembers.MemberAmount != 0;

                        string squareCustomerId = "";
                        string squareCardId = "";          // reusable card-on-file token (card_xxx)
                        string tokenForPayment = squareNonce; // will swap to card_xxx if recurring

                        // ── ADDED: STEP 1 — Create Customer + Save Card on File (only if recurring)
                        // Nonce is single-use — save it to a customer FIRST to get a
                        // reusable card_id; that card_id is then used for the charge AND subscription
                        if (isRecurring)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = $"Recurring detected — Creating Square Customer | MemberId:{MemberId}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            try
                            {
                                // ── Create Customer ──────────────────────────────────────────────
                                var createCustomerBody = new Square.Models.CreateCustomerRequest.Builder()
                                    .IdempotencyKey(Guid.NewGuid().ToString())
                                    .GivenName(objMembers.FirstName)
                                    .FamilyName(objMembers.LastName)
                                    .EmailAddress(objMembers.Email)
                                    .PhoneNumber(objMembers.MobilePhone)
                                    .ReferenceId(MemberId.ToString())
                                    .Build();

                                var customerResponse = await squareClient.CustomersApi.CreateCustomerAsync(createCustomerBody);
                                squareCustomerId = customerResponse.Customer.Id;

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Payment",
                                    LogDescription = $"Square Customer Created: {squareCustomerId} | MemberId:{MemberId}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                // ── Save Card on File ────────────────────────────────────────────
                                var createCardBody = new Square.Models.CreateCardRequest.Builder(
                                        Guid.NewGuid().ToString(),
                                        squareNonce,
                                        new Square.Models.Card.Builder()
                                            .CustomerId(squareCustomerId)
                                            .CardholderName($"{objMembers.FirstName} {objMembers.LastName}")
                                            .Build())
                                    .Build();

                                var cardResponse = await squareClient.CardsApi.CreateCardAsync(createCardBody);
                                squareCardId = cardResponse.Card.Id;  // reusable card_xxx
                                tokenForPayment = squareCardId;           // charge uses card_id, not nonce

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Payment",
                                    LogDescription = $"Card on File saved: {squareCardId} | CustomerId:{squareCustomerId} | MemberId:{MemberId}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
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
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }
                        // ── END ADDED STEP 1 ────────────────────────────────────────────────────

                        // ── EXISTING: charge builder — only change is using tokenForPayment
                        //    instead of squareNonce so recurring uses card_id automatically ──────
                        long amountCents = (long)(objMembers.MemberAmount * 100);
                        var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();
                        // CHANGED: was Builder(squareNonce, ...) — now uses tokenForPayment
                        // so that recurring charges use card_id while one-time still uses nonce
                        var createPaymentBodyBuilder = new Square.Models.CreatePaymentRequest.Builder(
                                tokenForPayment,
                                Guid.NewGuid().ToString(),
                                money)
                            .LocationId(squareLocationId)
                            .Note($"Member Registration - {objMembers.MembershipType} | {objMembers.FirstName} {objMembers.LastName}");

                        // ADDED: link payment to customer when recurring (required for card-on-file charges)
                        if (!string.IsNullOrEmpty(squareCustomerId))
                            createPaymentBodyBuilder = createPaymentBodyBuilder.CustomerId(squareCustomerId);

                        var createPaymentBody = createPaymentBodyBuilder.Build();

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
                                HttpContext.Session.SetString("CTotalAmount", objMembers.MemberAmount.ToString());

                                string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                                    ? objMembers.FirstName + " " + objMembers.LastName
                                    : User.Identity?.Name ?? "";

                                var objMembershipOrders = new Entities.MembershipOrders
                                {
                                    OrderType = "Square",
                                    Amount = objMembers.MemberAmount,
                                    ExpiryDate = objMembers.ExpiryDate,
                                    PaymentMethod = "Square",
                                    TransactionId = squareTransactionId,
                                    PaymentStatus = "Completed",
                                    MemberId = MemberId,
                                    MembershipTypeId = objMembers.MembershipTypeId,
                                    MembershipType = objMembers.MembershipType,
                                    OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                                    UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                                    UpdatedBy = Username
                                };

                                long result5 = _Members.InsertMemberOrder(objMembershipOrders);
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Membership Orders",
                                    LogDescription = "Inserted Square MembershipOrders Details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                int result2 = 0;
                                objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result2);

                                DateTime startDateTimePP = objMembers.objMembershipOrder.ExpiryDate;
                                if (objMembers.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                                {
                                    startDateTimePP = objMembers.objMembershipOrder.ExpiryDate.Date
                                        .AddHours(objMembers.objMembershipOrder.ExpiryDate.Hour)
                                        .AddMinutes(objMembers.objMembershipOrder.ExpiryDate.Minute);
                                }

                                StringBuilder sbPP = new StringBuilder();
                                sbPP.AppendLine("BEGIN:VCALENDAR");
                                sbPP.AppendLine("VERSION:2.0");
                                sbPP.AppendLine("PRODID:-//Membership Registration//EN");
                                sbPP.AppendLine("METHOD:REQUEST");
                                sbPP.AppendLine("BEGIN:VEVENT");
                                sbPP.AppendLine("UID:" + Guid.NewGuid());
                                sbPP.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sbPP.AppendLine("DTSTART:" + startDateTimePP.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                sbPP.AppendLine("SUMMARY:Member - Membership Renewal Notification for a " + objMembers.objMembershipOrder.MembershipType);
                                sbPP.AppendLine("DESCRIPTION:" + objMembers.State);
                                sbPP.AppendLine("City:" + objMembers.City);
                                sbPP.AppendLine("STATUS:CONFIRMED");
                                sbPP.AppendLine("END:VEVENT");
                                sbPP.AppendLine("END:VCALENDAR");

                                byte[] byteArrayPP = Encoding.UTF8.GetBytes(sbPP.ToString());
                                MemoryStream msPP = new MemoryStream(byteArrayPP);
                                Attachment calendarAttachment = new Attachment(msPP, "Membership.ics", "text/calendar");

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = " Membership Order ",
                                    LogDescription = "Updated Member Order details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                if (result5 == 1)
                                {
                                    // ── ADDED: STEP 3 — Create Subscription (only if recurring) ──────
                                    // Customer + card already saved in Step 1 — safe to use here
                                    string recStatus = "", squareSubProfileId = "", squareSubPaymentProfileId = "",
                                           squareSubShippingProfileId = "", squareSubscriptionId = "";

                                    if (isRecurring)
                                    {
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Square Payment",
                                            LogDescription = $"Creating Square Subscription | CustomerId:{squareCustomerId} | CardId:{squareCardId} | MemberId:{MemberId}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });

                                        //DateTime regDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                                        //// ✅ TESTING PURPOSE ONLY — 1 day subscription for quick checking
                                        //bool isTestMode = true; // ⚠️ REMOVE THIS BEFORE PRODUCTION — set false or delete this block

                                        //DateTime julyFirst;
                                        //DateTime juneThirty;

                                        //if (isTestMode)
                                        //{
                                        //    // Test: expiry after just 1 day from registration
                                        //    juneThirty = regDate.AddDays(1);
                                        //    julyFirst = juneThirty.AddDays(1); // next day, kept for consistency with existing variable usage
                                        //}
                                        //else
                                        //{
                                        //    julyFirst = regDate.Month <= 3
                                        //        ? new DateTime(regDate.Year, 7, 1)       // Jan-Mar → this year July 1
                                        //        : new DateTime(regDate.Year + 1, 7, 1);  // Apr-Dec → next year July 1

                                        //    juneThirty = julyFirst.AddDays(-1);
                                        //}

                                        DateTime regDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                                        DateTime julyFirst = regDate.Month <= 3
                                            ? new DateTime(regDate.Year, 7, 1)       // Jan-Mar → this year July 1
                                            : new DateTime(regDate.Year + 1, 7, 1);  // Apr-Dec → next year July 1

                                        DateTime juneThirty = julyFirst.AddDays(-1);


                                        var objDonorProxy = new Entities.Members
                                        {
                                            FirstName = objMembers.FirstName,
                                            LastName = objMembers.LastName,
                                            Email = objMembers.Email,
                                            MobilePhone = objMembers.MobilePhone,
                                            RecurringType = RecurringType,
                                            MemberAmount = objMembers.MemberAmount,
                                            InsertedTime = juneThirty,
                                            MembershipType = objMembers.MembershipType,
                                            //NoOfReccurs = 9999
                                        };

                                        (recStatus, squareSubscriptionId) = await _RecurringPaymentController.CreateSquareSubscriptionAsync(
                                            squareAccessToken,
                                            squareEnvironment,
                                            squareLocationId,
                                            objDonorProxy,
                                            squareCustomerId,
                                            squareCardId);

                                        squareSubProfileId = squareCustomerId;
                                        squareSubPaymentProfileId = squareCardId;

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Square Payment",
                                            LogDescription = $"Square Subscription Result:{recStatus} | SubscriptionId:{squareSubscriptionId} | MemberId:{MemberId}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });


                                        if (!string.IsNullOrEmpty(squareSubscriptionId))
                                        {
                                            _Members.InsertMemberSubscriptions(new Entities.MemberSubscriptions
                                            {
                                                MemberId = MemberId,
                                                SubscriptionId = squareSubscriptionId,
                                                ProfileId = squareSubProfileId,
                                                PaymentProfileId = squareSubPaymentProfileId,
                                                RecurringType = RecurringType,
                                                RecurringAmount = objMembers.MemberAmount,
                                                RecurringStartTime = DateOnly.FromDateTime(juneThirty)
                                            });

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"Subscription IDs saved | SubId:{squareSubscriptionId} | CustId:{squareSubProfileId} | CardId:{squareSubPaymentProfileId}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // One-time payment — no subscription required
                                        recStatus = "";
                                    }
                                    // ── END ADDED STEP 3 ────────────────────────────────────────────

                                    // ── EXISTING: email sending code — completely unchanged ─────────
                                    int _status1PP = 0;
                                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1PP);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                                        body.Replace("[MemberId]", objMembers.SpouseCell);
                                        body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                                        body.Replace("[FirstName]", objMembers.FirstName);
                                        body.Replace("[MemberSkils]", objMembers.MemberSkils);
                                        body.Replace("[LastName]", objMembers.LastName);
                                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                                        body.Replace("[Email]", objMembers.Email);
                                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                                        body.Replace("[MembershipType]", objMembers.MembershipType);
                                        body.Replace("[ChapterName]", objMembers.ChapterName);
                                        body.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                                        body.Replace("[PaymentType]", "Square");
                                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplates.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            if (startDateTimePP == DateTime.MinValue)
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objMembers.Email, objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body.ToString());
                                                //var responsce = _sentmail.SendMailSendinbrevomuiltiple(objMembers.Email, subject, body.ToString());
                                            }
                                            else
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objMembers.Email, objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body.ToString());
                                                //var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);
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
                                    }

                                    int statusAdminPP = 0;
                                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdminPP);
                                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                                    {
                                        StringBuilder body1 = new StringBuilder();
                                        body1.Append(objTemplatesAdmin.Description);
                                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                        body1.Replace("[MemberId]", objMembers.SpouseCell);
                                        body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                                        body1.Replace("[FirstName]", objMembers.FirstName);
                                        body1.Replace("[LastName]", objMembers.LastName);
                                        body1.Replace("[Email]", objMembers.Email);
                                        body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                                        body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                                        body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                                        body1.Replace("[ChapterName]", objMembers.ChapterName);
                                        body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                                        body1.Replace("[MembershipType]", objMembers.MembershipType);
                                        body1.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                                        body1.Replace("[PaymentType]", "Square");
                                        body1.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                                        body1.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplatesAdmin.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                        if (objAppInfo.Email == "Yes")
                                        {
                                            if (startDateTimePP == DateTime.MinValue)
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body1.ToString());
                                                //var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.MemberEmail, subject, body1.ToString());
                                            }
                                            else
                                            {
                                                var responsce = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body1.ToString());
                                                //var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString(), calendarAttachment);
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
                                    }

                                    return RedirectToAction("Acknowledgement", "Members", new { MemberId = MemberId, PaymentMethod = "Square", amt = objMembers.MemberAmount });
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
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            return RedirectToAction("PaymentFailed", "Home");
                        }
                        catch (Exception ex)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Charge FAILED",
                                LogDescription = $"Unexpected error: {ex.Message}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            TempData["message"] = "<div class=\"error1 closable\">An unexpected error occurred. Please try again.</div>";
                            return RedirectToAction("PaymentFailed", "Home");
                        }
                    }

                    else if (objMembers.PaymentMethod == "Credit Card")
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
                            Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();

                            // ✅ 1. Get Amount safely
                            decimal amount = objMembers.MemberAmount;

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
                                objMembershipOrders.PaymentMethod = "Credit Card";
                                objMembershipOrders.TransactionId = transaction.Id;
                                objMembershipOrders.PaymentStatus = "Completed";
                                objMembershipOrders.MemberId = MemberId;
                                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                                objMembershipOrders.MembershipType = objMembers.MembershipType;
                                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                                // objMembershipOrders.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


                                objMembershipOrders.UpdatedBy = objMembers.FirstName;

                                if (objMembers.Donate == "Yes" && objMembers.DonationAmount != 0)
                                {
                                    objMembershipOrders.Amount = objMembers.MemberAmount - objMembers.DonationAmount;
                                }
                                else
                                {
                                    objMembershipOrders.Amount = objMembers.MemberAmount;
                                }


                                Int64 result1 = _Members.InsertMemberOrder(objMembershipOrders);
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Credit Card Payment",
                                    LogDescription = "Credit Card MemberOrder Inserted ",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                if (result1 == 1)
                                {
                                    int status = 0;
                                    Entities.Members objMember = _Members.GetMemberFullDetailsByEmail(objMembers.Email, ref status);
                                    DateTime startDateTime = objMember.objMembershipOrder.ExpiryDate;

                                    if (objMember.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                                    {
                                        startDateTime = objMember.objMembershipOrder.ExpiryDate.Date
                                               .AddHours(objMember.objMembershipOrder.ExpiryDate.Hour)
                                               .AddMinutes(objMember.objMembershipOrder.ExpiryDate.Minute);
                                    }

                                    // Step 2: Generate iCalendar content
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("BEGIN:VCALENDAR");
                                    sb.AppendLine("VERSION:2.0");
                                    //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                                    sb.AppendLine("PRODID:-//Membership Registration//EN");
                                    sb.AppendLine("METHOD:REQUEST");
                                    sb.AppendLine("BEGIN:VEVENT");
                                    sb.AppendLine("UID:" + Guid.NewGuid());
                                    sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    //sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                                    // sb.AppendLine("SUMMARY:Member - " + Membership Renewal Notification for a objMember.objMembershipOrder.MembershipType);
                                    sb.AppendLine("SUMMARY:Member - " + "Membership Renewal Notification for a " + objMember.objMembershipOrder.MembershipType);
                                    sb.AppendLine("DESCRIPTION:" + objMember.State);
                                    sb.AppendLine("City:" + objMember.City);
                                    // sb.AppendLine("LOCATION:Temple");
                                    sb.AppendLine("STATUS:CONFIRMED");
                                    sb.AppendLine("END:VEVENT");
                                    sb.AppendLine("END:VCALENDAR");

                                    string icsContent = sb.ToString();

                                    // Step 3: Convert to byte stream
                                    byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                                    MemoryStream stream = new MemoryStream(byteArray);
                                    Attachment calendarAttachment = new Attachment(stream, "Membership.ics", "text/calendar");
                                    int _status1 = 0;
                                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                                        body.Replace("[MemberId]", objMembers.SpouseCell);

                                        body.Replace("[MEMBERID]", MemberId.ToString());
                                        body.Replace("[FirstName]", objMembers.FirstName);
                                        body.Replace("[LastName]", objMembers.LastName);
                                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                                        body.Replace("[Email]", objMembers.Email);
                                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                                        body.Replace("[MembershipType]", objMembers.MembershipType);
                                        body.Replace("[TransactionId]", objMembers.TransactionId);
                                        body.Replace("[PaymentType]", "Credit Card");
                                        body.Replace("[ChapterName]", objMember.ChapterName);
                                        body.Replace("[MemberSkils]", objMember.MemberSkils);
                                        body.Replace("[PaymentStatus]", "Completed");
                                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[Password]", Password);
                                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplates.Subject;
                                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);

                                        if (objAppInfo.Email == "Yes")
                                        {
                                            if (startDateTime == DateTime.MinValue)
                                            {

                                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString());


                                                logs = new Entities.LogReport();
                                                logs.LogTitle = "Members";
                                                logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                                lstLogReport.Add(logs);
                                            }
                                            else
                                            {

                                                var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);

                                                logs = new Entities.LogReport();
                                                logs.LogTitle = "Members";
                                                logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                                lstLogReport.Add(logs);

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
                                    }

                                    int statusAdmin = 0;
                                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
                                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                                    {
                                        StringBuilder body1 = new StringBuilder();
                                        body1.Append(objTemplatesAdmin.Description);
                                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                        body1.Replace("[MemberId]", objMembers.SpouseCell);
                                        body1.Replace("[MEMBERID]", MemberId.ToString());
                                        body1.Replace("[FirstName]", objMembers.FirstName);
                                        body1.Replace("[LastName]", objMembers.LastName);
                                        body1.Replace("[Email]", objMembers.Email);
                                        body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                                        body1.Replace("[ChapterName]", objMember.ChapterName);
                                        body1.Replace("[MemberSkils]", objMember.MemberSkils);
                                        body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body1.Replace("[SpouseCell]", objMembers.SpouseCell);

                                        body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                                        body1.Replace("[MembershipType]", objMembers.MembershipType);
                                        body1.Replace("[TransactionId]", objMembers.TransactionId);
                                        body1.Replace("[PaymentType]", "Credit Card");
                                        body1.Replace("[PaymentStatus]", "Completed");
                                        body1.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToShortDateString());
                                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                                        string subject = objTemplates.Subject;
                                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                                        if (objAppInfo.Email == "Yes")
                                        {


                                            if (startDateTime == DateTime.MinValue)
                                            {

                                                var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString());


                                                logs = new Entities.LogReport();
                                                logs.LogTitle = "Members";
                                                logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                                lstLogReport.Add(logs);
                                            }


                                            else
                                            {

                                                var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString(), calendarAttachment);

                                                logs = new Entities.LogReport();
                                                logs.LogTitle = "Members";
                                                logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                                lstLogReport.Add(logs);

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
                                    }

                                    return RedirectToAction("Acknowledgement", "Members", new { MemberId = MemberId, PaymentMethod = "Credit Card", cname = cname });
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
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
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
                    // ── AuthorizeNet ──────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Bank/Transfer")
                    {

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Bank Transfer Members Payment",
                            LogDescription = "Entered Into Bank Transfer Member Registration",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        DateTime regDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        DateTime julyFirst = regDate.Month <= 3
                            ? new DateTime(regDate.Year, 7, 1)       // Jan-Mar → this year July 1
                            : new DateTime(regDate.Year + 1, 7, 1);  // Apr-Dec → next year July 1

                        DateTime juneThirty = julyFirst.AddDays(-1);

                        Int64 result = 0;
                        Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = juneThirty;
                        objMembershipOrders.PaymentMethod = "Bank/Transfer";
                        objMembershipOrders.PaymentStatus = "Pending";
                        objMembershipOrders.MemberId = MemberId;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.UserName;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Members.InsertMemberOrder(objMembershipOrders);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders",
                            LogDescription = "Inserted MembershipOrders Details Successfully",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int statusCC = 0;
                        int result2 = 0;
                        Entities.Members objMember = _Members.GetMemberFullDetailsByEmail(objMembers.Email, ref statusCC);
                        DateTime startDateTime = objMember.objMembershipOrder.ExpiryDate;
                        if (objMember.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                        {
                            startDateTime = objMember.objMembershipOrder.ExpiryDate.Date
                                .AddHours(objMember.objMembershipOrder.ExpiryDate.Hour)
                                .AddMinutes(objMember.objMembershipOrder.ExpiryDate.Minute);
                        }
                        StringBuilder sbCC = new StringBuilder();
                        sbCC.AppendLine("BEGIN:VCALENDAR");
                        sbCC.AppendLine("VERSION:2.0");
                        sbCC.AppendLine("PRODID:-//Membership Registration//EN");
                        sbCC.AppendLine("METHOD:REQUEST");
                        sbCC.AppendLine("BEGIN:VEVENT");
                        sbCC.AppendLine("UID:" + Guid.NewGuid());
                        sbCC.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbCC.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbCC.AppendLine("SUMMARY:Member - Membership Renewal Notification for a " + objMember.objMembershipOrder.MembershipType);
                        sbCC.AppendLine("DESCRIPTION:" + objMember.State);
                        sbCC.AppendLine("City:" + objMember.City);
                        sbCC.AppendLine("STATUS:CONFIRMED");
                        sbCC.AppendLine("END:VEVENT");
                        sbCC.AppendLine("END:VCALENDAR");

                        byte[] byteArrayCC = Encoding.UTF8.GetBytes(sbCC.ToString());
                        MemoryStream msCC = new MemoryStream(byteArrayCC);
                        Attachment calendarAttachment = new Attachment(msCC, "Membership.ics", "text/calendar");
                        objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result2);
                        int _status1PP = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1PP);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                            body.Replace("[MemberId]", objMembers.SpouseCell);
                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body.Replace("[FirstName]", objMembers.FirstName);
                            body.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body.Replace("[LastName]", objMembers.LastName);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body.Replace("[Email]", objMembers.Email);
                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body.Replace("[MembershipType]", objMembers.MembershipType);
                            body.Replace("[ChapterName]", objMembers.ChapterName);
                            body.Replace("[TransactionId]", "N/A");
                            body.Replace("[PaymentType]", "Bank/Transfer");
                            body.Replace("[PaymentStatus]", "Pending");
                            body.Replace("[PaymentDate]", DateTime.Now.ToString("MM/dd/yyyy"));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplates.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTime == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objMembers.Email, objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body.ToString());
                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objMembers.Email, objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body.ToString());

                                    //var responsce = _sentmail.SendMailWithAttachmentICOmuiltiple(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);
                                    //var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);
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
                        }

                        int statusAdminPP = 0;
                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdminPP);
                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplatesAdmin.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[MemberId]", objMembers.SpouseCell);
                            body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body1.Replace("[FirstName]", objMembers.FirstName);
                            body1.Replace("[LastName]", objMembers.LastName);
                            body1.Replace("[Email]", objMembers.Email);
                            body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                            body1.Replace("[ChapterName]", objMembers.ChapterName);
                            body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body1.Replace("[MembershipType]", objMembers.MembershipType);
                            body1.Replace("[TransactionId]", "N/A");
                            body1.Replace("[PaymentType]", "Bank/Transfer");
                            body1.Replace("[PaymentDate]", DateTime.Now.ToString("MM/dd/yyyy"));
                            body1.Replace("[PaymentStatus]", "Pending");

                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                            body1.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplatesAdmin.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTime == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body1.ToString());
                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfo.MemberEmail, subject + objAppInfo.SiteName, body1.ToString());

                                    //var responsce = _sentmail.SendMailWithAttachmentICOmuiltiple(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString(), calendarAttachment);
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
                        }

                        return RedirectToAction("Acknowledgement", "Members", new { MemberId = MemberId, PaymentMethod = "Bank/Transfer", amt = objMembers.MemberAmount });

                    }

                    // ── PayPal ────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "PayPal")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PayPal Payment",
                            LogDescription = "PayPal Payment started",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                            ? objMembers.FirstName + " " + objMembers.LastName
                            : User.Identity?.Name ?? "";

                        Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                        objMembershipOrders.OrderType = "Paypal";
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "PayPal";
                        objMembershipOrders.TransactionId = objMembers.TransactionId;
                        objMembershipOrders.PaymentStatus = "Completed";
                        objMembershipOrders.MemberId = MemberId;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.MembershipType = objMembers.MembershipType;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedBy = Username;

                        _Members.InsertMemberOrder(objMembershipOrders);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Membership Orders",
                            LogDescription = "Inserted Paypal MembershipOrders Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int result2 = 0;
                        objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result2);

                        DateTime startDateTimePP = objMembers.objMembershipOrder.ExpiryDate;
                        if (objMembers.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                        {
                            startDateTimePP = objMembers.objMembershipOrder.ExpiryDate.Date
                                .AddHours(objMembers.objMembershipOrder.ExpiryDate.Hour)
                                .AddMinutes(objMembers.objMembershipOrder.ExpiryDate.Minute);
                        }

                        StringBuilder sbPP = new StringBuilder();
                        sbPP.AppendLine("BEGIN:VCALENDAR");
                        sbPP.AppendLine("VERSION:2.0");
                        sbPP.AppendLine("PRODID:-//Membership Registration//EN");
                        sbPP.AppendLine("METHOD:REQUEST");
                        sbPP.AppendLine("BEGIN:VEVENT");
                        sbPP.AppendLine("UID:" + Guid.NewGuid());
                        sbPP.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbPP.AppendLine("DTSTART:" + startDateTimePP.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbPP.AppendLine("SUMMARY:Member - Membership Renewal Notification for a " + objMembers.objMembershipOrder.MembershipType);
                        sbPP.AppendLine("DESCRIPTION:" + objMembers.State);
                        sbPP.AppendLine("City:" + objMembers.City);
                        sbPP.AppendLine("STATUS:CONFIRMED");
                        sbPP.AppendLine("END:VEVENT");
                        sbPP.AppendLine("END:VCALENDAR");

                        byte[] byteArrayPP = Encoding.UTF8.GetBytes(sbPP.ToString());
                        MemoryStream msPP = new MemoryStream(byteArrayPP);
                        Attachment calendarAttachment = new Attachment(msPP, "Membership.ics", "text/calendar");


                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = " Membership Order ",
                            LogDescription = "Updated Member Order details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        int _status1PP = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1PP);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                            body.Replace("[MemberId]", objMembers.SpouseCell);
                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body.Replace("[FirstName]", objMembers.FirstName);
                            body.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body.Replace("[LastName]", objMembers.LastName);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body.Replace("[Email]", objMembers.Email);
                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body.Replace("[MembershipType]", objMembers.MembershipType);
                            body.Replace("[ChapterName]", objMembers.ChapterName);
                            body.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                            body.Replace("[PaymentType]", "PayPal");
                            body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                            body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplates.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTimePP == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevo(objMembers.Email, subject, body.ToString());

                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);

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
                        }

                        int statusAdminPP = 0;
                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdminPP);
                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplatesAdmin.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[MemberId]", objMembers.SpouseCell);
                            body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body1.Replace("[FirstName]", objMembers.FirstName);
                            body1.Replace("[LastName]", objMembers.LastName);
                            body1.Replace("[Email]", objMembers.Email);
                            body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                            body1.Replace("[ChapterName]", objMembers.ChapterName);
                            body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body1.Replace("[MembershipType]", objMembers.MembershipType);
                            body1.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                            body1.Replace("[PaymentType]", "PayPal");
                            body1.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                            body1.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                            body1.Replace("[Year]", DateTime.Now.Year.ToString());

                            string subject = objTemplatesAdmin.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTimePP == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.MemberEmail, subject, body1.ToString());

                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString(), calendarAttachment);
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
                            var redirectUrl = Url.Action("Acknowledgement", "Members", new
                            {
                                area = "User",
                                MemberId = MemberId,
                                cname = cname,
                                PaymentMethod = objMembers.PaymentMethod,
                                tid = objMembers.TransactionId
                            });
                            return Json(new { success = true, redirectUrl = redirectUrl });
                            //return RedirectToAction("Acknowledgement", "Members", new { MemberId = MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
                        }
                    }

                    // ── Zelle ─────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Zelle")
                    {

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Zelle Payment",
                            LogDescription = $"Entered into Zelle Member Registration",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        Int64 result = 0;
                        Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "Zelle";
                        objMembershipOrders.PaymentStatus = "Pending";
                        objMembershipOrders.MemberId = MemberId;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.UserName;
                        objMembershipOrders.TransactionId = objMembers.TransactionId;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Members.InsertMemberOrder(objMembershipOrders);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders",
                            LogDescription = "Inserted Zelle MembershipOrders Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int statusZ = 0;
                        Entities.Members objMember = _Members.GetMemberFullDetailsByEmail(objMembers.Email, ref statusZ);

                        DateTime startDateTimeZ = objMember.objMembershipOrder.ExpiryDate;
                        if (objMember.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                        {
                            startDateTimeZ = objMember.objMembershipOrder.ExpiryDate.Date
                                .AddHours(objMember.objMembershipOrder.ExpiryDate.Hour)
                                .AddMinutes(objMember.objMembershipOrder.ExpiryDate.Minute);
                        }

                        StringBuilder sbZ = new StringBuilder();
                        sbZ.AppendLine("BEGIN:VCALENDAR");
                        sbZ.AppendLine("VERSION:2.0");
                        sbZ.AppendLine("PRODID:-//Membership Registration//EN");
                        sbZ.AppendLine("METHOD:REQUEST");
                        sbZ.AppendLine("BEGIN:VEVENT");
                        sbZ.AppendLine("UID:" + Guid.NewGuid());
                        sbZ.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbZ.AppendLine("DTSTART:" + startDateTimeZ.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                        sbZ.AppendLine("SUMMARY:Member - Membership Renewal Notification for a " + objMember.objMembershipOrder.MembershipType);
                        sbZ.AppendLine("DESCRIPTION:" + objMember.State);
                        sbZ.AppendLine("City:" + objMember.City);
                        sbZ.AppendLine("STATUS:CONFIRMED");
                        sbZ.AppendLine("END:VEVENT");
                        sbZ.AppendLine("END:VCALENDAR");

                        byte[] byteArrayZ = Encoding.UTF8.GetBytes(sbZ.ToString());
                        MemoryStream msZ = new MemoryStream(byteArrayZ);
                        Attachment calendarAttachment = new Attachment(msZ, "Membership.ics", "text/calendar");

                        int _status1Z = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1Z);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMember.FirstName + " " + objMember.LastName));
                            body.Replace("[MemberId]", objMember.SpouseCell);
                            body.Replace("[MEMBERID]", MemberId.ToString());
                            body.Replace("[FirstName]", objMember.FirstName);
                            body.Replace("[LastName]", objMember.LastName);
                            body.Replace("[MemberSkils]", objMember.MemberSkils);
                            body.Replace("[ChapterName]", objMember.ChapterName);
                            body.Replace("[IntrestedArea]", objMember.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMember.SpouseEmail);
                            body.Replace("[Email]", objMember.Email);
                            body.Replace("[MobilePhone]", objMember.MobilePhone);
                            body.Replace("[MembershipType]", objMember.MembershipType);
                            body.Replace("[TransactionId]", "NA");
                            body.Replace("[PaymentType]", "Zelle");
                            body.Replace("[PaymentStatus]", "Pending");
                            body.Replace("[PaymentDate]", objMember.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Password]", Password);

                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTimeZ == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());
                                    if (Notifications == "Yes") { int[] arr = { 3 }; BLL.Common.AddContactList(objMembers.Email, objMembers.FirstName, objMembers.LastName, arr, objMembers.MobilePhone); }
                                    logs = new Entities.LogReport(); logs.LogTitle = "Members";
                                    logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                    lstLogReport.Add(logs);
                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString(), calendarAttachment);
                                    if (Notifications == "Yes") { int[] arr = { 3 }; BLL.Common.AddContactList(objMembers.Email, objMembers.FirstName, objMembers.LastName, arr, objMembers.MobilePhone); }
                                    logs = new Entities.LogReport(); logs.LogTitle = "Members";
                                    logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                    lstLogReport.Add(logs);
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
                        }

                        int statusAdminZ = 0;
                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdminZ);
                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplatesAdmin.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[MemberId]", objMember.SpouseCell);
                            body1.Replace("[MEMBERID]", MemberId.ToString());
                            body1.Replace("[FirstName]", objMember.FirstName);
                            body1.Replace("[LastName]", objMember.LastName);
                            body1.Replace("[Email]", objMember.Email);
                            body1.Replace("[ChapterName]", objMember.ChapterName);
                            body1.Replace("[MemberSkils]", objMember.MemberSkils);
                            body1.Replace("[MobilePhone]", objMember.MobilePhone);
                            body1.Replace("[IntrestedArea]", objMember.SpouseSkils);
                            body1.Replace("[SpouseCell]", objMember.SpouseCell);
                            body1.Replace("[SpouseEmail]", objMember.SpouseEmail);
                            body1.Replace("[MembershipType]", objMember.MembershipType);
                            body1.Replace("[TransactionId]", "NA");
                            body1.Replace("[PaymentType]", "Zelle");
                            body1.Replace("[PaymentStatus]", "Pending");
                            body1.Replace("[PaymentDate]", objMember.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));
                            body1.Replace("[SiteName]", objAppInfo.SiteName);

                            if (objAppInfo.Email == "Yes")
                            {
                                if (startDateTimeZ == DateTime.MinValue)
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());
                                    logs = new Entities.LogReport();
                                    logs.LogTitle = "Members";
                                    logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                    lstLogReport.Add(logs);
                                }
                                else
                                {
                                    var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString(), calendarAttachment);
                                    logs = new Entities.LogReport();
                                    logs.LogTitle = "Members";
                                    logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + MemberId + " - " + DateTime.UtcNow + ")";
                                    lstLogReport.Add(logs);

                                }
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Admin Email Sent",
                                    LogDescription = $"Admin notification email sent to: {objAppInfo.MemberEmail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                        }

                        if (result == 1)
                        {
                            return RedirectToAction("Acknowledgement", "Members", new { area = "User", MemberId = MemberId, PaymentMethod = "Zelle", cname = cname });
                        }
                    }

                    // ── Stripe ────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Stripe")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Stripe Payment",
                            LogDescription = "Entered into Stripe Member Registration",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        Int64 result = 0;
                        Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "Stripe";
                        objMembershipOrders.MemberId = MemberId;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.UserName;
                        objMembershipOrders.TransactionId = objMembers.TransactionId;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Members.InsertMemberOrder(objMembershipOrders);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders",
                            LogDescription = "Updated Stripe MembershipOrders Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        int status2 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);
                        StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;

                        var priceOptions = new PriceCreateOptions
                        {
                            UnitAmount = (long)(objMembers.MemberAmount * 100),
                            Currency = "usd",
                            ProductData = new PriceProductDataOptions { Name = "Membership" }
                        };
                        var price = new PriceService().Create(priceOptions);

                        string baseUrl = $"{Request.Scheme}://{Request.Host}";
                        var sessionOptions = new SessionCreateOptions
                        {
                            PaymentMethodTypes = new List<string> { "card" },
                            LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
                    },
                            Mode = "payment",
                            SuccessUrl = $"{baseUrl}/User/Members/Acknowledgement?session_id={{CHECKOUT_SESSION_ID}}&MemberId={MemberId}&PaymentMethod=Stripe&cname={cname}",
                            CancelUrl = $"{baseUrl}/User/Members/Acknowledgement"
                        };

                        var session = new SessionService().Create(sessionOptions);

                        // Core cookie
                        Response.Cookies.Append("StripeSessionId", session.Id);

                        if (result == 1)
                        {
                            if (!string.IsNullOrEmpty(session.Url))
                                return Redirect(session.Url);
                        }
                    }

                    // ── Swire Pay ─────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Swire Pay")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Swire Pay Payment",
                            LogDescription = $"Entered into Swire Pay Members Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int status2 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);



                        string redirectUri = Url.Action("Acknowledgement", "Members",
                            new { area = "User", MemberId = MemberId, PaymentMethod = "Swire Pay", cname = cname }, Request.Scheme)!;

                        decimal amountInUSD = objMembers.MemberAmount * 100;

                        var jsonContent = $@"{{
                    ""amount"": ""{amountInUSD}"",
                    ""currencyCode"": ""USD"",
                    ""paymentMethodType"": [""CARD""],
                    ""redirectUri"": ""{redirectUri}"",
                    ""description"": ""Membership payment for {objMembers.UserName}"",
                    ""statementDescriptor"": ""Membership Payment"",
                    ""expiresAt"": ""{DateTime.UtcNow.AddDays(7):yyyy-MM-ddTHH:mm:ss.fffZ}"",
                    ""email"": ""{objMembers.Email}"",
                    ""phone"": ""{objMembers.MobilePhone}"",
                    ""name"": ""{objMembers.UserName}""
                }}";

                        using var client1 = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");
                        request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);
                        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await client1.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        var responseBody = await response.Content.ReadAsStringAsync();
                        var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                        var paymentLink = jsonData["entity"]?["link"]?.ToString();

                        logs = new Entities.LogReport();
                        logs.LogTitle = "Members"; logs.LogDescription = paymentLink; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
                        lstLogReport.Add(logs);

                        if (!string.IsNullOrEmpty(paymentLink))
                            return Redirect(paymentLink);
                        else
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Members",
                                LogDescription = $"Swire Pay Payment link not found",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            return RedirectToAction("Acknowledgement", "Members", new { area = "User", MemberId = MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
                        }
                    }
                }

                // ── Free Membership ───────────────────────────────────────────────
                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Membership Free Entering ",
                        LogDescription = "Membership Amount is Zero Free Enterng inserted.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    Int64 result = 0;
                    Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                    objMembershipOrders.Amount = objMembers.MemberAmount;
                    objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                    objMembershipOrders.PaymentMethod = "Free";
                    objMembershipOrders.PaymentStatus = "Pending";
                    objMembershipOrders.MemberId = MemberId;
                    objMembershipOrders.PaymentBy = "N/A";
                    objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                    objMembershipOrders.UpdatedBy = objMembers.UserName;
                    objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    result = _Members.InsertMemberOrder(objMembershipOrders);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "MembershipOrders  ",
                        LogDescription = "MembershipOrders Amount is Zero Free Inserted.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    int fStatus = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank You for Free Membership", 0, ref fStatus);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmailId]", objAppInfo.CompanyEmail);
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                        body.Replace("[MemberId]", objMembers.SpouseCell);
                        body.Replace("[MEMBERID]", MemberId.ToString());
                        body.Replace("[UserName]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                        body.Replace("[FirstName]", objMembers.FirstName);
                        body.Replace("[LastName]", objMembers.LastName);
                        body.Replace("[Email]", objMembers.Email);
                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                        body.Replace("[MembershipType]", objMembers.MembershipType);
                        body.Replace("[TransactionId]", objMembers.TransactionId);
                        body.Replace("[PaymentType]", "Free");
                        body.Replace("[PaymentStatus]", objMembers.PaymentStatus);
                        body.Replace("[Password]", Password);
                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                            _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString());

                        int[] arr = { 3 };
                        BLL.Common.AddContactList(objMembers.Email, objMembers.FirstName, objMembers.LastName, arr, objMembers.MobilePhone);
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

                    return RedirectToAction("Acknowledgement", "Members", new { cname = cname });
                }

                TempData["message"] = "<div class=\"success closable\">Your registration completed successfully and our team will contact soon.</div>";
                return RedirectToAction("Thankyou", "Members", new { cname = cname });
            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Acknowledgement", "Members", new { cname = cname });
            }
        }


        //public IActionResult PaymentCancelled(long id = 0)
        //{
        //    string filename = "Log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";

        //    // ✅ Correct path in ASP.NET Core
        //    string folderPath = Path.Combine(_env.WebRootPath, "Content", "logfiles");

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    string filepath = Path.Combine(folderPath, filename);

        //    logreport("Payment failed", Request.Path, filepath);

        //    int result1 = 0;
        //    Entities.Members objMembers = new Entities.Members();
        //    var objMembershipOrders = new MembershipOrders();
        //    string status = "";

        //    try
        //    {
        //        objMembers = _Members.GetMemberFullDetailsById(id, ref result1);

        //        int qstatus = 0;
        //        var objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref qstatus);

        //        // ⚠️ OLD PayPal logic (keep temporarily or remove later)
        //        objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;

        //        var objPDT = objPayByPayPal.objPDTHolder;

        //        objMembershipOrders.PaymentStatus = "Cancelled";
        //        objMembershipOrders.PaymentMethod = "PayPal";
        //        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //        objMembershipOrders.UpdatedBy = objMembers.FirstName;
        //        objMembershipOrders.MemberId = objMembers.MemberId;
        //        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //        objMembershipOrders.Amount = objMembers.MemberAmount;

        //        long result = _Members.InsertMemberOrder(objMembershipOrders);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "Your payment did not go through. Please contact board@teluguassociationtoolweb.org for assistance.";

        //        // Optional: log error
        //        logreport(ex.Message, Request.Path, filepath);
        //    }

        //    ViewBag.status = status;

        //    return View();
        //}
        // PayPal IPN
        [HttpPost]
        //public ActionResult MemberIPN(Int64 id = 0)
        //{
        //    int result1 = 0;
        //    Entities.Members objMembers = new Entities.Members();
        //    PayByPayPal.PDTHolder objPDT = new PayByPayPal.PDTHolder();
        //    Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //    objMembers = _Members.GetMemberFullDetailsById(id, ref result1);
        //    string status = "";
        //    try
        //    {
        //        int qstatus = 0;
        //        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "PayPal", ref qstatus);
        //        objPayByPayPal.objPaymentSettings.PaymentUrl = objPaymentSettings.PaymentUrl;
        //        status = await objPayByPayPal.Ipn(formData);
        //            if (status == "VERIFIED")
        //        {
        //            objPDT = objPayByPayPal.objPDTHolder;
        //            objMembershipOrders.PaymentMethod = "PayPal";
        //            objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //            objMembershipOrders.TransactionId = objPDT.TransactionId;
        //            objMembershipOrders.PaymentStatus = objPDT.PaymentStatus;
        //            objMembershipOrders.UpdatedBy = objMembers.FirstName;
        //            objMembershipOrders.MemberId = objMembers.MemberId;
        //            objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //            objMembershipOrders.Amount = Convert.ToDecimal(objPDT.GrossTotal);

        //            Int64 result = _Members.InsertMemberOrder(objMembershipOrders);
        //        }

        //    }
        //    catch
        //    {
        //        TempData["message"] = "Your payment did not go through please contact  board@TeluguAssociationToolweb.org for assistance.";

        //    }

        //    ViewBag.status = status;

        //    return View();
        //}
        // PayPal Acknowledgement
        [HttpGet]
        public async Task<ActionResult> Acknowledgement(Int64 MemberId = 0, string cname = "", string PaymentMethod = "PayPal", string tid = "", string pstatus = "", string session_id = "", string spPaymentLink = "")
        {
            int status1 = 0;
            string Username = "";
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            Entities.LogReport logs = new Entities.LogReport();

            DateTime paydate = DateTime.UtcNow;
            Entities.Members objMemberss = new Entities.Members();
            int result2 = 0;
            objMemberss = _Members.GetMemberFullDetailsById(MemberId, ref result2);
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "Membership Registration",
                LogDescription = "Members Acknowledgement Page Entering",
                LogDate = DateTime.Now,
                InsertedBy = objMemberss.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objMemberss.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            try
            {
                if (PaymentMethod == "Stripe")
                {
                    int status2 = 0;
                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Stripe Payment",
                        LogDescription = "Donation Acknowledgement  Stripe PaymentMethod  getting payment details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
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

                    var sessionServiceInstance = new SessionService();
                    var session = sessionServiceInstance.Get(session_id);

                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);


                    Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();

                    objMembershipOrders.PaymentMethod = PaymentMethod;
                    objMembershipOrders.TransactionId = paymentIntent.Id;
                    objMembershipOrders.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                    objMembershipOrders.MemberId = MemberId;
                    objMembershipOrders.UpdatedBy = Username;
                    objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "MembershipOrders",
                        LogDescription = "Updated MembershipOrders Transaction details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });


                }
                else if (PaymentMethod == "Swire Pay")

                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Swire pay Payment",
                        LogDescription = "Members Acknowledgement Swire pay PaymentMethod Entering",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    // Validate query string parameter
                    spPaymentLink = Request.Query["sp-payment-link"].ToString();

                    if (string.IsNullOrWhiteSpace(spPaymentLink))
                        throw new ArgumentException("Missing 'sp-payment-link' query parameter.");


                    // Fetch payment settings
                    int status2 = 0;
                    var objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

                    if (objPaymentSettings == null || string.IsNullOrWhiteSpace(objPaymentSettings.PaymentUrl))
                        throw new InvalidOperationException("Swire Pay API key could not be retrieved from payment settings.");

                    // Call Swire Pay verify API
                    var verifyUrl = $"https://api.swirepay.com/v1/payment-link/{spPaymentLink}/verify";

                    using var httpClient = new HttpClient();
                    using var httpRequest = new HttpRequestMessage(HttpMethod.Get, verifyUrl);
                    httpRequest.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);

                    var response = await httpClient.SendAsync(httpRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException(
                            $"Swire Pay API returned [{(int)response.StatusCode}]: {errorBody}");
                    }

                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonData = JObject.Parse(responseData);

                    // Extract payment session
                    var paymentSession = jsonData["entity"]?["paymentSession"];
                    var receiptNumber = paymentSession["receiptNumber"]?.ToString();
                    var paymentType = paymentSession["paymentType"]?.ToString();
                    var paymentStatus = paymentSession["status"]?.ToString();

                    pstatus = paymentType;
                    tid = receiptNumber;


                    // Update membership order
                    var adjustedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    var objMembershipOrders = new Entities.MembershipOrders
                    {
                        PaymentMethod = PaymentMethod,
                        TransactionId = receiptNumber,
                        PaymentStatus = paymentStatus == "SUCCEEDED" ? "Completed" : "Pending",
                        MemberId = MemberId,
                        UpdatedBy = Username,
                        UpdatedTime = adjustedTime,
                        OrderDate = adjustedTime
                    };

                    Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Members",
                        LogDescription = "Updated Transaction details",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                }
                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Members Acknowledgement",
                        LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                }
                Entities.Members objMembers = new Entities.Members();
                int result1 = 0;
                objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result1);

                DateTime startDateTime = objMembers.objMembershipOrder.ExpiryDate;

                if (objMembers.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                {
                    startDateTime = objMembers.objMembershipOrder.ExpiryDate.Date
                           .AddHours(objMembers.objMembershipOrder.ExpiryDate.Hour)
                           .AddMinutes(objMembers.objMembershipOrder.ExpiryDate.Minute);
                }

                // Step 2: Generate iCalendar content
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("VERSION:2.0");
                //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                sb.AppendLine("PRODID:-//Membership Registration//EN");

                sb.AppendLine("METHOD:REQUEST");
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                //sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                // sb.AppendLine("SUMMARY:Member - " + Membership Renewal Notification for a objMember.objMembershipOrder.MembershipType);

                sb.AppendLine("SUMMARY:Member - " + "Membership Renewal Notification for a " + objMembers.objMembershipOrder.MembershipType);


                sb.AppendLine("DESCRIPTION:" + objMembers.State);
                sb.AppendLine("City:" + objMembers.City);
                // sb.AppendLine("LOCATION:Temple");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("END:VEVENT");
                sb.AppendLine("END:VCALENDAR");

                string icsContent = sb.ToString();

                // Step 3: Convert to byte stream
                byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                MemoryStream stream = new MemoryStream(byteArray);
                Attachment calendarAttachment = new Attachment(stream, "Membership.ics", "text/calendar");


                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
                if (PaymentMethod == "Stripe" || PaymentMethod == "Swire Pay")
                {
                    int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
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
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                        body.Replace("[MemberId]", objMembers.SpouseCell);
                        body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                        body.Replace("[FirstName]", objMembers.FirstName);
                        body.Replace("[LastName]", objMembers.LastName);
                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                        body.Replace("[Email]", objMembers.Email);
                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                        body.Replace("[MemberSkils]", objMembers.MemberSkils);
                        body.Replace("[MembershipType]", objMembers.MembershipType);
                        body.Replace("[ChapterName]", objMembers.ChapterName);
                        body.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                        body.Replace("[PaymentType]", objMembers.objMembershipOrder.PaymentMethod);
                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body.Replace("[SiteName]", objAppInfo.SiteName);

                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                        {

                            if (startDateTime == DateTime.MinValue)
                            {
                                var responsce = _sentmail.SendMailSendinbrevo(objMembers.Email, subject, body.ToString());

                            }
                            else
                            {

                                var responsce = _sentmail.SendMailWithAttachmentICO(objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString(), calendarAttachment);


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

                    }

                    int statusAdmin = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                    {
                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplatesAdmin.Description);
                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body1.Replace("[MemberId]", objMembers.SpouseCell);
                        body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                        body1.Replace("[FirstName]", objMembers.FirstName);
                        body1.Replace("[LastName]", objMembers.LastName);
                        body1.Replace("[Email]", objMembers.Email);
                        body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                        body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                        body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                        body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                        body1.Replace("[ChapterName]", objMembers.ChapterName);
                        body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                        body1.Replace("[MembershipType]", objMembers.MembershipType);
                        body1.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                        body1.Replace("[PaymentType]", objMembers.objMembershipOrder.PaymentMethod);
                        body1.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                        body1.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body1.Replace("[SiteName]", objAppInfo.SiteName);

                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                        {

                            if (startDateTime == DateTime.MinValue)
                            {
                                var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.MemberEmail, subject, body1.ToString());
                            }
                            else
                            {
                                var responsce = _sentmail.SendMailWithAttachmentICO(objAppInfo.MemberEmail, objAppInfo.MemberEmail, subject, body1.ToString(), calendarAttachment);
                            }
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "admin Email Sent",
                                LogDescription = $"Confirmation email sent to admin: {objAppInfo.MemberEmail}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                    }

                }
                ViewBag.objMembers = objMembers;

            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members Acknowledgement",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objMemberss.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }


            ViewBag.cname = cname;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MembershipRenewal(
     Entities.Members objMembers,
     IFormFile file2,
     IFormFile filezelle,
     List<Entities.ChildrenInfo> lstChildrenInfo,
     Entities.MembershipTypes objMembershipType,
     Entities.MembershipOrders objMembershipOrders,
     string Phc1 = "",
     string Phc2 = "",
     string Phc3 = "",
     string MCaptcha = "",
     string cname = "",
     string PhoneNo = "",
     string Notifications = "",
     string recaptchaToken = "",
     string Honeypot = "",
     string ValidationCode = "")
        {
            Int64 LRstatus = 0;
            IBraintreeGateway gateway = _config.GetGateway();

            // IP Address (Core)
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            objMembers.WebsiteAddress = ip;
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "MembershipRenewal",
                LogDescription = $"New MembershipRenewal from IP: {ip}",
                LogDate = DateTime.Now,
                InsertedBy = objMembers.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
            Entities.LogReport logs = new Entities.LogReport();

            // Honeypot validation
            if (!string.IsNullOrWhiteSpace(Honeypot))
            {
                logs.LogTitle = "Members";
                logs.LogDescription = "Honeypot Bot activity detected - (" + objMembers.FirstName + " " + objMembers.LastName + ")";
                lstLogReport.Add(logs);
                LRstatus = _Users.InsertLogReport(lstLogReport);
                ModelState.AddModelError("", "Bot activity detected.");
                return RedirectToAction("AddMember", "Members", new { cname = cname });
            }

            if (Request.Form["ClientValidationStatus"] == "true")
                ModelState.Clear();




            try
            {

                // ── Log Captcha Failure Redirect ──
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = "After reCAPTCHA Entered to the Process.",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                string imageurl = "NA";
                string receipturl = "NA";
                string SanitizeFileName(string fileName)
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);

                    // Remove spaces and special chars
                    name = name.Replace(" ", "_")
                               .Replace("(", "")
                               .Replace(")", "")
                               .Trim('.');

                    return name + ext;
                }

                // Upload to temp with sanitized name
                if (file2 != null && file2.Length > 0)
                {
                    imageurl = SanitizeFileName(file2.FileName);
                    receipturl = imageurl;
                    string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", imageurl);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                    await using var stream1 = new FileStream(tempPath, FileMode.Create);
                    await file2.CopyToAsync(stream1);
                }

                if (filezelle != null && filezelle.Length > 0)
                {
                    receipturl = SanitizeFileName(filezelle.FileName);
                    string tempPath = Path.Combine(objAppInfo.UploadPath, "temp", receipturl);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                    await using var stream2 = new FileStream(tempPath, FileMode.Create);
                    await filezelle.CopyToAsync(stream2);
                }

                // ✅ Save original names before DB insert
                string originalImageUrl = imageurl;
                string originalReceiptUrl = receipturl;




                objMembers.IsApproved = objMembers.PaymentMethod != "Cash/Cheque";
                objMembers.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objMembers.InsertedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objMembers.IsLockedOut = false;
                string RecurringType = objMembers.RecurringType;

                objMembershipOrders.UpdatedBy = "Admin";
                Int64 _status = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "MembershipRenewal Insert Successfully",
                    LogDescription = _status == 1
                                          ? $"Enquiry inserted successfully"
                                          : $"Enquiry insert failed. Status: {_status}",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });



                // ✅ Move using ORIGINAL temp name → rename to DB name
                if (file2 != null && file2.Length > 0 && imageurl != "NA")
                {


                    string finalFolder = Path.Combine(objAppInfo.UploadPath, "MemberReceiptImages");
                    Directory.CreateDirectory(finalFolder);

                    string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalImageUrl);
                    string dest = Path.Combine(finalFolder, imageurl); // imageurl now has MemberId prefix
                    if (System.IO.File.Exists(tempSrc))
                        System.IO.File.Move(tempSrc, dest, overwrite: true);
                }

                if (filezelle != null && filezelle.Length > 0 && receipturl != "NA")
                {


                    string finalFolder = Path.Combine(objAppInfo.UploadPath, "MemberReceiptImages");
                    Directory.CreateDirectory(finalFolder);

                    string tempSrc = Path.Combine(objAppInfo.UploadPath, "temp", originalReceiptUrl);
                    string dest = Path.Combine(finalFolder, receipturl); // receipturl now has MemberId prefix
                    if (System.IO.File.Exists(tempSrc))
                        System.IO.File.Move(tempSrc, dest, overwrite: true);
                }




                // Children info

                if (objMembers.MemberAmount != 0)
                {
                    // Session (Core)
                    HttpContext.Session.SetString("TotalAmount", objMembers.MemberAmount.ToString());
                    if (objMembers.PaymentMethod == "Square")
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

                        // ── ADDED: Recurring detection ────────────────────────────────────────────
                        bool isRecurring = !string.IsNullOrEmpty(objMembers.RecurringType)
                                           && objMembers.MemberAmount != 0;

                        string squareCustomerId = "";
                        string squareCardId = "";
                        string tokenForPayment = squareNonce; // will swap to card_xxx if recurring

                        // ── ADDED: STEP 1 — Create Customer + Save Card on File (only if recurring) ──
                        if (isRecurring)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = $"Recurring detected — Creating Square Customer | MemberId:{objMembers.MemberId}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                            try
                            {
                                // ── Create Customer ──────────────────────────────────────────────────
                                var createCustomerBody = new Square.Models.CreateCustomerRequest.Builder()
                                    .IdempotencyKey(Guid.NewGuid().ToString())
                                    .GivenName(objMembers.FirstName)
                                    .FamilyName(objMembers.LastName)
                                    .EmailAddress(objMembers.Email)
                                    .PhoneNumber(objMembers.MobilePhone)
                                    .ReferenceId(objMembers.MemberId.ToString())
                                    .Build();

                                var customerResponse = await squareClient.CustomersApi.CreateCustomerAsync(createCustomerBody);
                                squareCustomerId = customerResponse.Customer.Id;

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Payment",
                                    LogDescription = $"Square Customer Created: {squareCustomerId} | MemberId:{objMembers.MemberId}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });

                                // ── Save Card on File ────────────────────────────────────────────────
                                var createCardBody = new Square.Models.CreateCardRequest.Builder(
                                        Guid.NewGuid().ToString(),
                                        squareNonce,
                                        new Square.Models.Card.Builder()
                                            .CustomerId(squareCustomerId)
                                            .CardholderName($"{objMembers.FirstName} {objMembers.LastName}")
                                            .Build())
                                    .Build();

                                var cardResponse = await squareClient.CardsApi.CreateCardAsync(createCardBody);
                                squareCardId = cardResponse.Card.Id;
                                tokenForPayment = squareCardId; // charge uses card_id, not nonce

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Payment",
                                    LogDescription = $"Card on File saved: {squareCardId} | CustomerId:{squareCustomerId} | MemberId:{objMembers.MemberId}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            catch (Square.Exceptions.ApiException sqEx)
                            {
                                string errDetail = string.Join(" | ", sqEx.Errors?.Select(e => $"{e.Code}: {e.Detail}") ?? new[] { sqEx.Message });
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Square Payment",
                                    LogDescription = $"Customer/Card creation failed: {errDetail} | MemberId:{objMembers.MemberId}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                return RedirectToAction("PaymentFailed", "Home");
                            }
                        }
                        // ── END ADDED STEP 1 ──────────────────────────────────────────────────────

                        long amountCents = (long)(objMembers.MemberAmount * 100);
                        var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();

                        // CHANGED: uses tokenForPayment so recurring uses card_id, one-time uses nonce
                        var createPaymentBodyBuilder = new Square.Models.CreatePaymentRequest.Builder(
                                tokenForPayment,
                                Guid.NewGuid().ToString(),
                                money)
                            .LocationId(squareLocationId)
                            .Note($"Membership Registration - {objMembers.MembershipType} | {objMembers.FirstName} {objMembers.LastName}");

                        // ADDED: link payment to customer when recurring (required for card-on-file charges)
                        if (!string.IsNullOrEmpty(squareCustomerId))
                            createPaymentBodyBuilder = createPaymentBodyBuilder.CustomerId(squareCustomerId);

                        var createPaymentBody = createPaymentBodyBuilder.Build();

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
                                HttpContext.Session.SetString("CTotalAmount", objMembers.MemberAmount.ToString());

                                string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                                    ? objMembers.FirstName + " " + objMembers.LastName
                                    : User.Identity?.Name ?? "";

                                objMembershipOrders.OrderType = "Square";
                                objMembershipOrders.Amount = objMembers.MemberAmount;
                                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                                objMembershipOrders.PaymentMethod = "Square";
                                objMembershipOrders.TransactionId = squareTransactionId;
                                objMembershipOrders.PaymentStatus = "Completed";
                                objMembershipOrders.MemberId = objMembers.MemberId;
                                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                                objMembershipOrders.MembershipType = objMembers.MembershipType;
                                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objMembershipOrders.UpdatedBy = Username;

                                Int64 result5 = _Members.InsertMemberOrder(objMembershipOrders);

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "MembershipOrders",
                                    LogDescription = "Updated MembershipOrders Details",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                                int result2 = 0;
                                objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result2);

                                if (result5 == 1)
                                {
                                    // ── STEP 3 — Create Subscription (only if recurring) ────────
                                    string recStatus = "", squareSubProfileId = "", squareSubPaymentProfileId = "",
                                           squareSubShippingProfileId = "", squareSubscriptionId = "";

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "Square Payment",
                                        LogDescription = $"STEP3 START | isRecurring:{isRecurring} | RecurringType:{RecurringType} | MemberId:{objMembers.MemberId}",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });

                                    if (isRecurring)
                                    {
                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Square Payment",
                                            LogDescription = $"isRecurring=TRUE | RecurringType:{RecurringType} | MemberId:{objMembers.MemberId}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });




                                        DateTime regDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                                        DateTime julyFirst = regDate.Month <= 3
                                            ? new DateTime(regDate.Year, 7, 1)       // Jan-Mar → this year July 1
                                            : new DateTime(regDate.Year + 1, 7, 1);  // Apr-Dec → next year July 1

                                        DateTime juneThirty = julyFirst.AddDays(-1);


                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Square Payment",
                                            LogDescription = $"regDate:{regDate:yyyy-MM-dd} | juneThirty:{juneThirty:yyyy-MM-dd} | ExpiryDate:{juneThirty.AddDays(-1):yyyy-MM-dd}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });

                                        var objDonorProxy = new Entities.Members
                                        {
                                            FirstName = objMembers.FirstName,
                                            LastName = objMembers.LastName,
                                            Email = objMembers.Email,
                                            MobilePhone = objMembers.MobilePhone,
                                            RecurringType = RecurringType,
                                            MemberAmount = objMembers.MemberAmount,
                                            InsertedTime = juneThirty,
                                            MembershipType = objMembers.MembershipType,
                                        };

                                        // ✅ CHECK: Does member already have subscription in DB?
                                        int subCheckStatus = 0;
                                        Entities.MemberSubscriptions existingSub =
                                            _Members.GetMemberSubscriptionByEmail(objMembers.Email, ref subCheckStatus);

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = "Square Payment",
                                            LogDescription = $"GetMemberSubscriptionByEmail | Email:{objMembers.Email} | subCheckStatus:{subCheckStatus} | existingSub:{(existingSub != null ? "FOUND" : "NULL")} | SubId:{existingSub?.SubscriptionId ?? "NULL"}",
                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });

                                        if (existingSub != null && !string.IsNullOrEmpty(existingSub.SubscriptionId))
                                        {
                                            squareSubscriptionId = existingSub.SubscriptionId;
                                            recStatus = "Success";

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE1: Existing subscription found | OldSubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

                                            // ✅ Update ExpiryDate in MembershipOrders
                                            Int64 updateExpiry = _Members.UpdateMemberOrderExpiryBySubId(
                                                new Entities.MembershipOrders
                                                {
                                                    MemberId = objMembers.MemberId,
                                                    ExpiryDate = juneThirty
                                                });

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE1: UpdateMemberOrderExpiryBySubId Result:{updateExpiry} | ExpiryDate:{juneThirty:yyyy-MM-dd}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

                                            // ✅ Create NEW Square subscription (old one is cancelled, can't resume)
                                            string newSubId = "";
                                            string newRecStatus = "";

                                            (newRecStatus, newSubId) = await _RecurringPaymentController.CreateSquareSubscriptionAsync(
                                                squareAccessToken,
                                                squareEnvironment,
                                                squareLocationId,
                                                objDonorProxy,
                                                squareCustomerId,
                                                squareCardId);

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE1: New Square sub created | NewSubId:{newSubId} | Status:{newRecStatus} | MemberId:{objMembers.MemberId}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

                                            // ✅ Use new SubId if created successfully, else keep old
                                            if (!string.IsNullOrEmpty(newSubId))
                                                squareSubscriptionId = newSubId;

                                            // ✅ Update DB row with new SubId + RecurringType + NextDate
                                            Int64 updateSub = _Members.UpdateSubscriptionNextDate(
                                                new Entities.MemberSubscriptions
                                                {
                                                    MemberId = objMembers.MemberId,
                                                    SubscriptionId = squareSubscriptionId,
                                                    RecurringType = RecurringType,
                                                    RecurringAmount = objMembers.MemberAmount,
                                                    RecurringStartTime = DateOnly.FromDateTime(juneThirty)
                                                });

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE1: UpdateSubscriptionNextDate Result:{updateSub} | NewSubId:{squareSubscriptionId} | NextDate:{juneThirty}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });
                                        }
                                        else
                                        {
                                            // ✅ CASE 2: No subscription — first time recurring
                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE2: No existing subscription | Creating new | MemberId:{objMembers.MemberId}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

                                            (recStatus, squareSubscriptionId) = await _RecurringPaymentController.CreateSquareSubscriptionAsync(
                                                squareAccessToken,
                                                squareEnvironment,
                                                squareLocationId,
                                                objDonorProxy,
                                                squareCustomerId,
                                                squareCardId);

                                            _Users.LogSubReportInsert(new Entities.Logdetails
                                            {
                                                LogId = LogId,
                                                LogTitle = "Square Payment",
                                                LogDescription = $"CASE2: CreateSquareSubscription Result:{recStatus} | SubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                                                LogDate = DateTime.Now,
                                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                InsertedDate = DateTime.Now,
                                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                            });

                                            // ✅ Insert new subscription to DB
                                            if (!string.IsNullOrEmpty(squareSubscriptionId))
                                            {
                                                _Members.InsertMemberSubscriptions(new Entities.MemberSubscriptions
                                                {
                                                    MemberId = objMembers.MemberId,
                                                    SubscriptionId = squareSubscriptionId,
                                                    ProfileId = squareCustomerId,
                                                    PaymentProfileId = squareCardId,
                                                    RecurringType = RecurringType,
                                                    RecurringAmount = objMembers.MemberAmount,
                                                    RecurringStartTime = DateOnly.FromDateTime(juneThirty)  // June 30
                                                });

                                                _Users.LogSubReportInsert(new Entities.Logdetails
                                                {
                                                    LogId = LogId,
                                                    LogTitle = "Square Payment",
                                                    LogDescription = $"CASE2: InsertMemberSubscriptions OK | SubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                                                    LogDate = DateTime.Now,
                                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                                    InsertedDate = DateTime.Now,
                                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                                });
                                            }
                                        }
                                       
                                    }
                                    return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = "Square", tid = objMembers.TransactionId });

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
                            else
                            {
                                string statusMsg = squareResponse.Payment?.Status ?? "null";

                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "squareResponse Payment response",
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
                        catch (Square.Exceptions.ApiException sqEx)
                        {
                            string errDetail = string.Join(" | ", sqEx.Errors?.Select(e => $"{e.Code}: {e.Detail}") ?? new[] { sqEx.Message });
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Square Payment",
                                LogDescription = $"Customer/Card creation failed: {errDetail} | MemberId:{objMembers.MemberId}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            return RedirectToAction("PaymentFailed", "Home");
                        }
                    }
                    //if (objMembers.PaymentMethod == "Square")
                    //{
                    //    _Users.LogSubReportInsert(new Entities.Logdetails
                    //    {
                    //        LogId = LogId,
                    //        LogTitle = "Square Payment",
                    //        LogDescription = "Square Payment started",
                    //        LogDate = DateTime.Now,
                    //        InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //        InsertedDate = DateTime.Now,
                    //        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //    });

                    //    int paystatus = 0;
                    //    Entities.PaymentSettings objsettings = _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

                    //    string squareAccessToken = objsettings.SuccessUrl;
                    //    string squareEnvironment = objsettings.AccountType == "Test" ? "sandbox" : "production";
                    //    string squareLocationId = objsettings.TokenNo;
                    //    string squareNonce = Request.Form["squareNonce"].ToString();
                    //    _Users.LogSubReportInsert(new Entities.Logdetails
                    //    {
                    //        LogId = LogId,
                    //        LogTitle = "Debug",
                    //        LogDescription = $"AccountType:{objsettings.AccountType} | " +
                    //             $"Environment:{squareEnvironment} | " +
                    //             $"TokenFirst15:{squareAccessToken?.Substring(0, Math.Min(15, squareAccessToken?.Length ?? 0))} | " +
                    //             $"LocationId:{squareLocationId} | " +
                    //             $"Nonce:{squareNonce?.Substring(0, Math.Min(10, squareNonce?.Length ?? 0))}",
                    //        LogDate = DateTime.Now,
                    //        InsertedBy = "Debug",
                    //        InsertedDate = DateTime.Now,
                    //        UpdatedBy = "Debug",
                    //        UpdatedDate = DateTime.Now
                    //    });
                    //    if (string.IsNullOrWhiteSpace(squareNonce))
                    //    {
                    //        TempData["message"] = "<div class=\"error1 closable\">Payment card was not submitted properly. Please try again.</div>";
                    //        return RedirectToAction("PaymentFailed", "Home");
                    //    }

                    //    var squareClient = new Square.SquareClient.Builder()
                    //        .Environment(squareEnvironment == "production" ? Square.Environment.Production : Square.Environment.Sandbox)
                    //        .AccessToken(squareAccessToken)
                    //        .Build();

                    //    // ── ADDED: Recurring detection ────────────────────────────────────────────
                    //    bool isRecurring = !string.IsNullOrEmpty(objMembers.RecurringType)
                    //                       && objMembers.MemberAmount != 0;

                    //    string squareCustomerId = "";
                    //    string squareCardId = "";
                    //    string tokenForPayment = squareNonce; // will swap to card_xxx if recurring

                    //    // ── ADDED: STEP 1 — Create Customer + Save Card on File (only if recurring) ──
                    //    if (isRecurring)
                    //    {
                    //        _Users.LogSubReportInsert(new Entities.Logdetails
                    //        {
                    //            LogId = LogId,
                    //            LogTitle = "Square Payment",
                    //            LogDescription = $"Recurring detected — Creating Square Customer | MemberId:{objMembers.MemberId}",
                    //            LogDate = DateTime.Now,
                    //            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //            InsertedDate = DateTime.Now,
                    //            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //        });

                    //        try
                    //        {
                    //            // ── Create Customer ──────────────────────────────────────────────────
                    //            var createCustomerBody = new Square.Models.CreateCustomerRequest.Builder()
                    //                .IdempotencyKey(Guid.NewGuid().ToString())
                    //                .GivenName(objMembers.FirstName)
                    //                .FamilyName(objMembers.LastName)
                    //                .EmailAddress(objMembers.Email)
                    //                .PhoneNumber(objMembers.MobilePhone)
                    //                .ReferenceId(objMembers.MemberId.ToString())
                    //                .Build();

                    //            var customerResponse = await squareClient.CustomersApi.CreateCustomerAsync(createCustomerBody);
                    //            squareCustomerId = customerResponse.Customer.Id;

                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "Square Payment",
                    //                LogDescription = $"Square Customer Created: {squareCustomerId} | MemberId:{objMembers.MemberId}",
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });

                    //            // ── Save Card on File ────────────────────────────────────────────────
                    //            var createCardBody = new Square.Models.CreateCardRequest.Builder(
                    //                    Guid.NewGuid().ToString(),
                    //                    squareNonce,
                    //                    new Square.Models.Card.Builder()
                    //                        .CustomerId(squareCustomerId)
                    //                        .CardholderName($"{objMembers.FirstName} {objMembers.LastName}")
                    //                        .Build())
                    //                .Build();

                    //            var cardResponse = await squareClient.CardsApi.CreateCardAsync(createCardBody);
                    //            squareCardId = cardResponse.Card.Id;
                    //            tokenForPayment = squareCardId; // charge uses card_id, not nonce

                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "Square Payment",
                    //                LogDescription = $"Card on File saved: {squareCardId} | CustomerId:{squareCustomerId} | MemberId:{objMembers.MemberId}",
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });
                    //        }
                    //        catch (Square.Exceptions.ApiException sqEx)
                    //        {
                    //            string errDetail = string.Join(" | ", sqEx.Errors?.Select(e => $"{e.Code}: {e.Detail}") ?? new[] { sqEx.Message });
                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "Square Payment",
                    //                LogDescription = $"Customer/Card creation failed: {errDetail} | MemberId:{objMembers.MemberId}",
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });
                    //            return RedirectToAction("PaymentFailed", "Home");
                    //        }
                    //    }
                    //    // ── END ADDED STEP 1 ──────────────────────────────────────────────────────

                    //    long amountCents = (long)(objMembers.MemberAmount * 100);
                    //    var money = new Square.Models.Money.Builder().Amount(amountCents).Currency("AUD").Build();

                    //    // CHANGED: uses tokenForPayment so recurring uses card_id, one-time uses nonce
                    //    var createPaymentBodyBuilder = new Square.Models.CreatePaymentRequest.Builder(
                    //            tokenForPayment,
                    //            Guid.NewGuid().ToString(),
                    //            money)
                    //        .LocationId(squareLocationId)
                    //        .Note($"Membership Registration - {objMembers.MembershipType} | {objMembers.FirstName} {objMembers.LastName}");

                    //    // ADDED: link payment to customer when recurring (required for card-on-file charges)
                    //    if (!string.IsNullOrEmpty(squareCustomerId))
                    //        createPaymentBodyBuilder = createPaymentBodyBuilder.CustomerId(squareCustomerId);

                    //    var createPaymentBody = createPaymentBodyBuilder.Build();

                    //    try
                    //    {
                    //        var squareResponse = await squareClient.PaymentsApi.CreatePaymentAsync(createPaymentBody);
                    //        if (squareResponse.Payment != null && squareResponse.Payment.Status == "COMPLETED")
                    //        {
                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "Square Payment",
                    //                LogDescription = "Square Payment squareResponse Status  COMPLETED",
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });
                    //            string squareTransactionId = squareResponse.Payment.Id;

                    //            // ✅ Session fix
                    //            HttpContext.Session.SetString("CTotalAmount", objMembers.MemberAmount.ToString());

                    //            string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                    //                ? objMembers.FirstName + " " + objMembers.LastName
                    //                : User.Identity?.Name ?? "";

                    //            objMembershipOrders.OrderType = "Square";
                    //            objMembershipOrders.Amount = objMembers.MemberAmount;
                    //            objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                    //            objMembershipOrders.PaymentMethod = "Square";
                    //            objMembershipOrders.TransactionId = squareTransactionId;
                    //            objMembershipOrders.PaymentStatus = "Completed";
                    //            objMembershipOrders.MemberId = objMembers.MemberId;
                    //            objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                    //            objMembershipOrders.MembershipType = objMembers.MembershipType;
                    //            objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    //            objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    //            objMembershipOrders.UpdatedBy = Username;

                    //            Int64 result5 = _Members.InsertMemberOrder(objMembershipOrders);

                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "MembershipOrders",
                    //                LogDescription = "Updated MembershipOrders Details",
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });
                    //            int result2 = 0;
                    //            objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result2);

                    //            if (result5 == 1)
                    //            {
                    //                // ── STEP 3 — Create Subscription (only if recurring) ────────
                    //                string recStatus = "", squareSubProfileId = "", squareSubPaymentProfileId = "",
                    //                       squareSubShippingProfileId = "", squareSubscriptionId = "";

                    //                _Users.LogSubReportInsert(new Entities.Logdetails
                    //                {
                    //                    LogId = LogId,
                    //                    LogTitle = "Square Payment",
                    //                    LogDescription = $"STEP3 START | isRecurring:{isRecurring} | RecurringType:{RecurringType} | MemberId:{objMembers.MemberId}",
                    //                    LogDate = DateTime.Now,
                    //                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                    InsertedDate = DateTime.Now,
                    //                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                });

                    //                if (isRecurring)
                    //                {
                    //                    _Users.LogSubReportInsert(new Entities.Logdetails
                    //                    {
                    //                        LogId = LogId,
                    //                        LogTitle = "Square Payment",
                    //                        LogDescription = $"isRecurring=TRUE | RecurringType:{RecurringType} | MemberId:{objMembers.MemberId}",
                    //                        LogDate = DateTime.Now,
                    //                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                        InsertedDate = DateTime.Now,
                    //                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                    });
                    //                    DateTime regDate = objMembers.InsertedTime != DateTime.MinValue
                    //                                                         ? objMembers.InsertedTime
                    //                                                         : DateTime.Now;

                    //                    //DateTime juneThirty = regDate.AddDays(1);



                    //                    DateTime julyFirst = regDate.Month < 4
                    //                        ? new DateTime(regDate.Year, 7, 1)
                    //                        : new DateTime(regDate.Year + 1, 7, 1);

                    //                    DateTime juneThirty = julyFirst.AddDays(-1);

                    //                    _Users.LogSubReportInsert(new Entities.Logdetails
                    //                    {
                    //                        LogId = LogId,
                    //                        LogTitle = "Square Payment",
                    //                        LogDescription = $"regDate:{regDate:yyyy-MM-dd} | juneThirty:{juneThirty:yyyy-MM-dd} | ExpiryDate:{juneThirty.AddDays(-1):yyyy-MM-dd}",
                    //                        LogDate = DateTime.Now,
                    //                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                        InsertedDate = DateTime.Now,
                    //                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                    });

                    //                    var objDonorProxy = new Entities.Members
                    //                    {
                    //                        FirstName = objMembers.FirstName,
                    //                        LastName = objMembers.LastName,
                    //                        Email = objMembers.Email,
                    //                        MobilePhone = objMembers.MobilePhone,
                    //                        RecurringType = RecurringType,
                    //                        MemberAmount = objMembers.MemberAmount,
                    //                        InsertedTime = juneThirty,
                    //                        MembershipType = objMembers.MembershipType,
                    //                    };

                    //                    // ✅ CHECK: Does member already have subscription in DB?
                    //                    int subCheckStatus = 0;
                    //                    Entities.MemberSubscriptions existingSub =
                    //                        _Members.GetMemberSubscriptionByEmail(objMembers.Email, ref subCheckStatus);

                    //                    _Users.LogSubReportInsert(new Entities.Logdetails
                    //                    {
                    //                        LogId = LogId,
                    //                        LogTitle = "Square Payment",
                    //                        LogDescription = $"GetMemberSubscriptionByEmail | Email:{objMembers.Email} | subCheckStatus:{subCheckStatus} | existingSub:{(existingSub != null ? "FOUND" : "NULL")} | SubId:{existingSub?.SubscriptionId ?? "NULL"}",
                    //                        LogDate = DateTime.Now,
                    //                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                        InsertedDate = DateTime.Now,
                    //                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                    });

                    //                    if (existingSub != null && !string.IsNullOrEmpty(existingSub.SubscriptionId))
                    //                    {
                    //                        // ✅ CASE 1: Existing subscription — card expired, member renewed manually
                    //                        squareSubscriptionId = existingSub.SubscriptionId;
                    //                        recStatus = "Success";

                    //                        _Users.LogSubReportInsert(new Entities.Logdetails
                    //                        {
                    //                            LogId = LogId,
                    //                            LogTitle = "Square Payment",
                    //                            LogDescription = $"CASE1: Existing subscription reused | SubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                    //                            LogDate = DateTime.Now,
                    //                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                            InsertedDate = DateTime.Now,
                    //                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                        });

                    //                        // ✅ Update ExpiryDate in MembershipOrders
                    //                        Int64 updateExpiry = _Members.UpdateMemberOrderExpiryBySubId(
                    //                            new Entities.MembershipOrders
                    //                            {
                    //                                MemberId = objMembers.MemberId,
                    //                                ExpiryDate = juneThirty   // June 30
                    //                            });

                    //                        _Users.LogSubReportInsert(new Entities.Logdetails
                    //                        {
                    //                            LogId = LogId,
                    //                            LogTitle = "Square Payment",
                    //                            LogDescription = $"CASE1: UpdateMemberOrderExpiryBySubId Result:{updateExpiry} | ExpiryDate:{juneThirty:yyyy-MM-dd}",
                    //                            LogDate = DateTime.Now,
                    //                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                            InsertedDate = DateTime.Now,
                    //                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                        });

                    //                        // ✅ Update RecurringStartTime in MemberSubscriptions
                    //                        Int64 updateSub = _Members.UpdateSubscriptionNextDate(
                    //                            new Entities.MemberSubscriptions
                    //                            {
                    //                                MemberId = objMembers.MemberId,
                    //                                SubscriptionId = squareSubscriptionId,
                    //                                RecurringType = RecurringType,
                    //                                RecurringStartTime = DateOnly.FromDateTime(juneThirty)
                    //                            });

                    //                        _Users.LogSubReportInsert(new Entities.Logdetails
                    //                        {
                    //                            LogId = LogId,
                    //                            LogTitle = "Square Payment",
                    //                            LogDescription = $"CASE1: UpdateSubscriptionNextDate Result:{updateSub} | NextDate:{juneThirty}",
                    //                            LogDate = DateTime.Now,
                    //                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                            InsertedDate = DateTime.Now,
                    //                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                        });
                    //                    }
                    //                    else
                    //                    {
                    //                        // ✅ CASE 2: No subscription — first time recurring
                    //                        _Users.LogSubReportInsert(new Entities.Logdetails
                    //                        {
                    //                            LogId = LogId,
                    //                            LogTitle = "Square Payment",
                    //                            LogDescription = $"CASE2: No existing subscription | Creating new | MemberId:{objMembers.MemberId}",
                    //                            LogDate = DateTime.Now,
                    //                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                            InsertedDate = DateTime.Now,
                    //                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                        });

                    //                        (recStatus, squareSubscriptionId) = await _RecurringPaymentController.CreateSquareSubscriptionAsync(
                    //                            squareAccessToken,
                    //                            squareEnvironment,
                    //                            squareLocationId,
                    //                            objDonorProxy,
                    //                            squareCustomerId,
                    //                            squareCardId);

                    //                        _Users.LogSubReportInsert(new Entities.Logdetails
                    //                        {
                    //                            LogId = LogId,
                    //                            LogTitle = "Square Payment",
                    //                            LogDescription = $"CASE2: CreateSquareSubscription Result:{recStatus} | SubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                    //                            LogDate = DateTime.Now,
                    //                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                            InsertedDate = DateTime.Now,
                    //                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                        });

                    //                        // ✅ Insert new subscription to DB
                    //                        if (!string.IsNullOrEmpty(squareSubscriptionId))
                    //                        {
                    //                            _Members.InsertMemberSubscriptions(new Entities.MemberSubscriptions
                    //                            {
                    //                                MemberId = objMembers.MemberId,
                    //                                SubscriptionId = squareSubscriptionId,
                    //                                ProfileId = squareCustomerId,
                    //                                PaymentProfileId = squareCardId,
                    //                                RecurringType = objMembers.RecurringType,
                    //                                RecurringAmount = objMembers.MemberAmount,
                    //                                RecurringStartTime = DateOnly.FromDateTime(juneThirty)  // June 30
                    //                            });

                    //                            _Users.LogSubReportInsert(new Entities.Logdetails
                    //                            {
                    //                                LogId = LogId,
                    //                                LogTitle = "Square Payment",
                    //                                LogDescription = $"CASE2: InsertMemberSubscriptions OK | SubId:{squareSubscriptionId} | MemberId:{objMembers.MemberId}",
                    //                                LogDate = DateTime.Now,
                    //                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                                InsertedDate = DateTime.Now,
                    //                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                            });
                    //                        }
                    //                    }
                    //                }
                    //                return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = "Square", tid = objMembers.TransactionId });

                    //            }

                    //            else
                    //            {
                    //                string statusMsg = squareResponse.Payment?.Status ?? "null";

                    //                _Users.LogSubReportInsert(new Entities.Logdetails
                    //                {
                    //                    LogId = LogId,
                    //                    LogTitle = "squre Payment Failed",
                    //                    LogDescription = "squre Payment Failed" + statusMsg,
                    //                    LogDate = DateTime.Now,
                    //                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                    InsertedDate = DateTime.Now,
                    //                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //                });

                    //                return RedirectToAction("PaymentFailed", "Home");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            string statusMsg = squareResponse.Payment?.Status ?? "null";

                    //            _Users.LogSubReportInsert(new Entities.Logdetails
                    //            {
                    //                LogId = LogId,
                    //                LogTitle = "squareResponse Payment response",
                    //                LogDescription = "squre Payment Failed" + statusMsg,
                    //                LogDate = DateTime.Now,
                    //                InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //                InsertedDate = DateTime.Now,
                    //                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //            });

                    //            return RedirectToAction("PaymentFailed", "Home");
                    //        }
                    //    }
                    //    catch (Square.Exceptions.ApiException sqEx)
                    //    {
                    //        string errDetail = string.Join(" | ", sqEx.Errors?.Select(e => $"{e.Code}: {e.Detail}") ?? new[] { sqEx.Message });
                    //        _Users.LogSubReportInsert(new Entities.Logdetails
                    //        {
                    //            LogId = LogId,
                    //            LogTitle = "Square Payment",
                    //            LogDescription = $"Customer/Card creation failed: {errDetail} | MemberId:{objMembers.MemberId}",
                    //            LogDate = DateTime.Now,
                    //            InsertedBy = objMembers.FirstName ?? "Anonymous",
                    //            InsertedDate = DateTime.Now,
                    //            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    //            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    //        });
                    //        return RedirectToAction("PaymentFailed", "Home");
                    //    }
                    //}
                    else if (objMembers.PaymentMethod == "Credit Card")
                    {

                        try
                        {

                            // ✅ 1. Get Amount safely
                            decimal amount = objMembers.MemberAmount;

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
                                string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                             ? objMembers.FirstName + " " + objMembers.LastName
                             : User.Identity?.Name ?? "";
                                decimal paymentAmount = objMembers.MemberAmount;

                                var transaction = result.Target;
                                objMembershipOrders.PaymentMethod = "Credit Card";
                                objMembershipOrders.TransactionId = transaction.Id;
                                objMembershipOrders.PaymentStatus = "Completed";
                                objMembershipOrders.OrderType = "Credit Card";
                                objMembershipOrders.Amount = objMembers.MemberAmount;
                                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                                objMembershipOrders.MemberId = objMembers.MemberId;
                                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                                objMembershipOrders.MembershipType = objMembers.MembershipType;
                                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                                objMembershipOrders.UpdatedBy = Username;

                                Int64 result1 = _Members.InsertMemberOrder(objMembershipOrders);
                                if (result1 == 1)
                                {


                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = "MembershipOrders",
                                        LogDescription = "Updated MembershipOrders Details",
                                        LogDate = DateTime.Now,
                                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                    int result2 = 0;
                                    objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result2);

                                    int _status3 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Renewal Membership", 0, ref _status3);
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
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                                        body.Replace("[MemberId]", objMembers.SpouseCell);
                                        body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                                        body.Replace("[FirstName]", objMembers.FirstName);
                                        body.Replace("[LastName]", objMembers.LastName);
                                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                                        body.Replace("[Email]", objMembers.Email);
                                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                                        body.Replace("[MembershipType]", objMembers.MembershipType);
                                        body.Replace("[ChapterName]", objMembers.ChapterName);

                                        body.Replace("[PaymentType]", "Credit Card");

                                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[MemberSkils]", objMembers.MemberSkils);

                                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);

                                        body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());



                                        var responsce = _sentmail.SendMailSendinbrevo(objMembers.Email, objTemplates.Subject, body.ToString());
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
                                    int status2 = 0; Entities.MailTemplates objTemplates2 = _MailTemplates.GetMailTemplateById("Renewal Membership Admin", 0, ref status2);
                                    if (objTemplates2 != null)
                                    {
                                        StringBuilder body = new StringBuilder();
                                        body.Append(objTemplates2.Description);
                                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName));
                                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                                        body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());

                                        body.Replace("[Amount]", objMembers.objMembershipOrder.Amount.ToString());
                                        body.Replace("[PType]", "Credit Card");
                                        body.Replace("[MTYPE]", objMembers.MembershipType.ToString());
                                        body.Replace("[MEMBERID]", objMembers.MemberSkils);
                                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                                        body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objMembers.FirstName));
                                        body.Replace("[Email]", (objMembers.Email));
                                        body.Replace("[LastName]", BLL.Common.UppercaseFirst(objMembers.LastName));
                                        body.Replace("[MembershipType]", BLL.Common.UppercaseFirst(objMembers.MembershipType));
                                        body.Replace("[Phone]", BLL.Common.UppercaseFirst(objMembers.MobilePhone));
                                        body.Replace("[SiteName]", objAppInfo.SiteName);
                                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

                                        body.Replace("[ChapterName]", objMembers.ChapterName);

                                        var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.MemberEmail, objTemplates.Subject, body.ToString());
                                        // ✅ Check if email was successful or failed
                                        bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                        _Users.LogSubReportInsert(new Entities.Logdetails
                                        {
                                            LogId = LogId,
                                            LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                            LogDescription = isEmailSuccess
                                                ? $"Confirmation email sent successfully to Admin: {objAppInfo.SenderEmail}"
                                                : $"Confirmation email FAILED for Admin: {objAppInfo.SenderEmail} | Response: {responsce}",

                                            LogDate = DateTime.Now,
                                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                                            InsertedDate = DateTime.Now,
                                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                        });
                                    }



                                    return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
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
                                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
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

                    else if (objMembers.PaymentMethod == "Cash/Cheque")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Cash/Cheque Payment",
                            LogDescription = $"Cash/Cheque PaymentMethod Entering ",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                    

                        Int64 result = 0;
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "Cash/Cheque";
                        objMembershipOrders.PaymentStatus = "Pending";
                        objMembershipOrders.MemberId = objMembers.MemberId;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.UserName;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Members.InsertMemberOrder(objMembershipOrders);
                        //result = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
                        int result6 = 0;
                        objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result6);


                        int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                            body.Replace("[MemberId]", objMembers.SpouseCell);
                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body.Replace("[FirstName]", objMembers.FirstName);
                            body.Replace("[LastName]", objMembers.LastName);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body.Replace("[Email]", objMembers.Email);
                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body.Replace("[MembershipType]", objMembers.MembershipType);
                            body.Replace("[TransactionId]", "NA");
                            body.Replace("[PaymentType]", "Cash/Cheque");
                            body.Replace("[PaymentStatus]", "Pending");
                            body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ChapterName]", objMembers.ChapterName);
                            body.Replace("[MemberSkils]", objMembers.MemberSkils);


                            _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());
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

                        int statusAdmin = 0;
                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplatesAdmin.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[MemberId]", objMembers.SpouseCell);
                            body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body1.Replace("[FirstName]", objMembers.FirstName);
                            body1.Replace("[LastName]", objMembers.LastName);
                            body1.Replace("[Email]", objMembers.Email);
                            body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                            body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body1.Replace("[MembershipType]", objMembers.MembershipType);
                            body1.Replace("[TransactionId]", "NA");
                            body1.Replace("[PaymentType]", "Cash/Cheque");
                            body1.Replace("[PaymentStatus]", "Completed");
                            body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                            body1.Replace("[ChapterName]", objMembers.ChapterName);

                            _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());
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
                            return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
                        }
                        if (result == 1)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Redirection Acknowledgement",
                                LogDescription = "Redirection Acknowledgement",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            return RedirectToAction("Acknowledgement", "Members", new { area = "User", MemberId = objMembers.MemberId, PaymentMethod = "Cash/Cheque", cname = cname });
                        }
                    }

                    // ── PayPal ────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "PayPal")

                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PayPal Payment",
                            LogDescription = "Payment PayPal PaymentMethod Entering",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        string Username = !string.IsNullOrEmpty(objMembers.FirstName)
                                ? objMembers.FirstName + " " + objMembers.LastName
                                : User.Identity?.Name ?? "";
                        decimal paymentAmount = objMembers.MemberAmount;


                        objMembershipOrders.OrderType = "Paypal";
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "PayPal";
                        objMembershipOrders.TransactionId = objMembers.TransactionId;
                        objMembershipOrders.PaymentStatus = "Completed";
                        objMembershipOrders.MemberId = objMembers.MemberId;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.MembershipType = objMembers.MembershipType;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedBy = Username;

                        _Members.InsertMemberOrder(objMembershipOrders);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders",
                            LogDescription = "Updated MembershipOrders Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int result2 = 0;
                        objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result2);

                        int _status3 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Renewal Membership", 0, ref _status3);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                            body.Replace("[MemberId]", objMembers.SpouseCell);
                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body.Replace("[FirstName]", objMembers.FirstName);
                            body.Replace("[LastName]", objMembers.LastName);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body.Replace("[Email]", objMembers.Email);
                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body.Replace("[MembershipType]", objMembers.MembershipType);
                            body.Replace("[ChapterName]", objMembers.ChapterName);

                            body.Replace("[PaymentType]", "Paypal");

                            body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[MemberSkils]", objMembers.MemberSkils);

                            body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);

                            body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());



                            var responsce = _sentmail.SendMailSendinbrevo(objMembers.Email, objTemplates.Subject, body.ToString());
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
                        int status2 = 0; Entities.MailTemplates objTemplates2 = _MailTemplates.GetMailTemplateById("Renewal Membership Admin", 0, ref status2);
                        if (objTemplates2 != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates2.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName));
                            body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                            body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());

                            body.Replace("[Amount]", objMembers.objMembershipOrder.Amount.ToString());
                            body.Replace("[PType]", "Paypal");
                            body.Replace("[MTYPE]", objMembers.MembershipType.ToString());
                            body.Replace("[MEMBERID]", objMembers.MemberSkils);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objMembers.FirstName));
                            body.Replace("[Email]", (objMembers.Email));
                            body.Replace("[LastName]", BLL.Common.UppercaseFirst(objMembers.LastName));
                            body.Replace("[MembershipType]", BLL.Common.UppercaseFirst(objMembers.MembershipType));
                            body.Replace("[Phone]", BLL.Common.UppercaseFirst(objMembers.MobilePhone));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

                            body.Replace("[ChapterName]", objMembers.ChapterName);

                            var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.MemberEmail, objTemplates.Subject, body.ToString());
                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to Admin: {objAppInfo.SenderEmail}"
                                    : $"Confirmation email FAILED for Admin: {objAppInfo.SenderEmail} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                        var redirectUrl = Url.Action("RenewalAcknowledgement", "Members", new
                        {
                            area = "User",
                            MemberId = objMembers.MemberId,
                            cname = cname,
                            PaymentMethod = objMembers.PaymentMethod,
                            tid = objMembers.TransactionId
                        });
                        return Json(new { success = true, redirectUrl = redirectUrl });


                        //return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
                    }

                    // ── Zelle ─────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Zelle")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Zelle Payment",
                            LogDescription = $"Entered Into Zelle Donation Proces",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        Int64 result = 0;
                        //Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "Zelle";
                        objMembershipOrders.PaymentStatus = "Pending";
                        objMembershipOrders.MemberId = objMembers.MemberId; ;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.FirstName;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



                        result = _Members.InsertMemberOrder(objMembershipOrders);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders",
                            LogDescription = $"Updated MembershipOrders Details",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        //result = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
                        int result6 = 0;
                        objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result6);

                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Donations",
                            LogDescription = $"Get MembersDetails info By Id",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
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
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                            body.Replace("[MemberId]", objMembers.SpouseCell);
                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body.Replace("[FirstName]", objMembers.FirstName);
                            body.Replace("[LastName]", objMembers.LastName);
                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body.Replace("[Email]", objMembers.Email);
                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body.Replace("[MembershipType]", objMembers.MembershipType);
                            body.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body.Replace("[TransactionId]", "NA");
                            body.Replace("[PaymentType]", "Zelle");
                            body.Replace("[PaymentStatus]", "Pending");
                            body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ChapterName]", objMembers.ChapterName);


                            _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());
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


                        int statusAdmin = 0;
                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                        {
                            StringBuilder body1 = new StringBuilder();
                            body1.Append(objTemplatesAdmin.Description);
                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body1.Replace("[MemberId]", objMembers.SpouseCell);
                            body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                            body1.Replace("[FirstName]", objMembers.FirstName);
                            body1.Replace("[LastName]", objMembers.LastName);
                            body1.Replace("[Email]", objMembers.Email);
                            body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                            body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                            body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                            body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                            body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                            body1.Replace("[MembershipType]", objMembers.MembershipType);
                            body1.Replace("[TransactionId]", "NA");
                            body1.Replace("[PaymentType]", "Zelle");
                            body1.Replace("[PaymentStatus]", "Completed");
                            body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            body1.Replace("[SiteName]", objAppInfo.SiteName);
                            body1.Replace("[ChapterName]", objMembers.ChapterName);

                            _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Admin Email Sent",
                                LogDescription = $"Admin notification email sent to: {objAppInfo.MemberEmail}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                        }
                        if (result == 1)
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Members redirecting Acknowledgement",
                                LogDescription = $"Members redirecting Acknowledgement",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            return RedirectToAction("Acknowledgement", "Members", new { area = "User", MemberId = objMembers.MemberId, PaymentMethod = "Zelle", cname = cname });
                        }
                    }

                    // ── Stripe ────────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Stripe")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Stripe Payment",
                            LogDescription = $"Entered into Stripe Members Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        Int64 result = 0;
                        objMembershipOrders.Amount = objMembers.MemberAmount;
                        objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
                        objMembershipOrders.PaymentMethod = "Stripe";
                        objMembershipOrders.MemberId = objMembers.MemberId;
                        objMembershipOrders.PaymentBy = objMembers.PaymentBy;
                        objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
                        objMembershipOrders.UpdatedBy = objMembers.UserName;
                        objMembershipOrders.TransactionId = objMembers.TransactionId;
                        objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                        objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                        result = _Members.InsertMemberOrder(objMembershipOrders);
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "MembershipOrders Stripe updated",
                            LogDescription = $"updated  Stripe MembershipOrders",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        int status2 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);
                        StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;

                        var priceOptions = new PriceCreateOptions
                        {
                            UnitAmount = (long)(objMembers.MemberAmount * 100),
                            Currency = "usd",
                            ProductData = new PriceProductDataOptions { Name = "Membership" }
                        };
                        var price = new PriceService().Create(priceOptions);

                        string baseUrl = $"{Request.Scheme}://{Request.Host}";
                        var sessionOptions = new SessionCreateOptions
                        {
                            PaymentMethodTypes = new List<string> { "card" },
                            LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
                    },
                            Mode = "payment",
                            SuccessUrl = $"{baseUrl}/User/Members/RenewalAcknowledgement?session_id={{CHECKOUT_SESSION_ID}}&MemberId={objMembers.MemberId}&PaymentMethod=Stripe&cname={cname}",
                            CancelUrl = $"{baseUrl}/User/Members/RenewalAcknowledgement"
                        };

                        var session = new SessionService().Create(sessionOptions);

                        // Core cookie
                        Response.Cookies.Append("StripeSessionId", session.Id);

                        if (result == 1)
                        {
                            if (!string.IsNullOrEmpty(session.Url))
                                return Redirect(session.Url);
                        }
                    }

                    // ── Swire Pay ─────────────────────────────────────────────────
                    else if (objMembers.PaymentMethod == "Swire Pay")
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Swire Pay Payment",
                            LogDescription = $"Entered into Swire Pay Donation Process",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });


                        int status2 = 0;
                        Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);


                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "PaymentMethod Details",
                            LogDescription = $" Swire Payment Get Details By PaymentMethod",
                            LogDate = DateTime.Now,
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });

                        string redirectUri = Url.Action("RenewalAcknowledgement", "Members",
                            new { area = "User", MemberId = objMembers.MemberId, PaymentMethod = "Swire Pay", cname = cname }, Request.Scheme)!;

                        decimal amountInUSD = objMembers.MemberAmount * 100;

                        var jsonContent = $@"{{
                    ""amount"": ""{amountInUSD}"",
                    ""currencyCode"": ""USD"",
                    ""paymentMethodType"": [""CARD""],
                    ""redirectUri"": ""{redirectUri}"",
                    ""description"": ""Membership payment for {objMembers.UserName}"",
                    ""statementDescriptor"": ""Membership Payment"",
                    ""expiresAt"": ""{DateTime.UtcNow.AddDays(7):yyyy-MM-ddTHH:mm:ss.fffZ}"",
                    ""email"": ""{objMembers.Email}"",
                    ""phone"": ""{objMembers.MobilePhone}"",
                    ""name"": ""{objMembers.UserName}""
                }}";

                        using var client1 = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");
                        request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);
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
                            InsertedBy = objMembers.FirstName ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objMembers.FirstName ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        if (!string.IsNullOrEmpty(paymentLink))
                            return Redirect(paymentLink);
                        else
                        {
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Members",
                                LogDescription = "Swire Pay Payment link not found in the response.",
                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                            return RedirectToAction("RenewalAcknowledgement", "Members", new { area = "User", MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });

                        }
                    }

                    TempData["message"] = "<div class=\"success closable\">Your registration completed successfully and our team will contact soon.</div>";
                    return RedirectToAction("RenewalAcknowledgement", "Members", new { cname = cname });
                }
                else
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "member amount 0 inserted",
                        LogDescription = "member amount 0 inserted",
                        LogDate = DateTime.Now,
                        InsertedBy = objMembers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMembers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    return RedirectToAction("Acknowledgement", "Members", new { cname = cname });

                }

            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Members",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Acknowledgement", "Members", new { cname = cname });
            }
        }

        public ActionResult Thankyou(string cname = "")
        {
            ViewBag.cname = cname;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> RenewalAcknowledgement(Int64 MemberId = 0, string cname = "", string PaymentMethod = "PayPal", string tid = "", string pstatus = "", string session_id = "", string spPaymentLink = "")
        {
            int status1 = 0;
            int result2 = 0;
            string Username = "";
            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            Entities.LogReport logs = new Entities.LogReport();

            Entities.Members objMemberss = new Entities.Members();
            DateTime paydate = DateTime.UtcNow;
            Int64 LogId = 0;
            objMemberss = _Members.GetMemberFullDetailsById(MemberId, ref result2);
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "MembershipRenewal",
                LogDescription = "Members Renewal Acknowledgementtion  Page entering",
                LogDate = DateTime.Now,
                InsertedBy = objMemberss.FirstName ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            try
            {
                if (PaymentMethod == "Stripe")
                {
                    int status2 = 0;
                    Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "MembersRenewal Stripe Payment",
                        LogDescription = "Renewal Acknowledgement Stripe PaymentMethod  getting payment details.",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
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

                    var sessionServiceInstance = new SessionService();
                    var session = sessionServiceInstance.Get(session_id);

                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);


                    Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();

                    objMembershipOrders.PaymentMethod = PaymentMethod;
                    objMembershipOrders.TransactionId = paymentIntent.Id;
                    objMembershipOrders.PaymentStatus = session.PaymentStatus == "paid" ? "Completed" : "Pending";
                    objMembershipOrders.MemberId = MemberId;
                    objMembershipOrders.UpdatedBy = Username;
                    objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "MembershipOrders details updated ",
                        LogDescription = "MembershipOrders details updated ",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });



                }
                if (PaymentMethod == "Swire Pay")
                {
                    // Validate query string parameter
                    spPaymentLink = Request.Query["sp-payment-link"].ToString();
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Swire Pay Payment",
                        LogDescription = "Swire Pay Started into renewal memebers Page",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    if (string.IsNullOrWhiteSpace(spPaymentLink))
                        throw new ArgumentException("Missing 'sp-payment-link' query parameter.");

                    // Fetch payment settings
                    int status2 = 0;
                    var objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

                    if (objPaymentSettings == null || string.IsNullOrWhiteSpace(objPaymentSettings.PaymentUrl))
                        throw new InvalidOperationException("Swire Pay API key could not be retrieved from payment settings.");

                    // Call Swire Pay verify API
                    var verifyUrl = $"https://api.swirepay.com/v1/payment-link/{spPaymentLink}/verify";

                    using var httpClient = new HttpClient();
                    using var httpRequest = new HttpRequestMessage(HttpMethod.Get, verifyUrl);
                    httpRequest.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);

                    var response = await httpClient.SendAsync(httpRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException(
                            $"Swire Pay API returned [{(int)response.StatusCode}]: {errorBody}");
                    }

                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonData = JObject.Parse(responseData);

                    // Extract payment session
                    var paymentSession = jsonData["entity"]?["paymentSession"];
                    var receiptNumber = paymentSession["receiptNumber"]?.ToString();
                    var paymentType = paymentSession["paymentType"]?.ToString();
                    var paymentStatus = paymentSession["status"]?.ToString();

                    pstatus = paymentType;
                    tid = receiptNumber;


                    // Update membership order
                    var adjustedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                    var objMembershipOrders = new Entities.MembershipOrders
                    {
                        PaymentMethod = PaymentMethod,
                        TransactionId = receiptNumber,
                        PaymentStatus = paymentStatus == "SUCCEEDED" ? "Completed" : "Pending",
                        MemberId = MemberId,
                        UpdatedBy = Username,
                        UpdatedTime = adjustedTime,
                        OrderDate = adjustedTime
                    };

                    Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "MembershipOrders",
                        LogDescription = "Updated MembershipOrders renewal Transaction details",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });


                }
                if (PaymentMethod == "Square")
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Square Pay Payment",
                        LogDescription = "Square Pay Started into renewal memebers Page To Mailtemplate",
                        LogDate = DateTime.Now,
                        InsertedBy = objMemberss.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    int status12 = 0;
                    Entities.AppInfo objAppInfos = _appinfo.GetAppInfoDetails(ref status12);

                    int _status1PP = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Renewal Membership", 0, ref _status1PP);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objAppInfos.BaseUrl);
                        body.Replace("[FBUrl]", objAppInfos.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfos.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfos.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfos.GooglePlusUrl);
                        body.Replace("[TPhone]", objAppInfos.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfos.CompanyEmail);
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMemberss.FirstName + " " + objMemberss.LastName));
                        body.Replace("[MemberId]", objMemberss.SpouseCell);
                        body.Replace("[MEMBERID]", objMemberss.MemberId.ToString());
                        body.Replace("[FirstName]", objMemberss.FirstName);
                        body.Replace("[MemberSkils]", objMemberss.MemberSkils);
                        body.Replace("[LastName]", objMemberss.LastName);
                        body.Replace("[IntrestedArea]", objMemberss.SpouseSkils);
                        body.Replace("[SpouseEmail]", objMemberss.SpouseEmail);
                        body.Replace("[Email]", objMemberss.Email);
                        body.Replace("[MobilePhone]", objMemberss.MobilePhone);
                        body.Replace("[MembershipType]", objMemberss.MembershipType);
                        body.Replace("[ChapterName]", objMemberss.ChapterName);
                        body.Replace("[TransactionId]", objMemberss.objMembershipOrder.TransactionId);
                        body.Replace("[PaymentType]", "Square");
                        body.Replace("[PaymentStatus]", "Completed");
                        body.Replace("[PaymentDate]", objMemberss.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body.Replace("[SiteName]", objAppInfos.SiteName);
                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject.Replace("[WebsiteName]", objAppInfos.SiteName);
                        if (objAppInfos.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objMemberss.Email, objAppInfos.MemberEmail, subject + objAppInfos.SiteName, body.ToString());
                            //var responsce = _sentmail.SendMailSendinbrevo(objMemberss.Email, subject, body.ToString().ToString());
                          

                           
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "User Email Sent",
                                LogDescription = $"Confirmation email sent to user: {objAppInfos.Email}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMemberss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
                    }

                    int statusAdminPP = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Renewal Membership Admin", 0, ref statusAdminPP);
                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                    {
                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplatesAdmin.Description);
                        body1.Replace("[usersiteurl]", objAppInfos.BaseUrl);
                        body1.Replace("[FBUrl]", objAppInfos.FacebookUrl);
                        body1.Replace("[TWUrl]", objAppInfos.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfos.YoutubeUrl);
                        body1.Replace("[GUrl]", objAppInfos.GooglePlusUrl);
                        body1.Replace("[TPhone]", objAppInfos.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfos.CompanyEmail);
                        body1.Replace("[MemberId]", objMemberss.SpouseCell);
                        body1.Replace("[MEMBERID]", objMemberss.MemberId.ToString());
                        body1.Replace("[FirstName]", objMemberss.FirstName);
                        body1.Replace("[LastName]", objMemberss.LastName);
                        body1.Replace("[Email]", objMemberss.Email);
                        body1.Replace("[MobilePhone]", objMemberss.MobilePhone);
                        body1.Replace("[MemberSkils]", objMemberss.MemberSkils);
                        body1.Replace("[IntrestedArea]", objMemberss.SpouseSkils);
                        body1.Replace("[SpouseCell]", objMemberss.SpouseCell);
                        body1.Replace("[ChapterName]", objMemberss.ChapterName);
                        body1.Replace("[SpouseEmail]", objMemberss.SpouseEmail);
                        body1.Replace("[MembershipType]", objMemberss.MembershipType);
                        body1.Replace("[TransactionId]", objMemberss.objMembershipOrder.TransactionId);
                        body1.Replace("[PaymentType]", "Square");
                        body1.Replace("[PaymentStatus]", "Completed");
                        body1.Replace("[PaymentDate]", objMemberss.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body1.Replace("[SiteName]", objAppInfos.SiteName);
                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplatesAdmin.Subject.Replace("[WebsiteName]", objAppInfos.SiteName);
                        if (objAppInfos.Email == "Yes")
                        {
                            var responsce = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfos.MemberEmail, subject + objAppInfos.SiteName, body1.ToString());
                            //var responsce = _sentmail.SendMailSendinbrevo(objAppInfos.MemberEmail, subject, body1.ToString());
                            
                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = "Admin Email Sent",
                                LogDescription = $"Confirmation email sent to Admin: {objAppInfos.MemberEmail}",
                                LogDate = DateTime.Now,
                                InsertedBy = objMemberss.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMemberss.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }
           
                    }


                }

                Entities.Members objMembers = new Entities.Members();
                int result1 = 0;
                objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result1);
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Member Full Details ",
                    LogDescription = $"Getting Member FullDetails Payment method used: {PaymentMethod ?? "Unknown"}.",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });

                DateTime startDateTime = objMembers.objMembershipOrder.ExpiryDate;

                if (objMembers.objMembershipOrder.ExpiryDate != DateTime.MinValue)
                {
                    startDateTime = objMembers.objMembershipOrder.ExpiryDate.Date
                           .AddHours(objMembers.objMembershipOrder.ExpiryDate.Hour)
                           .AddMinutes(objMembers.objMembershipOrder.ExpiryDate.Minute);
                }

                // Step 2: Generate iCalendar content
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("VERSION:2.0");
                //sb.AppendLine("PRODID:-//HTVA// Event Booking//EN");
                sb.AppendLine("PRODID:-//Membership Registration//EN");

                sb.AppendLine("METHOD:REQUEST");
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'"));
                sb.AppendLine("DTSTART:" + startDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                //sb.AppendLine("DTEND:" + endDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                // sb.AppendLine("SUMMARY:Member - " + Membership Renewal Notification for a objMember.objMembershipOrder.MembershipType);

                sb.AppendLine("SUMMARY:Member - " + "Membership Renewal Notification for a " + objMembers.objMembershipOrder.MembershipType);


                sb.AppendLine("DESCRIPTION:" + objMembers.State);
                sb.AppendLine("City:" + objMembers.City);
                // sb.AppendLine("LOCATION:Temple");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("END:VEVENT");
                sb.AppendLine("END:VCALENDAR");

                string icsContent = sb.ToString();

                // Step 3: Convert to byte stream
                byte[] byteArray = Encoding.UTF8.GetBytes(icsContent);
                MemoryStream stream = new MemoryStream(byteArray);
                Attachment calendarAttachment = new Attachment(stream, "Membership.ics", "text/calendar");

                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Redirecting Acknowledgement",
                    LogDescription = $"Payment method used: {PaymentMethod ?? "Unknown"}.",
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
                if (PaymentMethod == "Stripe" || PaymentMethod == "Swire Pay")
                {
                    int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
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
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                        body.Replace("[MemberId]", objMembers.SpouseCell);
                        body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                        body.Replace("[FirstName]", objMembers.FirstName);
                        body.Replace("[LastName]", objMembers.LastName);
                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                        body.Replace("[Email]", objMembers.Email);
                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
                        body.Replace("[MemberSkils]", objMembers.MemberSkils);
                        body.Replace("[MembershipType]", objMembers.MembershipType);
                        body.Replace("[ChapterName]", objMembers.ChapterName);
                        body.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                        body.Replace("[PaymentType]", objMembers.objMembershipOrder.PaymentMethod);
                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body.Replace("[SiteName]", objAppInfo.SiteName);

                        body.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                        {


                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());
                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "User Email Sent" : "User Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to user: {objMembers.Email}"
                                    : $"Confirmation email FAILED for user: {objMembers.Email} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });

                        }

                    }

                    int statusAdmin = 0;
                    Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
                    if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
                    {
                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplatesAdmin.Description);
                        body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body1.Replace("[MemberId]", objMembers.SpouseCell);
                        body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
                        body1.Replace("[FirstName]", objMembers.FirstName);
                        body1.Replace("[LastName]", objMembers.LastName);
                        body1.Replace("[Email]", objMembers.Email);
                        body1.Replace("[MobilePhone]", objMembers.MobilePhone);
                        body1.Replace("[MemberSkils]", objMembers.MemberSkils);
                        body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
                        body1.Replace("[SpouseCell]", objMembers.SpouseCell);
                        body1.Replace("[ChapterName]", objMembers.ChapterName);
                        body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
                        body1.Replace("[MembershipType]", objMembers.MembershipType);
                        body1.Replace("[TransactionId]", objMembers.objMembershipOrder.TransactionId);
                        body1.Replace("[PaymentType]", objMembers.objMembershipOrder.PaymentMethod);
                        body1.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
                        body1.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy hh:mm:ss tt"));
                        body1.Replace("[SiteName]", objAppInfo.SiteName);

                        body1.Replace("[Year]", DateTime.Now.Year.ToString());

                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        if (objAppInfo.Email == "Yes")
                        {

                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());

                            // ✅ Check if email was successful or failed
                            bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                            _Users.LogSubReportInsert(new Entities.Logdetails
                            {
                                LogId = LogId,
                                LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                LogDescription = isEmailSuccess
                                    ? $"Confirmation email sent successfully to Admin: {objAppInfo.MemberEmail}"
                                    : $"Confirmation email FAILED for Admin: {objAppInfo.MemberEmail} | Response: {responsce}",

                                LogDate = DateTime.Now,
                                InsertedBy = objMembers.FirstName ?? "Anonymous",
                                InsertedDate = DateTime.Now,
                                UpdatedBy = objMembers.FirstName ?? "Anonymous",
                                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                            });
                        }

                    }

                }
                ViewBag.objMembers = objMembers;

            }
            catch (Exception ex)
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "renewal  Acknowledgement failed",
                    LogDescription = ex.Message,
                    LogDate = DateTime.Now,
                    InsertedBy = objMembers.FirstName ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objMembers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
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

        public ActionResult GetMemberAmount(Int64 MembershipTypeId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.MembershipTypes _objMembershipTypes = _MembershipType.GetMembershipTypeById(MembershipTypeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objMembershipTypes });
                }
                else
                {
                    str = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error1 closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }



        public ActionResult GetPayment(String PaymentMethod = "")
        {

            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.PaymentSettings _objPaymentSettings = _paymentSettings.PaymentSettingsDetails(PaymentMethod, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objPaymentSettings });
                }
                else
                {
                    str = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"error1 closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        public JsonResult CheckProfileEmailAvailability(Int64 MemberId, string Email)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByEmail(Email, ref status);
                bool data = (objMembers.MemberId == MemberId || objMembers.MemberId == 0 ? true : false);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckEmailAvailability(string Email)
        {
            int status = 0;
            try
            {
                objMembers = _Members.FEGetMemberFullDetailsByEmail(Email, ref status);
                bool data = (objMembers != null && objMembers.objMembershipOrder.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckUserNameAvailability(string UserName)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByUserName(UserName, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckMemberIdAvailability(string SpouseCell, string LastName)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsBySpouseCell(SpouseCell, LastName, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckMemberIdAvailabilityBySpouse(string SpouseCell)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsBySpouse(SpouseCell, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckMemberIdAvailabilityByLastName(string LastName)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByLastName(LastName, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {

            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                json = await reader.ReadToEndAsync();
            }
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    "whsec_OhVVuHvWbrGEBUVLg4xuVkF0miIgOHkA"
                );
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    // Store session ID in database (Example: Save session.Id)
                }
                return Ok(new { received = true });
            }
            catch (StripeException ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        #endregion

        #region Member Account

        public IActionResult LogOn(string cname = "", string rs = "", string url = "")
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            // ✅ Fix 1: Read cookie directly as string
            cname = Request.Cookies["ChapterName"] ?? string.Empty;

            // Build redirect URL
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Contains("event/upcoming/")
                    ? objAppInfo.BaseUrl + url
                    : objAppInfo.BaseUrl + "event-registration.html?" + url;
            }
            else
            {
                url = "profile";
            }

            // ✅ Fix 2: Use Request.Headers["Referer"] instead of Request.UrlReferrer
            var referrer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referrer)
                && referrer != objAppInfo.BaseUrl + "member-login.html"
                && referrer != objAppInfo.BaseUrl + "index.html"
                && referrer != objAppInfo.BaseUrl + "forgot-password.html"
                && referrer.Contains(objAppInfo.BaseUrl + "member-acknowledgement.html")
                && referrer.Contains(objAppInfo.BaseUrl + "member-registration-acknowledgement.html")
                && referrer.Contains(objAppInfo.BaseUrl + "event/upcoming"))
            {
                url = referrer;
            }

            Int64 ChapterId = 0;
            int qstatus = 0;

            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
                cname = "global";
            }

            ViewBag.cname = cname;
            ViewBag.returnurl = url;
            ViewBag.rs = rs;

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogOn(LogOnModel model, string returnUrl = "", string cname = "", string rs = "")
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int status = 0;
                var appInfo = _appinfo.GetAppInfoDetails(ref status);

                int memberStatus = 0;
                var member = _Members.GetMemberFullDetailsByEmail(model.Email, ref memberStatus);

                if (member == null || member.MemberId == 0)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Email or password is incorrect.";
                    return RedirectToAction("LogOn", "Members", new { cname });
                }

                if (!member.IsApproved)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Your account is awaiting admin approval.";
                    return RedirectToAction("LogOn", "Members", new { cname });
                }

                int pwdStatus = 0;
                var storedPassword = _Members.GetPassword(member.MemberId, ref pwdStatus);

                if (pwdStatus != 1 || !AWS_AssociationToolCore.BLL.Password.VerifyHash(model.Password.Trim(), "SHA512", storedPassword))
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Email or password is incorrect.";
                    return RedirectToAction("LogOn", "Members", new { cname });
                }

                // ✅ Create Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, member.FirstName + " " + member.LastName),
            new Claim(ClaimTypes.Email, member.Email),
            new Claim("MemberId", member.MemberId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                };

                // ✅ Sign in user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // ✅ Handle Return URL (same logic as old MVC if block)
                if (!string.IsNullOrEmpty(returnUrl)
                    && Url.IsLocalUrl(returnUrl)
                    && !returnUrl.Contains("member-login")
                    && !returnUrl.Contains("member-registration")
                    && !returnUrl.Contains("reset-password"))
                {
                    // ✅ Append MemberId and MembershipType like old MVC code
                    returnUrl = $"{returnUrl}&uid={member.MemberId}&MembershipType={member.MembershipType}";
                    return Redirect(returnUrl);
                }
                else
                {
                    // ✅ Default redirect - pass cname and rs like old MVC else block
                    if (rs == "reset")
                        return RedirectToAction("Profile", "Members", new { cname, rs });
                    else
                        return RedirectToAction("Profile", "Members", new { cname });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction.";
                return RedirectToAction("LogOn", "Members", new { cname });
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CULLogOn(LogOnModel model, string ReturnUrl = "")
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            try
            {
                int _status = 0;
                Entities.Members objMembers = _Members.GetMemberFullDetailsByEmail(model.CulturalEmail, ref _status);
                if (objMembers.objMembershipOrder.MembershipType == "Cultural Admin")
                {
                    if (objMembers != null && objMembers.MemberId != 0)
                    {
                        if (objMembers.IsApproved == true)
                        {
                            int _qstatus = 0;
                            string password = _Members.GetPassword(objMembers.MemberId, ref _qstatus);
                            if (_qstatus == 1)
                            {
                                if (AWS_AssociationToolCore.BLL.Password.VerifyHash(model.Password.Trim(), "SHA512", password) == true)
                                {
                                    objMembers.UserName = objMembers.FirstName + " " + objMembers.LastName;
                                    HttpContext.Session.SetString("username", objMembers.Email);
                                    HttpContext.Session.SetString("MemberID", objMembers.MemberId.ToString());

                                    // ✅ Create Claims
                                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, objMembers.UserName),
                                new Claim(ClaimTypes.Email, objMembers.Email),
                                new Claim("MemberId", objMembers.MemberId.ToString()),
                                new Claim(ClaimTypes.Role, "CulturalAdmin")
                            };

                                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                    var authProperties = new AuthenticationProperties
                                    {
                                        IsPersistent = model.RememberMe,
                                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                                    };

                                    // ✅ Sign in user
                                    await HttpContext.SignInAsync(
                                        CookieAuthenticationDefaults.AuthenticationScheme,
                                        new ClaimsPrincipal(claimsIdentity),
                                        authProperties
                                    );

                                    if ((ReturnUrl != null && ReturnUrl != "" && ReturnUrl != objAppInfo.BaseUrl + "member-registration.html" && ReturnUrl != objAppInfo.BaseUrl + "index.html" && ReturnUrl != objAppInfo.BaseUrl + "member-login.html"))
                                    {
                                        if (this.User.Identity.IsAuthenticated && this.User.Identity.Name != null && HttpContext.Session.GetString("username") != null)
                                        {
                                            ReturnUrl = ReturnUrl + "&uid=" + objMembers.MemberId + "&MembershipType=" + objMembers.MembershipType;
                                            return new RedirectResult(ReturnUrl);
                                        }
                                        else
                                        {
                                            return new RedirectResult(ReturnUrl);
                                        }
                                    }
                                    else
                                    {
                                        return RedirectToAction("Profile", "Members");
                                    }
                                }
                                else
                                {
                                    TempData["message1"] = "<div class=\"error1 closable\">Email or password is incorrect</div>";
                                }
                            }
                            else
                            {
                                TempData["message1"] = "<div class=\"error1 closable\">Email or password is incorrect</div>";
                            }
                        }
                        else
                        {
                            TempData["message1"] = "<div class=\"error1 closable\">Your status has been deactivated.Please contact to admin</div>";
                            return new RedirectResult(ReturnUrl);
                        }
                    }
                    else
                    {
                        TempData["message1"] = "<div class=\"error1 closable\">Email or password is incorrect</div>";
                    }
                }
                else
                {
                    TempData["message1"] = "<div class=\"error1 closable\">The given details are not a Member of Cultural Secretary</div>";
                }
            }
            catch
            {
                TempData["message1"] = "<div class=\"error closable\">Failed Transaction</div>";
            }
            return RedirectToAction("QRCodeView", "Event", new { strcul = "login-failed" });
        }
        public ActionResult ForgotPassword(string cname = "")
        {
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
            ViewBag.cname = cname;
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            try
            {
                int _status = 0;
                Entities.Members _objMember = new Entities.Members();
                if (model.Email != null && model.Email != "")
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(model.Email, ref _status);
                }
                else if (model.UserName != null && model.UserName != "")
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(model.UserName, ref _status);
                }

                if (_objMember == null || _objMember.MemberId == 0)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "You've entered an incorrect email";
                }
                else
                {
                    // ✅ 30 min reset link
                    string token = BuildResetToken(_objMember.Email);
                    string resetLink = Url.Action("LogOn", "Members",
                        new { area = "User", rp = token, cname = model.ChapterName }, Request.Scheme);

                    int status = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Forgot Password", 0, ref status);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(_objMember.FirstName));
                        body.Replace("[EMAIL]", _objMember.Email);
                        body.Replace("[RESETLINK]", resetLink);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ChapterName]", model.ChapterName);
                        body.Replace("[Year]", DateTime.Now.Year.ToString());
                        string subject = objTemplates.Subject;
                        subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                        _sentmail.SendMailSendinbrevo(_objMember.Email, subject, body.ToString());
                    }

                    TempData["messageType"] = "success";
                    TempData["message"] = "Password reset link sent to your registered email.";
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction.";
            }
            return RedirectToAction("ForgotPassword", "Members");
        }


        private string BuildResetToken(string email)
        {
            string code = Convert.ToHexString(RandomNumberGenerator.GetBytes(6)).ToLower();
            _cache.Set("rp_" + code, email, TimeSpan.FromMinutes(30));   // 30 నిమిషాలు మాత్రమే
            return code;
        }

        private bool TryReadResetToken(string token, out string email)
        {
            email = "";
            if (string.IsNullOrWhiteSpace(token)) return false;
            if (!_cache.TryGetValue("rp_" + token.Trim().ToLower(), out string found)) return false;
            email = found;
            return true;
        }
        [HttpGet]
        public IActionResult CheckResetToken(string token)
        {
            if (TryReadResetToken(token, out string email))
                return Json(new { ok = true, email = email });
            return Json(new { ok = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPasswordSubmit(string token, string NewPassword, string ConfPassword1)
        {
            try
            {
                if (!TryReadResetToken(token, out string email))
                    return Json(new { ok = false, message = "Password link expired. Please try again from the Forgot Password page." });

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfPassword1)
                    return Json(new { ok = false, message = "Passwords do not match." });

                if (!System.Text.RegularExpressions.Regex.IsMatch(NewPassword, @"^(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$"))
                    return Json(new { ok = false, message = "Minimum 8 characters, at least one special character & one numeric character." });

                int st = 0;
                var member = _Members.GetMemberFullDetailsByEmail(email, ref st);
                if (member == null || member.MemberId == 0)
                    return Json(new { ok = false, message = "Member not found." });

                string hash = AWS_AssociationToolCore.BLL.Password.ComputeHash(NewPassword, "SHA512", null);
                Int64 res = _Members.ChangePassword(member.MemberId, hash, "");

                return Json(res == 1
                    ? new { ok = true, message = "Password updated successfully." }
                    : new { ok = false, message = "Failed to update password. Please try again." });
            }
            catch
            {
                return Json(new { ok = false, message = "Failed Transaction." });
            }
        }
        [HttpPost]
        public ActionResult ForgotPasswordBKP(ForgotPasswordModel model)
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            try
            {
                int _status = 0;
                Entities.Members _objMember = new Entities.Members();
                if (model.Email != null && model.Email != "")
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(model.Email, ref _status);
                }
                else if (model.UserName != null && model.UserName != "")
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(model.UserName, ref _status);
                }

                if (_objMember.MemberId == 0)
                {

                    TempData["messageType"] = "error";
                    TempData["message"] = "You've entered an incorrect email";


                    //TempData["messagelogin"] = "<div class=\"error1 closable\">You've entered an incorrect email</div>";
                }
                else
                {
                    string _password = BLL.Password.GetUniqueKey(8);
                    string _passwordhash = BLL.Password.ComputeHash(_password, "SHA512", null);

                    string FType = "Yes";

                    Int64 _pstatus = _Members.ChangePassword(_objMember.MemberId, _passwordhash, FType);
                    if (_pstatus == 1)
                    {
                        int status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Forgot Password", 0, ref status);
                        if (objTemplates != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(_objMember.FirstName));
                            body.Replace("[EMAIL]", _objMember.Email);
                            body.Replace("[PASSWORD]", _password);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ChapterName]", model.ChapterName);
                            body.Replace("[Year]", DateTime.Now.Year.ToString());
                            string subject = objTemplates.Subject;
                            subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                            _sentmail.SendMailSendinbrevo(_objMember.Email, subject, body.ToString());
                        }

                        //TempData["messagelogin"] = "<div class=\"success closable\"> Password details sent to registered email. </div>";

                        TempData["messageType"] = "success";
                        TempData["message"] = "Password details sent to registered email.";

                    }
                    else
                    {
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed generating password.";

                        // TempData["messagelogin"] = "<div class=\"error1 closable\"> Failed generating password. </div>";
                    }
                }
            }
            catch
            {

                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction.";
                // TempData["Message"] = "<div class=\"error1 closable\">Failed Transaction.</div>";
            }
            return RedirectToAction("ForgotPassword", "Members");
        }
        [HttpGet]
        public async Task<IActionResult> LogOff(string cname = "", string type = "", string ChildrenId = "")
        
        {
            string chapterName = "";
            if (Request.Cookies.TryGetValue("chaperInfo", out var cookieValue))
            {
                chapterName = cookieValue;
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            if (type == "member")
            {
                return RedirectToAction("AddMember", "Members", new { cname = chapterName, ChildrenId = ChildrenId });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { cname = chapterName });
            }
        }
        #endregion

        #region Profile

        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        public ActionResult Profile(string cname = "", string rs = "")
        {
            Entities.Members _objMember = new Entities.Members();
            try
            {
                int qstatus = 0;
                Int64 ChapterId = 0;
                int _qstatus = 0;

                // ✅ Get Email from Claims
                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userName))
                {
                    TempData["message"] = "<div class=\"error1 closable\">Session expired. Please login again.</div>";
                    return RedirectToAction("LogOn", "Members");
                }

                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);

                if (_objMember == null || _objMember.MemberId == 0)
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("LogOn", "Members");
                }

                // ✅ Resolve chapter safely
                if (!string.IsNullOrEmpty(cname))
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                    ChapterId = objChapters?.ChapterId ?? 0;
                }

                // ✅ Default to global if no chapter found
                if (ChapterId == 0)
                {
                    ChapterId = 1;
                    cname = "global";
                }

                if (_qstatus == 1)
                {
                    ViewBag.objMemberDetails = _objMember;
                    ViewBag.status = _qstatus;
                    ViewBag.objMembersType = objMembers;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objMembers = _objMember;
            ViewBag.cname = cname;   // will be "global" if not provided
            ViewBag.rs = rs;
            return View();
        }
        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        public ActionResult ProfileEdit()
        {
            Entities.Members _objMember = new Entities.Members();
            try
            {
                string currentUserEmail = "";
                currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
                int _qstatus = 0;
                _objMember = _Members.GetMemberFullDetailsByEmail(currentUserEmail, ref _qstatus);

                if (_objMember.MemberId == 0)
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("Index1", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.objMembers = _objMember;
            return View();
        }

        public ActionResult MembershipRenewal(string MembershipType = "", string str = "", string cname = "")
        {
            Entities.Members _objMember = new Entities.Members();

            // ✅ Read cookie safely
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }
            string currentUserEmail = "";
            currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            int qstatus = 0;
            Int64 ChapterId = 0;
            try
            {
                int _qstatus = 0;
                string Email = "";

                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                _objMember = _Members.GetMemberFullDetailsByEmail(currentUserEmail, ref _qstatus);

                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(_objMember.ChapterName, ref qstatus);
                    //cname = _objMember.ChapterName;
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                    cname = "global";
                }

                if (_objMember.MemberId == 0)
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("Index1", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.MembershipType = MembershipType;
            ViewBag.objMemberDetails = _objMember;
            ViewBag.objMembers = objMembers;
            ViewBag.str = str;
            ViewBag.cname = cname;
            return View();
        }

        [HttpPost]
        //public async Task<ActionResult> MembershipRenewal(HttpPostedFileBase file, HttpPostedFileBase filezelle, Entities.Members objMembers, Entities.MembershipOrders objMembershipOrders, string recaptchaResponse = "", string cname = "")
        //{
        //    string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
        //    objMembers.WebsiteAddress = ip;
        //    Int64 LRstatus = 0;


        //    List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

        //    Entities.LogReport logs = new Entities.LogReport();
        //    int status1 = 0;
        //    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);



        //    string YoursecretKey = objAppInfo.CapchaSecreatKey; //Put Your Secret Key .

        //    //If capcha is "Yes", recapcha verification is process, if capcha is "No" without verification we can submit form...
        //    var capcha = objAppInfo.ShowCapcha;

        //    // Verify reCAPTCHA token
        //    bool isHuman = await BLL.Common.VerifyRecaptchaToken(recaptchaResponse);


        //    if (capcha == "No")
        //    {
        //        isHuman = true;
        //    }
        //    else if (capcha == "Yes")
        //    {
        //        isHuman = true;
        //    }



        //    if (isHuman == true)
        //    {



        //        try
        //        {
        //            string imageurl = "NA";

        //            string receipturl = "NA";

        //            if (file != null)
        //            {
        //                file.SaveAs(objAppInfo.UploadPath + "\\temp\\" + file.FileName);
        //                WebImage image3 = new WebImage(objAppInfo.UploadPath + "\\temp\\" + file.FileName);
        //                receipturl = (image3 != null ? image3.ImageFormat : "NA");
        //            }

        //            if (filezelle != null)
        //            {
        //                filezelle.SaveAs(objAppInfo.UploadPath + "\\temp\\" + filezelle.FileName);
        //                WebImage image3 = new WebImage(objAppInfo.UploadPath + "\\temp\\" + filezelle.FileName);
        //                receipturl = (image3 != null ? image3.ImageFormat : "NA");
        //            }
        //            if (objMembers.PaymentMethod == "Paypal")
        //            {

        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "Renewal Membership";

        //                logs.LogDescription = "Client side PayPal Payment Process is Completed(" + objMembers.FirstName + " " + objMembers.LastName + " -" + DateTime.UtcNow + ")&PaymentStatus:" + objMembers.PaymentStatus + "&TransactionId:" + objMembers.TransactionId + "";

        //                //logs.LogDescription = "Entered into PayPal Payment Process"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                lstLogReport.Add(logs);
        //            }
        //            objMembershipOrders.UpdatedBy = HttpContext.User.Identity.Name.ToString();

        //            Int64 _status = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
        //            if (file != null)
        //            {
        //                file.SaveAs(objAppInfo.UploadPath + "\\MemberReceiptImages\\" + receipturl);
        //                //file1.SaveAs(Session["UploadPath"] + "\\events\\flyers\\" + imageurl3);

        //            }
        //            if (filezelle != null)
        //            {
        //                filezelle.SaveAs(objAppInfo.UploadPath + "\\MemberReceiptImages\\" + receipturl);
        //            }

        //            if (objMembers.PaymentMethod == "AuthorizeNet")
        //            {
        //                int status = 0;
        //                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "AuthorizeNet", ref status);
        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "MembershipRenewal";

        //                logs.LogDescription = "Entered Into AuthorizeNet Payment Process(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + "  " + DateTime.UtcNow + ")";



        //                lstLogReport.Add(logs);

        //                //Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //                //Payment
        //                string PayPalRequest = "TRXTYPE=S" //S - sale transaction
        //                      + "&TENDER=C" //C - AuthorizeNet
        //                        + "&FIRSTNAME=" + objMembers.FirstName //card number
        //                        + "&STREET=" + objMembers.Address + "," + objMembers.Address2 //card number
        //                        + "&CITY=" + objMembers.City //card number
        //                        + "&STATE=" + objMembers.State //card number
        //                        + "&ZIP=" + objMembers.ZipCode //card number
        //                        + "&LASTNAME=" + objMembers.LastName //card number
        //                       + "&ACCT=" + objMembers.CardNumber.Replace(" ", "") //card number
        //                       + "&EXPDATE=" + objMembers.CSVMonth + objMembers.CSVYear.Substring(2, 2)
        //                       + "&CVV2=" + objMembers.Cvv   //card validation value (card security code)               
        //                       + "&AMT=" + objMembers.MemberAmount
        //                       + "&COMMENT1=Members"
        //                       + "&USER=" + objPaymentSettings.SuccessUrl
        //                       + "&VENDOR=" + objPaymentSettings.CancelUrl
        //                       + "&PARTNER=" + objPaymentSettings.NotifyUrl
        //                       + "&PWD=" + objPaymentSettings.TokenNo;

        //                // Create an instantce of PayflowNETAPI.
        //                PayflowNETAPI PayflowNETAPI = new PayflowNETAPI();

        //                // RequestId is a unique string that is required for each & every transaction. 
        //                // The merchant can use her/his own algorithm to generate this unique request id or 
        //                // use the SDK provided API to generate this as shown below (PayflowUtility.RequestId).
        //                string PayPalResponse = PayflowNETAPI.SubmitTransaction(PayPalRequest, PayflowUtility.RequestId);

        //                //place data from PayPal into a namevaluecollection
        //                NameValueCollection RequestCollection = BLL.Common.GetPayPalCollection(PayflowNETAPI.TransactionRequest);
        //                NameValueCollection reqcollection = RequestCollection;
        //                string ACCT = reqcollection["ACCT"];
        //                string EXPDATE = reqcollection["EXPDATE"];

        //                NameValueCollection ResponseCollection = BLL.Common.GetPayPalCollection(PayPalResponse);
        //                NameValueCollection collection = ResponseCollection;
        //                string RESPMSG = collection["RESPMSG"];
        //                string PNREF = collection["PNREF"];
        //                if (RESPMSG != "Approved")
        //                {
        //                    objMembershipOrders.OrderType = objMembers.OrderType;
        //                    objMembershipOrders.Amount = objMembers.MemberAmount;
        //                    objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //                    objMembershipOrders.PaymentMethod = "AuthorizeNet";
        //                    objMembershipOrders.TransactionId = "";
        //                    objMembershipOrders.PaymentStatus = "Pending";
        //                    objMembershipOrders.MemberId = objMembers.MemberId;
        //                    objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //                    objMembershipOrders.MembershipType = objMembers.MembershipType;
        //                    objMembershipOrders.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")); ;
        //                    objMembershipOrders.UpdatedBy = objMembers.FirstName;

        //                    Int64 result = _Members.InsertMemberOrder(objMembershipOrders);

        //                    logs = new Entities.LogReport();
        //                    logs.LogTitle = "Members"; logs.LogDescription = "RESPMSG Not Approved - Members Orders Inserted"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName + objMembers.MemberId;
        //                    lstLogReport.Add(logs);
        //                }
        //                if (RESPMSG == "Approved")
        //                {
        //                    objMembershipOrders.PaymentMethod = "AuthorizeNet";
        //                    objMembershipOrders.TransactionId = PNREF;
        //                    objMembershipOrders.PaymentStatus = "Completed";
        //                    objMembershipOrders.CSVExpiry = objMembershipOrders.CSVMonth + "-" + objMembershipOrders.CSVYear;
        //                    objMembershipOrders.CardNumber = ACCT;
        //                    objMembershipOrders.MemberId = objMembers.MemberId;
        //                    objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //                    objMembershipOrders.MembershipType = objMembers.MembershipType;
        //                    objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //                    objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                    objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

        //                    //objMembershipOrders.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        //                    objMembershipOrders.UpdatedBy = objMembers.FirstName;

        //                    if (objMembers.Donate == "Yes" && objMembers.DonationAmount != 0)
        //                    {
        //                        objMembershipOrders.Amount = objMembers.MemberAmount - objMembers.DonationAmount;
        //                    }
        //                    else
        //                    {
        //                        objMembershipOrders.Amount = objMembers.MemberAmount;
        //                    }
        //                    //Int64 result = _Members.InsertMemberOrder(objMembershipOrders);
        //                    Int64 result = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);


        //                    int result6 = 0;
        //                    objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result6);


        //                    logs = new Entities.LogReport();
        //                    logs.LogTitle = "Members";
        //                    logs.LogDescription = "RESPMSG Approved - Members Orders Inserted"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName + objMembers.MemberId;
        //                    lstLogReport.Add(logs);

        //                    //if (objMembers.Donate == "Yes" && objMembers.DonationAmount != 0)
        //                    //{
        //                    //    objServiceDonations.PaymentMethod = "PayPal";
        //                    //    objServiceDonations.TransactionId = PNREF;
        //                    //    objServiceDonations.PaymentStatus = "Completed";
        //                    //    objServiceDonations.ServiceDonationId = ServiceDonationId;
        //                    //    objServiceDonations.OrderDate = DateTime.UtcNow;
        //                    //    objServiceDonations.UpdatedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
        //                    //    objServiceDonations.Amount = objMembers.DonationAmount;
        //                    //    Int64 dresult = _ServiceDonations.UpdateServiceDonations(objServiceDonations);

        //                    //    logreport("Updated Service Donations", Request.Path, filepath);

        //                    //}
        //                    if (result == 1)
        //                    {
        //                        ViewBag.PayerEmail = objMembers.Email;
        //                        ViewBag.OrderType = objMembershipOrders.OrderType;
        //                        ViewBag.UserName = objMembers.FirstName;
        //                        //ViewBag.ReceiverEmail = objPDT.ReceiverEmail;
        //                        //ViewBag.AddressCity = objPDT.AddressCity;
        //                        //ViewBag.AddressCountry = objPDT.AddressCountry;
        //                        //ViewBag.AddressName = objPDT.AddressName;
        //                        //ViewBag.AddressState = objPDT.AddressState;
        //                        //ViewBag.AddressStreet = objPDT.AddressStreet;
        //                        //ViewBag.AddressZip = objPDT.AddressZip;
        //                        ViewBag.GrossTotal = objMembershipOrders.Amount;
        //                        ViewBag.PaymentStatus = "Completed";
        //                        ViewBag.TransactionId = PNREF;
        //                        ViewBag.MemberId = objMembers.MemberId;
        //                        DateTime Date = DateTime.UtcNow;
        //                        ViewBag.PaymentDate = Date.ToString("MM/dd/yyyy");
        //                        ViewBag.Type = objMembers.MembershipType;

        //                        int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
        //                        if (objTemplates != null)
        //                        {
        //                            StringBuilder body = new StringBuilder();
        //                            body.Append(objTemplates.Description);
        //                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
        //                            body.Replace("[MemberId]", objMembers.SpouseCell);
        //                            body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                            body.Replace("[FirstName]", objMembers.FirstName);
        //                            body.Replace("[LastName]", objMembers.LastName);
        //                            body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                            body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                            body.Replace("[Email]", objMembers.Email);
        //                            body.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                            body.Replace("[MembershipType]", objMembers.MembershipType);
        //                            body.Replace("[ChapterName]", objMembers.ChapterName);
        //                            body.Replace("[TransactionId]", PNREF);
        //                            body.Replace("[PaymentType]", "AuthorizeNet");
        //                            body.Replace("[PaymentStatus]", "Completed");
        //                            body.Replace("[PaymentDate]", Date.ToShortDateString());
        //                            body.Replace("[SiteName]", objAppInfo.SiteName);
        //                            body.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                            //body.Replace("[Password]", Password);


        //                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for -" + objAppInfo.SiteName, body.ToString());


        //                            logs = new Entities.LogReport();
        //                            logs.LogTitle = "Members";

        //                            logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + " - " + DateTime.UtcNow + ")";

        //                            //logs.LogDescription = "User Notification Email Sent"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                            lstLogReport.Add(logs);
        //                        }


        //                        int statusAdmin = 0;
        //                        Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
        //                        if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
        //                        {
        //                            StringBuilder body1 = new StringBuilder();
        //                            body1.Append(objTemplatesAdmin.Description);
        //                            body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                            body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                            body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                            body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                            body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                            body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                            body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                            body1.Replace("[MemberId]", objMembers.SpouseCell);
        //                            body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                            body1.Replace("[FirstName]", objMembers.FirstName);
        //                            body1.Replace("[LastName]", objMembers.LastName);
        //                            body1.Replace("[Email]", objMembers.Email);
        //                            body1.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                            body1.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                            body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                            body1.Replace("[SpouseCell]", objMembers.SpouseCell);
        //                            body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                            body1.Replace("[MembershipType]", objMembers.MembershipType);
        //                            body1.Replace("[ChapterName]", objMembers.ChapterName);
        //                            body1.Replace("[TransactionId]", PNREF);
        //                            body1.Replace("[PaymentType]", "AuthorizeNet");
        //                            body1.Replace("[PaymentStatus]", "Completed");
        //                            body1.Replace("[PaymentDate]", Date.ToShortDateString());
        //                            body1.Replace("[SiteName]", objAppInfo.SiteName);


        //                            var responsce = _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());
        //                            logs = new Entities.LogReport();
        //                            logs.LogTitle = "Members";

        //                            logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + " - " + DateTime.UtcNow + ")";


        //                            //logs.LogDescription = "Admin Notification Email Sent"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                            lstLogReport.Add(logs);
        //                        }

        //                        LRstatus = _Users.InsertLogReport(lstLogReport);


        //                        return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });

        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("Error404", "Error");
        //                    }
        //                }
        //                else
        //                {
        //                    StringBuilder body1 = new StringBuilder();
        //                    body1.Append("<p>Dear Admin, <br /><br />For your convenience, quick reminder email that payment for Member Registration has been failed. <br />");
        //                    body1.Append("<br /><b>Member Id:</b> " + objMembers.MemberId + "<br />");
        //                    body1.Append("<b>User Name:</b> " + objMembers.FirstName + "" + objMembers.LastName + "<br />");
        //                    body1.Append("<b>Email:</b> " + objMembers.Email + "<br />");
        //                    body1.Append("<b>Phone No:</b> " + objMembers.MobilePhone + "<br />");
        //                    body1.Append("<b>Payment Status:</b> Pending <br />");
        //                    body1.Append("<b>Paypal Response:</b> " + RESPMSG + "<br /><br />");
        //                    body1.Append("Thank You,<br />Admin</p>");

        //                    string tomail = ConfigurationManager.AppSettings["paymentfailemail"];


        //                    _sentmail.SendMailSendinbrevowithfrom(tomail, objAppInfo.MemberEmail, "Quick Remainder Payment failure Email for Event Registration on " + DateTime.Now.ToString("MMM dd - yyyy") + " - " + objAppInfo.SiteName, body1.ToString());



        //                    logs = new Entities.LogReport();
        //                    logs.LogTitle = "Members";


        //                    logs.LogDescription = "Payment failed" + RESPMSG; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName + objMembers.MemberId;
        //                    lstLogReport.Add(logs);


        //                    LRstatus = _Users.InsertLogReport(lstLogReport);


        //                    return RedirectToAction("PaymentFailed", "Home");
        //                }
        //            }


        //            else if (objMembers.PaymentMethod == "PayPal")
        //            {

        //                decimal paymentAmount = objMembers.MemberAmount;


        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "Renewal Membership";


        //                logs.LogDescription = "Payment Responsive Got Approved(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + "  " + DateTime.UtcNow + ")";


        //                // logs.LogDescription = "Payment Responsive Get Approved"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                lstLogReport.Add(logs);
        //                string Username = "";


        //                Entities.Members objMember = new Entities.Members();
        //                //int _status3 = 0;
        //                Int64 result = 0;
        //                int result2 = 0;
        //                //string Username = "";
        //                //objMember = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result2);


        //                if (objMembers.FirstName != null)
        //                {
        //                    Username = objMembers.FirstName + " " + objMembers.LastName;
        //                }
        //                else
        //                {
        //                    Username = HttpContext.User.Identity.Name.ToString();
        //                }



        //                if (Username != null)
        //                {

        //                    //Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();


        //                    objMembershipOrders.OrderType = "Paypal";
        //                    objMembershipOrders.Amount = objMembers.MemberAmount;
        //                    objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //                    objMembershipOrders.PaymentMethod = "PayPal";
        //                    objMembershipOrders.TransactionId = objMembers.TransactionId;
        //                    objMembershipOrders.PaymentStatus = "Completed";
        //                    objMembershipOrders.MemberId = objMembers.MemberId;
        //                    objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //                    objMembershipOrders.MembershipType = objMembers.MembershipType;
        //                    objMembershipOrders.UpdatedBy = Username;
        //                    objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                    objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                    result = _Members.InsertMemberOrder(objMembershipOrders);


        //                    int result5 = 0;
        //                    objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result5);


        //                    int _status3 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Renewal Membership", 0, ref _status3);
        //                    if (objTemplates != null)
        //                    {
        //                        StringBuilder body = new StringBuilder();
        //                        body.Append(objTemplates.Description);

        //                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
        //                        body.Replace("[MemberId]", objMembers.SpouseCell);
        //                        body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                        body.Replace("[FirstName]", objMembers.FirstName);
        //                        body.Replace("[LastName]", objMembers.LastName);
        //                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                        body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                        body.Replace("[Email]", objMembers.Email);
        //                        body.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                        body.Replace("[MembershipType]", objMembers.MembershipType);
        //                        body.Replace("[ChapterName]", objMembers.ChapterName);

        //                        body.Replace("[PaymentType]", "Paypal");

        //                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

        //                        body.Replace("[SiteName]", objAppInfo.SiteName);
        //                        body.Replace("[MemberSkils]", objMembers.MemberSkils);

        //                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);

        //                        body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());



        //                        var responsce = _sentmail.SendMailSendinbrevo(objMembers.Email, objTemplates.Subject, body.ToString());

        //                        logs = new Entities.LogReport();
        //                        logs.LogTitle = "Renewal Membership";

        //                        //logs.LogDescription = "responsce(" + objServiceDonations.FirstName + " " + objServiceDonations.LastName + " - " + ServiceDonationId + ")";
        //                        logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + " - " + DateTime.UtcNow + ")";

        //                        //  logs.LogDescription = responsce; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
        //                        lstLogReport.Add(logs);

        //                    }
        //                    int status2 = 0; Entities.MailTemplates objTemplates2 = _MailTemplates.GetMailTemplateById("Renewal Membership Admin", 0, ref status2);
        //                    if (objTemplates2 != null)
        //                    {
        //                        StringBuilder body = new StringBuilder();
        //                        body.Append(objTemplates2.Description);
        //                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                        body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                        body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMember.FirstName));
        //                        body.Replace("[PaymentStatus]", objMembers.objMembershipOrder.PaymentStatus);
        //                        body.Replace("[TransactionID]", objMembers.objMembershipOrder.TransactionId.ToString());

        //                        body.Replace("[Amount]", objMembers.objMembershipOrder.Amount.ToString());
        //                        body.Replace("[PType]", "Paypal");
        //                        body.Replace("[MTYPE]", objMembers.MembershipType.ToString());
        //                        body.Replace("[MEMBERID]", objMembers.MemberSkils);
        //                        body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                        body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objMembers.FirstName));
        //                        body.Replace("[Email]", (objMembers.Email));
        //                        body.Replace("[LastName]", BLL.Common.UppercaseFirst(objMembers.LastName));
        //                        body.Replace("[MembershipType]", BLL.Common.UppercaseFirst(objMembers.MembershipType));
        //                        body.Replace("[Phone]", BLL.Common.UppercaseFirst(objMembers.MobilePhone));
        //                        body.Replace("[SiteName]", objAppInfo.SiteName);
        //                        body.Replace("[PaymentDate]", objMembers.objMembershipOrder.OrderDate.ToString("MM/dd/yyyy"));

        //                        body.Replace("[ChapterName]", objMembers.ChapterName);

        //                        var responsce = _sentmail.SendMailSendinbrevo(objAppInfo.SenderEmail, objTemplates.Subject, body.ToString());

        //                        logs = new Entities.LogReport();
        //                        logs.LogTitle = "Renewal Membership";

        //                        //logs.LogDescription = "responsce(" + objServiceDonations.FirstName + " " + objServiceDonations.LastName + " - " + ServiceDonationId + ")";
        //                        logs.LogDescription = responsce + "(" + objMembers.FirstName + " " + objMembers.LastName + " - " + objMembers.MemberId + " - " + DateTime.UtcNow + ")";

        //                        //  logs.LogDescription = responsce; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objServiceDonations.FirstName + " " + objServiceDonations.LastName;
        //                        lstLogReport.Add(logs);

        //                    }


        //                    // BLL.Common.SendMailwithCC(objAppInfo.SecretaryPhone, objAppInfo.SenderEmail, objAppInfo.PresidentPhone + "," + objAppInfo.CompanyEmail, objTemplates2.Subject, body.ToString());


        //                    LRstatus = _Users.InsertLogReport(lstLogReport);

        //                    return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });

        //                }

        //            }

        //            else if (objMembers.PaymentMethod == "Cash/Cheque")
        //            {
        //                Int64 result = 0;
        //                //Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //                objMembershipOrders.Amount = objMembers.MemberAmount;
        //                objMembershipOrders.OrderType = objMembers.OrderType;
        //                objMembershipOrders.MembershipOrderId = objMembers.MembershipOrderId;
        //                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //                objMembershipOrders.PaymentMethod = "Cash/Cheque";
        //                objMembershipOrders.PaymentStatus = "Pending";
        //                objMembershipOrders.MemberId = objMembers.MemberId;
        //                objMembershipOrders.UserComment = objMembers.UserComment;
        //                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //                objMembershipOrders.PaymentBy = objMembers.PaymentBy;
        //                objMembershipOrders.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //                objMembershipOrders.IsVolunteer = objMembers.IsVolunteer;
        //                objMembershipOrders.BankName = objMembers.BankName;
        //                objMembershipOrders.ChequeNo = objMembers.ChequeNo;
        //                objMembershipOrders.ChequeDate = objMembers.ChequeDate;
        //                Session["MembersType"] = objMembers.MembershipType;

        //                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();


        //                result = _Members.InsertMemberOrder(objMembershipOrders);

        //                //result = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
        //                int result6 = 0;
        //                objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result6);


        //                int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
        //                if (objTemplates != null)
        //                {
        //                    StringBuilder body = new StringBuilder();
        //                    body.Append(objTemplates.Description);
        //                    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);

        //                    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                    body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                    body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
        //                    body.Replace("[MemberId]", objMembers.SpouseCell);
        //                    body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                    body.Replace("[FirstName]", objMembers.FirstName);
        //                    body.Replace("[LastName]", objMembers.LastName);
        //                    body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                    body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                    body.Replace("[Email]", objMembers.Email);
        //                    body.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                    body.Replace("[MembershipType]", objMembers.MembershipType);
        //                    body.Replace("[TransactionId]", "NA");
        //                    body.Replace("[PaymentType]", "Cash/Cheque");
        //                    body.Replace("[PaymentStatus]", "Pending");
        //                    body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                    body.Replace("[SiteName]", objAppInfo.SiteName);
        //                    body.Replace("[ChapterName]", objMembers.ChapterName);
        //                    body.Replace("[MemberSkils]", objMembers.MemberSkils);


        //                    _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());

        //                }


        //                int statusAdmin = 0;
        //                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
        //                if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
        //                {
        //                    StringBuilder body1 = new StringBuilder();
        //                    body1.Append(objTemplatesAdmin.Description);
        //                    body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                    body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                    body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                    body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                    body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                    body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                    body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                    body1.Replace("[MemberId]", objMembers.SpouseCell);
        //                    body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                    body1.Replace("[FirstName]", objMembers.FirstName);
        //                    body1.Replace("[LastName]", objMembers.LastName);
        //                    body1.Replace("[Email]", objMembers.Email);
        //                    body1.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                    body1.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                    body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                    body1.Replace("[SpouseCell]", objMembers.SpouseCell);
        //                    body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                    body1.Replace("[MembershipType]", objMembers.MembershipType);
        //                    body1.Replace("[TransactionId]", "NA");
        //                    body1.Replace("[PaymentType]", "Cash/Cheque");
        //                    body1.Replace("[PaymentStatus]", "Completed");
        //                    body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                    body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                    body1.Replace("[ChapterName]", objMembers.ChapterName);

        //                    _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());

        //                    return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
        //                }

        //            }
        //            else if (objMembers.PaymentMethod == "Zelle")
        //            {


        //                Int64 result = 0;
        //                //Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //                objMembershipOrders.Amount = objMembers.MemberAmount;
        //                objMembershipOrders.ExpiryDate = objMembers.ExpiryDate;
        //                objMembershipOrders.PaymentMethod = "Zelle";
        //                objMembershipOrders.PaymentStatus = "Pending";
        //                objMembershipOrders.MemberId = objMembers.MemberId; ;
        //                objMembershipOrders.PaymentBy = objMembers.PaymentBy;
        //                objMembershipOrders.MembershipTypeId = objMembers.MembershipTypeId;
        //                objMembershipOrders.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



        //                result = _Members.InsertMemberOrder(objMembershipOrders);

        //                //result = _Members.InsertMemberOrderRenewal(objMembershipOrders, ref imageurl, ref receipturl);
        //                int result6 = 0;
        //                objMembers = _Members.GetMemberFullDetailsById(objMembers.MemberId, ref result6);
        //                int _status1 = 0; Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
        //                if (objTemplates != null)
        //                {
        //                    StringBuilder body = new StringBuilder();
        //                    body.Append(objTemplates.Description);
        //                    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                    body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                    body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
        //                    body.Replace("[MemberId]", objMembers.SpouseCell);
        //                    body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                    body.Replace("[FirstName]", objMembers.FirstName);
        //                    body.Replace("[LastName]", objMembers.LastName);
        //                    body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                    body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                    body.Replace("[Email]", objMembers.Email);
        //                    body.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                    body.Replace("[MembershipType]", objMembers.MembershipType);
        //                    body.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                    body.Replace("[TransactionId]", "NA");
        //                    body.Replace("[PaymentType]", "Zelle");
        //                    body.Replace("[PaymentStatus]", "Pending");
        //                    body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                    body.Replace("[SiteName]", objAppInfo.SiteName);
        //                    body.Replace("[ChapterName]", objMembers.ChapterName);


        //                    _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());

        //                }


        //                int statusAdmin = 0;
        //                Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
        //                if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
        //                {
        //                    StringBuilder body1 = new StringBuilder();
        //                    body1.Append(objTemplatesAdmin.Description);
        //                    body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                    body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                    body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                    body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                    body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                    body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                    body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                    body1.Replace("[MemberId]", objMembers.SpouseCell);
        //                    body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                    body1.Replace("[FirstName]", objMembers.FirstName);
        //                    body1.Replace("[LastName]", objMembers.LastName);
        //                    body1.Replace("[Email]", objMembers.Email);
        //                    body1.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                    body1.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                    body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                    body1.Replace("[SpouseCell]", objMembers.SpouseCell);
        //                    body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                    body1.Replace("[MembershipType]", objMembers.MembershipType);
        //                    body1.Replace("[TransactionId]", "NA");
        //                    body1.Replace("[PaymentType]", "Zelle");
        //                    body1.Replace("[PaymentStatus]", "Completed");
        //                    body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                    body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                    body1.Replace("[ChapterName]", objMembers.ChapterName);

        //                    _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());


        //                }

        //            }
        //            else if (objMembers.PaymentMethod == "Stripe")
        //            {


        //                Int64 result = 0;
        //                Entities.MembershipOrders objMembershipOrders1 = new Entities.MembershipOrders();
        //                objMembershipOrders1.Amount = objMembers.MemberAmount;
        //                objMembershipOrders1.ExpiryDate = objMembers.ExpiryDate;
        //                objMembershipOrders1.MembershipOrderId = objMembers.MembershipOrderId;
        //                objMembershipOrders1.PaymentMethod = "Stripe";
        //                objMembershipOrders1.PaymentStatus = "Completed";
        //                objMembershipOrders1.MemberId = objMembers.MemberId; ;
        //                objMembershipOrders1.PaymentBy = objMembers.PaymentBy;
        //                objMembershipOrders1.MembershipTypeId = objMembers.MembershipTypeId;
        //                objMembershipOrders1.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //                objMembershipOrders1.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                objMembershipOrders1.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();



        //                result = _Members.InsertMemberOrder(objMembershipOrders1);
        //                int status2 = 0;

        //                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);


        //                // Set your Stripe Secret Key
        //                StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;
        //                string currency = "usd";
        //                var priceOptions = new PriceCreateOptions
        //                {
        //                    UnitAmount = (long)(objMembers.MemberAmount * 100),
        //                    Currency = currency.ToLower(),
        //                    ProductData = new PriceProductDataOptions { Name = "Sample Product" }
        //                };

        //                var priceService = new PriceService();
        //                var price = priceService.Create(priceOptions);

        //                var sessionOptions = new SessionCreateOptions
        //                {
        //                    PaymentMethodTypes = new List<string> { "card" },
        //                    LineItems = new List<SessionLineItemOptions>
        //                            {
        //                                new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
        //                            },
        //                    Mode = "payment",
        //                    SuccessUrl = $"{objAppInfo.BaseUrl}Members/RenewalAcknowledgement?session_id={{CHECKOUT_SESSION_ID}}&MemberId={objMembers.MemberId}&PaymentMethod=Stripe&cname={cname}",
        //                    CancelUrl = Url.Action("RenewalAcknowledgement", "Members", null, Request.Url.Scheme)
        //                };

        //                var sessionService = new SessionService();
        //                var session = sessionService.Create(sessionOptions);

        //                // Store Session ID in Cookie (Fallback)
        //                Response.Cookies.Add(new HttpCookie("StripeSessionId", session.Id));

        //                if (result == 1)
        //                {

        //                    if (!string.IsNullOrEmpty(session.Url))
        //                    {
        //                        // return Json(new { success = true, paymentLinkUrl = session.Url });
        //                        return Redirect(session.Url);

        //                    }
        //                }
        //            }
        //            else if (objMembers.PaymentMethod == "Swire Pay")
        //            {
        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "Members"; logs.LogDescription = "Card process started"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                lstLogReport.Add(logs);
        //                // Initialize HttpClient and the API request
        //                var client1 = new HttpClient();
        //                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.swirepay.com/v1/payment-link");
        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "Members"; logs.LogDescription = "Entered into Card Payment Process"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                lstLogReport.Add(logs);
        //                int status2 = 0;
        //                Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

        //                // Add API key to headers
        //                request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);


        //                // Prepare JSON content for the API request
        //                var redirectUri = Url.Action("RenewalAcknowledgement", "Members", new
        //                {
        //                    MemberId = objMembers.MemberId,  // Ensure MemberId is set
        //                    PaymentMethod = "Swire Pay",
        //                    cname = cname
        //                }, protocol: Request.Url.Scheme);
        //                decimal amount = objMembers.MemberAmount;
        //                decimal amountInINR = objMembers.MemberAmount;
        //                //decimal exchangeRate = 0.012M;  // Example exchange rate
        //                decimal amountInUSD = amountInINR * 100;// Assuming this is 50
        //                string formattedAmount = $"${amount}"; // This will give you "$50"

        //                // Or if you want to append the $ at the end, you can do:
        //                string formattedAmountWithPostfix = $"{amount}$";
        //                // Prepare JSON content with interpolated values
        //                var jsonContent = $@"
        //                                {{
        //                                    ""amount"": ""{amountInUSD}"",
        //                                    ""currencyCode"": ""USD"",
        //                                    ""paymentMethodType"": [ ""CARD"" ],
        //                                    ""redirectUri"": ""{redirectUri}"",
        //                                    ""description"": ""Membership payment for {objMembers.UserName}"",
        //                                    ""statementDescriptor"": ""Membership Payment"",
        //                                    ""expiresAt"": ""{DateTime.UtcNow.AddDays(7).AddYears(2050 - DateTime.UtcNow.Year):yyyy-MM-ddTHH:mm:ss.fffZ}"",
        //                                    ""email"": ""{objMembers.Email}"",
        //                                    ""phone"": ""{objMembers.MobilePhone}"",
        //                                    ""name"": ""{objMembers.UserName}""
        //                                }}";


        //                // Set the request content
        //                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


        //                // Send the request to the SwirePay API
        //                var response = await client1.SendAsync(request);
        //                response.EnsureSuccessStatusCode();  // Throws an exception if status code is not 2xx

        //                // Read the response content as JSON
        //                var responseBody = await response.Content.ReadAsStringAsync();

        //                // Parse the response JSON to get the 'link' field
        //                var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
        //                var paymentLink = jsonData["entity"]?["link"]?.ToString();
        //                logs.LogTitle = "Members"; logs.LogDescription = paymentLink; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                lstLogReport.Add(logs);
        //                // Check if the payment link is available and redirect to it
        //                if (!string.IsNullOrEmpty(paymentLink))
        //                {
        //                    return Redirect(paymentLink);  // Redirects the user to the payment link
        //                }
        //                else
        //                {
        //                    logs = new Entities.LogReport();
        //                    logs.LogTitle = "Members"; logs.LogDescription = "Payment link not found in the response."; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = objMembers.FirstName + " " + objMembers.LastName;
        //                    lstLogReport.Add(logs);

        //                    return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });

        //                }

        //            }
        //            TempData["message"] = "<div class=\"success closable\">Thank you. Your membership has been renewed successfully.</div>";
        //            return RedirectToAction("RenewalAcknowledgement", "Members", new { MemberId = objMembers.MemberId, cname = cname, PaymentMethod = objMembers.PaymentMethod, tid = objMembers.TransactionId });
        //            //return RedirectToAction("RenewalAcknowledgement", "Members", new { OrderType = objMembers.OrderType });
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
        //            // return RedirectToAction("AddMember", "Members");
        //            return RedirectToAction("MembershipRenewal", "Members", new { cname = cname });
        //        }

        //    }
        //    else
        //    {
        //        TempData["message"] = "<div class=\"error1 closable\">Google reCaptcha not validated Successfully</div>";
        //        // return RedirectToAction("AddMember", "Members");
        //        return RedirectToAction("MembershipRenewal", "Members", new { cname = cname });
        //    }

        //}

        //public async Task<ActionResult> RenewalAcknowledgement(Int64 MemberId = 0, string PaymentMethod = "PayPal", string OrderType = "", string cname = "", string session_id = null, string tid = "", string pstatus = "", string spPaymentLink = "")
        //{
        //    int status1 = 0; int result6 = 0;
        //    int result1 = 0;
        //    string Username = "";
        //    List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();
        //    Entities.LogReport logs = new Entities.LogReport();

        //    DateTime paydate = DateTime.UtcNow;
        //    try
        //    {

        //        if (PaymentMethod == "Stripe")
        //        {
        //            int status2 = 0;
        //            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Stripe", ref status2);

        //            StripeConfiguration.ApiKey = objPaymentSettings.PaymentUrl;

        //            if (string.IsNullOrEmpty(session_id))
        //            {
        //                // Retrieve from Cookies if missing in query string
        //                if (Request.Cookies["StripeSessionId"] != null)
        //                {
        //                    session_id = Request.Cookies["StripeSessionId"].Value;
        //                }
        //            }

        //            if (string.IsNullOrEmpty(session_id))
        //            {
        //                // Retrieve the latest session from Stripe
        //                var sessionService = new SessionService();
        //                var sessions = sessionService.List(new SessionListOptions { Limit = 1 });
        //                var latestSession = sessions.Data.Count > 0 ? sessions.Data[0] : null;

        //                if (latestSession != null)
        //                {
        //                    session_id = latestSession.Id;
        //                }
        //            }
        //            System.Diagnostics.Debug.WriteLine("Received session_id: " + session_id);
        //            if (string.IsNullOrEmpty(session_id))
        //            {
        //                throw new Exception("Session ID not found.");
        //            }

        //            var sessionServiceInstance = new SessionService();
        //            var session = sessionServiceInstance.Get(session_id);

        //            var paymentIntentService = new PaymentIntentService();
        //            var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

        //            Int64 result2 = 0;

        //            Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //            objMembershipOrders.PaymentMethod = PaymentMethod;
        //            objMembershipOrders.TransactionId = paymentIntent.Id;
        //            objMembershipOrders.PaymentStatus = "Completed";
        //            objMembershipOrders.MemberId = MemberId;
        //            objMembershipOrders.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //            objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //            objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

        //            Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);

        //            //result2 = _Members.InsertMemberOrder(objMembershipOrders);



        //        }
        //        if (PaymentMethod == "Swire Pay")
        //        {
        //            logs = new Entities.LogReport();
        //            logs.LogTitle = "Members"; logs.LogDescription = "Swire Pay Started into Acknowledgement Page" + Request.QueryString["sp-payment-link"] + ""; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = MemberId.ToString();
        //            lstLogReport.Add(logs);

        //            spPaymentLink = Request.QueryString["sp-payment-link"];
        //            var client = new HttpClient();
        //            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.swirepay.com/v1/payment-link/" + spPaymentLink + "/verify");

        //            int status2 = 0;
        //            Entities.PaymentSettings objPaymentSettings = _paymentSettings.GetPaymentSettingsDetails(0, "Swire Pay", ref status2);

        //            request.Headers.Add("x-api-key", objPaymentSettings.PaymentUrl);

        //            var response = await client.SendAsync(request);
        //            var responseData1 = await response.Content.ReadAsStringAsync();

        //            response.EnsureSuccessStatusCode();
        //            Console.WriteLine(await response.Content.ReadAsStringAsync());
        //            var responseBody = await response.Content.ReadAsStringAsync();



        //            var jsonData = JObject.Parse(responseData1);

        //            // Check if "entity" and "paymentSession" exist in the response JSON
        //            if (jsonData["entity"]?["paymentSession"] != null)
        //            {
        //                var paymentSession = jsonData["entity"]["paymentSession"];

        //                var receiptNumber = paymentSession["receiptNumber"]?.ToString();
        //                var paymentType = paymentSession["paymentType"]?.ToString();
        //                var status = paymentSession["status"]?.ToString();
        //                //var status = jsonData["entity"]?["status"]?.ToString();
        //                pstatus = paymentType;
        //                tid = receiptNumber;

        //                // Output the values
        //                Console.WriteLine($"Receipt Number: {receiptNumber ?? "Not Available"}");
        //                Console.WriteLine($"Payment Type: {paymentType ?? "Not Available"}");


        //                logs = new Entities.LogReport();
        //                logs.LogTitle = "Members"; logs.LogDescription = "Response Status is " + status + " into Acknowledgement Page"; logs.LogDate = DateTime.UtcNow; logs.InsertedBy = MemberId.ToString();
        //                lstLogReport.Add(logs);
        //                Int64 result3 = 0;
        //                Entities.MembershipOrders objMembershipOrders = new Entities.MembershipOrders();
        //                objMembershipOrders.PaymentMethod = PaymentMethod;
        //                objMembershipOrders.TransactionId = receiptNumber;
        //                objMembershipOrders.PaymentStatus = "Completed";
        //                objMembershipOrders.MemberId = MemberId;
        //                objMembershipOrders.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //                objMembershipOrders.OrderDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
        //                objMembershipOrders.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

        //                Int64 result = _Members.MemberTransactionIdUpdate(objMembershipOrders);
        //            }
        //            //var jsonData = Newtonsoft.Json.Linq.JObject.Parse(responseBody);

        //        }

        //        Entities.Members objMembers = new Entities.Members();
        //        //int result1 = 0;
        //        objMembers = _Members.GetMemberFullDetailsById(MemberId, ref result1);
        //        Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
        //        if (PaymentMethod == "Stripe" || PaymentMethod == "Swire Pay")
        //        {
        //            int _status1 = 0;
        //            Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref _status1);
        //            if (objTemplates != null)
        //            {
        //                StringBuilder body = new StringBuilder();
        //                body.Append(objTemplates.Description);
        //                body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                body.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                body.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
        //                body.Replace("[MemberId]", objMembers.SpouseCell);
        //                body.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                body.Replace("[FirstName]", objMembers.FirstName);
        //                body.Replace("[LastName]", objMembers.LastName);
        //                body.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                body.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                body.Replace("[Email]", objMembers.Email);
        //                body.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                body.Replace("[MembershipType]", objMembers.MembershipType);
        //                body.Replace("[TransactionId]", objMembers.TransactionId);
        //                body.Replace("[PaymentType]", objMembers.PaymentMethod);
        //                body.Replace("[PaymentStatus]", "Completed");
        //                body.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                body.Replace("[SiteName]", objAppInfo.SiteName);
        //                body.Replace("[ChapterName]", objMembers.ChapterName);


        //                _sentmail.SendMailSendinbrevowithfrom(objMembers.Email, objAppInfo.MemberEmail, "Thank you for Becoming a Member for - " + objAppInfo.SiteName, body.ToString());

        //            }


        //            int statusAdmin = 0;
        //            Entities.MailTemplates objTemplatesAdmin = _MailTemplates.GetMailTemplateById("Admin Member Registration", 0, ref statusAdmin);
        //            if (objTemplatesAdmin != null && objTemplatesAdmin.MailTemplateId != 0)
        //            {
        //                StringBuilder body1 = new StringBuilder();
        //                body1.Append(objTemplatesAdmin.Description);
        //                body1.Replace("[usersiteurl]", objAppInfo.BaseUrl);
        //                body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
        //                body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
        //                body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
        //                body1.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
        //                body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
        //                body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
        //                body1.Replace("[MemberId]", objMembers.SpouseCell);
        //                body1.Replace("[MEMBERID]", objMembers.MemberId.ToString());
        //                body1.Replace("[FirstName]", objMembers.FirstName);
        //                body1.Replace("[LastName]", objMembers.LastName);
        //                body1.Replace("[Email]", objMembers.Email);
        //                body1.Replace("[MobilePhone]", objMembers.MobilePhone);
        //                body1.Replace("[MemberSkils]", objMembers.MemberSkils);
        //                body1.Replace("[IntrestedArea]", objMembers.SpouseSkils);
        //                body1.Replace("[SpouseCell]", objMembers.SpouseCell);
        //                body1.Replace("[SpouseEmail]", objMembers.SpouseEmail);
        //                body1.Replace("[MembershipType]", objMembers.MembershipType);
        //                body1.Replace("[TransactionId]", objMembers.TransactionId);
        //                body1.Replace("[PaymentType]", objMembers.PaymentMethod);
        //                body1.Replace("[PaymentStatus]", "Pending");
        //                body1.Replace("[PaymentDate]", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        //                body1.Replace("[SiteName]", objAppInfo.SiteName);
        //                body1.Replace("[ChapterName]", objMembers.ChapterName);

        //                _sentmail.SendMailSendinbrevowithfrom(objAppInfo.MemberEmail, objAppInfo.MemberEmail, "Registered Member Details - " + objAppInfo.SiteName, body1.ToString());


        //            }

        //        }
        //        ViewBag.objMembers = objMembers;

        //    }
        //    catch
        //    {
        //        return RedirectToAction("Error404", "Error");
        //    }

        //    ViewBag.cname = cname;

        //    return View();
        //}

        public ActionResult GetAmount(Int64 MembershipTypeId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                BLL.MembershipTypes _MembershipTypes = new BLL.MembershipTypes();
                Entities.MembershipTypes _objMembershipTypes = _MembershipTypes.GetMembershipTypeById(MembershipTypeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objMembershipTypes });
                }
                else
                {
                    str = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error1 closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        //   [AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult ProfileEdit(Entities.Members _objMembers, List<Entities.ChildrenInfo> lstChildrenInfo, string ChapterName = "", string cname = "")
        {
            try
            {
                _objMembers.UpdatedBy = _objMembers.FirstName;
                if (cname == "")
                {

                    cname = "global";
                }
                Int64 _qstatus = _Members.UpdateMemberProfile(_objMembers);
                if (_qstatus == 1)
                {
                    if (lstChildrenInfo != null)
                    {
                        foreach (Entities.ChildrenInfo objChildrenInfo in lstChildrenInfo)
                        {
                            objChildrenInfo.MemberId = _objMembers.MemberId;
                            if (objChildrenInfo != null && objChildrenInfo.FirstName != null)
                            {
                                Int64 estatus = _Members.InsertChildrenInfo(objChildrenInfo);
                                if (estatus == -1)
                                {
                                    TempData["message"] = "<div class=\"error1 closable\">Failed inserting member details.</div>";

                                    return RedirectToAction("Profile", "Members", new { MemberId = objMembers.MemberId, cname = @cname });
                                    //return RedirectToAction("Profile", "Members", new {  });

                                }
                            }
                        }
                    }
                    TempData["message"] = "<div class=\"success closable\">Profile Details Updated.</div>";



                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed Editing profile details.</div>";


                }
                TempData["message"] = "<div class=\"success closable\">Profile Details Updated.</div>";



            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            //return RedirectToAction("Profile", "Members");
            return RedirectToAction("Profile", "Members", new { cname = @cname });
        }

        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        [HttpPost]

        [Authorize]
        public async Task<IActionResult> ProfileEmailUpdate(string cname = "", string CHEmail = "", long CEMemberId = 0)
        {
            try
            {
                long status = _Members.ProfileEmailUpdate(CHEmail, CEMemberId);

                if (status == 1)
                {
                    // ✅ Get current user claims
                    var claims = User.Claims.ToList();

                    // ✅ Remove old email claim
                    claims.RemoveAll(c => c.Type == ClaimTypes.Email);

                    // ✅ Add updated email
                    claims.Add(new Claim(ClaimTypes.Email, CHEmail));
                    claims.Add(new Claim(ClaimTypes.Name, CHEmail));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // ✅ Re-sign in user (this replaces FormsAuthentication)
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity)
                    );

                    TempData["message"] = "Updated Email successfully.";

                    if (string.IsNullOrEmpty(cname))
                        cname = "global";
                }
                else
                {
                    TempData["message"] = "Failed Editing email.";
                }
            }
            catch
            {
                TempData["message"] = "Failed Transaction";
            }

            return RedirectToAction("Profile", "Members", new { cname });
        }

        [Authorize]
        [HttpPost]
        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        public ActionResult ChangePassword(ChangePasswordModel model, string cname = "")
        {
            int status1 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
            try
            {
                if (cname == "")
                {

                    cname = "global";
                }
                if (model.CPMemberId != 0)
                {
                    int _qstatus = 0;
                    string oldpass = _Members.GetPassword(model.CPMemberId, ref _qstatus);

                    if (_qstatus == 1)
                    {
                        if (AWS_AssociationToolCore.BLL.Password.VerifyHash(model.OldPassword.Trim(), "SHA512", oldpass) == true)
                        {
                            string newpass = AWS_AssociationToolCore.BLL.Password.ComputeHash(model.NewPassword, "SHA512", null);
                            Int64 _pstatus = _Members.ChangePassword(model.CPMemberId, newpass, "");
                            if (_pstatus == 1)
                            {
                                TempData["message"] = "<div class=\"success closable\">Password changed successfully.</div>";
                                Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Change Password", 0, ref _qstatus);
                                if (objTemplates != null)
                                {
                                    objMembers = _Members.GetMemberFullDetailsById(model.CPMemberId, ref _qstatus);
                                    StringBuilder body = new StringBuilder();
                                    body.Append(objTemplates.Description);
                                    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                                    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                                    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                                    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                                    body.Replace("[TPhoneNo]", objAppInfo.CompanyPhone);
                                    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName));
                                    body.Replace("[EMAIL]", objMembers.Email);
                                    //body.Replace("[PASSWORD]", model.NewPassword);
                                    body.Replace("[SiteName]", objAppInfo.SiteName);

                                    body.Replace("[Year]", DateTime.Now.Year.ToString());

                                    string subject = objTemplates.Subject;
                                    subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);

                                    _sentmail.SendMailSendinbrevo(objMembers.Email, subject, body.ToString());
                                }
                            }
                            else
                            {
                                TempData["message"] = "<div class=\"error1 closable\">The current password is incorrect or the new password is invalid.</div>";
                            }
                        }
                        else
                        {
                            TempData["message"] = "<div class=\"error1 closable\">The current password is incorrect or the new password is invalid.</div>";
                        }
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">The current password is incorrect or the new password is invalid.</div>";
                    }
                }
                return RedirectToAction("Profile", "Members", new { cname = @cname });

            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
                return RedirectToAction("Profile", "Members", new { cname = @cname });
            }
        }

        [HttpPost]
        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        public ActionResult CheckPassword(string OldPassword, Int64 MemberId)
        {
            try
            {
                if (MemberId != 0)
                {
                    int _qstatus = 0;
                    string oldpass = _Members.GetPassword(MemberId, ref _qstatus);

                    if (_qstatus == 1)
                    {
                        if (AWS_AssociationToolCore.BLL.Password.VerifyHash(OldPassword.Trim(), "SHA512", oldpass) == true)
                        {
                            return Json(new { ok = true, data = true });
                        }
                        else
                        {
                            return Json(new { ok = false, data = false });
                        }
                    }
                    else
                    {
                        return Json(new { ok = false, data = false });
                    }
                }
                return Json(new { ok = false, data = false });
            }
            catch
            {
                return Json(new { ok = false, data = false });
            }
        }

        [HttpPost]
        //[AWS_AssociationToolCore.Areas.User.Models.SessionClass.SessionExpireFilter]
        [Authorize]
        public async Task<IActionResult> ProfilePic(IFormFile file, long PCMemberId, string cname = "")
        {
            int status1 = 0;
            var appInfo = _appinfo.GetAppInfoDetails(ref status1);

            try
            {
                if (string.IsNullOrEmpty(cname))
                    cname = "global";

                string imageurl = "NA";
                string receipturl = "NA";
                if (file != null && file.Length > 0)
                {
                    // ✅ Step 1: Sanitize and store original names
                    string originalImageFile = "";
                    string originalReceiptFile = "";
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
                    if (file != null && file.Length > 0)
                    {
                        originalImageFile = SanitizeFileName(file.FileName);
                        imageurl = originalImageFile;
                        receipturl = imageurl;
                    }
                    long status = _Members.UpdateUserProfileImage(PCMemberId, ref imageurl);


                    if (status == 1)
                    {
                        string finalFolder = Path.Combine(appInfo.UploadPath, "MemberProfileImages");
                        Directory.CreateDirectory(finalFolder);

                        if (file != null && file.Length > 0 && imageurl != "NA")
                        {
                            // imageurl now has SP-generated name e.g. "183-test.about_img_1.jpg"
                            string finalPath = Path.Combine(finalFolder, imageurl);
                            await using var stream1 = new FileStream(finalPath, FileMode.Create);
                            await file.CopyToAsync(stream1);
                        }
                        TempData["message"] = "Changed Your Profile Picture Successfully.";
                    }
                    else
                    {


                        TempData["message"] = "Failed to update profile picture.";
                    }
                }
            }
            catch
            {
                TempData["message"] = "Failed Transaction";
            }

            return RedirectToAction("Profile", "Members", new { cname });
        }
        [Authorize]
        public ActionResult DeleteChildInfo(Int64 ChildInfoId, Int64 MemberId = 0, string cname = "")
        {
            try
            {
                Int64 _status = _Members.DeleteChildInfo(ChildInfoId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"success closable\">Record Deleted Successfully</div>";
                    return RedirectToAction("Profile", "Members", new { cname });  // ✅ pass cname
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                    return RedirectToAction("Profile", "Members", new { cname });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                return RedirectToAction("Profile", "Members", new { cname });
            }
        }

        #endregion

        #region CAPTCHA

        //public CaptchaImageResult ShowCaptchaImage()
        //{
        //    return new CaptchaImageResult();
        //}
        //[HttpPost]
        //public JsonResult GetCaptcha()
        //{
        //    try
        //    {
        //        string str = HttpContext.Session["captchastring"].ToString();

        //        return Json(new { ok = true, data = str, message = "" });
        //    }
        //    catch
        //    {
        //        return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
        //    }
        //}

        #endregion

        public ActionResult Members()
        {
            int Total = 0;
            ViewBag.MembershipType = _MembershipType.GetMembershipTypesList(ref Total);

            return View();
        }

        public ActionResult Volunteers()
        {
            return View();
        }

        //public ActionResult VolunteersList(string Search = "", string SortColumn = "InsertedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        //{
        //    string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
        //    int Total = 0;
        //    try
        //    {
        //        lstVolunteers = _Volunteers.FEGetVolunteersList(Search, Sort, PageNo, Items, ref Total);
        //    }
        //    catch
        //    {
        //        Total = -1;
        //    }
        //    ViewBag.total = Total;
        //    ViewBag.pageno = PageNo;
        //    ViewBag.items = Items;
        //    ViewBag.lstVolunteers = lstVolunteers;
        //    ViewBag.sortcolumn = SortColumn;
        //    ViewBag.sortorder = SortOrder.ToLower();
        //    return View();
        //}

        #region Facebook Login
        [HttpGet]
        public async Task<IActionResult> FacebookLogin(string name = "", string first_name = "", string last_name = "", string username = "", string gender = "", string email = "", bool verified = false)
        {
            int status2 = 0;
            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);
            string msg = "";
            int status = 0;
            try
            {
                Entities.Members objMembers = _Members.GetMemberFullDetailsByEmail(email, ref status);

                if (status != -1)
                {
                    if (objMembers != null && objMembers.MemberId != 0)
                    {
                        if (objMembers != null && objMembers.MemberId != 0)
                        {
                            if (objMembers.IsApproved)
                            {
                                // ✅ Session
                                HttpContext.Session.SetString("username", objMembers.UserName);
                                HttpContext.Session.SetString("MemberID", objMembers.MemberId.ToString());

                                // ✅ Login (Claims)
                                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objMembers.Email),
                        new Claim("FullName", objMembers.UserName),
                        new Claim("MemberId", objMembers.MemberId.ToString())
                    };

                                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));

                                return RedirectToAction("Profile", "Members");
                            }
                            else
                            {
                                ViewBag.Message = "<div class=\"error1 closable\">Your status has been deactivated.Please contact to admin.</div>";
                            }
                        }
                        else
                        {
                            ViewBag.Message = "<div class=\"error1 closable\">Email or password is incorrect.</div>";
                        }
                    }
                    else
                    {
                        if (verified)
                        {
                            Int64 MemberId = 0;
                            string ImageUrl = "NA";
                            objMembers.IsApproved = true;
                            objMembers.IsLockedOut = false;
                            objMembers.FailedPasswordAttemptCount = 0;
                            objMembers.LastPasswordChangedDate = DateTime.UtcNow;
                            objMembers.LastActivityDate = DateTime.UtcNow;
                            objMembers.InsertedTime = DateTime.UtcNow;
                            objMembers.UpdatedBy = objMembers.FirstName;
                            objMembers.UpdatedTime = DateTime.UtcNow;
                            objMembers.IsActivated = true;
                            objMembers.DateActivated = DateTime.UtcNow;
                            string _password = BLL.Password.GetUniqueKey(8);
                            string _hashpassword = BLL.Password.ComputeHash(_password, "SHA512", null);
                            objMembers.Password = _hashpassword;
                            objMembers.Email = email;
                            objMembers.FirstName = first_name;
                            objMembers.LastName = last_name;
                            objMembers.MobilePhone = "";
                            string receipturl = "NA";

                            Int64 _status = _Members.FEInsertMembers(objMembers, ref MemberId, ref ImageUrl, ref receipturl);

                            if (_status == 1)
                            {
                                int status1 = 0;
                                StringBuilder body1 = new StringBuilder();
                                Entities.MailTemplates objadminTemplate = _MailTemplates.GetMailTemplateById("User Registration", 0, ref status1);
                                body1.Append(objadminTemplate.Description);
                                body1.Replace("[baseurl]", objAppInfo.BaseUrl);
                                body1.Replace("[Email]", email);
                                body1.Replace("[Password]", _password);
                                body1.Replace("[UserName]", first_name);

                                //_sentmail.SendMailSendinbrevo(email.ToString(), objadminTemplate.Subject, body1.ToString());
                                HttpContext.Session.SetString("username", objMembers.UserName);
                                HttpContext.Session.SetString("MemberID", objMembers.MemberId.ToString());
                                // ✅ Login (Claims)
                                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objMembers.Email),
                        new Claim("FullName", objMembers.UserName),
                        new Claim("MemberId", objMembers.MemberId.ToString())
                    };

                                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));
                                return RedirectToAction("Profile", "Members");
                            }
                            else
                            {
                                msg = "Failed processing your request. Please try later,..";
                            }
                        }
                        else
                        {
                            msg = "Sorry,.. Facebook account is not yet verified.";
                        }
                    }
                }
                else
                {
                    msg = "Failed processing your request. Please try later,..";
                }
            }
            catch
            { return RedirectToAction("Error404", "Error"); }

            ViewBag.msg = msg;
            return View();
        }

        #endregion Facebook Login

        #region Google Login

        [HttpGet]
        public async Task<IActionResult> ReturnFromGoogle(string name = "", string first_name = "", string last_name = "", string username = "", string gender = "", string email = "", string TransferGUID = "")
        {
            int status2 = 0;
            var objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

            string msg = "";
            int status = 0;

            try
            {
                var objMembers = _Members.GetMemberFullDetailsByEmail(email, ref status);

                // ✅ If user exists
                if (objMembers != null && objMembers.MemberId != 0)
                {
                    if (objMembers.IsApproved)
                    {
                        string fullName = objMembers.FirstName + " " + objMembers.LastName;

                        // ✅ Claims Authentication (Core way)
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objMembers.Email),
                    new Claim("FullName", fullName),
                    new Claim("MemberId", objMembers.MemberId.ToString())
                };

                        var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                        await HttpContext.SignInAsync(
                            "MyCookieAuth",
                            new ClaimsPrincipal(identity),
                            new AuthenticationProperties { IsPersistent = false }
                        );

                        // ✅ Optional session
                        HttpContext.Session.SetString("username", fullName);
                        HttpContext.Session.SetString("MemberID", objMembers.MemberId.ToString());

                        // ✅ Referrer fix
                        string url = objAppInfo.BaseUrl;

                        if (Request.Headers["Referer"].ToString() != "" &&
                            !Request.Headers["Referer"].ToString().Contains("acknowledgement"))
                        {
                            url = Request.Headers["Referer"].ToString();
                        }

                        return Redirect(url);
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Your status has been deactivated. Please contact admin.</div>";
                    }
                }
                else
                {
                    // ✅ Create new user
                    Entities.Members _objMembersInsert = new Entities.Members();
                    _objMembersInsert.Email = email;
                    _objMembersInsert.IsApproved = true;
                    _objMembersInsert.UpdatedTime = DateTime.UtcNow;
                    _objMembersInsert.UserName = first_name + " " + last_name;
                    _objMembersInsert.IsLockedOut = false;
                    string _password = BLL.Password.GetUniqueKey(8);
                    string _hashpassword = BLL.Password.ComputeHash(_password, "SHA512", null);
                    _objMembersInsert.Password = _hashpassword;
                    _objMembersInsert.IsActivated = true;

                    _objMembersInsert.DateActivated = DateTime.MinValue;
                    _objMembersInsert.LastPasswordChangedDate = DateTime.MinValue;
                    _objMembersInsert.UpdatedBy = first_name;
                    Int64 MemberId = 0;
                    string ImageUrl = "NA";
                    string receipturl = "NA";


                    long memberId = 0;
                    string imageUrl = "NA";
                    string receiptUrl = "NA";

                    long insertStatus = _Members.FEInsertMembers(_objMembersInsert, ref memberId, ref imageUrl, ref receiptUrl);

                    if (insertStatus == 1)
                    {
                        // ✅ Send email
                        int status1 = 0;
                        var template = _MailTemplates.GetMailTemplateById("Thank you for Registration", 0, ref status1);

                        var body = new StringBuilder(template.Description);
                        body.Replace("[UserName]", name);
                        body.Replace("[Email]", email);
                        body.Replace("[MemberId]", memberId.ToString());
                        body.Replace("[Password]", _password);
                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);

                        _sentmail.SendMailSendinbrevo(email, template.Subject, body.ToString());

                        // ✅ Sign in user
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim("MemberId", memberId.ToString())
                };

                        var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                        await HttpContext.SignInAsync(
                            "MyCookieAuth",
                            new ClaimsPrincipal(identity)
                        );

                        string url = objAppInfo.BaseUrl;

                        if (!string.IsNullOrEmpty(Request.Headers["Referer"]))
                        {
                            url = Request.Headers["Referer"].ToString();
                        }

                        return Redirect(url);
                    }
                    else
                    {
                        TempData["message"] = "Failed processing your request. Please try later.";
                    }
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.msg = msg;
            return View();
        }

        #endregion Google Login

        public IActionResult MemberList(string cname = "")
        {
            Entities.InnerPages objLifeMembers = new Entities.InnerPages(); int status = 0;

            // ✅ Read cookie safely
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }

            // ✅ Get data
            objLifeMembers = _InnerPages
                .GetInnerPagesListById(0, "TCA Life Members 2018", 0, 0, ref status);

            int _qstatus = 0;
            objMembers = _Members.AddMembershipRequirement(ref _qstatus);

            // ✅ Pass to view
            ViewBag.objLifeMembers = objLifeMembers;
            ViewBag.objMembers = objMembers;
            ViewBag.cname = cname;

            return View();
        }
        [HttpGet]
        public ActionResult SubscriptionDetails(string cname)
        {
            int _qstatus = 0;
            string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            var objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);
            ViewBag.objMember = objMember;
            ViewBag.cname = cname;

            if (!string.IsNullOrEmpty(objMember.SubscriptionId))
            {
                int paystatus = 0;
                Entities.PaymentSettings objsettings =
                    _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

                string accessToken = objsettings.SuccessUrl;
                string squareEnvironment = objsettings.AccountType == "Test" ? "sandbox" : "production";
                string baseUrl = squareEnvironment == "production"
                                                ? "https://connect.squareup.com"
                                                : "https://connect.squareupsandbox.com";

                try
                {
                    using (var http = new System.Net.Http.HttpClient())
                    {
                        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                        http.DefaultRequestHeaders.Add("Square-Version", "2023-10-18");
                        http.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // ── STEP 1: Get Subscription ─────────────────────────────
                        var subResponse = http.GetAsync(
                            $"{baseUrl}/v2/subscriptions/{objMember.SubscriptionId}").Result;
                        string subBody = subResponse.Content.ReadAsStringAsync().Result;

                        if (subResponse.IsSuccessStatusCode)
                        {
                            dynamic subResult = Newtonsoft.Json.JsonConvert.DeserializeObject(subBody);

                            string rawStatus = subResult?.subscription?.status?.ToString() ?? "";
                            string cancelledDate = subResult?.subscription?.canceled_date?.ToString() ?? "";
                            string chargedThrough = subResult?.subscription?.charged_through_date?.ToString() ?? "";
                            string startDate = subResult?.subscription?.start_date?.ToString() ?? "";
                            string createdAt = subResult?.subscription?.created_at?.ToString() ?? "";

                            // ✅ Check if start is scheduled in the future (PENDING + future start_date)
                            bool isStartScheduled = false;
                            if (rawStatus == "PENDING" && !string.IsNullOrEmpty(startDate))
                            {
                                if (DateTime.TryParse(startDate, out DateTime parsedStart))
                                    isStartScheduled = parsedStart > DateTime.UtcNow;
                            }

                            // ✅ Smart status logic
                            string displayStatus;
                            string statusBadgeClass;

                            if (rawStatus == "CANCELED")
                            {
                                displayStatus = "Cancelled";
                                statusBadgeClass = "badge-cancelled";
                            }
                            else if (!string.IsNullOrEmpty(cancelledDate))
                            {
                                // ACTIVE but cancellation is scheduled
                                displayStatus = "Cancellation Scheduled";
                                statusBadgeClass = "badge-scheduled";
                            }
                            else if (rawStatus == "ACTIVE")
                            {
                                displayStatus = "Active";
                                statusBadgeClass = "badge-active";
                            }
                            else if (rawStatus == "PENDING" && isStartScheduled)
                            {
                                // Future-dated subscription not yet started
                                displayStatus = "Start Scheduled";
                                statusBadgeClass = "badge-start-scheduled";
                            }
                            else if (rawStatus == "PENDING")
                            {
                                displayStatus = "Pending";
                                statusBadgeClass = "badge-pending";
                            }
                            else if (rawStatus == "PAUSED")
                            {
                                displayStatus = "Paused";
                                statusBadgeClass = "badge-paused";
                            }
                            else if (rawStatus == "DEACTIVATED")
                            {
                                displayStatus = "Deactivated";
                                statusBadgeClass = "badge-cancelled";
                            }
                            else
                            {
                                displayStatus = rawStatus;
                                statusBadgeClass = "badge-pending";
                            }

                            // ✅ Pass all values to ViewBag
                            ViewBag.SubStatus = displayStatus;
                            ViewBag.SubStatusClass = statusBadgeClass;
                            ViewBag.RawStatus = rawStatus;
                            ViewBag.StartDate = startDate;
                            ViewBag.CreatedAt = createdAt;
                            ViewBag.ChargeAt = chargedThrough;
                            ViewBag.CancelledDate = cancelledDate;
                            ViewBag.StartScheduled = isStartScheduled;
                            ViewBag.NextBillingNote = !string.IsNullOrEmpty(cancelledDate)
                                                       ? $"Ends on {cancelledDate}"
                                                       : chargedThrough;

                            string customerId = subResult?.subscription?.customer_id?.ToString() ?? "";
                            string cardId = subResult?.subscription?.card_id?.ToString() ?? "";

                            // ── STEP 2: Get Recurring Type from plan ─────────────
                            string planVariationId = subResult?.subscription?.plan_variation_id?.ToString() ?? "";
                            string recurringType = "";

                            if (!string.IsNullOrEmpty(planVariationId))
                            {
                                var planResp = http.GetAsync(
                                    $"{baseUrl}/v2/catalog/object/{planVariationId}").Result;

                                if (planResp.IsSuccessStatusCode)
                                {
                                    string planBody = planResp.Content.ReadAsStringAsync().Result;
                                    dynamic planResult = Newtonsoft.Json.JsonConvert.DeserializeObject(planBody);

                                    try
                                    {
                                        string cadence = "";
                                        var phases = planResult?.catalog_object?.subscription_plan_data?.phases;
                                        if (phases != null && phases.Count > 0)
                                            cadence = phases[0]?.cadence?.ToString() ?? "";

                                        recurringType = cadence switch
                                        {
                                            "WEEKLY" => "Weekly",
                                            "MONTHLY" => "Monthly",
                                            "NINETY_DAYS" => "Quarterly",
                                            "SIXTY_DAYS" => "Half Yearly",
                                            "ANNUAL" => "Annual",
                                            _ => cadence
                                        };
                                    }
                                    catch { }
                                }
                            }

                            // Fallback to DB value if plan fetch fails
                            if (string.IsNullOrEmpty(recurringType))
                                recurringType = objMember.RecurringType ?? "";

                            ViewBag.RecurringType = recurringType;

                            // ── STEP 3: Get Card Details ─────────────────────────
                            if (!string.IsNullOrEmpty(cardId))
                            {
                                var cardResponse = http.GetAsync($"{baseUrl}/v2/cards/{cardId}").Result;
                                string cardBody = cardResponse.Content.ReadAsStringAsync().Result;

                                if (cardResponse.IsSuccessStatusCode)
                                {
                                    dynamic cardResult = Newtonsoft.Json.JsonConvert.DeserializeObject(cardBody);
                                    ViewBag.CardBrand = cardResult?.card?.card_brand?.ToString() ?? "";
                                    ViewBag.CardLast4 = cardResult?.card?.last_4?.ToString() ?? "";
                                    ViewBag.CardExpMonth = cardResult?.card?.exp_month?.ToString() ?? "";
                                    ViewBag.CardExpYear = cardResult?.card?.exp_year?.ToString() ?? "";
                                }
                            }

                            // ── STEP 4: Get Invoices ─────────────────────────────
                            var invoicesList = new List<dynamic>();
                            var invPayload = new
                            {
                                query = new
                                {
                                    filter = new
                                    {
                                        subscription_ids = new[] { objMember.SubscriptionId }
                                    }
                                },
                                limit = 20
                            };

                            string invJson = Newtonsoft.Json.JsonConvert.SerializeObject(invPayload);
                            var invContent = new System.Net.Http.StringContent(
                                                       invJson, System.Text.Encoding.UTF8, "application/json");
                            var invResponse = http.PostAsync($"{baseUrl}/v2/invoices/search", invContent).Result;
                            string invBody = invResponse.Content.ReadAsStringAsync().Result;

                            if (invResponse.IsSuccessStatusCode)
                            {
                                dynamic invResult = Newtonsoft.Json.JsonConvert.DeserializeObject(invBody);
                                if (invResult?.invoices != null)
                                {
                                    foreach (var inv in invResult.invoices)
                                    {
                                        decimal amount = 0;
                                        if (inv.payment_requests != null && inv.payment_requests.Count > 0)
                                        {
                                            string amtStr = inv.payment_requests[0]
                                                               .computed_amount_money?.amount?.ToString() ?? "0";
                                            decimal.TryParse(amtStr, out decimal amtCents);
                                            amount = amtCents / 100m;
                                        }

                                        string paidAt = "";
                                        if (inv.payment_requests != null && inv.payment_requests.Count > 0)
                                            paidAt = inv.payment_requests[0]
                                                       .card_details?.paid_at?.ToString() ?? "";

                                        invoicesList.Add(new
                                        {
                                            InvoiceId = inv.id?.ToString() ?? "",
                                            Status = inv.status?.ToString() ?? "",
                                            Amount = amount,
                                            IssuedAt = inv.created_at?.ToString() ?? "",
                                            PaidAt = paidAt,
                                            InvoiceLink = inv.public_url?.ToString() ?? ""
                                        });
                                    }
                                }
                            }
                            ViewBag.Invoices = invoicesList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.SubError = ex.Message;
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult GetCaptcha()
        {
            try
            {
                // ✅ Correct way to read session in ASP.NET Core
                string str = HttpContext.Session.GetString("captchastring");

                if (!string.IsNullOrEmpty(str))
                {
                    return Ok(new { ok = true, data = str, message = "" });
                }
                else
                {
                    return Ok(new { ok = false, message = "Captcha not found" });
                }
            }
            catch
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "<div class=\"error closable\">Failed Transaction</div>"
                });
            }
        }
        public ActionResult FEMembersList(Int64 MembershipTypeId = 0, string Search = "", string SortColumn = "MemberId", string SortOrder = "ASC", int PageNo = 1, int Items = 40)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {
                lstMembers = _Members.FEGetMembersListByVariable(MembershipTypeId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstMembers = lstMembers;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();

        }

        public ActionResult VolunteerIndex(Int64 MemberId = 0, string cname = "")
        {
            ViewBag.MemberId = MemberId;
            ViewBag.cname = cname;
            return View();
        }


        public ActionResult FEVolunteersMemberIdList(Int64 MemberId)
        {
            int status = 0;
            List<Entities.Volunteers> lstVolunteers = new List<Entities.Volunteers>();

            try
            {
                lstVolunteers = _Volunteers.FEGetVolunteersMemberIdList(MemberId, ref status);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.lstVolunteers = lstVolunteers;
            ViewBag.MemberId = MemberId;
            return View();
        }


        public ActionResult DonarIndex(Int64 MemberId = 0, string cname = "")
        {
            ViewBag.MemberId = MemberId;
            ViewBag.cname = cname;
            return View();
        }

        public ActionResult FEDonarsList(Int64 MemberId = 0, string cname = "")
        {
            int status = 0;
            BLL.Donors _Donors = new BLL.Donors();
            List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();
            try
            {
                lstServiceDonations = _Donors.FEDonorsMemberIdGetList(MemberId, "", "UpdatedDate DESC", 1, 1000, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.MemberId = MemberId;
            ViewBag.lstDonors = lstServiceDonations;
            ViewBag.cname = cname;
            //ViewBag.ClinicId = cid;
            return View();
        }


        public ActionResult EventIndex(Int64 MemberId = 0, string cname = "")
        {
            ViewBag.MemberId = MemberId;
            ViewBag.cname = cname;
            return View();
        }

        public ActionResult FEEventList(Int64 MemberId = 0)
        {
            int status = 0;
            BLL.Donors _Donors = new BLL.Donors();
            List<Entities.EventUserInfo> lstEventUserInfo = new List<Entities.EventUserInfo>();
            try
            {
                lstEventUserInfo = _Events.FEGetEventMemberInfoListByVariable(MemberId, "", "UpdatedTime DESC", 1, 1000, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.MemberId = MemberId;
            ViewBag.lstEventUserInfo = lstEventUserInfo;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserAuth(LogOnModel model, string returnUrl = "", long chapterId = 0, string cname = "")
        {
            int status1 = 0;
            var objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            try
            {
                ViewBag.returnurl = returnUrl;

                int _status = 0;
                var objMembers = _Members.GetMemberFullDetailsByEmail(model.Email, ref _status);

                if (objMembers != null && objMembers.MemberId != 0)
                {
                    if (objMembers.IsApproved)
                    {
                        int _qstatus = 0;
                        string password = _Members.GetPassword(objMembers.MemberId, ref _qstatus);

                        if (_qstatus == 1 && model.Password.Trim() == password)
                        {
                            string fullName = objMembers.FirstName + " " + objMembers.LastName;

                            // ✅ Claims (Core replacement for FormsAuth)
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objMembers.Email),
                        new Claim("FullName", fullName),
                        new Claim("MemberId", objMembers.MemberId.ToString()),
                        new Claim("Role", "FEUser")
                    };

                            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                            };

                            // ✅ Sign in user
                            await HttpContext.SignInAsync(
                                "MyCookieAuth",
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties
                            );

                            // ✅ Optional session
                            HttpContext.Session.SetString("username", fullName);
                            HttpContext.Session.SetString("MemberID", objMembers.MemberId.ToString());

                            // ✅ Redirect logic
                            if (!string.IsNullOrEmpty(returnUrl) &&
                                returnUrl != objAppInfo.BaseUrl + cname + "/member-registration" &&
                                returnUrl != objAppInfo.BaseUrl + cname + "/index" &&
                                returnUrl != objAppInfo.BaseUrl + cname + "/member-login")
                            {
                                return Redirect(returnUrl);
                            }

                            return RedirectToAction("Profile", "Members", new { cname = cname });
                        }
                        else
                        {
                            TempData["message"] = "<div class=\"error1 closable\">Email or password is incorrect.</div>";
                        }
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Your status has been deactivated. Please contact admin.</div>";
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Email or password is incorrect.</div>";
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
            }

            return RedirectToAction("LogOn", "Members", new { cname = cname });
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

        #region BalalaSambaraluList

        public ActionResult BalalaSambaraluIndex(string cname = "")
        {
            string Email = HttpContext.User.Identity.Name.ToString();
            ViewBag.cname = cname;
            ViewBag.Email = Email;
            return View();
        }

        public ActionResult FEBalalaSambaraluList(string Email = "")
        {
            int status = 0;
            BLL.Donors _Donors = new BLL.Donors();
            List<Entities.KidsRegistrations> lstSambaralu = new List<Entities.KidsRegistrations>();
            try
            {
                lstSambaralu = _BalalaSambaralu.FEGetKidsRegistrationsList(Email, "", "UpdatedTime DESC", 1, 1000, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.Email = Email;
            ViewBag.lstSambaralu = lstSambaralu;
            return View();
        }

        #endregion

        //public static bool ReCaptchaPassed(string gRecaptchaResponse)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=" + @System.Configuration.ConfigurationManager.AppSettings["CapchaSecreateKey"] + "&response=" + gRecaptchaResponse).Result;
        //    if (res.StatusCode != HttpStatusCode.OK)
        //        return false;

        //    string JSONres = res.Content.ReadAsStringAsync().Result;
        //    dynamic JSONdata = JObject.Parse(JSONres);
        //    if (JSONdata.success != "true")
        //        return false;

        //    return true;
        //}

        //public async Task ScheduleEmailAsync(string recipient, string subject, string body, DateTime scheduledTime)
        //{
        //    string apiUrl = "https://api.brevo.com/v3/smtp/email"; // Replace with the actual Brevo API endpoint
        //    string apiKey = "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";
        //    // Construct the request payload
        //    var payload = new
        //    {
        //        Recipient = recipient,
        //        Subject = subject,
        //        Body = body,
        //        ScheduledTime = scheduledTime.ToString("yyyy-MM-ddTHH:mm:ss")
        //    };

        //    // Convert the payload to JSON
        //    string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

        //    // Set up headers
        //    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        //    httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");


        //    // Make the HTTP POST request
        //    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        //    // Handle the API response
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Email scheduled successfully
        //        Console.WriteLine("Email scheduled successfully!");
        //    }
        //    else
        //    {
        //        // Handle error
        //        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        //    }
        //}



        public ActionResult ProfileMenu(string tab = "", string cname = "")
        {
            Entities.Members _objMember = new Entities.Members();
            try
            {
                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;


                int _qstatus = 0;
                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);

                Int64 MemberId = _objMember.MemberId;

                if (_objMember.objMembershipOrder.ExpiryDate != null)
                {
                    DateTime expiryDate = DateTime.Parse(_objMember.objMembershipOrder.ExpiryDate.ToString());

                    // Get the current date
                    DateTime currentDate = DateTime.Now;

                    // Calculate the number of months remaining
                    int monthsRemaining = (expiryDate.Year - currentDate.Year) * 12 + (expiryDate.Month - currentDate.Month);
                    TimeSpan timeRemaining = expiryDate - currentDate;
                    // Calculate the number of days remaining
                    int daysRemaining = (int)timeRemaining.TotalDays;

                    ViewBag.monthsRemaining = monthsRemaining;
                    ViewBag.daysRemaining = daysRemaining;
                }


                if (_qstatus == 1)
                {
                    ViewBag.objMemberDetails = _objMember;
                    ViewBag.status = _qstatus;
                    //  ViewBag.MembershipType = MembershipType;
                    ViewBag.objMembersType = objMembers;
                }


                //if (_objMember.MemberId == 0)
                //{
                //    TempData["message"] = "<div class=\"error1 closable\">Failed transaction.</div>";
                //    return RedirectToAction("LogOff", "Members");
                //}
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objMembers = _objMember;

            ViewBag.tab = tab;
            ViewBag.cname = cname;

            return View();
        }


        public ActionResult ViewServiceDonation(Int64 ServiceDonationId = 0)
        {
            Entities.Members _objMember = new Entities.Members();
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            int status = 0;
            int _qstatus = 0;
            try
            {
                objServiceDonations = _Donor.GetServiceDonationsById(ServiceDonationId, ref status);
                _objMember = _Members.GetMemberFullDetailsByEmail(HttpContext.User.Identity.Name.ToString(), ref _qstatus);

                if (objServiceDonations == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Profile", "Members");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Profile", "Members");
            }

            ViewBag.objServiceDonations = objServiceDonations;
            ViewBag.objMember = _objMember;

            return View();
        }


        public ActionResult MemberInfo(string cname = "")
        {
            List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();
            Entities.Members _objMember = new Entities.Members();
            if (Request.Cookies.TryGetValue("ChapterName", out string cookieValue))
            {
                cname = cookieValue;
            }
            try
            {
                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
                int qstatus = 0;
                Int64 ChapterId = 0;

                int _qstatus = 0;
                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);
                // lstServiceDonations = _ServiceDonations.GetServiceDonationsLists(this.User.Identity.Name.ToString(), ref _qstatus);

                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(_objMember.ChapterName, ref qstatus);
                    //cname = _objMember.ChapterName;
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                    cname = "global";
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
            ViewBag.objMember = _objMember;
            ViewBag.lstServiceDonations = lstServiceDonations;
            return View();
        }
        private bool CancelSquareSubscription(string subscriptionId)
        {
            try
            {
                int paystatus = 0;
                Entities.PaymentSettings objsettings = _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

                string accessToken = objsettings.SuccessUrl;
                string squareEnvironment = objsettings.AccountType == "Test" ? "sandbox" : "production";
                string baseUrl = squareEnvironment == "production"
                                 ? "https://connect.squareup.com"
                                 : "https://connect.squareupsandbox.com";

                using (var http = new System.Net.Http.HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    http.DefaultRequestHeaders.Add("Square-Version", "2021-06-16");

                    // ✅ Square cancel subscription API
                    var response = http.PostAsync(
                        $"{baseUrl}/v2/subscriptions/{subscriptionId}/cancel",
                        new System.Net.Http.StringContent("", System.Text.Encoding.UTF8, "application/json")
                    ).Result;

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<IActionResult> CancelSubscription(string cname = "")
        {
            Entities.Members _objMember = new Entities.Members();
            DAL.Members _MembersDAL = new DAL.Members();
            int qstatus = 0;

            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
                cname = cookieValue;

            if (!string.IsNullOrEmpty(cname))
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);

            try
            {
                int _qstatus = 0;
                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);

                // ✅ STEP 1: Cancel Square subscription first
                string squareSubscriptionId = _objMember?.SubscriptionId ?? "";

                if (!string.IsNullOrEmpty(squareSubscriptionId))
                {
                    bool squareCancelled = CancelSquareSubscription(squareSubscriptionId);
                    if (!squareCancelled)
                    {
                        TempData["ErrorMessage"] = "Failed to cancel Square subscription. Please try again.";
                        ViewBag.cname = cname;
                        ViewBag.objMember = _objMember;
                        return View();
                    }
                }

                // ✅ STEP 2: Cancel in your DB
                int cancelStatus = (int)_MembersDAL.CancelMemberSubscription(_objMember.MemberId, ref _qstatus);

                if (cancelStatus == 1)
                {
                    Response.Cookies.Delete("chaperInfo");
                    TempData["SuccessMessage"] = "Your subscription has been cancelled successfully. We hope to see you again!";
                }
                else if (cancelStatus == 2)
                {
                    TempData["ErrorMessage"] = "No active subscription found.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong. Please try again.";
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
            ViewBag.objMember = _objMember;
            return View();
        }


        [HttpGet]
        public ActionResult DonorsList(string cname = "", Int64 ChapterId = 0)
        {
            List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();
            Entities.Members _objMember = new Entities.Members();

            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            int qstatus = 0;


            // ✅ Read cookie safely
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }
            int status1 = 0;
            lstChapters = _Chapters.GetChaptersList(ref status1);

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
                cname = "global";
            }

            try
            {
                //int qstatus = 0;
                //Int64 ChapterId = 0;

                int _qstatus = 0;

                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);
                lstServiceDonations = _ServiceDonations.GetServiceDonationsLists(userName, ChapterId, ref _qstatus);

                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(_objMember.ChapterName, ref qstatus);
                    //cname = _objMember.ChapterName;
                }


            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
            ViewBag.objMember = _objMember;
            ViewBag.lstServiceDonations = lstServiceDonations;
            return View();
        }
        [HttpGet]
        public IActionResult FEMemEventsList(string cname = "", long chapterId = 0)
        {
            List<Entities.EventUserInfo> lstEventUserInfo = new List<Entities.EventUserInfo>();
            Entities.Members _objMember = new Entities.Members();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            int qstatus = 0;

            // ✅ Read cookie safely
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }

            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            int status1 = 0;
            lstChapters = _Chapters.GetChaptersList(ref status1);

            if (objChapters != null)
            {
                chapterId = objChapters.ChapterId;
            }

            if (chapterId == 0)
            {
                chapterId = 1;
                cname = "global";
            }

            try
            {
                int _qstatus = 0;

                // ✅ Safe user identity access
                //string email = User?.Identity?.Name;
                string email = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;

                if (!string.IsNullOrEmpty(email))
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(email, ref _qstatus);

                    lstEventUserInfo = _Events
                        .FEEventsRegistrationGetListByEmail(HttpContext.User.Identity.Name.ToString(), chapterId, ref _qstatus);
                }

                if (!string.IsNullOrEmpty(cname) && _objMember != null)
                {
                    objChapters = _Chapters
                        .GetChaptersListByName(_objMember.ChapterName, ref qstatus);
                }

                if (objChapters != null)
                {
                    chapterId = objChapters.ChapterId;
                }

                if (chapterId == 0)
                {
                    chapterId = 1;
                    cname = "global";
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.cname = cname;
            ViewBag.objMember = _objMember;
            ViewBag.lstEventUserInfo = lstEventUserInfo;

            return View();
        }
        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [Authorize]

        public ActionResult ViewEventUser(Int64 EventUserInfoId = 0)
        {
            Entities.Members _objMember = new Entities.Members();
            Int64 EventId = 0;
            try
            {

                int _qstatus = 0;
                int status = 0;
                List<Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<Entities.EventRegistrationCounts>();
                List<Entities.EventsTickets> lstEventsTickets = new List<Entities.EventsTickets>();
                Entities.Events objEvents = new Entities.Events();
                List<Entities.EventRegTickets> lstEventRegTickets = new List<Entities.EventRegTickets>();
                string userName = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
                objEventUserInfo = _Events.GetEventUserInfoFullDetailsById(EventUserInfoId, EventId, ref objEvents, ref lstEventRegistrationCounts, ref lstEventsTickets, ref lstEventRegTickets, ref _qstatus);
                lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref status);
                _objMember = _Members.GetMemberFullDetailsByEmail(userName, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.lstEventRegistrationCounts = lstEventRegistrationCounts;
                    ViewBag.objEventUserInfo = objEventUserInfo;
                    ViewBag.lstEventCompetitions = lstEventCompetitions;
                    ViewBag.objMember = _objMember;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                    return RedirectToAction("Profile", "Members");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Profile", "Members");
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> InsertPaymentAsync([FromBody] dynamic paymentData)
        {
            try
            {
                // Log the incoming data
                System.Diagnostics.Debug.WriteLine("InsertPaymentAsync called");
                System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(paymentData));

                // Parse the data
                string firstName = paymentData.FirstName;
                string lastName = paymentData.LastName;
                string email = paymentData.Email;
                string mobilePhone = paymentData.MobilePhone;
                string password = paymentData.Password;
                string city = paymentData.City;
                string state = paymentData.State;
                string address = paymentData.Address;
                string address2 = paymentData.Address2;
                string zipCode = paymentData.ZipCode;
                string maritalStatus = paymentData.MaritalStatus;
                string gender = paymentData.gender;
                int chapterId = paymentData.ChapterId;
                int membershipTypeId = paymentData.MembershipTypeId;
                string membershipType = paymentData.membershipType;
                decimal amountPaid = paymentData.AmountPaid;
                string recaptchaToken = paymentData.recaptchaToken;

                //// Create payment record
                //var payment = new Payment
                //{
                //    Amount = amountPaid,
                //    PaymentDate = DateTime.Now,
                //    PaymentStatus = "Pending", // Initially pending until PayPal confirms
                //    MembershipType = membershipType,
                //    Email = email,
                //    FirstName = firstName,
                //    LastName = lastName,
                //    Phone = mobilePhone,
                //    // Add other fields as needed
                //};

                //// Save to database
                //db.Payments.Add(payment);
                //await db.SaveChangesAsync();

                // Return success with payment ID
                return Json(new
                {
                    success = true,
                    //paymentId = payment.PaymentId, // Make sure this property name matches
                    message = "Payment record created successfully"
                });
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error in InsertPaymentAsync: " + ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

                return Json(new
                {
                    success = false,
                    data = ex.Message,
                    message = "Error creating payment record"
                });
            }
        }
        [HttpPost]
        public IActionResult PayPalSaveLog(string message)
        {

            try
            {
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                try
                {
                    // ✅ Fallback log

                }
                catch
                {
                    // If even fallback fails, silently ignore (avoid crash)
                }

                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        public static class PayflowUtility
        {
            public static string RequestId
            {
                get
                {
                    // Generate unique request ID
                    return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                }
            }
        }


        // For .NET Core, use this helper method instead of the old GetPayPalCollection
        public static Dictionary<string, string> GetPayPalDictionary(string payPalInfo)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(payPalInfo))
                return dict;

            string[] arrayResponses = payPalInfo.Split('&');

            for (int i = 0; i < arrayResponses.Length; i++)
            {
                string[] temp = arrayResponses[i].Split('=');
                if (temp.Length == 2)
                {
                    dict[temp[0]] = Uri.UnescapeDataString(temp[1]);
                }
            }
            return dict;
        }

        // If you must use NameValueCollection (for backward compatibility)
        public static NameValueCollection GetPayPalCollection(string payPalInfo)
        {
            //place the responses into collection
            NameValueCollection PayPalCollection = new System.Collections.Specialized.NameValueCollection();
            string[] ArrayReponses = payPalInfo.Split('&');

            for (int i = 0; i < ArrayReponses.Length; i++)
            {
                string[] Temp = ArrayReponses[i].Split('=');
                if (Temp.Length == 2)
                {
                    PayPalCollection.Add(Temp[0], Temp[1]);
                }
            }
            return PayPalCollection;
        }
        public class PayflowNETAPI
        {
            private const string PAYFLOW_URL = "https://payflowpro.paypal.com/";

            public string TransactionRequest { get; set; }
            public string TransactionResponse { get; set; }

            public string SubmitTransaction(string paramList, string requestId)
            {
                try
                {
                    TransactionRequest = paramList;

                    // Force TLS 1.2
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.Expect100Continue = false;

                    // Create web request (same as MVC)
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PAYFLOW_URL);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.KeepAlive = false;
                    request.Timeout = 45000; // 45 seconds

                    // Add required headers
                    request.Headers.Add("X-VPS-REQUEST-ID", requestId);
                    request.Headers.Add("X-VPS-CLIENT-TIMEOUT", "45");
                    request.Headers.Add("X-VPS-VIT-OS-NAME", "Windows NT");
                    request.Headers.Add("X-VPS-VIT-OS-VERSION", "10.0");
                    request.Headers.Add("X-VPS-VIT-SDK-NAME", "PayflowPro");
                    request.Headers.Add("X-VPS-VIT-SDK-VERSION", "4.3.1.0");
                    request.Headers.Add("X-VPS-VIT-RUNTIME-VER", "8.0");
                    request.Headers.Add("X-VPS-VIT-INTEGRATION-PRODUCT", "PayflowPro");
                    request.Headers.Add("X-VPS-VIT-INTEGRATION-VERSION", "4.3.1.0");

                    // Convert paramList to bytes using ASCII encoding (PayPal's expected format)
                    byte[] byteArray = Encoding.ASCII.GetBytes(paramList);
                    request.ContentLength = byteArray.Length;

                    // Write the data to the request stream
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }

                    // Get the response
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            TransactionResponse = reader.ReadToEnd();
                        }
                    }

                    return TransactionResponse;
                }
                catch (WebException ex)
                {
                    // Read error response from PayPal
                    if (ex.Response != null)
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            TransactionResponse = reader.ReadToEnd();
                        }
                        return TransactionResponse;
                    }
                    throw new Exception($"Payment transaction failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Payment transaction failed: {ex.Message}");
                }
            }
        }
        // Validate and clean your paramList before sending
        private string ValidateAndCleanParamList(string paramList)
        {
            if (string.IsNullOrWhiteSpace(paramList))
                throw new Exception("Parameter list is empty");

            // Remove any whitespace, line breaks, or carriage returns
            paramList = paramList.Replace("\r", "").Replace("\n", "").Replace(" ", "");

            // Log for debugging (mask sensitive data)
            var pairs = paramList.Split('&');
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    if (parts[0] == "PWD" || parts[0] == "ACCT" || parts[0] == "CVV2")
                        Console.WriteLine($"{parts[0]}=******");
                    else
                        Console.WriteLine(pair);
                }
            }

            return paramList;
        }
        public class HttpClientFactory
        {
            public HttpClient CreateClient()
            {
                return new HttpClient();
            }
        }

    }
}


