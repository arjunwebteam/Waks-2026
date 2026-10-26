using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.Entities;
using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nancy;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Members,ChapterAdmin,SiteAdmin,Administrator,Treasurer,DeveloperAdmin,")]
    public class MembersController : Controller
    {
        BLL.Members _Members = new BLL.Members();
        DAL.Members _DMembers = new DAL.Members();
        List<Entities.Members> lstMembers = new List<Entities.Members>();
        Entities.Members objMembers = new Entities.Members();
        BLL.MembershipTypes _MembershipType = new BLL.MembershipTypes();
        List<Entities.MembershipTypes> lstMembershipType = new List<Entities.MembershipTypes>();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.ServiceDonations _ServiceDonations = new BLL.ServiceDonations();
        BLL.Users _Users = new BLL.Users();
        BLL.SendMail _sendmail = new BLL.SendMail();
        BLL.PaymentSettings _paymentSettings = new BLL.PaymentSettings();


        #region Members
        [Authorize]
        public IActionResult Index(long MembershipTypeId = 0, long ChapterId = 0, long mid = 0, string type = "allactive")
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

                int qStatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref qStatus);
                lstChapters = _Chapters.GetChaptersList(ref status);

                objuserroles = _Users.GetRoleDetialsById(userId, mid, ref status);

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

            ViewBag.ChapterId = chapterId;


            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            ViewBag.objMembers = objMembers;
            ViewBag.MembershipTypeId = MembershipTypeId;

            ViewBag.type = type;

            return View();
        }

        public ActionResult MembersList(string Search = "", Int64 MembershipTypeId = 0, Int64 PaymentStatusId = 0, string StartDate = "", string EndDate = "", string ExpiryDate = "", string IsVolunteer = "", Int64 ChapterId = 0, string SortColumn = "InsertedTime", string SortOrder = "DESC", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0, string type = "")
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {
                lstMembers = _Members.GetMembersListByVariable(Search, MembershipTypeId, PaymentStatusId, StartDate, EndDate, ExpiryDate, IsVolunteer, ChapterId, type, Sort, PageNo, Items, ref Total);

                //if(lstMembers != null && lstMembers.Count != 0)
                //{
                //    foreach(var item in lstMembers)
                //    {
                //        if((item.MobilePhone != null && item.MobilePhone != "") || (item.HomePhone != null && item.HomePhone != ""))
                //        {
                //            string Password = "";
                //            if (item.MobilePhone != null && item.MobilePhone != "")
                //            {
                //                 Password = item.MobilePhone.Replace("-", "");
                //            }
                //            else if(item.HomePhone != null && item.HomePhone != "")
                //            {
                //                Password = item.HomePhone.Replace("-", "");
                //            }

                //            item.Password = BLL.Password.ComputeHash(Password, "SHA512", null);

                //            Int64 _pstatus = _Members.ChangePassword(item.MemberId, item.Password, "");
                //        }
                //    }
                //}
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstMembers = lstMembers;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            ViewBag.type = type;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        public ActionResult AddMember(Int64 mid = 0, string type = "", Int64 ChapterId = 0)
        {
            try
            {
                int _qstatus = 0;
                int Total = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();

                lstChapters = _Chapters.GetChaptersList(ref Total);

                if (_qstatus == 1)
                {
                    ViewBag.lstChapters = lstChapters;
                    ViewBag.objMembers = objMembers;
                    ViewBag.status = _qstatus;
                    ViewBag.mid = mid;
                    ViewBag.type = type;
                    ViewBag.ChapterId = ChapterId;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Transaction";
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Members", new { mid = mid });
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Members", new { mid = mid });
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddMember(Entities.Members objMembers, List<Entities.ChildrenInfo> lstChildrenInfo, IFormFile ProfileImage, long mid = 0, string type = "")
        {
            try
            {
                Entities.AppInfo objAppInfo = new Entities.AppInfo();
                int status1 = 0;
                objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                string imageUrl = "NA";

                // Handle profile image upload
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadFolder = Path.Combine(objAppInfo.UploadPath, "Members");
                    Directory.CreateDirectory(uploadFolder); // Ensure folder exists
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(stream);
                    }

                    // Optional: Resize / crop with System.Drawing.Common
                    using (var bitmap = new Bitmap(filePath))
                    {
                        var resized = new Bitmap(bitmap, new Size(94, 93));
                        resized.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    imageUrl = fileName;
                }
                if (objMembers.PaymentStatusId == 1)
                {
                    objMembers.IsApproved = true;

                }
                else
                {
                    objMembers.IsApproved = false;

                }
                // Membership setup
                Guid guid = AWS_AssociationToolCore.BLL.Common.generateGUID();

                objMembers.UpdatedTime = DateTime.UtcNow;
                objMembers.IsLockedOut = false;
                objMembers.Password = Password.ComputeHash(BLL.Common.GetRandomString(8), "SHA512", null);
                objMembers.IsActivated = false;
                objMembers.RegistrationGUID = guid;
                objMembers.DateActivated = DateTime.MinValue;
                objMembers.LastPasswordChangedDate = DateTime.MinValue;
                objMembers.UserName = objMembers.FirstName + " " + objMembers.LastName;
                objMembers.objMembershipOrder.UpdatedBy = User.Identity.Name;
                objMembers.objMembershipOrder.ExpiryDate = objMembers.ExpiryDate;

                if (objMembers.SameAdress == "Yes")
                {
                    objMembers.BillingAdress = objMembers.Address;
                    objMembers.BillingCity = objMembers.City;
                    objMembers.BillingState = objMembers.State;
                    objMembers.BillingZipCode = objMembers.ZipCode;
                }

                long memberId = objMembers.MemberId;
                string receiptUrl = "NA";

                // Insert member
                long insertStatus = _Members.InsertMembers(objMembers, ref memberId, ref imageUrl, ref receiptUrl);

                // Insert children info if any
                if (insertStatus == 1 && lstChildrenInfo != null && lstChildrenInfo.Count > 0)
                {
                    foreach (var child in lstChildrenInfo)
                    {
                        child.MemberId = memberId;
                        if (!string.IsNullOrWhiteSpace(child.FirstName))
                        {
                            long childStatus = _Members.InsertChildrenInfo(child);
                            if (childStatus == -1)
                            {
                                TempData["messageType"] = "error";
                                TempData["message"] = "Failed inserting member's children details.";
                                return RedirectToAction("Index", "Members", new { mid, type });
                            }
                        }
                    }
                }

                // Generate random password
                string plainPassword = BLL.Password.GetUniqueKey(8);
                string passwordHash = BLL.Password.ComputeHash(plainPassword, "SHA512", null);
                string forceChange = "Yes";

                _Members.ChangePassword(memberId, passwordHash, forceChange);
                objMembers = _Members.GetMemberFullDetailsById(memberId, ref status1);

                // Send email
                var template = _MailTemplates.GetMailTemplateById("Thank you for Becoming a Member", 0, ref status1);
                if (template != null)
                {
                    // ✅ Initialize StringBuilder WITH template content
                    StringBuilder body = new StringBuilder(template.Description);

                    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMembers.FirstName + " " + objMembers.LastName));
                    body.Replace("[MEMBERID]", memberId.ToString());
                    body.Replace("[FirstName]", objMembers.FirstName);
                    body.Replace("[LastName]", objMembers.LastName);
                    body.Replace("[Password]", plainPassword);
                    body.Replace("[SiteName]", objAppInfo.SiteName);
                    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                    body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                    body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                    body.Replace("[PaymentType]", objMembers.PaymentMethod);
                    body.Replace("[PaymentStatus]", objMembers.PaymentStatus);

                    string subject = template.Subject;
                    subject = subject.Replace("[WebsiteName]", objAppInfo.SiteName);
                    subject = subject.Replace("[SiteName]", objAppInfo.SiteName);

                    var response = _sendmail.SendMailSendinbrevowithfrom(
                        objMembers.Email, objAppInfo.MemberEmail, subject, body.ToString());
                }

                TempData["messageType"] = "success";
                TempData["message"] = "Member added successfully and password sent via email.";
                return RedirectToAction("Index", "Members", new { mid, type });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "MembershipTypes", new { mid, type });
            }
        }

        [HttpPost]
        public IActionResult UpdateMember(Entities.Members objMembers, List<Entities.ChildrenInfo> lstChildrenInfo, IFormFile file, string Thumbimage = "", long mid = 0)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);
                // Update member billing address if same as home
                if (objMembers.SameAdress == "Yes")
                {
                    objMembers.BillingAdress = objMembers.Address;
                    objMembers.BillingCity = objMembers.City;
                    objMembers.BillingState = objMembers.State;
                    objMembers.BillingZipCode = objMembers.ZipCode;
                }
                if (objMembers.PaymentStatusId == 1)
                {
                    objMembers.IsApproved = true;

                }
                else
                {
                    objMembers.IsApproved = false;
                }
                objMembers.UpdatedBy = objMembers.FirstName;
                // Update member basic details
                long status = _Members.UpdateMembers(objMembers);

                //string imageUrl = "NA";
                //Image image = null;

                // Handle profile image upload
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

                    // Update image record in DB
                    long statusImage = _Members.UpdateUserProfileImage(objMembers.MemberId, ref imageUrl);
                    if ((status == 1 || status == 2) && image != null)
                    {
                        string normalPath = Path.Combine(objappinfo.UploadPath, "MemberProfileImages", imageUrl);
                        string thumbPath = Path.Combine(objappinfo.UploadPath, "MemberProfileImages", "thumb", imageUrl);
                        Directory.CreateDirectory(Path.GetDirectoryName(normalPath));
                        Directory.CreateDirectory(Path.GetDirectoryName(thumbPath));

                        file.CopyTo(new FileStream(normalPath, FileMode.Create));
                        image.Save(thumbPath);
                    }
                    if (status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changed your profile picture successfully.";
                    }
                    else if (status == 2)
                    {
                        TempData["messageType"] = "warning";
                        TempData["message"] = "Changed your profile picture successfully.";
                    }
                    else
                    {
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                    }


                }

                // Update children info
                if (status != -1 && lstChildrenInfo != null)
                {
                    foreach (var child in lstChildrenInfo)
                    {
                        child.MemberId = objMembers.MemberId;
                        if (!string.IsNullOrWhiteSpace(child.FirstName))
                        {
                            long childStatus = _Members.InsertChildrenInfo(child);
                            if (childStatus == -1)
                            {
                                TempData["messageType"] = "error";
                                TempData["message"] = "Failed inserting member's children details";
                                return RedirectToAction("Index", "Members", new { MemberId = objMembers.MemberId, mid });
                            }
                        }
                    }
                }

                // Final status messages
                if (status == 1 || status == 2 || status != -1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes have been updated successfully";
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction";
                }

                return RedirectToAction("Index", "Members", new { MemberId = objMembers.MemberId, mid });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Members", new { MemberId = objMembers.MemberId, mid });
            }
        }
        public ActionResult EditMember(Int64 MemberId = 0, Int64 mid = 0, string type = "", Int64 ChapterId = 0)
        {
            List<Entities.Members> lstMemberLogs = new List<Entities.Members>();
            try
            {

                int qstatus = 0;
                int Total = 0;
                objMembers = _Members.AddMembershipRequirement(ref qstatus);
                lstMemberLogs = _Members.MemberLogsGetList(MemberId, ref qstatus);
                List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();

                lstChapters = _Chapters.GetChaptersList(ref Total);

                if (qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.lstChapters = lstChapters;
                    ViewBag.status = qstatus;
                    ViewBag.ChapterId = ChapterId;
                }
                int _qstatus = 0;
                Entities.Members _objMembers = _Members.GetMemberFullDetailsById(MemberId, ref _qstatus);

                if (_qstatus == 1 && qstatus == 1)
                {
                    ViewBag.objMemberDetails = _objMembers;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Transaction";
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Members", new { mid = mid, type = type });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Members", new { mid = mid, type = type });
            }
            ViewBag.mid = mid;
            ViewBag.type = type;
            ViewBag.lstMemberLogs = lstMemberLogs;
            return View();
        }

        [HttpGet]
        public JsonResult GetSquareSubscriptionDetails(string subscriptionId)
        {
            int paystatus = 0;
            Entities.PaymentSettings objsettings = _paymentSettings.GetPaymentSettingsDetails(0, "Square", ref paystatus);

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

                    // ── STEP 1: Get Subscription ─────────────────────────────────────
                    var subResponse = http.GetAsync($"{baseUrl}/v2/subscriptions/{subscriptionId}").Result;
                    string subBody = subResponse.Content.ReadAsStringAsync().Result;

                    if (!subResponse.IsSuccessStatusCode)
                        return Json(new { Error = $"Subscription fetch failed: {subBody}" });

                    dynamic subResult = Newtonsoft.Json.JsonConvert.DeserializeObject(subBody);
                    string rawStatus = subResult?.subscription?.status?.ToString() ?? "";
                    string cancelledDate = subResult?.subscription?.canceled_date?.ToString() ?? "";
                    string chargedThrough = subResult?.subscription?.charged_through_date?.ToString() ?? "";
                    string startDate = subResult?.subscription?.start_date?.ToString() ?? "";
                    string createdAt = subResult?.subscription?.created_at?.ToString() ?? "";
                    string customerId = subResult?.subscription?.customer_id?.ToString() ?? "";
                    string cardId = subResult?.subscription?.card_id?.ToString() ?? "";

                    // ── Determine if Start is Scheduled (future date + PENDING) ──────
                    bool isStartScheduled = false;
                    if (!string.IsNullOrEmpty(startDate) && rawStatus == "PENDING")
                    {
                        if (DateTime.TryParse(startDate, out DateTime parsedStart))
                            isStartScheduled = parsedStart.Date > DateTime.Today;
                    }

                    // ── Smart Status ─────────────────────────────────────────────────
                    string displayStatus;
                    if (rawStatus == "CANCELED")
                        displayStatus = "CANCELED";
                    else if (!string.IsNullOrEmpty(cancelledDate))
                        displayStatus = "CANCELLATION SCHEDULED";
                    else if (rawStatus == "ACTIVE")
                        displayStatus = "ACTIVE";
                    else if (rawStatus == "PENDING" && isStartScheduled)
                        displayStatus = "START SCHEDULED";  // ✅ future start date
                    else if (rawStatus == "PENDING")
                        displayStatus = "PENDING";           // ✅ plain pending
                    else if (rawStatus == "PAUSED")
                        displayStatus = "PAUSED";
                    else if (rawStatus == "DEACTIVATED")
                        displayStatus = "DEACTIVATED";
                    else
                        displayStatus = rawStatus;

                    // ── Always compute Next Billing Date ─────────────────────────────
                    // ACTIVE  → charged_through_date  (next charge date)
                    // PENDING / START SCHEDULED → start_date (first charge date)
                    // Fallback: whichever is available
                    string nextBillingDate = !string.IsNullOrEmpty(chargedThrough)
                                             ? chargedThrough
                                             : startDate;

                    // ── STEP 2: Get Customer ─────────────────────────────────────────
                    string customerName = "", customerEmail = "", customerPhone = "";
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        var custResponse = http.GetAsync($"{baseUrl}/v2/customers/{customerId}").Result;
                        if (custResponse.IsSuccessStatusCode)
                        {
                            dynamic custResult = Newtonsoft.Json.JsonConvert.DeserializeObject(
                                                    custResponse.Content.ReadAsStringAsync().Result);
                            customerName = $"{custResult?.customer?.given_name ?? ""} {custResult?.customer?.family_name ?? ""}".Trim();
                            customerEmail = custResult?.customer?.email_address?.ToString() ?? "";
                            customerPhone = custResult?.customer?.phone_number?.ToString() ?? "";
                        }
                    }

                    // ── STEP 3: Get Card Details ─────────────────────────────────────
                    string cardBrand = "", cardLast4 = "", cardExpMonth = "", cardExpYear = "";
                    if (!string.IsNullOrEmpty(cardId))
                    {
                        var cardResponse = http.GetAsync($"{baseUrl}/v2/cards/{cardId}").Result;
                        if (cardResponse.IsSuccessStatusCode)
                        {
                            dynamic cardResult = Newtonsoft.Json.JsonConvert.DeserializeObject(
                                                    cardResponse.Content.ReadAsStringAsync().Result);
                            cardBrand = cardResult?.card?.card_brand?.ToString() ?? "";
                            cardLast4 = cardResult?.card?.last_4?.ToString() ?? "";
                            cardExpMonth = cardResult?.card?.exp_month?.ToString() ?? "";
                            cardExpYear = cardResult?.card?.exp_year?.ToString() ?? "";
                        }
                    }

                    // ── STEP 4: Get Invoices ─────────────────────────────────────────
                    var invoicesList = new List<object>();
                    var invSearchPayload = new
                    {
                        query = new { filter = new { subscription_ids = new[] { subscriptionId } } },
                        limit = 20
                    };
                    string invJson = Newtonsoft.Json.JsonConvert.SerializeObject(invSearchPayload);
                    var invContent = new System.Net.Http.StringContent(
                                         invJson, System.Text.Encoding.UTF8, "application/json");
                    var invResponse = http.PostAsync($"{baseUrl}/v2/invoices/search", invContent).Result;

                    if (invResponse.IsSuccessStatusCode)
                    {
                        dynamic invResult = Newtonsoft.Json.JsonConvert.DeserializeObject(
                                                invResponse.Content.ReadAsStringAsync().Result);
                        if (invResult?.invoices != null)
                        {
                            foreach (var inv in invResult.invoices)
                            {
                                decimal amount = 0;
                                if (inv.payment_requests != null && inv.payment_requests.Count > 0)
                                {
                                    decimal.TryParse(
                                        inv.payment_requests[0].computed_amount_money?.amount?.ToString() ?? "0",
                                        out decimal amtCents);
                                    amount = amtCents / 100m;
                                }

                                string paidAt = "";
                                if (inv.payment_requests != null && inv.payment_requests.Count > 0)
                                    paidAt = inv.payment_requests[0].card_details?.paid_at?.ToString() ?? "";

                                invoicesList.Add(new
                                {
                                    InvoiceId = inv.id?.ToString() ?? "",
                                    Status = inv.status?.ToString() ?? "",
                                    Amount = amount,
                                    Currency = "USD",
                                    IssuedAt = inv.created_at?.ToString() ?? "",
                                    PaidAt = paidAt,
                                    InvoiceLink = inv.public_url?.ToString() ?? ""
                                });
                            }
                        }
                    }

                    // ── Build Result ─────────────────────────────────────────────────
                    return Json(new
                    {
                        SubscriptionId = subscriptionId,
                        Status = displayStatus,       // ✅ Smart status
                        RawStatus = rawStatus,            // ✅ Original Square status
                        IsStartScheduled = isStartScheduled,     // ✅ true when START SCHEDULED
                        CancelledDate = cancelledDate,         // ✅ Ends-on date
                        CreatedAt = createdAt,
                        ChargeAt = chargedThrough,        // ✅ Charged through date (raw)
                        StartDate = nextBillingDate,       // ✅ Always has a value (next billing)
                        PaymentMethod = cardBrand,
                        CardLast4 = cardLast4,
                        CardExpiry = $"{cardExpMonth}/{cardExpYear}",
                        CardMandateId = cardId,
                        SubscriptionLink = squareEnvironment == "production"
                                           ? $"https://squareup.com/dashboard/subscriptions/{subscriptionId}"
                                           : $"https://squareupsandbox.com/dashboard/subscriptions/{subscriptionId}",
                        CustomerName = customerName,
                        CustomerEmail = customerEmail,
                        CustomerPhone = customerPhone,
                        Invoices = invoicesList
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
        private JsonResult Json(object value, object allowGet)
        {
            throw new NotImplementedException();
        }

        public ActionResult ViewMember(Int64 MemberId = 0, Int64 mid = 0)
        {
            try
            {
                int _qstatus = 0;
                Entities.Members _objMembers = _Members.GetMemberFullDetailsById(MemberId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objMemberDetails = _objMembers;
                    ViewBag.mid = mid;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Transaction";
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Members", new { mid = mid });
                }
            }
            catch (Exception ex)

            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Members", new { mid = mid });
            }
            return View();
        }

        [HttpPost]
        public JsonResult DeleteMember(Int64 MemberId)
        {
            string str = "";
            try
            {
                Int64 _status = _Members.DeleteMembers(MemberId);
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

        public ActionResult DeleteChildInfo(Int64 ChildInfoId, Int64 MemberId = 0)
        {
            try
            {
                Int64 _status = _Members.DeleteChildInfo(ChildInfoId);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Deleted Successfully";

                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return RedirectToAction("EditMember", "Members", new { MemberId = MemberId });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Transaction";
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("EditMember", "Members", new { MemberId = MemberId });
                }
            }
            catch (Exception ex)

            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("EditMember", "Members", new { MemberId = MemberId });
            }
        }

        [HttpPost]
        public JsonResult MembersDeleteAll(string MemberId)
        {
            string str = "";
            try
            {
                Int64 _status = _Members.DeleteAllMembers(MemberId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Records Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the Record"
                    });
                    //str = "<div class=\"error-alert closable\">Failed deleting members.</div>";
                    //return Json(new { ok = false, data = str });
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
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        public JsonResult MemberStatus(Int64 MemberId)
        {
            string str = "";
            try
            {

                int status1 = 0;

                Int64 _status = _Members.UpdateMemberStatus(MemberId);
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
                int status = 0;
                if (_status == 1)
                {
                    //string _password = BLL.Password.GetUniqueKey(8);
                    //string _passwordhash = BLL.Password.ComputeHash(_password, "SHA512", null);

                    //Int64 _pstatus = _Members.ChangePassword(MemberId, _passwordhash, "Yes");
                    //TempData["messageType"] = "warning"; // Assuming "info" for update
                    //TempData["message"] = "Data Updated";




                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Confirm Member", 0, ref status);
                    if (objTemplates != null)
                    {
                        Entities.Members objMember = _Members.GetMemberFullDetailsById(MemberId, ref status);

                        if (objMember.IsApproved == true)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.UserSiteUrl);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.SupportEmail);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objMember.UserName + " " + objMember.FirstName));
                            body.Replace("[MTYPE]", objMember.MembershipType.ToString());
                            body.Replace("[MemberId]", MemberId.ToString());
                            body.Replace("[Email]", objMember.Email);
                            //body.Replace("[Password]", _password);
                            body.Replace("[SiteName]", objAppInfo.SiteName);

                            _sendmail.SendMailSendinbrevo(objMember.Email, objTemplates.Subject, body.ToString());
                        }
                    }
                    // return Json(new { ok = true, data = str });
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });



                }
                else
                {
                    //return Json(new
                    //{
                    //    ok = false,
                    //    messageType = "error",
                    //    message = "Failed updating Member status"
                    //});

                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating Member status"
                    });



                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed Updating Member Status</div>";
                    //return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
                //return Json(new
                //{
                //    ok = false,
                //    messageType = "error",
                //    message = "Failed transaction."
                //});
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction.</div>";
                //return Json(new { ok = true, data = str });
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
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
                //return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>"});
            }
        }

        [HttpPost]
        public JsonResult CheckEmailAvailability(string Email)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByEmail(Email, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
                //return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>" });
            }
        }

        //[HttpPost]
        //public JsonResult CheckUserNameAvailability(string UserName)
        //{
        //    int status = 0;
        //    try
        //    {
        //        objMembers = _Members.GetMemberFullDetailsByUserName(UserName, ref status);
        //        bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

        //        return Json(new { ok = true, data = data, message = "" });
        //    }
        //    catch
        //    {
        //        return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>"});
        //    }
        //}


        [HttpPost]
        public JsonResult CheckProfilePhoneNoAvailability(Int64 MemberId, string MobilePhone)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByPhoneNo(MobilePhone, ref status);


                bool data = (objMembers.MemberId == MemberId || objMembers.MemberId == 0 ? true : false);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
                //return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>" });
            }
        }

        [HttpPost]
        public JsonResult CheckPhoneNoAvailability(string MobilePhone)
        {
            int status = 0;
            try
            {
                objMembers = _Members.GetMemberFullDetailsByPhoneNo(MobilePhone, ref status);
                bool data = (objMembers != null && objMembers.MemberId != 0 ? false : true);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
                //return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>" });
            }
        }


        #endregion
        public IActionResult MembersExporttoExcel(
            long ChapterId = 0,
            string Search = "",
            long MembershipTypeId = 0,
            long PaymentStatusId = 0,
            string StartDate = "",
            string EndDate = "",
            string IsVolunteer = "",
            string SortColumn = "M.InsertedTime",
            string SortOrder = "DESC",
            string type = "allactive")   // ✅ new parameter, default = allactive
        {
            try
            {
                string Sort = !string.IsNullOrEmpty(SortColumn) ? $"{SortColumn} {SortOrder}" : "";
                // Get data from DB
                DataTable dtUni = _DMembers.ExportMembersList(
                    ChapterId, Search, MembershipTypeId, PaymentStatusId, StartDate, EndDate, IsVolunteer, Sort, type);
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Member-Details-Export");
                    for (int col = 0; col < dtUni.Columns.Count; col++)
                    {
                        worksheet.Cells[1, col + 1].Value = dtUni.Columns[col].ColumnName;
                        worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, col + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, col + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 216, 230));
                    }
                    for (int row = 0; row < dtUni.Rows.Count; row++)
                    {
                        for (int col = 0; col < dtUni.Columns.Count; col++)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = dtUni.Rows[row][col];
                        }
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    var content = package.GetAsByteArray();
                    string fileName = $"Member-Details-Export-{DateTime.UtcNow:MM-dd-yyyy}.xlsx";
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
        //#region BulkuploadOrganizationMembers

        //[Authorize]
        //[HttpPost]
        //public ActionResult MembersBulkUpload(HttpPostedFileBase Document, Entities.Members objMembers1)
        //{
        //    string ShipmentsXML = "";
        //    string Summary = "";
        //    int status = 0;
        //    try
        //    {
        //        Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status);
        //        //Upload and save the file
        //        string excelPath = Server.MapPath("~/Content/temp/") + DateTime.UtcNow.ToString("MM-dd-yyyy") + "-" + Path.GetFileName(Document.FileName);
        //        Document.SaveAs(excelPath);

        //        string conString = string.Empty;
        //        string extension = Path.GetExtension(Document.FileName);
        //        switch (extension)
        //        {
        //            case ".xls": //Excel 97-03
        //                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
        //                break;
        //            case ".xlsx": //Excel 07 or higher
        //                conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
        //                break;





        //        }
        //        conString = string.Format(conString, excelPath);
        //        using (OleDbConnection excel_con = new OleDbConnection(conString))
        //        {
        //            excel_con.Open();
        //            string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
        //            DataTable dtExcelData = new DataTable();

        //            using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
        //            {
        //                oda.Fill(dtExcelData);
        //            }
        //            List<Entities.Members> lstMembers = new List<Entities.Members>();
        //            if (dtExcelData.Rows.Count != 0)
        //            {
        //                foreach (DataRow dr in dtExcelData.Rows)
        //                {
        //                    Guid guid = AWS_AssociationToolCore.BLL.Common.generateGUID();

        //                    Entities.Members objMembers = new Entities.Members();
        //                    objMembers.UserName = dr["First Name"].ToString() + " " + dr["Last Name"].ToString();
        //                    objMembers.FirstName = (dr["First Name"] != DBNull.Value ? (dr["First Name"]).ToString() : null);
        //                    objMembers.LastName = (dr["Last Name"] != DBNull.Value ? (dr["Last Name"]).ToString() : null);
        //                    objMembers.Email = (dr["Email"] != DBNull.Value ? (dr["Email"]).ToString() : null);
        //                    //if(dr["Password"].ToString() != null && dr["Password"].ToString() != "")
        //                    //{
        //                    //    objMembers.Password = Password.ComputeHash(dr["Password"].ToString().Trim(), "SHA512", null);
        //                    //}
        //                    //else{
        //                    //    objMembers.Password = (dr["Password"] != DBNull.Value ? (dr["Password"]).ToString() : null);
        //                    //}
        //                    objMembers.Occupation = (dr["Occupation"] != DBNull.Value ? (dr["Occupation"]).ToString() : null);
        //                    objMembers.MemberAge = (dr["Member Age"] != DBNull.Value ? (dr["Member Age"]).ToString() : null);
        //                    //objMembers.MemberSkils = (dr["Member Skils"] != DBNull.Value ? (dr["Member Skils"]).ToString() : null);
        //                    objMembers.SpouseFirstName = (dr["Spouse FirstName"] != DBNull.Value ? (dr["Spouse FirstName"]).ToString() : null);
        //                    objMembers.SpouseLastName = (dr["Spouse LastName"] != DBNull.Value ? (dr["Spouse LastName"]).ToString() : null);
        //                    objMembers.SpouseOccupation = (dr["Spouse Occupation"] != DBNull.Value ? (dr["Spouse Occupation"]).ToString() : null);
        //                    objMembers.SpouseEmail = (dr["Spouse Email"] != DBNull.Value ? (dr["Spouse Email"]).ToString() : null);
        //                    objMembers.SpouseCell = (dr["SpouseCell"] != DBNull.Value ? (dr["SpouseCell"]).ToString() : null);
        //                    objMembers.SpouseSkils = (dr["SpouseSkils"] != DBNull.Value ? (dr["SpouseSkils"]).ToString() : null);
        //                    objMembers.Address = (dr["Address"] != DBNull.Value ? (dr["Address"]).ToString() : null);
        //                    objMembers.City = (dr["City"] != DBNull.Value ? (dr["City"]).ToString() : null);
        //                    objMembers.State = (dr["State"] != DBNull.Value ? (dr["State"]).ToString() : null);
        //                    objMembers.ZipCode = (dr["Zip Code"] != DBNull.Value ? (dr["Zip Code"]).ToString() : null);
        //                    objMembers.HomePhone = (dr["Home Phone"] != DBNull.Value ? (dr["Home Phone"]).ToString() : null);
        //                    objMembers.MobilePhone = (dr["Mobile Phone"] != DBNull.Value ? (dr["Mobile Phone"]).ToString() : null);
        //                    //objMembers.RegistrationGUID = guid;
        //                   // objMembers.MembershipType = (dr["Membership Type"] != DBNull.Value ? (dr["Membership Type"]).ToString() : null);
        //                    objMembers.Comments = (dr["Comments"] != DBNull.Value ? (dr["Comments"]).ToString() : null);
        //                   // objMembers.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"].ToString()) : 0);

        //                    lstMembers.Add(objMembers);
        //                }
        //            }

        //            ShipmentsXML = BLL.Common.CreateXMLForObject(lstMembers);
        //        }

        //        if (System.IO.File.Exists(objappinfo.UploadPath + @"\\temp\\" + Path.GetFileName(Document.FileName)))
        //        {
        //            System.IO.File.Delete(objappinfo.UploadPath + @"\\temp\\" + Path.GetFileName(Document.FileName));
        //        }

        //        Int64 _status = _Members.MembersBulkUpload(ShipmentsXML, ref Summary);
        //        switch (_status)
        //        {
        //            case 1:
        //                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\"> Inserted / Updated Member(s) successfully.</div>";
        //                return RedirectToAction("Index", "Members");
        //            case 2:
        //                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\"> Inserted / Updated Member(s) successfully.</div>";
        //                return RedirectToAction("Index", "Members");
        //            case -1:
        //                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\"> Failed updating Member(s) details.</div>";
        //                return RedirectToAction("Index", "Members");
        //            default:
        //                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\"> Failed updating Member(s) details.</div>";
        //                return RedirectToAction("Index", "Members");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
        //        return RedirectToAction("Index", "Members");
        //    }
        //}

        //[Authorize]
        //public FileStreamResult CreateFile(string str = "", string type = "")
        //{
        //    //todo: add some data from your database into that string:
        //    var string_with_your_data = str;
        //    string_with_your_data = str.Replace("\\\\r\\\\n", Environment.NewLine);
        //    var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
        //    var stream = new MemoryStream(byteArray);

        //    return File(stream, "text/plain", "Member-Bulk-Upload-output.txt");
        //}

        //#endregion



        #region BulkuploadOrganizationMembers

        [HttpPost]
        public async Task<IActionResult> MembersBulkUpload(IFormFile Document, Entities.Members objMembers1)
        {
            string ShipmentsXML = "";
            string Summary = "";
            int status1 = 0;

            Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

            if (Document == null || Document.Length == 0)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Please upload a valid file.";
                return RedirectToAction("Index", "Members");
            }

            try
            {
                // Save uploaded file temporarily
                var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "temp");
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                var fileName = DateTime.UtcNow.ToString("MM-dd-yyyy") + "-" + Path.GetFileName(Document.FileName);
                var filePath = Path.Combine(tempPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Document.CopyToAsync(stream);
                }

                // Read Excel file
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                DataTable dtExcelData = new DataTable();

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    dtExcelData = result.Tables[0];
                }

                // Map Excel rows to member objects
                List<Entities.Members> lstMembers = new List<Entities.Members>();
                foreach (DataRow dr in dtExcelData.Rows)
                {
                    Entities.Members objMembers = new Entities.Members
                    {
                        FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null,
                        LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null,
                        Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null,
                        SpouseFirstName = dr["SpouseFirstName"] != DBNull.Value ? dr["SpouseFirstName"].ToString() : null,
                        SpouseLastName = dr["SpouseLastName"] != DBNull.Value ? dr["SpouseLastName"].ToString() : null,
                        gender = dr["gender"] != DBNull.Value ? dr["gender"].ToString() : null,
                        MaritalStatus = dr["MaritalStatus"] != DBNull.Value ? dr["MaritalStatus"].ToString() : null,
                        SpouseEmail = dr["SpouseEmail"] != DBNull.Value ? dr["SpouseEmail"].ToString() : null,
                        MemberSkils = dr["MemberSkils"] != DBNull.Value ? dr["MemberSkils"].ToString() : null,
                        SpouseCell = dr["SpouseCell"] != DBNull.Value ? dr["SpouseCell"].ToString() : null,
                        Address = dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null,
                        Address2 = dr["Address2"] != DBNull.Value ? dr["Address2"].ToString() : null,
                        //City = dr["City"] != DBNull.Value ? dr["City"].ToString() : null,
                        //City = dr["City"] != DBNull.Value ? dr["City"].ToString() : null,
                        //State = dr["State"] != DBNull.Value ? dr["State"].ToString() : null,
                        ZipCode = dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null,
                        //HomePhone = dr["HomePhone"] != DBNull.Value ? dr["HomePhone"].ToString() : null,
                        MobilePhone = dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : null,
                        MembershipType = dr["MembershipType"] != DBNull.Value ? dr["MembershipType"].ToString() : null,
                        //TransactionId = dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null,
                        UserComment = dr["UserComment"] != DBNull.Value ? dr["UserComment"].ToString() : null,
                        PaymentStatusId = dr["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentStatusId"]) : 0,
                        PaymentMethodId = dr["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentMethodId"]) : 0,
                        Amount = dr["Amount"] != DBNull.Value ? Convert.ToInt64(dr["Amount"]) : 0,
                        ExpiryDate = ParseExcelDate(dr["ExpiryDate"], DateTime.MinValue),
                        InsertedTime = ParseExcelDate(dr["InsertedTime"], DateTime.Now),
                        UpdatedTime = ParseExcelDate(dr["UpdatedTime"], DateTime.UtcNow),

                        // ✅ IsApproved — handles TRUE/FALSE string, true/false boolean, 1/0 number
                        IsApproved = dr["IsApproved"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["IsApproved"].ToString())
                ? !(dr["IsApproved"].ToString().Trim().ToUpper() == "FALSE" || dr["IsApproved"].ToString().Trim() == "0")
                : true,


                        Child1FirstName = dr["Child1_FirstName"] != DBNull.Value ? dr["Child1_FirstName"].ToString() : null,
                        Child1LastName = dr["Child1_LastName"] != DBNull.Value ? dr["Child1_LastName"].ToString() : null,
                        Child1Age = ParseChildAge(dr["Child1_Age"]),
                        Child2FirstName = dr["Child2_FirstName"] != DBNull.Value ? dr["Child2_FirstName"].ToString() : null,
                        Child2LastName = dr["Child2_LastName"] != DBNull.Value ? dr["Child2_LastName"].ToString() : null,
                        Child2Age = ParseChildAge(dr["Child2_Age"]),
                        Child3FirstName = dr["Child3_FirstName"] != DBNull.Value ? dr["Child3_FirstName"].ToString() : null,
                        Child3LastName = dr["Child3_LastName"] != DBNull.Value ? dr["Child3_LastName"].ToString() : null,
                        Child3Age = ParseChildAge(dr["Child3_Age"]),
                    };

                    lstMembers.Add(objMembers);
                }
                ShipmentsXML = BLL.Common.CreateXMLForObject(lstMembers);

                // Delete temp file
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                Int64 _status = _Members.MembersBulkUploads(ShipmentsXML, ref Summary);

                switch (_status)
                {
                    case 1:
                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Inserted / Updated Member(s) successfully.";
                        TempData["CloseModal"] = true;
                        return CreateFile(Summary, "Members");

                    case -1:
                    default:
                        TempData["CloseModal"] = true;
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed Transaction";
                        return RedirectToAction("Index", "Members");
                }
            }
            catch (Exception ex)
            {
                TempData["CloseModal"] = true;
                TempData["messageType"] = "error";
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">{ex.Message}</div>";
                return RedirectToAction("Index", "Members");
            }
        }
        private DateTime ParseExcelDate(object cellValue, DateTime defaultValue)
        {
            if (cellValue == null || cellValue == DBNull.Value)
                return defaultValue;

            string dateStr = cellValue.ToString().Trim();
            if (string.IsNullOrWhiteSpace(dateStr))
                return defaultValue;

            // If Excel stored it as OLE Automation Date (numeric)
            if (double.TryParse(dateStr, out double oaDate))
            {
                try { return DateTime.FromOADate(oaDate); }
                catch { }
            }

            string[] formats = {
        "M/d/yyyy H:mm",
        "M/d/yyyy HH:mm",
        "MM/dd/yyyy HH:mm",
        "MM/dd/yyyy H:mm",
        "M/d/yyyy",
        "MM/dd/yyyy",
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "d/MM/yyyy H:mm",
        "d/MM/yyyy"
    };

            if (DateTime.TryParseExact(dateStr, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime result))
            {
                return result;
            }

            // Last fallback
            if (DateTime.TryParse(dateStr, out DateTime fallback))
                return fallback;

            return defaultValue; // never crash
        }
        // Helper to extract age from Excel cell (handles numeric, string, and Excel date-serial)
        private string ParseChildAge(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value) return null;
            string raw = cellValue.ToString().Trim();
            if (string.IsNullOrEmpty(raw)) return null;

            DateTime dob = DateTime.MinValue;

            // ExcelDataReader returned it as DateTime directly
            if (cellValue is DateTime dt)
            {
                dob = dt;
            }
            // It came as a string like "6/1/2010"
            else if (DateTime.TryParse(raw, out DateTime parsed))
            {
                dob = parsed;
            }
            // It came as OLE date serial number like 36624
            else if (double.TryParse(raw, out double oaDate) && oaDate > 1000)
            {
                try { dob = DateTime.FromOADate(oaDate); } catch { }
            }

            if (dob != DateTime.MinValue && dob.Year > 1900)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age)) age--;
                return age.ToString();
            }

            // Fallback: plain small number already (e.g. "8")
            if (double.TryParse(raw, out double d) && d < 200)
                return ((int)d).ToString();

            return null;
        }
        public FileStreamResult CreateFile(string str = "", string type = "")
        {
            //todo: add some data from your database into that string:
            var string_with_your_data = str;
            string_with_your_data = str.Replace("\\\\r\\\\n", Environment.NewLine);
            var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain", "Member-Bulk-Upload-output.txt");
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
