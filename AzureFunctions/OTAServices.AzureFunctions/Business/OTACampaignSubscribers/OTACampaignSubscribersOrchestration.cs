using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using OTAServices.Business.Entities.LeaseRequest;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Functions.RollbackStarters.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.AzureFunctions.Business.OTACampaignSubscribers
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class OTACampaignSubscribersOrchestration
    {
        [FunctionName("OTACampaignSubscribersTrigger")]
        public static async System.Threading.Tasks.Task StartAsync(
            [BlobTrigger("otacampaignsubscribers/{name}")]Stream otaCampaignSubscribersData,
            string name,
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            var fileContent = new StreamReader(otaCampaignSubscribersData).ReadToEnd();

            await starter.StartNewAsync("OTACampaignSubscribersOrchestration", new OTACampaignSubscribersStarter(fileContent, name));
        }

        [FunctionName("OTACampaignSubscribersOrchestration")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger]DurableOrchestrationContext context, 
            [Inject]IJsonLogger logger)
        {
            var savedOasisRequestsIds = new List<int>();
            var lockedProductIccids = new List<string>();
            var oldSimOrderLineSimProfiles = new List<UiccidSimProfileId>();

            //Setup infinite retry, for Finalize, and Rollback steps
            var infiniteRetryOptions = new RetryOptions(TimeSpan.FromHours(1), int.MaxValue);

            try
            {
                var input = context.GetInput<OTACampaignSubscribersStarter>();
                logger.LogEntry(input);

                var parseResult = await context.CallActivityAsync<OTACampaignSubscribersParseDataResult>("OTACampaignSubscribersOrchestration_ParseCSV", input);

                var validationResult = await context.CallActivityAsync<OTACampaignSubscribersValidateResult>("OTACampaignSubscribersOrchestration_Validate", parseResult);
                validationResult.InstanceId = context.InstanceId;

                var lockResult = await context.CallActivityAsync<OTACampaignSubscribersLockProductsResult>("OTACampaignSubscribersOrchestration_LockProducts", validationResult);
                lockedProductIccids.AddRange(validationResult.ValidatedOasisRequests.Where(x => string.IsNullOrEmpty(x.ErrorMessage)).Select(x => x.Iccid));

                var updateSimProfileResult = await context.CallActivityAsync<OTACampaignSubscribersSetSimProfileResult>("OTACampaignSubscribersOrchestration_UpdateSimProfile", validationResult);
                oldSimOrderLineSimProfiles = updateSimProfileResult.UiccidSimProfileIds;

                var leaseCompletedTask = context.WaitForExternalEvent<bool>("LeaseCompleted");
                var leaseImsisResult = await context.CallActivityAsync<OTACampaignSubscribersLeaseImsiResult>("OTACampaignSubscribersOrchestration_Lease", validationResult);

                using (var timeoutCts = new CancellationTokenSource())
                {
                    var timeoutAt = context.CurrentUtcDateTime.AddHours(4);
                    var timeoutTask = context.CreateTimer(timeoutAt, timeoutCts.Token);

                    if (leaseCompletedTask == await Task.WhenAny(leaseCompletedTask, timeoutTask))
                    {
                        timeoutCts.Cancel();
                        if (!leaseCompletedTask.Result) throw new InvalidOperationException($"IMSI lease failed. Instance Id: {context.InstanceId }");
                    }
                    else
                    {
                        throw new InvalidOperationException($"IMSI lease failed. Timeout exception. Instance Id: {context.InstanceId }");
                    }
                }

                var enrichOasisRequestsResult = await context.CallActivityAsync<OTACampaignSubscribersEnrichOasisRequestResult>("OTACampaignSubscribersOrchestration_EnrichOasisRequest", leaseImsisResult);

                var saveOasisRequestsResult = await context.CallActivityAsync<OTACampaignSubscribersSaveOasisRequestResult>("OTACampaignSubscribersOrchestration_SaveOasisRequest", enrichOasisRequestsResult);
                savedOasisRequestsIds.AddRange(saveOasisRequestsResult.SavedOasisRequestsIds);

                var triggerSagaResult = await context.CallActivityAsync<OTACampaignSubscribersTriggerSagaResult>("OTACampaignSubscribersOrchestration_TriggerSaga", saveOasisRequestsResult);

                await context.CallActivityWithRetryAsync("OTACampaignSubscribersOrchestration_Finalize", infiniteRetryOptions ,triggerSagaResult);
                
                logger.LogExit(null);
            }
            catch (Exception exc)
            {
                if (!context.IsReplaying)
                {
                    logger.LogException(exc, $"Failed to Sync OTA Campaign Subscribers data with file {context.GetInput<OTACampaignSubscribersStarter>().FileName}.");
                }

                try
                {
                    //Run Compensation`s
                    if (savedOasisRequestsIds.Count != 0)
                    {
                        var otaCampaignSubscribersImportDataRollback = new OTACampaignSubscribersSaveOasisRequestsRollback(context.GetInput<OTACampaignSubscribersStarter>().FileName, savedOasisRequestsIds);
                        await context.CallActivityWithRetryAsync("OTACampaignSubscribersOrchestration_RollbackImportData", infiniteRetryOptions, otaCampaignSubscribersImportDataRollback);
                    }
                    if (lockedProductIccids.Count != 0)
                    {
                        var otaCampaignSubscribersLockProductsRollback = new OTACampaignSubscribersLockProductsRollback(context.GetInput<OTACampaignSubscribersStarter>().FileName, lockedProductIccids);
                        await context.CallActivityWithRetryAsync("OTACampaignSubscribersOrchestration_RollbackLockProducts", infiniteRetryOptions, otaCampaignSubscribersLockProductsRollback);
                    }

                    if (oldSimOrderLineSimProfiles.Count != 0)
                    {
                        var otaCampaignUpdateSimProfileRollback = new OTACampaignSubscribersUpdateSimProfileRollback(context.GetInput<OTACampaignSubscribersStarter>().FileName, oldSimOrderLineSimProfiles);
                        await context.CallActivityWithRetryAsync("OTACampaignSubscribersOrchestration_RollbackSimProfile", infiniteRetryOptions, otaCampaignUpdateSimProfileRollback);
                    }

                    await context.CallActivityWithRetryAsync("OTACampaignSubscribersOrchestration_Error", infiniteRetryOptions, context.GetInput<OTACampaignSubscribersStarter>());
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

        [FunctionName("OTACampaignSubscribersOrchestration_ParseCSV")]
        public static OTACampaignSubscribersParseDataResult OTACampaignSubscribersOrchestration_ParseCSV(
            [ActivityTrigger]OTACampaignSubscribersStarter input,
            [Inject]IOTACampaignSubscribersParseData parser)
        {
            var result = parser.Parse(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_Validate")]
        public static async Task<OTACampaignSubscribersValidateResult> OTACampaignSubscribersOrchestration_ValidateAsync(
            [ActivityTrigger]OTACampaignSubscribersParseDataResult input,
            [Inject]IOTACampaignSubscribersValidate validator)
        {
            var result = await validator.ValidateAsync(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_LockProducts")]
        public static async Task<OTACampaignSubscribersLockProductsResult> OTACampaignSubscribersOrchestration_LockProductsAsync(
            [ActivityTrigger]OTACampaignSubscribersValidateResult input,
            [Inject]IOTACampaignSubscribersLockProducts locker)
        {
            var result = await locker.LockProductsAsync(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_RollbackLockProducts")]
        public static void OTACampaignSubscribersOrchestration_RollbackLockProducts(
            [ActivityTrigger]OTACampaignSubscribersLockProductsRollback input,
            [Inject]IOTACampaignSubscribersLockProducts locker)
        {
            locker.Rollback(input);
        }

        [FunctionName("OTACampaignSubscribersOrchestration_Lease")]
        public static async Task<OTACampaignSubscribersLeaseImsiResult> OTACampaignSubscribersOrchestration_Lease(
            [ActivityTrigger] OTACampaignSubscribersValidateResult input,
            [Inject]IOTACampaignSubscribersLeaseImsi leaser)
        {
            var result = await leaser.LeaseImsisAsync(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_WaitForASB")]
        public static async Task OTACampaignSubscribersOrchestration_WaitForASB(
            [ServiceBusTrigger("imsies.lease.finished", Connection = "ServiceBusConnection")]string myQueueItem,
            [OrchestrationClient]DurableOrchestrationClient client)
        {
            var otaCampaignFinished = JsonConvert.DeserializeObject<LeaseForOTACampaignFinished>(myQueueItem);

            await client.RaiseEventAsync(otaCampaignFinished.InstanceId, "LeaseCompleted", otaCampaignFinished.IsSuccedeed);
        }

        [FunctionName("OTACampaignSubscribersOrchestration_EnrichOasisRequest")]
        public static async Task<OTACampaignSubscribersEnrichOasisRequestResult> OTACampaignSubscribersOrchestration_EnrichOasisRequestAsync(
            [ActivityTrigger]OTACampaignSubscribersLeaseImsiResult input,
            [Inject]IOTACampaignSubscribersEnrichOasisRequest importer)
        {
            var result = await importer.EnrichOasisRequestAsync(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_SaveOasisRequest")]
        public static async Task<OTACampaignSubscribersSaveOasisRequestResult> OTACampaignSubscribersOrchestration_SaveOasisRequestAsync(
            [ActivityTrigger]OTACampaignSubscribersEnrichOasisRequestResult input,
            [Inject]IOTACampaignSubscribersSaveOasisRequest importer)
        {
            var result = await importer.SaveOasisRequestAsync(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_RollbackImportData")]
        public static async Task OTACampaignSubscribersOrchestration_RollbackImportDataAsync(
            [ActivityTrigger]OTACampaignSubscribersSaveOasisRequestsRollback input,
            [Inject]IOTACampaignSubscribersSaveOasisRequest importer)
        {
            await importer.RollbackAsync(input);
        }

        [FunctionName("OTACampaignSubscribersOrchestration_TriggerSaga")]
        public static Task<OTACampaignSubscribersTriggerSagaResult> OTACampaignSubscribersOrchestration_TriggerSaga(
            [ActivityTrigger]OTACampaignSubscribersSaveOasisRequestResult input,
            [Inject]IOTACampaignSubscribersTriggerSaga sagaStarter,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Started Saga for processing Subscribers List in OTA.Bus.");
            var result = sagaStarter.TriggerSaga(input);

            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_Finalize")]
        public static void OTACampaignSubscribersOrchestration_Finalize(
            [ActivityTrigger]OTACampaignSubscribersTriggerSagaResult input,
            [Inject]IOTACampaignSubscribersFinalizeProcessing finalizer,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Moving file to Processed folder. Deleting Blob.");

            finalizer.FinalizeWithSuccess(input);
        }

        [FunctionName("OTACampaignSubscribersOrchestration_Error")]
        public static void OTACampaignSubscribersOrchestration_Error(
            [ActivityTrigger]OTACampaignSubscribersStarter input,
            [Inject]IOTACampaignSubscribersFinalizeProcessing finalizer,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Moving file to Error folder. Deleting Blob.");

            finalizer.FinalizeWithError(input);
        }

        [FunctionName("OTACampaignSubscribersOrchestration_UpdateSimProfile")]
        public static Task<OTACampaignSubscribersSetSimProfileResult> OTACampaignSubscribersOrchestration_UpdateSimProfile(
            [ActivityTrigger]OTACampaignSubscribersValidateResult input,
            [Inject]IOTACampaignSubscribersUpdateSimProfile updater)
        {
            var result = updater.SetSimProfileForUiccidBatch(input);
            
            return result;
        }

        [FunctionName("OTACampaignSubscribersOrchestration_RollbackSimProfile")]
        public static void OTACampaignSubscribersOrchestration_UpdateSimProfileError(
            [ActivityTrigger]OTACampaignSubscribersUpdateSimProfileRollback input,
            [Inject]IOTACampaignSubscribersUpdateSimProfile updater)
        {
            updater.Rollback(input);
        }
    }
}