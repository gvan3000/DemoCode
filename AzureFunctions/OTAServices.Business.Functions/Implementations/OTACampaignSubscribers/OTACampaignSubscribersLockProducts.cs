using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersLockProducts : IOTACampaignSubscribersLockProducts
    {
        private const int LockProductsBatchSize = 10000;

        private readonly IJsonLogger _logger;
        private readonly IMaximityDbUnitOfWork _maximityDbUnitOfWork;

        public OTACampaignSubscribersLockProducts(IMaximityDbUnitOfWork maximityDbUnitOfWork, IJsonLogger logger)
        {
            _maximityDbUnitOfWork = maximityDbUnitOfWork;
            _logger = logger;
        }

        public async Task<OTACampaignSubscribersLockProductsResult> LockProductsAsync(OTACampaignSubscribersValidateResult input)
        {
            _logger.LogEntry(input);

            try
            {
                _maximityDbUnitOfWork.BeginTransaction();

                var batches = input.ValidatedOasisRequests.Where(x => string.IsNullOrEmpty(x.ErrorMessage)).Select(x=>x.Iccid).ToList().BatchBy(LockProductsBatchSize);

                foreach (var batch in batches)
                {
                    _maximityDbUnitOfWork.ProductProcessLockRepository.AddProductProcessLockBulk(batch);
                }

                var res = new OTACampaignSubscribersLockProductsResult();

                await _maximityDbUnitOfWork.CommitTransactionAsync();

                _logger.LogExit(res);
                return res;
            }
            catch (Exception ex)
            {
                _maximityDbUnitOfWork.RollbackTransaction();
                _logger.LogException(ex, $"Failed to Lock Products for {input.FileName}.");
                throw;
            }
        }

        public void Rollback(OTACampaignSubscribersLockProductsRollback input)
        {
            _logger.LogEntry(input);

            try
            {
                var batches = input.LockedProductIccids.BatchBy(LockProductsBatchSize);

                foreach (var batch in batches)
                {
                    _maximityDbUnitOfWork.ProductProcessLockRepository.DeleteProductProcessLockBulk(batch);
                }
                _logger.LogExit(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to rollback data {input.FileName}.");
                throw;
            }
        }
    }
}