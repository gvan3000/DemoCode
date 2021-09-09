using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersFinalizeProcessing
    {
        void FinalizeWithSuccess(OTACampaignSubscribersTriggerSagaResult input);
        void FinalizeWithError(OTACampaignSubscribersStarter input);
    }
}
