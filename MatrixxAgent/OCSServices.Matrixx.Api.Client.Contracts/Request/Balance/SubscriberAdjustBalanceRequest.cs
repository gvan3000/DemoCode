using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxRequestSubscriberAdjustBalance")]
    public class SubscriberAdjustBalanceRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

        [MatrixxContractMember(Name = "BalanceResourceId")]
        public int BalanceResourceId { get; set; }

        [MatrixxContractMember(Name = "AdjustType")]
        public int? AdjustType { get; set; }

        [MatrixxContractMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [MatrixxContractMember(Name = "Reason")]
        public string Reason { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime? EndTime { get; set; }

        [MatrixxContractMember(Name = "StartTime")]
        public DateTime? StartTime { get; set; }
    }
}
