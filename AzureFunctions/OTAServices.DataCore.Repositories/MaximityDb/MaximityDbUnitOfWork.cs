using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using OTAServices.Business.Interfaces.Repository;
using OTAServices.Business.Interfaces.UnitOfWork;

namespace OTAServices.DataCore.MaximityDb
{
    public class MaximityDbUnitOfWork : IMaximityDbUnitOfWork
    {
        #region [ Private fields ]

        private readonly MaximityDbContext _dbContext;
        private IDbContextTransaction _transaction;

        private ISimInfoRepository _simInfoRepository;
        private IProductInfoRepository _productInfoRepository;
        private IProductProcessLockRepository _productRepository;

        #endregion

        #region [ Constructor ]

        public MaximityDbUnitOfWork(MaximityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region [ IOTACampaignUnitOfWork ] 

        public ISimInfoRepository SimInfoRepository
        {
            get
            {
                if (_simInfoRepository == null)
                {
                    _simInfoRepository = new SimInfoRepository(_dbContext);
                }
                return _simInfoRepository;
            }
        }
        public IProductInfoRepository ProductInfoRepository
        {
            get
            {
                if (_productInfoRepository == null)
                {
                    _productInfoRepository = new ProductInfoRepository(_dbContext);
                }
                return _productInfoRepository;
            }
        }

        public IProductProcessLockRepository ProductProcessLockRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductProcessLockRepository(_dbContext);
                }
                return _productRepository;
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
