using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.SimManagement
{
    [DataContract]
    public class OriginalTargetSimProfilePair
    {
        [DataMember]
        public int TargetSimProfileId { get; set; }
        [DataMember]
        public int? OriginalSimProfileId { get; set; }
    }
}
