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
using SkiaSharp;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,News,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class NewsController : Controller
    {
        BLL.News _news = new BLL.News();
        Entities.News objNews = new Entities.News();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.Users _users = new BLL.Users();

        [Authorize]
        public ActionResult Index(Int64 ChapterId = 0, Int64 mid = 0)
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
            int _qstatus = 0;
            int CategoryLevel = 0;

            try
            {

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

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult NewsList(Int64 ChapterId = 0, Int64 MemberId = 0, string Type = "", string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            int Total = 0;
            List<Entities.News> lstNews = new List<Entities.News>();
            try
            {
                string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
                lstNews = _news.GetNewsListByVariable(ChapterId, MemberId, Type, Search, Sort, PageNo, Items, ref Total);
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.total = Total;
            ViewBag.lstNews = lstNews;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.Type = Type;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        public ActionResult EditNews(Int64 NewsId = 0, Int64 mid = 0)
        {
            try
            {
                int status = 0;
                int Total = 0;
                objNews = _news.GetNewsById(NewsId, ref status);
                List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
                lstChapters = _Chapters.GetChaptersList(ref Total);
                ViewBag.lstChapters = lstChapters;

                if (status != -1 && objNews != null)
                {
                    ViewBag.objNews = objNews;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed processing your request.";
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
            }
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult ViewNews(Int64 NewsId, Int64 mid = 0)
        {
            try
            {
                int status = 0;
                objNews = _news.GetNewsById(NewsId, ref status);
                if (status != -1 && objNews != null)
                {
                    ViewBag.objNews = objNews;
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed transaction.";
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
            }
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        public ActionResult AddNews(Int64 mid = 0)
        {
            int Total = 0;
            List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
            lstChapters = _Chapters.GetChaptersList(ref Total);
            ViewBag.lstChapters = lstChapters;
            ViewBag.mid = mid;
            return View();
        }

        //[Areas.Admin.Models.SessionClass.SessionExpireFilter]
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Save(Entities.News objNews, HttpPostedFileBase ImageUrl, string Thumbimage = "", Int64 mid = 0)
        //{
        //    try
        //    {
        //        //var image = WebImage.GetImageFromRequest();
        //        //string imageurl = (image != null ? image.ImageFormat : "NA");

        //        Image image = null;
        //        string imageurl = "NA";

        //        if (ImageUrl != null)
        //        {
        //            ImageUrl.SaveAs(Session["UploadPath"] + "\\temp\\" + ImageUrl.FileName);
        //            WebImage image2 = new WebImage(Session["UploadPath"] + "\\temp\\" + ImageUrl.FileName);
        //            imageurl = (image2 != null ? image2.ImageFormat : "NA");

        //            //string base64PartTemp = Thumbimage.Split(',')[1];
        //            //string base64PartFinal = base64PartTemp.Split('\"')[0];
        //            //byte[] bytes = Convert.FromBase64String(base64PartFinal);

        //            //using (MemoryStream ms = new MemoryStream(bytes))
        //            //{
        //            //    image = Image.FromStream(ms);
        //            //}
        //        }

        //        Int64 _status = _news.InsertNews(objNews, ref imageurl);
        //        switch (_status)
        //        {
        //            case 1:
        //                if (imageurl != null)
        //                {


        //                    ImageUrl.SaveAs(Session["UploadPath"] + "\\news\\" + imageurl);
        //                    ImageUrl.SaveAs(Session["UploadPath"] + "\\news\\thumb\\" + imageurl);

        //                    //image.Resize(350, 350, true, false);
        //                    //image.Crop(1, 1, 1, 1);
        //                    //image.Save(Session["UploadPath"] + "\\news\\" + imageurl);
        //                }

        //                TempData["messageType"] = "success";
        //                TempData["message"] = "Added Record Successfully";
        //                // TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Added Record Successfully</div>";
        //                break;
        //            case 2:
        //                if (image != null)
        //                {
        //                    ImageUrl.SaveAs(Session["UploadPath"] + "\\news\\" + imageurl);
        //                    ImageUrl.SaveAs(Session["UploadPath"] + "\\news\\thumb\\" + imageurl);
        //                    //image.Resize(350, 350, true, false);
        //                    //image.Crop(1, 1, 1, 1);
        //                    //image.Save(Session["UploadPath"] + "\\news\\" + imageurl);
        //                }


        //                TempData["messageType"] = "success"; // Assuming "info" for update
        //                TempData["message"] = "Changes has been Updated Successfully";
        //                //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
        //                break;
        //            case -1:
        //                TempData["messageType"] = "warning"; // Assuming "info" for update
        //                TempData["message"] = "Failed Adding news details.";
        //                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Adding news details.</div>";
        //                break;
        //            default:
        //                TempData["messageType"] = "warning"; // Assuming "info" for update
        //                TempData["message"] = "Failed Adding news details.";
        //                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Adding news details.</div>";
        //                break;
        //        }
        //    }
        //    catch(Exception EX)
        //    {
        //        TempData["messageType"] = "warning"; // Assuming "info" for update
        //        TempData["message"] = EX.Message;
        //        //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //    return RedirectToAction("Index", "News", new { mid = mid });
        //}


        [Authorize]
        [HttpPost]
        public ActionResult Save(Entities.News objNews, IFormFile file, string Thumbimage = "", Int64 mid = 0)
        {
            try
            {
                // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfoHelper.GetAppInfoDetails(ref status1);
                string uploadPath = objAppInfo.UploadPath;

                string imageurl = "NA";
                string fileName = "";
                byte[] thumbBytes = null;

                if (file != null && file.Length > 0)
                {
                    fileName = Path.GetFileName(file.FileName);

                    // ✅ Save to temp folder first
                    string tempDir = Path.Combine(uploadPath, "temp");
                    string tempPath = Path.Combine(tempDir, fileName);
                    Directory.CreateDirectory(tempDir);

                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    imageurl = fileName; // ✅ Use filename as imageurl (WebImage.ImageFormat replaced)

                    // ✅ Decode base64 thumbnail safely
                    if (!string.IsNullOrEmpty(Thumbimage) && Thumbimage.Contains(","))
                    {
                        string base64Part = Thumbimage.Split(',')[1].Split('"')[0];
                        thumbBytes = Convert.FromBase64String(base64Part);
                    }
                }

                objNews.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                // ✅ Insert/Update DB — imageurl may change after this
                Int64 _status = _news.InsertNews(objNews, ref imageurl);

                // ✅ Save final file after DB call
                if (file != null && file.Length > 0 && imageurl != "NA" && imageurl != "")
                {
                    string newsDir = Path.Combine(uploadPath, "news");
                    string finalPath = Path.Combine(newsDir, imageurl);
                    Directory.CreateDirectory(newsDir);

                    // ✅ Save original file to news folder
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        file.OpenReadStream().CopyTo(stream);
                    }

                    // ✅ Save thumb image if base64 was provided
                    if (thumbBytes != null)
                    {
                        string thumbDir = Path.Combine(uploadPath, "news", "thumb");
                        string thumbPath = Path.Combine(thumbDir, imageurl);
                        Directory.CreateDirectory(thumbDir);

                        using (var ms = new MemoryStream(thumbBytes))
                            ImageHelper.SaveAs(ms, thumbPath);
                    }

                    // ✅ Clean up temp file
                    string tempFile = Path.Combine(uploadPath, "temp", fileName);
                    if (System.IO.File.Exists(tempFile))
                        System.IO.File.Delete(tempFile);
                }

                switch (_status)
                {
                    case 1:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Inserted Record Successfully";
                        break;
                    case 2:
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes has been Updated Successfully";
                        break;
                    case -1:
                    default:
                        TempData["messageType"] = "error";
                        TempData["message"] = "Failed inserting news details.";
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
            }

            return RedirectToAction("Index", "News", new { mid = mid });
        }
        [HttpPost]
        [Authorize]
        public JsonResult Delete(Int64 NewsId, string filename)
        {
            //string str = "";
            //bool _bool = true;

            try
            {
                Int64 _status = _news.DeleteNews(NewsId);
                //switch (_status)
                //{
                //    case 1:
                //        if (filename != null && filename != "")
                //        {
                //            System.IO.File.Delete(Session["UploadPath"] + @"\news\" + filename);
                //        }
                //        ok = true,
                //        messageType = "success",
                //        message = "Record Deleted successfully"




                //        str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                //        _bool = true;
                //        break;
                //    case -1:
                //        str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting news.</div>";
                //        _bool = false;
                //        break;
                //}


                if (_status == 1)
                {
                    //System.IO.File.Delete(Session["UploadPath"] + @"\news\" + filename);
                    //str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });

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
                        message = "Failed Deleting the Record"
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
            //return Json(new { ok = _bool, data = str });
        }

        [Authorize]
        [HttpPost]
        public JsonResult NewsStatus(Int64 NewsId)
        {
            string str = "";
            try
            {
                Int64 _status = _news.UpdateNewsStatus(NewsId);
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
        [HttpPost]
        public JsonResult NewsDisplayOrder(int OrderNo, Int64 NewsId)
        {
            string str = "";
            try
            {
                Int64 _status = _news.UpdateNewsDisplayOrder(OrderNo, NewsId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Order Successfully"
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
