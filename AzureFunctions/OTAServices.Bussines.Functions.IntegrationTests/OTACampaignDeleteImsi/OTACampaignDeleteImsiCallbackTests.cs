using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaignDeleteImsi;
using System;
using System.Collections.Generic;
using System.Text;
using TeleenaFileLogging.AzureFunctions;
using System.Threading.Tasks;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignDeleteImsi
{
    [TestClass]
    public class OTACampaignDeleteImsiCallbackTests
    {
        /*
         Recommended unit test method naming:

         methodUnderTest_scenarioUnderTest_expectedResult
        */

        private OTACampaignDeleteImsiCallback _OTACampaignDeleteImsiCallback;
        private Mock<IDeleteImsiCallbackResponseQueueClient> _queueClientMock;


        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<ILogger<OTACampaignDeleteImsiCallback>> _logger;

        private string _subscribersListId;
        private string _imsi;

        private OasisCallback _callback;

        [TestInitialize]
        public void Setup()
        {
            SetupValues();

            SetupObjects();

            SetupStubs();

            SetupMock();

            TestHelper.InitializeEventFlowRepo();
        }

        private void SetupValues()
        {
            _subscribersListId = "2345";
            _imsi = "1234567788";
        }

        private void SetupObjects()
        {
            _callback = new OasisCallback
            {
                Action = "action",
                EId = "eid",
                Iccid = "iccid",
                Status = "status",
                TId = 123,
                TimeStart = DateTime.Now,
                TimeEnd = DateTime.Now
            };
        }

        private void SetupStubs()
        {
            _queueClientMock = new Mock<IDeleteImsiCallbackResponseQueueClient>();
            _queueClientMock.Setup(x => x.SendToQueue(It.IsAny<string>())).Returns(Task.CompletedTask);

            _logger = new Mock<ILogger<OTACampaignDeleteImsiCallback>>(MockBehavior.Loose);
            _loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Loose);
            _loggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_logger.Object);
        }

        private void SetupMock()
        {
            _OTACampaignDeleteImsiCallback = new OTACampaignDeleteImsiCallback(_queueClientMock.Object, new AzureFunctionJsonLogger());
        }

        [TestMethod]
        public async Task UpdateDeleteImsiCallback_HttpTriggered_MessageSentToQueue()
        {
            try
            {
                await _OTACampaignDeleteImsiCallback.DeleteImsi(_callback, _subscribersListId, _imsi);
            }
            catch(Exception exc)
            {
                throw;
            }
            _queueClientMock.Verify(x => x.SendToQueue(It.IsAny<string>()));
        }
    }
}
