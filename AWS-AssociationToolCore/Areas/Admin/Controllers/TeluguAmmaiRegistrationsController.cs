using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    public class TeluguAmmaiRegistrationsController : Controller
    {
        BLL.TeluguAmmaiRegistrations _TeluguAmmaiRegistrations = new BLL.TeluguAmmaiRegistrations();
        BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();
        BLL.Members _Members = new BLL.Members();
        Entities.Members objMembers = new Entities.Members();
        BLL.Events _Events = new BLL.Events();
        DAL.TeluguAmmaiRegistrations _DTeluguRegistrations = new DAL.TeluguAmmaiRegistrations();
        List<Entities.PaymentStatus> listPaymentStatus = new List<Entities.PaymentStatus>();

        List<Entities.EventCompetitions> lstEventCompetitions = new List<Entities.EventCompetitions>();
        BLL.EventCompetitions _EventCompetitions = new BLL.EventCompetitions();

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 RegistrationCategoryId = 0, string Type = "")
        {
            int _qstatus = 0;
            ViewBag.lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref _qstatus);
            ViewBag.lstEvents = _Events.GetEventsList(ref _qstatus);
            int _qstatus1 = 0;

            listPaymentStatus = _PaymentSettings.GetPaymentStatusList(ref _qstatus1);
            lstEventCompetitions = _EventCompetitions.GetEventCompetitionsList(ref _qstatus1);

            objMembers = _Members.AddMembershipRequirement(ref _qstatus1);

            ViewBag.objMembers = objMembers;
            ViewBag.listPaymentStatus = listPaymentStatus;
            ViewBag.lstEventCompetitions = lstEventCompetitions;
            return View();
        }

        [HttpGet]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult TeluguAmmaiList(Int64 EventId = 0, Int64 RegistrationCategoryId = 0, Int64 TeluguAmmaiRegistrationId = 0, Int64 ChapterId = 0, Int64 PaymentStatusId = 0, string CompetitionCategory1 = "", string search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.TeluguAmmaiRegistrations> lstTeluguAmmaiRegistrations = new List<Entities.TeluguAmmaiRegistrations>();
            try
            {
                lstTeluguAmmaiRegistrations = _TeluguAmmaiRegistrations.GetTeluguAmmaiRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

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
            ViewBag.lstTeluguAmmaiRegistrations = lstTeluguAmmaiRegistrations;
            return View();
        }

        public JsonResult DeleteTeluguAmmaiRegistration(Int64 TeluguAmmaiRegistrationId)
        {
            string str = "";
            try
            {
                Int64 _status = _TeluguAmmaiRegistrations.DeleteTeluguAmmaiRegistrations(TeluguAmmaiRegistrationId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting item</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }


        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditTeluguAmmaiRegistration(Int64 TeluguAmmaiRegistrationId)
        {
            Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations = new Entities.TeluguAmmaiRegistrations();
            List<Entities.PaymentMethods> lstPaymentMethods = new List<Entities.PaymentMethods>();
            List<Entities.PaymentStatus> lstPaymentStatus = new List<Entities.PaymentStatus>();
            int status = 0;
            try
            {
                lstPaymentMethods = _PaymentSettings.GetPaymentMethodsList(ref status);
                lstPaymentStatus = _PaymentSettings.GetPaymentStatusList(ref status);

                objTeluguAmmaiRegistrations = _TeluguAmmaiRegistrations.GetTeluguAmmaiRegistrationsGetById(TeluguAmmaiRegistrationId, ref status);

                if (objTeluguAmmaiRegistrations == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "TeluguAmmaiRegistrations");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "TeluguAmmaiRegistrations");
            }

            ViewBag.objTeluguAmmaiRegistrations = objTeluguAmmaiRegistrations;
            ViewBag.TeluguAmmaiRegistrationId = TeluguAmmaiRegistrationId;
            ViewBag.lstPaymentStatus = lstPaymentStatus;
            ViewBag.lstPaymentMethods = lstPaymentMethods;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddTeluguAmmaiRegistration(Entities.TeluguAmmaiRegistrations objTeluguAmmaiRegistrations)
        {
            try
            {
                Int64 TeluguAmmaiRegistrationId = 0;
                objTeluguAmmaiRegistrations.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objTeluguAmmaiRegistrations.InsertedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _TeluguAmmaiRegistrations.FETeluguAmmaiRegistrationsInsert(objTeluguAmmaiRegistrations, ref TeluguAmmaiRegistrationId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "TeluguAmmaiRegistrations");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "TeluguAmmaiRegistrations");
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("Index", "TeluguAmmaiRegistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "TeluguAmmaiRegistrations");

            }

            //return RedirectToAction("Index", "TeluguAmmaiRegistrations");
        }


        #region export to excel

        //public void TeluguREgistrationExporttoExcel(Int64 EventId = 0, Int64 RegistrationCategoryId = 0, Int64 TeluguAmmaiRegistrationId = 0, Int64 ChapterId = 0, Int64 PaymentStatusId = 0, string CompetitionCategory1 = "", string search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 20)
        //{
        //    try
        //    {
        //        string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
        //        int Total = 0;
        //        DataTable dtUni = _DTeluguRegistrations.ExportToTeluguAmmayiRegistration(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);
        //        //MemoryStream stream = BLL.Common.CSVUtility.GetCSV(dtUni);

        //        //var filename = "CSV-Donor-Registration-Export-" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".csv";
        //        //var contenttype = "text/csv";
        //        //Response.Clear();
        //        //Response.ContentType = contenttype;
        //        //Response.AddHeader("content-disposition", "attachment;filename=" + filename);
        //        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        //Response.BinaryWrite(stream.ToArray());
        //        //Response.End();

        //        using (XLWorkbook wb = new XLWorkbook())
        //        {
        //            wb.Worksheets.Add(dtUni, "teluguammayi-reg-Export");

        //            Response.Clear();
        //            Response.Buffer = true;
        //            Response.Charset = "";
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment;filename=CSV-teluguammayi-reg-Export-" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".xlsx");
        //            using (MemoryStream MyMemoryStream = new MemoryStream())
        //            {
        //                wb.SaveAs(MyMemoryStream);
        //                MyMemoryStream.WriteTo(Response.OutputStream);
        //                Response.Flush();
        //                Response.End();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
        //    }
        //}

        #endregion

    }
}
