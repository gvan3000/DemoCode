using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Agent.Contracts.Wallet
{
    public class GetGroupWalletRequest
    {
        public string ExternalId { get; set; }

        public Endpoint Endpoint { get; set; }
    }
}