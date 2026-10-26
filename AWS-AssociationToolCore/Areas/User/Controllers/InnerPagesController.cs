using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    
    public class InnerPagesController : Controller
    {
        BLL.InnerPages _innerpage = new BLL.InnerPages();
       
        Entities.InnerPages objInnerPages = new Entities.InnerPages();
        
        public ActionResult GetPageDetails(string PageTitle = "", Int64 PageId = 0)
        {
            try
            {
                int status = 0;

                objInnerPages = _innerpage.GetInnerPagesListById(PageId, PageTitle,0,0, ref status);
                if (objInnerPages.Heading != null && objInnerPages.Heading != "")
                {
                    ViewBag.InnerPage = objInnerPages;
                }
                else
                {
                    ViewBag.PageTitle = PageTitle;
                }
            }
            catch
            {
                return RedirectToAction("Error404", "Error");
            }
           return View();
        }

    }
}
