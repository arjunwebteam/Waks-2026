using AWS_AssociationToolCore.BLL;
using Grpc.Core;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
//using PaymentProcess;
//using PayPal.Payments.Common.Utility;
//using PayPal.Payments.Communication;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;


namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class QRCodeController : Controller
    {
        AWS_AssociationToolCore.DAL.QrCode _QrCodeD = new AWS_AssociationToolCore.DAL.QrCode();
        AWS_AssociationToolCore.BLL.QrCode _QrCode = new AWS_AssociationToolCore.BLL.QrCode();
        BLL.Users _users = new BLL.Users();
        Entities.QrCode objqrcode = new Entities.QrCode();

        // GET: Admin/QRCode

        [Authorize]

        public ActionResult QRCodeIndex(Int64 mid = 0, string rolename = "")
        {
         int Total = 0;

            long userId = 0;

            var userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                long.TryParse(userIdStr, out userId);
            }

            if (Request.Cookies.TryGetValue("UserRole", out string userRoleValue))
            {
                HttpContext.Session.SetString("userrole", userRoleValue);
            }
            long chapterId = 0;

            var chapterValue = HttpContext.Session.GetString("chapterid");

            if (!string.IsNullOrEmpty(chapterValue))
            {
                long.TryParse(chapterValue, out chapterId);
            }

            List<Entities.Chapters> lstChapters = new();
            List<Entities.PageDetails> lstPageDetails = new();
            List<Entities.MenuItems> lstMenuItems = new();
            Entities.UserRoles objuserroles = new();

            int status = 0;
            int _qstatus = 0;
            int CategoryLevel = 0;

            try
            {

                objuserroles = _users.GetRoleDetialsById(userId, mid, ref status);
                // ✅ Session (Core way)
                HttpContext.Session.SetString("IsEdit", objuserroles.IsEdit.ToString());
                HttpContext.Session.SetString("IsView", objuserroles.IsView.ToString());
                HttpContext.Session.SetString("IsDelete", objuserroles.IsDelete.ToString());
                HttpContext.Session.SetString("IsExport", objuserroles.IsExport.ToString());
                HttpContext.Session.SetString("IsAdd", objuserroles.IsAdd.ToString());

                // ✅ ViewBag (optional)
                ViewBag.IsEdit = objuserroles.IsEdit;
                ViewBag.IsView = objuserroles.IsView;
                ViewBag.IsDelete = objuserroles.IsDelete;
                ViewBag.IsExport = objuserroles.IsExport;
                ViewBag.IsAdd = objuserroles.IsAdd;
                ViewBag.mid = mid;

            }
            catch
            {
                status = -1;
            }

            // ✅ ViewBag assignments
            ViewBag.ChapterId = chapterId;

            ViewBag.lstPageDetails = lstPageDetails;
            ViewBag.lstMenuItems = lstMenuItems;
            ViewBag.Mid = mid;
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult QrCodeList(string Search = "", string SortColumn = "InsertedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 10, bool IsEdit = false, bool IsView = false, bool IsDelete = false, bool IsExport = false, bool IsAdd = false, Int64 Mid = 0)
        {
            int Total = 0;
            List<Entities.QrCode> lstQrCode = new List<Entities.QrCode>();
            try
            {
                string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
                lstQrCode = _QrCode.GetQrCodeGetListByVariable(Search, Sort, PageNo, Items, ref Total);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("QRCodeIndex", "QrCode");
            }
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.total = Total;
            ViewBag.lstQrCode = lstQrCode;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder;
            ViewBag.IsEdit = IsEdit;
            ViewBag.IsView = IsView;
            ViewBag.IsDelete = IsDelete;
            ViewBag.IsExport = IsExport;
            ViewBag.IsAdd = IsAdd;
            ViewBag.Mid = Mid;
            return View();
        }

        [HttpPost]  // ✅ Must be HttpPost since AJAX sends POST
        [Authorize]
        public IActionResult GenerateDynamicQR([FromBody] Entities.QrCode objqrcode, Int64 mid = 0)
        // ✅ [FromBody] needed because AJAX sends contentType: 'application/json'
        {
            try
            {
                string base64Image = GenerateQRCodeWithLabel(
                    objqrcode.qrLevel,
                    objqrcode.labelText,
                    objqrcode.pixelSize,
                    out string fileName,
                    objqrcode.RefUrl
                );

                DateTime utcNow = DateTime.UtcNow;
                objqrcode.InsertedDate = utcNow;
                objqrcode.UpdatedDate = utcNow;
                objqrcode.fileName = fileName;

                Int64 _status = _QrCode.InsertQrCode(objqrcode);

                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Inserted Record Successfully.";
                    // ✅ Don't use RedirectToAction inside JsonResult — removed
                }

                return Json(new
                {
                    ok = true,
                    ImagePath = base64Image,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    message = ex.Message
                });
            }
        }
        [Authorize]

        public string GenerateQRCodeWithLabel(string level, string labelText, int pixelSize, out string fileName, string RefUrl)
        {
            QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)Enum.Parse(typeof(QRCodeGenerator.ECCLevel), level);

            string uniqueCode = Guid.NewGuid().ToString("N").Substring(0, 8);
            fileName = $"QR_{uniqueCode}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

            string EmployeeRefUrl = RefUrl;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(EmployeeRefUrl, eccLevel);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrBitmap = qrCode.GetGraphic(pixelSize))
            {
                int canvasWidth = qrBitmap.Width + 40;
                int canvasHeight = qrBitmap.Height + 80;

                using (Bitmap canvas = new Bitmap(canvasWidth, canvasHeight))
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.White);

                    int qrX = 20, qrY = 20;
                    g.DrawImage(qrBitmap, qrX, qrY);

                    Font font = new Font("Arial", 16, FontStyle.Bold);
                    SizeF labelSize = g.MeasureString(labelText, font);
                    float labelX = (canvasWidth - labelSize.Width) / 2;
                    float labelY = qrY + qrBitmap.Height + 10;
                    g.DrawString(labelText, font, Brushes.Black, labelX, labelY);

                    Pen pen = new Pen(Color.Black, 4);
                    g.DrawRectangle(pen, qrX - 5, qrY - 5, qrBitmap.Width + 10, qrBitmap.Height + 10);

                    // ✅ Use AppInfo.UploadPath instead of Server.MapPath
                    int status = 0;
                    BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                    Entities.AppInfo objAppInfo = _appinfoHelper.GetAppInfoDetails(ref status);

                    string outputDir = Path.Combine(objAppInfo.UploadPath, "Content", "QRCodeGeneration");
                    Directory.CreateDirectory(outputDir); // ✅ Create folder if not exists

                    string outputFilePath = Path.Combine(outputDir, fileName);
                    canvas.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    // ✅ Return base64 image
                    using (MemoryStream ms = new MemoryStream())
                    {
                        canvas.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        string base64Image = Convert.ToBase64String(ms.ToArray());
                        return "data:image/png;base64," + base64Image;
                    }
                }
            }
        }
        [HttpGet]
        [Authorize]
        public IActionResult DownloadQRCode(string code)
        {
            try
            {
                string fileName;

                // ✅ Check if code already starts with "QR_" and ends with ".jpg"
                if (code.StartsWith("QR_") && code.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = code;
                }
                else
                {
                    fileName = $"QR_{code}.jpg";
                }

                // ✅ Use AppInfo.UploadPath instead of Server.MapPath
                int status = 0;
                BLL.AppInfo _appinfoHelper = new BLL.AppInfo();
                Entities.AppInfo objAppInfo = _appinfoHelper.GetAppInfoDetails(ref status);

                string filePath = Path.Combine(objAppInfo.UploadPath, "Content", "QRCodeGeneration", fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("QR Code not found.");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                return File(fileBytes, "image/jpeg", fileName);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]

        [Authorize]
        public JsonResult QrCodeStatus(Int64 QrCodeId)
        {
            string str = "";
            try
            {
                Int64 _status = _QrCode.UpdateQrCodeStatus(QrCodeId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Status Successfully" });

                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating status" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });

            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ViewQrCode(Int64 QrCodeId)
        {
            string str = "";
            try
            {
                int status = 0;
                objqrcode = _QrCode.GetQrCodeById(QrCodeId, ref status);
                if (status != -1 && objqrcode != null)
                {
                    ViewBag.objqrcode = objqrcode;
                    return Json(new { ok = true, data = objqrcode });
                }
                else
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Transaction Failed.";
                    return RedirectToAction("QRCodeIndex", "QRCode");
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "error";
                TempData["message"] = ex.Message;
                return RedirectToAction("QRCodeIndex", "QRCode");
            }
        }


        [HttpPost]

        [Authorize]
        public JsonResult QrCodeDelete(Int64 QRCodeId)
        {
            string str = "";
            try
            {
                Int64 _status = _QrCode.DeleteQrCode(QRCodeId);
                if (_status == 1)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Deleted Item Successfully.";
                    return Json(new { ok = true, messageType = "success", message = "Record Deleted Successfully" });

                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed deleting item" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });

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
        public JsonResult QrCodeDisplayOrder(int OrderNo, Int64 QrCodeId)
        {
            string str = "";
            try
            {
                Int64 _status = _QrCode.UpdateQrCodeDisplayOrder(OrderNo, QrCodeId);
                if (_status == 1)
                {
                    return Json(new { ok = true, messageType = "success", message = "Updated Order Successfully" });

                }
                else
                {
                    return Json(new { ok = false, messageType = "error", message = "Failed updating status" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, messageType = "error", message = ex.Message });

            }
        }



    }
}