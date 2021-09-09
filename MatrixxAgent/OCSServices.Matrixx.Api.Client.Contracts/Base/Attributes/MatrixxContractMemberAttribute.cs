using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MatrixxContractMemberAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
