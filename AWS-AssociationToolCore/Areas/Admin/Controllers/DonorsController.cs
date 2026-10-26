using AWS_AssociationToolCore.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Data;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Helpers;
using ClosedXML.Excel;
using System.Data.OleDb;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,Treasurer,DeveloperAdmin,")]
    public class DonorsController : Controller
    {
        AWS_AssociationToolCore.BLL.Donors _Donor = new AWS_AssociationToolCore.BLL.Donors();
        DAL.Donors _DonorsD = new DAL.Donors();
        BLL.Members _Members = new BLL.Members();
        BLL.DonationCategories _DonationCategories = new BLL.DonationCategories();
        AWS_AssociationToolCore.Entities.Donors objRequestCalls = new AWS_AssociationToolCore.Entities.Donors();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 DonationCategoryId = 0, Int64 ChapterId = 0)
        {
            try
            {
                int _qstatus = 0;
                List<Entities.DonationCategories> _lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref _qstatus);
                List<Entities.Chapters> lstChapters = _Chapters.GetChaptersList(ref _qstatus);

               
                if (_qstatus == 1)
                {
                    ViewBag.lstDonationCategories = _lstDonationCategories;
                    ViewBag.lstChapters = lstChapters;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Donors");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.DonationCategoryId = DonationCategoryId;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        [HttpGet]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult DonorsList(Int64 MemberId=0,Int64 ChapterId=0,Int64 PaymentMethod = 0, Int64 PaymentStatus = 0, string search = "", string SortColumn = "InsertedDate", string SortOrder = "Desc", int PageNo = 1, int PageItems = 10)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.Donors> lstDonors = new List<Entities.Donors>();
            try
            {
                lstDonors = _Donor.GetDonorsListByVariable(MemberId,ChapterId, PaymentMethod, PaymentStatus, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);
               
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message+"</div>";
            }
            ViewBag.ChapterId = ChapterId;
            ViewBag.Total = Total;
            ViewBag.items = PageItems;
            ViewBag.PageNo = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.lstDonors = lstDonors;
            return View();
        }

        public JsonResult DeleteDonor(Int64 DonorId)
        {
            string str = "";
            try
            {
                Int64 _status = _Donor.DeleteDonor(DonorId);
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
        public ActionResult ViewDonor(Int64 DonorId)
        {
            Entities.Donors objDonors = new Entities.Donors();
            int status = 0;
            try
            {
                objDonors = _Donor.DonorsGetById(DonorId, ref status);

                if (objDonors == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "Donors");
                }

            }
            catch(Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex .Message+ "</div>";
                return RedirectToAction("Index", "Donors");
            }

            ViewBag.objDonors = objDonors;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditDonor(Int64 DonorId)
        {
            int Total = 0;
            Entities.Donors objDonors = new Entities.Donors();
            Entities.Members objMembers = new Entities.Members();
            List<Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            lstChapters = _Chapters.GetChaptersList(ref Total);

            int status = 0;
           
            int Donationstatus = 0;
            try
            {
                int _qstatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                objDonors = _Donor.DonorsGetById(DonorId, ref status);
                lstDonationCategories = _DonationCategories.FEDonationCategoriesGetList(ref Donationstatus);
               
                if (objDonors == null && objMembers == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "Donors");
                }

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                return RedirectToAction("Index", "Donors");
            }
            ViewBag.lstDonationCategories = lstDonationCategories;
            ViewBag.lstChapters = lstChapters;
            ViewBag.objDonors = objDonors;
            ViewBag.objMembers = objMembers;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddDonor(Entities.Donors objDonors)
        {
            try
            {
                Int64 DonorId = 0;

                objDonors.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objDonors.InsertedBy = HttpContext.User.Identity.Name.ToString();
                if(objDonors.SameBilling == "1")
                {
                    objDonors.BillingAdress = objDonors.Address;
                    objDonors.BillingCity = objDonors.City;
                    objDonors.BillingCountry = objDonors.Country;
                    objDonors.BillingState = objDonors.State;
                    objDonors.BillingZipCode = objDonors.ZipCode;
                }
                

                Int64 _status = _Donor.InsertDonors(objDonors, ref DonorId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "Donors");
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("EditDonor", "Donors", new { DonorId = objDonors.DonorId });
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("Index", "Donors");
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message+"</div>";
                return RedirectToAction("Index", "Donors");
           
            }

            return View();
        }

        [Models.SessionClass.SessionExpireFilter]

        public ActionResult AddDonor(Int64 DonationCategoryId = 0, Int64 ChapterId = 0)
        {

            int Total = 0;
            Entities.Members objMembers = new Entities.Members();
            List<Entities.DonationCategories> lstDonationCategories = new List<Entities.DonationCategories>();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            lstChapters = _Chapters.GetChaptersList(ref Total);

            int Donationstatus = 0;
            try
            {
                int _qstatus = 0;
                objMembers = _Members.AddMembershipRequirement(ref _qstatus);
                lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref Donationstatus);
                //List<Entities.DonationCategories> lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref _qstatus);
                if (_qstatus == 1)
                {
                    ViewBag.lstDonationCategories = lstDonationCategories;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Donors");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.DonationCategoryId = DonationCategoryId;
            ViewBag.objMembers = objMembers;
            ViewBag.lstChapters = lstChapters;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        //{
        //    int _qstatus = 0;
        //    try
        //    {
        //        List<Entities.DonationCategories> lstDonationCategories = _DonationCategories.GetDonationCategoriesList(ref _qstatus);

        //        if (_qstatus == 1)
        //        {
        //            ViewBag.lstDonationCategories = lstDonationCategories;
        //            ViewBag.status = _qstatus;
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //            return RedirectToAction("Index", "Donors");
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //    ViewBag.CategoryName = CategoryName;
        //    return View();
        //}
       

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult DonorsStatus(Int64 DonorId)
        {
            string str = "";
            try
            {
                Int64 _status = _Donor.UpdateDonorsStatus(DonorId);
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

        public IActionResult DonorRegistrationsExportToExcel(
            long paymentStatusId = 0,
            long paymentMethodId = 0,
            string search = "",
            string sortColumn = "InsertedDate",
            string sortOrder = "DESC")
        {
            try
            {
                string sort = !string.IsNullOrEmpty(sortColumn)
                                ? $"{sortColumn} {sortOrder}"
                                : "";

                DataTable dtUni = _DonorsD.DonorRegistrationsExportToExcel(
                    paymentStatusId,
                    paymentMethodId,
                    search,
                    sort
                );

                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dtUni, "Donor-Registration-Export");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        string fileName = $"Donor-Registration-Export-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">{ex.Message}</div>";
                return RedirectToAction("Index"); // change if needed
            }
        }
        #endregion

    }
}
