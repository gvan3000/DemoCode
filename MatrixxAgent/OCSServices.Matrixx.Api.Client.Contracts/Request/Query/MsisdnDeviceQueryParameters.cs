using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/device/query/AccessNumber/#Msisdn#")]
    public class MsisdnDeviceQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "Msisdn")]
        public string Msisdn { get; private set; }

        public MsisdnDeviceQueryParameters(string msisdn)
        {
            Msisdn = msisdn;
        }
    }
}
