using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestMulti")]
    public class MultiRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "RequestList")]
        public RequestCollection RequestCollection { get; set; }

        public MultiRequest()
        {
            //RequestList = new List<MatrixxObject>();
        }
    }
    [MatrixxContract(Name = "RequestList")]
    public class RequestCollection : MatrixxObject
    {
        [MatrixxContractMember]
        public List<MatrixxObject> Values { get; set; }

        public RequestCollection()
        {
            Values = new List<MatrixxObject>();
        }
    }


    //public class SubscriberConfiguration
    //{
    //    [MatrixxKeyValuePairList(Name="")]
    //    public Dictionary<string, string> Values { get; set; } 
    //}
}
