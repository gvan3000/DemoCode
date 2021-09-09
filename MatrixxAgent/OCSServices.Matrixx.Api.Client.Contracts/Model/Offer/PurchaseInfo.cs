using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchaseInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "ProductOfferId")]
        public int ProductOfferId { get; set; }

        [MatrixxContractMember(Name = "ProductOfferVersion")]
        public int ProductOfferVersion { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ProductOfferExternalId { get; set; }

        [MatrixxContractMember(Name = "ResourceId")]
        public int ResourceId { get; set; }

        [MatrixxContractMember(Name = "RequiredBalanceArray")]
        public RequiredBalances Balances { get; set; }
    }
}
