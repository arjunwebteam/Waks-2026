using AWS_AssociationToolCore.Areas.Admin.Models;
using AWS_AssociationToolCore.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Http;         
using Microsoft.AspNetCore.Mvc;          
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace AWS_AssociationToolCore.Areas.Admin.Controllers
{
    //raju today
    //sd
    [Area("Admin")]
    public class AccountController : Controller
    {
        AWS_AssociationToolCore.BLL.Users _user = new AWS_AssociationToolCore.BLL.Users();
        AWS_AssociationToolCore.BLL.Members _Members = new AWS_AssociationToolCore.BLL.Members();
        Entities.Chapters objChapters = new Entities.Chapters();
        BLL.Chapters _Chapters = new BLL.Chapters();
        BLL.AppInfo _AppInfo = new BLL.AppInfo();
        BLL.SendMail _sendmail = new BLL.SendMail();

        public ActionResult LogOn(string str = "")
        {
            
            if (str == "session")
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Session Expired.</div>";
            }
            if (str == "noaccess")
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Admin need to provide role access.</div>";
            }            
            return View();
        }
        public ActionResult App(string str = "")
        {
            
            if (str == "session")
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Session Expired.</div>";
            }
            if (str == "noaccess")
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Admin need to provide role access.</div>";
            }            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> App(LogOnModel model, string returnUrl)
        {
            try
            {
                int _status = 0;
                var objUser = _user.GetAdminUsersGetByEmail(model.Email, ref _status);

                if (objUser != null && objUser.UserId != 0 && objUser.RoleName != "EndUser")
                {
                    if (objUser.IsApproved)
                    {
                        string pwd = BLL.Password.UnEncryptPassword(objUser.Password);

                        if (model.Password.Trim() == pwd)
                        {
                            string UserRole = objUser.RoleName;

                            bool isMobileRequest = IsMobileDevice(Request);

                            if (UserRole.Contains("AppUser") && !isMobileRequest)
                            {
                                // TEMP DEBUG - remove after diagnosing
                                string debugUA = Request.Headers["User-Agent"].ToString();
                                System.IO.File.AppendAllText(
                                    Path.Combine(Path.GetTempPath(), "ua-debug.txt"),
                                    $"{DateTime.Now}: {debugUA}{Environment.NewLine}");

                                TempData["messageType"] = "warning";
                                TempData["message"] = "This account can only be accessed from the mobile app/site.";
                                return View();
                            }

                            // ✅ Create Claims (REPLACES Session + FormsAuthTicket)
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objUser.UserName),
                        new Claim(ClaimTypes.Email, objUser.Email),
                        new Claim("UserId", objUser.UserId.ToString()),
                        new Claim("ChapterId", objUser.ChapterId.ToString()),
                        new Claim(ClaimTypes.Role, objUser.RoleName)
                    };

                            var identity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                            };

                            // ✅ Sign in user (replaces FormsAuthentication + Cookie)
                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                principal,
                                authProperties);

                            // ✅ Role-based redirects (same as your logic)
                            if (UserRole.Contains("AppUser"))
                                return RedirectToAction("Index", "MembersAppUsers", new { mid = 73 });
                            else if (UserRole.Contains("SuperAdmin") || UserRole.Contains("DeveloperAdmin"))
                            {
                                TempData["messageType"] = "warning";
                                TempData["message"] = "Username or Password is incorrect.Please contact admin.";
                                return View();
                            }

                        }
                        else
                        {
                            TempData["messageType"] = "warning";
                            TempData["message"] = "Username or Password is incorrect.";
                        }
                    }
                    else
                    {
                        TempData["messageType"] = "warning";
                        TempData["message"] = "Your status has been deactivated. Please contact admin.";
                    }
                }
                else
                {
                    TempData["messageType"] = "warning";
                    TempData["message"] = "Username or Password is incorrect.";
                }
            }
            catch
            {
                TempData["messageType"] = "warning";
                TempData["message"] = "Failed transaction.";
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogOn(LogOnModel model, string returnUrl)
        {
            try
            {
                int _status = 0;
                var objUser = _user.GetAdminUsersGetByEmail(model.Email, ref _status);

                if (objUser != null && objUser.UserId != 0 && objUser.RoleName != "EndUser")
                {
                    if (objUser.IsApproved)
                    {
                        string pwd = BLL.Password.UnEncryptPassword(objUser.Password);

                        if (model.Password.Trim() == pwd)
                        {
                            // ✅ Create Claims (REPLACES Session + FormsAuthTicket)
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objUser.UserName),
                        new Claim(ClaimTypes.Email, objUser.Email),
                        new Claim("UserId", objUser.UserId.ToString()),
                        new Claim("ChapterId", objUser.ChapterId.ToString()),
                        new Claim(ClaimTypes.Role, objUser.RoleName)
                    };

                            var identity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                            };

                            // ✅ Sign in user (replaces FormsAuthentication + Cookie)
                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                principal,
                                authProperties);

                            string UserRole = objUser.RoleName;

                            // ✅ Role-based redirects (same as your logic)
                            if (UserRole.Contains("SubAdmin"))
                                return RedirectToAction("Registrations", "EventRegistrations");

                            else if (UserRole.Contains("SuperAdmin") || UserRole.Contains("DeveloperAdmin"))
                                return RedirectToAction("Index", "MenuItems", new { mid = 4 });
                            else if (UserRole.Contains("AppUser"))
                            {
                                TempData["messageType"] = "warning";
                                TempData["message"] = "This account can only be accessed from the mobile app/site.";
                                return View();
                            }

                            else if (UserRole.Contains("ChapterAdmin"))
                                return RedirectToAction("Index", "MenuItems");

                            else
                                return RedirectToAction("Profile", "Account");
                        }
                        else
                        {
                            TempData["messageType"] = "warning";
                            TempData["message"] = "Username or Password is incorrect.";
                        }
                    }
                    else
                    {
                        TempData["messageType"] = "warning";
                        TempData["message"] = "Your status has been deactivated. Please contact admin.";
                    }
                }
                else
                {
                    TempData["messageType"] = "warning";
                    TempData["message"] = "Username or Password is incorrect.";
                }
            }
            catch
            {
                TempData["messageType"] = "warning";
                TempData["message"] = "Failed transaction.";
            }

            return View();
        }

        // ✅ Helper: simple mobile device detection via User-Agent
        private bool IsMobileDevice(HttpRequest request)
        {
            string userAgent = request.Headers["User-Agent"].ToString();

            string chMobile = request.Headers["Sec-CH-UA-Mobile"].ToString();
            if (chMobile == "?1")
                return true;

            if (string.IsNullOrEmpty(userAgent))
                return false;

            // Broader keyword list — added "Mobile" (capital, some UAs use full word not "Mobi"),
            // "wv" (Android WebView marker), and vendor-neutral tokens
            string[] mobileKeywords = { "Mobi", "Mobile", "Android", "iPhone", "iPod",
                                  "BlackBerry", "IEMobile", "Opera Mini", "wv" };

            return mobileKeywords.Any(keyword =>
                userAgent.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        //[HttpPost]
        //public async Task<IActionResult> LogOn(LogOnModel model, string returnUrl)
        //{
        //    try
        //    {
        //        int _status = 0;
        //        var objUser = _user.GetAdminUsersGetByEmail(model.Email, ref _status);

        //        if (objUser != null && objUser.UserId != 0 && objUser.RoleName != "EndUser")
        //        {
        //            if (objUser.IsApproved)
        //            {
        //                string pwd = BLL.Password.UnEncryptPassword(objUser.Password);

        //                if (model.Password.Trim() == pwd)
        //                {
        //                    // ✅ Create Claims (REPLACES Session + FormsAuthTicket)
        //                    var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, objUser.UserName),
        //                new Claim(ClaimTypes.Email, objUser.Email),
        //                new Claim("UserId", objUser.UserId.ToString()),
        //                new Claim("ChapterId", objUser.ChapterId.ToString()),
        //                new Claim(ClaimTypes.Role, objUser.RoleName)
        //            };

        //                    var identity = new ClaimsIdentity(
        //                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //                    var principal = new ClaimsPrincipal(identity);

        //                    var authProperties = new AuthenticationProperties
        //                    {
        //                        IsPersistent = model.RememberMe,
        //                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
        //                    };

        //                    // ✅ Sign in user (replaces FormsAuthentication + Cookie)
        //                    await HttpContext.SignInAsync(
        //                        CookieAuthenticationDefaults.AuthenticationScheme,
        //                        principal,
        //                        authProperties);

        //                    string UserRole = objUser.RoleName;

        //                    // ✅ Role-based redirects (same as your logic)
        //                    if (UserRole.Contains("SubAdmin"))
        //                        return RedirectToAction("Registrations", "EventRegistrations");


        //                    else if (UserRole.Contains("SuperAdmin") || UserRole.Contains("DeveloperAdmin"))
        //                        return RedirectToAction("Index", "MenuItems", new { mid = 4 });
        //                    else if (UserRole.Contains("AppUser") )
        //                        return RedirectToAction("Index", "MembersAppUsers", new { mid = 73 });

        //                    else if (UserRole.Contains("ChapterAdmin"))
        //                        return RedirectToAction("Index", "MenuItems");

        //                    else
        //                        return RedirectToAction("Profile", "Account");
        //                }
        //                else
        //                {
        //                    TempData["messageType"] = "warning";
        //                    TempData["message"] = "Username or Password is incorrect.";
        //                }
        //            }
        //            else
        //            {
        //                TempData["messageType"] = "warning";
        //                TempData["message"] = "Your status has been deactivated. Please contact admin.";
        //            }
        //        }
        //        else
        //        {
        //            TempData["messageType"] = "warning";
        //            TempData["message"] = "Username or Password is incorrect.";
        //        }
        //    }
        //    catch
        //    {
        //        TempData["messageType"] = "warning";
        //        TempData["message"] = "Failed transaction.";
        //    }

        //    return View();
        //}
        #region Change Password - App Header Modal (separate from Profile page)

        [Authorize]
        public PartialViewResult ChangePasswordApp()
        {
            return PartialView("ChangePasswordApp", new ChangePasswordModel());
        }

        [Authorize]
        [HttpPost]
        public JsonResult ChangePasswordApp(ChangePasswordModel model)
        {
            try
            {
                string UserEmail = HttpContext.Session.GetString("UserEmail");

                if (string.IsNullOrEmpty(UserEmail))
                    return Json(new { success = false, field = "", message = "Session expired. Please login again." });

                if (string.IsNullOrWhiteSpace(model.OldPassword))
                    return Json(new { success = false, field = "OldPassword", message = "Required." });

                if (string.IsNullOrWhiteSpace(model.NewPassword))
                    return Json(new { success = false, field = "NewPassword", message = "Required." });

                if (model.NewPassword.Length < 5)
                    return Json(new { success = false, field = "NewPassword", message = "At least 5 characters." });

                if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
                    return Json(new { success = false, field = "ConfirmPassword", message = "Required." });

                if (model.NewPassword != model.ConfirmPassword)
                    return Json(new { success = false, field = "ConfirmPassword", message = "Passwords do not match." });

                int _qstatus = 0;
                Entities.Users _objuser = _user.GetUserByUserName(UserEmail, ref _qstatus);
                Entities.Users objUser = _user.GetAdminUsersGetByEmail(_objuser.Email, ref _qstatus);

                if (_qstatus != 1 || objUser == null)
                    return Json(new { success = false, field = "", message = "Unable to verify your account. Please try again." });

                string currentPassword = BLL.Password.UnEncryptPassword(objUser.Password);

                if (model.OldPassword.Trim() != currentPassword)
                    return Json(new { success = false, field = "OldPassword", message = "Old password is incorrect." });

                if (model.NewPassword == model.OldPassword)
                    return Json(new { success = false, field = "NewPassword", message = "New password cannot be the same as old password." });

                string encryptedPassword = BLL.Password.EncryptPassword(model.NewPassword);
                Int64 _pstatus = _user.ChangePassword(UserEmail, encryptedPassword);

                return _pstatus == 1
                    ? Json(new { success = true, field = "", message = "Password changed successfully." })
                    : Json(new { success = false, field = "", message = "Failed to update password. Please try again." });
            }
            catch
            {
                return Json(new { success = false, field = "", message = "Failed transaction." });
            }
        }
       
        
   
        #endregion
        public async Task<IActionResult> LogOff()
        {
            // ✅ Sign out user (removes auth cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ Optional: clear session if you are using it
            HttpContext.Session.Clear();

            return RedirectToAction("LogOn", "Account");
        }
        public async Task<IActionResult> LogOffApp()
        {
            // ✅ Sign out user (removes auth cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ Optional: clear session if you are using it
            HttpContext.Session.Clear();

            return RedirectToAction("App", "Account");
        }

        #region Profile

        public ActionResult ChangePassword()
        {
            int _qstatus = 0;
            Entities.Users _objuser = new Entities.Users();
            try
            {
                _objuser = _user.GetUserByUserName(HttpContext.User.Identity.Name.ToString(), ref _qstatus);
               
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }

            ViewBag.objuser = _objuser;
            return View();
        }

        [Authorize]
        [HttpPost]
        [Areas.Admin.Models.SessionClass.SessionExpireFilter]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                string UserEmail = HttpContext.Session.GetString("UserEmail");
                if (UserEmail != "")
                {
                    int _qstatus = 0;
                    string UserEmails = HttpContext.Session.GetString("UserEmail");

                    string oldpass = _user.GetPassword(UserEmails, ref _qstatus);

                    if (_qstatus == 1)
                    {
                        //if (AWS_AssociationToolCore.BLL.Password.VerifyHash(model.OldPassword.Trim(), "SHA512", oldpass) == true)
                        //{
                        //string UserEmail = HttpContext.Session.GetString("UserEmail");
                        string UserEmaill = HttpContext.Session.GetString("UserEmail");

                        string _password = model.NewPassword;
                        string encryptedPassword = BLL.Password.EncryptPassword(_password);
                        //string newpass = AWS_AssociationToolCore.BLL.Password.ComputeHash(model.NewPassword, "SHA512", null);
                        Int64 _pstatus = _user.ChangePassword(UserEmaill, encryptedPassword);

                        if (_pstatus == 1)
                        {
                            TempData["messageType"] = "success";
                            TempData["message"] = "Password Changed Successfully.";
                        }
                        else
                        {
                            TempData["messageType"] = "warning";
                            TempData["message"] = "The current password is incorrect or the new password is invalid.";

                        }
                        //}
                        //else
                        //{
                        //    TempData["messageType"] = "warning";
                        //    TempData["message"] = "The current password is incorrect or the new password is invalid.";
                        //}
                    }
                    else
                    {
                        TempData["messageType"] = "warning";
                        TempData["message"] = "The current password is incorrect or the new password is invalid.";
                    }
                }
                return RedirectToAction("Profile", "Account");
            }
            catch
            {

                TempData["messageType"] = "warning";
                TempData["message"] = "Failed transaction.";

                return RedirectToAction("Profile", "Account");
            }

        }
        [Authorize]
        public ActionResult Profile()
        {
            try
            {
                string UserEmail = HttpContext.Session.GetString("UserEmail");

                int _qstatus = 0;

         
                Entities.Users _objuser = _user.GetUserByUserName(UserEmail, ref _qstatus);
                Entities.Users objUser = _user.GetAdminUsersGetByEmail(_objuser.Email, ref _qstatus);
                string pwd = BLL.Password.UnEncryptPassword(objUser.Password);
                if (_qstatus == 1)
                {
                    ViewBag.objuser = _objuser;
                    ViewBag.pwd = pwd;
                }
                else
                {

                    TempData["messageType"] = "warning";
                    TempData["message"] = "Failed transaction.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch 
            {

                TempData["messageType"] = "warning";
                TempData["message"] = "Failed transaction.";
            }

            return View();
        }

        [HttpPost]
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> ProfilePic(IFormFile file, long UserId)
        {
            try
            {
                int status = 0;
                var objappinfo = _AppInfo.GetAppInfoDetails(ref status);

                if (file != null && file.Length > 0)
                {
                    // ✅ Generate unique file name
                    string fileExtension = Path.GetExtension(file.FileName);
                    string imageName = Guid.NewGuid().ToString() + fileExtension;

                    // ✅ Save path
                    string folderPath = Path.Combine(objappinfo.UploadPath, "UserProfileImages");
                    string filePath = Path.Combine(folderPath, imageName);

                    // ✅ Ensure directory exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // ✅ Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // ✅ Update DB
                    long _status = _user.UpdateUserProfileImage(UserId, ref imageName);

                    if (_status == 1)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "Changed Your Profile Picture Successfully.";
                    }
                    else
                    {
                        System.IO.File.Delete(filePath);

                        TempData["messageType"] = "danger";
                        TempData["message"] = "Failed uploading image.";
                    }
                }
            }
            catch
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Failed transaction.";
            }

            return RedirectToAction("Profile", "Account");
        }
        #endregion

        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult AppForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AppForgotPassword(ForgotPasswordModel model)
        {
            BLL.SendMail _sentmail = new BLL.SendMail();
            try
            {
                int _status = 0;
                Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref _status);
                Entities.Users _objuser = new Entities.Users();
                _objuser = _user.GetUserByEmail(model.Email, ref _status);

                if (_objuser.UserId == 0 && _objuser.Email != model.Email)
                {
                    ViewBag.Message = "<div class=\"alert alert-danger alert-dismissable\">User Name (or) Email is not valid.</div>";
                }
                //string _password = BLL.Password.GetUniqueKey(8);
                //string _passwordhash = BLL.Password.ComputeHash(_password, "SHA512", null);

                string _password = BLL.Password.GetUniqueKey(8);
                string encryptedPassword = BLL.Password.EncryptPassword(_password);


                Int64 _pstatus = _user.ChangePassword(_objuser.UserId, encryptedPassword);
                if (_pstatus == 1)
                {
                    StringBuilder body = new StringBuilder();

                    body.Append("<p>Dear " + _objuser.UserName + ",<br /><br />");

                    body.Append("You have requested a new password for your account. ");
                    body.Append("Please find your new password below.<br /><br />");

                    body.Append("<b>New Password: " + _password + "</b><br /><br />");

                    body.Append("Please use this new password to log in to the mobile app.<br /><br />");

                    body.Append("Thank You,<br />Admin</p>");

                    _sentmail.SendMailSendinbrevo(
                        model.Email,
                        "Your New Password - Mobile App Login",
                        body.ToString()
                    );
                    TempData["messageType"] = "success";
                    TempData["message"] = "Password Details Sent to Email Id Registered.";



                }
                else
                {

                    TempData["messageType"] = "warning";
                    TempData["message"] = "The email address you entered does not exist. Please check and try again.";

                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            BLL.SendMail _sentmail = new BLL.SendMail();
            try
            {
                int _status = 0;
                Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref _status);
                Entities.Users _objuser = new Entities.Users();
                _objuser = _user.GetUserByEmail(model.Email, ref _status);

                if (_objuser.UserId == 0 && _objuser.Email != model.Email)
                {
                    ViewBag.Message = "<div class=\"alert alert-danger alert-dismissable\">User Name (or) Email is not valid.</div>";
                }
                //string _password = BLL.Password.GetUniqueKey(8);
                //string _passwordhash = BLL.Password.ComputeHash(_password, "SHA512", null);

                string _password = BLL.Password.GetUniqueKey(8);
                string encryptedPassword = BLL.Password.EncryptPassword(_password);


                Int64 _pstatus = _user.ChangePassword(_objuser.UserId, encryptedPassword);
                if (_pstatus == 1)
                {
                    StringBuilder body = new StringBuilder();
                    body.Append("<p>Dear " + _objuser.UserName + ", <br /><br />You have requested password retrieve. Please find the login details below. <br />");
                    body.Append("<br />Password: " + _password + " <br /><br /><a href=\"" + objappinfo.AdminSiteUrl + "\">Click here to Login.</a><br /><br />");
                    body.Append("Thank You,<br />Admin</p>");
                    //_sendmail.SendMailSendinbrevo(model.Email, "Password Details From Admin Team", body.ToString());
                    _sentmail.SendMailSendinbrevo(model.Email, "Password Details From Admin Team", body.ToString());
                    TempData["messageType"] = "success";
                    TempData["message"] = "Password Details Sent to Email Id Registered.";



                }
                else
                {
                
                    TempData["messageType"] = "warning";
                    TempData["message"] = "The email address you entered does not exist. Please check and try again.";

                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" + ex.Message + "</div>";
            }
            return View();
        }
        public ActionResult UserValidate(string name, string id = "")
        {
            try
            {
                int _status = 0;
                Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref _status);
                Entities.Users objuser = _user.GetUserByEmail(name, ref _status);
                if (objuser.UserName != null)
                {
                    if (_status != -1 && objuser != null)
                    {
                        if (id == "reactivate")
                        {
                            Guid guid = AWS_AssociationToolCore.BLL.Common.generateGUID();
                            objuser.IsActivated = false;
                            objuser.RegistrationGUID = guid;
                            Int64 _guidStatus = _user.UpdateRegistrationGUID(objuser.UserId, "false", guid);
                            if (_guidStatus != -1)
                            {
                                StringBuilder body = new StringBuilder();
                                body.Append("<p>Dear " + objuser.UserName + ", <br /><br />Request for reactivation is accepted, please find the activation link <a href=\"" + objappinfo.BaseUrl + "Admin/Account/UserValidate?name=" + objuser.Email + "&id=" + guid.ToString() + "\">here</a>. <br />");
                                body.Append("Thank You,<br />Admin</p>");
                                _sendmail.SendMailSendinbrevo(objuser.Email, "Account Reactivation Link - " + objappinfo.SiteName, body.ToString());

                                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Activation details sent to mail.</div>";
                            }
                            else
                            {
                                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed reactivating account.</div>";
                            }
                        }
                        else
                        {
                            if (!objuser.IsActivated && objuser.RegistrationGUID != Guid.Empty)
                            {
                                if (objuser.RegistrationGUID.ToString() == id)
                                {
                                    string _password = BLL.Password.GetUniqueKey(8);
                                    string encryptedPassword = BLL.Password.EncryptPassword(_password);
                                    //string _hashpassword = BLL.Password.ComputeHash(_password, "SHA512", null);


                                    Int64 _passStatus = _user.ChangePassword(name, encryptedPassword);
                                    if (_passStatus != -1)
                                    {
                                        Int64 _guidStatus = _user.UpdateRegistrationGUID(objuser.UserId, "true", Guid.Empty);

                                        StringBuilder body = new StringBuilder();
                                        body.Append("<p>Dear " + objuser.UserName + ", <br /><br />Your account is activated and password is reset, please find the details below. <br />");
                                        body.Append("<br />Email: " + objuser.Email + "<br />Password: " + _password + " <br /><br /><a href=\"" + objappinfo.BaseUrl + "Admin/Account/Logon\">Click here to Login.</a><br /><br />");
                                        body.Append("Thank You,<br />Admin</p>");
                                        _sendmail.SendMailSendinbrevo(objuser.Email, "Activation Details From Admin Team", body.ToString());
                                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Hi " + objuser.UserName + ", <br/> Your account is activated successfully. Further details are posted to mail registered.</div>";
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Activation link is not valid. Please try <a href=\"UserValidate?name=" + objuser.UserName + "&id=reactivate\">reactivating</a> account.</div>";
                                }
                            }
                            else
                            {
                                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">User is already activated. <br/>Please click here to <a class=\"red-t\" href='Admin/Account/LogOn'>login</a></div>";
                            }
                        }
                    }
                    else
                    {
                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Invalid activation link. Please try again from mail.</div>";
                    }
                }
                else
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Your account has been removed. So please contact to admin.</div>";
                }
            }
            catch
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
            }
            return View();
        }

        //public ActionResult UserValidate(string name, string id = "")
        //{
        //    try
        //    {
        //        int _status = 0;
        //        Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref _status);
        //        Entities.Users objuser = _user.GetUserByEmail(name, ref _status);
        //        if (objuser.UserName != null)
        //        {
        //            if (_status != -1 && objuser != null)
        //            {
        //                if (id == "reactivate")
        //                {
        //                    Guid guid = AWS_AssociationToolCore.BLL.Common.generateGUID();
        //                    objuser.IsActivated = false;
        //                    objuser.RegistrationGUID = guid;
        //                    Int64 _guidStatus = _user.UpdateRegistrationGUID(objuser.UserId, "false", guid);
        //                    if (_guidStatus != -1)
        //                    {
        //                        StringBuilder body = new StringBuilder();
        //                        body.Append("<p>Dear " + objuser.UserName + ", <br /><br />Request for reactivation is accepted, please find the activation link <a href=\"" + objappinfo.BaseUrl + "Admin/Account/UserValidate?name=" + objuser.Email + "&id=" + guid.ToString() + "\">here</a>. <br />");
        //                        body.Append("Thank You,<br />Admin</p>");
        //                        //_sendmail.SendMailSendinbrevo(objuser.Email, "Account Reactivation Link - " + objappinfo.SiteName, body.ToString());

        //                        TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Activation details sent to mail.</div>";
        //                    }
        //                    else
        //                    {
        //                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed reactivating account.</div>";
        //                    }
        //                }
        //                else
        //                {
        //                    if (!objuser.IsActivated && objuser.RegistrationGUID != Guid.Empty)
        //                    {
        //                        if (objuser.RegistrationGUID.ToString() == id)
        //                        {
        //                            string _password = BLL.Password.GetUniqueKey(8);
        //                            string encryptedPassword = BLL.Password.EncryptPassword(_password);
        //                            //string _hashpassword = BLL.Password.ComputeHash(_password, "SHA512", null);


        //                            Int64 _passStatus = _user.ChangePassword(name, encryptedPassword);
        //                            if (_passStatus != -1)
        //                            {
        //                                Int64 _guidStatus = _user.UpdateRegistrationGUID(objuser.UserId, "true", Guid.Empty);

        //                                StringBuilder body = new StringBuilder();
        //                                body.Append("<p>Dear " + objuser.UserName + ", <br /><br />Your account is activated and password is reset, please find the details below. <br />");
        //                                body.Append("<br />Email: " + objuser.Email + "<br />Password: " + _password + " <br /><br /><a href=\"" + objappinfo.BaseUrl + "Admin/Account/Logon\">Click here to Login.</a><br /><br />");
        //                                body.Append("Thank You,<br />Admin</p>");
        //                                _sendmail.SendMailSendinbrevo(objuser.Email, "Activation Details From Admin Team", body.ToString());
        //                                TempData["message"] = "<div class=\"alert alert-success alert-dismissable\">Hi " + objuser.UserName + ", <br/> Your account is activated successfully. Further details are posted to mail registered.</div>";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Activation link is not valid. Please try <a href=\"UserValidate?name=" + objuser.UserName + "&id=reactivate\">reactivating</a> account.</div>";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">User is already activated. <br/>Please click here to <a class=\"red-t\" href='Admin/Account/LogOn'>login</a></div>";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Invalid activation link. Please try again from mail.</div>";
        //            }
        //        }
        //        else
        //        {
        //            TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Your account has been removed. So please contact to admin.</div>";
        //        }
        //    }
        //    catch 
        //    {
        //        TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>";
        //    }
        //    return View();
        //}

        //[Authorize]
        [HttpPost]
        public JsonResult CheckProfileEmailAvailability(Int64 MemberId, string Email)
        {
            try
            {
                int _status = 0;
                
                Entities.Members objMembers = _Members.GetMemberFullDetailsByEmail(Email, ref _status);
               
                bool data = (objMembers.MemberId == MemberId || objMembers.MemberId == 0 ? true : false);

                return Json(new { ok = true, data = data, message = "" });
            }
            catch 
            {
                return Json(new { ok = false, message = "<div class=\"alert alert-danger alert-dismissable\">Failed transaction.</div>" });
            }
        }

        #region CAPTCHA

        #region CAPTCHA

        // ✅ GET - Returns Captcha Image
        public IActionResult ShowCaptchaImage()
        {
            string code = GetRandomText();

            // ✅ Core Session - SetString instead of Session["key"]
            HttpContext.Session.SetString("captchastring", code);

            using (Bitmap bitmap = new Bitmap(200, 60, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (MemoryStream ms = new MemoryStream())
            {
                using (Pen pen = new Pen(Color.Yellow))
                using (SolidBrush blue = new SolidBrush(Color.CornflowerBlue))
                using (SolidBrush black = new SolidBrush(Color.Black))
                {
                    Rectangle rect = new Rectangle(0, 0, 200, 60);
                    g.FillRectangle(blue, rect);
                    g.DrawRectangle(pen, rect);

                    int counter = 0;
                    foreach (var c in code)
                    {
                        using (Font font = new Font("Tahoma",
                            15 + _rand.Next(5, 15), FontStyle.Italic))
                        {
                            g.DrawString(c.ToString(), font, black,
                                new PointF(10 + counter, 10));
                        }
                        counter += 28;
                    }
                    DrawRandomLines(g);
                }

                bitmap.Save(ms, ImageFormat.Gif);
                return File(ms.ToArray(), "image/gif");
            }
        }

        // ✅ POST - Get Captcha string from Session
        [HttpPost]
        public JsonResult GetCaptcha()
        {
            try
            {
                // ✅ Core Session - GetString instead of Session["key"]
                string str = HttpContext.Session.GetString("captchastring");

                if (string.IsNullOrEmpty(str))
                    return Json(new
                    {
                        ok = false,
                        message = "<div class=\"alert alert-danger " +
                                  "alert-dismissable\">Session expired.</div>"
                    });

                return Json(new { ok = true, data = str, message = "" });
            }
            catch
            {
                return Json(new
                {
                    ok = false,
                    message = "<div class=\"alert alert-danger " +
                              "alert-dismissable\">Failed transaction.</div>"
                });
            }
        }

        // ✅ Helper - Random lines noise
        private static Random _rand = new Random();

        private void DrawRandomLines(Graphics g)
        {
            using (Pen yellowPen = new Pen(Color.Yellow, 1))
            {
                for (int i = 0; i < 20; i++)
                {
                    g.DrawLines(yellowPen, GetRandomPoints());
                }
            }
        }

        private Point[] GetRandomPoints()
        {
            return new Point[]
            {
        new Point(_rand.Next(0, 200), _rand.Next(0, 60)),
        new Point(_rand.Next(0, 200), _rand.Next(0, 60))
            };
        }

        private string GetRandomText()
        {
            const string chars =
                "0123456789ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";
            char[] result = new char[6];
            for (int i = 0; i < 6; i++)
                result[i] = chars[_rand.Next(chars.Length)];
            return new string(result);
        }

        #endregion


        [HttpPost]
        public async Task<IActionResult> AdminAuth(LogOnModel model, string returnUrl)
        {
            try
            {
                int _status = 0;

                // Get app info and user details
                Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref _status);
                Entities.Users objUser = _user.GetAdminUsersGetByEmail(model.Email, ref _status);

                // Validate user exists
                if (objUser == null || objUser.UserId == 0)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Username or Password is incorrect.</div>";
                    return View("LogOn");
                }

                // Get chapter info
                int qstatus = 0;
                Int64 ChapterId = objUser.ChapterId;
                string cname = "";

                if (ChapterId != 0)
                {
                    objChapters = _Chapters.GetChaptersById(ChapterId, ref qstatus);
                    cname = objChapters?.ChapterName ?? "";
                }

                if (ChapterId == 0)
                    ChapterId = 1;

                // Check role — EndUser not allowed
                if (objUser.RoleName == "EndUser")
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Username or Password is incorrect.</div>";
                    return View("LogOn");
                }

                // Check if approved
                if (!objUser.IsApproved)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Your status has been deactivated. Please contact admin.</div>";
                    return View("LogOn");
                }

                // Check email matches
                if (model.Email != objUser.Email)
                {
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Username or Password is incorrect.</div>";
                    return View("LogOn");
                }

                // ✅ Set Session values (replaces old Session["key"] = value)
                HttpContext.Session.SetString("username", objUser.UserName);
                HttpContext.Session.SetString("userrole", objUser.RoleName);
                HttpContext.Session.SetString("chapterid", objUser.ChapterId.ToString());
                HttpContext.Session.SetString("ChapterName", cname);

                // ✅ Create Claims (replaces FormsAuthenticationTicket)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,           objUser.UserName),
            new Claim(ClaimTypes.Email,          objUser.Email),
            new Claim(ClaimTypes.Role,           objUser.RoleName),
            new Claim("UserId",                  objUser.UserId.ToString()),
            new Claim("ChapterId",               objUser.ChapterId.ToString()),
            new Claim("ChapterName",             cname)
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                };

                // ✅ Sign In (replaces FormsAuthentication.Encrypt + HttpCookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // ✅ Role-based redirect
                string UserRole = objUser.RoleName;

                if (UserRole.Contains("SubAdmin"))
                    return RedirectToAction("Registrations", "EventRegistrations",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("SuperAdmin"))
                    return RedirectToAction("Index", "MenuItems",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("ChapterAdmin"))
                    return RedirectToAction("Index", "MenuItems",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("SiteAdmin"))
                    return RedirectToAction("Index", "MenuItems",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Volunteers"))
                    return RedirectToAction("Index", "Volunteers",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Events"))
                    return RedirectToAction("Index", "Events",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Enquiry"))
                    return RedirectToAction("Index", "Enquiry",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("PhotoGallery"))
                    return RedirectToAction("Index", "PhotoGallery",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("VideoGallery"))
                    return RedirectToAction("Index", "VideoGallery",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Committees"))
                    return RedirectToAction("Index", "Committees",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Youth"))
                    return RedirectToAction("Index", "Youth",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Members"))
                    return RedirectToAction("Index", "Members",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Sponsors"))
                    return RedirectToAction("Index", "Sponsors",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("ThemeBanners"))
                    return RedirectToAction("Index", "WebsiteBanners",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Chapters"))
                    return RedirectToAction("Index", "Chapters",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Services"))
                    return RedirectToAction("Index", "Services",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Donations"))
                    return RedirectToAction("Index", "Donations",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Sponsorships"))
                    return RedirectToAction("Index", "Sponsorships",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("PatrikaRegistrations"))
                    return RedirectToAction("Index", "PatrikaRegistrations",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("News"))
                    return RedirectToAction("Index", "News",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Treasurer"))
                    return RedirectToAction("ServiceDonors", "ServiceDonations",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("Helpline"))
                    return RedirectToAction("Index", "Services",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else if (UserRole.Contains("ProgramsManager"))
                    return RedirectToAction("Index", "Events",
                        new { area = "Admin", ChapterId = objUser.ChapterId });

                else
                {
                    // No role access
                    TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                          "Admin needs to provide role access.</div>";

                    // ✅ Redirect using configured BaseUrl
                    string url = objappinfo.BaseUrl + "Admin/Account/LogOn?str=noaccess";
                    return Redirect(url);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger alert-dismissable\">" +
                                      "Failed transaction.</div>";
            }

            return View("LogOn");
        }
        #endregion
        public ActionResult Unauthorized()
        {
            return View();
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

        public class CustomAuthorizeAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var user = context.HttpContext.User;

                // ✅ Check authentication (instead of cookie manually)
                if (!user.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToActionResult(
                        "Login", "Account", new { area = "Admin" });
                }

                base.OnActionExecuting(context);
            }
        }
    }
}
