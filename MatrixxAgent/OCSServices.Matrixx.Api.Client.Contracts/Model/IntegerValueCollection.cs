using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model
{
    public class IntegerValueCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "value")]
        public List<int> Values { get; set; }

        public IntegerValueCollection()
        {
            Values = new List<int>();
        }
    }
}
