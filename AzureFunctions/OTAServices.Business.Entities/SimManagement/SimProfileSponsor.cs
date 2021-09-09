using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.SimManagement
{
    [DataContract]
    public class SimProfileSponsor
    {
        [DataMember]
        public int SimProfileId { get; set; }
        [DataMember]
        public string SponsorPrefix { get; set; }
        [DataMember]
        public string SponsorExternalId { get; set; }
        [DataMember]
        public string MCC { get; set; }
    }
}
