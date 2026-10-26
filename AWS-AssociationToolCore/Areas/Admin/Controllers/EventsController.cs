using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
//using System.Web.Helpers;
//using System.Web.Security;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Events,ChapterAdmin,SiteAdmin,Administrator,ProgramsManager,Treasurer,DeveloperAdmin,")]
    [Area("Admin")]

    [Authorize]
    public class EventsController : Controller
    {
        AWS_AssociationToolCore.BLL.Events _Events = new AWS_AssociationToolCore.BLL.Events();
        AWS_AssociationToolCore.BLL.EventCategories _EventCategories = new AWS_AssociationToolCore.BLL.EventCategories();
        AWS_AssociationToolCore.BLL.Chapters _Chapters = new AWS_AssociationToolCore.BLL.Chapters();
        AWS_AssociationToolCore.BLL.Committees _Committees = new AWS_AssociationToolCore.BLL.Committees();
        List<Entities.EventDates> lstEventDates = new List<Entities.EventDates>();
        BLL.RegistrationCategories _RegistrationCategories = new BLL.RegistrationCategories();
        BLL.Users _users = new BLL.Users();

        AWS_AssociationToolCore.BLL.PhotosImageSettings _PhotosImageSettings = new AWS_AssociationToolCore.BLL.PhotosImageSettings();

        #region Events

        [Authorize]
        public ActionResult Index(string EventType = "", Int64 ChapterId = 0, Int64 mid = 0)
        {
            long userId = 0;

            var userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                long.TryParse(userIdStr, out userId);
            }

            if (Request.Cookies.TryGetValue("UserRole", out string userRoleValue))
            {
                HttpContext.Session.SetString("userrole", userRoleValue);
            }
            long chapterId = 0;

            var chapterValue = HttpContext.Session.GetString("chapterid");

            if (!string.IsNullOrEmpty(chapterValue))
            {
                long.TryParse(chapterValue, out chapterId);
            }



            List<Entities.Chapters> lstChapters = new();
            List<Entities.PageDetails> lstPageDetails = new();
            List<Entities.MenuItems> lstMenuItems = new();
            Entities.UserRoles objuserroles = new();

            int status = 0;
            int CategoryLevel = 0;

            try
            {
                //lstMenuItems = _MenuItems.GetMenuItemsByLevel(CategoryLevel, ref status);
                //lstPageDetails = _PageDetails.GetPageDetailsList(ref status);
                lstChapters = _Chapters.GetChaptersList(ref status);

                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);

                // ✅ Session (Core way)
                HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
                HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
                HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
                HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
                HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

                // ✅ ViewBag (optional)
                ViewBag.IsEdit = objuserroles.IsEdit;
                ViewBag.IsView = objuserroles.IsView;
                ViewBag.IsDelete = objuserroles.IsDelete;
                ViewBag.IsExport = objuserroles.IsExport;
                ViewBag.IsAdd = objuserroles.IsAdd;
                ViewBag.mid = mid;
            }
            catch
            {
                status = -1;
            }

            // ✅ ViewBag assignments
            ViewBag.ChapterId = chapterId;
            //ViewBag.IsFooterBar = IsFooterBar;
            //ViewBag.IsMenuBar = IsMenuBar;
            //ViewBag.IsQuickLinks = IsQuickLinks;
            ViewBag.EventType = EventType;

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EventsList(Int64 MemberId = 0, Int64 EventStatusId = 0, string EventType = "upcoming", string StartDate = "", string EndDate = "", Int64 ChapterId = 0, string search = "", string SortColumn = "StartDate", string SortOrder = "ASC", int PageNo = 1, int PageItems = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            List<Entities.Events> lstEventslist = new List<Entities.Events>();
            int Total = 0;
            try
            {
                lstEventslist = _Events.GetEventsListByVariable(MemberId, EventStatusId, EventType, StartDate, EndDate, ChapterId, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.items = PageItems;
            ViewBag.pageno = PageNo;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.Eventslist = lstEventslist;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        public ActionResult AddEvent(Int64 mid = 0)
        {
            List<Entities.EventCategories> lstEventCategories = new List<Entities.EventCategories>();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            // List<Entities.PhotosImageSettings> objPhotosImageSettings = new List<Entities.PhotosImageSettings>();
            int Total = 0;

            int _qstatus = 0;
            try
            {
                lstEventCategories = _EventCategories.GetEventCategoriesList(ref Total);
                lstChapters = _Chapters.GetChaptersList(ref Total);
                // Entities.PhotosImageSettings objPhotosImageSettings = _PhotosImageSettings.GetPhotosImageSettingsDetails(ref status);



                ViewBag.lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref _qstatus);

            }
            catch
            {
                Total = -1;
            }
            ViewBag.lstEventCategories = lstEventCategories;
            ViewBag.lstChapters = lstChapters;
            ViewBag.mid = mid;
            // ViewBag.objPhotosImageSettings = objPhotosImageSettings;
            return View();
        }
        [HttpPost]
        public IActionResult AddEvent(IFormFile file,IFormFile file1,Entities.Events objEvents,
    List<Entities.EventRegistrationTypes> lstEventRegistrationTypes,
    List<Entities.EventDates> lstEventDates,
    List<Entities.RegistrationCategories> lstRegistrationCategories,
    string Thumbimage = "",
    Int64 mid = 0)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string imageurl2 = "NA";
                string imageurl3 = "NA";
                Image thumbImage = null;

                // ── Banner file (file) ──────────────────────────────────────────
                if (file != null && file.Length > 0)
                {
                    string ext2 = Path.GetExtension(file.FileName);
                    imageurl2 = $"{Path.GetFileNameWithoutExtension(file.FileName)}" +
                                $"-{DateTime.Now:ddMMyyyyHHmmss}{ext2}";

                    // Build thumbnail from base64 string
                    if (!string.IsNullOrEmpty(Thumbimage) && Thumbimage.Contains(','))
                    {
                        string base64PartTemp = Thumbimage.Split(',')[1];
                        string base64PartFinal = base64PartTemp.Split('"')[0];
                        byte[] bytes = Convert.FromBase64String(base64PartFinal);

                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            thumbImage = Image.FromStream(ms);
                        }
                    }
                }

                // ── Flyer file (file1) ──────────────────────────────────────────
                if (file1 != null && file1.Length > 0)
                {
                    string ext3 = Path.GetExtension(file1.FileName);
                    imageurl3 = $"{Path.GetFileNameWithoutExtension(file1.FileName)}" +
                                $"-{DateTime.Now:ddMMyyyyHHmmss}{ext3}";
                }

                // ── Build XML fields ────────────────────────────────────────────
                if (lstEventRegistrationTypes != null && lstEventRegistrationTypes.Count != 0
                    && lstEventRegistrationTypes[0].RegistrationTitle != null)
                {
                    objEvents.XMLRegistrations = BLL.Common.CreateXMLForObject(lstEventRegistrationTypes);
                }

                //if (lstEventDates != null && lstEventDates.Count != 0
                //    && lstEventDates[0].StartDate != null)
                //{
                //    objEvents.XMLDates = BLL.Common.CreateXMLForObject(lstEventDates);
                //}
                if (lstEventDates != null && lstEventDates.Count != 0
    && lstEventDates[0].StartDate != DateTime.MinValue
    && lstEventDates[0].StartDate.Year > 1753)
                {
                    objEvents.XMLDates = BLL.Common.CreateXMLForObject(lstEventDates);
                }
                if (lstRegistrationCategories != null && lstRegistrationCategories.Count != 0)
                {
                    objEvents.XMLRegistrationsCat = BLL.Common.CreateXMLForObject(lstRegistrationCategories);
                }

                objEvents.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objEvents.IsActive = true;

                // ── Insert / Update ─────────────────────────────────────────────
                Int64 _status = _Events.InsertEvent(objEvents, ref imageurl2, ref imageurl3);

                // ── Save banner + thumbnail ─────────────────────────────────────
                if (_status > 0 && file != null && file.Length > 0 && imageurl2 != "NA")
                {
                    string bannerFolder = Path.Combine(objappinfo.UploadPath, "events", "banners");
                    string thumbFolder = Path.Combine(objappinfo.UploadPath, "events", "banners", "thumb");

                    Directory.CreateDirectory(bannerFolder);
                    Directory.CreateDirectory(thumbFolder);

                    // Save original banner
                    string bannerPath = Path.Combine(bannerFolder, imageurl2);
                    using (var stream = new FileStream(bannerPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Save thumbnail derived from base64
                    if (thumbImage != null)
                    {
                        string thumbPath = Path.Combine(thumbFolder, imageurl2);
                        thumbImage.Save(thumbPath);
                        thumbImage.Dispose();
                    }
                }

                // ── Save flyer ──────────────────────────────────────────────────
                if (_status > 0 && file1 != null && file1.Length > 0 && imageurl3 != "NA")
                {
                    string flyerFolder = Path.Combine(objappinfo.UploadPath, "events", "flyers");
                    Directory.CreateDirectory(flyerFolder);

                    string flyerPath = Path.Combine(flyerFolder, imageurl3);
                    using (var stream = new FileStream(flyerPath, FileMode.Create))
                    {
                        file1.CopyTo(stream);
                    }
                }

                // ── Status switch ───────────────────────────────────────────────
                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully";
                        return RedirectToAction("Index", "Events",
                            new { EventType = "upcoming", mid });

                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes have been Updated Successfully";
                        return RedirectToAction("Index", "Events",
                            new { EventType = "upcoming", EventId = objEvents.EventId, mid });

                    case -1:
                        // Clean up saved files on failure
                        CleanupEventFiles(objappinfo.UploadPath, imageurl2, imageurl3);
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed updating event details.";
                        return RedirectToAction("Index", "Events",
                            new { EventType = "upcoming", mid });

                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed updating event details.";
                        return RedirectToAction("Index", "Events",
                            new { EventType = "upcoming", mid });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Events",
                    new { EventType = "upcoming", mid });
            }
        }

        // ── Helper: delete files if DB insert failed ────────────────────────────────
        private void CleanupEventFiles(string uploadPath, string imageurl2, string imageurl3)
        {
            void TryDelete(string path)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            if (imageurl2 != "NA")
            {
                TryDelete(Path.Combine(uploadPath, "events", "banners", imageurl2));
                TryDelete(Path.Combine(uploadPath, "events", "banners", "thumb", imageurl2));
            }

            if (imageurl3 != "NA")
            {
                TryDelete(Path.Combine(uploadPath, "events", "flyers", imageurl3));
            }
        }

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult AddEvent(HttpPostedFileBase file, HttpPostedFileBase file1, Entities.Events objEvents, List<Entities.EventRegistrationTypes> lstEventRegistrationTypes, List<Entities.EventDates> lstEventDates, List<Entities.RegistrationCategories> lstRegistrationCategories, string Thumbimage = "", Int64 mid = 0)
        //{
        //    try
        //    {

        //        Image image = null;
        //        string imageurl2 = "NA";

        //        if (file != null)
        //        {
        //            file.SaveAs(Session["UploadPath"] + "\\temp\\" + file.FileName);
        //            WebImage image2 = new WebImage(Session["UploadPath"] + "\\temp\\" + file.FileName);
        //            imageurl2 = (image2 != null ? image2.ImageFormat : "NA");


        //            string base64PartTemp = Thumbimage.Split(',')[1];
        //            string base64PartFinal = base64PartTemp.Split('\"')[0];
        //            byte[] bytes = Convert.FromBase64String(base64PartFinal);

        //            using (MemoryStream ms = new MemoryStream(bytes))
        //            {
        //                image = Image.FromStream(ms);
        //            }
        //        }

        //        string imageurl3 = "NA";

        //        if (file1 != null)
        //        {
        //            file1.SaveAs(Session["UploadPath"] + "\\temp\\" + file1.FileName);
        //            WebImage image3 = new WebImage(Session["UploadPath"] + "\\temp\\" + file1.FileName);
        //            imageurl3 = (image3 != null ? image3.ImageFormat : "NA");
        //        }

        //        //objEvents.UpdatedBy = this.User.Identity.Name;
        //        if (lstEventRegistrationTypes != null && lstEventRegistrationTypes.Count != 0 && lstEventRegistrationTypes[0].RegistrationTitle != null)
        //        {
        //            objEvents.XMLRegistrations = BLL.Common.CreateXMLForObject(lstEventRegistrationTypes);
        //        }
        //        if (lstEventDates != null && lstEventDates.Count != 0 && lstEventDates[0].StartDate != null)
        //        {
        //            objEvents.XMLDates = BLL.Common.CreateXMLForObject(lstEventDates);
        //        }
        //        if (lstRegistrationCategories != null && lstRegistrationCategories.Count != 0)
        //        {
        //            objEvents.XMLRegistrationsCat = BLL.Common.CreateXMLForObject(lstRegistrationCategories);
        //        }

        //        objEvents.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

        //        Int64 _status = _Events.InsertEvent(objEvents, ref imageurl2, ref imageurl3);
        //        if (image != null)
        //        {
        //            file.SaveAs(Session["UploadPath"] + "\\events\\banners\\" + imageurl2);
        //            image.Save(Session["UploadPath"] + "\\events\\banners\\thumb\\" + imageurl2);
        //        }
        //        if (file1 != null)
        //        {
        //            file1.SaveAs(Session["UploadPath"] + "\\events\\flyers\\" + imageurl3);
        //        }
        //        switch (_status)
        //        {
        //            case 1:

        //                TempData["messageType"] = "success";
        //                TempData["message"] = "Record Inserted Successfully";
        //                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
        //            case 2:

        //                TempData["messageType"] = "success";
        //                TempData["message"] = "Changes has been Updated Successfully";
        //                return RedirectToAction("Index", "Events", new { EventId = objEvents.EventId, mid = mid });
        //            case -1:
        //                if (image != null)
        //                {
        //                    System.IO.File.Delete(Session["UploadPath"] + "\\events\\banners\\" + imageurl2);
        //                    System.IO.File.Delete(Session["UploadPath"] + "\\events\\flyers\\" + imageurl3);
        //                    System.IO.File.Delete(Session["UploadPath"] + "\\events\\banners\\thumb\\" + imageurl2);
        //                }

        //                TempData["messageType"] = "error";
        //                TempData["message"] = "Failed updating event details.";
        //                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
        //            default:

        //                TempData["messageType"] = "error";
        //                TempData["message"] = "Failed updating event details.";
        //                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["messageType"] = "error";
        //        TempData["message"] = ex.Message;
        //        return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
        //    }
        //}

        [Authorize]
        [HttpPost]
        public IActionResult EventDelete(Int64 EventId, string ImageFileName)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                Int64 _status = _Events.DeleteEventById(EventId);

                if (_status == 1)
                {
                    // ── Clean up banner + thumbnail ─────────────────────────────
                    if (!string.IsNullOrEmpty(ImageFileName))
                    {
                        string bannerPath = Path.Combine(objappinfo.UploadPath, "events", "banners", ImageFileName);
                        string thumbPath = Path.Combine(objappinfo.UploadPath, "events", "banners", "thumb", ImageFileName);

                        if (System.IO.File.Exists(bannerPath))
                            System.IO.File.Delete(bannerPath);

                        if (System.IO.File.Exists(thumbPath))
                            System.IO.File.Delete(thumbPath);
                    }

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the Event"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }

        [Authorize]
        public ActionResult EditEvent(Int64 EventId = 0, string EventName = "", Int64 mid = 0)
        {

            if (EventId == 0)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
            }

            int status = 0;
            int Total = 0;
            int _qstatus = 0;
            Int64 RegCount = 0;
            List<Entities.EventCategories> lstEventCategories = new List<Entities.EventCategories>();
            List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
            List<Entities.EventDates> lstEventDates = new List<Entities.EventDates>();
            Entities.Events objEvent = new Entities.Events();
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            List<Entities.RegistrationCategories> lstEventRegistrationCategories = new List<Entities.RegistrationCategories>();
            ViewBag.lstRegistrationCategories = _RegistrationCategories.GetRegistrationCategoriesList(ref _qstatus);
            try
            {
                lstEventCategories = _EventCategories.GetEventCategoriesList(ref Total);
                objEvent = _Events.GetEventById(EventId, EventName, ref lstEventRegistrationTypes, ref lstEventDates, ref lstEventRegistrationCategories, ref status);
                lstChapters = _Chapters.GetChaptersList(ref Total);

                if (objEvent == null)
                {
                    TempData["messageType"] = "warning"; // Assuming "info" for update
                    TempData["message"] = "Sorry, failed processing your request.";
                    return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
                }
            }
            catch (Exception EX)
            {
                TempData["messageType"] = "warning"; // Assuming "info" for update
                TempData["message"] = EX.Message;
                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
            }

            ViewBag.objEvent = objEvent;

            ViewBag.lstEventCategories = lstEventCategories;
            ViewBag.lstEventDates = lstEventDates;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            ViewBag.lstEventRegistrationCategories = lstEventRegistrationCategories;
            ViewBag.status = (Total != -1 || status != -1 ? 1 : -1);
            ViewBag.lstChapters = lstChapters;
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult ViewEvent(Int64 EventId = 0, string EventName = "", Int64 mid = 0)
        {

            if (EventId == 0)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
            }

            int status = 0;
            Entities.Events objEvent = new Entities.Events();
            List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();
            List<Entities.RegistrationCategories> lstEventRegistrationCategories = new List<Entities.RegistrationCategories>();
            try
            {
                objEvent = _Events.GetEventById(EventId, EventName, ref lstEventRegistrationTypes, ref lstEventDates, ref lstEventRegistrationCategories, ref status);
                if (objEvent == null)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Sorry, failed processing your request.</div>";
                    return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Events", new { EventType = "upcoming", mid = mid });
            }

            ViewBag.objEvent = objEvent;
            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            ViewBag.status = status;
            ViewBag.mid = mid;

            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult EventStatus(Int64 EventId)
        {
            string str = "";
            try
            {
                Int64 _status = _Events.UpdateEventStatus(EventId);
                if (_status == 1)
                {


                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {

                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating status"
                    });
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }

        #endregion

        #region Event Controls

        [Authorize]
        public ActionResult EventMenu(Int64 EventId = 0, string EventName = "", string tab = "")
        {
            ViewBag.EventId = EventId;
            ViewBag.EventName = EventName;
            ViewBag.tab = tab;

            return View();
        }

        #endregion

        #region EventRegistrationTypes

        [Authorize]
        public ActionResult AddEventRegistrationTypes(Entities.EventRegistrationTypes objRegistrationTypes)
        {
            string str = "";
            bool _bool = true;

            Entities.EventRegistrationTypes objEventRegistrationTypes = new Entities.EventRegistrationTypes();
            try
            {
                objRegistrationTypes.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _Events.InsertRegistrationTypes(objRegistrationTypes);

                if (_status == 1)
                {

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Changes has been Updated Successfully"
                    });


                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    // return Json(new { ok = true, data = str });
                }



                else if (_status == 2)


                {


                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Changes has been Updated Successfully"
                    });

                }

                else
                {
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating status"
                    });
                }



                //switch (_status)
                //{
                //    case 1:
                //        TempData["messageType"] = "success";
                //        TempData["message"] = "Record Inserted Successfully.";
                //        _bool = true;
                //        break;
                //    case 2:
                //        TempData["messageType"] = "warning";
                //        TempData["message"] = "Changes has been Updated Successfully.";
                //        _bool = true;
                //        break;
                //    case -1:
                //        TempData["messageType"] = "error";
                //        TempData["message"] = "Failed processing your request.";
                //        _bool = false;
                //        break;
                //    default:
                //        TempData["messageType"] = "error";
                //        TempData["message"] = "Failed processing your request.";
                //        _bool = false;
                //        break;
                //}
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });

                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }

            //return Json(new { ok = _bool, data = str });
        }

        [Authorize]
        public ActionResult AddEventDates(Entities.EventDates objEventDates)
        {
            string str = "";
            bool _bool = true;


            try
            {
                Int64 _status = _Events.UpdateEventDates(objEventDates);

                switch (_status)
                {
                    case 1:
                        //str = "<div class=\"success closable\"> Inserted Registration Dates details successfully.</div>";
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Registration Dates details successfully.</div>";
                        _bool = true;
                        break;
                    case 2:
                        //str = "<div class=\"success closable\"> Updated Registration Dates details successfully.</div>";
                        TempData["messageType"] = "warning";
                        TempData["message"] = "Updated Registration Dates details successfully.";
                        _bool = true;
                        break;
                    case -1:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        _bool = false;
                        break;
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed processing your request.";
                        _bool = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "" + ex + "Failed transaction.";
            }

            return Json(new { ok = _bool, data = str });
        }


        [Authorize]
        public ActionResult EventRegistrationTypesList(Int64 EventId = 0)
        {

            int Total = 0;
            List<Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<Entities.EventRegistrationTypes>();

            try
            {
                lstEventRegistrationTypes = _Events.GetEventRegistrationTypesList(EventId, ref Total);

            }
            catch
            {
                Total = -1;
            }

            ViewBag.lstEventRegistrationTypes = lstEventRegistrationTypes;
            return View();
        }

        [Authorize]
        public ActionResult EventDates(Int64 EventId = 0)
        {

            int Total = 0;
            List<Entities.EventDates> lstEventDates = new List<Entities.EventDates>();

            try
            {
                lstEventDates = _Events.GetEventsDatesList(EventId, ref Total);

            }
            catch
            {
                Total = -1;
            }

            ViewBag.lstEventDates = lstEventDates;
            return View();
        }


        [Authorize]
        [HttpPost]
        public JsonResult DeleteRegistrationType(Int64 EventRegistrationTypeId)
        {
            string str = "";
            try
            {

                Int64 _status = _Events.DeleteEventRegistrationTypes(EventRegistrationTypeId);
                if (_status == 1)
                {
                    //System.IO.File.Delete(Session["UploadPath"] + @"\news\" + filename);
                    //str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "EventRegistrationTypes Deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the EventRegistrationTypes"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteEventDates(Int64 EventDatesId)
        {
            string str = "";
            try
            {

                Int64 _status = _Events.DeleteEventDates(EventDatesId);
                if (_status == 1)
                {
                    //System.IO.File.Delete(Session["UploadPath"] + @"\news\" + filename);
                    //str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "EventDates Deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the EventDates"
                    });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult EventRegistrationTypesStatus(Int64 EventRegistrationTypeId)
        {
            string str = "";
            try
            {
                Int64 _status = _Events.UpdateEventRegistrationTypesStatus(EventRegistrationTypeId);
                if (_status == 1)
                {


                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {

                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating status"
                    });
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }

        [Authorize]
        public JsonResult UpdateEventRegistrationTypesDisplayOrder(int OrderNo, Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _Events.UpdateEventRegistrationTypesDisplayOrder(OrderNo, EventRegistrationTypeId);
            if (_status == 1)
            {
                return Json(new
                {
                    ok = true,
                    messageType = "success",
                    message = "Updated Order No Successfully"
                });
                //str = "<div class=\"alert alert-success alert-dismissable\">Updated Order Successfully</div>";
                //return Json(new { ok = true, data = str });
            }
            else
            {

                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed updating Order No"
                });
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                //return Json(new { ok = false, data = str });
            }

        }

        [Authorize]
        public JsonResult GetCommitteeDetailedInfo(Int64 ChapterId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.Committees _objCommitteeInfo = _Committees.GetEventCommitteeByChapterId(ChapterId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objCommitteeInfo });
                }
                else
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        public JsonResult GetEventssByCategory(Int64 RegistrationCategoryId = 0)
        {
            try
            {
                List<Entities.Events> lstEvents = new List<Entities.Events>();

                int status = 0;
                lstEvents = _Events.GetEventsByCategory(RegistrationCategoryId, ref status);
                return Json(new { ok = true, data = lstEvents, message = "" });
            }
            catch
            {
                return Json(new { ok = false, message = "<div class=\"error closable\">Failed Transaction</div>" });
            }
        }


        [HttpPost]
        [Authorize]
        public JsonResult Eventtatus(Int64 EventId)
        {
            string str = "";
            try
            {
                Int64 _status = _Events.UpdateWebsiteBannersStatus(EventId);
                if (_status == 1)
                {

                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });


                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    // return Json(new { ok = true, data = str });
                }
                else
                {
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating status"
                    });
                }
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });

                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        #endregion

        public class AuthorizeAttribute : ActionFilterAttribute
        {
            BLL.Users _user = new BLL.Users();
            BLL.Roles _Roles = new BLL.Roles();
            private object FormsAuthentication;
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string userRole = null;
                int status = 0;

                // ✅ Get ClaimsPrincipal — replaces HttpCookie + FormsAuthentication.Decrypt
                var user = filterContext.HttpContext.User;

                if (user?.Identity != null && user.Identity.IsAuthenticated)
                {
                    // ✅ Read role from Claims — replaces authTicket.UserData
                    userRole = user.FindFirst(ClaimTypes.Role)?.Value;

                    // ✅ Read email from Claims — replaces authTicket.Name
                    string emailFromClaim = user.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(emailFromClaim))
                    {
                        // Get user from DB
                        var objuser = _user.GetAdminUsersGetByEmail(
                            emailFromClaim, ref status);

                        // Inside AuthorizeAttribute, where objuser != null:
                        if (objuser != null)
                        {
                            filterContext.HttpContext.Session.SetString("UserName", objuser.UserName ?? "");
                            filterContext.HttpContext.Session.SetString("UserId", objuser.UserId.ToString());
                            filterContext.HttpContext.Session.SetString("UserEmail", emailFromClaim ?? ""); // ✅ ADD THIS
                            filterContext.HttpContext.Session.SetString("chapterid", objuser.ChapterId.ToString());
                            filterContext.HttpContext.Session.SetString("userrole", userRole ?? "");
                        }
                    }
                }

                // ✅ If no role — redirect to LogOn
                //    replaces new UrlHelper(filterContext.RequestContext)
                if (string.IsNullOrEmpty(userRole))
                {
                    filterContext.Result = new RedirectToActionResult(
                        "LogOn", "Account", new { area = "Admin" });
                    return;
                }

                // ✅ Get all allowed roles from DB
                int roleStatus = 0;
                List<Entities.Roles> lstRoles = _Roles.GetRolesList(ref roleStatus);

                // Build flat list of allowed role names
                List<string> allowedRoles = lstRoles
                    .Select(r => r.RoleName.Trim())
                    .ToList();

                // Split user's roles (comma separated) and check
                var userRoles = userRole
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList();

                bool isAuthorized = userRoles.Any(r => allowedRoles.Contains(r));

                if (!isAuthorized)
                {
                    // ✅ Redirect to Unauthorized — replaces UrlHelper + RedirectResult
                    filterContext.Result = new RedirectToActionResult(
                        "Unauthorized", "Account", new { area = "Admin" });
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
        }

    }
}
