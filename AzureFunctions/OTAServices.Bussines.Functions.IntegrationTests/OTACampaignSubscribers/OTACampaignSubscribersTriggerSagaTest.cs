using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using System;
using System.Collections.Generic;
using TeleenaFileLogging.AzureFunctions;
using System.Threading.Tasks;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersTriggerSagaTest
    {
        private string _fileName;

        private OTACampaignSubscribersTriggerSaga _OTACampaignSubscribersTriggerSaga;
        private Mock<IOTAServicesBusTopicClient> _topicClientMock;


        private OTACampaignSubscribersSaveOasisRequestResult _OTACampaignSubscribersImportDataResult;
        

        private OTASubscribersListProcessingOperationType _otaSubscribersListProcessingOperationType;


        [TestInitialize]
        public void Setup()
        {
            SetupValues();

            SetupObjects();

            SetupStubs();

            SetupMock();

            TestHelper.InitializeEventFlowRepo();
        }

        private void SetupMock()
        {
            _OTACampaignSubscribersTriggerSaga = new OTACampaignSubscribersTriggerSaga( _topicClientMock.Object, string.Empty, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
           
            _topicClientMock = new Mock<IOTAServicesBusTopicClient>();
            _topicClientMock.Setup(x => x.SendToTopic(It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        private void SetupObjects()
        {
            _OTACampaignSubscribersImportDataResult = new OTACampaignSubscribersSaveOasisRequestResult(_fileName, Guid.NewGuid(), new List<int> { 1, 2, 3 }, 3, 1, _otaSubscribersListProcessingOperationType);
        }

        private void SetupValues()
        {
            _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;

            _fileName = "OTA_CAMPGN_DTL_11.csv";
        }


        [TestMethod]
        public async Task TriggerSaga_AddImsiesOperationType_SagaStarted()
        {
            var res = await _OTACampaignSubscribersTriggerSaga.TriggerSaga(_OTACampaignSubscribersImportDataResult);
            _topicClientMock.Verify(x => x.SendToTopic(It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TriggerSaga_DeleteImsiesOperationType_SagaStarted()
        {
            _OTACampaignSubscribersImportDataResult.OTASubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.DeleteImsies;

            var res = await _OTACampaignSubscribersTriggerSaga.TriggerSaga(_OTACampaignSubscribersImportDataResult);
            _topicClientMock.Verify(x => x.SendToTopic(It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TriggerSaga_UpdatePLMNOperationType_SagaStarted()
        {
            _OTACampaignSubscribersImportDataResult.OTASubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.UpdatePlmnLists;

            bool error = false;

            try
            {
                await _OTACampaignSubscribersTriggerSaga.TriggerSaga(_OTACampaignSubscribersImportDataResult);
            }
            catch (InvalidOperationException ex)
            {
                error = ex.Message.Contains("Update PLMN list use-case detected, implementation for this use case is not finished.");
            }
            Assert.IsTrue(error, "Error for 'Update PLMN list use-case detected, implementation for this use case is not finished.'");  
        }
    }
}
