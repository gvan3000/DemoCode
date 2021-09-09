using System.Runtime.Serialization;

namespace OTAServices.Business.Functions.WorkflowStarters.OTACampaign
{
    [DataContract]
    public class OTACampaignStarter
    {
        public OTACampaignStarter(string fileContent, string fileName)
        {
            FileContent = fileContent;
            FileName = fileName;
        }

        [DataMember]
        public string FileContent { get; private set; }
        [DataMember]
        public string FileName { get; private set; }
    }
}
