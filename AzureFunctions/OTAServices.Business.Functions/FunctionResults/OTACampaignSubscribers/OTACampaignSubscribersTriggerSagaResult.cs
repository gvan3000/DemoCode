using OTAServices.Business.Entities.SimManagement;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersTriggerSagaResult
    {
        public OTACampaignSubscribersTriggerSagaResult(string fileName)
        {
            FileName = fileName;
        }

        [DataMember]
        public string FileName { get; private set; }
    }
}
