using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{


    [Area("User")]
    public class PerformanceCategoryController : Controller
    {
        BLL.PerformanceCategory _PerformanceCategory = new BLL.PerformanceCategory();

        // GET: User/PerformanceCategory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPerformanceCategoryById(Int64 PerformanceCategoryID)
        {
            string str = "";
            try
            {
                int status = 0;
                int _qstatus = 0;
                Entities.PerformanceCategory _objPerformanceCategory = _PerformanceCategory.GetPerformanceCategoryById(PerformanceCategoryID, ref status);

                if (status == 1)
                {
                    return Json(new { ok = true, data = _objPerformanceCategory });
                }
                else
                {
                    str = "<div class=\"error1 closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                str = "<div class=\"error1 closable\">Failed Transaction</div>";
                return Json(new { ok = false, data = str });
            }
        }

    }
}