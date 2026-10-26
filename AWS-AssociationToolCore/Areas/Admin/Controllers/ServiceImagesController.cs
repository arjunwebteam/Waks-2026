using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    public class ServiceImagesController : Controller
    {
        BLL.ServiceImages _ServiceImages = new BLL.ServiceImages();
        List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();
        Entities.ServiceImages objServiceImages = new Entities.ServiceImages();


        #region ServiceImages

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 ServiceId = 0, string ServiceTitle = "")
        {
            ViewBag.ServiceTitle = ServiceTitle;
            ViewBag.ServiceId = ServiceId;
            return View();
        }


        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ServiceImagesList(Int64 ServiceId = 0, string Search = "", string SortColumn = "E.UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.ServiceImages> lstServiceImages = new List<Entities.ServiceImages>();

            try
            {
                lstServiceImages = _ServiceImages.GetServiceImagesListByVariableee(ServiceId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                ViewBag.message = "<div class=\"error closable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstServiceImages = lstServiceImages;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }



        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult AddServiceImage(Entities.ServiceImages objServiceImages,  HttpPostedFileBase file1, Int64 ServiceId, string ServiceTitle = "")
        //{
        //    try
        //    {
        //        objServiceImages.UpdatedBy = HttpContext.User.Identity.Name.ToString();
        //        objServiceImages.UpdatedDate = DateTime.UtcNow;
        //        objServiceImages.ServiceId = ServiceId;
        //         objServiceImages.IsDefault = true;

        //        var image = (file1 == null ? "NA" : Path.GetExtension(file1.FileName));

        //        Int64 _status = _ServiceImages.InsertServiceImages(objServiceImages, ref image);
        //        if (_status == 1)
        //        {
        //            if (image != "")
        //            {
        //                string FilePath = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\ServiceImages\\Images\\", image);
        //                file1.SaveAs(FilePath);

        //            }

        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Added Service Image  successfully.</div>";

        //            return RedirectToAction("Index", "ServiceImages", new { ServiceId = ServiceId });

        //        }
        //        if (_status == 2)
        //        {
        //            if (image != "")
        //            {
        //                string FilePath = Path.Combine(ConfigurationManager.AppSettings["uploadPath"] + "\\ServiceImages\\Images\\", image);
        //                file1.SaveAs(FilePath);
        //            }

        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Updated Service Image  Successfully</div>";
        //            return RedirectToAction("Index", "ServiceImages", new { ServiceId = ServiceId });
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Failed transaction.</div>";
        //            return RedirectToAction("Index", "ServiceImages", new { ServiceId = ServiceId });
        //        }
        //    }
        //    catch
        //    {
        //        TempData["message"] = "<div class=\"error closable\">Failed transaction.</div>";
        //        return RedirectToAction("Index", "ServiceImages", new { ServiceId = ServiceId });
        //    }

        //}


        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
 
        public IActionResult AddServiceImage(
    Entities.ServiceImages objServiceImages,
    IFormFile Imageurl,          // ✅ replaces HttpPostedFileBase
    Int64 ServiceId = 0,
    string ServiceTitle = "")
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string uploadPath = objappinfo.UploadPath;
                string imageurl = "NA";
                string savedPath = "";

                // ✅ Handle image upload — replaces Imageurl.SaveAs() + WebImage
                if (Imageurl != null && Imageurl.Length > 0)
                {
                    // Get extension — replaces image.ImageFormat
                    string ext = Path.GetExtension(Imageurl.FileName).TrimStart('.');
                    imageurl = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + ext;

                    string imageDir = Path.Combine(uploadPath, "ServiceImages", "Images");
                    Directory.CreateDirectory(imageDir);

                    savedPath = Path.Combine(imageDir, Imageurl.FileName);

                    // ✅ Save file — replaces Imageurl.SaveAs()
                    using (var stream = new FileStream(savedPath, FileMode.Create))
                        Imageurl.CopyTo(stream);
                }

                // ✅ Set properties
                objServiceImages.UpdatedBy = HttpContext.User.Identity?.Name ?? "";
                objServiceImages.UpdatedDate = DateTime.UtcNow;
                objServiceImages.ServiceId = ServiceId;
                objServiceImages.IsDefault = true;

                Int64 _status = _ServiceImages.InsertServiceImages(
                    objServiceImages, ref imageurl);

                if (_status != -1)
                {
                    // ✅ Copy saved file to final imageurl name — replaces image.Save()
                    if (!string.IsNullOrEmpty(imageurl) && imageurl != "NA"
                        && System.IO.File.Exists(savedPath))
                    {
                        string imageDir = Path.Combine(uploadPath, "ServiceImages", "Images");
                        string finalPath = Path.Combine(imageDir, imageurl);

                        if (savedPath != finalPath)
                            System.IO.File.Copy(savedPath, finalPath, true);
                    }

                    if (_status == 1)
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" +
                                              "Inserted Image Successfully.</div>";
                    else if (_status == 2)
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" +
                                              "Changes have been Updated Successfully.</div>";

                    return RedirectToAction("Index", "ServiceImages",
                        new { area = "Admin", ServiceId = ServiceId });
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Uploading Failed.</div>";
                    return RedirectToAction("Index", "ServiceImages",
                        new { area = "Admin", ServiceId = ServiceId });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                      "Failed transaction.</div>";
                return RedirectToAction("Index", "ServiceImages",
                    new { area = "Admin", ServiceId = ServiceId });
            }
        }




        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditServiceImage(Int64 ServiceImageId = 0)
        {

            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.ServiceImages _objServiceImagess = _ServiceImages.GetServiceImagesById(ServiceImageId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objServiceImagess });
                }
                else
                {
                    str = "<div class=\"success closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error closable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }



        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //public ActionResult ServiceImagesDelete(Int64 ServiceImageId, Int64 ServiceId = 0, string ImageUrl = "")
        //{
        //    try
        //    {
        //        Int64 _status = _ServiceImages.DeleteServiceImages(ServiceImageId);
        //        if (_status == 1)
        //        {
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Deleted image successfully</div>";
        //            System.IO.File.Delete(ConfigurationManager.AppSettings["uploadpath"] + @"\\ServiceImages\\Images\\" + ImageUrl);
        //            System.IO.File.Delete(ConfigurationManager.AppSettings["uploadpath"] + @"\\ServiceImages\\Images\\" + ImageUrl);
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Failed deleting image</div>";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" + ex.Message + "</div>";
        //    }
        //    return RedirectToAction("ServiceImages", "Services", new { ServiceId = ServiceId });
        //}


        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ImageDefaultImage(Int64 ServiceImageId = 0, Int64 ServiceId = 0)
        {
            string str = "";
            try
            {
                Int64 _status = _ServiceImages.ImageDefaultImage(ServiceImageId, ServiceId);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Set Default Image Successfully</div>";
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Failed Updated</div>";
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" + ex.Message + "</div>";
            }
            return RedirectToAction("ServiceImages", "Services", new { ServiceId = ServiceId });
        }


        #endregion 

    }
}
