using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using AWS_AssociationToolCore.Entities;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,DeveloperAdmin,")]
    public class ExhibitsRegistrationsController : Controller
    {
        AWS_AssociationToolCore.BLL.ExhibitsRegistrations _ExhibitsRegistrations = new AWS_AssociationToolCore.BLL.ExhibitsRegistrations();
        AWS_AssociationToolCore.DAL.ExhibitsRegistrations _ExhibitsRegistrationsD = new AWS_AssociationToolCore.DAL.ExhibitsRegistrations();
        AWS_AssociationToolCore.BLL.Events _EventRegistrations = new AWS_AssociationToolCore.BLL.Events();
        BLL.Users _users = new BLL.Users();

        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        public IActionResult Index(Int64 mid = 0)
        {
            int status = 0;
            string userIdStr = HttpContext.Session.GetString("UserId");
            long? userId = null;
            if (!string.IsNullOrEmpty(userIdStr) && long.TryParse(userIdStr, out long id))
            {
                userId = id;
            }

            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }
            var objUserRoles = new UserRoles();
            int qStatus = 0;

            var lstPaymentMethods = new List<PaymentMethods>();
            var lstPaymentStatus = new List<PaymentStatus>();

            try
            {
                // Populate payment methods and status
                _EventRegistrations.EventRegistrationsAddRequirements(ref lstPaymentMethods, ref lstPaymentStatus, ref qStatus);

                objUserRoles = _users.GetRoleDetialsById(userId.Value, mid, ref status);

                if (qStatus != 1)
                {
                    TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error closable\">{ex.Message}</div>";
            }

            // Pass lists to view
            ViewBag.lstPaymentMethods = lstPaymentMethods;
            ViewBag.lstPaymentStatus = lstPaymentStatus;

            // Store user role permissions in session
            HttpContext.Session.SetInt32("IsEdit", objUserRoles.IsEdit ? 1 : 0);
            HttpContext.Session.SetInt32("IsView", objUserRoles.IsView ? 1 : 0);
            HttpContext.Session.SetInt32("IsDelete", objUserRoles.IsDelete ? 1 : 0);
            HttpContext.Session.SetInt32("IsExport", objUserRoles.IsExport ? 1 : 0);
            HttpContext.Session.SetInt32("IsAdd", objUserRoles.IsAdd ? 1 : 0);

            // Pass role permissions to view
            ViewBag.IsEdit = objUserRoles.IsEdit;
            ViewBag.IsView = objUserRoles.IsView;
            ViewBag.IsDelete = objUserRoles.IsDelete;
            ViewBag.IsExport = objUserRoles.IsExport;
            ViewBag.IsAdd = objUserRoles.IsAdd;

            ViewBag.mid = mid;

            return View();
        }
        [HttpGet]
        public ActionResult ExhibitsRegistrationsList(string search = "", Int64 PaymentStatusId = 0, Int64 PaymentMethodId = 0, string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int PageItems = 10, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.ExhibitsRegistrations> lstExhibitsRegistrations = new List<Entities.ExhibitsRegistrations>();
            try
            {
                lstExhibitsRegistrations = _ExhibitsRegistrations.GetExhibitsRegistrationsListByVariable(HttpUtility.UrlDecode(search.Trim()), PaymentStatusId, PaymentMethodId, Sort, PageNo, PageItems, ref Total);

            }
            catch
            {
                Total = -1;
            }
            ViewBag.Total = Total;
            ViewBag.items = PageItems;
            ViewBag.PageNo = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.lstExhibitsRegistrations = lstExhibitsRegistrations;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [HttpPost]
        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        public ActionResult AddExhibitsRegistrations(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            try
            {
               Int64 ExhibitId=0;
                objExhibitsRegistrations.UpdatedBy = this.User.Identity.Name;
                objExhibitsRegistrations.InsertedBy = this.User.Identity.Name;
                objExhibitsRegistrations.IsActive = true;
                Int64 _status = _ExhibitsRegistrations.InsertExhibitsRegistrations(objExhibitsRegistrations, ref ExhibitId);
                if (_status != -1)
                {
                    TempData["message"] = "<div class=\"success closable\">Donor Uploaded successfully.</div>";
                    return RedirectToAction("Index", "ExhibitsRegistrations");

                }
                else
                {
                    TempData["message"] = "<div class=\"error closable\"Uploading Failed.</div>";
                    return RedirectToAction("Index", "ExhibitsRegistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error closable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ExhibitsRegistrations");
            }
        }

        [HttpPost]
        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        public ActionResult AdminExhibitsRegistrationsUpdate(Entities.ExhibitsRegistrations objExhibitsRegistrations)
        {
            try
            { 
                objExhibitsRegistrations.UpdatedBy = this.User.Identity.Name; 
                objExhibitsRegistrations.IsActive = true;
                Int64 _status = _ExhibitsRegistrations.AdminExhibitsRegistrationsUpdate(objExhibitsRegistrations);
                if (_status != -1)
                {
                    TempData["message"] = "<div class=\"success closable\">ExhibitsRegistrations Uploaded successfully.</div>";
                    return RedirectToAction("Index", "ExhibitsRegistrations");

                }
                else
                {
                    TempData["message"] = "<div class=\"error closable\"Uploading Failed.</div>";
                    return RedirectToAction("Index", "ExhibitsRegistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error closable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "ExhibitsRegistrations");
            }
        } 

        public JsonResult DeleteExhibitsRegistrations(Int64 ExhibitId)
        {
            string str = "";
            try
            {
                Int64 _status = _ExhibitsRegistrations.DeleteExhibitsRegistrations(ExhibitId);
                if (_status == 1)
                {
                    str = "<div class=\"success closable\">Successfully Deleted</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"error closable\">Failed deleting item</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult EditExhibitsRegistrations(Int64 ExhibitId)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.ExhibitsRegistrations objExhibitsRegistrations = _ExhibitsRegistrations.GetExhibitsRegistrationsById(ExhibitId, ref status);
                if (objExhibitsRegistrations != null)
                {
                    return Json(new { ok = true, data = objExhibitsRegistrations });
                }
                else
                {
                    str = "<div class=\"success closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch 
            {
                str = "<div class=\"success closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        //[AWS_AssociationToolCore.Models.SessionClass.SessionExpireFilter]
        public JsonResult ExhibitsRegistrationstatus(Int64 ExhibitId)
        {
            string str = "";
            try
            {
                Int64 _status = _ExhibitsRegistrations.UpdateExhibitsRegistrationsStatus(ExhibitId);
                if (_status == 1)
                {
                    str = "<div class=\"success closable\">Updated status successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"error closable\">Failed updating status</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"error closable\">Failed updating status</div>";
                return Json(new { ok = false, data = str });
            }
        }


        #region export to excel

        public IActionResult ExhibitsRegistrationsExportToExcel(
            long PaymentStatusId = 0,
            long PaymentMethodId = 0,
            string Search = "",
            string SortColumn = "UpdatedDate",
            string SortOrder = "DESC")
        {
            try
            {
                string sort = !string.IsNullOrEmpty(SortColumn) ? $"{SortColumn} {SortOrder}" : "";

                // Get the data from database
                DataTable dtUni = _ExhibitsRegistrationsD.ExhibitsRegistrationsExportToExcel(
                    PaymentStatusId, PaymentMethodId, Search, sort);

                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dtUni, "Exhibits-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Exhibits-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        // Return Excel file as a download
                        return File(
                            stream,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error closable\">{ex.Message}</div>";
                return RedirectToAction("Index"); // or any fallback page
            }
        }
        #endregion

    }
}
