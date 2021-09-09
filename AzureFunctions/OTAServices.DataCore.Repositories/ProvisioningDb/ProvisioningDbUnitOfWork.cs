using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using OTAServices.Business.Interfaces.Repository;
using OTAServices.Business.Interfaces.UnitOfWork;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class ProvisioningDbUnitOfWork : IProvisioningDbUnitOfWork
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;
        private IDbContextTransaction _transaction;

        private ISimContentRepository _simContentRepository;
        private IImsiInfoRepository _imsiInfoRepository;
        private ISimProfileSponsorRepository _simProfileSponsorRepository;
        private ISubscriberListLeaseRequestRepository _leaseRequestRepository;
        private IImsiSponsorsStatusRepository _imsiSponsorsStatusRepository;
        private IProvisioningDataInfoRepository _provisioningDataInfoRepository;
        private ISimOrderLineRepository _simOrderLineRepository;

        #endregion

        #region [ Constructor ]

        public ProvisioningDbUnitOfWork(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region [ IProvisioningDbUnitOfWork ] 

        public IProvisioningDataInfoRepository ProvisioningDataInfoRepository
        {
            get
            {
                if (_provisioningDataInfoRepository == null)
                {
                    _provisioningDataInfoRepository = new ProvisioningDataInfoRepository(_dbContext);
                }
                return _provisioningDataInfoRepository;
            }
        }

        public ISimContentRepository SimContentRepository
        {
            get
            {
                if (_simContentRepository == null)
                {
                    _simContentRepository = new SimContentRepository(_dbContext);
                }
                return _simContentRepository;
            }
        }
        public IImsiInfoRepository ImsiInfoRepository
        {
            get
            {
                if (_imsiInfoRepository == null)
                {
                    _imsiInfoRepository = new ImsiInfoRepository(_dbContext);
                }
                return _imsiInfoRepository;
            }
        }
        public ISimProfileSponsorRepository SimProfileSponsorRepository
        {
            get
            {
                if (_simProfileSponsorRepository == null)
                {
                    _simProfileSponsorRepository = new SimProfileSponsorRepository(_dbContext);
                }
                return _simProfileSponsorRepository;
            }
        }

        public ISubscriberListLeaseRequestRepository LeaseRequestRepository
        {
            get
            {
                if (_leaseRequestRepository == null)
                {
                    _leaseRequestRepository = new SubscriberListLeaseRequestRepository(_dbContext);
                }
                return _leaseRequestRepository;
            }
        }

        public IImsiSponsorsStatusRepository ImsiSponsorsStatusRepository
        {
            get
            {
                if (_imsiSponsorsStatusRepository == null)
                {
                    _imsiSponsorsStatusRepository = new ImsiSponsorsStatusRepository(_dbContext);
                }
                return _imsiSponsorsStatusRepository;
            }
        }

        public ISimOrderLineRepository SimOrderLineRepository
        {
            get
            {
                if (_simOrderLineRepository == null)
                {
                    _simOrderLineRepository = new SimOrderLineRepository(_dbContext);
                }
                return _simOrderLineRepository;
            }
        }

        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            await this._dbContext.SaveChangesAsync();

            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }

        #endregion
    }
}
