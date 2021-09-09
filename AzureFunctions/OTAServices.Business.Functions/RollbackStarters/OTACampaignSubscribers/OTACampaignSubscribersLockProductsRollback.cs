using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersLockProductsRollback
    {
        public OTACampaignSubscribersLockProductsRollback(string fileName, List<string> lockedProductIccids)
        {
            FileName = fileName;
            LockedProductIccids = lockedProductIccids;
        }

        [DataMember]
        public string FileName { get; private set; }
        [DataMember]
        public List<string> LockedProductIccids { get; set; }
    }
}
