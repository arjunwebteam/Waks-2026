using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    public class ChessTournamentRegistrationsController : Controller
    {
        BLL.ChessTournamentRegistrations _ChessTournamentRegistrations = new BLL.ChessTournamentRegistrations();
        BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();
        BLL.Members _Members = new BLL.Members();
        Entities.Members objMembers = new Entities.Members();
        BLL.Events _Events = new BLL.Events();
        DAL.ChessTournamentRegistrations _DTeluguRegistrations = new DAL.ChessTournamentRegistrations();
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
        public ActionResult ChessTournamentList(Int64 EventId = 0, Int64 RegistrationCategoryId = 0, Int64 ChessTournamentRegistrationId = 0, Int64 ChapterId = 0, Int64 PaymentStatusId = 0, string CompetitionCategory1 = "", string search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.ChessTournamentRegistrations> lstChessTournamentRegistrations = new List<Entities.ChessTournamentRegistrations>();
            try
            {
                lstChessTournamentRegistrations = _ChessTournamentRegistrations.GetChessTournamentRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

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
            ViewBag.lstChessTournamentRegistrations = lstChessTournamentRegistrations;
            return View();
        }

        public JsonResult DeleteTeluguAmmaiRegistration(Int64 ChessTournamentRegistrationId)
        {
            string str = "";
            try
            {
                Int64 _status = _ChessTournamentRegistrations.DeleteChessTournamentRegistrations(ChessTournamentRegistrationId);
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
        public ActionResult EditChess(Int64 ChessTournamentRegistrationId)
        {
            Entities.ChessTournamentRegistrations objChessTournamentRegistrations = new Entities.ChessTournamentRegistrations();
            List<Entities.PaymentMethods> lstPaymentMethods = new List<Entities.PaymentMethods>();
            List<Entities.PaymentStatus> lstPaymentStatus = new List<Entities.PaymentStatus>();
            int status = 0;
            try
            {
                lstPaymentMethods = _PaymentSettings.GetPaymentMethodsList(ref status);
                lstPaymentStatus = _PaymentSettings.GetPaymentStatusList(ref status);

                objChessTournamentRegistrations = _ChessTournamentRegistrations.GetChessTournamentRegistrationsGetById(ChessTournamentRegistrationId, ref status);

                if (objChessTournamentRegistrations == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "ChessTournamentRegistrations");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ChessTournamentRegistrations");
            }

            ViewBag.objChessTournamentRegistrations = objChessTournamentRegistrations;
            ViewBag.ChessTournamentRegistrationId = ChessTournamentRegistrationId;
            ViewBag.lstPaymentStatus = lstPaymentStatus;
            ViewBag.lstPaymentMethods = lstPaymentMethods;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddTeluguAmmaiRegistration(Entities.ChessTournamentRegistrations objChessTournamentRegistrations)
        {
            try
            {
                Int64 ChessTournamentRegistrationId = 0;
                objChessTournamentRegistrations.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objChessTournamentRegistrations.InsertedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _ChessTournamentRegistrations.FEChessTournamentRegistrationsInsert(objChessTournamentRegistrations, ref ChessTournamentRegistrationId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "ChessTournamentRegistrations");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "ChessTournamentRegistrations");
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("Index", "ChessTournamentRegistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ChessTournamentRegistrations");

            }

           
        }


        #region export to excel

        [Authorize]
        public IActionResult TeluguREgistrationExporttoExcel(
            Int64 EventId = 0,
            Int64 RegistrationCategoryId = 0,
            Int64 ChessTournamentRegistrationId = 0,
            Int64 ChapterId = 0,
            Int64 PaymentStatusId = 0,
            string CompetitionCategory1 = "",
            string search = "",
            string SortColumn = "UpdatedDate",
            string SortOrder = "Desc",
            int PageNo = 1,
            int PageItems = 20)
        {
            try
            {
                string Sort = (!string.IsNullOrEmpty(SortColumn)
                                ? SortColumn + " " + SortOrder : "");
                int Total = 0;

                // ✅ UrlDecode — replaces HttpUtility.UrlDecode
                string decodedSearch = Uri.UnescapeDataString(search.Trim());

                DataTable dtUni = _DTeluguRegistrations
                    .ExportToChessTournamentRegistration(
                        EventId, RegistrationCategoryId, ChapterId,
                        PaymentStatusId, CompetitionCategory1,
                        decodedSearch, Sort, PageNo, PageItems, ref Total);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "chess-reg-Export");

                    // ✅ Use MemoryStream + File() — replaces Response.Clear/Buffer/End
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        ms.Position = 0;

                        string fileName = "CSV-chess-reg-Export-"
                            + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".xlsx";

                        // ✅ Return file directly — replaces Response.OutputStream
                        return File(
                            ms.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">"
                                    + ex.Message + "</div>";
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}
