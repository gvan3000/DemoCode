using OTAServices.Business.Functions.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersSaveOasisRequestResult
    {
        public OTACampaignSubscribersSaveOasisRequestResult(string fileName, Guid subscribersId, List<int> savedOasisRequestsIds, int validOasisRequestsCount, int campaignId, OTASubscribersListProcessingOperationType otaSubscribersListProcessingOperationType)
        {
            FileName = fileName;
            SubscriberListId = subscribersId;
            SavedOasisRequestsIds = savedOasisRequestsIds;
            ValidOasisRequestsCount = validOasisRequestsCount;
            CampaignId = campaignId;
            OTASubscribersListProcessingOperationType = otaSubscribersListProcessingOperationType;
        }

        [DataMember]
        public string FileName { get; private set; }
        [DataMember]
        public Guid SubscriberListId { get; set; }
        [DataMember]
        public List<int> SavedOasisRequestsIds { get; set; }
        [DataMember]
        public int ValidOasisRequestsCount { get; set; }
        [DataMember]
        public int CampaignId { get; set; }
        [DataMember]
        public OTASubscribersListProcessingOperationType OTASubscribersListProcessingOperationType { get; set; }
    }
}
