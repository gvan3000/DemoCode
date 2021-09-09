using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestSubscriberAddThreshold")]
    public class SubscriberAddThresholdRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

        [MatrixxContractMember(Name = "BalanceResourceId")]
        public int BalanceResourceId { get; set; }

        [MatrixxContractMember(Name = "Threshold")]
        public SubscriberThresholdData ThresholdData { get; set; }

    }
}
