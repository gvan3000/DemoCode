using OTAServices.Business.Entities.SimManagement;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersSetSimProfileResult
    {
        public OTACampaignSubscribersSetSimProfileResult(List<UiccidSimProfileId> uiccidSimProfileIds)
        {
            UiccidSimProfileIds = uiccidSimProfileIds;
        }

        [DataMember]
        public List<UiccidSimProfileId> UiccidSimProfileIds { get; set; }
    }
}
