using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response
{
    [MatrixxContract(Name= "MtxResponse")]
    public class MatrixxResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "Result")]
        public int? Code { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string Text { get; set; }
    }
}
