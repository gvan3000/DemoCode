using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.Implementations.OTACampaignDeleteImsi;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignDeleteImsi
{
    [TestClass]
    public class OTACampaignProcessDeleteImsiCallbackTests
    {
        /*
         Recommended unit test method naming:

         methodUnderTest_scenarioUnderTest_expectedResult
        */

        private OTACampaignProcessDeleteImsiCallback _OTACampaignProcessDeleteImsiCallback;
        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;


        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<ILogger<OTACampaignProcessDeleteImsiCallback>> _logger;

        private Guid _subscribersListId;
        private string _imsi;
        private string _iccid;
        private string _status;
        private int _oasisRequestId;

        private DeleteImsiCallbackResult _deleteImsiCallbackResult;
        private OasisRequest _dataOasisRequest;
        private DeleteIMSICallback _deleteImsiCallback;

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
            _OTACampaignProcessDeleteImsiCallback = new OTACampaignProcessDeleteImsiCallback(_otaDbUnitOfWork.Object, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
            _otaDbUnitOfWork = new Mock<IOtaDbUnitOfWork>();

            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.GetByIccidAndSubscriberListId(It.IsAny<string>(), It.IsAny<Guid>())).Returns(_dataOasisRequest);
            _otaDbUnitOfWork.Setup(x => x.DeleteIMSICallbackRepository.GetByImsiAndOasisRequestId(It.IsAny<string>(), It.IsAny<int>())).Returns(_deleteImsiCallback);
            _otaDbUnitOfWork.Setup(x => x.DeleteIMSICallbackRepository.Update(It.IsAny<DeleteIMSICallback>()));

            _logger = new Mock<ILogger<OTACampaignProcessDeleteImsiCallback>>(MockBehavior.Loose);

            _loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Loose);
            _loggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_logger.Object);

        }

        private void SetupObjects()
        {
            _deleteImsiCallback = new DeleteIMSICallback
            {
                Id = 1,
                IMSI = _imsi,
                Status = _status,
                OasisRequestId = _oasisRequestId
            };

            _deleteImsiCallbackResult = new DeleteImsiCallbackResult
            {
                Iccid = _iccid,
                Imsi = _imsi,
                Status = _status,
                SubscriberListId = _subscribersListId.ToString()
            };

            _dataOasisRequest = new OasisRequest()
            {
                Id = _oasisRequestId,
                CampaignId = 1,
                Notes = "some note",
                Iccid = _iccid,
                SubscriberListId = _subscribersListId,
                TargetSimProfileId = 101
            };

            
        }

        private void SetupValues()
        {
            _oasisRequestId = 101;
            _iccid = "123456789012";
            _subscribersListId = new Guid("5ce601c7-2853-436e-a324-87f7f221c374");
            _imsi = "12345566677";
            _status = "Succeeded";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateDeleteImsiCallback_SubscriberListIdInvalid_ThrowException()
        {
            _deleteImsiCallbackResult.SubscriberListId = String.Empty;

            await _OTACampaignProcessDeleteImsiCallback.UpdateDeleteImsiCallback(_deleteImsiCallbackResult);
            _otaDbUnitOfWork.Verify(x => x.DeleteIMSICallbackRepository.Update(It.IsAny<DeleteIMSICallback>()));
            _otaDbUnitOfWork.Verify(x => x.CommitTransactionAsync());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task UpdateDeleteImsiCallback_OasisRequestNotFound_ThrowException()
        {
            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.GetByIccidAndSubscriberListId(It.IsAny<string>(), It.IsAny<Guid>())).Returns<OasisRequest>(null);

            await _OTACampaignProcessDeleteImsiCallback.UpdateDeleteImsiCallback(_deleteImsiCallbackResult);
            _otaDbUnitOfWork.Verify(x => x.DeleteIMSICallbackRepository.Update(It.IsAny<DeleteIMSICallback>()));
            _otaDbUnitOfWork.Verify(x => x.CommitTransactionAsync());
        }

        [TestMethod]
        public async Task UpdateDeleteImsiCallback_AllDataCorrect_SucceesfulyUpdatedDB()
        {
            await _OTACampaignProcessDeleteImsiCallback.UpdateDeleteImsiCallback(_deleteImsiCallbackResult);
            _otaDbUnitOfWork.Verify(x => x.DeleteIMSICallbackRepository.Update(It.IsAny<DeleteIMSICallback>()));
            _otaDbUnitOfWork.Verify(x => x.CommitTransactionAsync());
        }

    }
}
