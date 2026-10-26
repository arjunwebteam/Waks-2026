using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AWS_AssociationToolCore.BLL
{
    public class SendMail
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private object _AppInfo;

        public SendMail(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly AWS_AssociationToolCore.BLL.AppInfo _AppInfo;
  
        public SendMail()
        {
            _AppInfo = new AWS_AssociationToolCore.BLL.AppInfo();
        }
        public string SendMailSendinbrevomuiltiplemeail(string toEmail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);

            string senderEmail = objappinfo.SenderEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;

            string filename = "Log_callstatus-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFolder = System.IO.Path.Combine(baseDirectory, "Content", "logfiles");
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }
            string filepath = System.IO.Path.Combine(logFolder, filename);
            string requestPath = "/SendMail";
            string mailResponse = "";

            try
            {
                // ✅ Split comma-separated emails into individual recipient objects
                var toList = toEmail
                    .Split(',')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Select(e => new
                    {
                        name = recipientName,
                        email = e
                    })
                    .ToArray();

                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", objappinfo.BrevoKey);

                    var emailBody = new
                    {
                        sender = new
                        {
                            name = senderName,
                            email = senderEmail
                        },
                        to = toList,   // ✅ now an array with one object per email
                        htmlContent = bodyContent,
                        subject = subject,
                        replyTo = new
                        {
                            email = senderEmail,
                            name = senderName
                        },
                        tags = new[] { "tag1", "tag2" }
                    };

                    var body = JsonConvert.SerializeObject(emailBody);
                    var content = new System.Net.Http.StringContent(
                        body,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var response = httpClient.PostAsync(
                        "https://api.brevo.com/v3/smtp/email", content
                    ).Result;

                    mailResponse = response.Content.ReadAsStringAsync().Result;
                    logreport(
                        "Brevo API Status: " + response.StatusCode + ", Response: " + mailResponse,
                        requestPath,
                        filepath
                    );
                }
            }
            catch (Exception ex)
            {
                mailResponse = "Error: " + ex.Message;
                logreport(
                    "Brevo API FAILED. Error: " + ex.Message,
                    requestPath,
                    filepath
                );
            }

            return mailResponse;
        }
        public string SendMailSendinbrevomuiltiple(string toEmail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);
            string senderEmail = objappinfo.SenderEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;

            // ✅ ASP.NET Core - No HttpContext.Current, use Path.Combine instead
            string filename = "Log_callstatus-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            // ✅ Get base directory for ASP.NET Core
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFolder = System.IO.Path.Combine(baseDirectory, "Content", "logfiles");
            // ✅ Create folder if it doesn't exist
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }
            string filepath = System.IO.Path.Combine(logFolder, filename);
            string requestPath = "/SendMail"; // ✅ Static path since no HttpContext here
            string mailResponse = "";

            try
            {
                // ✅ toEmail can contain multiple comma-separated addresses - split into recipient objects
                var toList = toEmail
                    .Split(',')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Select(e => new
                    {
                        name = recipientName,
                        email = e
                    })
                    .ToArray();

                // ✅ HttpClient - works perfectly in ASP.NET Core
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", objappinfo.BrevoKey);
                    var emailBody = new
                    {
                        sender = new
                        {
                            name = senderName,
                            email = senderEmail
                        },
                        to = toList,   // ✅ array with one object per email
                        htmlContent = bodyContent,
                        subject = subject,
                        replyTo = new
                        {
                            email = senderEmail,
                            name = senderName
                        },
                        tags = new[] { "tag1", "tag2" }
                    };
                    var body = JsonConvert.SerializeObject(emailBody);
                    var content = new System.Net.Http.StringContent(
                        body,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );
                    var response = httpClient.PostAsync(
                        "https://api.brevo.com/v3/smtp/email", content
                    ).Result;
                    mailResponse = response.Content.ReadAsStringAsync().Result;
                    logreport(
                        "Brevo API Status: " + response.StatusCode + ", Response: " + mailResponse,
                        requestPath,
                        filepath
                    );
                }
            }
            catch (Exception ex)
            {
                mailResponse = "Error: " + ex.Message;
                logreport(
                    "Brevo API FAILED. Error: " + ex.Message,
                    requestPath,
                    filepath
                );
            }
            return mailResponse;
        }



        public string SendMailSendinbrevo(string toEmail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);
            string senderEmail = objappinfo.SenderEmail;
            string to = toEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;
            // ✅ ASP.NET Core - No HttpContext.Current, use Path.Combine instead
            string filename = "Log_callstatus-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            // ✅ Get base directory for ASP.NET Core
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFolder = System.IO.Path.Combine(baseDirectory, "Content", "logfiles");
            // ✅ Create folder if it doesn't exist
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }
            string filepath = System.IO.Path.Combine(logFolder, filename);
            string requestPath = "/SendMail"; // ✅ Static path since no HttpContext here
            string mailResponse = "";
            try
            {
                // ✅ HttpClient - works perfectly in ASP.NET Core
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", objappinfo.BrevoKey);
                    var emailBody = new
                    {
                        sender = new
                        {
                            name = senderName,
                            email = senderEmail
                        },
                        to = new[]
                        {
                    new
                    {
                        name = recipientName,
                        email = to
                    }
                },
                        htmlContent = bodyContent,
                        subject = subject,
                        replyTo = new
                        {
                            email = senderEmail,
                            name = senderName
                        },
                        tags = new[] { "tag1", "tag2" }
                    };
                    var body = JsonConvert.SerializeObject(emailBody);
                    var content = new System.Net.Http.StringContent(
                        body,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );
                    var response = httpClient.PostAsync(
                        "https://api.brevo.com/v3/smtp/email", content
                    ).Result;
                    mailResponse = response.Content.ReadAsStringAsync().Result;

                    // NEW: include last 6 chars of API key + sender + recipient to identify which Brevo account/domain this hit
                    string keySuffix = !string.IsNullOrEmpty(objappinfo.BrevoKey) && objappinfo.BrevoKey.Length >= 6
                        ? objappinfo.BrevoKey.Substring(objappinfo.BrevoKey.Length - 6)
                        : "N/A";

                    logreport(
                        "Brevo API Status: " + response.StatusCode +
                        ", Response: " + mailResponse +
                        ", SenderEmail: " + senderEmail +
                        ", To: " + to +
                        ", ApiKeyEndingIn: " + keySuffix,
                        requestPath,
                        filepath
                    );
                }
            }
            catch (Exception ex)
            {
                mailResponse = "Error: " + ex.Message;
                logreport(
                    "Brevo API FAILED. Error: " + ex.Message,
                    requestPath,
                    filepath
                );
            }
            return mailResponse;
        }


        public async Task<string> SendMailSendinbrevoWithFromAsync(string toEmail, string fromMail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objAppInfo = _AppInfo.GetAppInfoDetails(ref status);

            string senderEmail = objAppInfo.SenderEmail;
            string senderName = objAppInfo.MailName;
            string brevoKey = objAppInfo.BrevoKey;

            var emailBody = new
            {
                sender = new { name = senderName, email = senderEmail },
                from = new { name = senderName, email = fromMail },
                to = new[] { new { name = senderName, email = toEmail } },
                subject = subject,
                htmlContent = bodyContent,
                replyTo = new { email = senderEmail, name = senderName },
                tags = new[] { "tag1", "tag2" }
            };

            string jsonBody = JsonConvert.SerializeObject(emailBody);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("api-key", brevoKey);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient
                .PostAsync("https://api.brevo.com/v3/smtp/email", content);

            string mailResponse = await response.Content.ReadAsStringAsync();

            // Log to file
            await LogBrevoResponseAsync(response.StatusCode.ToString(), mailResponse);

            return mailResponse;
        }

        private async Task LogBrevoResponseAsync(string statusCode, string responseContent)
        {
            try
            {
                string filename = $"Log_callstatus-{DateTime.Now:dd-MM-yyyy}.txt";
                string logDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Content", "logfiles");
                string filepath = Path.Combine(logDirectory, filename);

                Directory.CreateDirectory(logDirectory); // creates if not exists

                string logMessage = $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] " +
                                    $"API call to Brevo completed. " +
                                    $"Status: {statusCode}, " +
                                    $"Response: {responseContent}{Environment.NewLine}";

                await File.AppendAllTextAsync(filepath, logMessage);
            }
            catch (Exception ex)
            {
                // Handle or rethrow logging errors as needed
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }
        public string SendMailSendinbrevowithfromusermuiltiple(string toEmail, string fromMail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);
            string senderEmail = objappinfo.SenderEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;
            string filename = "Log_callstatus-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            string logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "logfiles");
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }
            string filepath = System.IO.Path.Combine(logFolder, filename);
            string requestPath = "/SendMailWithFrom";
            string mailResponse = "";

            try
            {
                // ✅ Split comma-separated emails into individual recipient objects
                var toList = toEmail
                    .Split(',')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Select(e => new
                    {
                        name = recipientName,
                        email = e
                    })
                    .ToArray();

                // ✅ ContactEmail can contain multiple comma-separated addresses.
                // replyTo only accepts ONE email, so pick the first valid one, fallback to senderEmail.
                string replyToEmail = senderEmail; // safe default
                if (!string.IsNullOrWhiteSpace(fromMail))
                {
                    var candidate = fromMail
                        .Split(',')
                        .Select(e => e.Trim())
                        .FirstOrDefault(e => IsValidEmail(e));

                    if (!string.IsNullOrEmpty(candidate))
                    {
                        replyToEmail = candidate;
                    }
                }

                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", objappinfo.BrevoKey);

                    var emailBody = new
                    {
                        sender = new
                        {
                            name = senderName,
                            email = senderEmail
                        },
                        to = toList,   // ✅ array of recipient objects
                        htmlContent = bodyContent,
                        subject = subject,
                        replyTo = new
                        {
                            email = replyToEmail,   // ✅ guaranteed single valid email
                            name = senderName
                        },
                        tags = new[] { "tag1", "tag2" }
                    };

                    var body = JsonConvert.SerializeObject(emailBody);
                    var content = new System.Net.Http.StringContent(
                        body,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var response = httpClient.PostAsync(
                        "https://api.brevo.com/v3/smtp/email", content
                    ).Result;

                    mailResponse = response.Content.ReadAsStringAsync().Result;
                    logreport(
                        "Brevo API Status: " + response.StatusCode + ", Response: " + mailResponse,
                        requestPath,
                        filepath
                    );
                }
            }
            catch (Exception ex)
            {
                mailResponse = "Error: " + ex.Message;
                logreport(
                    "Brevo API FAILED. Error: " + ex.Message,
                    requestPath,
                    filepath
                );
            }

            return mailResponse;
        }

        // ✅ Simple email format validator - place this alongside the method (same class)
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string SendMailSendinbrevowithfrom(string toEmail, string fromMail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);

            string senderEmail = objappinfo.SenderEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;

            // ✅ Log file setup - no HttpContext.Current needed
            string filename = "Log_callstatus-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            string logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "logfiles");
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }
            string filepath = System.IO.Path.Combine(logFolder, filename);
            string requestPath = "/SendMailWithFrom";

            string mailResponse = "";

            try
            {
                // ✅ Use HttpClient - works in ALL .NET versions, no RestSharp issues
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", objappinfo.BrevoKey);

                    var emailBody = new
                    {
                        sender = new
                        {
                            name = senderName,
                            email = senderEmail
                        },
                        // ✅ fromMail parameter used here
                        from = new
                        {
                            name = senderName,
                            email = fromMail
                        },
                        to = new[]
                        {
                    new
                    {
                        name = recipientName,
                        email = toEmail
                    }
                },
                        htmlContent = bodyContent,
                        subject = subject,
                        replyTo = new
                        {
                            email = fromMail,  // ✅ reply goes to fromMail
                            name = senderName
                        },
                        tags = new[] { "tag1", "tag2" }
                    };

                    var body = JsonConvert.SerializeObject(emailBody);
                    var content = new System.Net.Http.StringContent(
                        body,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // ✅ Post to Brevo API
                    var response = httpClient.PostAsync(
                        "https://api.brevo.com/v3/smtp/email", content
                    ).Result;

                    mailResponse = response.Content.ReadAsStringAsync().Result;

                    // ✅ Log success - using pre-built filepath
                    logreport(
                        "Brevo API Status: " + response.StatusCode + ", Response: " + mailResponse,
                        requestPath,
                        filepath
                    );
                }
            }
            catch (Exception ex)
            {
                mailResponse = "Error: " + ex.Message;

                // ✅ Log failure
                logreport(
                    "Brevo API FAILED. Error: " + ex.Message,
                    requestPath,
                    filepath
                );
            }

            return mailResponse;
        }

        public void logreport(string error = "", string pageName = "", string filepath = "")
        {

            if (System.IO.File.Exists(filepath))
            {
                using (StreamWriter stwriter = new StreamWriter(filepath, true))
                {
                    stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                    stwriter.WriteLine("Page :" + pageName);
                    stwriter.WriteLine(error);
                    stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                }
            }
            else
            {
                StreamWriter stwriter = System.IO.File.CreateText(filepath);
                stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                stwriter.WriteLine("Page :" + pageName);
                stwriter.WriteLine(error);
                stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                stwriter.Close();
            }
        }



        //public string SendMailSendinbrevowithfrom(string toEmail, string fromMail, string subject, string bodyContent)
        //{
        ////    int status = 0;
        ////    Entities.AppInfo objappinfo = new Entities.AppInfo();
        ////    objappinfo = _AppInfo.GetAppInfoDetails(ref status);

        ////    string senderEmail = objappinfo.SenderEmail;
        ////    String to = toEmail;
        ////    string senderName = objappinfo.MailName;
        ////    string recipientName = objappinfo.MailName;

        ////    var client = new RestClient("https://api.brevo.com/v3/smtp/email");
        ////    //client.Timeout = -1;

        ////    var request = new RestRequest(Method.POST);
        ////    request.AddHeader("Content-Type", "application/json");
        ////    request.AddHeader("Accept", "application/json");
        ////    request.AddHeader("api-key", objappinfo.BrevoKey);
        ////    request.AddHeader("Cookie", "__cf_bm=7kmPe.jXN.sX1d1haElqTiBmnOHuyngn8hDeM25QOHE-1697541054-0-AeerXw5guGGB46IqGh5PXa961aZe4dBeTXyeyrGXwlylyEJHzjR3aCost7xionooYdtuMLmOXlvTGymoDARalPE=");

        ////    var emailBody = new
        ////    {
        ////        sender = new
        ////        {
        ////            name = senderName,
        ////            email = senderEmail
        ////        },
        ////        from = new
        ////        {
        ////            name = senderName,
        ////            email = fromMail
        ////        },
        ////        to = new[]
        ////        {
        ////    new
        ////    {
        ////        name = recipientName,
        ////        email = to
        ////    }
        ////},
        ////        htmlContent = bodyContent,
        ////        subject = subject,
        ////        replyTo = new
        ////        {
        ////            email = senderEmail,
        ////            name = senderName
        ////        },
        ////        tags = new[] { "tag1", "tag2" }
        ////    };

        ////    var body = JsonConvert.SerializeObject(emailBody);
        ////    request.AddParameter("application/json", body, ParameterType.RequestBody);

        ////    IRestResponse response = client.Execute(request);
        //    //var mailResponse = response.Content;
        //    return ();
        //}

        public string SendMailwithCC(string toEmail, string fromEmail, string ccEmail, string subject, string bodyContent)
        {
            int status = 0;
            Entities.AppInfo objappinfo = new Entities.AppInfo();
            objappinfo = _AppInfo.GetAppInfoDetails(ref status);

            string senderEmail = fromEmail;
            string to = toEmail;
            string[] ccAddresses = ccEmail.Split(',');
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;

            // ✅ Fix — use RestClientOptions for timeout
            var options = new RestClientOptions("https://api.brevo.com/v3/smtp/email")
            {
                Timeout = TimeSpan.FromMilliseconds(-1) // ✅ no timeout
            };
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddHeader("Accept", "application/json");
            request.AddHeader("api-key", objappinfo.BrevoKey);
            request.AddHeader("Cookie", "__cf_bm=7kmPe.jXN.sX1d1haElqTiBmnOHuyngn8hDeM25QOHE-1697541054-0-AeerXw5guGGB46IqGh5PXa961aZe4dBeTXyeyrGXwlylyEJHzj");

            // ✅ CC Recipients
            List<object> ccRecipients = new List<object>();
            foreach (var ccAddress in ccAddresses)
            {
                ccRecipients.Add(new
                {
                    name = "CC Recipient",
                    email = ccAddress.Trim()
                });
            }

            var emailBody = new
            {
                sender = new
                {
                    name = senderName,
                    email = senderEmail
                },
                to = new[]
                {
            new { name = recipientName, email = toEmail }
        },
                cc = ccRecipients,
                subject = subject,
                htmlContent = bodyContent
            };

            request.AddJsonBody(emailBody);
            request.Method = Method.Post;

            // ✅ RestResponse replaces IRestResponse
            RestResponse response = client.Execute(request);

            return response.Content ?? "";
        }
        public string SendMailwithAttachment(string toEmail, string subject, string bodyContent, string attachment)
        {
            int status = 0;
            Entities.AppInfo objappinfo = _AppInfo.GetAppInfoDetails(ref status);

            string senderEmail = objappinfo.SenderEmail;
            string senderName = objappinfo.MailName;
            string recipientName = objappinfo.MailName;
            string brevoKey = objappinfo.BrevoKey;

            // Convert attachment to base64
            string attachmentBase64 = "";
            string attachmentName = attachment ?? "";

            if (!string.IsNullOrEmpty(attachment))
            {
                string filepath = objappinfo.UploadPath + "\\events\\TicketImage\\" + attachment;
                if (File.Exists(filepath))
                {
                    byte[] bytes = File.ReadAllBytes(filepath);
                    attachmentBase64 = Convert.ToBase64String(bytes);
                }
            }

            var emailBody = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { name = recipientName, email = toEmail } },
                htmlContent = bodyContent,
                subject = subject,
                replyTo = new { email = senderEmail, name = senderName },
                tags = new[] { "tag1", "tag2" },
                attachments = new[]
                {
            new
            {
                content  = attachmentBase64,
                filename = attachmentName,
                type     = "application/pdf"
            }
        }
            };

            string jsonBody = JsonConvert.SerializeObject(emailBody,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("api-key", brevoKey);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient
                .PostAsync("https://api.brevo.com/v3/smtp/email", content)
                .GetAwaiter()
                .GetResult();

            string mailResponse = response.Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            return mailResponse;
        }

        private interface IRestResponse
        {
            string Content { get; set; }
            string StatusCode { get; set; }
        }





        public string SendMailWithAttachmentICOmuiltiple(string toEmail, string fromMail, string subject, string bodyContent, System.Net.Mail.Attachment attachment)
        {
            try
            {
                // Fetch app configuration
                int status = 0;
                var appInfo = _AppInfo.GetAppInfoDetails(ref status);

                if (appInfo == null)
                    throw new InvalidOperationException("Failed to retrieve application configuration.");

                // ✅ fromMail / appInfo.SenderEmail could contain multiple comma-separated addresses.
                // sender/replyTo only accept ONE email each - pick the first valid one.
                string rawSender = !string.IsNullOrWhiteSpace(fromMail) ? fromMail : appInfo.SenderEmail;
                string senderEmail = rawSender
                    .Split(',')
                    .Select(e => e.Trim())
                    .FirstOrDefault(e => IsValidEmail(e)) ?? appInfo.SenderEmail?.Trim();

                string senderName = appInfo.MailName;

                if (string.IsNullOrWhiteSpace(toEmail))
                    throw new ArgumentException("Recipient email address cannot be empty.", nameof(toEmail));

                if (attachment == null)
                    throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null.");

                // ✅ toEmail can contain multiple comma-separated addresses - split into recipient objects
                var toList = toEmail
                    .Split(',')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Select(e => new
                    {
                        name = senderName,
                        email = e
                    })
                    .ToArray();

                if (!toList.Any())
                    throw new ArgumentException("No valid recipient email address found.", nameof(toEmail));

                // Convert attachment stream to Base64
                string attachmentBase64;
                using (var memoryStream = new MemoryStream())
                {
                    attachment.ContentStream.Position = 0;
                    attachment.ContentStream.CopyTo(memoryStream);
                    attachmentBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }

                // Build request payload
                var emailPayload = new
                {
                    sender = new
                    {
                        name = senderName,
                        email = senderEmail
                    },
                    to = toList,   // ✅ array of recipient objects
                    subject = subject,
                    htmlContent = bodyContent,
                    replyTo = new
                    {
                        name = senderName,
                        email = senderEmail
                    },
                    attachment = new[]
                    {
                new
                {
                    content = attachmentBase64,
                    name    = attachment.Name,
                    type    = attachment.ContentType?.MediaType ?? "application/octet-stream"
                }
            }
                };

                // Send request via HttpClient
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", appInfo.BrevoKey);

                    var jsonBody = JsonConvert.SerializeObject(emailPayload);
                    var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", httpContent)
                                             .GetAwaiter()
                                             .GetResult();

                    string responseContent = response.Content.ReadAsStringAsync()
                                                      .GetAwaiter()
                                                      .GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Log or surface the error details from Brevo
                        throw new HttpRequestException(
                            $"Brevo API error [{(int)response.StatusCode}]: {responseContent}");
                    }

                    return responseContent;
                }
            }
            catch (Exception ex)
            {
                // Replace with your project's logger if available
                Console.Error.WriteLine($"[SendMailWithAttachmentICO] Error: {ex.Message}");
                throw;
            }
        }

        // ✅ Simple email format validator (add once per class, or move to a shared helper class)
     
        public string SendMailWithAttachmentICO(string toEmail, string fromMail, string subject, string bodyContent, System.Net.Mail.Attachment attachment)
        {
            try
            {
                // Fetch app configuration
                int status = 0;
                var appInfo = _AppInfo.GetAppInfoDetails(ref status);

                if (appInfo == null)
                    throw new InvalidOperationException("Failed to retrieve application configuration.");

                string senderEmail = !string.IsNullOrWhiteSpace(fromMail) ? fromMail : appInfo.SenderEmail;
                string senderName = appInfo.MailName;

                if (string.IsNullOrWhiteSpace(toEmail))
                    throw new ArgumentException("Recipient email address cannot be empty.", nameof(toEmail));

                if (attachment == null)
                    throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null.");

                // Convert attachment stream to Base64
                string attachmentBase64;
                using (var memoryStream = new MemoryStream())
                {
                    attachment.ContentStream.Position = 0;
                    attachment.ContentStream.CopyTo(memoryStream);
                    attachmentBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }

                // Build request payload
                var emailPayload = new
                {
                    sender = new
                    {
                        name = senderName,
                        email = senderEmail
                    },
                    to = new[]
                    {
                new
                {
                    name  = senderName,
                    email = toEmail
                }
            },
                    subject = subject,
                    htmlContent = bodyContent,
                    replyTo = new
                    {
                        name = senderName,
                        email = senderEmail
                    },
                    attachment = new[]
                    {
                new
                {
                    content = attachmentBase64,
                    name    = attachment.Name,
                    type    = attachment.ContentType?.MediaType ?? "application/octet-stream"
                }
            }
                };

                // Send request via HttpClient
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("api-key", appInfo.BrevoKey);

                    var jsonBody = JsonConvert.SerializeObject(emailPayload);
                    var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", httpContent)
                                             .GetAwaiter()
                                             .GetResult();

                    string responseContent = response.Content.ReadAsStringAsync()
                                                      .GetAwaiter()
                                                      .GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Log or surface the error details from Brevo
                        throw new HttpRequestException(
                            $"Brevo API error [{(int)response.StatusCode}]: {responseContent}");
                    }

                    return responseContent;
                }
            }
            catch (Exception ex)
            {
                // Replace with your project's logger if available
                Console.Error.WriteLine($"[SendMailWithAttachmentICO] Error: {ex.Message}");
                throw;
            }
        }

        //public string SendMailwithAttachmentICO(string toEmail, string fromMail,string subject, string bodyContent, Attachment attachment)
        //{
        //    int status = 0;
        //    Entities.AppInfo objappinfo = new Entities.AppInfo();
        //    objappinfo = _AppInfo.GetAppInfoDetails(ref status);

        //    string senderEmail = objappinfo.SenderEmail;
        //    string senderName = objappinfo.MailName;
        //    string recipientName = objappinfo.MailName;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    // Use fromMail if provided, otherwise fallback to senderEmail from config
        //    string actualSenderEmail = !string.IsNullOrWhiteSpace(fromMail) ? fromMail : senderEmail;

        //    var client = new RestClient("https://api.brevo.com/v3/smtp/email");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Accept", "application/json");
        //    request.AddHeader("api-key", objappinfo.BrevoKey);

        //    // Convert Attachment object to Base64
        //    string attachmentBase64 = "";
        //    string attachmentFileName = attachment.Name;
        //    string attachmentContentType = attachment.ContentType.MediaType;

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        attachment.ContentStream.Position = 0; // Reset stream to beginning
        //        attachment.ContentStream.CopyTo(memoryStream);
        //        attachmentBase64 = Convert.ToBase64String(memoryStream.ToArray());
        //    }

        //    var emailBody = new
        //    {
        //        sender = new
        //        {
        //            name = senderName,
        //            email = actualSenderEmail
        //        },
        //        to = new[]
        //        {
        //    new
        //    {
        //        name = recipientName,
        //        email = toEmail
        //    }
        //},
        //        htmlContent = bodyContent,
        //        subject = subject,
        //        replyTo = new
        //        {
        //            email = actualSenderEmail,
        //            name = senderName
        //        },
        //        tags = new[] { "tag1", "tag2" },
        //        attachment = new[]
        //        {
        //    new
        //    {
        //        content = attachmentBase64,
        //        name = attachmentFileName,
        //        type = attachmentContentType
        //    }
        //}
        //    };

        //    var body = JsonConvert.SerializeObject(emailBody);
        //    request.AddParameter("application/json", body, ParameterType.RequestBody);

        //    IRestResponse response = client.Execute(request);
        //    return response.Content;
        //}


        #region AddingContactsIntoBrevo

        //public string AddContactList(string email, string FIRSTNAME, string LASTNAME, int[] arr, string SMS)
        //{
        //    int status = 0;
        //    Entities.AppInfo objappinfo = new Entities.AppInfo();
        //    objappinfo = _AppInfo.GetAppInfoDetails(ref status);

        //    SMS = "+1000-000-0000";
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        //               | SecurityProtocolType.Tls11
        //               | SecurityProtocolType.Tls12
        //               | SecurityProtocolType.Ssl3;
        //    var client = new RestClient("https://api.brevo.com/v3/contacts");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Accept", "application/json");
        //    request.AddHeader("api-key", objappinfo.BrevoKey);
        //    request.AddHeader("Cookie", "__cf_bm=7kmPe.jXN.sX1d1haElqTiBmnOHuyngn8hDeM25QOHE-1697541054-0-AeerXw5guGGB46IqGh5PXa961aZe4dBeTXyeyrGXwlylyEJHzjR3aCost7xionooYdtuMLmOXlvTGymoDARalPE=");

        //    var emailBody = "{\"email\": \"" + email + "\",\"attributes\":{\"FIRSTNAME\":\"" + FIRSTNAME + "\",\"LASTNAME\":\"" + LASTNAME + "\",\"SMS\":\"" + SMS + "\"},\"listIds\":" + arr + "}";

        //    request.AddParameter("application/json", emailBody, ParameterType.RequestBody);

        //    IRestResponse response = client.Execute(request);
        //    var mailResponse = response.Content;
        //    return mailResponse;

        //}

        //public string ScheduleEmails(string toEmail, string subject, string bodyContent, string rfc33399format)
        //{
        //    int status = 0;
        //    Entities.AppInfo objappinfo = new Entities.AppInfo();
        //    objappinfo = _AppInfo.GetAppInfoDetails(ref status);

        //    string senderEmail = objappinfo.SenderEmail;
        //    //string senderEmail = fromMail; // Use the new parameter for sender's email
        //    String to = toEmail;
        //    string senderName = objappinfo.MailName;
        //    string recipientName = objappinfo.MailName;

        //    var client = new RestClient("https://api.brevo.com/v3/smtp/email");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Accept", "application/json");
        //    request.AddHeader("api-key", objappinfo.BrevoKey);
        //    request.AddHeader("Cookie", "__cf_bm=7kmPe.jXN.sX1d1haElqTiBmnOHuyngn8hDeM25QOHE-1697541054-0-AeerXw5guGGB46IqGh5PXa961aZe4dBeTXyeyrGXwlylyEJHzjR3aCost7xionooYdtuMLmOXlvTGymoDARalPE=");

        //    var emailBody = new
        //    {
        //        sender = new
        //        {
        //            name = senderName,
        //            email = senderEmail
        //        },
        //        to = new[]
        //        {
        //            new
        //            {
        //               name = recipientName,
        //                email = to

        //            }
        //        },
        //        templateId = 2,
        //        //scheduledAt= "2023-12-17T11:25:00+05:30a",
        //        scheduledAt = rfc33399format,
        //        htmlContent = bodyContent,
        //        subject = subject,
        //        replyTo = new
        //        {
        //            email = senderEmail,
        //            name = senderName
        //        },
        //        tags = new[] { "tag1", "tag2" }
        //    };

        //    var body = JsonConvert.SerializeObject(emailBody);
        //    request.AddParameter("application/json", body, ParameterType.RequestBody);

        //    IRestResponse response = client.Execute(request);
        //    var mailResponse = response.Content;
        //    return mailResponse;
        //}
        #endregion

    }
}
