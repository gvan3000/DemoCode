using Newtonsoft.Json;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi;
using System;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignDeleteImsi
{
    public class OTACampaignDeleteImsiCallback : IOTACampaignDeleteImsiCallback
    {
        private readonly IJsonLogger _logger;
        private readonly IDeleteImsiCallbackResponseQueueClient _deleteImsiCallbackResponseQueueClient;

        public OTACampaignDeleteImsiCallback(IDeleteImsiCallbackResponseQueueClient queueClient, IJsonLogger logger)
        {
            _deleteImsiCallbackResponseQueueClient = queueClient;
            _logger = logger;
        }

        public async Task DeleteImsi(OasisCallback callback, string subscribersListId, string imsi)
        {
            _logger.LogEntry($"Started handling {nameof(OTACampaignDeleteImsiCallback)}.{nameof(DeleteImsi)} for {nameof(subscribersListId)}={subscribersListId} and {nameof(imsi)} = {imsi}.");

            try
            {
                var azureBusMessage = new DeleteImsiCallbackResult()
                {
                    SubscriberListId = subscribersListId,
                    Iccid = callback.Iccid,
                    Imsi = imsi,
                    Status = callback.Status
                };

                await _deleteImsiCallbackResponseQueueClient.SendToQueue(JsonConvert.SerializeObject(azureBusMessage));

                _logger.LogExit($"Finished handling {nameof(OTACampaignDeleteImsiCallback)}.{nameof(DeleteImsi)} for {nameof(subscribersListId)}={subscribersListId} and {nameof(imsi)} = {imsi}.");

            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to sent message to queue for {nameof(subscribersListId)} = {subscribersListId} and {nameof(imsi)} = {imsi}.");
                throw;
            }
        }
    }
}