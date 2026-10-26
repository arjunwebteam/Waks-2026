using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Committees,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class CommitteeMemberController : Controller
    {
        BLL.CommitteeMembers _CommitteeMembers = new BLL.CommitteeMembers();
        BLL.Committees _Committees = new BLL.Committees();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Members> lstCommittees = new List<Entities.Members>();
        BLL.CommitteeCategories _CommitteeCategory = new BLL.CommitteeCategories();
        List<Entities.CommitteeCategories> lstCommitteeCategory = new List<Entities.CommitteeCategories>();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.Users _users = new BLL.Users();

        #region Committee Members

        [Authorize]
        public ActionResult Index(Int64 CommitteeCategoryId = 0, Int64 ChapterId = 0, Int64 mid = 0)
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
            int _qstatus1 = 0;
            List<Entities.CommitteeMembers> _lstCommittee = new List<Entities.CommitteeMembers>();
            List<Entities.Chapters> lstLChapters = new List<Entities.Chapters>();
            int status = 0;
            List<Entities.CommitteeMembers> lstCommitteebyedit = new List<Entities.CommitteeMembers>();
 
            int _qstatus = 0;
            int CategoryLevel = 0;

            try
            {
                _lstCommittee = _Committees.GetCommitteeMembersListById(CommitteeCategoryId, chapterId, ref _qstatus);
                lstCommitteeCategory = _CommitteeCategory.GetCommitteeCategoriesList(ref _qstatus);
                lstChapters = _Chapters.GetChaptersList(ref status);

                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
                if (_qstatus != 1)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Committees");
                }
                int Total = 0;
                lstLChapters = _Chapters.GetChaptersList(ref Total);
                lstCommitteeCategory = _CommitteeCategory.GetCommitteeCategoriesList(ref _qstatus1);
                lstCommitteebyedit = _Committees.CommitteesGetListByeditId(CommitteeCategoryId, chapterId, ref _qstatus);
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
                ViewBag.ChapterId = chapterId;
                ViewBag.mid = mid;
            }
            catch
            {
                status = -1;
            }

            // ✅ ViewBag assignments
            ViewBag.ChapterId = chapterId;

            ViewBag.lstCommitteeCategory = lstCommitteeCategory;
            ViewBag.lstCommittee = _lstCommittee;
            ViewBag.lstCommitteebyedit = lstCommitteebyedit;
            ViewBag.CommitteeCategoryId = CommitteeCategoryId;
            ViewBag.lstChapters = lstChapters;
            ViewBag.lstLChapters = lstLChapters;
            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;

            return View();
        }



        [Authorize]
        public ActionResult CommitteeMemberList(Int64 CommitteeCategoryId = 0, Int64 ChapterId = 0, string Search = "", string SortColumn = "DisplayOrder", string SortOrder = "Asc", int PageNo = 1, int Items = 25, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {

            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstCommittees = _CommitteeMembers.CommitteeMembersList(CommitteeCategoryId, ChapterId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstCommittees = lstCommittees;
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
        public ActionResult AddCommitteeMember(Entities.CommitteeMembers objCommitteeMember, Int64 mid = 0)
        {
            try
            {

                Int64 _status = _CommitteeMembers.InsertCommitteeMembers(objCommitteeMember);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Inserted Record Successfully";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId, mid = mid });
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId, mid = mid });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Failed Transaction.";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId, mid = mid });
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = " + ex.Message + ";
                return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId, mid = mid });
            }

        }

        [Authorize]
        public ActionResult EditCommitteeMember(Int64 CommitteeMemberId = 0)
        {
            try
            {

                int _qstatus = 0;
                Entities.CommitteeMembers _objCommitteeMember = _CommitteeMembers.GetCommitteeMemberById(CommitteeMemberId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objCommitteeMember });
                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed Transaction" });
                }
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed Transaction" });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteCommitteeMember(Int64 CommitteeMemberId)
        {
            try
            {

                Int64 _status = _CommitteeMembers.DeleteCommitteeMember(CommitteeMemberId);
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

        [Authorize]
        [HttpPost]
        public JsonResult CommitteeMemberDisplayOrder(int DisplayOrder, Int64 CommitteeMemberId)
        {
            string str = "";
            try
            {
                Int64 _status = _CommitteeMembers.UpdateCommitteeMemberDisplayOrder(DisplayOrder, CommitteeMemberId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });
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

        [HttpPost]
        [Authorize]
        public JsonResult CommitteeStatus(Int64 CommitteeMemberId)
        {
            string str = "";
            try
            {

                Int64 _status = _CommitteeMembers.CommitteeMembersUpdateStatus(CommitteeMemberId);
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
