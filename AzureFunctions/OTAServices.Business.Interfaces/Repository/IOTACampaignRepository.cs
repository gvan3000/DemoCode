using OTAServices.Business.Entities.OTACampaign;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IOTACampaignRepository
    {
        void AddCampaign(Campaign campaign);
        void UpdateCampaign(Campaign campaign);
        Campaign GetCampaign(int id);
    }
}
