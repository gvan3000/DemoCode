using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.Common;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersSaveOasisRequestTests
    {
        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */

        private string _fileName;
        private int _oasisRequestId;
        private List<int> _oasisRequestIds;
        private OasisRequest _dataOasisRequest;
        private List<OasisRequest> _dataOasisRequests;
        private List<ProductInfo> _productInfoList;
        private List<ImsiInfo> _imsiInfoList;
        private List<ProvisioningDataInfo> _provisioningDataList;
        private List<SimProfileSponsor> _simProfileSponsorList;
        private OTACampaignSubscribersEnrichOasisRequestResult _otaCampaignSubscribersEnrichOasisRequestResult;
        private OTACampaignSubscribersSaveOasisRequestResult _otaCampaignSubscribersImportDataResult;
        private OTACampaignSubscribersSaveOasisRequestsRollback _otaCampaignSubscribersImportDataRollback;

        private Mock<IProvisioningDbUnitOfWork> _provisioningUnitOfWork;
        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;
        private Mock<IMaximityDbUnitOfWork> _maximityDbUnitOfWork;

        private OTACampaignSubscribersSaveOasisRequest _otaCampaignSubscribersSaveOasisRequest;
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
            _otaCampaignSubscribersSaveOasisRequest = new OTACampaignSubscribersSaveOasisRequest(_otaDbUnitOfWork.Object, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
            _otaDbUnitOfWork = new Mock<IOtaDbUnitOfWork>();

            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.AddOasisRequests(It.IsAny<List<OasisRequest>>()));

            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.DeleteOasisRequests(It.IsAny<List<int>>()));

            _maximityDbUnitOfWork = new Mock<IMaximityDbUnitOfWork>();

            _maximityDbUnitOfWork.Setup(x => x.ProductInfoRepository.GetProductInfos(It.IsAny<List<string>>()))
                .Returns(_productInfoList);

            _provisioningUnitOfWork = new Mock<IProvisioningDbUnitOfWork>();

            _provisioningUnitOfWork.Setup(x => x.ProvisioningDataInfoRepository.GetProvisioningDataInfos(It.IsAny<List<string>>()))
                .Returns(_provisioningDataList);
            _provisioningUnitOfWork.Setup(x => x.ImsiInfoRepository.GetImsiInfos(It.IsAny<List<string>>(), It.IsAny<int>()))
                .Returns(_imsiInfoList);
            _provisioningUnitOfWork.Setup(x => x.SimProfileSponsorRepository.GetSimProfileSponsorList(It.IsAny<List<int>>()))
                .Returns(_simProfileSponsorList);

        }

        private void SetupObjects()
        {
            _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;

            _dataOasisRequest = new OasisRequest()
            {
                Id = 101,
                CampaignId = 1,
                Notes = "some note",
                Iccid = "123456789012",
                SubscriberListId = Guid.Empty,
                TargetSimProfileId = 101
            };

            _dataOasisRequests = new List<OasisRequest>();

            _dataOasisRequests.Add(_dataOasisRequest);

            _otaCampaignSubscribersEnrichOasisRequestResult = new OTACampaignSubscribersEnrichOasisRequestResult(_fileName, _dataOasisRequests, Guid.NewGuid(), _otaSubscribersListProcessingOperationType);

            _otaCampaignSubscribersImportDataResult = new OTACampaignSubscribersSaveOasisRequestResult(_fileName, Guid.NewGuid(), new List<int>() { _oasisRequestId }, 1, 1, _otaSubscribersListProcessingOperationType);

            _otaCampaignSubscribersImportDataRollback = new OTACampaignSubscribersSaveOasisRequestsRollback(_fileName, new List<int>() { _oasisRequestId });

            _oasisRequestIds = new List<int>();

            _oasisRequestIds.Add(_oasisRequestId);

            _imsiInfoList = new List<ImsiInfo>
            {
                new ImsiInfo
                {
                    Iccid = _dataOasisRequest.Iccid,
                    Imsi  = 1232321,
                    ImsiSponsorExternalId = "Sponsor"
                }

            };
            _productInfoList = new List<ProductInfo>
            {
                new ProductInfo {
                    Msisdn = "06333232",
                    PrimaryImsi = "1232321",
                    Uiccid = _dataOasisRequest.Iccid
                }
            };
            _simProfileSponsorList = new List<SimProfileSponsor>
            {
                new SimProfileSponsor
                {
                    MCC = "DSSSA2",
                    SimProfileId = 101,
                    SponsorExternalId = "Sponsor",
                    SponsorPrefix = "Prefix"
                }
            };

            _provisioningDataList = new List<ProvisioningDataInfo>
            {
                new ProvisioningDataInfo
                {
                    Uiccid = _dataOasisRequest.Iccid,
                    KIC = "123",
                    KID = "123",
                    KIK = "123"
                }
            };
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_DTL_11.csv";
            _oasisRequestId = 101;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task SaveOasisRequest_AndExceptionIsThrownFromOasisRequestRepository_ThrowExceptionToCallerAsync()
        {
            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.AddOasisRequests(It.IsAny<List<OasisRequest>>())).Throws(new Exception());

            await _otaCampaignSubscribersSaveOasisRequest.SaveOasisRequestAsync(_otaCampaignSubscribersEnrichOasisRequestResult);
        }
        
        [TestMethod]
        public async System.Threading.Tasks.Task SaveOasisRequest_NewOasisRequests_AddedViaAddOasisRequestAndAddedIdInResultAsync()
        {
            var res = await _otaCampaignSubscribersSaveOasisRequest.SaveOasisRequestAsync(_otaCampaignSubscribersEnrichOasisRequestResult);

            _otaDbUnitOfWork.Verify(x => x.OasisRequestRepository.AddOasisRequests(_dataOasisRequests));
            _otaDbUnitOfWork.Verify(x => x.CommitTransactionAsync());

            Assert.IsTrue(res.SavedOasisRequestsIds.Count == 1);
            Assert.IsTrue(res.SavedOasisRequestsIds[0] == _dataOasisRequest.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task Rollback_ExceptionIsThrownFromRepository_ThrowExceptionToCallerAsync()
        {
            _otaDbUnitOfWork.Setup(x => x.OasisRequestRepository.DeleteOasisRequests(It.IsAny<List<int>>())).Throws(new Exception());

            await _otaCampaignSubscribersSaveOasisRequest.RollbackAsync(_otaCampaignSubscribersImportDataRollback);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Rollback_PassedAddedOasisRequestsFromPreviousStep_DeletedViaDeleteOasisRequestAsync()
        {
            await _otaCampaignSubscribersSaveOasisRequest.RollbackAsync(_otaCampaignSubscribersImportDataRollback);

            _otaDbUnitOfWork.Verify(x => x.OasisRequestRepository.DeleteOasisRequests(_oasisRequestIds));
            _otaDbUnitOfWork.Verify(x => x.CommitTransactionAsync());
        }
    }
}
