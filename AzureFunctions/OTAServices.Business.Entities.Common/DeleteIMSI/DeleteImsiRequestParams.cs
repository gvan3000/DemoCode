using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.Common.DeleteIMSI
{
    [DataContract]
    public class DeleteImsiRequestParams
    {
        public DeleteImsiRequestParams(string iccid, string imsi, string baseCallbackUrl)
        {
            EId = iccid;
            Iccid = iccid;
            ImsiValue = imsi;
            CallbackUrl = baseCallbackUrl + "/" + imsi;
        }

        [DataMember(Name = "eid")]
        public string EId { get; set; }

        [DataMember(Name = "iccid")]
        public string Iccid { get; set; }

        [DataMember(Name = "imsiValue")]
        public string ImsiValue { get; set; }

        [DataMember(Name = "callback")]
        public string CallbackUrl { get; set; }
    }
}
