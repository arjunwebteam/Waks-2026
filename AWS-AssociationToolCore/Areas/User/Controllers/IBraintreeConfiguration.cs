using Braintree;
using Microsoft.Extensions.Configuration;

namespace AWS_AssociationToolCore.BLL
{
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
        string GetConfigurationSetting(string setting);
    }

    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        private IBraintreeGateway _gateway;

        // ✅ Read directly from appsettings.json — no IConfiguration needed
        private readonly string _merchantId;
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly string _environment;

        // ✅ Constructor with IConfiguration (used by DI)
        public BraintreeConfiguration(IConfiguration configuration)
        {
            _environment = configuration["BrainTree:Environment"];
            _merchantId = configuration["BrainTree:MerchantId"];
            _publicKey = configuration["BrainTree:PublicKey"];
            _privateKey = configuration["BrainTree:PrivateKey"];
        }

        public BraintreeConfiguration()
        {
        }

        public IBraintreeGateway CreateGateway()
        {
            // ✅ Pick environment from appsettings.json
            var env = _environment?.ToLower() == "production"
                ? Braintree.Environment.PRODUCTION
                : Braintree.Environment.SANDBOX;

            return new BraintreeGateway(
                environment: env,
                merchantId: _merchantId,
                publicKey: _publicKey,
                privateKey: _privateKey
            );
        }

        public IBraintreeGateway GetGateway()
        {
            if (_gateway == null)
            {
                _gateway = CreateGateway();
            }
            return _gateway;
        }

        public string GetConfigurationSetting(string setting)
        {
            // ✅ Return stored values directly — no null _configuration risk
            return setting switch
            {
                "BrainTree:MerchantId" => _merchantId,
                "BrainTree:PublicKey" => _publicKey,
                "BrainTree:PrivateKey" => _privateKey,
                "BrainTree:Environment" => _environment,
                _ => string.Empty
            };
        }
    }
}