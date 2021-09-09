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
    public class OTACampaignSubscribersEnrichOasisRequestTests
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
        private ImsiInfo _imsiInfo;
        private List<ImsiInfo> _imsiInfoList;
        private List<ProvisioningDataInfo> _provisioningDataList;
        private List<SimProfileSponsor> _simProfileSponsorList;
        private OTACampaignSubscribersLeaseImsiResult _otaCampaignSubscribersLeaseImsiResult;
        private OTACampaignSubscribersSaveOasisRequestResult _otaCampaignSubscribersImportDataResult;
        private OTACampaignSubscribersSaveOasisRequestsRollback _otaCampaignSubscribersImportDataRollback;

        private Mock<IProvisioningDbUnitOfWork> _provisioningUnitOfWork;
        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;
        private Mock<IMaximityDbUnitOfWork> _maximityDbUnitOfWork;

        private OTACampaignSubscribersEnrichOasisRequest _otaCampaignSubscribersEnrichOasisRequest;
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
            _otaCampaignSubscribersEnrichOasisRequest = new OTACampaignSubscribersEnrichOasisRequest(_maximityDbUnitOfWork.Object, _provisioningUnitOfWork.Object, new AzureFunctionJsonLogger());
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
                TargetSimProfileId = 101,
                OriginalSimProfileId = 102
            };

            _dataOasisRequests = new List<OasisRequest>();

            _dataOasisRequests.Add(_dataOasisRequest);

            _otaCampaignSubscribersLeaseImsiResult = new OTACampaignSubscribersLeaseImsiResult(_fileName, _dataOasisRequests, Guid.NewGuid(), new Dictionary<string, int>(), _otaSubscribersListProcessingOperationType);
            _otaCampaignSubscribersLeaseImsiResult.SimImsiesCount.Add("123456789012", 1);

            _otaCampaignSubscribersImportDataResult = new OTACampaignSubscribersSaveOasisRequestResult(_fileName, Guid.NewGuid(), new List<int>() { _oasisRequestId }, 1, 1, _otaSubscribersListProcessingOperationType);

            _otaCampaignSubscribersImportDataRollback = new OTACampaignSubscribersSaveOasisRequestsRollback(_fileName, new List<int>() { _oasisRequestId });

            _oasisRequestIds = new List<int>();

            _oasisRequestIds.Add(_oasisRequestId);

            _imsiInfo = new ImsiInfo
            {
                Iccid = _dataOasisRequest.Iccid,
                Imsi = 1232321,
                ImsiSponsorExternalId = "Sponsor",
                ImsiPrefix = "Prefix"
            };

            _imsiInfoList = new List<ImsiInfo>
            {
                _imsiInfo
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
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_AndExceptionIsThrownFromProductInfoRepository_ThrowExceptionToCallerAsync()
        {
            _maximityDbUnitOfWork.Setup(x => x.ProductInfoRepository.GetProductInfos(It.IsAny<List<string>>())).Throws(new Exception());

            await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_AndExceptionIsThrownFromImsiInfoRepository_ThrowExceptionToCallerAsync()
        {
            _provisioningUnitOfWork.Setup(x => x.ImsiInfoRepository.GetImsiInfos(It.IsAny<List<string>>(), It.IsAny<int>())).Throws(new Exception());

            await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_AndExceptionIsThrownFromSimProfileSponsorRepository_ThrowExceptionToCallerAsync()
        {
            _provisioningUnitOfWork.Setup(x => x.SimProfileSponsorRepository.GetSimProfileSponsorList(It.IsAny<List<int>>())).Throws(new Exception());

            await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_AndExceptionIsThrownFromProvisioningDataInfoRepository_ThrowExceptionToCallerAsync()
        {
            _provisioningUnitOfWork.Setup(x => x.ProvisioningDataInfoRepository.GetProvisioningDataInfos(It.IsAny<List<string>>())).Throws(new Exception());

            await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_CreateImsiSponsorInfoWithImsiOperationDeleteImsi()
        {
            _otaCampaignSubscribersLeaseImsiResult.OTASubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.DeleteImsies;

            _simProfileSponsorList.Clear();

            var res = await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);

            Assert.IsTrue(res.EnrichedOasisRequests[0].ImsiInfo == "[{\"Iccid\":\"123456789012\",\"Imsi\":\"1232321\",\"SponsorName\":\"Sponsor\",\"ImsiIndex\":null,\"MccList\":null,\"ImsiOperation\":1}]");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_CreateImsiSponsorInfoWithImsiOperationAddImsi()
        {
            _imsiInfo.IsFromOngoingCampaign = true;

            _otaCampaignSubscribersLeaseImsiResult.OTASubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;

            var res = await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);

            Assert.IsTrue(res.EnrichedOasisRequests[0].ImsiInfo == "[{\"Iccid\":\"123456789012\",\"Imsi\":\"1232321\",\"SponsorName\":\"Sponsor\",\"ImsiIndex\":\"2\",\"MccList\":\"DSSSA2\",\"ImsiOperation\":0}]");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task EnrichAndSaveOasisRequest_CreateImsiSponsorInfoWithImsiOperationUpdatePLMN()
        {
            _simProfileSponsorList.Add(
                new SimProfileSponsor
                {
                    MCC = "MSSSA3",
                    SimProfileId = 102,
                    SponsorExternalId = "Sponsor",
                    SponsorPrefix = "Prefix"
                });
            

            _otaCampaignSubscribersLeaseImsiResult.OTASubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.UpdatePlmnLists;

            var res = await _otaCampaignSubscribersEnrichOasisRequest.EnrichOasisRequestAsync(_otaCampaignSubscribersLeaseImsiResult);

            Assert.IsTrue(res.EnrichedOasisRequests[0].ImsiInfo == "[{\"Iccid\":\"123456789012\",\"Imsi\":\"1232321\",\"SponsorName\":\"Sponsor\",\"ImsiIndex\":null,\"MccList\":\"DSSSA2\",\"ImsiOperation\":2}]");
        }
    }
}
