using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    public class ThresholdCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "ThresholdId")]
        public int ThresholdId { get; set; }

        [MatrixxContractMember(Name = "Amount")]
        public decimal Amount { get; set; }

    }
}
