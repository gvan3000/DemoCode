using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi;
using System.IO;
using System.Threading.Tasks;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.AzureFunctions.Business.OTACampaignDeleteImsi
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class OTACampaignDeleteImsiFunction
    {
        [FunctionName("OTACampaignDeleteImsiTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "simotacampaigndeleteimsicallbacks/{subscribersListId}/{imsi}")]HttpRequest req,
            string subscribersListId,
            string imsi,
            [Inject]IOTACampaignDeleteImsiCallback processor)
        {

            if (string.IsNullOrWhiteSpace(subscribersListId))
            {
                return new BadRequestObjectResult("No SubscribersListId in request query.");
            }

            if (string.IsNullOrWhiteSpace(imsi))
            {
                return new BadRequestObjectResult("No imsi in request query.");
            }

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<OasisCallback>(requestBody);

                if (data.Status != DeleteIMSIStatus.Active.ToString() && data.Status != DeleteIMSIStatus.Completed.ToString() && data.Status != DeleteIMSIStatus.Failed.ToString())
                {
                    return new BadRequestObjectResult("Provided status is not supported.");
                }

                processor.DeleteImsi(data, subscribersListId, imsi);

                return new OkResult();
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}