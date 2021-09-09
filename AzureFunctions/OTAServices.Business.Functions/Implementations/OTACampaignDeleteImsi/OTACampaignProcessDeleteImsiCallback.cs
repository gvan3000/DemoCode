using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignDeleteImsi
{
    public class OTACampaignProcessDeleteImsiCallback : IOTACampaignProcessDeleteImsiCallback
    {
        private readonly IJsonLogger _logger;
        private readonly IOtaDbUnitOfWork _otaDbUnitOfWork;

        public OTACampaignProcessDeleteImsiCallback(IOtaDbUnitOfWork otaDbUnitOfWork, IJsonLogger logger)
        {
            _otaDbUnitOfWork = otaDbUnitOfWork;
            _logger = logger;
        }

        public async Task UpdateDeleteImsiCallback(DeleteImsiCallbackResult callback)
        {
            _logger.LogEntry($"Started handling {nameof(OTACampaignProcessDeleteImsiCallback)}.{nameof(UpdateDeleteImsiCallback)} for {nameof(callback.Imsi)}={callback.Imsi}.");

            try
            {
                _otaDbUnitOfWork.BeginTransaction();

                if (!Guid.TryParse(callback.SubscriberListId, out var subscriberListId))
                {
                    throw new ArgumentException($"{nameof(callback.SubscriberListId)} is invalid.");
                }

                var oasisRequest = _otaDbUnitOfWork.OasisRequestRepository.GetByIccidAndSubscriberListId(callback.Iccid, subscriberListId);

                if (oasisRequest == null)
                {
                    throw new InvalidOperationException($"OasisRequest not found for {nameof(callback.Imsi)}={callback.Imsi}, and {nameof(callback.Iccid)}={callback.Iccid}.");
                }

                var deleteImsiCallback = _otaDbUnitOfWork.DeleteIMSICallbackRepository.GetByImsiAndOasisRequestId(callback.Imsi, oasisRequest.Id);

                if (deleteImsiCallback == null)
                {
                    throw new InvalidOperationException($"DeleteIMSICallback not found for {nameof(callback.Imsi)}={callback.Imsi}, and OasisRequest Id {oasisRequest.Id}.");
                }

                if (deleteImsiCallback.Status != DeleteIMSIStatus.Completed.ToString() && deleteImsiCallback.Status != DeleteIMSIStatus.Failed.ToString())
                {
                    deleteImsiCallback.Status = callback.Status;
                    deleteImsiCallback.ModificationDate = DateTime.UtcNow;

                    _otaDbUnitOfWork.DeleteIMSICallbackRepository.Update(deleteImsiCallback);
                }
                else
                {
                    _logger.LogExit($"Handling of {nameof(OTACampaignProcessDeleteImsiCallback)}.{nameof(UpdateDeleteImsiCallback)} for {nameof(callback.Imsi)}={callback.Imsi} failed, Oasis already provided final status for this Imsi.");
                }
                
                _logger.LogExit($"Finished handling {nameof(OTACampaignProcessDeleteImsiCallback)}.{nameof(UpdateDeleteImsiCallback)} for {nameof(callback.Imsi)}={callback.Imsi}");

                await _otaDbUnitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _otaDbUnitOfWork.RollbackTransaction();
                _logger.LogException(ex, $"Failed to update delete imsi callback for {nameof(callback.Imsi)}={callback.Imsi}.");
                throw;
            }
        }
    }
}