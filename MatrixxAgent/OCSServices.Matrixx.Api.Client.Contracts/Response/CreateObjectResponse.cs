using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response
{
    [MatrixxContract(Name = "MtxResponseCreate")]
    public class CreateObjectResponse : MatrixxResponse
    {
        [MatrixxContractMember(Name = "ObjectId")]
        public string ObjectId { get; set; }
    }
}
