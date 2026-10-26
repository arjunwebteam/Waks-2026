using Braintree;
using Microsoft.AspNetCore.Mvc;
using AWS_AssociationToolCore.BLL;

namespace AWS_AssociationToolCore.Areas.User.Controllers
{
    [Area("User")]
    public class BrainTreePaymentController : Controller
    {
        // ✅ Use DI — remove "new BraintreeConfiguration()"
        private readonly IBraintreeConfiguration _config;

        BLL.Members _Members = new BLL.Members();
        BLL.Users _Users = new BLL.Users();

        // ✅ Constructor injection
        public BrainTreePaymentController(IBraintreeConfiguration config)
        {
            _config = config;
        }

        public static readonly TransactionStatus[] transactionSuccessStatuses =
        {
            TransactionStatus.AUTHORIZED,
            TransactionStatus.AUTHORIZING,
            TransactionStatus.SETTLED,
            TransactionStatus.SETTLING,
            TransactionStatus.SETTLEMENT_CONFIRMED,
            TransactionStatus.SETTLEMENT_PENDING,
            TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };

        public ActionResult BrainTree(long tid = 0, decimal amt = 0,
                                       string type = "", string name = "")
        {
            // ✅ Use injected _config instead of "config"
            IBraintreeGateway gateway = _config.GetGateway();
            string clientToken = gateway.ClientToken.Generate();

            ViewBag.ClientToken = clientToken;
            ViewBag.tid = tid;
            ViewBag.amt = amt;
            ViewBag.username = name;
            ViewBag.type = type;

            return View();
        }
    }
}