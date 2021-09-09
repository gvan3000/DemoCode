using OTAServices.Business.Entities.LeaseRequest;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class SubscriberListLeaseRequestRepository : ISubscriberListLeaseRequestRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public SubscriberListLeaseRequestRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddSubscriberListLeaseRequest(SubscriberListLeaseRequest subscriberListLeaseRequest)
        {
            _dbContext.AddEntity(subscriberListLeaseRequest);
        }
        
        #endregion
    }
}
