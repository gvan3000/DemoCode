using Newtonsoft.Json;
using OTAServices.Bus.Contracts.Events.Subscriber;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using System;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersTriggerSaga : IOTACampaignSubscribersTriggerSaga
    {
        private readonly string _partialDeleteImsiCallbackUrl;
        private readonly IJsonLogger _logger;
        private readonly IOTAServicesBusTopicClient _otaServicesBusTopicClient;

        public OTACampaignSubscribersTriggerSaga(IOTAServicesBusTopicClient otaServicesBusTopicClient, string partialDeleteImsiCallbackUrl, IJsonLogger logger)
        {
            _otaServicesBusTopicClient = otaServicesBusTopicClient;
            _partialDeleteImsiCallbackUrl = partialDeleteImsiCallbackUrl;
            _logger = logger;
        }

        public async Task<OTACampaignSubscribersTriggerSagaResult> TriggerSaga(OTACampaignSubscribersSaveOasisRequestResult input)
        {
            _logger.LogEntry(input);

            var res = new OTACampaignSubscribersTriggerSagaResult(input.FileName);

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented,
            };

            try
            {
                switch (input.OTASubscribersListProcessingOperationType)
                {
                    case Common.OTASubscribersListProcessingOperationType.AddImsies:
                        {
                            var sagaTrigger = new InitializedProcessSubscriberListToOasis()
                            {
                                BusinessCaseId = Guid.NewGuid(),
                                CampaignId = input.CampaignId,
                                IccidCount = input.ValidOasisRequestsCount,
                                SubscriberListId = input.SubscriberListId,
                                ActionId = Guid.NewGuid()
                            };

                            await _otaServicesBusTopicClient.SendToTopic(JsonConvert.SerializeObject(sagaTrigger, jsonSerializerSettings));

                            _logger.LogExit(null);
                           
                            break;
                        }
                    case Common.OTASubscribersListProcessingOperationType.DeleteImsies:
                        {
                            var sagaTrigger = new InitializedProcessSubscriberListToOasisDeleteImsi()
                            {
                                BusinessCaseId = Guid.NewGuid(),
                                CampaignId = input.CampaignId,
                                IccidCount = input.ValidOasisRequestsCount,
                                SubscriberListId = input.SubscriberListId,
                                ActionId = Guid.NewGuid(),
                                DeleteImsiCallbackUrlPreffix = $"{_partialDeleteImsiCallbackUrl}/{input.SubscriberListId}/"
                            };

                            await _otaServicesBusTopicClient.SendToTopic(JsonConvert.SerializeObject(sagaTrigger, jsonSerializerSettings));

                            _logger.LogExit(null);
                            
                            break;
                        }
                    case Common.OTASubscribersListProcessingOperationType.UpdatePlmnLists:
                        {
                            throw new InvalidOperationException($"Update PLMN list use-case detected, implementation for this use case is not finished.");
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unsupported OTASubscribersListProcessingOperationType detected.");
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to trigger saga for {input.FileName}.");
                throw;
            }

            return res;
        }
    }
}