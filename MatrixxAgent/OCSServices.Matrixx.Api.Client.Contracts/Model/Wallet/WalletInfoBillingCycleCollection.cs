using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Wallet
{

    public class WalletInfoBillingCycleCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxBillingCycleInfo")]
        public List<BillCycleInfo> Values { get; set; }
        public WalletInfoBillingCycleCollection()
        {
            Values = new List<BillCycleInfo>();
        }
    }
}