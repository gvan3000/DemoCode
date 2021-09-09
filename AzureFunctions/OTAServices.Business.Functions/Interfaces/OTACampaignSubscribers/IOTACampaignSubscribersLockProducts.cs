using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersLockProducts
    {
        Task<OTACampaignSubscribersLockProductsResult> LockProductsAsync(OTACampaignSubscribersValidateResult input);
        void Rollback(OTACampaignSubscribersLockProductsRollback input);
    }
}
