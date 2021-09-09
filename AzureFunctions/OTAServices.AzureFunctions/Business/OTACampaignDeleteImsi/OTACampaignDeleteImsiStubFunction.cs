using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OTAServices.Business.Entities.Common.DeleteIMSI;
using System;
using System.Text;
using Teleena.AzureFunctions.DependencyInjection;
using TeleenaFileLogging.AzureFunctions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.AzureFunctions.Business.OTACampaignDeleteImsi
{
    [AzureFunctionJsonLogger]
    [AzureFunctionExceptionJsonLogger]
    public static class OTACampaignDeleteImsiStubFunction
    {
        private static StubState _currentStubState = StubState.Success;

        [FunctionName("OTACampaignDeleteImsiStubTrigger")]
        public static IActionResult OTACampaignDeleteImsiStubTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OTA.DeleteIMSI")]HttpRequest req, 
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage("Checking Authorization header.");

            var header = req.Headers["Authorization"];

            var expectedAuthorizationHeaderContent = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("testUserName:testPassword"));

            if (header.Count == 0 || header[0] != expectedAuthorizationHeaderContent)
            {
                return new UnauthorizedResult();
            }

            logger.LogMessage("Authorization passed.");

            logger.LogMessage($"Stub is returning {_currentStubState.ToString()}");

            switch (_currentStubState)
            {
                case StubState.Success:
                {
                    return new OkObjectResult(new ResponseParameterDTO { Iccid = "12345678912345", PorArray = new[] { "Test" }, Status = "Success", TransactionId = "12345", ErrorMessage = "" });
                }
                case StubState.Failure:
                {
                    return new OkObjectResult(new ResponseParameterDTO { Iccid = "12345678912345", PorArray = new[] { "Test" }, Status = "Failed", TransactionId = "12345", ErrorMessage = "Validation failed." });
                }
                case StubState.InternalServerError:
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                default:
                {
                    return new OkObjectResult(new ResponseParameterDTO { Iccid = "12345678912345", PorArray = new[] { "Test" }, Status = "Success", TransactionId = "12345", ErrorMessage = "" });
                }
            }
        }

        [FunctionName("OTACampaignDeleteImsiStubSetReturnSuccessTrigger")]
        public static IActionResult OTACampaignDeleteImsiStubSetReturnSuccessTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "simotacampaigndeleteimsistub/setreturnsuccess")]HttpRequest req, 
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage($"Stub will returning {StubState.Success.ToString()}");

            _currentStubState = StubState.Success;

            return new OkResult();
        }

        [FunctionName("OTACampaignDeleteImsiStubSetReturnFailureTrigger")]
        public static IActionResult OTACampaignDeleteImsiStubSetReturnFailureTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "simotacampaigndeleteimsistub/setreturnfailure")]HttpRequest req,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage($"Stub will returning {StubState.Failure.ToString()}");

            _currentStubState = StubState.Failure;

            return new OkResult();
        }

        [FunctionName("OTACampaignDeleteImsiStubSetReturnInternalServerErrorTrigger")]
        public static IActionResult OTACampaignDeleteImsiStubSetReturnInternalServerErrorTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "simotacampaigndeleteimsistub/setreturninternalservererror")]HttpRequest req,
            [Inject]IJsonLogger logger)
        {
            logger.LogMessage($"Stub will returning {StubState.InternalServerError.ToString()}");

            _currentStubState = StubState.InternalServerError;

            return new OkResult();
        }

        private enum StubState
        {
            Success,
            Failure,
            InternalServerError
        }
    }
}