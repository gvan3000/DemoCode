using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersLockProductsTests
    {

        private string _fileName;
        private OasisRequest _dataOasisRequest;
        private List<OasisRequest> _dataOasisRequests;
        private List<string> _lockedProductsIccids;

        private Mock<IMaximityDbUnitOfWork> _maximityDbUnitOfWorkMock;
        private OTACampaignSubscribersLockProducts _OTACampaignSubscribersLockProducts;

        private OTACampaignSubscribersValidateResult _OTACampaignSubscribersValidationResult;
        private OTACampaignSubscribersLockProductsRollback _OTACampaignSubscribersLockProductsRollback;

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
            _OTACampaignSubscribersLockProducts = new OTACampaignSubscribersLockProducts(_maximityDbUnitOfWorkMock.Object, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
           
            _maximityDbUnitOfWorkMock = new Mock<IMaximityDbUnitOfWork>();
            _maximityDbUnitOfWorkMock.Setup(x => x.ProductProcessLockRepository.AddProductProcessLockBulk(It.IsAny<List<string>>()));
            _maximityDbUnitOfWorkMock.Setup(x => x.ProductProcessLockRepository.DeleteProductProcessLockBulk(It.IsAny<List<string>>()));
        }

        private void SetupObjects()
        {
            _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;

            _dataOasisRequest = new OasisRequest()
            {
                Id = 101,
                CampaignId = 1,
                Notes = "some note",
                Iccid = "89310706160339500783",
                SubscriberListId = Guid.Empty,
                TargetSimProfileId = 101
            };

            _lockedProductsIccids = new List<string>() { "89310706160339500783" };

            _dataOasisRequests = new List<OasisRequest>();

            _dataOasisRequests.Add(_dataOasisRequest);

            _OTACampaignSubscribersValidationResult = new OTACampaignSubscribersValidateResult(_fileName, _dataOasisRequests, _otaSubscribersListProcessingOperationType);

            _OTACampaignSubscribersLockProductsRollback = new OTACampaignSubscribersLockProductsRollback(_fileName, _lockedProductsIccids);
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_DTL_11.csv";
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task LockProducts_ProductProcessLockRepositoryThrowException_ThrowExceptionToCallerAsync()
        {
            _maximityDbUnitOfWorkMock.Setup(x => x.ProductProcessLockRepository.AddProductProcessLockBulk(It.IsAny<List<string>>())).Throws(new Exception());

            await _OTACampaignSubscribersLockProducts.LockProductsAsync(_OTACampaignSubscribersValidationResult);
        }

        [TestMethod]
        public async Task LockProducts_OneValidatedOasisRequest_ProductLocked()
        {
            await _OTACampaignSubscribersLockProducts.LockProductsAsync(_OTACampaignSubscribersValidationResult);

           _maximityDbUnitOfWorkMock.Verify(x => x.ProductProcessLockRepository.AddProductProcessLockBulk(It.IsAny<List<string>>()));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Rollback_ProductProcessLockRepositoryThrowException_ThrowExceptionToCallerAsync()
        {
            _maximityDbUnitOfWorkMock.Setup(x => x.ProductProcessLockRepository.DeleteProductProcessLockBulk(It.IsAny<List<string>>())).Throws(new Exception());

            _OTACampaignSubscribersLockProducts.Rollback(_OTACampaignSubscribersLockProductsRollback);
        }

        [TestMethod]
        public void Rollback_OneValidatedOasisRequest_ProductUnLocked()
        {
            _OTACampaignSubscribersLockProducts.Rollback(_OTACampaignSubscribersLockProductsRollback);

            _maximityDbUnitOfWorkMock.Verify(x => x.ProductProcessLockRepository.DeleteProductProcessLockBulk(It.IsAny<List<string>>()));
        }
    }
}
