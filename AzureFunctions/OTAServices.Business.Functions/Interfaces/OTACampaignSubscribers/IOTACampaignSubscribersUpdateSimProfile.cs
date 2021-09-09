using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.RollbackStarters.OTACampaignSubscribers;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersUpdateSimProfile
    {
        Task<OTACampaignSubscribersSetSimProfileResult> SetSimProfileForUiccidBatch(OTACampaignSubscribersValidateResult data);
        void Rollback(OTACampaignSubscribersUpdateSimProfileRollback uiccidSimProfileIds);
    }
}
