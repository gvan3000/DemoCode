using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.ImsiManagement;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class ImsiSponsorsStatusRepository : IImsiSponsorsStatusRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public ImsiSponsorsStatusRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        public List<ImsiSponsorsStatus> GetImsiSponsorsStatusBySimProfileId(int simProfileId)
        {
            var result = _dbContext.ImsiSponsorStatus.FromSql("SELECT * FROM GetAvailableImsisCountBySponsorsForSimProfile({0})", simProfileId).ToList();

            return result;
        }
    }
}
