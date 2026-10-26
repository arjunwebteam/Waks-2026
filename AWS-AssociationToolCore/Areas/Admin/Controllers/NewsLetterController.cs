using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,NewsLetter,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class NewsLetterController : Controller
    {
        AWS_AssociationToolCore.BLL.NewsLetter _NewsLetter = new AWS_AssociationToolCore.BLL.NewsLetter();
        AWS_AssociationToolCore.DAL.NewsLetter _DNewsLetter = new AWS_AssociationToolCore.DAL.NewsLetter();
        AWS_AssociationToolCore.Entities.NewsLetter objNewsLetter = new AWS_AssociationToolCore.Entities.NewsLetter();
        BLL.Users _users = new BLL.Users();

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public IActionResult Index(long mid = 0)
        {
            // Get UserId from session
            long userId = 0;
            if (HttpContext.Session.GetString("UserId") != null)
            {
                userId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            }

            int status = 0;
            var objUserRoles = _users.GetRoleDetialsById(userId, mid, ref status);

            // Store permissions in session
            HttpContext.Session.SetString("IsEdit", objUserRoles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objUserRoles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objUserRoles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objUserRoles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objUserRoles.IsAdd.ToString());

            // Pass permissions to ViewBag
            ViewBag.IsEdit = objUserRoles.IsEdit;
            ViewBag.IsView = objUserRoles.IsView;
            ViewBag.IsDelete = objUserRoles.IsDelete;
            ViewBag.IsExport = objUserRoles.IsExport;
            ViewBag.IsAdd = objUserRoles.IsAdd;
            ViewBag.mid = mid;

            return View();
        }

        [HttpGet]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult NewsLetterList(string search = "",string SortColumn = "", string SortOrder = "", int PageNo = 1, int PageItems = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            int Total = 0;
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            List<Entities.NewsLetter> lstNewsLetter = new List<Entities.NewsLetter>();
            try
            {
                lstNewsLetter = _NewsLetter.GetNewsLetterListByVariable(HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);               
            }
            catch 
            {
                Total = -1;
            }

            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.Total = Total;
            ViewBag.items = PageItems;
            ViewBag.PageNo = PageNo;
            ViewBag.NewsLetterlist = lstNewsLetter;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }


       [HttpPost]
       [Areas.Admin.Models.SessionClass.SessionExpireFilter]

        public JsonResult DeleteNewsLetter(Int64 LetterId)
        {
            string str = "";
            try
            {
                Int64 _status = _NewsLetter.DeleteNewsLetter(LetterId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed transaction."
                    });
                }
            }
            catch 
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult NewsLetterStatus(Int64 LetterId)
        {
            
            try
            {
                Int64 _status = _NewsLetter.UpdateNewsLettertatusById(LetterId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Status Updated Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating user status"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",

                });
            }
        }

        //public void NewsLetterExporttoExcel(string Search = "", string SortColumn = "RegisteredDate", string SortOrder = "DESC")
        //{
        //    string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
        //    try
        //    {
        //        DataTable dtUni = _DNewsLetter.ExportNewsLetterGetList(Search,Sort);
        //        MemoryStream stream = BLL.Common.CSVUtility.GetCSV(dtUni);

        //        var filename = "CSV-Subscribers-Export-" + DateTime.UtcNow.ToString("MM-dd-yyyy") + ".csv";
        //        var contenttype = "text/csv";
        //        Response.Clear();
        //        Response.ContentType = contenttype;
        //        Response.AddHeader("content-disposition", "attachment;filename=" + filename);
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.BinaryWrite(stream.ToArray());
        //        Response.End();
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //}

        public IActionResult NewsLetterExporttoExcel(string search = "", string sortColumn = "RegisteredDate", string sortOrder = "DESC")
        {
            try
            {
                // Build sort string
                string sort = !string.IsNullOrEmpty(sortColumn) ? $"{sortColumn} {sortOrder}" : "";

                // Get data from service or repository
                DataTable dtNewsLetter = _DNewsLetter.ExportNewsLetterGetList(search, sort);

                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtNewsLetter, "NewsLetter-Export");

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Position = 0; // Reset stream position

                        string fileName = $"NewsLetter-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        // Return as FileResult for download
                        return File(stream.ToArray(), contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = $"Failed transaction: {ex.Message}";
                return RedirectToAction("Index", "NewsLetter");
            }
        }
    }
}
