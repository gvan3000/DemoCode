using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersLeaseImsiResult
    {
        public OTACampaignSubscribersLeaseImsiResult(string fileName, List<OasisRequest> validatedOasisRequests, Guid subscribersId, Dictionary<string, int> simImsiesCount, OTASubscribersListProcessingOperationType otaSubscribersListProcessingOperationType)
        {
            FileName = fileName;
            ValidatedOasisRequests = validatedOasisRequests;
            SubscriberListId = subscribersId;
            SimImsiesCount = simImsiesCount;
            OTASubscribersListProcessingOperationType = otaSubscribersListProcessingOperationType;
        }

        [DataMember]
        public string FileName { get; private set; }

        [DataMember]
        public List<OasisRequest> ValidatedOasisRequests { get; set; }

        [DataMember]
        public Dictionary<string, int> SimImsiesCount { get; set; }

        [DataMember]
        public Guid SubscriberListId { get; set; }

        [DataMember]
        public OTASubscribersListProcessingOperationType OTASubscribersListProcessingOperationType { get; set; }
    }
}
