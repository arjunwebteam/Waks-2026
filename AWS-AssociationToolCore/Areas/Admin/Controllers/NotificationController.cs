using ClosedXML.Excel;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class NotificationController : Controller
    {
        BLL.AppUsers _AppUsers = new BLL.AppUsers();

        #region AppUsers

        public ActionResult Index()
        {

            return View();
        }


        public ActionResult NotificationUsersList(string Search = "", string SortColumn = "UpdatedTime", string SortOrder = "Desc", int PageNo = 1, int Items = 20)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<Entities.AppUsers> lstAppUsers = new List<Entities.AppUsers>();

            try
            {
                lstAppUsers = _AppUsers.GetNotificationListByVariable(Search, Sort, PageNo, Items, ref Total);

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



        public ActionResult NotificationView(Int64 NotificationID = 0)
        {
            string str = "";

            try
            {
                int _status = 0;
                Entities.AppUsers _objAppUsers = _AppUsers.ViewById(NotificationID, ref _status);
                if (_objAppUsers != null)
                {

                    return Json(new { ok = true, data = _objAppUsers });

                }
                else
                {
                    str = "<div class=\"success closable\">Failed Transaction</div>";
                    return Json(new { ok = false, data = str });
                }
            }
            catch (Exception ex)
            {
                str = "<div class=\"error closable\">" + ex.Message + "</div>";
                return Json(new { ok = false, data = str });
            }
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
        public JsonResult DeleteAppUsers(Int64 NotificationID)
        {
            string str = "";
            try
            {

                Int64 _status = _AppUsers.AppNotificationDelete(NotificationID);
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



        [HttpPost]
    
        public async Task<IActionResult> SendNotification(
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
                // ✅ Use AppInfo.UploadPath instead of Session
                int status1 = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfoHelper.GetAppInfoDetails(ref status1);
                string baseUrl = objAppInfo.AdminImageUrl;
                string uploadPath = objAppInfo.UploadPath;

                // ✅ Handle image upload - IFormFile instead of WebImage
                string imageurl = "NA";
                string finalImageUrl = "";

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
                    // ✅ Save thumb using ImageSharp
                        ImageHelper.Resize(imgStream, thumbPath, 130, 130);

                    imageurl = fileName;
                    finalImageUrl = baseUrl + "Notification/NormalImages/" + imageurl;
                }

                objAppUsers.Title = Title;
                objAppUsers.Body = Body;

                // ✅ Get AppUsers list
                int status = 0;
                List<Entities.AppUsers> lstAppUsers = _AppUsers.UsersList(ref status);

                // ✅ Parse PaymentDate to UTC
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

                // ✅ Get MobileAppInfo
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

                if (lstAppUsers != null && lstAppUsers.Count != 0)
                {
                    // ✅ Get device IDs
                    List<string> deviceIds = new List<string>();
                    foreach (var item in lstAppUsers)
                    {
                        if (!string.IsNullOrEmpty(item.DeviceID) && item.DeviceID.Length == 36)
                            deviceIds.Add(item.DeviceID);
                    }
                    objAppUsers.DeviceID = string.Join(",", deviceIds);

                    // ✅ Get AppUser IDs
                    List<long> AppuserIds = new List<long>();
                    foreach (var item in lstAppUsers)
                    {
                        if (item.UserID > 0)
                            AppuserIds.Add(item.UserID);
                    }
                    objAppUsers.AppuserId = string.Join(",", AppuserIds);

                    if (deviceIds.Count > 0)
                    {
                        objAppUsers.ActivityTobeopen = Action;
                        objAppUsers.Values = (Action == "Events" ? Eid :
                                             (Action == "EventList" ? Els :
                                             (Action == "WebViews" ? Wv :
                                             (Action == "Services" ? SS :
                                             (Action == "Leadership" ? LS :
                                             (Action == "SavedEvents" ? SE : ""))))));

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
                                string responseText = await httpResponse.Content.ReadAsStringAsync();
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

            return Json(new
            {
                ok = true,
                messageType = "success",
                message = "Sent notification successfully."
            });
        }

        #endregion



    }
}
