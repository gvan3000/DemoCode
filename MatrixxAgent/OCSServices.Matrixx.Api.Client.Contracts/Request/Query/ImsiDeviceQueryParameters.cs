using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/device/query/imsi/#Imsi#")]
    public class ImsiDeviceQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "Imsi")]
        public string Imsi { get; private set; }

        public ImsiDeviceQueryParameters(string imsi)
        {
            Imsi = imsi;
        }
    }
}
