using System;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiVersionAttribute : Attribute
    {
        public ApiVersion Version { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public ApiVersionAttribute(ApiVersion version, HttpMethod httpMethod)
        {
            Version = version;
            HttpMethod = httpMethod;
        }
    }
}