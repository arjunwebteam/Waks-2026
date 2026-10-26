using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
//using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Administrator,Forms,Events,DeveloperAdmin,")]
    public class Form1Controller : Controller
    {
        BLL.Form1 _Form1 = new BLL.Form1();
        List<Entities.Form1> lstForm1 = new List<Entities.Form1>();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        List<Entities.RegistrationCategories> lstRegistrationCategories = new List<Entities.RegistrationCategories>();
        DAL.Form1 _DForm1 = new DAL.Form1();
        BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();
        BLL.Members _Members = new BLL.Members();
        Entities.Members objMembers = new Entities.Members();
        BLL.Events _Events = new BLL.Events();

        #region  Form1

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 RegistrationCategoryId = 0, string Type = "")
        {
            int _qstatus = 0;
            ViewBag.lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref _qstatus);
            ViewBag.lstEvents = _Events.GetEventsList(ref _qstatus);
            int _qstatus1 = 0;

            objMembers = _Members.AddMembershipRequirement(ref _qstatus1);

            ViewBag.objMembers = objMembers;
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult Form1List(Int64 EventId = 0, Int64 RegistrationCategoryId = 0, Int64 PaymentMethodId = 0, Int64 PaymentStatusId = 0, string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {

            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstForm1 = _Form1.GetForm1ListByVariable(EventId, RegistrationCategoryId, PaymentMethodId, PaymentStatusId, Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                Total = -1;
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstForm1 = lstForm1;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult AddForm1(Int64 RegistrationCategoryId = 0)
        {
            try
            {

                int _qstatus = 0;
                ViewBag.lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref _qstatus);
                ViewBag.lstEvents = _Events.GetEventsList(ref _qstatus);

                int _qstatus1 = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus1);
                
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return RedirectToAction("Index", "Form1");
            }
            ViewBag.RegistrationCategoryId = RegistrationCategoryId;
            ViewBag.objMembers = objMembers;
            
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult AddForm1(Entities.Form1 objForm1)
        {
            try
            {
                objForm1.InstertedBy = HttpContext.User.Identity.Name.ToString();
                objForm1.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objForm1.IsApproved = true;
                Int64 _status = _Form1.InsertForm1(objForm1);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Event Registration Successfully</div>";
                    return RedirectToAction("Index", "Form1");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Event Registration Successfully</div>";
                    return RedirectToAction("Index", "Form1");
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("AddForm1", "Form1");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return RedirectToAction("AddForm1", "Form1");
            }

        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult EditForm1(Int64 CompetitionRegistrationId = 0)
        {
            Entities.Members objMembers = new Entities.Members();
            try
            {

                int qstatus = 0;
                int _qstatus1 = 0;
                List<Entities.RegistrationCategories> _lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref qstatus);
                objMembers = _Members.AddMembershipRequirement(ref qstatus);
                ViewBag.lstEvents = _Events.GetEventsList(ref _qstatus1);

                int _qstatus = 0;
                Entities.Form1 _objForm1 = _Form1.GetForm1ById(CompetitionRegistrationId, ref _qstatus);
                
                if (_qstatus == 1)
                {
                    ViewBag.objForm1 = _objForm1;
                    ViewBag.lstRegistrationCategories = _lstRegistrationCategories;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", " Form1");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return RedirectToAction("Index", " Form1");
            }
            ViewBag.objMembers = objMembers;
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewForm1(Int64 CompetitionRegistrationId = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.Form1 _objForm1 = _Form1.GetForm1ById(CompetitionRegistrationId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objForm1 = _objForm1;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Form1");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return RedirectToAction("Index", "Form1");
            }
            return View();
        }

        [Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteForm1(Int64 CompetitionRegistrationId)
        {
            string str = "";
            try
            {

                Int64 _status = _Form1.DeleteForm1(CompetitionRegistrationId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Deleted Event Registration successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting Event Registration</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + ".</div>";
                return Json(new { ok = false, data = str });
            }
        }


        [HttpPost]
        public JsonResult Form1Status(Int64 CompetitionRegistrationId)
        {
            string str = "";
            try
            {
                Int64 _status = _Form1.UpdateForm1StatusById(CompetitionRegistrationId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated status successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
            }
        }

        #endregion

        #region Export


        public IActionResult Form1ExportToExcel(long EventId = 0, long RegistrationCategoryId = 0, long MemberId = 0,
                                               long PaymentMethodId = 0, long PaymentStatusId = 0, string Search = "",
                                               string SortColumn = "UpdatedDate", string SortOrder = "DESC")
        {
            try
            {
                int total = 0;
                string sort = !string.IsNullOrEmpty(SortColumn) ? SortColumn + " " + SortOrder : "";

                // Fetch DataTable from your data layer
                DataTable dtUni = _DForm1.Form1ExportToExcel(EventId, RegistrationCategoryId, MemberId, PaymentMethodId, PaymentStatusId, Search, sort, ref total);

                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dtUni, "Form1-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Form1-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        // Return Excel file as download
                        return File(stream,
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">{ex.Message}</div>";
                return RedirectToAction("Index"); // Redirect to your Index or appropriate page
            }
        }

        #endregion

    }
}
