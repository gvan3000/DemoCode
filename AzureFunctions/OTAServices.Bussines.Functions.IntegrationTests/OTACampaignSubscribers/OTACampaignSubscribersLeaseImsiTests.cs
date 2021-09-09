using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.Common.SubscriberListLeaseRequestEnrichment;
using OTAServices.Business.Entities.LeaseRequest;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersLeaseImsiTests
    {

        private string _fileName;
        private OasisRequest _dataOasisRequest;
        private List<OasisRequest> _dataOasisRequests;
        private LeaseRequest _leaseRequest;
        private SubscriberListLeaseRequest _leaseRequestRepository;
        private List<SimProfileSponsor> _simProfileSponsorList;

        private SimContent _simContent;
        private List<SimContent> _simContentValidationResult;

        private Mock<IProvisioningDbUnitOfWork> _provisioningDbUnitOfWorkMock;
        private Mock<IProvisioningServicesBusQueueClient> _queueClientMock;
        private OTACampaignSubscribersLeaseImsi _OTACampaignSubscribersLeaseImsi;

        private OTACampaignSubscribersValidateResult _OTACampaignSubscribersValidateResult;

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
            _OTACampaignSubscribersLeaseImsi = new OTACampaignSubscribersLeaseImsi(_provisioningDbUnitOfWorkMock.Object, _queueClientMock.Object, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
           
            _provisioningDbUnitOfWorkMock = new Mock<IProvisioningDbUnitOfWork>();
            _provisioningDbUnitOfWorkMock.Setup(x => x.LeaseRequestRepository.AddSubscriberListLeaseRequest(It.IsAny<OTAServices.Business.Entities.LeaseRequest.SubscriberListLeaseRequest>()));
            _provisioningDbUnitOfWorkMock.Setup(x => x.SimProfileSponsorRepository.GetSimProfileSponsorList(It.IsAny<List<int>>()))
               .Returns(_simProfileSponsorList);

            _provisioningDbUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(_simContentValidationResult);
            
            _queueClientMock = new Mock<IProvisioningServicesBusQueueClient>();
            _queueClientMock.Setup(x => x.SendToNserviceBus(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        private void SetupObjects()
        {
            _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;

            _dataOasisRequest = new OasisRequest()
            {
                Id = 101,
                CampaignId = 1,
                Notes = "some note",
                Iccid = "123",
                SubscriberListId = Guid.Empty,
                TargetSimProfileId = 101
            };

            _dataOasisRequests = new List<OasisRequest>();

            _dataOasisRequests.Add(_dataOasisRequest);

            _leaseRequest = new LeaseRequest()
            {
                SponsorPrefix = "MMM",
                Uiccid = "123"
            };

            _leaseRequestRepository = new SubscriberListLeaseRequest()
            {
                CampaignId = 1,
                LeaseRequests = "[{\"Uiccid\":\"123\",\"SponsorPrefix\":\"MMM\"}]",
                SubscriberListId = Guid.NewGuid()
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

            _simContentValidationResult = new List<SimContent>();

            _simContent = new SimContent()
            {
                Uiccid = "123",
                ImsiSponsorPrefix = "12345"
            };

            _simContentValidationResult.Add(_simContent);

            _OTACampaignSubscribersValidateResult = new OTACampaignSubscribersValidateResult(_fileName, _dataOasisRequests, _otaSubscribersListProcessingOperationType);
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_DTL_11.csv";
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task LeaseImsi_ValidatedOasisRequestsIsEmpty_ThrowExceptionToCallerAsync()
        {
            _OTACampaignSubscribersValidateResult.ValidatedOasisRequests = null;
            await _OTACampaignSubscribersLeaseImsi.LeaseImsisAsync(_OTACampaignSubscribersValidateResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task LeaseImsi_SendMessageThrowsException_ThrowExceptionToCallerAsync()
        {
            _queueClientMock.Setup(x => x.SendToNserviceBus(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            await _OTACampaignSubscribersLeaseImsi.LeaseImsisAsync(_OTACampaignSubscribersValidateResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task LeaseImsi_AddLeaseRequestThrowsException_ThrowExceptionToCallerAsync()
        {
            _provisioningDbUnitOfWorkMock.Setup(x => x.LeaseRequestRepository.AddSubscriberListLeaseRequest(It.IsAny<SubscriberListLeaseRequest>())).Throws(new Exception());
            await _OTACampaignSubscribersLeaseImsi.LeaseImsisAsync(_OTACampaignSubscribersValidateResult);
        }

        [TestMethod]
        public async Task LeaseImsi_NewValidLeaseRequest_RecordInsertedAndSagaStartedAsync()
        {
            await _OTACampaignSubscribersLeaseImsi.LeaseImsisAsync(_OTACampaignSubscribersValidateResult);

           _provisioningDbUnitOfWorkMock.Verify(x => x.LeaseRequestRepository.AddSubscriberListLeaseRequest(It.IsAny<SubscriberListLeaseRequest>()));
           _provisioningDbUnitOfWorkMock.Verify(x => x.CommitTransactionAsync());

            _queueClientMock.Verify(x => x.SendToNserviceBus(It.IsAny<string>(), It.IsAny<string>()));
        }


    }
}
