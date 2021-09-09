using System.Linq;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.OtaDb
{
    public class OTACampaignRepository : IOTACampaignRepository
    {
        #region [ Private fields ]

        private readonly OtaDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public OTACampaignRepository(OtaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region [ IOTACampaignRepository ]

        public void AddCampaign(Campaign campaign)
        {
            _dbContext.AddEntity(campaign);
        }

        public Campaign GetCampaign(int id)
        {
            return _dbContext.Campaign.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateCampaign(Campaign campaign)
        {
            _dbContext.UpdateEntity(campaign);
        }

        #endregion
    }
}
