using AWS_AssociationToolCore;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    //[Models.SessionClass.PermitAccess(Roles = "SuperAdmin,ChapterAdmin,SiteAdmin,Administrator,DeveloperAdmin,")]
    public class PaymentSettingsController : Controller
    {
        BLL.PaymentSettings _PaymentSettings = new AWS_AssociationToolCore.BLL.PaymentSettings();
        Entities.PaymentSettings objPaymentSettings = new AWS_AssociationToolCore.Entities.PaymentSettings();
        BLL.AdminMenuItems _adminmenu = new BLL.AdminMenuItems();
        BLL.PaymentMethods _PaymentMethods = new BLL.PaymentMethods();

        [Authorize]
        public ActionResult Index(Int64 mid = 0)
        {
            int status = 0;
            Entities.AdminMenuItems objmenuitem = new Entities.AdminMenuItems();
            objmenuitem = _adminmenu.GetAdminMenuItemsById(mid, ref status);
            ViewBag.IsEdit = objmenuitem.IsEdit;
            ViewBag.IsView = objmenuitem.IsView;
            ViewBag.IsDelete = objmenuitem.IsDelete;
            ViewBag.IsExport = objmenuitem.IsExport;
            ViewBag.IsAdd = objmenuitem.IsQuickLinks;
            ViewBag.mid = mid;
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult PartialList(string search = "", int PageNo = 1, int PageItems = 15, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            List<Entities.PaymentMethods> lstPaymentMethods = new List<Entities.PaymentMethods>();
            int status = 0;
            int Total = 0;
            List<Entities.PaymentSettings> lstPaymentSettings = new List<Entities.PaymentSettings>();
            try
            {

                lstPaymentSettings = _PaymentSettings.GetPaymentSettingsList(HttpUtility.UrlDecode(search.Trim()), PageNo, PageItems, ref Total);

                lstPaymentMethods = _PaymentMethods.GetPaymentmethodsList(ref status);
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.pageno = PageNo;
            ViewBag.iterms = PageItems;
            ViewBag.total = Total;
            ViewBag.lstPaymentSettings = lstPaymentSettings;
            ViewBag.lstPaymentMethods = lstPaymentMethods;

            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            //ViewBag.items = Items;
            return View();
        }

        [Authorize]
        public ActionResult Add(Int64 PaymentSettingId = 0, Int64 mid = 0)
        {
            try
            {
                int status = 0;
                ViewBag.CurrencyCodesList = _PaymentSettings.GetCurrencyCodesList(ref status);
                ViewBag.PaymentMethodsList = _PaymentSettings.GetPaymentMethodsList(ref status);

                if (PaymentSettingId != 0)
                {
                    AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = _PaymentSettings.GetPaymentSettingsDetails(PaymentSettingId, "", ref status);
                    ViewBag.objPaymentSettings = objPaymentSettings;
                }
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Failed Transaction";
                //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            ViewBag.mid = mid;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult View(Int64 Id)
        {
            string str = "";
            int status = 0;
            try
            {
                AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings = _PaymentSettings.GetPaymentSettingsDetails(Id, "", ref status);
                if (objPaymentSettings != null)
                {
                    return Json(new { ok = true, data = objPaymentSettings });
                }
                else
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed"
                    });
                //{
                //    str = "<div class=\"alert alert-danger alert-dismissable\">Failed</div>";
                //    return Json(new { ok = false, data = str });
                //}

            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction"
                });

            }
        }

        [Authorize]
        public ActionResult UpdateAppInfo(AWS_AssociationToolCore.Entities.PaymentSettings objPaymentSettings, Int64 mid = 0)
        {
            try
            {
                objPaymentSettings.PaymentPassword = "";
                Int64 status = _PaymentSettings.UpdatePaymentSettingsDetails(objPaymentSettings);
                if (status != -1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Changes has been Updated Successfully";
                    //TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Changes has been Updated Successfully</div>";
                }
                else
                {
                    TempData["messageType"] = "warning"; // Assuming "info" for update
                    TempData["message"] = "Failed updating payment settings";
                    //TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed updating payment settings.</div>";
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction"
                });
            }
            return RedirectToAction("Index", "PaymentSettings", new { mid = mid });
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(Int64 Id)
        {
            string str = "";
            try
            {
                Int64 _status = _PaymentSettings.DeletePaymentSettingById(Id);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Record Deleted successfully"
                    });

                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed Deleting the Record"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });
            }
        }


        [HttpPost]
        [Authorize]
        public JsonResult PaymentSettingstatus(Int64 PaymentSettingId)
        {
            string str = "";
            try
            {
                Int64 _status = _PaymentSettings.UpdatePaymentSettingApprovalById(PaymentSettingId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });

                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating user status"
                    });

                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });

            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult PaymentMethodstatus(Int64 PaymentMethodId)
        {
            string str = "";
            try
            {
                Int64 _status = _PaymentSettings.UpdatePaymentMethodstatus(PaymentMethodId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Status Successfully"
                    });

                    //str = "<div class=\"alert alert-success alert-dismissable\">Updated Status Successfully</div>";
                    //return Json(new { ok = true, data = str });
                }
                else
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Failed updating status"
                    });
                    //str = "<div class=\"alert alert-danger alert-dismissable\">Failed updating status</div>";
                    //return Json(new { ok = false, data = str });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });

            }
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

        [HttpPost]
        [Authorize]
        public JsonResult UpdateMode(Int64 PaymentSettingId, string AccountType, Int64 PaymentMethodId)
        {
            string str = "";
            try
            {
                Int64 _status = _PaymentSettings.UpdatePaymentMode(PaymentSettingId, AccountType, PaymentMethodId);
                if (_status == 1)
                {
                    return Json(new
                    {
                        ok = true,
                        messageType = "success",
                        message = "Updated Mode Successfully"
                    });

                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        messageType = "error",
                        message = "Failed updating user Mode"
                    });

                }
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    messageType = "error",
                    message = "Failed transaction."
                });

            }
        }
    }
}
