using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersParseData
    {
        OTACampaignSubscribersParseDataResult Parse(OTACampaignSubscribersStarter input);
    }
}
