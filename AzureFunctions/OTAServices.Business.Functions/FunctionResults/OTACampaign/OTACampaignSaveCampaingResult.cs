using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.FunctionResults.OTACampaign
{

    [DataContract]
    public class OTACampaignSaveCampaingResult
    {
        public OTACampaignSaveCampaingResult(string fileName)
        {
            FileName = fileName;
        }

        [DataMember]
        public string FileName { get; private set; }
    }
    
}
