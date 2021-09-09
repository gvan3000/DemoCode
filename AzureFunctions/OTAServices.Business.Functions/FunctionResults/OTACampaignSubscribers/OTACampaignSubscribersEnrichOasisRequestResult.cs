using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersEnrichOasisRequestResult
    {

        public OTACampaignSubscribersEnrichOasisRequestResult(string fileName, List<OasisRequest> enrichedOasisRequests, Guid subscirberListId, OTASubscribersListProcessingOperationType otaSubscribersListProcessingOperationType)
        {
            FileName = fileName;
            EnrichedOasisRequests = enrichedOasisRequests;
            SubscriberListId = subscirberListId;
            OTASubscribersListProcessingOperationType = otaSubscribersListProcessingOperationType;
        }

        [DataMember]
        public string FileName { get; private set; }

        [DataMember]
        public List<OasisRequest> EnrichedOasisRequests { get; set; }
        
        [DataMember]
        public Guid SubscriberListId { get; set; }

        [DataMember]
        public OTASubscribersListProcessingOperationType OTASubscribersListProcessingOperationType { get; set; }

    }
}
