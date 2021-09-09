using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersSaveOasisRequest : IOTACampaignSubscribersSaveOasisRequest
    {
        private readonly IOtaDbUnitOfWork _otaDbUnitOfWork;
        private readonly IJsonLogger _logger;

        public OTACampaignSubscribersSaveOasisRequest(IOtaDbUnitOfWork otaDbUnitOfWork, IJsonLogger logger)
        {
            _otaDbUnitOfWork = otaDbUnitOfWork;
            _logger = logger;
        }

        public async Task<OTACampaignSubscribersSaveOasisRequestResult> SaveOasisRequestAsync(OTACampaignSubscribersEnrichOasisRequestResult input)
        {
            _logger.LogEntry(input);

            try
            {
                _otaDbUnitOfWork.BeginTransaction();
                
                _otaDbUnitOfWork.OasisRequestRepository.AddOasisRequests(input.EnrichedOasisRequests);

                await _otaDbUnitOfWork.CommitTransactionAsync();

                var result = new OTACampaignSubscribersSaveOasisRequestResult(
                    input.FileName,
                    input.SubscriberListId,
                    input.EnrichedOasisRequests.Select(x => x.Id).ToList(),
                    input.EnrichedOasisRequests.Count(x => string.IsNullOrWhiteSpace(x.ErrorMessage)),
                    input.EnrichedOasisRequests.First().CampaignId,
                    input.OTASubscribersListProcessingOperationType);

                _logger.LogExit(result);
                return result;
            }
            catch (Exception ex)
            {
                _otaDbUnitOfWork.RollbackTransaction();
                _logger.LogException(ex, $"Failed to import data {input.FileName}.");
                throw;
            }
        }

        public async Task RollbackAsync(OTACampaignSubscribersSaveOasisRequestsRollback input)
        {
            _logger.LogEntry(input);

            try
            {
                _otaDbUnitOfWork.OasisRequestRepository.DeleteOasisRequests(input.SavedOasisRequestsIds);

                await _otaDbUnitOfWork.CommitTransactionAsync();

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