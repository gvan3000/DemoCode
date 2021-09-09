using OTAServices.Business.Entities.OTACampaign;

namespace OTAServices.Business.Common.OTACampaignInterface
{
    public interface IOTACampaignRepository
    {
        void AddCampaign(Campaign campaign);
        void UpdateCampaign(Campaign campaign);
        Campaign GetCampaign(int id);
    }
}
