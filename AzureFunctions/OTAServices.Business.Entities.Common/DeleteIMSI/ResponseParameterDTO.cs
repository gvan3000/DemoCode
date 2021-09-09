using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.Common.DeleteIMSI
{
    [DataContract]
    public class ResponseParameterDTO
    {
        [DataMember(Name = "iccid")]
        public string Iccid { get; set; }

        [DataMember(Name = "porArray")]
        public string[] PorArray { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "tid")]
        public string TransactionId { get; set; }

        [DataMember(Name = "message")]
        public string ErrorMessage { get; set; }
    }
}
