using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Balance
{
    public class BalanceInfoCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxBalanceInfo")]
        public List<BalanceInfo> Values { get; set; }
        public BalanceInfoCollection()
        {
            Values = new List<BalanceInfo>();
        }
    }
}