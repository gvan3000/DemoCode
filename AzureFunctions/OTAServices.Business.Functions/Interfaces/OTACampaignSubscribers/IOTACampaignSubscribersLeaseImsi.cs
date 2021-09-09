using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersLeaseImsi
    {
        Task<OTACampaignSubscribersLeaseImsiResult> LeaseImsisAsync(OTACampaignSubscribersValidateResult input);
    }
}
