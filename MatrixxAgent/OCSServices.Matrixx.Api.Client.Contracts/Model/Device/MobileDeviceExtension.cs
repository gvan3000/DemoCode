using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class MobileDeviceExtension : MatrixxObject
    {
        [MatrixxContractMember(Name = "Imsi")]
        public string Imsi { get; set; }

        [MatrixxContractMember(Name = "AccessNumberArray")]
        public StringValueCollection AccessNumberList { get; set; }
    }

}
