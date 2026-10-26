using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Committees,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class CommitteeMasterController : Controller
    {
        BLL.CommitteeCategories _CommitteeCategory = new BLL.CommitteeCategories();
        List<Entities.CommitteeCategories> lstCommitteeCategory = new List<Entities.CommitteeCategories>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.Users _users = new BLL.Users();

        #region CommitteeCategories

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public IActionResult Index(Int64 ChapterId = 0, Int64 mid = 0)
        {
            int status = 0;
            int Total = 0;
            Entities.UserRoles objuserroles = new Entities.UserRoles();

            // ✅ Get UserId from Claims — replaces Session["UserId"]
            var userIdClaim = User.FindFirst("UserId");
            Int64 UserId = userIdClaim != null ? Convert.ToInt64(userIdClaim.Value) : 0;

            // ✅ Get ChapterId from Claims if not passed — replaces Session["chapterid"]
            if (ChapterId == 0)
            {
                var chapterClaim = User.FindFirst("ChapterId");
                ChapterId = chapterClaim != null ? Convert.ToInt64(chapterClaim.Value) : 0;
            }

            // Get chapters list
            ViewBag.lstChapters = _Chapters.GetChaptersList(ref Total);

            // Get user role details
            objuserroles = _users.GetRoleDetialsById(UserId, mid, ref status);

            // ✅ Store in Session — replaces Session["IsEdit"] = value
            HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
            HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
            HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
            HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
            HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

            // ✅ Pass directly to ViewBag — no need to read back from Session
            ViewBag.IsEdit = objuserroles.IsEdit;
            ViewBag.IsView = objuserroles.IsView;
            ViewBag.IsDelete = objuserroles.IsDelete;
            ViewBag.IsExport = objuserroles.IsExport;
            ViewBag.IsAdd = objuserroles.IsAdd;

            ViewBag.ChapterId = ChapterId;
            ViewBag.mid = mid;

            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult CommitteeCategoriesList(string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "ASC", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstCommitteeCategory = _CommitteeCategory.GetCommitteeCategoriesListByVariable(Search, Sort, PageNo, Items, ref Total);
                
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstCommitteeCategory = lstCommitteeCategory;
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

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddCommitteeCategory(Entities.CommitteeCategories objCommitteeCategory)
        {
            string str = "";
            bool _bool = true;
            try
            {
               
                //objCommitteeCategory.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                Int64 _status = _CommitteeCategory.InsertCommitteeCategories(objCommitteeCategory);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return RedirectToAction("Index", "CommitteeMaster", new { CommitteeCategoryId = objCommitteeCategory.CommitteeCategoryId });
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "CommitteeMaster", new { CommitteeCategoryId = objCommitteeCategory.CommitteeCategoryId });
                }

               
            }
            catch 
            {
              
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction.</div>";

                return RedirectToAction("Index", "CommitteeMaster", new { CommitteeCategoryId = objCommitteeCategory. CommitteeCategoryId });
            }

           

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditCommitteeCategory(Int64 CommitteeCategoryId = 0)
        {
            string str = "";
            try
            {
               
                int _qstatus = 0;
                Entities.CommitteeCategories _objCommitteeCategory = _CommitteeCategory.GetCommitteeCategoryById(CommitteeCategoryId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objCommitteeCategory, data1 = _objCommitteeCategory.lstChapterCommittees, data2 = _objCommitteeCategory.lstChapters });
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

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteCommitteeCategory(Int64 CommitteeCategoryId)
        {
            string str = "";
            try
            {
               
                Int64 _status = _CommitteeCategory.DeleteCommitteeCategory(CommitteeCategoryId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting page</div>";
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
        public JsonResult CommitteeStatus(Int64 CommitteeCategoryId)
        {
            string str = "";
            try
            {
               
                Int64 _status = _CommitteeCategory.UpdateCommitteeCategoryStatus(CommitteeCategoryId);
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

        [HttpPost]
        public JsonResult CommitteeCategoryDisplayOrder(int DisplayOrder, Int64 CommitteeCategoryId)
        {
            string str = "";
            try
            {
                Int64 _status = _CommitteeCategory.UpdateCommitteeCategoriesDisplayOrder(DisplayOrder, CommitteeCategoryId);
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

        #endregion  

    }
}
