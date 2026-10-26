using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class RecurringPaymentController : Controller
    {
        HomeController _HomeController = new HomeController();
        AWS_AssociationToolCore.BLL.PaymentSettings _PaymentSettings = new BLL.PaymentSettings();

        public async Task<(string status, string subscriptionId)> CreateSquareSubscriptionAsync(
            string accessToken,
            string squareEnvironment,
            string locationId,
            Entities.Members objDonors,
            string customerId,
            string cardId)
        {
            try
            {
                // ── Map RecurringType → Square cadence ───────────────────────────────
                string cadence;
                switch (objDonors.RecurringType)
                {
                    case "Weekly": cadence = "WEEKLY"; break;
                    case "Monthly": cadence = "MONTHLY"; break;
                    case "Quarterly": cadence = "NINETY_DAYS"; break;
                    case "Half Yearly": cadence = "SIXTY_DAYS"; break;
                    case "Yearly": cadence = "ANNUAL"; break;
                    default:
                        return ($"Unknown RecurringType: {objDonors.RecurringType}", "");
                }

                DateTime nextBilling = objDonors.InsertedTime != DateTime.MinValue
                                           ? objDonors.InsertedTime
                                           : DateTime.UtcNow;

                long amountInCents = (long)(objDonors.MemberAmount * 100);
                string baseUrl = squareEnvironment == "production"
                                       ? "https://connect.squareup.com"
                                       : "https://connect.squareupsandbox.com";

                using (var http = new System.Net.Http.HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    http.DefaultRequestHeaders.Add("Square-Version", "2023-10-18");
                    http.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // ── STEP A: Create Catalog Plan + Variation ──────────────────────
                    string tempPlanId = $"#plan-{Guid.NewGuid():N}";
                    string tempVariationId = $"#variation-{Guid.NewGuid():N}";

                    var catalogPayload = new
                    {
                        idempotency_key = Guid.NewGuid().ToString(),
                        batches = new[]
                        {
                            new
                            {
                                objects = new object[]
                                {
                                    new
                                    {
                                        type = "SUBSCRIPTION_PLAN",
                                        id = tempPlanId,
                                        subscription_plan_data = new
                                        {
                                            name = $"{objDonors.MembershipType ?? "Membership"} - {objDonors.RecurringType}"
                                        }
                                    },
                                    new
                                    {
                                        type = "SUBSCRIPTION_PLAN_VARIATION",
                                        id = tempVariationId,
                                        subscription_plan_variation_data = new
                                        {
                                            name = $"{objDonors.RecurringType} Variation",
                                            subscription_plan_id = tempPlanId,
                                            phases = new[]
                                            {
                                                new
                                                {
                                                    cadence = cadence,
                                                    ordinal = 0,
                                                    pricing = new          // ✅ FIXED: was recurring_price_money
                                                    {
                                                        type = "STATIC",
                                                        price_money = new
                                                        {
                                                            amount   = amountInCents,
                                                            currency = "AUD"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    string catalogJson = Newtonsoft.Json.JsonConvert.SerializeObject(catalogPayload);
                    var catalogContent = new System.Net.Http.StringContent(
                        catalogJson, System.Text.Encoding.UTF8, "application/json");

                    var catalogHttpResponse = await http.PostAsync($"{baseUrl}/v2/catalog/batch-upsert", catalogContent);
                    var catalogBody = await catalogHttpResponse.Content.ReadAsStringAsync();

                    if (!catalogHttpResponse.IsSuccessStatusCode)
                        return ($"Catalog plan creation failed: {catalogBody}", "");

                    dynamic catalogResult = Newtonsoft.Json.JsonConvert.DeserializeObject(catalogBody);
                    string planVariationId = "";

                    foreach (var mapping in catalogResult.id_mappings)
                    {
                        string clientId = mapping.client_object_id?.ToString() ?? "";
                        if (clientId.StartsWith("#variation-"))
                        {
                            planVariationId = mapping.object_id?.ToString() ?? "";
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(planVariationId))
                        return ($"Catalog plan creation failed: variation id empty | Body:{catalogBody}", "");

                    // ── STEP B: Create Subscription ──────────────────────────────────
                    // ── STEP B: Update customer email in Square ──────────────────────────
                    if (!string.IsNullOrEmpty(objDonors.Email))
                    {
                        var updateCustomerPayload = new
                        {
                            email_address = objDonors.Email
                        };
                        string updateJson = Newtonsoft.Json.JsonConvert.SerializeObject(updateCustomerPayload);
                        var updateContent = new System.Net.Http.StringContent(
                            updateJson, System.Text.Encoding.UTF8, "application/json");

                        await http.PutAsync($"{baseUrl}/v2/customers/{customerId}", updateContent);
                    }

                    // ── STEP C: Create Subscription ──────────────────────────────────────
                    var subscriptionPayload = new
                    {
                        idempotency_key = Guid.NewGuid().ToString(),
                        location_id = locationId,
                        customer_id = customerId,
                        card_id = cardId,
                        plan_variation_id = planVariationId,
                        start_date = nextBilling.ToString("yyyy-MM-dd")
                        // ✅ removed buyer_email_address — not valid field
                    };

                    string subJson = Newtonsoft.Json.JsonConvert.SerializeObject(subscriptionPayload);
                    var subContent = new System.Net.Http.StringContent(
                        subJson, System.Text.Encoding.UTF8, "application/json");

                    var subHttpResponse = await http.PostAsync($"{baseUrl}/v2/subscriptions", subContent);
                    var subBody = await subHttpResponse.Content.ReadAsStringAsync();

                    if (!subHttpResponse.IsSuccessStatusCode)
                        return ($"Subscription failed: {subBody}", "");

                    dynamic subResult = Newtonsoft.Json.JsonConvert.DeserializeObject(subBody);
                    string subId = subResult.subscription.id?.ToString() ?? "";

                    objDonors.SubscriptionId = subId;
                    return ("Success", subId);
                }
            }
            catch (Exception ex)
            {
                return ($"Subscription failed: {ex.Message}", "");
            }
        }
    }
}