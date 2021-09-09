using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using OTAServices.Business.Interfaces.Repository;
using OTAServices.Business.Interfaces.UnitOfWork;

namespace OTAServices.DataCore.Repositories.OtaDb
{
    public class OtaDbUnitOfWork : IOtaDbUnitOfWork
    {
        #region [ Private fields ]

        private readonly OtaDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private IOasisRequestRepository _oasisRequestRepository;
        private IOTACampaignRepository _OTACampaignRepository;
        private IDeleteIMSICallbackRepository _deleteIMSICallbackRepository;
        #endregion

        public OtaDbUnitOfWork(OtaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IOasisRequestRepository OasisRequestRepository
        {
            get
            {
                if(_oasisRequestRepository == null)
                {
                    _oasisRequestRepository = new OasisRequestRepository(_dbContext);
                }
                return _oasisRequestRepository;
            }
        }

        public IOTACampaignRepository OTACampaignRepository
        {
            get
            {
                if (_OTACampaignRepository == null)
                {
                    _OTACampaignRepository = new OTACampaignRepository(_dbContext);
                }
                return _OTACampaignRepository;
            }
        }

        public IDeleteIMSICallbackRepository DeleteIMSICallbackRepository
        {
            get
            {
                if (_deleteIMSICallbackRepository == null)
                {
                    _deleteIMSICallbackRepository = new DeleteIMSICallbackRepository(_dbContext);
                }
                return _deleteIMSICallbackRepository;
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
    }
}
