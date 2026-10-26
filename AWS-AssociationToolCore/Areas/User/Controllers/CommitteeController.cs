using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class CommitteeController : Controller
    {
        BLL.Committees _Committees = new BLL.Committees();
        BLL.CommitteeCategories _CommitteeCategories = new BLL.CommitteeCategories();
        List<Entities.Members> lstCommittees = new List<Entities.Members>();
        BLL.CommitteeMembers _CommitteeMembers = new BLL.CommitteeMembers();
        List<Entities.Members> lstCommitteeMembers = new List<Entities.Members>();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();

        public IActionResult Index(string Year = "", long ChapterId = 0, string cname = "")
        {
            // Set current year

            //Year = DateTime.Now.Year.ToString();
            //string year = BLL.Common.EncodeURL(Year);

            // Read cookie (ASP.NET Core way)
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is JSON, deserialize (recommended)
                // Example assumes simple value (adjust if needed)
                cname = cookieValue;
            }

            int qstatus = 0;

            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters?.ChapterId ?? 0;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            List<Entities.CommitteeCategoryMaster> lstCommitteeCategories = new();
            List<Entities.CommitteeCategories> lstCommitteeCategories1 = new();
            List<Entities.CommitteeCategories> lstCommitteeTypes = new();
            List<Entities.CommitteeCategoryMaster> lstCommitteeCategoriesyears = new();
            Entities.PageDetails objMenuPageDetails = new();

            try
            {
                int status = 0;

                lstCommittees = _Committees.FECommitteesGetList(
                    ref lstCommitteeCategories,
                    ref lstCommitteeMembers,
                    ref lstCommitteeTypes,
                    Year,
                    ChapterId,
                    ref status
                );

                lstCommitteeCategories1 = _CommitteeCategories
                    .GetCommitteeCategoriesListByChapterId(ChapterId, ref status);

                lstCommitteeCategoriesyears = _Committees.Committeeyears(ref status);

                long qmstatus = 0;

                objMenuPageDetails = _PageDetails
                    .GetBreadCrumbListByUrl($"c/{cname}/leadership", ref qmstatus);
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error1 closable\">{ex.Message}</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.lstCommittees = lstCommittees;
            ViewBag.lstCommitteeCategories = lstCommitteeCategories;
            ViewBag.lstCommitteeCategories1 = lstCommitteeCategories1;
            ViewBag.lstCommitteeMembers = lstCommitteeMembers;
            ViewBag.lstCommitteeTypes = lstCommitteeTypes;
            ViewBag.lstCommitteeCategoriesyears = lstCommitteeCategoriesyears;

            ViewBag.Year = Year;
            ViewBag.cname = cname;

            return View();
        }
        public IActionResult Founder(string Year = "", long ChapterId = 0, string cname = "")
        {
            // Set current year

            //Year = DateTime.Now.Year.ToString();
            //string year = BLL.Common.EncodeURL(Year);

            // Read cookie (ASP.NET Core way)
            if (Request.Cookies.TryGetValue("chaperInfo", out string cookieValue))
            {
                // If cookie is JSON, deserialize (recommended)
                // Example assumes simple value (adjust if needed)
                cname = cookieValue;
            }

            int qstatus = 0;

            if (!string.IsNullOrEmpty(cname))
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters?.ChapterId ?? 0;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            List<Entities.CommitteeCategoryMaster> lstCommitteeCategories = new();
            List<Entities.CommitteeCategories> lstCommitteeCategories1 = new();
            List<Entities.CommitteeCategories> lstCommitteeTypes = new();
            List<Entities.CommitteeCategoryMaster> lstCommitteeCategoriesyears = new();
            Entities.PageDetails objMenuPageDetails = new();

            try
            {
                int status = 0;

                lstCommittees = _Committees.FECommitteesGetList(
                    ref lstCommitteeCategories,
                    ref lstCommitteeMembers,
                    ref lstCommitteeTypes,
                    Year,
                    ChapterId,
                    ref status
                );

                lstCommitteeCategories1 = _CommitteeCategories
                    .GetCommitteeCategoriesListByChapterId(ChapterId, ref status);

                lstCommitteeCategoriesyears = _Committees.Committeeyears(ref status);

                long qmstatus = 0;

                objMenuPageDetails = _PageDetails
                    .GetBreadCrumbListByUrl($"c/{cname}/leadership", ref qmstatus);
            }
            catch (Exception ex)
            {
                TempData["message"] = $"<div class=\"error1 closable\">{ex.Message}</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.lstCommittees = lstCommittees;
            ViewBag.lstCommitteeCategories = lstCommitteeCategories;
            ViewBag.lstCommitteeCategories1 = lstCommitteeCategories1;
            ViewBag.lstCommitteeMembers = lstCommitteeMembers;
            ViewBag.lstCommitteeTypes = lstCommitteeTypes;
            ViewBag.lstCommitteeCategoriesyears = lstCommitteeCategoriesyears;

            ViewBag.Year = Year;
            ViewBag.cname = cname;

            return View();
        }

        public ActionResult PastPresidents(string Type = "", string Year = "", string cname = "", Int64 ChapterId = 0)
        {
            List<Entities.CommitteeCategories> lstCommitteeCategories = new List<Entities.CommitteeCategories>();
            List<Entities.CommitteeCategories> lstCommitteeCategories1 = new List<Entities.CommitteeCategories>();
            try
            {
                int qstatus = 0;
                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }

                ChapterId = objChapters.ChapterId;

                if (ChapterId == 0)
                {
                    ChapterId = 1;
                }
                int status = 0;
                lstCommittees = _Committees.FEPastCommitteesGetList(ref lstCommitteeCategories, Type, Year, ChapterId, ref status);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }
            ViewBag.lstCommittees = lstCommittees;
            ViewBag.lstCommitteeCategories = lstCommitteeCategories;
            ViewBag.Type = Type;
            ViewBag.Year = Year;
            return View();
        }

        public ActionResult Committeeinfo(Int64 CommitteeId = 0)
        {
            string str = "";
            try
            {

                int _qstatus = 0;
                Entities.Committees _objCommittees = _Committees.GetCommitteeById(CommitteeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objCommittees });
                }
                else
                {
                    str = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch(Exception ex)
            {
                str = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
            }
        }

        public ActionResult HelpLine(string CType = "", Int64 ChapterId = 0, string cname = "")
        {
            int qstatus = 0;
            if (cname != null && cname != "")
            {
                objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
            }

            ChapterId = objChapters.ChapterId;

            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            string Ctype = BLL.Common.EncodeURL(CType);
            Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();

            try
            {
                int status = 0;
                lstCommittees = _Committees.FEHelplineCommitteesGetList(ref lstCommitteeMembers, CType, ChapterId, ref status);

                Int64 qmstatus = 0;
                objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl("nats-helpline", ref qmstatus);
            }
            catch(Exception ex)
            {
                TempData["message"] = "<div class=\"error1 closable\">" + ex.Message + "</div>";
                return RedirectToAction("Error404", "Error");
            }

            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.lstCommittees = lstCommittees; 
            ViewBag.lstCommitteeMembers = lstCommitteeMembers;
            ViewBag.Type = CType;
            ViewBag.cname = cname;
            return View();
        }

    }
}
