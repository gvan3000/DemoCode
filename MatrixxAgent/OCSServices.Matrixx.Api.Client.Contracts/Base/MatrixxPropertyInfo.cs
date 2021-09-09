using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base
{
    public class MatrixxPropertyInfo
    {
        public string MatrixxName { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
        public string PropertyName { get; set; }
    }
}
