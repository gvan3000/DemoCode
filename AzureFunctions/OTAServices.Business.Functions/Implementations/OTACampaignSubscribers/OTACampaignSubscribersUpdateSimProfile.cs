using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Functions.RollbackStarters.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersUpdateSimProfile : IOTACampaignSubscribersUpdateSimProfile
    {
        private readonly IJsonLogger _logger;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;
        private const int UpdateSimWithSimProfileBatchSize = 10000;

        public OTACampaignSubscribersUpdateSimProfile(IProvisioningDbUnitOfWork provisioningDbUnitOfWork, IJsonLogger logger)
        {
            _provisioningDbUnitOfWork = provisioningDbUnitOfWork;
            _logger = logger;
        }


        public void Rollback(OTACampaignSubscribersUpdateSimProfileRollback input)
        {
            _logger.LogEntry(input);
            try
            {
                var batches = input.UiccidSimProfileIds.BatchBy(UpdateSimWithSimProfileBatchSize);

                foreach (var batch in batches)
                {
                    _provisioningDbUnitOfWork.SimOrderLineRepository.SetSimOrderLineSimProfileIdBatch(batch);
                }
                _logger.LogExit(null);
            }
            catch(Exception e)
            {
                _logger.LogException(e, $"Failed to rollback {nameof(OTACampaignSubscribersUpdateSimProfile)}");
                throw;
            }
        }

        public async Task<OTACampaignSubscribersSetSimProfileResult> SetSimProfileForUiccidBatch(OTACampaignSubscribersValidateResult data)
        {
            _logger.LogEntry(data);

            try
            {
                _provisioningDbUnitOfWork.BeginTransaction();

                var batches = data.ValidatedOasisRequests.Where(x=>String.IsNullOrEmpty(x.ErrorMessage)).BatchBy(UpdateSimWithSimProfileBatchSize);

                var oldConfigurationData = new List<UiccidSimProfileId>();

                foreach (var batch in batches)
                {

                    var oldData = _provisioningDbUnitOfWork.SimOrderLineRepository.GetSimProfileByUiccidBatch(batch.Select(x => x.Iccid).ToList());
                    oldConfigurationData.AddRange(oldData);

                    var newData = batch.Select(x => new UiccidSimProfileId { SimProfileId = x.TargetSimProfileId, Uiccid = x.Iccid }).ToList();

                    _provisioningDbUnitOfWork.SimOrderLineRepository.SetSimOrderLineSimProfileIdBatch(newData);
                }

                await _provisioningDbUnitOfWork.CommitTransactionAsync();

                var result = new OTACampaignSubscribersSetSimProfileResult(oldConfigurationData);

                _logger.LogExit(result);
                return result;
            }
            catch(Exception e)
            {
                _provisioningDbUnitOfWork.RollbackTransaction();
                _logger.LogException(e, $"Failed to Set SimProfile {nameof(OTACampaignSubscribersUpdateSimProfile)} for file name {data.FileName}");
                throw;
            }
        }
    }
}