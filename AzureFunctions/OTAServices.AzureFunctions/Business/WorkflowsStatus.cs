using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.AzureFunctions.Business
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class WorkflowsStatus
    {
        [FunctionName("GetAllStatus")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient client,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Listing all in-proggress Workflow instances:");

            var counter = 0;

            var instances = await client.GetStatusAsync();
            foreach (var instance in instances)
            {
                counter++;
                logger.LogMessage(JsonConvert.SerializeObject(instance));
            };

            if (counter == 0)
            {
                logger.LogMessage("There are no in-progress Workflow instances.");
            }
        }
    }
}