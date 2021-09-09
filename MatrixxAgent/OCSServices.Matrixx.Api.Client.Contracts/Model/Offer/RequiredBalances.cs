using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class RequiredBalances : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxRequiredBalanceInfo")]
        public List<RequiredBalanceInfo> Values { get; set; }

        public RequiredBalances()
        {
            Values = new List<RequiredBalanceInfo>();
        }
    }
}
