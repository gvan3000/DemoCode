using System.Collections.Generic;
using System.Runtime.Serialization;
using OTAServices.Business.Entities.OTACampaign;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaign
{
    [DataContract]
    public class OTACampaignParseDataResult
    {
        public OTACampaignParseDataResult(string fileName)
        {
            FileName = fileName;
            Campaigns = new List<Campaign>();
        }

        [DataMember]
        public string FileName { get; private set; }

        [DataMember]
        public List<Campaign> Campaigns { get; }

    }
}
