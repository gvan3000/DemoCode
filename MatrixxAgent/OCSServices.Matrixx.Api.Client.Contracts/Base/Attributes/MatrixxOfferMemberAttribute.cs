using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MatrixxOfferMemberAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
