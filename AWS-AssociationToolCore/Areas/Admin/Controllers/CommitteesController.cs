using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
//using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;   
using Microsoft.AspNetCore.Hosting;         
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Models.SessionClass.PermitAccess(Roles = "SuperAdmin,Committees,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class CommitteesController : Controller
    {
        BLL.Committees _Committees = new BLL.Committees();
        List<Entities.Committees> lstCommittees = new List<Entities.Committees>();
        BLL.CommitteeCategories _CommitteeCategory = new BLL.CommitteeCategories();
        List<Entities.CommitteeCategories> lstCommitteeCategory = new List<Entities.CommitteeCategories>();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();

        #region Committees

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult Index(Int64 CommitteeCategoryId = 0, Int64 mid = 0)
        {
            int status = 0;
            Entities.AdminMenuItems objmenuitem = new Entities.AdminMenuItems();
            List<Entities.CommitteeMembers> _lstCommittee = new List<Entities.CommitteeMembers>();
            try
            {

                if (CommitteeCategoryId != 0)
                {
                    int _qstatus = 0;
                   // _lstCommittee = _Committees.GetCommitteeMembersListById(CommitteeCategoryId, ref _qstatus);
                    objmenuitem = _adminmenu.GetAdminMenuItemsById(mid, ref status);
                    if (_qstatus != 1)
                    {
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                        return RedirectToAction("Index", "Committees");
                    }
                    
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.lstCommittee = _lstCommittee;
            ViewBag.CommitteeCategoryId = CommitteeCategoryId;
            ViewBag.IsEdit = objmenuitem.IsEdit;
            ViewBag.IsView = objmenuitem.IsView;
            ViewBag.IsDelete = objmenuitem.IsDelete;
            ViewBag.IsExport = objmenuitem.IsExport;
            ViewBag.IsAdd = objmenuitem.IsQuickLinks;
            ViewBag.mid = mid;
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult CommitteesList(Int64 CommitteeCategoryId = 0 , string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 20, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;

            try
            {
                lstCommittees = _Committees.GetCommitteesListByVariable(CommitteeCategoryId,Search, Sort, PageNo, Items, ref Total);
               
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
        public ActionResult AddCommittee()
        {           
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        //public IActionResult AddCommittee(Entities.Committees objCommittees,IFormFile PhotoFile)   // ✅ Removed Configuration parameter — use _configuration field
        //{
        //    try
        //    {
        //        string imageurl = "NA";
        //        string savedPath = "";

        //        // ✅ Use _configuration field (injected in constructor) NOT Configuration parameter
        //        string uploadPath = _configuration["AppSettings:uploadPath"] ?? "";

        //        // Handle image upload
        //        if (PhotoFile != null && PhotoFile.Length > 0)
        //        {
        //            string ext = Path.GetExtension(PhotoFile.FileName).TrimStart('.');
        //            imageurl = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + ext;

        //            string normalDir = Path.Combine(uploadPath, "committees", "normalphoto");
        //            Directory.CreateDirectory(normalDir);

        //            savedPath = Path.Combine(normalDir, imageurl);

        //            using (var stream = new FileStream(savedPath, FileMode.Create))
        //                PhotoFile.CopyTo(stream);
        //        }

        //        objCommittees.UpdatedBy = HttpContext.User.Identity?.Name ?? "";
        //        objCommittees.IsActive = true;

        //        Int64 _status = _Committees.InsertCommittees(objCommittees, ref imageurl);

        //        switch (_status)
        //        {
        //            case 1:
        //                if (PhotoFile != null && imageurl != "NA")
        //                {
        //                    string thumbDir = Path.Combine(uploadPath, "committees", "thumbphoto");
        //                    Directory.CreateDirectory(thumbDir);
        //                    System.IO.File.Copy(savedPath, Path.Combine(thumbDir, imageurl), true);
        //                }
        //                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" +
        //                                      "Inserted Record Successfully.</div>";
        //                return RedirectToAction("Index", "Committees", new { area = "Admin" });

        //            case 2:
        //                if (PhotoFile != null && imageurl != "NA")
        //                {
        //                    string thumbDir = Path.Combine(uploadPath, "committees", "thumbphoto");
        //                    Directory.CreateDirectory(thumbDir);
        //                    System.IO.File.Copy(savedPath, Path.Combine(thumbDir, imageurl), true);
        //                }
        //                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">" +
        //                                      "Changes have been Updated Successfully.</div>";

        //                return objCommittees.CommitteeCategoryId == 0
        //                    ? RedirectToAction("Index", "Committees", new { area = "Admin" })
        //                    : RedirectToAction("Index", "CommitteeMember",
        //                        new
        //                        {
        //                            area = "Admin",
        //                            CommitteeCategoryId = objCommittees.CommitteeCategoryId
        //                        });

        //            case -1:
        //                TempData["message"] = "<div class=\"error-alert closable\">" +
        //                                      "Failed processing your request.</div>";
        //                if (PhotoFile != null && imageurl != "NA")
        //                {
        //                    string np = Path.Combine(uploadPath, "committees", "normalphoto", imageurl);
        //                    string tp = Path.Combine(uploadPath, "committees", "thumbphoto", imageurl);
        //                    if (System.IO.File.Exists(np)) System.IO.File.Delete(np);
        //                    if (System.IO.File.Exists(tp)) System.IO.File.Delete(tp);
        //                }
        //                return RedirectToAction("AddCommittee", "Committees", new { area = "Admin" });

        //            default:
        //                TempData["message"] = "<div class=\"error-alert closable\">" +
        //                                      "Failed processing your request.</div>";
        //                return RedirectToAction("AddCommittee", "Committees", new { area = "Admin" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
        //                              ex.Message + "</div>";
        //        return RedirectToAction("AddCommittee", "Committees", new { area = "Admin" });
        //    }
        //}
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult EditCommittee(Int64 CommitteeId = 0, Int64 CommitteeCategoryId=0)
        {
            try
            {
               
                int _qstatus = 0;
                Entities.Committees _objCommittees = _Committees.GetCommitteeById(CommitteeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objCommittees = _objCommittees;
                    ViewBag.CommitteeCategoryId = CommitteeCategoryId;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Committees");
                }
            }
            catch 
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Committees");
            }
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ViewCommittee(Int64 CommitteeId = 0)
        {
            try
            {
               
                int _qstatus = 0;
                Entities.Committees _objCommittees = _Committees.GetCommitteeById(CommitteeId, ref _qstatus);

                if (_qstatus == 1)
                {
                    ViewBag.objDetails = _objCommittees;
                    ViewBag.status = _qstatus;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "Committees");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "Committees");
            }
            return View();
        }

        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteCommittee(Int64 CommitteeId,string ImageUrl)
        {
            string str = "";
            try
            {
               
                Int64 _status = _Committees.DeleteCommittee(CommitteeId);
                if (_status == 1)
                {
                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["uploadPath"] + @"\committees\normalphoto\" + ImageUrl))
                    {
                        if (System.IO.File.Exists(ConfigurationManager.AppSettings["uploadPath"] + @"\committees\normalphoto\" + ImageUrl))
                        {
                            System.IO.File.Delete(ConfigurationManager.AppSettings["uploadPath"] + @"\committees\normalphoto\" + ImageUrl);
                            System.IO.File.Delete(ConfigurationManager.AppSettings["uploadPath"] + @"\committees\thumbphoto\" + ImageUrl);
                        }
                    }
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
        public JsonResult CommitteeStatus(Int64 CommitteeId)
        {
            string str = "";
            try
            {
                Int64 _status = _Committees.UpdateCommitteeStatus(CommitteeId);
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
        public JsonResult CommitteeDisplayOrder(int DisplayOrder, Int64 CommitteeId)
        {
            string str = "";
            try
            {
                Int64 _status = _Committees.UpdateCommitteesDisplayOrder(DisplayOrder, CommitteeId);
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
