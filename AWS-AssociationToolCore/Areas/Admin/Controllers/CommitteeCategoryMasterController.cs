using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CommitteeCategoryMasterController : Controller
    {//

        BLL.CommitteeCategoryMaster _CommitteeCategoryMaster = new BLL.CommitteeCategoryMaster();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.Users _users = new BLL.Users();

        [Authorize]
        public ActionResult Index(Int64 mid = 0, Int64 ChapterId = 0)
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
        public ActionResult CommitteeCategoryMasterList(Int64 YearId = 0, Int64 ChapterId = 0, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            try
            {
                int status = 0;
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = _CommitteeCategoryMaster.GetCommitteeCategoryMasterAll(YearId, ChapterId, ref lstCommitteeCategoryMaster2, ref lstCommitteeCategoryMaster3, ref lstCommitteeCategoryMaster4, ref status);
                if (status == 1)
                {
                    ViewBag.lstCommitteeCategoryMaster = lstCommitteeCategoryMaster;
                    ViewBag.lstCommitteeCategoryMaster2 = lstCommitteeCategoryMaster2;
                    ViewBag.lstCommitteeCategoryMaster3 = lstCommitteeCategoryMaster3;
                    ViewBag.lstCommitteeCategoryMaster4 = lstCommitteeCategoryMaster4;
                    ViewBag.total = lstCommitteeCategoryMaster.Count;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            ViewBag.ChapterId = ChapterId;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [Authorize]
        public ActionResult CreateCommitteeCategoryMaster(Int64 mid = 0, Int64 YearId = 0, Int64 ChapterId = 0)
        {
            try
            {
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4 = new List<Entities.CommitteeCategoryMaster>();
                int status = 0;

                int Total = 0;
                ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);




                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = _CommitteeCategoryMaster.GetCommitteeCategoryMasterDD(YearId, ChapterId, ref lstCommitteeCategoryMaster2, ref lstCommitteeCategoryMaster3, ref lstCommitteeCategoryMaster4, ref status);
                if (status == 1)
                {

                    ViewBag.lstCommitteeCategoryMaster = lstCommitteeCategoryMaster;
                    ViewBag.lstCommitteeCategoryMaster2 = lstCommitteeCategoryMaster2;
                    ViewBag.lstCommitteeCategoryMaster3 = lstCommitteeCategoryMaster3;
                    ViewBag.lstCommitteeCategoryMaster4 = lstCommitteeCategoryMaster4;

                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            ViewBag.mid = mid;
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateCommitteeCategoryMaster(Entities.CommitteeCategoryMaster objCommitteeCategoryMaster, Int64 mid = 0, Int64 ChapterId = 0)
        {
            try
            {
                Int64 _status = 0;
                objCommitteeCategoryMaster.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                objCommitteeCategoryMaster.InsertedBy = HttpContext.User.Identity.Name.ToString();
                objCommitteeCategoryMaster.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                objCommitteeCategoryMaster.IsActive = true;

                _status = _CommitteeCategoryMaster.InsertCommitteeCategoryMaster(objCommitteeCategoryMaster);
                if (_status == 1 || _status == 2)
                {
                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully";
                        return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid, ChapterId = ChapterId });

                    }

                    else if (_status == 2)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Committe Cateogry Name was Updated Successfully";
                        return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid, ChapterId = ChapterId });

                    }


                    else
                    {
                        return RedirectToAction("EditCommitteeCategoryMaster", "CommitteeCategoryMaster", new { CommitteeCategoryId = objCommitteeCategoryMaster.CommitteeCategoryId, mid = mid, ChapterId = ChapterId });
                    }
                }
                else
                {
                    ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">Failed " + (_status == 2 ? "updating " : "inserting ") + " Menu Item details.</div>";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return View();
            }




        }


        [Authorize]
        public ActionResult CommitteeCategoryMasterBylist(Int64 YearId = 0, Int64 ChapterId = 0)
        {
            string str = "";
            string message = "";

            try
            {
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4 = new List<Entities.CommitteeCategoryMaster>();
                int status = 0;
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = _CommitteeCategoryMaster.GetCommitteeCategoryMasterDD(YearId, ChapterId, ref lstCommitteeCategoryMaster2, ref lstCommitteeCategoryMaster3, ref lstCommitteeCategoryMaster4, ref status);
                if (status == 1)
                {
                    return Json(new { ok = true, data = lstCommitteeCategoryMaster, data2 = lstCommitteeCategoryMaster2, data3 = lstCommitteeCategoryMaster3, data4 = lstCommitteeCategoryMaster4 });
                }
                else
                {
                    message = "error";
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                message = "error";
                return Json(new { ok = true, data = str });
            }
        }

        [Authorize]
        public ActionResult EditCommitteeCategoryMaster(Int64 CommitteeCategoryId, Int64 mid = 0, Int64 ChapterId = 0)
        {
            try
            {
                int _status = 0;
                int _list = 0;
                int Total = 0;
                ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);
                Entities.CommitteeCategoryMaster objCommitteeCategoryMaster = _CommitteeCategoryMaster.GetCommitteeCategoryMasterById(CommitteeCategoryId, ref _status);
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster2 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster3 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster4 = new List<Entities.CommitteeCategoryMaster>();
                List<Entities.CommitteeCategoryMaster> lstCommitteeCategoryMaster = _CommitteeCategoryMaster.GetCommitteeCategoryMasterDD(mid, ChapterId, ref lstCommitteeCategoryMaster2, ref lstCommitteeCategoryMaster3, ref lstCommitteeCategoryMaster4, ref _list);
                if (_list == 1)
                {
                    ViewBag.lstCommitteeCategoryMaster = lstCommitteeCategoryMaster;
                    ViewBag.lstCommitteeCategoryMaster2 = lstCommitteeCategoryMaster2;
                    ViewBag.lstCommitteeCategoryMaster3 = lstCommitteeCategoryMaster3;
                    ViewBag.lstCommitteeCategoryMaster4 = lstCommitteeCategoryMaster4;
                }
                if (_status == 1)
                {

                    ViewBag.objCommitteeCategoryMaster = objCommitteeCategoryMaster;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid, ChapterId = ChapterId });
                }
                ViewBag.mid = mid;
                ViewBag.ChapterId = ChapterId;
                ViewBag.CommitteeCategoryId = CommitteeCategoryId;

                return View();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditCommitteeCategoryMaster(Entities.CommitteeCategoryMaster objCommitteeCategoryMaster, Int64 mid = 0, Int64 ChapterId = 0)
        {
            try
            {
                Int64 _status = 0;
                objCommitteeCategoryMaster.IsActive = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["masterstatus"]);

                objCommitteeCategoryMaster.InsertedBy = "Admin";
                objCommitteeCategoryMaster.UpdatedBy = "Admin";
                objCommitteeCategoryMaster.UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();
                objCommitteeCategoryMaster.InsertedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime();

                _status = _CommitteeCategoryMaster.InsertCommitteeCategoryMaster(objCommitteeCategoryMaster);
                if (_status == 1 || _status == 2)
                {

                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Record Inserted Successfully";
                        return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid, ChapterId = ChapterId });

                    }

                    else if (_status == 2)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Committe Cateogry Name was Updated Successfully";
                        return RedirectToAction("Index", "CommitteeCategoryMaster", new { mid = mid, ChapterId = ChapterId });

                    }

                    else
                    {
                        return RedirectToAction("Index", "CommitteeCategoryMaster", new { CommitteeCategoryId = objCommitteeCategoryMaster.CommitteeCategoryId, mid = mid, ChapterId = ChapterId });
                    }




                }


                else
                {
                    ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">Failed " + (_status == 2 ? "updating " : "inserting ") + " Menu Item details.</div>";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteCommitteeMember(Int64 CommitteeCategoryId)
        {
            try
            {

                Int64 _status = _CommitteeCategoryMaster.DeleteCommitteeMember(CommitteeCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed deleting page" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed Transaction" });
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult CommitteeCategoryMasterStatus(Int64 CommitteeCategoryId)
        {
            string str = "";
            try
            {

                Int64 _status = _CommitteeCategoryMaster.CommitteeMembersUpdateStatus(CommitteeCategoryId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Status Successfully" });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating status" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed Transaction" });
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