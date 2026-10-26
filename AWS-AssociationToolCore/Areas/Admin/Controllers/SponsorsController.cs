using iText.StyledXmlParser.Jsoup.Nodes;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Sponsors,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class SponsorsController : Controller
    {
        AWS_AssociationToolCore.BLL.Sponsors _Sponsors = new AWS_AssociationToolCore.BLL.Sponsors();
        List<Entities.Sponsors> lstSponsors = new List<Entities.Sponsors>();
        AWS_AssociationToolCore.BLL.SponsorCategories _SponsorCategory = new AWS_AssociationToolCore.BLL.SponsorCategories();
        List<Entities.SponsorCategories> lstSponsorCategories = new List<Entities.SponsorCategories>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.Users _users = new BLL.Users();
        BLL.AppInfo _appinfo = new BLL.AppInfo();


        [Authorize]

        public ActionResult Index(Int64 SponsorCategoryId = 0, Int64 ChapterId = 0, Int64 mid = 0)
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

                lstSponsorCategories = _SponsorCategory.GetSponsorCategoriesList(chapterId, ref status);

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
            ViewBag.lstSponsorCategories = lstSponsorCategories;
            ViewBag.SponsorCategoryId = SponsorCategoryId;

            ViewBag.lstChapters = lstChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }




        [HttpGet]
        [Authorize]
        public ActionResult SponsorsList(Int64 MemberId = 0, Int64 SponsorCategoryId = 0, string search = "", int PageNo = 1, int PageItems = 25, string SortColumn = "InsertedTime", string SortOrder = "Desc", bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            try
            {

                lstSponsors = _Sponsors.GetSponsorsListByVariable(MemberId, SponsorCategoryId, HttpUtility.UrlDecode(search.Trim()), Sort, PageNo, PageItems, ref Total);

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
            ViewBag.lstSponsorsList = lstSponsors;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [HttpPost]
      
        [Authorize]
        public ActionResult AddSponsors(List<IFormFile> file, Entities.Sponsors objSponsor, string Thumbimage = "", Int64 mid = 0)
        {
            try
            {
                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                Entities.AppInfo objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string imageurl = "";      // ✅ Declare OUTSIDE if block
                Image image = null;        // ✅ Declare OUTSIDE if block

                // ✅ Use IFormFile correctly (single file from list)
                IFormFile file1 = file != null && file.Count > 0 ? file[0] : null;

                if (file1 != null && file1.Length > 0)
                {
                    imageurl = Path.GetFileName(file1.FileName); // ✅ Now accessible everywhere

                    // Decode base64 thumbnail if provided
                    if (!string.IsNullOrEmpty(Thumbimage))
                    {
                        string base64String = Thumbimage.Contains(",") ? Thumbimage.Split(',')[1] : Thumbimage;
                        byte[] bytes = Convert.FromBase64String(base64String);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                        }
                    }

                    // Save uploaded file temporarily
                    string tempPath = Path.Combine(objappinfo.UploadPath, "temp", imageurl);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        file1.CopyTo(stream);
                    }
                }

                objSponsor.InsertedBy = this.User.Identity.Name;
                objSponsor.InsertedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                // ✅ imageurl declared before — no error now
                Int64 _status = _Sponsors.InsertSponsors(objSponsor, ref imageurl);

                if (_status != -1)
                {
                    if (image != null && !string.IsNullOrEmpty(imageurl))
                    {
                        string normalPath = Path.Combine(objappinfo.UploadPath, "Sponsors", imageurl);
                        string thumbPath = Path.Combine(objappinfo.UploadPath, "Sponsors", "thumb", imageurl);

                        Directory.CreateDirectory(Path.GetDirectoryName(normalPath));
                        Directory.CreateDirectory(Path.GetDirectoryName(thumbPath));

                        // ✅ Save normal file
                        using (var stream = new FileStream(normalPath, FileMode.Create))
                        {
                            file1.CopyTo(stream);
                        }

                        // ✅ Save thumb image
                        image.Save(thumbPath);
                    }

                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully";
                    }
                    else if (_status == 2)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changes has been Updated Successfully";
                    }

                    return RedirectToAction("Index", "Sponsors", new { SponsorCategoryId = objSponsor.SponsorCategoryId, mid = mid });
                }
                else
                {
                    TempData["messageType"] = "warning";
                    TempData["message"] = "Uploading Failed";
                    return RedirectToAction("Index", "Sponsors", new { SponsorCategoryId = objSponsor.SponsorCategoryId, mid = mid });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Sponsors", new { SponsorCategoryId = objSponsor.SponsorCategoryId, mid = mid });
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditSponsor(Int64 SponsorId)
        {
            string str = "";
            int status = 0;
            try
            {
                Entities.Sponsors objSponsors = _Sponsors.GetSponsorById(SponsorId, ref status);
                if (objSponsors != null)
                {
                    return Json(new { ok = true, data = objSponsors });
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

        [HttpPost]
        [Authorize]
     
        public JsonResult DeleteSponsor(Int64 SponsorId, string BannerUrl = "")
        {
            try
            {
                Int64 _status = _Sponsors.DeleteSponsor(SponsorId);

                if (_status == 1)
                {
                    // ✅ Use AppInfo.UploadPath instead of Session["UploadPath"]
                    int status = 0;
                    BLL.AppInfo _appinfo = new BLL.AppInfo();
                    Entities.AppInfo objAppInfo = _appinfo.GetAppInfoDetails(ref status);

                    if (!string.IsNullOrEmpty(BannerUrl))
                    {
                        string normalPath = Path.Combine(objAppInfo.UploadPath, "Sponsors", "NormalImages", BannerUrl);
                        string thumbPath = Path.Combine(objAppInfo.UploadPath, "Sponsors", "ThumbImages", BannerUrl);

                        if (System.IO.File.Exists(normalPath))
                            System.IO.File.Delete(normalPath);

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
                        message = "Failed Deleting the Sponsor"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = $"Failed transaction: {ex.Message}"
                });
            }
        }
        [HttpPost]
        [Authorize]
        public JsonResult SponsorStatus(Int64 SponsorId)
        {
            string str = "";
            try
            {
                Int64 _status = _Sponsors.UpdateSponsorsStatus(SponsorId);
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

                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult SponsorDisplayOrder(int DisplayOrder, Int64 SponsorId)
        {
            string str = "";
            try
            {
                Int64 _status = _Sponsors.UpdateSponsorsDisplayOrder(DisplayOrder, SponsorId);
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
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });


                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
            }
        }

        #region Sponsor Categories

        [Authorize]
        public ActionResult SponsorCategory(Int64 ChapterId = 0, Int64 mid = 0)
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
        public ActionResult SponsorCategoriesList(Int64 ChapterId = 0, string Search = "", string SortColumn = "", string SortOrder = "", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {

                lstSponsorCategories = _SponsorCategory.GetSponsorCategoriesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstSponsorCategories = lstSponsorCategories;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddSponsorCategory(Entities.SponsorCategories objSponsorCategory, Int64 mid = 0)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objSponsorCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objSponsorCategory.UpdatedTime = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                Int64 _status = _SponsorCategory.InsertSponsorCategories(objSponsorCategory);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = " Record Inserted Successfully";


                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Added Record Successfully</div>";
                    return RedirectToAction("SponsorCategory", "Sponsors", new { SponsorCategoryId = objSponsorCategory.SponsorCategoryId, mid = mid });
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";


                    // TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("SponsorCategory", "Sponsors", new { SponsorCategoryId = objSponsorCategory.SponsorCategoryId, mid = mid });
                }
                else
                {
                    TempData["messageType"] = "warning"; // Assuming "info" for update
                    TempData["message"] = "Failed uploading page.";


                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed uploading page.</div>";
                    return RedirectToAction("SponsorCategory", "Sponsors", new { SponsorCategoryId = objSponsorCategory.SponsorCategoryId, mid = mid });
                }
                //switch (_status)
                //{
                //    case 1:
                //        str = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                //        _bool = true;
                //        break;
                //    case 2:
                //        str = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                //        _bool = true;
                //        break;
                //    case -1:
                //        str = "<div class=\"error-alert closable\">Failed processing your request.</div>";
                //        _bool = false;
                //        break;
                //    default:
                //        str = "<div class=\"error-alert closable\">Failed processing your request.</div>";
                //        _bool = false;
                //        break;
                //}
            }
            catch
            {
                TempData["messageType"] = "warning"; // Assuming "info" for update
                TempData["message"] = "Failed uploading page.";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Sponsors", new { mid = mid });

                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //_bool = false;
            }

            //return Json(new { ok = _bool, data = str });

        }

        [Authorize]
        public ActionResult EditSponsorCategory(Int64 SponsorCategoryId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.SponsorCategories _objSponsorCategory = _SponsorCategory.GetSponsorCategoryById(SponsorCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objSponsorCategory });
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
        [HttpPost]
        public JsonResult DeleteSponsorCategory(Int64 SponsorCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _SponsorCategory.DeleteSponsorCategory(SponsorCategoryId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted Successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //return Json(new { ok = true, data = str });
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
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult SponsorCategoriesStatus(Int64 SponsorCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _SponsorCategory.UpdateSponsorcatrgoryStatus(SponsorCategoryId);
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
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateSponsorsCategoryDisplayOrder(int DisplayOrder, Int64 SponsorCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _SponsorCategory.UpdateSponsorsCategoryDisplayOrder(DisplayOrder, SponsorCategoryId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Order Successfully"
                    });
                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Order No Successfully</div>";
                    //return Json(new { ok = true, data = str });
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
                //str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                //return Json(new { ok = false, data = str });
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = ex.Message
                });
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

