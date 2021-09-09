using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Wallet
{
    public class GetWalletRequest
    {
        public string MsIsdn { get; set; }

        public Guid? ProductId { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
