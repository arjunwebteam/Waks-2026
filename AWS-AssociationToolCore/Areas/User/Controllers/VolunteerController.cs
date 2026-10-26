using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class VolunteerController : Controller
    {
        BLL.Volunteers _Volunteers = new BLL.Volunteers();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        List<Entities.Volunteers> lstVolunteers = new List<Entities.Volunteers>();
        BLL.VolunteerCategories _VolunteerCategory = new BLL.VolunteerCategories();
        List<Entities.VolunteerCategories> lstVolunteerCategory = new List<Entities.VolunteerCategories>();
        BLL.Events _Events = new BLL.Events();
         BLL.Members _Members = new BLL.Members();
         Entities.Members _objMember = new Entities.Members();
         AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
         HomeController _HomeController = new HomeController();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        Entities.Events objEvents = new Entities.Events();
        List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
        List<Entities.EventDates> lstEventDates = new List<Entities.EventDates>();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        BLL.SendMail _sentmail = new BLL.SendMail();
        BLL.Users _Users = new BLL.Users();

        //
        [HttpGet]
        public ActionResult AddVolunteer(Int64 VolunteerCategoryId = 0, Int64 EventId = 0, Int64 MemberId = 0, Int64 ChapterId = 0, string cname = "", Int64 CountryID = 0)
        {
            string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);
            // ✅ Read cookie safely (ASP.NET Core way)
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                cname = cookieValue;
            }
            int _qstatus = 0;
            int status = 0;
            int qstatus = 0;
            int status1 = 0;
            List<Entities.country> lstCountries = new List<Entities.country>();
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            List<Entities.VolunteerCategories> lstVolunteerCategory = _VolunteerCategory.GetVolunteerCategoriesListByChapterId(ChapterId, ref _qstatus);
            List<Entities.RegistrationCategories> lstEventRegistrationCategories = new List<Entities.RegistrationCategories>();
            lstCountries = _Volunteers.GetCountriesList(ref status1);

            if (EventId != 0)
            {
                objEvents = _Events.GetEventById(EventId, "", ref lstEventRegistrationTypes, ref lstEventDates, ref lstEventRegistrationCategories, ref status);
            }
            string currentUserEmail = "";
            currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";

            // Auth check in Core
            // ✅ Fixed - clean and correct
            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(currentUserEmail))
            {
                _objMember = _Members.GetMemberFullDetailsByEmail(currentUserEmail, ref _qstatus);
            }
            MemberId = Convert.ToInt64(HttpContext.Session.GetString("MemberID") ?? "0");
            Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();

            Int64 qmstatus = 0;
            objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl(cname + "/volunteer-registration", ref qmstatus);
            // Save the code in a cookie
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,                              // Prevent client-side scripts from accessing the cookie
                Secure = Request.IsHttps,                     // Ensure the cookie is sent over HTTPS only
                SameSite = SameSiteMode.Strict,               // Extra security
                Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Set expiry
            };

            Response.Cookies.Append("FormValidationCode", generatedCode, cookieOptions); // ✅ Keep only this

       

            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.Memberdetails = _objMember;
            ViewBag.MemberId = MemberId;
            ViewBag.lstVolunteerCategory = lstVolunteerCategory;
            ViewBag.VolunteerCategoryId = VolunteerCategoryId;
            ViewBag.EventId = EventId;
            ViewBag.MemberId = MemberId;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.objEvents = objEvents;
            ViewBag.lstCountries = lstCountries;
            ViewBag.ValidationCode = generatedCode;
            return View();
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddVolunteerRegistration(Entities.Volunteers objVolunteers, string cname = "", string MCaptcha = "", string recaptchaResponse = "", string Honeypot = "", string ValidationCode = "")
        {
                try
                {
                Int64 LogId = 0;
                String WaksCategoryId = "";
                Int64 KannadaCategoryId = 0;
                var objLogReport = new Entities.ApplicationLogs
                {
                    LogTitle = "Volunteers",
                    LogDescription = "Volunteers Form Submission",
                    LogDate = DateTime.Now,
                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objVolunteers.Email ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                };
                _Users.LogReportInsert(objLogReport, ref LogId);
                int status1 = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                //Form Security

                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Volunteers",
                        LogDescription = "Honeypot Bot activity detected",
                        LogDate = DateTime.Now,
                        InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objVolunteers.Email ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }
                if (Request.Form["ClientValidationStatus"] == "true")
                {
                    // Skip ModelState validation for the payment gateway fields
                    ModelState.Clear();
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }

                string YoursecretKey = objAppInfo.CapchaSecreatKey; //Put Your Secret Key .

                var capcha = objAppInfo.ShowCapcha;

                // Verify reCAPTCHA token
                bool isHuman = await BLL.Common.VerifyRecaptchaToken(recaptchaResponse);

              

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
                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()


                };


                _Users.LogSubReportInsert(LogSubReport);
                if (!isHuman)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }

                // Retrieve the code from the cookie
                string cookieValidationCode = Request.Cookies["FormValidationCode"];
                if (string.IsNullOrEmpty(cookieValidationCode))
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Volunteers",
                        LogDescription = "Failed Retrieve the code from the cookie.",
                        LogDate = DateTime.Now,
                        InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }

                // Compare the hidden field value with the cookie value
                if (cookieValidationCode != ValidationCode)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Volunteers",
                        LogDescription = "Failed Security validation failed..",
                        LogDate = DateTime.Now,
                        InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }

                if (ModelState.IsValid)
                {
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Volunteers",
                        LogDescription = "After reCAPTCHA Entered to the Process",
                        LogDate = DateTime.Now,
                        InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    objVolunteers.InsertedBy = objVolunteers.FirstName;
                    objVolunteers.UpdatedBy = objVolunteers.FirstName;
                    objVolunteers.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objVolunteers.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                    objVolunteers.IsActive = false;
                    string Password = "";
                    if (objVolunteers.Field1 == null || objVolunteers.Field1 == "")
                    {
                        Password = BLL.Common.GetRandomString(8);
                        string _passwordhash = BLL.Password.ComputeHash(Password.Trim(), "SHA512", null);
                        objVolunteers.Field1 = _passwordhash.ToString();
                    }
                    else
                    {
                        Password = objVolunteers.Field1;
                        string _passwordhash = BLL.Password.ComputeHash(objVolunteers.Field1.Trim(), "SHA512", null);
                        objVolunteers.Field1 = _passwordhash.ToString();
                    }

                    Int64 _status = _Volunteers.FEVolunteerInsert(objVolunteers);

                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Volunteer Insert",
                        LogDescription = status1 == 1
                                                 ? $"Volunteer inserted successfully."
                                                 : $"Volunteer insert failed. Status: {status1}",
                        LogDate = DateTime.Now,
                        InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });

                    if (_status == 1 || _status == 2)
                    {
                        int status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("User Volunteer Registration", 0, ref status);
                        if (objTemplates != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objVolunteers.FirstName));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[Email]", objVolunteers.Email);
                            body.Replace("[PASSWORD]", Password);


                            if (objAppInfo.Email == "Yes")
                            {
                                _sentmail.SendMailSendinbrevowithfrom(objVolunteers.Email, objAppInfo.VolunteerEmail, "Thanks For Becoming A Volunteer - " + objAppInfo.SiteName, body.ToString());
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "User Email Sent",
                                    LogDescription = $"Confirmation email sent to user: {objVolunteers.Email}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                        }

                        int status2 = 0;
                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Volunteer Registration For Admin", 0, ref status2);
                        if (objTemplates1 != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates1.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objVolunteers.FirstName));
                            body.Replace("[LastName]", BLL.Common.UppercaseFirst(objVolunteers.LastName));
                            body.Replace("[Email]", objVolunteers.Email);
                            body.Replace("[PhoneNo]", objVolunteers.PhoneNo);
                            body.Replace("[HoursSpent]", objVolunteers.HoursSpent);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                _sentmail.SendMailSendinbrevowithfrom(objAppInfo.VolunteerEmail, objAppInfo.VolunteerEmail, "You have a new Volunteer Registration from " + objAppInfo.SiteName, body.ToString());
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Admin Email Sent",
                                    LogDescription = $"Confirmation email sent to Admin: {objAppInfo.VolunteerEmail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            
                            }
                        return RedirectToAction("ThankYou", "Volunteer", new { cname = cname });

                    }
                    if (_status == 3)
                    {
                        int status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Volunteer Registration", 0, ref status);
                        if (objTemplates != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objVolunteers.FirstName));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[EventName]", objVolunteers.EventName);
                            if (objAppInfo.Email == "Yes")
                            {
                                _sentmail.SendMailSendinbrevowithfrom(objVolunteers.Email, objAppInfo.VolunteerEmail, "Thanks For Becoming A Volunteer" + objAppInfo.SiteName, body.ToString());
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "User Email Sent",
                                    LogDescription = $"Confirmation email sent to user: {objVolunteers.Email}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            }

                        int status2 = 0;
                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Volunteer Registration For Admin", 0, ref status2);
                        if (objTemplates1 != null)
                        {

                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates1.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[FirstName]", BLL.Common.UppercaseFirst(objVolunteers.FirstName));
                            body.Replace("[LastName]", BLL.Common.UppercaseFirst(objVolunteers.LastName));
                            body.Replace("[Email]", objVolunteers.Email);
                            body.Replace("[PhoneNo]", objVolunteers.PhoneNo);
                            body.Replace("[HoursSpent]", objVolunteers.HoursSpent);
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            if (objAppInfo.Email == "Yes")
                            {
                                _sentmail.SendMailSendinbrevowithfrom(objAppInfo.VolunteerEmail, objAppInfo.VolunteerEmail, "You have a new Volunteer Registration from " + objAppInfo.SiteName, body.ToString());
                                _Users.LogSubReportInsert(new Entities.Logdetails
                                {
                                    LogId = LogId,
                                    LogTitle = "Admin Email Sent",
                                    LogDescription = $"Confirmation email sent to Admin: {objAppInfo.MemberEmail}",
                                    LogDate = DateTime.Now,
                                    InsertedBy = objVolunteers.FirstName ?? "Anonymous",
                                    InsertedDate = DateTime.Now,
                                    UpdatedBy = objVolunteers.FirstName ?? "Anonymous",
                                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                });
                            }
                            
                            }
                        return RedirectToAction("ThankYou", "Volunteer", new { cname = cname });
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                        return RedirectToAction("AddVolunteer", "Volunteer");
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"error1 closable\">Google reCaptcha not validated Successfully</div>";
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }

                //else
                //{
                //    TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                //    return RedirectToAction("AddVolunteer", "Volunteer");
                //}
            }
                catch
                {
                    TempData["message"] = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return RedirectToAction("AddVolunteer", "Volunteer");
                }
        }

        public ActionResult ThankYou(string cname = "")
        {
            ViewBag.cname = cname;
            return View();
        }

        [HttpGet]
        public ActionResult Certicate(Int64 ChapterId=0)
        {
            int status = 0;

            List<Entities.VolunteerCategories> lstVolunteerCategories = new List<Entities.VolunteerCategories>();
            List<Entities.Events> lstEvents = _Events.GetEventsListall(ref status);

            lstVolunteerCategories = _VolunteerCategory.GetVolunteerCategoriesList(ChapterId,ref status);


            ViewBag.lstVolunteerCategories = lstVolunteerCategories;
            ViewBag.lstEvents = lstEvents;
            return View();
        }
        public ActionResult FEVolunteersList(Int64 VolunteerCategoryId = 0, Int64 EventId = 0, string Email = "", string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.Volunteers> lstVolunteers = new List<Entities.Volunteers>();

            try
            {
                lstVolunteers = _Volunteers.FEGetVolunteersList(VolunteerCategoryId, EventId, Email, Search, Sort, PageNo, Items, ref Total);
            }
            catch(Exception EX)
            {
                throw EX;
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstVolunteers = lstVolunteers;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.Email = Email;
            return View();
        }


        [HttpPost]
        public JsonResult CheckEmailAvailability(string Email)
        {
            int status = 0;
            try
            {
                Entities.Volunteers objVolunteers = _Volunteers.VolunteerValidationByEmail(Email, ref status);
                bool data = (objVolunteers != null && objVolunteers.VolunteerId == 0 ? true : false);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed transaction.</div>" });
            }
        }

       
        public JsonResult UpdateVolunteerHours(string HoursSpent, Int64 VolunteerId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _Volunteers.UpdateVolunteerHours(HoursSpent, VolunteerId);
            if (_status == 1)
            {
                str = "<div class=\"alert alert-success alert-dismissable\">Update Hours Successfully</div>";
                return Json(new { ok = true, data = str });
            }
            else
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }

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

            return View();
        }
     
        [HttpPost]
        public JsonResult GetCaptcha()
        {
            try
            {
                string str = HttpContext.Session.GetString("captchastring");

                if (string.IsNullOrEmpty(str))
                    return Json(new { ok = false, message = "<div class=\"error closable\">Session Expired</div>" });

                return Json(new { ok = true, data = str, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error closable\">Failed Transaction</div>" });
            }
        }
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

        public ActionResult GetStatesByCountryID(Int64 CountryID = 0)
        {
            string str = "";
            string message = "";
            BLL.Volunteers _Volunteers = new BLL.Volunteers();

            try
            {
                int status = 0;
                List<AWS_AssociationToolCore.Entities.State> lstcountry = _Volunteers.GetStatesByCountryID(CountryID, ref status);
                if (status == 1)
                {
                    return Json(new { ok = true, data = lstcountry });
                }
                else
                {
                    message = "error";
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                message = "error";
                return Json(new { ok = true, data = str });
            }
        }

    }
}
