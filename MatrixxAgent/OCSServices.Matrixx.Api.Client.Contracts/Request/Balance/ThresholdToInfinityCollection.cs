using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    public class ThresholdToInfinityCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "ThresholdId")]
        public int ThresholdId { get; set; }

        [MatrixxContractMember(Name = "Amount")]
        public string Amount => "INFINITY";
    }
}
