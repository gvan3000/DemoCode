using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersSaveOasisRequestsRollback
    {
        public OTACampaignSubscribersSaveOasisRequestsRollback(string fileName, List<int> savedOasisRequestsIds)
        {
            FileName = fileName;
            SavedOasisRequestsIds = savedOasisRequestsIds;
        }

        [DataMember]
        public string FileName { get; private set; }
        [DataMember]
        public List<int> SavedOasisRequestsIds { get; set; }
    }
}
