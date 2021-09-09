using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Wallet
{
    public class WalletInfoPeriodicCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxBalanceInfoPeriodic")]
        public List<WalletInfo> Values { get; set; }
        public WalletInfoPeriodicCollection()
        {
            Values = new List<WalletInfo>();
        }
    }
}
