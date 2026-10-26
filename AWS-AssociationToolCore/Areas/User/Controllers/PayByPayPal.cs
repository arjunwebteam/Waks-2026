using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentProcess
{
    public class PayByPayPal
    {
        public CartItems objCartItems { get; set; }
        public List<CartItems> lstCartItems { get; set; } = new List<CartItems>();
        public PaymentSettings objPaymentSettings { get; set; } = new PaymentSettings();
        public PDTHolder objPDTHolder { get; set; }

        // 🔹 STEP 1: Redirect user to PayPal
        public string CheckOut()
        {
            var url = $"{objPaymentSettings.PaymentUrl}?cmd=_xclick" +
                      $"&business={objPaymentSettings.BusinessEmail}" +
                      $"&item_name={objPaymentSettings.ItemName}" +
                      $"&amount={objPaymentSettings.DonationAmount}" +
                      $"&currency_code={objPaymentSettings.CurrencyCode}" +
                      $"&return={objPaymentSettings.ReturnUrl}" +
                      $"&cancel_return={objPaymentSettings.CancelUrl}" +
                      $"&notify_url={objPaymentSettings.NotifyUrl}";

            return url;
        }

        // 🔹 STEP 2: Handle IPN (PayPal server callback)
        public async Task<string> Ipn(string formData)
        {
            using var client = new HttpClient();

            var content = new StringContent("cmd=_notify-validate&" + formData);

            var response = await client.PostAsync(objPaymentSettings.PaymentUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            return result; // VERIFIED or INVALID
        }

        // 🔹 STEP 3: Success (after payment)
        public int Success(string tx)
        {
            // You can validate transaction here
            if (!string.IsNullOrEmpty(tx))
                return 1;

            return 0;
        }

        // 🔹 STEP 4: Recurring (basic example)
        public string RecurringPayment()
        {
            var url = $"{objPaymentSettings.PaymentUrl}?cmd=_xclick-subscriptions" +
                      $"&business={objPaymentSettings.BusinessEmail}" +
                      $"&item_name={objPaymentSettings.ItemName}" +
                      $"&a3={objPaymentSettings.DonationAmount}" +
                      $"&p3={objPaymentSettings.RecurMonths}" +
                      $"&t3={objPaymentSettings.RecurType}" +
                      $"&src=1" +
                      $"&return={objPaymentSettings.ReturnUrl}";

            return url;
        }

        internal string Ipn()
        {
            throw new NotImplementedException();
        }

        // ================= INNER CLASSES =================

        public class CartItems
        {
            public string ItemName { get; set; }
            public decimal ItemPrice { get; set; }
        }

        public class PaymentSettings
        {
            public string BusinessEmail { get; set; }
            public string ItemName { get; set; }
            public string ItemNumber { get; set; }
            public string RecurType { get; set; }
            public string RecurMonths { get; set; }
            public decimal DonationAmount { get; set; }
            public string CurrencyCode { get; set; }
            public string ReturnUrl { get; set; }
            public string CancelUrl { get; set; }
            public string NotifyUrl { get; set; }
            public string PaymentUrl { get; set; } = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            public string PDTToken { get; set; }
            public string CBT { get; set; }
            public string Cmd { get; set; }
        }

        public class PDTHolder
        {
            public string TransactionId { get; set; }
            public string PaymentStatus { get; set; }
            public string PayerEmail { get; set; }
            public double GrossTotal { get; set; }
            public object PaymentDate { get; internal set; }

            public static PDTHolder Parse(string data)
            {
                var holder = new PDTHolder();

                var lines = data.Split('\n');

                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length < 2) continue;

                    var key = parts[0];
                    var value = parts[1];

                    switch (key)
                    {
                        case "txn_id":
                            holder.TransactionId = value;
                            break;
                        case "payment_status":
                            holder.PaymentStatus = value;
                            break;
                        case "payer_email":
                            holder.PayerEmail = value;
                            break;
                        case "mc_gross":
                            holder.GrossTotal = Convert.ToDouble(value);
                            break;
                    }
                }

                return holder;
            }
        }
    }
}