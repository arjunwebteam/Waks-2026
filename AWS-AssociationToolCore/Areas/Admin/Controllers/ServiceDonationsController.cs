using AWS_AssociationToolCore.BLL;
//using System.Web.Mvc;
//using System.Web.Helpers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
//using System.Web.Security;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using NPOI.SS.UserModel;       // IWorkbook, ISheet, IRow, ICell, CellType, DateUtil
using NPOI.HSSF.UserModel;     // HSSFWorkbook  (.xls)
using NPOI.XSSF.UserModel;     // XSSFWorkbook  (.xlsx)
using System.Data;
using System.IO;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{

    [Area("Admin")]

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,Treasurer,Helpline,Donations,DeveloperAdmin,")]
    public class ServiceDonationsController : Controller
    {
        AWS_AssociationToolCore.BLL.ServiceDonations _Donor = new AWS_AssociationToolCore.BLL.ServiceDonations();

        AWS_AssociationToolCore.BLL.Services _Services = new AWS_AssociationToolCore.BLL.Services();

        AWS_AssociationToolCore.BLL.AppInfo _appinfo = new AWS_AssociationToolCore.BLL.AppInfo();
        AWS_AssociationToolCore.BLL.MailTemplates _MailTemplates = new AWS_AssociationToolCore.BLL.MailTemplates();

        DAL.ServiceDonations _ServiceDonationsD = new DAL.ServiceDonations();
        BLL.Members _Members = new BLL.Members();
        BLL.DonationCategories _DonationCategories = new BLL.DonationCategories();
        AWS_AssociationToolCore.Entities.ServiceDonations objRequestCalls = new AWS_AssociationToolCore.Entities.ServiceDonations();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        AWS_AssociationToolCore.Entities.Services objServices = new AWS_AssociationToolCore.Entities.Services();
        List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();
        BLL.ServiceDonations _ServiceDonations = new BLL.ServiceDonations();
        BLL.Users _users = new BLL.Users();
        BLL.SendMail _sentmail = new BLL.SendMail();


        [Authorize]
        public IActionResult Index(Int64 ServiceId = 0, string ServiceTitle = "", Int64 ServiceDonationId = 0, Int64 mid = 0, string Search = "", Int64 ChapterId = 0)
        {
            List<Entities.Services> lstServices = new List<Entities.Services>();
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

                lstChapters = _Chapters.GetChaptersList(ref status);
                int _qstatus = 0;
                int _sstatus = 0;
                Entities.Members objMembers = new Entities.Members();

                lstServices = _Services.GetServicesList(ref _sstatus);

                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);

                if (_qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.status = _qstatus;
                }



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
                ViewBag.ServiceId = ServiceId;
                ViewBag.ServiceTitle = ServiceTitle;
                ViewBag.lstServices = lstServices;
                ViewBag.Search = Search;


            }
            catch
            {
                status = -1;
            }

            // ✅ ViewBag assignments
            ViewBag.ChapterId = chapterId;


            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ServiceDonationsList(Int64 ServiceId = 0, Int64 ServiceDonationId = 0, Int64 PaymentMethod = 0, string StartDate = "", string EndDate = "", Int64 PaymentStatus = 0, string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0, Int64 ChapterId = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            decimal TotalDonationAmount = 0;
            Int64 noofdonors = 0;
            decimal SumofDonation = 0;
            List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();
            try
            {
                lstServiceDonations = _Donor.GetServiceDonationsListByVariableId(ServiceId, ServiceDonationId, PaymentMethod, PaymentStatus, StartDate, EndDate, ref SumofDonation, ref noofdonors, HttpUtility.UrlDecode(Search.Trim()), Sort, PageNo, Items, ChapterId, ref Total);


                //List<Entities.ServiceDonations> lstdonations = lstServiceDonations.FindAll(x => x.PaymentStatus == "Completed");

                //List<string> foo = new List<string>();

                //if (lstdonations != null && lstdonations.Count != 0)
                //{
                //    foreach(var item in lstdonations)
                //    {
                //        //decimal amount = Convert.ToDecimal(item.TotalAmount);
                //        foo.Add(item.Amount.ToString());
                //    }
                //}
                //TotalDonationAmount = foo.Sum(x => Convert.ToDecimal(x));

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            ViewBag.Total = Total;
            ViewBag.items = Items;
            ViewBag.PageNo = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.ServiceId = ServiceId;
            ViewBag.lstServiceDonations = lstServiceDonations;
            ViewBag.total = Total;
            ViewBag.SumofDonation = SumofDonation;
            ViewBag.noofdonors = noofdonors;

            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            ViewBag.ChapterId = ChapterId;

            return View();
        }

        [Authorize]
        public JsonResult DeleteServiceDonation(Int64 ServiceDonationId)
        {
            string str = "";
            try
            {
                Int64 _status = _Donor.DeleteDonor(ServiceDonationId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Deleting the Record" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        public ActionResult ViewServiceDonation(Int64 ServiceDonationId, Int64 mid = 0)
        {
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            int status = 0;
            try
            {
                objServiceDonations = _Donor.GetServiceDonationsById(ServiceDonationId, ref status);

                if (objServiceDonations == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid });
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ServiceDonations", new { mid = mid });
            }

            ViewBag.objServiceDonations = objServiceDonations;
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult EdiServiceDonation(Int64 ServiceDonationId, Int64 ServiceId = 0, Int64 mid = 0, Int64 ChapterId = 0)
        {
            int Total = 0;
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            Entities.Members objMembers = new Entities.Members();
            List<Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            List<Entities.Services> lstServices = new List<Entities.Services>();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            lstChapters = _Chapters.GetChaptersList(ref Total);

            int status = 0;

            int Donationstatus = 0;
            try
            {
                int _qstatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                objServiceDonations = _Donor.GetServiceDonationsById(ServiceDonationId, ref status);
                lstDonationCategories = _DonationCategories.FEDonationCategoriesGetList(ref Donationstatus);
                lstDonationCategories = _DonationCategories.FEDonationCategoriesGetList(ref Donationstatus);
                lstServices = _Services.GetServicesList(ref Donationstatus);




                if (objServiceDonations == null && objMembers == null)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid });
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "ServiceDonations", new { mid = mid });
            }
            ViewBag.lstDonationCategories = lstDonationCategories;
            ViewBag.lstServices = lstServices;
            ViewBag.lstChapters = lstChapters;
            ViewBag.objServiceDonations = objServiceDonations;
            ViewBag.objMembers = objMembers;
            ViewBag.ServiceId = ServiceId;
            ViewBag.ServiceDonationId = ServiceDonationId;
            ViewBag.mid = mid;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddServiceDonation(Entities.ServiceDonations objServiceDonations, List<Entities.ServiceContributions> lstServiceContributions, Int64 ServiceId, Int64 mid = 0)
        {
            Entities.ServiceDonations objDonations = new Entities.ServiceDonations();

            try
            {
                Int64 ServiceDonationId = 0;
                objServiceDonations.ServiceId = ServiceId;
                objServiceDonations.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objServiceDonations.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                Int64 _status = _Donor.InsertServiceDonationsByAdmin(objServiceDonations, ref ServiceDonationId);
                int status = 0;


                if (_status == 1)
                {
                    int _qstatus = 0;
                    string contributions = "";


                    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);
                    objDonations = _Donor.GetServiceDonationsById(ServiceDonationId, ref status);

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

                    int _status1 = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status1);
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
                        if (objServiceDonations.DonationCauses == "General Donation")
                        {
                            body.Replace("[DonationCause]", "");
                        }
                        else
                        {
                            body.Replace("[DonationCause]", "<span style=\"color:#0490bf;\">" + objServiceDonations.ServiceTitle + "</span>");
                        }

                        body.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                        body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                        body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                        body.Replace("[PaymentType]", "Paypal");
                        body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfo.SupportEmail);
                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body.Replace("[Contributions]", contributions);

                        _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.SecretaryPhone, objTemplates.Subject, body.ToString());
                    }

                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid, ServiceId = ServiceId });
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Changes has been Updated Successfully";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid, ServiceId = ServiceId });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Data Error";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid, ServiceId = ServiceId });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "ServiceDonations", new { mid = mid, ServiceId = ServiceId });

            }

            return RedirectToAction("Index", "ServiceDonations", new { mid = mid, ServiceId = ServiceId });
        }


        [Authorize]
        [HttpPost]
        public ActionResult GetServiceId(Int64 id)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.Services objServices = _Services.GetServicesById(id, ref status);
                if (objServices != null)
                {
                    return Json(new { ok = true, data = objServices });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddServiceDonationFE(Entities.ServiceDonations objServiceDonations, List<Entities.ServiceContributions> lstServiceContributions, string cname = "", Int64 ServiceDonationId = 0, string MCaptcha = "", Int64 mid = 0)
        {

            try
            {
                List<Entities.ServiceContributions> lstcontribution = new List<Entities.ServiceContributions>();

                decimal Percentage = 0;
                decimal IndAmount = objServiceDonations.Amount;
                decimal TotalAmount = objServiceDonations.Amount;
                objServiceDonations.UpdatedBy = objServiceDonations.FirstName;
                if (objServiceDonations.DonationCause != "General Donation")
                {
                    Entities.ServiceContributions controbution = new Entities.ServiceContributions();

                    controbution.ServiceId = objServiceDonations.ServiceId;
                    controbution.ServiceTitle = objServiceDonations.DonationCause;


                    lstcontribution.Add(controbution);
                }

                var receipturl = "NA";

                if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                {

                    Int64 CCount = lstServiceContributions.Count(x => x.ServiceId != 0);
                    if (CCount != 0)
                    {
                        objServiceDonations.Amount = 0;
                    }
                }
                else
                {
                    Int64 CCount = lstcontribution.Count(x => x.ServiceId != 0);
                    if (CCount != 0)
                    {
                        objServiceDonations.Amount = 0;
                    }
                }
                Int64 status1 = _ServiceDonations.InsertServiceDonations(objServiceDonations, ref ServiceDonationId, ref receipturl);

                if (objServiceDonations.DonationCause != "General Donation")
                {
                    Entities.ServiceContributions contribution = new Entities.ServiceContributions();

                    contribution.ServiceId = objServiceDonations.ServiceId;
                    contribution.ServiceTitle = objServiceDonations.DonationCause;
                    contribution.ServiceDonationId = ServiceDonationId;
                    contribution.Percentage = objServiceDonations.Amount.ToString();

                    lstcontribution.Add(contribution);
                }

                if (ServiceDonationId != 0)
                {
                    int status = 0;

                    if (lstServiceContributions != null && lstServiceContributions.Count() != 0)
                    {
                        Int64 CCount1 = lstServiceContributions.Count(x => x.ServiceId != 0);
                        Int64 gencount = lstServiceContributions.Count(x => x.ServiceId == 10);
                        decimal contriAmount = 0;
                        decimal percent = 0;
                        if (CCount1 != 0)
                        {

                            if (gencount != 0)
                            {
                                var contributions = lstServiceContributions.FindAll(x => x.ServiceId != 0 && x.ServiceId != 10);

                                foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                {
                                    IndAmount = (TotalAmount / 100) * Convert.ToDecimal(objServiceContributions.Percentage);
                                    objServiceContributions.CAmount = IndAmount;
                                    objServiceContributions.ServiceDonationId = ServiceDonationId;
                                    contriAmount = contriAmount + IndAmount;
                                    percent = percent + Convert.ToDecimal(objServiceContributions.Percentage);
                                    Int64 estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);



                                    if (estatus == -1)
                                    {

                                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                        return RedirectToAction("Acknowledgement", "Services");
                                    }
                                }

                                Entities.ServiceContributions objGenContributions = new Entities.ServiceContributions();
                                IndAmount = TotalAmount - contriAmount;
                                objGenContributions.Percentage = (100 - percent).ToString();
                                objGenContributions.CAmount = IndAmount;
                                objGenContributions.ServiceDonationId = ServiceDonationId;
                                objGenContributions.ServiceId = 10;
                                Int64 estatus1 = _ServiceDonations.InsertServiceContributions(objGenContributions);

                            }
                            else
                            {
                                int z = 1;
                                var contributions = lstServiceContributions.FindAll(x => x.ServiceId != 0);

                                foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                {
                                    Int64 estatus = 0;
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

                                    }

                                    if (estatus == -1)
                                    {
                                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                        return RedirectToAction("Acknowledgement", "Services");
                                    }

                                    z++;
                                }
                            }
                        }
                    }
                    else
                    {
                        Int64 CCount1 = lstcontribution.Count(x => x.ServiceId != 0);
                        Int64 gencount = lstcontribution.Count(x => x.ServiceId == 10);
                        decimal contriAmount = 0;
                        decimal percent = 0;
                        if (CCount1 != 0)
                        {

                            if (gencount != 0)
                            {
                                var contributions = lstcontribution.FindAll(x => x.ServiceId != 0 && x.ServiceId != 10);

                                foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                {
                                    IndAmount = (TotalAmount / 100) * Convert.ToDecimal(objServiceContributions.Percentage);
                                    objServiceContributions.CAmount = IndAmount;
                                    objServiceContributions.ServiceDonationId = ServiceDonationId;
                                    contriAmount = contriAmount + IndAmount;
                                    percent = percent + Convert.ToDecimal(objServiceContributions.Percentage);
                                    Int64 estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);



                                    if (estatus == -1)
                                    {

                                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                        return RedirectToAction("Acknowledgement", "Services");
                                    }
                                }

                                Entities.ServiceContributions objGenContributions = new Entities.ServiceContributions();
                                IndAmount = TotalAmount - contriAmount;
                                objGenContributions.Percentage = (100 - percent).ToString();
                                objGenContributions.CAmount = IndAmount;
                                objGenContributions.ServiceDonationId = ServiceDonationId;
                                objGenContributions.ServiceId = 10;
                                Int64 estatus1 = _ServiceDonations.InsertServiceContributions(objGenContributions);

                            }
                            else
                            {
                                if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                                {
                                    int z = 1;
                                    var contributions = lstServiceContributions.FindAll(x => x.ServiceId != 0);

                                    foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                    {
                                        Int64 estatus = 0;
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

                                        }

                                        if (estatus == -1)
                                        {
                                            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                            return RedirectToAction("Acknowledgement", "Services");
                                        }

                                        z++;
                                    }
                                }
                                else
                                {
                                    int z = 1;
                                    var contributions = lstcontribution.FindAll(x => x.ServiceId != 0);

                                    foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                    {
                                        Int64 estatus = 0;
                                        if (z != lstcontribution.Count(x => x.ServiceId != 0))
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

                                        }

                                        if (estatus == -1)
                                        {
                                            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                            return RedirectToAction("Acknowledgement", "Services");
                                        }

                                        z++;
                                    }
                                }
                            }
                        }
                    }

                    int status2 = 0;
                    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status2);

                    int _qstatus = 0;
                    string contributions1 = "";

                    lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);



                    if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                    {
                        contributions1 = "<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><tr><th width=\"10%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">S.No</span></th><th width=\"48%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\">Donation Cause</span></th><th width=\"23%\" align=\"center\" bgcolor=\"#f1f1f1\" scope=\"col\"><span style=\"font-size: 13px\"> Amount </span></th></tr>";

                        int i = 1;
                        foreach (var item in lstServiceContributions)
                        {
                            contributions1 = contributions1 + "<tr><td align=\"center\">" + i + "</td><td align=\"center\">" + item.ServiceTitle + "</td><td align=\"center\">$" + Math.Round(item.Amount, 2) + "</td></tr>";
                            i++;
                        }
                        contributions1 = contributions1 + "</table>";
                    }

                    int _status = 0;
                    Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref _status);
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("Thank you for giving service donation", 0, ref _status);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objappinfo.BaseUrl);
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

                        body.Replace("[DonationAmount]", TotalAmount.ToString());
                        body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                        body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                        body.Replace("[PaymentType]", "Paypal");
                        body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfo.SupportEmail);
                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body.Replace("[Contributions]", contributions1);

                        _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.SecretaryPhone, objTemplates.Subject, body.ToString());


                    }
                    Entities.MailTemplates objTemplates1 = _MailTemplates.GetMailTemplateById("Service Donate for Admin", 0, ref _status);
                    if (objTemplates1 != null)
                    {
                        StringBuilder body1 = new StringBuilder();
                        body1.Append(objTemplates1.Description);
                        body1.Replace("[usersiteurl]", objappinfo.BaseUrl);
                        body1.Replace("[UserName]", BLL.Common.UppercaseFirst(objServiceDonations.FirstName + " " + objServiceDonations.LastName));
                        body1.Replace("[FirstName]", objServiceDonations.FirstName);
                        body1.Replace("[LastName]", objServiceDonations.LastName);
                        body1.Replace("[Email]", objServiceDonations.Email);
                        body1.Replace("[PhoneNo]", objServiceDonations.PhoneNo);
                        body1.Replace("[Address]", (objServiceDonations.Address == "" ? "N/A" : objServiceDonations.Address));
                        if (objServiceDonations.DonationCause == "General Donation")
                        {
                            body1.Replace("[DonationCause]", "");
                        }
                        else
                        {
                            body1.Replace("[DonationCause]", "Cause : <span style=\"color:#0490bf;\">" + objServiceDonations.DonationCause + "</span>");
                        }

                        body1.Replace("[DonationProgram]", objServiceDonations.DonationCause);
                        body1.Replace("[DonationAmount]", TotalAmount.ToString());
                        body1.Replace("[TransactionId]", objServiceDonations.TransactionId);
                        body1.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                        body1.Replace("[PaymentType]", "Paypal");
                        body1.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body1.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body1.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body1.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body1.Replace("[GUrl]", objAppInfo.SupportEmail);
                        body1.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body1.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body1.Replace("[SiteName]", objAppInfo.SiteName);
                        body1.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body1.Replace("[Contributions]", contributions1);

                        _sentmail.SendMailSendinbrevowithfrom(objAppInfo.SecretaryPhone, objAppInfo.SecretaryPhone, objTemplates1.Subject, body1.ToString());
                    }

                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">added Donor details Successfully..!!</div>";
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ServiceDonations");
            }

            return RedirectToAction("Index", "ServiceDonations");
        }

        [Authorize]
        public ActionResult AddServiceDonation(Int64 ServiceId = 0, Int64 mid = 0, Int64 ChapterId = 0)
        {
            Entities.Members objMembers = new Entities.Members();

            int status = 0;
            List<Entities.Services> lstServices = new List<Entities.Services>();
            try
            {
                int _qstatus = 0;


                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                objServices = _Services.FEGetServicesDataById(ServiceId, ref lstServiceImages, ref status);
                lstServices = _Services.GetServicesList(ref status);

                ViewBag.lstServices = lstServices;
                if (_qstatus == 1)
                {
                    ViewBag.objMembers = objMembers;
                    ViewBag.status = _qstatus;
                }

                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.ServiceId = ServiceId;
            ViewBag.objServices = objServices;
            ViewBag.mid = mid;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        //[Models.SessionClass.SessionExpireFilter]

        //public ActionResult AddServiceDonation(Int64 DonationCategoryId = 0)
        //{

        //    int Total = 0;
        //    Entities.Members objMembers = new Entities.Members();
        //    List<Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
        //    List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        //    lstChapters = _Chapters.GetChaptersList(ref Total);

        //    int Donationstatus = 0;
        //    try
        //    {
        //        int _qstatus = 0;

        //        lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref Donationstatus);
        //        //List<Entities.DonationCategories> lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref _qstatus);
        //        if (_qstatus == 1)
        //        {
        //            ViewBag.lstDonationCategories = lstDonationCategories;
        //            ViewBag.status = _qstatus;
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //            return RedirectToAction("Index", "ServiceDonations");
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //    ViewBag.DonationCategoryId = DonationCategoryId;
        //    ViewBag.objMembers = objMembers;
        //    ViewBag.lstChapters = lstChapters;
        //    return View();
        //}

        [HttpPost]
        [Authorize]
        public JsonResult ServiceDonationsStatus(Int64 ServiceDonationId)
        {
            string str = "";
            try
            {
                Int64 _status = _Donor.UpdateServiceDonationsStatus(ServiceDonationId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "status Updated successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating user status" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }


        #region export to excel
        public IActionResult DonorRegistrationsExportToExcel(long ServiceId = 0,long PaymentStatusId = 0, long PaymentMethodId = 0,string StartDate = "",string EndDate = "",string Search = "", string SortColumn = "UpdatedDate",string SortOrder = "DESC", long chapterId = 0)
        {
            try
            {
                string sort = !string.IsNullOrEmpty(SortColumn) ? $"{SortColumn} {SortOrder}" : "";

                DataTable dtUni = _ServiceDonationsD.ServiceDonationsRegistrationsExportToExcel(
                    ServiceId, PaymentStatusId, PaymentMethodId, StartDate, EndDate, Search, sort, chapterId);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Donor-Registration-Export");

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
                    string fileName = $"Donor-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        private DataTable ReadExcelToDataTable(string filePath, string extension)
        {
            DataTable dt = new DataTable();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = extension == ".xls"
                    ? (IWorkbook)new HSSFWorkbook(fs)
                    : new XSSFWorkbook(fs);

                ISheet sheet = workbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int colCount = headerRow.LastCellNum;

                for (int i = 0; i < colCount; i++)
                {
                    string colName = headerRow.GetCell(i)?.ToString() ?? $"Col{i}";
                    dt.Columns.Add(colName);
                }

                for (int r = 1; r <= sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    if (row == null) continue;

                    // ✅ Skip fully empty rows
                    bool rowHasAnyValue = false;
                    for (int c = 0; c < colCount; c++)
                    {
                        var checkCell = row.GetCell(c);
                        if (checkCell != null &&
                            checkCell.CellType != CellType.Blank &&
                            !string.IsNullOrWhiteSpace(checkCell.ToString()))
                        {
                            rowHasAnyValue = true;
                            break;
                        }
                    }
                    if (!rowHasAnyValue) continue;

                    DataRow dr = dt.NewRow();

                    for (int c = 0; c < colCount; c++)
                    {
                        try
                        {
                            var cell = row.GetCell(c);
                            if (cell == null || cell.CellType == CellType.Blank)
                            {
                                dr[c] = DBNull.Value;
                                continue;
                            }

                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                // ✅ Date cell — format as ISO string SQL can parse
                                DateTime dateVal = DateUtil.GetJavaDate(cell.NumericCellValue);
                                dr[c] = dateVal.ToString("yyyy-MM-ddTHH:mm:ss");
                            }
                            else if (cell.CellType == CellType.Numeric)
                            {
                                // ✅ Regular number
                                dr[c] = cell.NumericCellValue.ToString();
                            }
                            else if (cell.CellType == CellType.String)
                            {
                                string val = cell.StringCellValue.Trim();
                                if (string.IsNullOrWhiteSpace(val))
                                {
                                    dr[c] = DBNull.Value;
                                }
                                else
                                {
                                    // ✅ Try parse string dates like "03/26/2025"
                                    dr[c] = DateTime.TryParse(val, out DateTime parsed)
                                            && dt.Columns[c].ColumnName.ToLower().Contains("date")
                                        ? parsed.ToString("yyyy-MM-ddTHH:mm:ss")
                                        : val;
                                }
                            }
                            else if (cell.CellType == CellType.Boolean)
                            {
                                dr[c] = cell.BooleanCellValue.ToString();
                            }
                            else
                            {
                                dr[c] = cell.ToString()?.Trim() ?? DBNull.Value.ToString();
                            }
                        }
                        catch
                        {
                            dr[c] = DBNull.Value;
                        }
                    }

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }


        [HttpPost]
        public async Task<IActionResult> ServiceDonationsBulkUpload(IFormFile Document)
        {
            string summary = "";
            try
            {
                if (Document == null || Document.Length == 0)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Please select a file to upload.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }

                string extension = Path.GetExtension(Document.FileName).ToLower();
                if (extension != ".xls" && extension != ".xlsx")
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Only .xls and .xlsx files are allowed.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }

                // Save file to temp folder
                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "temp");
                Directory.CreateDirectory(tempFolder);
                string filePath = Path.Combine(tempFolder,
                    DateTime.UtcNow.ToString("MM-dd-yyyy-HHmmss") + "-" + Document.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Document.CopyToAsync(stream);
                }

                DataTable dtExcelData = ReadExcelToDataTable(filePath, extension);

                // Delete temp file immediately after reading
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();

                if (dtExcelData != null && dtExcelData.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtExcelData.Rows)
                    {
                        // ✅ Skip rows with no Email or FirstName
                        string email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString().Trim();
                        string firstName = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString().Trim();

                        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName))
                            continue;

                        Entities.ServiceDonations obj = new Entities.ServiceDonations();

                        obj.FirstName = firstName;
                        obj.LastName = dr["LastName"] == DBNull.Value ? "" : dr["LastName"].ToString().Trim();
                        obj.Email = email;
                        obj.PhoneNo = dr["PhoneNo"] == DBNull.Value ? "" : dr["PhoneNo"].ToString().Trim();
                        obj.Country = dr["Country"] == DBNull.Value ? "" : dr["Country"].ToString().Trim();
                        obj.ZipCode = dr["ZipCode"] == DBNull.Value ? "" : dr["ZipCode"].ToString().Trim();
                        obj.Address = dr["Address"] == DBNull.Value ? "" : dr["Address"].ToString().Trim();
                        obj.DonationCause = dr["ServiceTitle"] == DBNull.Value ? "" : dr["ServiceTitle"].ToString().Trim();
                        obj.TransactionId = dr["TransactionId"] == DBNull.Value ? "" : dr["TransactionId"].ToString().Trim();
                        obj.PaymentStatus = dr["PaymentStatus"] == DBNull.Value ? "" : dr["PaymentStatus"].ToString().Trim();
                        obj.PaymentMethod = dr["PaymentMethod"] == DBNull.Value ? "" : dr["PaymentMethod"].ToString().Trim();
                        obj.IsActive = true;

                        // ✅ Safe Amount parse
                        string amountStr = dr["Amount"] == DBNull.Value ? "0" : dr["Amount"].ToString().Trim();
                        obj.Amount = decimal.TryParse(amountStr, out decimal parsedAmount) ? parsedAmount : 0;

                        // ✅ Safe OrderDate parse
                        string dateStr = dr["OrderDate"] == DBNull.Value ? "" : dr["OrderDate"].ToString().Trim();
                        obj.OrderDate = DateTime.TryParse(dateStr, out DateTime parsedDate)
                            ? parsedDate
                            : DateTime.MinValue;

                        lstServiceDonations.Add(obj);
                    }
                }

                if (lstServiceDonations.Count == 0)
                {
                    TempData["message"] = "<div class=\"alert alert-warning alert-dismissable\">No valid records found in the uploaded file.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }

                string shipmentsXML = BLL.Common.CreateXMLForObject(lstServiceDonations);
                Int64 status = _Donor.ServiceDonationsBulkUpload(shipmentsXML, ref summary);
                if (status != -1)
                {
                    TempData["messageType"] = "success"; // Assuming "info" for update
                    TempData["message"] = "Uploading Successfully Completed";
                    return RedirectToAction("Index", "ServiceDonations");
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Uploading";
                    return RedirectToAction("Index", "ServiceDonations");
                }
             
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
            
               
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">Error: {ex.Message}</div>";
            }

            return RedirectToAction("Index", "ServiceDonations");
        }
        [Authorize]
        public FileStreamResult CreateFile(string str = "", string type = "")
        {
            //todo: add some data from your database into that string:
            var string_with_your_data = str;
            string_with_your_data = str.Replace("\\\\r\\\\n", Environment.NewLine);
            var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain", "Donors-Bulk-Upload-output.txt");
        }

        [Authorize]
        public ActionResult ServiceDonors(Int64 ServiceId = 0, string ServiceTitle = "", Int64 mid = 0, Int64 ChapterId = 0)
        {
            List<Entities.Services> lstServices = new List<Entities.Services>();

            int status = 0;
            // string RoleName = "Donations";
            long userId = 0;

            var userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                long.TryParse(userIdStr, out userId);
            }
            Entities.UserRoles objuserroles = new Entities.UserRoles();

            long chapterId = 0;

            var chapterValue = HttpContext.Session.GetString("chapterid");

            if (!string.IsNullOrEmpty(chapterValue))
            {
                long.TryParse(chapterValue, out chapterId);
            }




            int _qstatus = 0;
            int _sstatus = 0;
            Entities.Members objMembers = new Entities.Members();

            lstServices = _Services.GetServicesList(ref _sstatus);

            objMembers = _Members.AddMembershipRequirement(ref _qstatus);
            objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
            if (_qstatus == 1)
            {
                ViewBag.objMembers = objMembers;
                ViewBag.status = _qstatus;
            }

            else
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "ServiceDonations");
            }
            ViewBag.ChapterId = chapterId;
            ViewBag.ServiceId = ServiceId;
            ViewBag.ServiceTitle = ServiceTitle;
            ViewBag.lstServices = lstServices;
            HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

            ViewBag.IsEdit = HttpContext.Session.GetString("IsEdit");
            ViewBag.IsView = HttpContext.Session.GetString("IsView");
            ViewBag.IsDelete = HttpContext.Session.GetString("IsDelete");
            ViewBag.IsExport = HttpContext.Session.GetString("IsExport");
            ViewBag.IsAdd = HttpContext.Session.GetString("IsAdd");
            ViewBag.mid = mid;

            return View();
        }


        [Authorize]
        public ActionResult ServiceDonarsList(Int64 ServiceId = 0, Int64 PaymentMethod = 0, Int64 PaymentStatus = 0, string search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int PageItems = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0, Int64 ChapterId = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.ServiceDonations> lstServiceDonors = new List<Entities.ServiceDonations>();
            try
            {
                lstServiceDonors = _Donor.GetServiceDonarsListByVariable(ServiceId, PaymentMethod, PaymentStatus, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ChapterId, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            ViewBag.Total = Total;
            ViewBag.items = PageItems;
            ViewBag.PageNo = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.ServiceId = ServiceId;
            ViewBag.lstServiceDonors = lstServiceDonors;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }


        #region export to excel

        [Authorize]
        public IActionResult ScholorshipRegistrationsExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.ScholorshipRegistrationsExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Scholorship-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Scholorship-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // change if needed
            }
        }
        [Authorize]
        public IActionResult FivekRegistrationExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.FivekRegistrationExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "5kRegistration-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"5kRegistration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // adjust if needed
            }
        }
        [Authorize]
        public IActionResult KidsRegistrationsExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.KidsRegistrationsExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Balala-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Balala-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // change if needed
            }
        }
        [Authorize]
        public IActionResult MembershipregistrationExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.MembershipChildrenRegistrationExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Children-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Children-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // update if needed
            }
        }

        [Authorize]
        public IActionResult PaadutateeyagaRegistrationExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.PaadutateeyagaRegistrationExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Paadutateeyaga-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Paadutateeyaga-Registration-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // adjust if needed
            }
        }

        [Authorize]
        public IActionResult SambararaluRegistrationExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.SambaraluRegistrationExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Sambaralu-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Sambaralu-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // adjust if needed
            }
        }

        [Authorize]
        public IActionResult SwarabhishekamRegistrationExportToExcel()
        {
            try
            {
                DataTable dtUni = _ServiceDonationsD.SwarabhishekamRegistrationExportToExcel();

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "Swarabhishekam-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Swarabhishekam-Registration-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class='alert alert-danger'>{ex.Message}</div>";
                return RedirectToAction("Index"); // adjust if needed
            }
        }
        #endregion

        #region ServiceDonation_ManualMailing

        [Authorize]
        public ActionResult ServiceDonationMailling(Int64 ServiceDonationId = 0, Int64 mid = 0)
        {
            int status = 0;
            int result1 = 0;
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            List<Entities.ServiceContributions> lstServiceContributions = new List<Entities.ServiceContributions>();
            try
            {
                int _qstatus = 0;
                string contributions = "";


                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);
                objServiceDonations = _Donor.GetServiceDonationsById(ServiceDonationId, ref status);

                lstServiceContributions = _ServiceDonations.GetContributionsById(ServiceDonationId, ref _qstatus);

                if (status == 1)
                {
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
                        StringBuilder body = new StringBuilder(objTemplates.Description);
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

                        body.Replace("[DonationAmount]", objServiceDonations.Amount.ToString());
                        body.Replace("[TransactionId]", objServiceDonations.TransactionId);
                        body.Replace("[PaymentStatus]", objServiceDonations.PaymentStatus);
                        body.Replace("[PaymentType]", "Paypal");
                        body.Replace("[PaymentDate]", objServiceDonations.OrderDate.ToString("MM/dd/yyyy"));
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfo.SupportEmail);
                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body.Replace("[Contributions]", contributions);

                        _sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.DonationEmail, objTemplates.Subject, body.ToString());


                        //_sentmail.SendMailSendinbrevowithfrom(objServiceDonations.Email, objAppInfo.SecretaryPhone, objTemplates.Subject, body.ToString());

                    }
                    TempData["messageType"] = "success";
                    TempData["message"] = "Sent mail sucessfully";
                    return RedirectToAction("Index", "ServiceDonations", new { mid = mid});

                }
                else
                {
                    TempData["messageType"] = "Failed";
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";

            }
            return RedirectToAction("Index", "ServiceDonations", new { mid = mid });

        }

        [Authorize]
        [HttpPost]
        public JsonResult ServicesGetListByChapterId()
        {

            long chapterId = long.TryParse(HttpContext.Session.GetString("ChapterId"), out var cid) ? cid : 0;

            List<Entities.Services> lstServices = new List<Entities.Services>();

            int status = 0;
            try
            {
                lstServices = _Services.FEServicesGetListByChapterId(chapterId, ref status);
                bool data = (lstServices != null && lstServices.Count != 0 ? false : true);

                return Json(new { ok = true, data = lstServices, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>" });
            }
        }

        #endregion

        #region ServiceDonationAdd

        [Authorize]
        public IActionResult AddDonation(
    long ServiceCategoryId = 0,
    long MemberId = 0,
    long ChapterId = 0,
    string Type = "",
    long ServiceId = 0,
    string cname = "",
    long ServiceDonationId = 0)
        {
            var objChapters = new Entities.Chapters();
            var lstServices = new List<Entities.Services>();
            var _objServices = new Entities.Services();
            var objMembers = new Entities.Members();
            var _objMember = new Entities.Members();

            try
            {
                int status = 0;
                int qstatus = 0;
                int _qstatus = 0;

                // Get chapter by name
                if (!string.IsNullOrWhiteSpace(cname))
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters?.ChapterId ?? 0;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }

                // Get services list
                lstServices = _Services.FEServicesGetListByChapterId(ChapterId, ref status);

                // Store ChapterId in session
                HttpContext.Session.SetInt32("ChapterId", (int)ChapterId);

                // Get service details
                _objServices = _Services.FEGetServicesDataById(ServiceId, ref lstServiceImages, ref status);

                // Get logged-in member
                if (User?.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    _objMember = _Members.GetMemberFullDetailsByEmail(User.Identity.Name, ref _qstatus);
                }

                // Store MemberId safely
                if (_objMember != null)
                {
                    HttpContext.Session.SetInt32("MemberId", (int)_objMember.MemberId);
                }

                // Membership requirement
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }

            // ViewBag assignments
            ViewBag.Memberdetails = _objMember;
            ViewBag.ServiceCategoryId = ServiceCategoryId;
            ViewBag.ChapterId = ChapterId;
            ViewBag.Type = Type;
            ViewBag.ServiceId = ServiceId;
            ViewBag.MemberId = _objMember?.MemberId ?? 0;
            ViewBag.objServices = _objServices;
            ViewBag.lstServiceImages = lstServiceImages;
            ViewBag.lstServices = lstServices;
            ViewBag.cname = cname;
            ViewBag.ServiceDonationId = ServiceDonationId;
            ViewBag.objMembers = objMembers;

            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult InsertDonation(Entities.ServiceDonations objServiceDonations, List<Entities.ServiceContributions> lstServiceContributions, string cname = "", Int64 ServiceDonationId = 0)
        {
            try
            {
                decimal Percentage = 0;
                decimal IndAmount = objServiceDonations.Amount;
                decimal TotalAmount = objServiceDonations.Amount;
                objServiceDonations.UpdatedBy = objServiceDonations.FirstName;

                if (lstServiceContributions != null && lstServiceContributions.Count != 0)
                {
                    Int64 CCount = lstServiceContributions.Count(x => x.ServiceId != 0);
                    if (CCount != 0)
                    {
                        objServiceDonations.Amount = 0;
                    }
                }
                string receipturl = "NA";
                Int64 status1 = _ServiceDonations.InsertServiceDonations(objServiceDonations, ref ServiceDonationId, ref receipturl);
                if (ServiceDonationId != 0)
                {
                    int status = 0;

                    if (lstServiceContributions != null && lstServiceContributions.Count() != 0)
                    {
                        Int64 CCount1 = lstServiceContributions.Count(x => x.ServiceId != 0);
                        Int64 gencount = lstServiceContributions.Count(x => x.ServiceId == 10);
                        decimal contriAmount = 0;
                        decimal percent = 0;
                        if (CCount1 != 0)
                        {
                            if (gencount != 0)
                            {
                                var contributions = lstServiceContributions.FindAll(x => x.ServiceId != 0 && x.ServiceId != 10);

                                foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                {
                                    IndAmount = (TotalAmount / 100) * Convert.ToDecimal(objServiceContributions.Percentage);
                                    objServiceContributions.CAmount = IndAmount;
                                    objServiceContributions.ServiceDonationId = ServiceDonationId;
                                    contriAmount = contriAmount + IndAmount;
                                    percent = percent + Convert.ToDecimal(objServiceContributions.Percentage);
                                    Int64 estatus = _ServiceDonations.InsertServiceContributions(objServiceContributions);

                                    if (estatus == -1)
                                    {
                                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                        return RedirectToAction("Index", "ServiceDonations");
                                    }
                                }

                                Entities.ServiceContributions objGenContributions = new Entities.ServiceContributions();
                                IndAmount = TotalAmount - contriAmount;
                                objGenContributions.Percentage = (100 - percent).ToString();
                                objGenContributions.CAmount = IndAmount;
                                objGenContributions.ServiceDonationId = ServiceDonationId;
                                objGenContributions.ServiceId = 10;
                                Int64 estatus1 = _ServiceDonations.InsertServiceContributions(objGenContributions);
                            }
                            else
                            {
                                int z = 1;
                                var contributions = lstServiceContributions.FindAll(x => x.ServiceId != 0);

                                foreach (Entities.ServiceContributions objServiceContributions in contributions)
                                {
                                    Int64 estatus = 0;
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
                                    }

                                    if (estatus == -1)
                                    {
                                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed adding member details.</div>";
                                        return RedirectToAction("Index", "ServiceDonations");
                                    }

                                    z++;
                                }
                            }
                        }
                    }
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Failed adding member details.</div>";
                    return RedirectToAction("Index", "ServiceDonations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ServiceDonations");
            }

            return View();
        }

        #endregion

        #region DonorsRemainderEmail

        [Authorize]
        public JsonResult DonorsRemainderEmails(Int64 ServiceDonationId = 0, string UserName = "", string Email = "")
        {
            int status = 0;
            int result1 = 0;
            Entities.ServiceDonations objServiceDonations = new Entities.ServiceDonations();
            List<Entities.ServiceContributions> lstServiceContributions = new List<Entities.ServiceContributions>();
            try
            {
                int _qstatus = 0;
                string contributions = "";


                Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);


                if (status == 1)
                {

                    int _status = 0;
                    Entities.MailTemplates objTemplates = _MailTemplates.GetMailTemplateById("A Quick Reminder to Change Lives", 0, ref _status);
                    if (objTemplates != null)
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append(objTemplates.Description);
                        body.Replace("[usersiteurl]", objAppInfo.BaseUrl);
                        body.Replace("[UserName]", BLL.Common.UppercaseFirst(UserName));
                        body.Replace("[Email]", Email);
                        body.Replace("[FBUrl]", objAppInfo.FacebookUrl);
                        body.Replace("[TWUrl]", objAppInfo.TwitterUrl);
                        body.Replace("[YUrl]", objAppInfo.YoutubeUrl);
                        body.Replace("[GUrl]", objAppInfo.SupportEmail);
                        body.Replace("[TPhone]", objAppInfo.CompanyPhone);
                        body.Replace("[TEmail]", objAppInfo.CompanyEmail);
                        body.Replace("[SiteName]", objAppInfo.SiteName);
                        body.Replace("[ApplicationName]", objAppInfo.SiteName);
                        body.Replace("[Contributions]", contributions);

                        _sentmail.SendMailSendinbrevowithfrom(Email, "", objTemplates.Subject, body.ToString());
                        return Json(new { ok = true, messageType = "success", message = "Reminder email sent successfully!" });
                    }
                    return Json(new { ok = true, messageType = "success", message = "Reminder email sent successfully!" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "There was an issue sending the reminder email. Please try again." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { ok = false, messageType = "error", message = "An error occurred while sending the email:" + ex.Message });
            }
        }


        #endregion






        #region BulkuploadOrganizationMembers

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        [HttpPost]
        public IActionResult BulkUpload(IFormFile document)
        {
            if (document == null || document.Length == 0)
            {
                TempData["message"] = "Please upload a valid file.";
                return RedirectToAction("Index", "ServiceDonations");
            }

            string summary = "";
            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(document.FileName));

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    document.CopyTo(stream);
                }

                List<Entities.ServiceDonations> lstServiceDonations = new List<Entities.ServiceDonations>();

                // Read Excel using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // skip header

                    foreach (var row in rows)
                    {
                        var obj = new Entities.ServiceDonations
                        {
                            FirstName = row.Cell(1).GetString(),
                            LastName = row.Cell(2).GetString(),
                            Email = row.Cell(3).GetString(),
                            PhoneNo = row.Cell(4).GetString(),
                            Country = row.Cell(5).GetString(),
                            ZipCode = row.Cell(6).GetString(),
                            Address = row.Cell(7).GetString(),
                            ServiceTitle = row.Cell(8).GetString(),
                            TransactionId = row.Cell(9).GetString(),
                            PaymentStatus = row.Cell(10).GetString(),
                            PaymentMethod = row.Cell(11).GetString(),
                            Amount = row.Cell(12).GetValue<decimal>(),
                            OrderDate = row.Cell(13).GetValue<DateTime>()
                        };

                        lstServiceDonations.Add(obj);
                    }
                }

                // Convert to XML
                string shipmentsXML = BLL.Common.CreateXMLForObject(lstServiceDonations);

                long status = _Donor.BulkUploads(shipmentsXML, ref summary);

                switch (status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Inserted / Updated ServiceDonation(s) successfully.";
                        return File(
                            System.Text.Encoding.UTF8.GetBytes(summary),
                            "text/plain",
                            "ServiceDonationsSummary.txt"
                        );

                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Inserted / Updated ServiceDonations(s) successfully.";
                        break;

                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed Transaction";
                        break;
                }

                return RedirectToAction("Index", "ServiceDonations");
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "ServiceDonations");
            }
        }
        //public FileStreamResult CreateFile(string str = "", string type = "")
        //{
        //    //todo: add some data from your database into that string:
        //    var string_with_your_data = str;
        //    string_with_your_data = str.Replace("\\\\r\\\\n", Environment.NewLine);
        //    var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
        //    var stream = new MemoryStream(byteArray);

        //    return File(stream, "text/plain", "ServiceDonations-Bulk-Upload-output.txt");
        //}

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