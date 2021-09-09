using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class RequiredBalanceInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "TemplateId")]
        public int TemplateId { get; set; }

        [MatrixxContractMember(Name = "ResourceId")]
        public int ResourceId { get; set; }
    }
}