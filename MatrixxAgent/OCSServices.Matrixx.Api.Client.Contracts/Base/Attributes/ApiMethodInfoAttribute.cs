using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiMethodInfoAttribute : Attribute
    {
        public string UrlTemplate { get; set; }
    }
}