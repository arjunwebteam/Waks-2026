using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    [Route("api/square-webhook")]
    public class SquareWebhookController : Controller
    {
        AWS_AssociationToolCore.BLL.Members _Members = new BLL.Members();
        AWS_AssociationToolCore.BLL.Users _Users = new BLL.Users();



        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Receive()
        {
            Int64 LogId = 0;
            try
            {
                var objLogReport = new Entities.ApplicationLogs
                {
                    LogTitle = "Square Webhook",
                    LogDescription = "Square Webhook Received",
                    LogDate = DateTime.Now,
                    InsertedBy = "Webhook",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = "Webhook",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common.GetTimeZoneAdjustedDateTime()
                };
                _Users.LogReportInsert(objLogReport, ref LogId);
            }
            catch { LogId = 0; }

            string body = "";
            try
            {
                using (var reader = new System.IO.StreamReader(Request.Body))
                    body = await reader.ReadToEndAsync();

                SafeLog(LogId, "Square Webhook", "Raw: " + body);

                if (string.IsNullOrEmpty(body))
                {
                    SafeLog(LogId, "Square Webhook", "❌ Empty body. Exiting.");
                    return Ok();
                }

                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
                string eventType = json?.type?.ToString() ?? "";

                SafeLog(LogId, "Square Webhook", $"EventType: {eventType}");

                // ✅ Only payment.updated — money is CAPTURED
                // payment.created = AUTHORIZED (money NOT taken yet) → skip
                // payment.updated = CAPTURED   (money taken)         → process
                if (eventType == "payment.updated")
                {
                    await HandlePaymentCaptured(json, LogId);
                }
                else
                {
                    SafeLog(LogId, "Square Webhook",
                        $"⚠️ Skipping EventType: {eventType}");
                }
            }
            catch (Exception ex)
            {
                SafeLog(LogId, "Square Webhook ERROR",
                    $"Receive Exception: {ex.Message}");
            }

            return Ok();
        }

        private async Task HandlePaymentCaptured(dynamic json, long LogId)
        {
            try
            {
                // ✅ FIXED: use ["object"]
                var payment = json?.data?["object"]?.payment;

                string paymentId = payment?.id?.ToString() ?? "";
                string cardStatus = payment?.card_details?.status?.ToString() ?? "";
                string entryMethod = payment?.card_details?.entry_method?.ToString() ?? "";
                string buyerEmail = payment?.buyer_email_address?.ToString() ?? "";
                decimal amount = 0;

                // ✅ FIXED: use payment directly (not object_)
                var amountMoney = payment?.amount_money;
                if (amountMoney != null)
                {
                    long cents = (long)(amountMoney.amount ?? 0);
                    amount = cents / 100m;
                }

                SafeLog(LogId, "Square Webhook",
                    $"payment.updated | PaymentId:{paymentId} | CardStatus:{cardStatus} | EntryMethod:{entryMethod} | Amount:{amount} | Email:{buyerEmail}");

                if (cardStatus != "CAPTURED")
                {
                    SafeLog(LogId, "Square Webhook",
                        $"ℹ️ Skipping — CardStatus:{cardStatus} not CAPTURED");
                    return;
                }

                if (entryMethod != "ON_FILE")
                {
                    SafeLog(LogId, "Square Webhook",
                        $"ℹ️ Skipping — EntryMethod:{entryMethod} not ON_FILE");
                    return;
                }

                SafeLog(LogId, "Square Webhook",
                    $"✅ Subscription auto-debit detected | PaymentId:{paymentId} | Amount:{amount}");

                int dupStatus = 0;
                bool alreadyProcessed = _Members.IsPaymentAlreadyProcessed(
                    paymentId, ref dupStatus);

                if (alreadyProcessed)
                {
                    SafeLog(LogId, "Square Webhook",
                        $"ℹ️ Duplicate — PaymentId:{paymentId} already processed. Skipping.");
                    return;
                }

                int refStatus = 0;
                Entities.MemberSubscriptions objSub = null;

                if (!string.IsNullOrEmpty(buyerEmail))
                {
                    objSub = _Members.GetMemberSubscriptionByEmail(buyerEmail, ref refStatus);
                    SafeLog(LogId, "Square Webhook",
                        $"GetMemberSubscriptionByEmail Result:{refStatus} | Email:{buyerEmail}");
                }

                if (objSub == null)
                {
                    SafeLog(LogId, "Square Webhook",
                        $"❌ No member found for Email:{buyerEmail}");
                    return;
                }

                SafeLog(LogId, "Square Webhook",
                    $"✅ Member found | MemberId:{objSub.MemberId} | RecurringStartTime:{objSub.RecurringStartTime}");

                DateTime fromDate = objSub.RecurringStartTime != DateOnly.MinValue
                    ? objSub.RecurringStartTime.ToDateTime(TimeOnly.MinValue)
                    : DateTime.Now;

                var (nextExpiry, autoDebitDate) = GetNextExpiryDate(fromDate);

                SafeLog(LogId, "Square Webhook",
                    $"ExpiryDate:{nextExpiry:yyyy-MM-dd} | NextAutoDebit:{autoDebitDate:yyyy-MM-dd}");

                Int64 orderResult = _Members.UpdateMemberOrderExpiryBySubId(
                    new Entities.MembershipOrders
                    {
                        MemberId = objSub.MemberId,
                        ExpiryDate = nextExpiry
                    });

                SafeLog(LogId, "Square Webhook",
                    $"UpdateMemberOrderExpiryBySubId Result:{orderResult}");

                Int64 subResult = _Members.UpdateSubscriptionNextDate(
                    new Entities.MemberSubscriptions
                    {
                        MemberId = objSub.MemberId,
                        SubscriptionId = objSub.SubscriptionId,
                        RecurringStartTime = DateOnly.FromDateTime(autoDebitDate)
                    });

                SafeLog(LogId, "Square Webhook",
                    $"UpdateSubscriptionNextDate Result:{subResult}");

                _Members.SaveProcessedPayment(paymentId, objSub.MemberId);

                SafeLog(LogId, "Square Webhook",
                    $"✅ FINAL SUCCESS | MemberId:{objSub.MemberId} | ExpiryDate:{nextExpiry:yyyy-MM-dd} | NextAutoDebit:{autoDebitDate:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                SafeLog(LogId, "Square Webhook ERROR",
                    $"HandlePaymentCaptured Exception: {ex.Message}");
            }
        }

        // ================================================================
        // HELPER: Calculate ExpiryDate AND AutoDebitDate
        //
        // Jun 30   → year+1 July 1 → expiry year+1 June 30
        // Jan–Mar  → same year July 1 → expiry same year June 30
        // Apr–Dec  → next year July 1 → expiry next year June 30
        //
        // Example:
        // RecurringStartTime = 2026-06-01
        // nextJuly1          = 2027-07-01
        // ExpiryDate         = 2027-06-30 ✅
        // AutoDebitDate      = 2027-06-30 ✅ (Square charges on this day)
        // ================================================================
        private (DateTime expiryDate, DateTime autoDebitDate) GetNextExpiryDate(
            DateTime fromDate)
        {
            DateTime nextJuly1;

            if (fromDate.Month == 6 && fromDate.Day == 30)
                // Payment ON June 30 → next = year+1 July 1
                nextJuly1 = new DateTime(fromDate.Year + 1, 7, 1);
            else if (fromDate.Month < 4)
                // Jan, Feb, Mar → same year July 1
                nextJuly1 = new DateTime(fromDate.Year, 7, 1);
            else
                // Apr to Dec → next year July 1
                nextJuly1 = new DateTime(fromDate.Year + 1, 7, 1);

            // ExpiryDate    = June 30 (1 day before July 1)
            // AutoDebitDate = June 30 (Square charges on this day)
            DateTime expiryDate = nextJuly1.AddDays(-1);
            DateTime autoDebitDate = expiryDate;

            return (expiryDate, autoDebitDate);
        }

        // ================================================================
        // SAFE LOG HELPER
        // log fail అయినా main flow break కాదు
        // ================================================================
        private void SafeLog(long LogId, string title, string description)
        {
            try
            {
                _Users.LogSubReportInsert(new Entities.Logdetails
                {
                    LogId = LogId,
                    LogTitle = title,
                    LogDescription = description,
                    LogDate = DateTime.Now,
                    InsertedBy = "Webhook",
                    InsertedDate = DateTime.Now,
                    UpdatedBy = "Webhook",
                    UpdatedDate = AWS_AssociationToolCore.BLL.Common
                                        .GetTimeZoneAdjustedDateTime()
                });
            }
            catch { }
        }
        //    // ================================================================
        //    // RECEIVE WEBHOOK FROM SQUARE
        //    // ================================================================

        // ⚠️ REMOVE THIS BEFORE PRODUCTION
        //[HttpGet("test-webhook")]
        //public async Task<IActionResult> TestWebhook()
        //{
        //    string fakeBody = @"{
        //    ""merchant_id"": ""MLXEMCFH7RE8Q"",
        //    ""type"": ""payment.updated"",
        //    ""event_id"": ""test-event-209339"",
        //    ""data"": {
        //        ""type"": ""payment"",
        //        ""id"": ""test-payment-209339"",
        //        ""object"": {
        //            ""payment"": {
        //                ""id"": ""test-payment-209339"",
        //                ""amount_money"": { ""amount"": 5000, ""currency"": ""AUD"" },
        //                ""buyer_email_address"": ""massshipalmahi4602@gmail.com"",
        //                ""card_details"": {
        //                    ""status"": ""CAPTURED"",
        //                    ""entry_method"": ""ON_FILE""
        //                }
        //            }
        //        }
        //    }
        //}";

        //    long LogId = 0;
        //    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(fakeBody);
        //    await HandlePaymentCaptured(json, LogId);

        //    return Ok("✅ Test fired for MemberId:209336 massshipalmahi4602@gmail.com");
        //}



    }
}