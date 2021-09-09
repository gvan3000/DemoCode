using OTAServices.Business.Functions.FunctionResults.OTACampaign;

namespace OTAServices.Business.Functions.Interfaces.OTACampaign
{
    public interface IOTACampaignSaveCampaing
    {
        System.Threading.Tasks.Task<OTACampaignSaveCampaingResult> SaveCampaingAsync(OTACampaignParseDataResult parsedData);
    }
}
