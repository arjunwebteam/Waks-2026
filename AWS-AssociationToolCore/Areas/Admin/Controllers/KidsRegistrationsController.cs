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
    public class KidsRegistrationsController : Controller
    {
        BLL.KidsRegistrations _KidsRegistrations = new BLL.KidsRegistrations();
        BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();
        BLL.Members _Members = new BLL.Members();
        Entities.Members objMembers = new Entities.Members();
        BLL.Events _Events = new BLL.Events();
        DAL.KidsRegistrations _DKidsRegistrations = new DAL.KidsRegistrations();
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
        public ActionResult KidsRegistrationsList(Int64 EventId = 0, Int64 RegistrationCategoryId = 0, Int64 KidsRegistrationsId = 0, Int64 ChapterId = 0, Int64 PaymentStatusId = 0, string CompetitionCategory1 = "", string search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.KidsRegistrations> lstKidsRegistrations = new List<Entities.KidsRegistrations>();
            try
            {
                lstKidsRegistrations = _KidsRegistrations.GetKidsRegistrationsListByVariableId(EventId, RegistrationCategoryId, ChapterId, PaymentStatusId, CompetitionCategory1, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

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
            ViewBag.lstKidsRegistrations = lstKidsRegistrations;
            return View();
        }

        public JsonResult DeleteKidsRegistrationsId(Int64 KidsRegistrationsId)
        {
            string str = "";
            try
            {
                Int64 _status = _KidsRegistrations.DeleteKidsRegistrations(KidsRegistrationsId);
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
        public ActionResult EditKidsRegistrations(Int64 KidsRegistrationsId)
        {
            Entities.KidsRegistrations objKidsRegistrations = new Entities.KidsRegistrations();
            List<Entities.PaymentMethods> lstPaymentMethods = new List<Entities.PaymentMethods>();
            List<Entities.PaymentStatus> lstPaymentStatus = new List<Entities.PaymentStatus>();
            int status = 0;
            try
            {
                lstPaymentMethods = _PaymentSettings.GetPaymentMethodsList(ref status);
                lstPaymentStatus = _PaymentSettings.GetPaymentStatusList(ref status);

                objKidsRegistrations = _KidsRegistrations.GetKidsRegistrationsGetById(KidsRegistrationsId, ref status);

                if (objKidsRegistrations == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "KidsRegistrations");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "KidsRegistrations");
            }

            ViewBag.objKidsRegistrations = objKidsRegistrations;
            ViewBag.KidsRegistrationsId = KidsRegistrationsId;
            ViewBag.lstPaymentStatus = lstPaymentStatus;
            ViewBag.lstPaymentMethods = lstPaymentMethods;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddKidsRegistrations(Entities.KidsRegistrations objKidsRegistrations)
        {
            try
            {
                Int64 KidsRegistrationsId = 0;
                objKidsRegistrations.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _KidsRegistrations.FEKidsRegistrationsInsert(objKidsRegistrations, ref KidsRegistrationsId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "KidsRegistrations");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "KidsRegistrations");
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("Index", "KidsRegistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "KidsRegistrations");

            }

            return RedirectToAction("Index", "KidsRegistrations");
        }


        #region export to excel

        public IActionResult KidsRegistrationsExportToExcel(
            long EventId = 0,
            long RegistrationCategoryId = 0,
            long KidsRegistrationsId = 0,
            long ChapterId = 0,
            long PaymentStatusId = 0,
            string CompetitionCategory1 = "",
            string search = "",
            string SortColumn = "UpdatedDate",
            string SortOrder = "Desc",
            int PageNo = 1,
            int PageItems = 20)
        {
            try
            {
                string Sort = !string.IsNullOrEmpty(SortColumn) ? $"{SortColumn} {SortOrder}" : "";
                int Total = 0;

                // Decode search query
                string decodedSearch = HttpUtility.UrlDecode(search?.Trim() ?? "");

                // Fetch data from DB
                DataTable dtUni = _DKidsRegistrations.ExportToKidsRegistrations(
                    EventId,
                    RegistrationCategoryId,
                    ChapterId,
                    PaymentStatusId,
                    CompetitionCategory1,
                    decodedSearch,
                    Sort,
                    PageNo,
                    PageItems,
                    ref Total
                );

                // Generate Excel file in memory
                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dtUni, "Kids-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0; // Reset stream position before returning

                        string fileName = $"Kids-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        return File(stream.ToArray(), contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">{ex.Message}</div>";
                // Redirect back to Index or a relevant page
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
