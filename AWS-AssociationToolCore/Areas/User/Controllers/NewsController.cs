using AWS_AssociationToolCore.BLL;
using AWS_AssociationToolCore.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PaymentProcess;
using Stripe;
using Stripe.Checkout;

using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Square;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Braintree;
namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class NewsController : Controller
    {
        BLL.News _News = new BLL.News();
        BLL.Users _users = new BLL.Users();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.PageDetails _PageDetails = new BLL.PageDetails();

        public ActionResult Index(Int64 nid = 0, Int64 ChapterId = 0, string cname = "", string Type = "")
        {
            int qstatus = 0;
            if (nid == 0)
            {
                if (cname != null && cname != "")
                {
                    objChapters = _Chapters.GetChaptersListByName(cname, ref qstatus);
                }
                ChapterId = objChapters.ChapterId;
            }
            
            if (ChapterId == 0)
            {
                ChapterId = 1;
            }

            Entities.PageDetails objMenuPageDetails = new Entities.PageDetails();
            Int64 qmstatus = 0;
            objMenuPageDetails = _PageDetails.GetBreadCrumbListByUrl(cname + "/news", ref qmstatus);

            ViewBag.objMenuPageDetails = objMenuPageDetails;
            ViewBag.NewsId = nid;
            ViewBag.ChapterId = ChapterId;
            ViewBag.cname = cname;
            ViewBag.Type = Type;
            return View();
        }

        public ActionResult NewsList(Int64 NewsId = 0, string Type = "", Int64 ChapterId = 0, string Search = "", string SortColumn = "OrderNo", string SortOrder = "ASC", int PageNo = 1, int Items = 15)
        {
            int status = 0;
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            try
            {
                ViewBag.NewsList = _News.FENewsGetListByVariableByChapterId(NewsId, Type, ChapterId, Search, Sort, PageNo, Items, ref status);
            }
            catch
            {
                status = -1;
            }
            ViewBag.PageNo = PageNo;
            ViewBag.Items = Items;
            ViewBag.Total = status;
            ViewBag.NewsId = NewsId;
            ViewBag.ChapterId = ChapterId;
            ViewBag.Type = Type;
            return View();
        }

    }
}
