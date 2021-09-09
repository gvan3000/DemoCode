using Microsoft.Azure.WebJobs;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using System.IO;
using System.Threading.Tasks;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.AzureFunctions.Business.OTACampaign
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class OTACampaignOrchestration
    {
        [FunctionName("OTACampaignTrigger")]
        public static async Task StartAsync([BlobTrigger("otacampaign/{name}")]Stream otaCampaignData, string name, [OrchestrationClient]DurableOrchestrationClient starter)
        {
            var fileContent = new StreamReader(otaCampaignData).ReadToEnd();

            await starter.StartNewAsync("OTACampaignOrchestration", new OTACampaignStarter(fileContent, name));
        }

        [FunctionName("OTACampaignOrchestration")]
        public static async Task RunOrchestrator([OrchestrationTrigger]DurableOrchestrationContext context, [Inject]IJsonLogger logger)
        {
            try
            {
                var input = context.GetInput<OTACampaignStarter>();
                logger.LogEntry(input);

                //Parsing
                var parseResult = await context.CallActivityAsync<OTACampaignParseDataResult>("OTACampaignOrchestration_ParseCSV", input);

                //Validation
                var validationResult = await context.CallActivityAsync<OTACampaignParseDataResult>("OTACampaignOrchestration_Validate", parseResult);

                //Import
                var importResult = await context.CallActivityAsync<OTACampaignSaveCampaingResult>("OTACampaignOrchestration_SaveCampaing", validationResult);

                //Finalize
                await context.CallActivityAsync("OTACampaignOrchestration_Finalize", importResult);

                logger.LogExit(importResult);
            }
            catch (Exception exc)
            {
                if (!context.IsReplaying)
                {
                    logger.LogException(exc, $"Failed to Sync OTA Campaign data with file {context.GetInput<OTACampaignStarter>().FileName}.");
                }

                try
                {
                    await context.CallActivityAsync("OTACampaignOrchestration_Error", context.GetInput<OTACampaignStarter>());
                }
                catch (Exception excErrorHandlingFailed)
                {
                    if (!context.IsReplaying)
                    {
                        logger.LogException(excErrorHandlingFailed, $"Failed to move file {context.GetInput<OTACampaignStarter>().FileName} to Error folder.");
                    }
                }
            }
        }

        [FunctionName("OTACampaignOrchestration_ParseCSV")]
        public static OTACampaignParseDataResult OTACampaignOrchestration_ParseCSV([ActivityTrigger]OTACampaignStarter input, [Inject]IOTACampaignParseData parser)
        {
            var result = parser.Parse(input);

            return result;
        }

        [FunctionName("OTACampaignOrchestration_Validate")]
        public static async Task<OTACampaignParseDataResult> OTACampaignOrchestration_ValidateAsync([ActivityTrigger]OTACampaignParseDataResult input, [Inject]IOTACampaignValidateData validator)
        {
            var result = await validator.ValidateAsync(input);

            return result;
        }
        
        [FunctionName("OTACampaignOrchestration_SaveCampaing")]
        public static async Task<OTACampaignSaveCampaingResult> OTACampaignOrchestration_SaveCampaingAsync([ActivityTrigger] OTACampaignParseDataResult input, [Inject]IOTACampaignSaveCampaing importer)
        {
            var result = await importer.SaveCampaingAsync(input);

            return result;
        }

        [FunctionName("OTACampaignOrchestration_Finalize")]
        public static void OTACampaignOrchestration_Finalize([ActivityTrigger]OTACampaignSaveCampaingResult input, [Inject]IOTACampaignFinalizeProcessing finalizer, [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Moving file to Processed folder. Deleting Blob.");

            finalizer.FinalizeWithSuccess(input);
        }

        [FunctionName("OTACampaignOrchestration_Error")]
        public static void OTACampaignOrchestration_Error([ActivityTrigger]OTACampaignStarter input, [Inject]IOTACampaignFinalizeProcessing finalizer, [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Moving file to Error folder. Deleting Blob.");

            finalizer.FinalizeWithError(input);
        }
    }
}