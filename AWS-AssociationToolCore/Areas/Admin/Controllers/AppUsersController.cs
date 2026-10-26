using ClosedXML.Excel;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nancy.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
//using System.Web.Helpers;
//using System.Web.Mvc;
//using System.Web.Script.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;



namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AppUsersController : Controller
    {
        BLL.AppUsers _AppUsers = new BLL.AppUsers();

        #region AppUsers

        public ActionResult Index()
        {

            return View();
        }


        public ActionResult AppUsersList(string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.AppUsers> lstAppUsers = new List<Entities.AppUsers>();

            try
            {
                lstAppUsers = _AppUsers.GetAppUsersListByVariable(Search, Sort, PageNo, Items, ref Total);

            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstAppUsers = lstAppUsers;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }


        public ActionResult AdminAddAppUsers()
        {

            return View();
        }


        [HttpPost]
        public ActionResult AddAppUsers(Entities.AppUsers objAppUsers)
        {
            try
            {
                objAppUsers.UpdatedBy = HttpContext.User.Identity.Name.ToString();
                // objAppUsers.Status = true;

                Int64 _status = _AppUsers.AdminInsertAppUser(objAppUsers);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Record Inserted Successfully";
                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Inserted Record Successfully</div>";
                    return RedirectToAction("Index", "AppUsers");
                }
                if (_status == 2)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";
                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                    return RedirectToAction("EditAppUsers", "AppUsers", new { UserID = objAppUsers.UserID });
                }
            }
            catch
            {
                TempData["messageType"] = "warning";
                TempData["message"] = "Uploading Failed";
                // TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("AddAppUsers", "AppUsers");
            }
            return View();
        }


        public ActionResult EditAppUsers(Int64 UserID = 0)
        {
            try
            {

                int _status = 0;
                Entities.AppUsers _objAppUsers = _AppUsers.GetAppUsersById(UserID, ref _status);
                if (_status == 1)
                {
                    ViewBag.objAppUsers = _objAppUsers;

                    ViewBag.status = _status;
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "AppUsers");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "AppUsers");
            }
            return View();
        }


        public ActionResult ViewAppUsers(Int64 UserID = 0)
        {
            int _qstatus = 0;
            Entities.AppUsers _objAppUsers = new Entities.AppUsers();
            try
            {
                _objAppUsers = _AppUsers.GetAppUsersById(UserID, ref _qstatus);

                if (_qstatus != 1)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                    return RedirectToAction("Index", "AppUsers");
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                return RedirectToAction("Index", "AppUsers");
            }

            ViewBag.objAppUsers = _objAppUsers;
            ViewBag.status = _qstatus;
            return View();
        }


        [HttpPost]
        public JsonResult DeleteAppUsers(Int64 UserID)
        {
            string str = "";
            try
            {

                Int64 _status = _AppUsers.AppUsersDelete(UserID);
                if (_status == 1)
                {
                    // str = "<div class=\"alert alert-success alert-dismissable\">Record Deleted Successfully</div>";
                    //  return Json(new { ok = true, data = str });
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Recored Deleted successfully"
                    });


                }
                else
                {
                    // str = "<div class=\"alert alert-danger alert-dismissable\">Failed deleting page</div>";
                    // return Json(new { ok = false, data = str });
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting"
                    });
                }
            }
            catch
            {
                // str = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
                // return Json(new { ok = false, data = str });
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed Deleting"
                });
            }
        }

        [HttpPost]
        public JsonResult UpdateAppUserstatus(Int64 UserID)
        {
            string str = "";
            try
            {
                Int64 _status = _AppUsers.UpdateAppUserstatus(UserID);
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
                return Json(new { ok = true, data = str });
            }
        }



        //[HttpPost]
        //public JsonResult SendNotification(string AppuserIds = "", string Title = "", string Body = "", string Action = "", string Eid = "", string Els = "", string Wv = "", string SS = "", string LS = "", string SE = "", string PaymentDate = "", HttpPostedFileBase BannerUrl=null)
        //{


        //    var image = WebImage.GetImageFromRequest();
        //    string imageurl = (image != null ? image.ImageFormat : "NA");
        //    string response = "";
        //    Entities.AppUsers objAppUsers = new Entities.AppUsers();
        //    try
        //    {
        //        int status = 0;
        //        objAppUsers.AppuserId = AppuserIds;
        //        objAppUsers.Title = Title;
        //        objAppUsers.Body = Body;
        //        // objAppUsers.PaymentDate = PaymentDate;


        //        List<Entities.AppUsers> lstAppUsers = new List<Entities.AppUsers>();
        //        lstAppUsers = _AppUsers.GetAppUsersList(ref status, ref AppuserIds);





        //        string isoUtc = "";
        //        if (!string.IsNullOrEmpty(PaymentDate))
        //        {
        //            DateTime localTime = DateTime.ParseExact(
        //                PaymentDate,
        //                "MM/dd/yyyy HH:mm",
        //                System.Globalization.CultureInfo.InvariantCulture
        //            );

        //            // Convert local (IST) to UTC
        //            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, istZone);

        //            isoUtc = utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        //        }



        //        string serverKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"]; // Something very long
        //        string senderId = System.Configuration.ConfigurationManager.AppSettings["SenderId"];
        //        string appId = System.Configuration.ConfigurationManager.AppSettings["NotificationAppId"];
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        if (lstAppUsers != null && lstAppUsers.Count != 0)
        //        {
        //            List<string> deviceIds = new List<string>();
        //            List<string> Ids = new List<string>();

        //            foreach (var item in lstAppUsers)
        //            {
        //                if (!string.IsNullOrEmpty(item.DeviceID) && item.DeviceID.Length == 36)
        //                {
        //                    deviceIds.Add(item.DeviceID);


        //                }

        //            }
        //            string deviceIdsString = string.Join(",", deviceIds);

        //            objAppUsers.DeviceID = deviceIdsString;


        //            if (deviceIds.Count > 0)
        //            {
        //                var request = WebRequest.Create("https://api.onesignal.com/notifications?c=push") as HttpWebRequest;

        //                request.KeepAlive = true;
        //                request.Method = "POST";
        //                request.ContentType = "application/json; charset=utf-8";

        //                // 🔐 Add Authorization Header (important)
        //                request.Headers.Add("Authorization", "Basic " + serverKey);

        //                var serializer1 = new JavaScriptSerializer();
        //                var obj = new
        //                {
        //                    app_id = appId,
        //                    small_icon = "ic_stat_name",
        //                    include_subscription_ids = deviceIds.ToArray(),

        //                    // send_after =_objWebinars.StartDate.ToString("yyyy-MMM-dd,  hh:mm tt"),
        //                    content_available = "true",
        //                    data = new { activityToBeOpened = Action, value = (Action == "Events" ? Eid : (Action == "EventList" ? Els : (Action == "WebViews" ? Wv : (Action == "Services" ? SS : (Action == "Leadership" ? LS : (Action == "SavedEvents" ? SE : "")))))) },
        //                    contents = new { en = Body },
        //                    headings = new { en = Title },
        //                    send_after = isoUtc,
        //                    android_channel_id = "bf862f96-47eb-4385-a72a-cdea11535762"
        //                };
        //                objAppUsers.ActivityTobeopen = Action;
        //                objAppUsers.Values = (Action == "Events" ? Eid : (Action == "EventList" ? Els : (Action == "WebViews" ? Wv : (Action == "Services" ? SS : (Action == "Leadership" ? LS : (Action == "SavedEvents" ? SE : ""))))));


        //                Int64 _status = _AppUsers.InsertAppUserNotifications(objAppUsers, ref imageurl);


        //                    if (imageurl != "")
        //                    {
        //                        image.Save(ConfigurationManager.AppSettings["uploadPath"] + "\\Notification\\NormalImages\\" + imageurl);

        //                        image.Resize(130, 130, true, false);
        //                        image.Crop(1, 1, 1, 1);
        //                        image.Save(ConfigurationManager.AppSettings["uploadPath"] + "\\Notification\\ThumbImages\\" + imageurl);
        //                    }

        //                    var param = serializer1.Serialize(obj);
        //                byte[] byteArray1 = Encoding.UTF8.GetBytes(param);

        //                string responseContent = null;

        //                try
        //                {
        //                    using (var writer = request.GetRequestStream())
        //                    {
        //                        writer.Write(byteArray1, 0, byteArray1.Length);
        //                    }

        //                    using (var response1 = request.GetResponse() as HttpWebResponse)
        //                    {
        //                        using (var reader = new StreamReader(response1.GetResponseStream()))
        //                        {
        //                            responseContent = reader.ReadToEnd();
        //                        }
        //                    }
        //                }

        //                catch (WebException ex)
        //                {
        //                    return Json(new { ok = false, data = ex.Message });
        //                }


        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = ex.Message;
        //        return Json(new { ok = false, data = response });
        //    }
        //    //  return Json(new { ok = true, data = "<div class=\"success closable\">Sent notification successfully.</div>" });
        //    return Json(new
        //    {
        //        ok = true,
        //        messageType = "success",
        //        message = "Sent notification successfully."
        //    });

        //}




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendNotification(
            string AppuserIds = "",
            string Title = "",
            string Body = "",
            string Action = "",
            string Eid = "",
            string Els = "",
            string Wv = "",
            string SS = "",
            string LS = "",
            string SE = "",
            string PaymentDate = "",
            IFormFile BannerUrl = null)  // ✅ IFormFile instead of HttpPostedFileBase
        {
            string response = "";
            Entities.AppUsers objAppUsers = new Entities.AppUsers();

            try
            {
                // ✅ Get AppInfo
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfoHelper.GetAppInfoDetails(ref status1);
                string baseUrl = objAppInfo.AdminImageUrl;
                string uploadPath = objAppInfo.UploadPath;

                // ✅ Get MobileAppInfo
                int status = 0;
                List<Entities.MobileAppInfo> lstMobileAppInfo = _AppUsers.APIMobileAppInfoGetList(ref status);
                string serverKey = "";
                string appId = "";
                string Androidchannelid = "";

                foreach (var item in lstMobileAppInfo)
                {
                    serverKey = item.ServerKey;
                    appId = item.NotificationAppId;
                    Androidchannelid = item.Androidchannelid;
                }

                // ✅ Handle image upload - IFormFile instead of WebImage
                string imageurl = "NA";
                string finalImageUrl = "";
                byte[] imageBytes = null;

                if (BannerUrl != null && BannerUrl.Length > 0)
                {
                    string fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{Path.GetExtension(BannerUrl.FileName)}";
                    string normalDir = Path.Combine(uploadPath, "Notification", "NormalImages");
                    string thumbDir = Path.Combine(uploadPath, "Notification", "ThumbImages");
                    Directory.CreateDirectory(normalDir);
                    Directory.CreateDirectory(thumbDir);

                    string normalPath = Path.Combine(normalDir, fileName);
                    string thumbPath = Path.Combine(thumbDir, fileName);

                    // ✅ Save normal image
                    using (var stream = new FileStream(normalPath, FileMode.Create))
                    {
                        BannerUrl.CopyTo(stream);
                    }

                    // ✅ Save thumb using ImageSharp
                    using (var imgStream = BannerUrl.OpenReadStream())
                        ImageHelper.Resize(imgStream, thumbPath, 130, 130);


                    imageurl = fileName;
                    finalImageUrl = baseUrl + "Notification/NormalImages/" + imageurl;
                }

                objAppUsers.AppuserId = AppuserIds;
                objAppUsers.Title = Title;
                objAppUsers.Body = Body;

                // ✅ Get AppUsers list
                List<Entities.AppUsers> lstAppUsers = _AppUsers.GetAppUsersList(ref status, ref AppuserIds);

                // ✅ Parse Payment Date to UTC
                string isoUtc = "";
                if (!string.IsNullOrEmpty(PaymentDate))
                {
                    DateTime localTime = DateTime.ParseExact(
                        PaymentDate,
                        "MM/dd/yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                    TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, istZone);
                    isoUtc = utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (lstAppUsers != null && lstAppUsers.Count != 0)
                {
                    List<string> deviceIds = new List<string>();

                    foreach (var item in lstAppUsers)
                    {
                        if (!string.IsNullOrEmpty(item.DeviceID) && item.DeviceID.Length == 36)
                            deviceIds.Add(item.DeviceID);
                    }

                    objAppUsers.DeviceID = string.Join(",", deviceIds);

                    if (deviceIds.Count > 0)
                    {
                        // ✅ Save notification to DB
                        Int64 _status = _AppUsers.InsertAppUserNotifications(objAppUsers, ref imageurl);

                        // ✅ Build OneSignal payload
                        var payload = new
                        {
                            app_id = appId,
                            small_icon = "ic_stat_name",
                            include_subscription_ids = deviceIds.ToArray(),
                            content_available = "true",
                            data = new
                            {
                                activityToBeOpened = Action,
                                value = (Action == "Events" ? Eid :
                                        (Action == "EventList" ? Els :
                                        (Action == "WebViews" ? Wv :
                                        (Action == "Services" ? SS :
                                        (Action == "Leadership" ? LS :
                                        (Action == "SavedEvents" ? SE : ""))))))
                            },
                            contents = new { en = Body },
                            headings = new { en = Title },
                            send_after = isoUtc,
                            android_channel_id = Androidchannelid,
                            big_picture = finalImageUrl,
                            ios_attachments = new { id = finalImageUrl }
                        };

                        // ✅ Use HttpClient instead of WebRequest
                        string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + serverKey);

                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            try
                            {
                                var httpResponse = await client.PostAsync(
                                    "https://api.onesignal.com/notifications?c=push", content);

                                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                            }
                            catch (Exception ex)
                            {
                                return Json(new { ok = false, data = ex.Message });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
                return Json(new { ok = false, data = response });
            }

            return Json(!string.IsNullOrEmpty(PaymentDate)
                ? new { ok = true, messageType = "success", message = "Notification Scheduled Successfully." }
                : new { ok = true, messageType = "success", message = "Sent Notification Successfully." }
            );
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













        #endregion



    }
}
