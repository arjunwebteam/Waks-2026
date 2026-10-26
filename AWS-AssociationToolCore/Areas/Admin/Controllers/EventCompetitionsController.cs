using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Events,Members,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class EventCompetitionsController : Controller
    {
        BLL.EventCompetitions _EventCompetitions = new BLL.EventCompetitions();
        List<Entities.EventCompetitions> lstEventCompetitions = new List<Entities.EventCompetitions>();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.Events _Events = new BLL.Events();

        #region EventCompetitions

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index()
        {
            int _qstatus = 0;
            ViewBag.lstEvents = _Events.GetUpcomingEventsList(ref _qstatus);
            
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EventCompetitionsList(Int64 EventId = 0, string Search = "", string SortColumn = "UpdatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstEventCompetitions = _EventCompetitions.GetEventCompetitionsListByVariable(EventId, Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
         
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstEventCompetitions = lstEventCompetitions;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public ActionResult AddEventCompetitions(Entities.EventCompetitions objEventCompetitions)
        {
            string str = "";
            bool _bool = true;
            try
            {
                objEventCompetitions.UpdatedBy = this.User.Identity.Name;
                objEventCompetitions.InsertedBy = this.User.Identity.Name;
                objEventCompetitions.InsertedDate = DateTime.UtcNow;
                objEventCompetitions.UpdatedDate = DateTime.UtcNow;
                objEventCompetitions.IsActive = true;
                Int64 _status = _EventCompetitions.EventCompetitionsInsert(objEventCompetitions);
                switch (_status)
                {
                    case 1:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                        _bool = true;
                        break;
                    case 2:
                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                        _bool = true;
                        break;
                    case -1:
                        TempData["message"] = "<div class=\"error-alert closable\">Failed processing your request.</div>";
                        _bool = false;
                        break;
                    default:
                        TempData["message"] = "<div class=\"error-alert closable\">Failed processing your request.</div>";
                        _bool = false;
                        break;
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                _bool = false;
            }

            return RedirectToAction("Index", "EventCompetitions");
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditEventCompetitions(Int64 EventCompetitionId = 0)
        {
            string str = "";
            try
            {
                int _qstatus = 0;
                Entities.EventCompetitions _objEventCompetitions = _EventCompetitions.GetEventCompetitionById(EventCompetitionId, ref _qstatus);

                if (_qstatus == 1)
                {
                    return Json(new { ok = true, data = _objEventCompetitions });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
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
        public JsonResult DeleteEventCompetitions(Int64 EventCompetitionId)
        {
            string str = "";
            try
            {
                Int64 _status = _EventCompetitions.DeleteEventCompetitions(EventCompetitionId);
                if (_status == 1)
                {
                    str = "<div class=\"alert alert-success alert-dismissable\">Competition details Deleted Successfully</div>";
                    return Json(new { ok = true, data = str });
                }
                else
                {
                    str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting Competition details</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return Json(new { ok = false, data = str });
            }
        }

        public JsonResult UpdateDisplayOrder(int DisplayOrder, Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _EventCompetitions.UpdateEventCompetitionDisplayOrder(DisplayOrder, EventCompetitionId);
            if (_status == 1)
            {
                str = "<div class=\"alert alert-success alert-dismissable\">Update Order Successfully</div>";
                return Json(new { ok = true, data = str });
            }
            else
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }

        }

        public JsonResult UpdateEventCompetitionstatus(Int64 EventCompetitionId)
        {
            Int64 _status = 0;
            string str = "";
            _status = _EventCompetitions.UpdateEventCompetitionstatus(EventCompetitionId);
            if (_status == 1)
            {
                str = "<div class=\"alert alert-success alert-dismissable\">Update Status Successfully</div>";
                return Json(new { ok = true, data = str });
            }
            else
            {
                str = "<div class=\"alert alert-danger alert-dismissable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }

        }


        #endregion  
    }
}
