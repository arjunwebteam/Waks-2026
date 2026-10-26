using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Committees,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class CommitteeMember2024Controller : Controller
    {
        BLL.CommitteeMembers _CommitteeMembers = new BLL.CommitteeMembers();
        BLL.Committees _Committees = new BLL.Committees();
        BLL.Chapters _Chapters = new BLL.Chapters();
        List<Entities.Members> lstCommittees = new List<Entities.Members>();
        BLL.CommitteeCategories _CommitteeCategory = new BLL.CommitteeCategories();
        List<Entities.CommitteeCategories> lstCommitteeCategory = new List<Entities.CommitteeCategories>();
        List<Entities.Chapters> lstChapters = new List<Entities.Chapters>();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();

        #region Committee Members

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 CommitteeCategoryId = 0, Int64 ChapterId = 0, Int64 mid = 0)
        {
            int _qstatus1 = 0;
            List<Entities.CommitteeMembers> _lstCommittee = new List<Entities.CommitteeMembers>();
            List<Entities.Chapters> lstLChapters = new List<Entities.Chapters>();
            int status = 0;
            Entities.AdminMenuItems objmenuitem = new Entities.AdminMenuItems();
            try
            {
                int _qstatus = 0;
                if (CommitteeCategoryId != 0)
                { 
                    //_lstCommittee = _Committees.GetCommitteeMembersListById(CommitteeCategoryId, ref _qstatus);
                    lstCommitteeCategory= _CommitteeCategory.GetCommitteeCategoriesList(ref _qstatus);
                    lstChapters = _Chapters.GetChaptersListById(CommitteeCategoryId, ref _qstatus);
                    objmenuitem = _adminmenu.GetAdminMenuItemsById(mid, ref status);

                    if (_qstatus != 1)
                    {
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                        return RedirectToAction("Index", "Committees");
                    }
                   
                }
                else
                {
                   // _lstCommittee = _Committees.GetCommitteeMembersListById(CommitteeCategoryId, ref _qstatus);
                }
                int Total = 0;
                lstLChapters = _Chapters.GetChaptersList(ref Total);
                lstCommitteeCategory = _CommitteeCategory.GetCommitteeCategoriesList(ref _qstatus1);
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.lstCommitteeCategory = lstCommitteeCategory;
            ViewBag.lstCommittee = _lstCommittee;
            ViewBag.CommitteeCategoryId = CommitteeCategoryId;
            ViewBag.lstChapters = lstChapters;
            ViewBag.lstLChapters = lstLChapters;
            ViewBag.ChapterId = ChapterId;
            ViewBag.IsEdit = objmenuitem.IsEdit;
            ViewBag.IsView = objmenuitem.IsView;
            ViewBag.IsDelete = objmenuitem.IsDelete;
            ViewBag.IsExport = objmenuitem.IsExport;
            ViewBag.IsAdd = objmenuitem.IsQuickLinks;
            ViewBag.mid = mid;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult CommitteeMemberList(Int64 CommitteeCategoryId = 0, Int64 ChapterId = 0, string Search = "", string SortColumn = "DisplayOrder", string SortOrder = "Asc", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
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

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddCommitteeMember(Entities.CommitteeMembers objCommitteeMember)
        {
            try
            {
               
                Int64 _status = _CommitteeMembers.InsertCommitteeMembers(objCommitteeMember);
                if (_status == 1)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId });
                }
                if (_status == 2)
                {
                    TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId });
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction.</div>";
                    return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId });
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "CommitteeMember", new { CommitteeCategoryId = objCommitteeMember.CommitteeCategoryId });
            }

        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditCommitteeMember(Int64 CommitteeMemberId = 0)
        {
            string str = "";
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
        public JsonResult DeleteCommitteeMember(Int64 CommitteeMemberId)
        {
            string str = "";
            try
            {
               
                Int64 _status = _CommitteeMembers.DeleteCommitteeMember(CommitteeMemberId);
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
        public JsonResult CommitteeMemberDisplayOrder(int DisplayOrder, Int64 CommitteeMemberId)
        {
            string str = "";
            try
            {
                Int64 _status = _CommitteeMembers.UpdateCommitteeMemberDisplayOrder(DisplayOrder, CommitteeMemberId);
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

        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public JsonResult CommitteeStatus(Int64 CommitteeMemberId)
        {
            string str = "";
            try
            {

                Int64 _status = _CommitteeMembers.CommitteeMembersUpdateStatus(CommitteeMemberId);
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

        #endregion       

    }
}
