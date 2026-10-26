using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    public class PatrikaRegistrationsController : Controller
    {
        AWS_AssociationToolCore.BLL.PatrikaRegistrations _PatrikaRegistrations = new AWS_AssociationToolCore.BLL.PatrikaRegistrations();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        private readonly object _env;

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            int Total = 0;
            lstChapters = _Chapters.GetChaptersList(ref Total);
            ViewBag.lstChapters = lstChapters;
            return View();
        }

        [HttpGet]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult PatrikaRegistrationsList(Int64 ChapterId = 0, string search = "", int PageNo = 1, int PageItems = 10, string SortColumn = "", string SortOrder = "")
        {
            int Total = 0;
            List<Entities.PatrikaRegistrations> PatrikaRegistrationsList = new List<Entities.PatrikaRegistrations>();
            try
            {
                string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");

                PatrikaRegistrationsList = _PatrikaRegistrations.GetPatrikaRegistrationsListByVariable(ChapterId, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.items = PageItems;
            ViewBag.pageno = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.PatrikaRegistrationsList = PatrikaRegistrationsList;
            return View();
        }

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //public IActionResult AddPatrikaRegistrations(Entities.PatrikaRegistrations objPatrikaRegistrations, IFormFile BannerUrl)
        //{
        //    try
        //    {
        //        string uploadRoot = Path.Combine(
        //            _env.WebRootPath, "Akshara_Deepika");
        //        string normalPath = Path.Combine(uploadRoot, "NormalImages");
        //        string thumbPath = Path.Combine(uploadRoot, "ThumbImages");

        //        // Ensure directories exist
        //        Directory.CreateDirectory(normalPath);
        //        Directory.CreateDirectory(thumbPath);

        //        string imageUrl = "";

        //        if (BannerUrl != null && BannerUrl.Length > 0)
        //        {
        //            string uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(BannerUrl.FileName)}";
        //            string normalFilePath = Path.Combine(normalPath, uniqueFileName);

        //            // Save original image
        //            using (var stream = new FileStream(normalFilePath, FileMode.Create))
        //            {
        //                BannerUrl.CopyTo(stream);
        //            }

        //            // Using WebImage for resizing/cropping like old MVC
        //            var image = new WebImage(normalFilePath);
        //            imageUrl = image.ImageFormat ?? "NA";

        //            // Save normal image
        //            image.Save(normalFilePath);

        //            // Save thumbnail
        //            image.Resize(130, 130, true, false);
        //            image.Crop(1, 1, 1, 1);
        //            string thumbFilePath = Path.Combine(thumbPath, uniqueFileName);
        //            image.Save(thumbFilePath);
        //        }

        //        objPatrikaRegistrations.InsertedBy = User.Identity.Name ?? "System";
        //        objPatrikaRegistrations.UpdatedBy = User.Identity.Name ?? "System";

        //        long status = _PatrikaRegistrations.InsertPatrikaRegistrations(objPatrikaRegistrations, ref imageUrl);

        //        if (status == 1)
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully.</div>";
        //        else if (status == 2)
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes have been Updated Successfully</div>";
        //        else
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Uploading Failed.</div>";

        //        return RedirectToAction("Index", "PatrikaRegistrations");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = $"<div class=\"alert alert-danger alert-dismissable\">{ex.Message}</div>";
        //        return RedirectToAction("Index", "PatrikaRegistrations");
        //    }
        //}
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult EditPatrikaRegistrations(Int64 PatrikaId)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.PatrikaRegistrations objPatrikaRegistrations = _PatrikaRegistrations.GetPatrikaRegistrationsById(PatrikaId, ref status);
                if (objPatrikaRegistrations != null)
                {
                    return Json(new { ok = true, data = objPatrikaRegistrations });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
           
            catch (Exception ex)
            {
                throw ex;
               
            }
        }

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]

        public JsonResult DeletePatrikaRegistrations(Int64 PatrikaId, string BannerUrl = "")
        {
            string str = "";
            try
            {
                Int64 _status = _PatrikaRegistrations.PatrikaRegistrationsDelete(PatrikaId);
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

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult PatrikaRegistrationsStatus(Int64 PatrikaId)
        {
            string str = "";
            try
            {
                Int64 _status = _PatrikaRegistrations.UpdatePatrikaRegistrationsStatus(PatrikaId);
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
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        public JsonResult PatrikaRegistrationsOrderNo(int OrderNo, Int64 PatrikaId)
        {
            string str = "";
            try
            {
                Int64 _status = _PatrikaRegistrations.UpdatePatrikaRegistrationsOrderNo(OrderNo, PatrikaId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
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

    }
}
