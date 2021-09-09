using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Wallet
{
    public class WalletInfoCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxBalanceInfoSimple")]
        public List<WalletInfo> Values { get; set; }
        public WalletInfoCollection()
        {
            Values = new List<WalletInfo>();
        }
    }
}
