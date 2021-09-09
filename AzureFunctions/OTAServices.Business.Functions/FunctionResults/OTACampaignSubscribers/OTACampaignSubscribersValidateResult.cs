using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersValidateResult
    {
        public OTACampaignSubscribersValidateResult(string fileName, List<OasisRequest> validatedOasisRequests, OTASubscribersListProcessingOperationType otaSubscribersListProcessingOperationType)
        {
            FileName = fileName;
            ValidatedOasisRequests = validatedOasisRequests;
            OTASubscribersListProcessingOperationType = otaSubscribersListProcessingOperationType;
        }

        [DataMember]
        public string FileName { get; private set; }

        [DataMember]
        public List<OasisRequest> ValidatedOasisRequests { get; set; }

        [DataMember]
        public string InstanceId { get; set; }

        [DataMember]
        public OTASubscribersListProcessingOperationType OTASubscribersListProcessingOperationType { get; set; }
    }
}
