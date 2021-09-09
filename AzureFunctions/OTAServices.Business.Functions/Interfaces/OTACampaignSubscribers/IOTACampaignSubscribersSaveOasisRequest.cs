using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersSaveOasisRequest
    {
        Task<OTACampaignSubscribersSaveOasisRequestResult> SaveOasisRequestAsync(OTACampaignSubscribersEnrichOasisRequestResult input);

        Task RollbackAsync(OTACampaignSubscribersSaveOasisRequestsRollback input);
    }
}
