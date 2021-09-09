using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model
{
    public class StringValueCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "value")]
        public List<string> Values { get; set; }

        public StringValueCollection()
        {
            Values = new List<string>();
        }
    }
}