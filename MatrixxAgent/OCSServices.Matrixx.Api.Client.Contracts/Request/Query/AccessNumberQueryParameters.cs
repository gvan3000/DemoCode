using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/subscriber/query/AccessNumber/#Msisdn#?querySize=2")]
    public class AccessNumberQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "Msisdn")]
        public string Msisdn { get; private set; }

        public AccessNumberQueryParameters(string msisdn)
        {
            Msisdn = msisdn;
        }
    }
}