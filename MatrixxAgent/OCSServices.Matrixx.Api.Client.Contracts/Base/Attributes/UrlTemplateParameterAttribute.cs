using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlTemplateParameterAttribute : Attribute
    {
        public string Name { get; set; }
    }
}