using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class ChargingSessionExtension : MatrixxObject
    {
        /// <summary>
        /// Stored value of Diameter User-Location-Info AVP
        /// </summary>
        [MatrixxContractMember(Name = "LocationInfo")]
        public long LocationInfo { get; set; }
    }
}
