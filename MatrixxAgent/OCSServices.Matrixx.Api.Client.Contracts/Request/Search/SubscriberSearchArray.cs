using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Search
{
    [MatrixxContract(Name = "SubscriberArray")]
    public class SubscriberSearchArray : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxSubscriberSearchData")]
        public SearchCollection SearchCollection { get; set; }
    }

}
