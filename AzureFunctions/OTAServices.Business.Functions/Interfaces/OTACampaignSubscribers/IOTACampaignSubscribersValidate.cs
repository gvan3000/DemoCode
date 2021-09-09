using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using SimProfileServiceReference;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersValidate
    {
        Task<OTACampaignSubscribersValidateResult> ValidateAsync(OTACampaignSubscribersParseDataResult input);
    }
}
