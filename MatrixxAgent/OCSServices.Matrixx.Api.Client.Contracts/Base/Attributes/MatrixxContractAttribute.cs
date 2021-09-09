using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MatrixxContractAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
