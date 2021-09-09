using System;
using System.Collections.Generic;
using System.Linq;
using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.OtaDb
{
    public class OasisRequestRepository : IOasisRequestRepository
    {
        #region [ Constants ]

        private const int BatchSize = 1000;

        #endregion

        #region [ Private fields ]

        private readonly OtaDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public OasisRequestRepository(OtaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region [ IOasisRequestRepository implementation ]

        public void AddOasisRequests(List<OasisRequest> oasisRequests)
        {
            _dbContext.OasisRequest.AddRange(oasisRequests);
        }

        public void DeleteOasisRequests(List<int> ids)
        {
            var batches = ids.BatchBy(BatchSize);

            foreach (var batch in batches)
            {
                _dbContext.OasisRequest.RemoveRange(_dbContext.OasisRequest.Where(x => batch.Contains(x.Id)));
            }
        }

        public OasisRequest GetByIccidAndSubscriberListId(string iccid, Guid subscriberListId)
        {
            return _dbContext.OasisRequest.FirstOrDefault(x => x.Iccid == iccid && x.SubscriberListId == subscriberListId);
        }

        #endregion
    }
}
