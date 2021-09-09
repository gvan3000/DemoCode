using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.AzureFunctions.Business.OTACampaignDeleteImsi
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class OTACampaignUpdateCallbackStatusFunction
    {
        [FunctionName("OTACampaignUpdateCallbackStatusFunction")]
        public static async void RunAsync(
            [ServiceBusTrigger("deleteimsicallbackresuts", Connection = "ServiceBusConnection")]string myQueueItem,
            [Inject]IOTACampaignProcessDeleteImsiCallback processor)
        {
            var callbackResult = JsonConvert.DeserializeObject<DeleteImsiCallbackResult>(myQueueItem);

            await processor.UpdateDeleteImsiCallback(callbackResult);
        }
    }
}