using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Multi
{
    [MatrixxContract(Name = "MtxResponseMulti")]
    public class MultiResponse : MatrixxResponse
    {
        //[MatrixxContractMember(Name = "ResponseList")]
        //public List<MatrixxResponse> ResponseList { get; set; }

        [MatrixxContractMember(Name = "ResponseList")]
        public ResponseCollection ResponseCollection { get; set; }

        public MultiResponse()
        {
            //ResponseList = new List<MatrixxResponse>();
        }
    }

    [MatrixxContract(Name = "ResponseList")]
    public class ResponseCollection : MatrixxObject
    {
        [MatrixxContractMember()]
        public List<MatrixxObject> ResponseList { get; set; }

        public ResponseCollection()
        {
            ResponseList = new List<MatrixxObject>();
        }
    }
}
