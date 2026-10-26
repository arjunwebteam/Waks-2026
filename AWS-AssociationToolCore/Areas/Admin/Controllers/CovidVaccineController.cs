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
    public class CovidVaccineController : Controller
    {
        BLL.CovidVaccine _CovidVaccine = new BLL.CovidVaccine();
        BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();

        DAL.CovidVaccine _DCovidVaccine = new DAL.CovidVaccine();

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult CovidVaccineList(Int64 CovidVaccineId = 0, Int64 ChapterId = 0, string search = "", string SortColumn = "UpdatedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.CovidVaccine> lstCovidVaccine = new List<Entities.CovidVaccine>();
            try
            {
                lstCovidVaccine = _CovidVaccine.GetCovidVaccineListByVariableId(ChapterId, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

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
            ViewBag.lstCovidVaccine = lstCovidVaccine;
            return View();
        }

        public JsonResult DeleteCovidVaccineId(Int64 CovidVaccineId)
        {
            string str = "";
            try
            {
                Int64 _status = _CovidVaccine.DeleteCovidVaccine(CovidVaccineId);
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
        public ActionResult EditCovidVaccine(Int64 CovidVaccineId)
        {
            Entities.CovidVaccine objCovidVaccine = new Entities.CovidVaccine();

            int status = 0;
            try
            {


                objCovidVaccine = _CovidVaccine.GetCovidVaccineGetById(CovidVaccineId, ref status);

                if (objCovidVaccine == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "CovidVaccine");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "CovidVaccine");
            }

            ViewBag.objCovidVaccine = objCovidVaccine;
            ViewBag.CovidVaccineId = CovidVaccineId;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddCovidVaccine(Entities.CovidVaccine objCovidVaccine)
        {
            try
            {
                Int64 CovidVaccineId = 0;
                objCovidVaccine.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _CovidVaccine.FECovidVaccineInsert(objCovidVaccine, ref CovidVaccineId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "CovidVaccine");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "CovidVaccine");
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("Index", "CovidVaccine");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "CovidVaccine");

            }

            return RedirectToAction("Index", "CovidVaccine");
        }


        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult CovidVaccineStatus(Int64 CovidVaccineId)
        {
            string str = "";
            try
            {
                Int64 _status = _CovidVaccine.UpdateCovidVaccine(CovidVaccineId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        #region export to excel

        public IActionResult CovidVaccineExporttoExcel(
            Int64 CovidVaccineId = 0,
            string Search = "",
            string SortColumn = "InsertedDate",
            string SortOrder = "ASC")
        {
            try
            {
                string Sort = !string.IsNullOrEmpty(SortColumn)
                              ? SortColumn + " " + SortOrder
                              : "";

                DataTable dtUni = _DCovidVaccine
                    .ExportToCovidVaccine(CovidVaccineId, Search, Sort);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtUni, "COVID-Vaccin-Drive-Export");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        ms.Position = 0;

                        string fileName = "CSV-COVID-Vaccin-Drive-Export-"
                            + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".xlsx";

                        // ✅ Return File — replaces Response.Clear/Buffer/Charset/End
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
                return RedirectToAction("Index", new { area = "Admin" });
            }
        }
        #endregion

    }
}
