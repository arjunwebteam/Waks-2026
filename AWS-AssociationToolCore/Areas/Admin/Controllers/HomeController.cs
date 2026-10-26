using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
    {
        BLL.MailTemplates _MailTemplate = new BLL.MailTemplates();
        BLL.Members _Members = new BLL.Members();
        Entities.Enquiries objEnquiries = new Entities.Enquiries();
        Entities.Members objMembers = new Entities.Members();
        BLL.Enquiries _Enquiries = new BLL.Enquiries();
        Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
        BLL.Events _Events = new BLL.Events();
        List<Entities.MailTemplates> lstMailTemplate = new List<Entities.MailTemplates>();
        Entities.Donors objDonors = new Entities.Donors();
        BLL.Donors _Donors = new BLL.Donors(); 
        Entities.Volunteers objVolunteers = new Entities.Volunteers();
        BLL.Volunteers _Volunteers = new BLL.Volunteers();
        Entities.NewsLetter objNewsLetter = new Entities.NewsLetter();
        BLL.NewsLetter _NewsLetter = new BLL.NewsLetter();
        BLL.AppUsers _AppUsers = new BLL.AppUsers();
        BLL.AppInfo _AppInfo = new BLL.AppInfo();
        BLL.SendMail _sentmail = new BLL.SendMail();

        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //public ActionResult Index()
        //{
        //   return View();
        //}
        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //public ActionResult SendMail(Int64 EnquiryId = 0, Int64 MemberId = 0, Int64 DonorId = 0, Int64 EventUserInfoId = 0, Int64 AdvertiseWithUsId = 0, Int64 VolunteerId = 0, Int64 LetterId = 0, Int64 mid = 0)
        //{
        //    try
        //    {
        //        int _qstatus = 0;
        //        string Email = "";

        //        if (EnquiryId != 0)
        //        {
        //            objEnquiries = _Enquiries.GetEnquirysById(EnquiryId, ref _qstatus);
        //            Email = objEnquiries.Email;
        //        }
        //        if (MemberId != 0)
        //        {
        //            objMembers = _Members.GetMemberFullDetailsById(MemberId, ref _qstatus);
        //            Email = objMembers.Email;
        //        }
        //        if (EventUserInfoId != 0)
        //        {
        //            objEventUserInfo = _Events.GetEventUserInfoById(EventUserInfoId, ref _qstatus);
        //            Email = objEventUserInfo.Email;
        //        }
        //        if (DonorId != 0)
        //        {
        //            objDonors = _Donors.GetDonorsById(DonorId, ref _qstatus);
        //            Email = objDonors.Email;
        //        } 
        //        if (VolunteerId != 0)
        //        {
        //            objVolunteers = _Volunteers.GetVolunteerById(VolunteerId, ref _qstatus);
        //            Email = objVolunteers.Email;
        //        }
        //        if (LetterId != 0)
        //        {
        //            objNewsLetter = _NewsLetter.GetNewsLetterById(LetterId);
        //            Email = objNewsLetter.EmailId;
        //        }
        //        ViewBag.Email = Email;
        //        ViewBag.objEnquiries = objEnquiries;
        //        lstMailTemplate = _MailTemplate.GetMailTemplatesList("Manual", ref _qstatus);

        //        if (_qstatus == 1)
        //        {
        //            ViewBag.lstMailTemplate = lstMailTemplate;
        //            ViewBag.status = _qstatus;
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //            return RedirectToAction("Index", "MailTemplates", new { mid = mid });
        //        }
        //    }
        //    catch 
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //    ViewBag.mid = mid;
        //    return View();
        //}

        ////[HttpPost]
        ////[ValidateInput(false)]
        ////public ActionResult Send(Entities.SendMail objSendMail, Int64 mid = 0)
        ////{
        ////    try
        ////    {
        ////        _sentmail.SendMailSendinbrevo(objSendMail.EmailTo, objSendMail.Subject, objSendMail.Description.ToString());
        ////        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Sent mail sucessfully.</div>";
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">"+ex.Message+"</div>";
        ////    }
        ////    return RedirectToAction("SendMail", "Home", new { mid = mid });
        ////}




        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Send(Entities.SendMail objSendMail, Int64 mid = 0)
        //{
        //    try
        //    {
        //        _sentmail.SendMailSendinbrevo(objSendMail.EmailTo, objSendMail.Subject, objSendMail.Description.ToString());
        //        TempData["messageType"] = "success";
        //        TempData["message"] = "Sent mail sucessfully.";
        //    }
        //    catch (Exception EX)
        //    {



        //        TempData["messageType"] = "error";
        //        TempData["message"] = EX.Message;
        //    }
        //    return RedirectToAction("SendMail", "Home", new { mid = mid });
        //}


        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //public ActionResult MailTemplate(string MailTemplateName = "")
        //{
        //    string str = "";
        //    try
        //    {
        //        int _qstatus = 0;
        //        Entities.MailTemplates objMailTemplate = _MailTemplate.GetMailTemplateById(MailTemplateName, 0, ref _qstatus);

        //        if (objMailTemplate != null)
        //        {
        //            return Json(new { ok = true, data = objMailTemplate });
        //        }
        //        else
        //        {
        //            str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
        //            return Json(new { ok = false, data = str });
        //        }
        //    }
        //    catch 
        //    {
        //        str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //        return Json(new { ok = false, data = str });
        //    }
        //}

        #region LogReport
        [Authorize]
        public ActionResult LogReport()
        {
            return View();
        }




        [HttpGet]
        [Authorize]
        public ActionResult LogReportGetList(string StartDate = "", string EndDate = "", string Search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            List<Entities.LogReport> lstLogReport = new List<Entities.LogReport>();

            try
            {
                lstLogReport = _AppInfo.GetLogReportListByVariable(StartDate, EndDate, Search.Trim(), Sort, PageNo, Items, ref Total);

            }
            catch
            {
                Total = -1;
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstLogReport = lstLogReport;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }
        [Authorize]
        public ActionResult SubLogReport(Int64 LogId = 0)
        {
            ViewBag.LogId = LogId;

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SubLogReportGetList(Int64 LogId = 0, string StartDate = "", string EndDate = "", string Search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            List<Entities.LogSubReport> lstSUbLogReport = new List<Entities.LogSubReport>();

            try
            {

                lstSUbLogReport = _AppUsers.SubGetLogReportListByVariable(LogId, StartDate, EndDate, Search.Trim(), Sort, PageNo, Items, ref Total);

            }
            catch
            {
                Total = -1;
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstSUbLogReport = lstSUbLogReport;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }





        #endregion

        #region Dashboard
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Dashboard()
        {
            Int64 MembersActiveCount = 0; decimal MembersActiveSumAmount = 0; Int64 MembersInActiveCount = 0;
            decimal MembersInActiveSumAmount = 0; decimal DonationsWeeklyAmount = 0; decimal DonationsMonthlyAmount = 0; Int64 DonationsWeeklyCount = 0;
            Int64 DonationsMonthlyCount = 0;
            int Items = 10;

            int status = 0;
            Entities.AppInfo objAppInfo = new Entities.AppInfo();
            try
            {
                objAppInfo = _AppInfo.AdminDashboard(ref Items, ref MembersActiveCount, ref MembersActiveSumAmount, ref MembersInActiveCount,
                    ref MembersInActiveSumAmount, ref DonationsWeeklyAmount, ref DonationsMonthlyAmount, ref DonationsWeeklyCount, ref DonationsMonthlyCount, ref status);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error closable\">" + ex.Message + "</div>";
            }
            ViewBag.MembersActiveCount = MembersActiveCount;
            ViewBag.MembersActiveSumAmount = MembersActiveSumAmount;
            ViewBag.MembersInActiveCount = MembersInActiveCount;

            ViewBag.MembersInActiveSumAmount = MembersInActiveSumAmount;
            ViewBag.DonationsWeeklyAmount = DonationsWeeklyAmount;
            ViewBag.DonationsMonthlyAmount = DonationsMonthlyAmount;
            ViewBag.DonationsWeeklyCount = DonationsWeeklyCount;
            ViewBag.DonationsMonthlyCount = DonationsMonthlyCount;

            ViewBag.lstactiveMembers = objAppInfo.lstactiveMembers;
            ViewBag.lstinactiveMembers = objAppInfo.lstinactiveMembers;
            ViewBag.lstServiceDonations = objAppInfo.lstServiceDonations;
            ViewBag.lstEvents = objAppInfo.lstEvents;
            ViewBag.lstTableCounts = objAppInfo.lstTableCounts;
            return View();
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
