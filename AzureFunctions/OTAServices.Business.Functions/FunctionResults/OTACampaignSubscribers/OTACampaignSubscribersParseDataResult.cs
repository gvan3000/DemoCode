using System.Collections.Generic;
using System.Runtime.Serialization;
using OTAServices.Business.Entities.OTACampaignSubscribers;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers
{
    [DataContract]
    public class OTACampaignSubscribersParseDataResult
    {
        public OTACampaignSubscribersParseDataResult(string fileName, List<OasisRequest> parsedOasisRequests)
        {
            ParsedOasisRequests = parsedOasisRequests;
            FileName = fileName;
        }

        [DataMember]
        public string FileName { get; private set; }

        [DataMember]
        public List<OasisRequest> ParsedOasisRequests { get; set; }
    }
}
