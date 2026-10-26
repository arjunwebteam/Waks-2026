using AWS_AssociationToolCore.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.Web.Mvc;
//using System.Web.Helpers;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    //Hii Mam
    [Area("User")]

    public class HomeController : Controller
    {
        BLL.AppInfo _AppInfo = new BLL.AppInfo();
        BLL.Enquiries _Enquiries = new BLL.Enquiries();
        BLL.MailTemplates _MailTemplates = new BLL.MailTemplates();
        BLL.Members _Members = new BLL.Members();
        Entities.Members _objMember = new Entities.Members();
        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new BLL.AppInfo();
        BLL.Events _Events = new BLL.Events();
        Entities.Enquiries objEnquiriy = new Entities.Enquiries();
        AWS_AssociationToolCore.BLL.Enquiries _Enquiry = new AWS_AssociationToolCore.BLL.Enquiries();
        Entities.Events objEvents = new Entities.Events();
        List<Entities.EventCategories> lstEventCategories = new List<Entities.EventCategories>();
        List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
        BLL.InnerPages _InnerPages = new BLL.InnerPages();
        BLL.NewsLetter _NewsLetter = new BLL.NewsLetter();
        BLL.Chapters _Chapters = new BLL.Chapters();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();
        BLL.Redirections _Redirections = new BLL.Redirections();
        Entities.Redirections objRedirections = new Entities.Redirections();
        BLL.SendMail _sentmail = new BLL.SendMail();
        BLL.Users _Users = new BLL.Users();
        private readonly ProjectHoneypotClient _honeypotService;

        public ActionResult Index(string str = "", string type = "", string ttype = "",Int64 ChapterId = 0, string cname = "", string w = "")
        {
            // ✅ Core way to get absolute URL
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            int status1 = 0;

            objRedirections = _Redirections.GetRedirectionsByurl(url, ref status1);

            if (objRedirections.ToUrl != null && objRedirections.ToUrl != "")
            {
                return new RedirectResult(objRedirections.ToUrl);
            }

            int qstatus = 0;

            // ✅ Core way to set cookies
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

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

            List<Entities.MenuItems> lstMenuItems = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems2 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems3 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> lstMenuItems4 = new List<Entities.MenuItems>();
            List<Entities.MenuItems> FooterMenuItems = new List<Entities.MenuItems>();
            List<Entities.MenuItems> QuickLinkItems = new List<Entities.MenuItems>();
            Entities.AppInfo objAppInfo = new Entities.AppInfo();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            List<Entities.Redirections> lstRedirections = new List<Entities.Redirections>();
            Entities.PageDetails objPInnerPages = new Entities.PageDetails();
            Entities.PageDetails objWInnerPages = new Entities.PageDetails();
            Entities.PageDetails objvInnerPages = new Entities.PageDetails();
            List<Entities.Sponsors> lstSponsors = new List<Entities.Sponsors>();
            List<Entities.SponsorCategories> lstSponsorCategories = new List<Entities.SponsorCategories>();
            List<Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();
            List<Entities.Videos> lstVideos = new List<Entities.Videos>();

            int status = 0;
            try
            {
                _AppInfo.FEFinalGetListInitialLoad(ChapterId, ref objPInnerPages, ref objWInnerPages,
                    ref lstSponsors, ref lstSponsorCategories, ref lstCommitteeCategories,
                    ref lstMenuItems, ref lstMenuItems2, ref lstMenuItems3, ref lstMenuItems4,
                    ref FooterMenuItems, ref objAppInfo, ref objvInnerPages, ref QuickLinkItems,
                    ref lstChapters, ref lstPhotos, ref lstVideos, ref status);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objAppInfo = objAppInfo;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.lstMenuItems2 = lstMenuItems2;
            ViewBag.lstMenuItems3 = lstMenuItems3;
            ViewBag.lstMenuItems4 = lstMenuItems4;
            ViewBag.FooterMenuItems = FooterMenuItems;
            ViewBag.QuickLinkItems = QuickLinkItems;
            ViewBag.str = str;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.w = w;
            ViewBag.objPInnerPages = objPInnerPages;
            ViewBag.objWInnerPages = objWInnerPages;
            ViewBag.objvInnerPages = objvInnerPages;
            ViewBag.lstPhotos = lstPhotos;
            ViewBag.lstVideos = lstVideos;
            ViewBag.lstSponsors = lstSponsors;
            ViewBag.lstSponsorCategories = lstSponsorCategories;
            ViewBag.lstCommitteeCategories = lstCommitteeCategories;
            ViewBag.lstChapters = lstChapters;

            return View();
        }
        public ActionResult Banners(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            List<Entities.WebsiteBanners> lstWebsiteBanners = new List<Entities.WebsiteBanners>();

            int status = 0;
            try
            {
                lstWebsiteBanners = _AppInfo.FEGetWebsiteBannersList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstWebsiteBanners = lstWebsiteBanners;

            return View();
        }
        public ActionResult BannerRightBlock(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            AWS_AssociationToolCore.BLL.Committees _Committees = new AWS_AssociationToolCore.BLL.Committees();
            List<AWS_AssociationToolCore.Entities.Members> lstCommittees = new List<AWS_AssociationToolCore.Entities.Members>();
            List<AWS_AssociationToolCore.Entities.Members> lstCommitteeMembers = new List<AWS_AssociationToolCore.Entities.Members>();

            int status = 0;
            try
            {
                lstCommitteeMembers = _AppInfo.FEGetCommitteeMembersList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstCommitteeMembers = lstCommitteeMembers;

            return View();
        }
        public ActionResult LatestNews(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            List<Entities.News> lstNews = new List<Entities.News>();

            int status = 0;
            try
            {
                lstNews = _AppInfo.FEGetNewsList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstNews = lstNews;

            return View();
        }
        public ActionResult VisionMission(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            Entities.PageDetails objNatsMissionInnerPages = new Entities.PageDetails();

            int status = 0;
            try
            {
                objNatsMissionInnerPages = _AppInfo.FEGetVisionDetails(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.objNatsMissionInnerPages = objNatsMissionInnerPages;

            return View();
        }
        public ActionResult UpcomingEvents(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            List<Entities.Events> lstUpcommingEvents = new List<Entities.Events>();

            int status = 0;
            try
            {
                lstUpcommingEvents = _AppInfo.FEGeUpcommingEventsList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstUpcommingEvents = lstUpcommingEvents;

            return View();
        }
        public ActionResult Contributions(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20)
            };
            Response.Cookies.Append("ChapterName", cname ?? "", cookieOptions);
            Response.Cookies.Append("ChapterId", ChapterId.ToString(), cookieOptions);

            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();

            int status = 0;
            try
            {
                objServiceDonations = _AppInfo.FEGetServiceDonationsGetTotalAmount(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.objServiceDonations = objServiceDonations;

            return View();
        }
        public ActionResult ActiveCauses(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            SetChapterCookie(cname, ChapterId);

            List<Entities.Services> lstServices = new List<Entities.Services>();

            int status = 0;
            try
            {
                lstServices = _AppInfo.FEGetActiveServicesList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstServices = lstServices;

            return View();
        }

        private void SetChapterCookie(string cname, long chapterId)
        {
            throw new NotImplementedException();
        }

        public ActionResult ChapterNews(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            SetChapterCookie(cname, ChapterId);

            List<Entities.News> lstNews = new List<Entities.News>();

            int status = 0;
            try
            {
                lstNews = _AppInfo.FEGetNewsList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstNews = lstNews;

            return View();
        }
        public ActionResult PhotoGallery(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            SetChapterCookie(cname, ChapterId);

            List<Entities.Photos> lstPhotos = new List<Entities.Photos>();

            int status = 0;
            try
            {
                lstPhotos = _AppInfo.FEGetPhotosList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstPhotos = lstPhotos;

            return View();
        }
        public ActionResult VideoGallery(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            SetChapterCookie(cname, ChapterId);

            List<Entities.Videos> lstVideos = new List<Entities.Videos>();

            int status = 0;
            try
            {
                lstVideos = _AppInfo.FEGetVideosList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstVideos = lstVideos;

            return View();
        }
        public ActionResult ChaptersList(Int64 ChapterId = 0, string cname = "")
        {
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

            // ✅ ASP.NET Core cookie way
            SetChapterCookie(cname, ChapterId);

            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();

            int status = 0;
            try
            {
                lstChapters = _AppInfo.FEGetChaptersList(ChapterId, ref status);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.lstChapters = lstChapters;

            return View();
        }
        public ActionResult NewsLetterForm(string NEmail = "")
        {
            try
            {
                int status1 = 0;
                if (NEmail != "")
                {
                    Entities.NewsLetter objNewsLetter = new Entities.NewsLetter();
                    objNewsLetter.EmailId = NEmail;
                    Int64 status = 0;
                    status = _NewsLetter.InsertNewsLetter(objNewsLetter);
                    if (status == 1)
                    {
                        Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);
                        int _status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("User News Letter Subscriber", 0, ref _status);
                        if (objTemplates != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]",objAppInfo.BaseUrl);
                            body.Replace("[USERNAME]", "User");
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ApplicationName]", objAppInfo.SiteName);

                            _sentmail.SendMailSendinbrevowithfrom(objNewsLetter.EmailId, objAppInfo.EnqueryEmail, "Thank you for Subscribing your Email Id", body.ToString());
                        }
                        string msg = "<article class=\"newssuccess border2 p3 Roboto-Medium t-b-m10 white-bg\" style=\"color:#fff;border-color: #fff;border: 1px solid #fff;padding: 10px;margin-right: 10px;text-align: center;\">Thank You for Subscribing into TATH, Will get back with updates Shortly. !!</article>";

                        return Json(new { ok = true, data = true, message = msg });
                    }
                    else
                    {
                        string msg = "<article class=\"newserror\">Failed to insert mailid.</article>";
                        return Json(new { ok = false, data = false, message = msg });
                    }
                }
                else
                {
                    string msg = "<article class=\"newserror\">Failed to insert mailid.</article>";
                    return Json(new { ok = false, data = false, message = msg });
                }
            }
            catch
            {
                string msg = "<article class=\"newserror\">Failed to insert mailid.</article>";
                return Json(new { ok = false, data = false, message = msg });
            }
        }

        public ActionResult ThankyouSubscription()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckEmailAvailabilityforNL(string Email)
        {
            try
            {
                int status = 0;
                Entities.NewsLetter objNewsLetter = _NewsLetter.GetNewsLetterByEmail(Email, ref status);
                bool data = (objNewsLetter != null && objNewsLetter.LetterId != 0 ? false : true);
                return Json(new { ok = true, data = data, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error1 closable\">Failed Transaction</div>" });
            }
        }

        #region Contact Us

        public ActionResult ContactUs(Int64 ChapterId = 0, string cname = "")
        {
            try
            {
                // ✅ ASP.NET Core way to READ a cookie
                cname = Request.Cookies["ChapterName"] ?? "";

                // Generate a 16-character random code
                string generatedCode = Guid.NewGuid().ToString("N").Substring(0, 16);

                // ✅ ASP.NET Core way to WRITE a cookie
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps  // ✅ Core way (replaces Request.IsSecureConnection)
                };
                Response.Cookies.Append("FormValidationCode", generatedCode, cookieOptions);

                int status = 0;
                int qstatus = 0;

                if (!string.IsNullOrEmpty(cname))
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = 1;
                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }

                Entities.InnerPages objInnerPageMap = new Entities.InnerPages();
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();
                Int64 qmstatus = 0;
                objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl(cname + "/contact-us", ref qmstatus);

                ViewBag.objMenuPageDetails = objMenuPageDetails;
                ViewBag.objAppInfo = objAppInfo;
                ViewBag.objInnerPageMap = objInnerPageMap;
                ViewBag.ChapterId = ChapterId;
                ViewBag.cname = cname;

                // Pass the code to the view using ViewBag (for hidden field)
                ViewBag.ValidationCode = generatedCode;
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUsBKP(Entities.Enquiries objEnquiry, string cname = "",
    string MCaptcha = "", string recaptchaToken = "", string Honeypot = "", string ValidationCode = "")
        {
            // ✅ ASP.NET Core way to get IP address
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            objEnquiry.Field1 = ip;

            try
            {
                int status1 = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("ContactUs", "Home");
                }

                // ✅ ASP.NET Core way to read form values
                if (!string.IsNullOrEmpty(Request.Form["email2"]))
                {
                    ModelState.AddModelError("", "Spam detected");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction("ContactUs", "Home");
                }

                var capcha = objAppInfo.ShowCapcha;

                // Verify reCAPTCHA token
                bool isHuman = await BLL.Common.VerifyRecaptchaToken(recaptchaToken);

                if (capcha == "No")
                {
                    isHuman = true;
                }

                if (!isHuman)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("ContactUs", "Home");
                }

                // ✅ ASP.NET Core way to READ a cookie
                string cookieValue = Request.Cookies["FormValidationCode"] ?? "";

                if (string.IsNullOrEmpty(cookieValue))
                {
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("ContactUs", "Home");
                }

                // Compare the hidden field value with the cookie value
                if (cookieValue != ValidationCode)
                {
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (isHuman = true)
                {
                    Int64 EnquiryId = 0;
                    Int64 _status = _Enquiries.InsertEnquiries(objEnquiry, ref EnquiryId);

                    int status3 = 0;
                    string EventName = "";
                    if (objEnquiry.EventId != 0)
                    {
                        objEvents = _Events.FEGetEventById(objEnquiry.EventId, EventName, ref lstEventRegistrationTypes, ref status3);
                    }

                    if (_status == 1)
                    {
                        //int status = 0;
                        //Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("User Enquiry", 0, ref status);
                        //if (objTemplates != null)
                        //{
                        //    StringBuilder body = new StringBuilder();
                        //    body.Append(objTemplates.Description);
                        //    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        //    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                        //    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        //    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        //    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        //    body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        //    body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        //    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        //    body.Replace("[SiteName]", objAppInfo.SiteName);
                        //    body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        //    body.Replace("[Name]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                        //    body.Replace("[Email]", objEnquiry.Email);
                        //    body.Replace("[PhoneNo]", objEnquiry.PhoneNo);
                        //    body.Replace("[Comments]", objEnquiry.Description);
                        //    body.Replace("[Subjects]", (!string.IsNullOrEmpty(objEnquiry.Subject)) ? objEnquiry.Subject : "N/A");
                        //    if (objAppInfo.Email == "Yes")
                        //    {
                        //        _sentmail.SendMailSendinbrevowithfrom(objEnquiry.Email, objAppInfo.ContactEmail,
                        //            "Thank You for Contacting us - " + objAppInfo.SiteName, body.ToString());
                        //    }
                        //}

                        //int status2 = 0;
                        //Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Enquiry for Admin", 0, ref status2);
                        //if (objTemplates1 != null)
                        //{
                        //    StringBuilder body = new StringBuilder();
                        //    body.Append(objTemplates1.Description);
                        //    body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        //    body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                        //    body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        //    body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        //    body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        //    body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                        //    body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        //    body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        //    body.Replace("[SiteName]", objAppInfo.SiteName);
                        //    body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        //    body.Replace("[Name]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                        //    body.Replace("[Email]", objEnquiry.Email);
                        //    body.Replace("[PhoneNo]", objEnquiry.PhoneNo);
                        //    body.Replace("[Comments]", objEnquiry.Description);
                        //    body.Replace("[Subjects]", (!string.IsNullOrEmpty(objEnquiry.Subject)) ? objEnquiry.Subject : "N/A");
                        //    if (objAppInfo.Email == "Yes")
                        //    {
                        //        _sentmail.SendMailSendinbrevo(objAppInfo.EnqueryEmail,
                        //            "You have a new Enquiry from - " + objAppInfo.SiteName, body.ToString());
                        //    }
                        //}

                        TempData["message"] = "<div class=\"success closable\">Sent your enquiry successfully</div>";
                        return RedirectToAction("ThankYou", "Home", new { cname = cname });
                    }

                    if (_status == 2)
                    {
                        TempData["message"] = "<div class=\"success closable\">Updated Enquiry Details Successfully</div>";
                        return RedirectToAction("ThankYou", "Home", new { cname = cname });
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "Please correct the errors below and try again.");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error1 closable\">Failed Transaction: {ex.Message}</div>";
                return RedirectToAction("ContactUs", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUs(Entities.Enquiries objEnquiry, string cname = "",
    string MCaptcha = "", string recaptchaResponse = "", string Honeypot = "",
    string ValidationCode = "", string website = "")
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            objEnquiry.Field1 = ip;

            var apiKey = "dzrwbsekljvf";
            var honeypotService = new ProjectHoneypotClient(apiKey);
            Int64 LogId = 0;
            var objLogReport = new Entities.ApplicationLogs
            {
                LogTitle = "ContactUs Form Submission",
                LogDescription = $"New enquiry from IP: {ip}",
                LogDate = DateTime.Now,
                InsertedBy = objEnquiry.Name ?? "Anonymous",
                InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                UpdatedBy = objEnquiry.Email ?? "Anonymous",
                UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()

            };
            _Users.LogReportInsert(objLogReport, ref LogId);
            try
            {
                int status1 = 0;
                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status1);

                // Honeypot validation
                if (!string.IsNullOrWhiteSpace(Honeypot))
                {
                    ModelState.AddModelError("", "Bot activity detected.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (!string.IsNullOrEmpty(website))
                {
                    ModelState.AddModelError("", "Bot submission detected.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (!string.IsNullOrEmpty(Request.Form["email2"]))
                {
                    ModelState.AddModelError("", "Spam detected");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (!BLL.RateLimiter.IsRequestAllowed(ip))
                {
                    ModelState.AddModelError("", "Too many requests detected. Please try again later.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction("ContactUs", "Home");
                }

                var capcha = objAppInfo.ShowCapcha;

                bool isHuman = await BLL.Common.VerifyRecaptchaToken(recaptchaResponse);

                if (capcha == "No" || capcha == "Yes")
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
                    InsertedBy = objEnquiry.Name ?? "Anonymous",
                    InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime(),
                    UpdatedBy = objEnquiry.Name ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()


                };
                _Users.LogSubReportInsert(LogSubReport);
                if (!isHuman)
                {
                    // ── Log Captcha Failure Redirect ──
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Captcha Failed - Redirect",
                        LogDescription = "User redirected back due to captcha failure.",
                        LogDate = DateTime.Now,
                        InsertedBy = objEnquiry.Name ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEnquiry.Name ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    return RedirectToAction("ContactUs", "Home");
                }

                // ✅ ASP.NET Core way to READ cookie
                string cookieValue = Request.Cookies["FormValidationCode"] ?? "";

                if (string.IsNullOrEmpty(cookieValue))
                {
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (cookieValue != ValidationCode)
                {
                    ModelState.AddModelError("", "Security validation failed.");
                    return RedirectToAction("ContactUs", "Home");
                }

                if (ModelState.IsValid)
                {
                    Int64 EnquiryId = 0;
                    Int64 _status = _Enquiries.InsertEnquiries(objEnquiry, ref EnquiryId);
                    _Users.LogSubReportInsert(new Entities.Logdetails
                    {
                        LogId = LogId,
                        LogTitle = "Enquiry Insert",
                        LogDescription = _status == 1
                                           ? $"Enquiry inserted successfully. EnquiryId: {EnquiryId}"
                                           : $"Enquiry insert failed. Status: {_status}",
                        LogDate = DateTime.Now,
                        InsertedBy = objEnquiry.Name ?? "Anonymous",
                        InsertedDate = DateTime.Now,
                        UpdatedBy = objEnquiry.Name ?? "Anonymous",
                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                    });
                    int status3 = 0;
                    string EventName = "";
                    if (objEnquiry.EventId != 0)
                    {
                        objEvents = _Events.FEGetEventById(objEnquiry.EventId, EventName, ref lstEventRegistrationTypes, ref status3);
                    }
                    if (_status == 1)
                    {
                        int status = 0;
                        Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("User Enquiry", 0, ref status);
                        if (objTemplates != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ApplicationName]", objAppInfo.SiteName);
                            body.Replace("[Name]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                            body.Replace("[Email]", objEnquiry.Email);
                            body.Replace("[PhoneNo]", objEnquiry.PhoneNo);
                            body.Replace("[Comments]", objEnquiry.Description);
                            body.Replace("[Subjects]", !string.IsNullOrEmpty(objEnquiry.Subject) ? objEnquiry.Subject : "N/A");
                            if (objAppInfo.Email == "Yes")
                            {
                                if (objAppInfo.IsSports == "Yes")
                                {
                                    var responsce = _sentmail.SendMailSendinbrevowithfromusermuiltiple(objEnquiry.Email, objAppInfo.ContactEmail, "Thank You for Contacting us - " + objAppInfo.SiteName, body.ToString());

                                    // ✅ Check if email was successful or failed
                                    bool isEmailSuccess = !string.IsNullOrEmpty(responsce) && responsce.Contains("messageId");

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = isEmailSuccess ? "User Email Sent" : "User Email Sent Failed",
                                        LogDescription = isEmailSuccess
                                            ? $"Confirmation email sent successfully to user: {objEnquiry.Email}"
                                            : $"Confirmation email FAILED for user: {objEnquiry.Email} | Response: {responsce}",

                                        LogDate = DateTime.Now,
                                        InsertedBy = objEnquiry.Name ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEnquiry.Name ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                        }

                        int status2 = 0;
                        Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Enquiry for Admin", 0, ref status2);
                        if (objTemplates1 != null)
                        {
                            StringBuilder body = new StringBuilder();
                            body.Append(objTemplates1.Description);
                            body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                            body.Replace("[USERNAME]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                            body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                            body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                            body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                            body.Replace("[GUrl]", objAppInfo.GooglePlusUrl);
                            body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                            body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                            body.Replace("[SiteName]", objAppInfo.SiteName);
                            body.Replace("[ApplicationName]", objAppInfo.SiteName);
                            body.Replace("[Name]", BLL.Common.UppercaseFirst(objEnquiry.Name));
                            body.Replace("[Email]", objEnquiry.Email);
                            body.Replace("[PhoneNo]", objEnquiry.PhoneNo);
                            body.Replace("[Comments]", objEnquiry.Description);
                            body.Replace("[Subjects]", !string.IsNullOrEmpty(objEnquiry.Subject) ? objEnquiry.Subject : "N/A");
                            if (objAppInfo.Email == "Yes")
                            {
                                if (objAppInfo.IsSports == "Yes")
                                {
                                    var responsces = _sentmail.SendMailSendinbrevomuiltiplemeail(objAppInfo.ContactEmail, "You have a new Enquiry from - " + objAppInfo.SiteName, body.ToString());

                                    bool isEmailSuccess = !string.IsNullOrEmpty(responsces) && responsces.Contains("messageId");

                                    _Users.LogSubReportInsert(new Entities.Logdetails
                                    {
                                        LogId = LogId,
                                        LogTitle = isEmailSuccess ? "Admin Email Sent" : "Admin Email Sent Failed",
                                        LogDescription = isEmailSuccess
                                            ? $"Confirmation email sent successfully to Admin: {objAppInfo.ContactEmail}"
                                            : $"Confirmation email FAILED for Admin: {objAppInfo.ContactEmail} | Response: {responsces}",

                                        LogDate = DateTime.Now,
                                        InsertedBy = objEnquiry.Name ?? "Anonymous",
                                        InsertedDate = DateTime.Now,
                                        UpdatedBy = objEnquiry.Name ?? "Anonymous",
                                        UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                                    });
                                }
                            }

                            TempData["message"] = "<div class=\"success closable\">Sent your enquiry successfully</div>";
                        }

                        return RedirectToAction("ThankYou", "Home", new { cname = cname });
                    }
                    else if (_status == 2)
                    {
                        TempData["message"] = "<div class=\"success closable\">Updated Enquiry Details Successfully</div>";
                        return RedirectToAction("ThankYou", "Home", new { cname = cname });
                    }
                    else
                    {
                        _Users.LogSubReportInsert(new Entities.Logdetails
                        {
                            LogId = LogId,
                            LogTitle = "Enquiry Insert Failed",
                            LogDescription = "Failed uploading Enquiry Details.",
                            LogDate = DateTime.Now,
                            InsertedBy = objEnquiry.Name ?? "Anonymous",
                            InsertedDate = DateTime.Now,
                            UpdatedBy = objEnquiry.Name ?? "Anonymous",
                            UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                        });
                        TempData["message"] = "<div class=\"error1 closable\">Failed uploading Enquiry Details.</div>";
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "Please correct the errors below and try again.");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (Exception ex)
            {
                // ── Log Exception ──
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = "Exception Occurred",
                    LogDescription = $"Exception in ContactUs: {ex.Message} | StackTrace: {ex.StackTrace}",
                    LogDate = DateTime.Now,
                    InsertedBy = objEnquiry.Name ?? "Anonymous",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = objEnquiry.Name ?? "Anonymous",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                });
                TempData["message"] = $"<div class=\"error1 closable\">Failed Transaction: {ex.Message}</div>";
                return RedirectToAction("ContactUs", "Home");
            }
        }
        private async Task<bool> IsSpamIPAsyncbkp(string ip)
        {
            bool isSpam = false;

            // Call to your IP reputation service
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://api.some-ip-reputation-service.com/check?ip={ip}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Assume the service returns a JSON object with an 'isSpam' flag
                    dynamic result = JsonConvert.DeserializeObject(json);
                    isSpam = result.isSpam; // Example of how to interpret response
                }
            }

            return isSpam;
        }


        public ActionResult Thankyou(string cname = "")
        {
            ViewBag.cname = cname;
            return View();
        }

        public ActionResult PaymentFailed()
        { 
            return View();
        }

        public ActionResult NewsLetter()
        {
            return View();
        }

        #endregion

        public ActionResult RedirectionPage(string PageUrl = "")
        {
            Entities.Redirections objAliasNames = new Entities.Redirections();

            int status1 = 0;

            if (PageUrl != "")
            { 
                objRedirections = _Redirections.GetRedirectionsByurl(PageUrl, ref status1); 
            }
            if (objRedirections.ToUrl == "Account/LogOn")
            {
                return RedirectToAction("LogOn", "Account", new { area = "Admin" });
            }

            else if (objRedirections.ToUrl != null && objRedirections.ToUrl != "")
            {
                return new RedirectResult(objRedirections.ToUrl);
            }
            else
            {
                return RedirectToAction("Error404", "Error");
            }
        }

        public ActionResult LaunchSoonPage()
        {
            return View();
        }
        #region CAPTCHA

        // Install NuGet: SixLabors.ImageSharp
        //public IActionResult ShowCaptchaImage()
        //{
        //    // Generate random code
        //    string code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        //    // Store in session
        //    HttpContext.Session.SetString("CaptchaCode", code);

        //    // Generate image bytes
        //    using var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(120, 40);
        //    // ... draw text on image

        //    using var ms = new MemoryStream();
        //    image.SaveAsPng(ms);

        //    return File(ms.ToArray(), "image/png");
        //}
        [HttpPost]
        public JsonResult GetCaptcha()
        {
            try
            {
                // ✅ ASP.NET Core way to read Session
                string str = HttpContext.Session.GetString("captchastring") ?? "";
                return Json(new { ok = true, data = str, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error closable\">Failed Transaction</div>" });
            }
        }

        #endregion

    }
}
