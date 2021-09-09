using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using SimProfileServiceReference;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersValidateTests
    {
        private static Guid SimStatusActive = new Guid("48489470-3FEE-452D-A919-20EF6DE1A261");
        private static Guid ProductStatusActive = new Guid("F8679E2B-2D23-4D5A-BEFD-6385E4CB4014");

        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */

        private string _fileName;
        private OTACampaignSubscribersParseDataResult _parsedDataInput;
        private OasisRequest _parsedOasisRequest;
        private Campaign _existingCampaign;
        private SimProfilesValidationContract _simProfilesValidationContract;
        private SimInfo _simInfo;
        private List<SimInfo> _simInfoValidationContent;

        private SimContent _simContent;
        private List<SimContent> _simContentValidationResult;

        private SimProfileSponsor _simProfileSponsor1;
        private SimProfileSponsor _simProfileSponsor2;
        private List<SimProfileSponsor> _simProfileSponsorList;

        private Mock<SimProfileService> _simProfileServiceMock;
        private Mock<IMaximityDbUnitOfWork> _simInfoUnitOfWorkMock;
        private Mock<IProvisioningDbUnitOfWork> _simContentUnitOfWorkMock;
        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;

        private OTACampaignSubscribersValidate _otaCampaignSubscribersValidateMock;

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
            _otaCampaignSubscribersValidateMock = new OTACampaignSubscribersValidate(
                _simProfileServiceMock.Object,
                _simContentUnitOfWorkMock.Object,
                _simInfoUnitOfWorkMock.Object,
                _otaDbUnitOfWork.Object,
                new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
            _simProfileServiceMock = new Mock<SimProfileService>();
            _simProfileServiceMock.Setup(x => x.ValidateSimProfilesAsync(It.IsAny<ValidateSimProfilesContract>())).Returns(Task.FromResult(_simProfilesValidationContract));

            _simInfoUnitOfWorkMock = new Mock<IMaximityDbUnitOfWork>();
            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(_simInfoValidationContent);

            _simContentUnitOfWorkMock = new Mock<IProvisioningDbUnitOfWork>();
            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(_simContentValidationResult);
            _simContentUnitOfWorkMock.Setup(x => x.SimProfileSponsorRepository.GetSimProfileSponsorList(It.IsAny<List<int>>())).Returns(_simProfileSponsorList);

            _otaDbUnitOfWork = new Mock<IOtaDbUnitOfWork>();
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.GetCampaign(It.IsAny<int>())).Returns(_existingCampaign);
        }

        private void SetupObjects()
        {
            _simProfileSponsor1 = new SimProfileSponsor()
            {
                SimProfileId = 101,
                SponsorExternalId = "Teleena",
                MCC = "123456",
                SponsorPrefix = "12345"
            };

            _simProfileSponsor2 = new SimProfileSponsor()
            {
                SimProfileId = 101,
                SponsorExternalId = "TATA",
                MCC = "123456",
                SponsorPrefix = "55555"
            };

            _simProfileSponsorList = new List<SimProfileSponsor>();

            _simProfileSponsorList.Add(_simProfileSponsor1);
            _simProfileSponsorList.Add(_simProfileSponsor2);

            _simContentValidationResult = new List<SimContent>();

            _simContent = new SimContent()
            {
                Uiccid = "1234567891234",
                ImsiSponsorPrefix = "12345"
            };

            _simContentValidationResult.Add(_simContent);

            _simInfoValidationContent = new List<SimInfo>();

            _simInfo = new SimInfo()
            {
                Uiccid = "1234567891234",
                ProductStatus = ProductStatusActive, 
                SimStatus = SimStatusActive, 
                SimType = "OASIS_TATA"
            };

            _simInfoValidationContent.Add(_simInfo);

            _simProfilesValidationContract = new SimProfilesValidationContract()
            {
                AreSimProfilesValid = true
            };

            _existingCampaign = new Campaign()
            {
                Description = "description",
                EndDate = DateTime.Now.AddDays(10),
                StartDate = DateTime.Now.AddDays(50),
                IccidCount = 100,
                Id = 1,
                TargetSimProfile = 101,
                Type = "OASIS",
                OriginalSimProfile = 100
            };

            _parsedOasisRequest = new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "1234567891234",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101,
                OriginalSimProfileId = 100
            };

            _parsedDataInput = new OTACampaignSubscribersParseDataResult(_fileName, new List<OasisRequest>() { _parsedOasisRequest });
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_DTL_11.csv";
        }

        [TestMethod]
        public async Task Validate_InputHaveOneParsedOasisRequest_ResultHaveOneValidOasisRequestsAndOneLeaseRequest()
        {
            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsTrue(validationResults.ValidatedOasisRequests.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveOneParsedOasisRequestWithWrongCampaignId_ThrowInvalidOperationException()
        {
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.GetCampaign(It.IsAny<int>())).Returns(default(Campaign));

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveOneParsedOasisRequestWithWrongOriginalProfile_ThrowInvalidOperationException()
        {
            _parsedOasisRequest.OriginalSimProfileId = 777;
            _existingCampaign.OriginalSimProfile = 888;

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveOneParsedOasisRequestWithNoOriginalProfile_ThrowInvalidOperationException()
        {
            _parsedOasisRequest.OriginalSimProfileId = null;
            _existingCampaign.OriginalSimProfile = 888;

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveOneParsedOasisRequestWithWrongTargetProfile_ThrowInvalidOperationException()
        {
            _parsedOasisRequest.OriginalSimProfileId = 888;
            _existingCampaign.OriginalSimProfile = 888;
            _parsedOasisRequest.TargetSimProfileId = 777;
            _existingCampaign.TargetSimProfile = 888;

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_IfAllIccidsAreNotValid_ThrowInvalidOperationException()
        {
            _simInfoValidationContent[0].SimType = "Not OASIS_TATA";

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveOneParsedOasisRequestWithWrongSimProfile_ThrowInvalidOperationException()
        {
            _simProfileServiceMock.Setup(x => x.ValidateSimProfilesAsync(It.IsAny<ValidateSimProfilesContract>())).Returns(Task.FromResult(new SimProfilesValidationContract() { AreSimProfilesValid = false, InvalidSimProfileIds = new[] { 1 }, ValidationErrorMessages = new[] { "Invalid sim profile" } }));

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        public async Task Validate_InputHaveTwoParsedOasisRequestWithOneInvalidSimType_ResultHaveOneValidOasisRequests()
        {
            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101,
                OriginalSimProfileId = 100
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "Invalid Sim",  ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" },
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive,SimStatus = SimStatusActive, Uiccid = "12345678912347" }
            });

            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101,
                OriginalSimProfileId = 100
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "12345", Uiccid = "12345678912347" }
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsTrue(validationResults.ValidatedOasisRequests.Where(x=>String.IsNullOrEmpty(x.ErrorMessage)).Count() == 1);
        }

        [TestMethod]
        public async Task Validate_InputHaveTwoParsedOasisRequest_AddImsiesDetected()
        {
            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" } //TODO
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" }
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsTrue(validationResults.OTASubscribersListProcessingOperationType == Business.Functions.Common.OTASubscribersListProcessingOperationType.AddImsies);
        }

        [TestMethod]
        public async Task Validate_InputHaveTwoParsedOasis_DeleteImsiesDetected()
        {
            _parsedOasisRequest.OriginalSimProfileId = 102;
            _existingCampaign.OriginalSimProfile = 102;

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" } //TODO
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                 new SimContent() { ImsiSponsorPrefix = "55555",Uiccid = "1234567891234" },
                  new SimContent() { ImsiSponsorPrefix = "77",Uiccid = "1234567891234" }
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsTrue(validationResults.OTASubscribersListProcessingOperationType == Business.Functions.Common.OTASubscribersListProcessingOperationType.DeleteImsies);
        }

        [TestMethod]
        public async Task Validate_InputHaveTwoParsedOasisRequest_UpdateImsiesDetected()
        {
            _parsedOasisRequest.OriginalSimProfileId = 102;
            _existingCampaign.OriginalSimProfile = 102;

            _simProfileSponsorList.Add(
                new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "12345" }
            );
            _simProfileSponsorList.Add(
                new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "55555" }
            );
            

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" } //TODO
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                 new SimContent() { ImsiSponsorPrefix = "55555",Uiccid = "1234567891234" }
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsTrue(validationResults.OTASubscribersListProcessingOperationType == Business.Functions.Common.OTASubscribersListProcessingOperationType.UpdatePlmnLists);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveTwoParsedOasisRequest_AddImsiesDetected_OneHaveSimWithImsieSponsorsNotDefinedBySimProfile_ThrowInvalidOperationException()
        {
            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" }, //TODO
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive,SimStatus = SimStatusActive, Uiccid = "12345678912347" } //TODO
            });

            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "6789", Uiccid = "12345678912347" } //this ImsiSponsorPrefix is not defined by SimProfile
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveTwoParsedOasis_DeleteImsiesDetected_RequestOneHaveSimWithMissingImsieSponsorsDefinedBySimProfile_ThrowInvalidOperationException()
        {
            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" }, //TODO
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive,SimStatus = SimStatusActive, Uiccid = "12345678912347" } //TODO
            });

            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "55555",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "6789",Uiccid = "1234567891234" }, //Sim does not have Imsies defined by SimProfile
                new SimContent() { ImsiSponsorPrefix = "6789", Uiccid = "12345678912347" } 
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveTwoParsedOasisRequest_UpdateImsiesDetected_OneHaveSimWithImsieSponsorsNotDefinedBySimProfile_ThrowInvalidOperationException()
        {
            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" }, //TODO
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive,SimStatus = SimStatusActive, Uiccid = "12345678912347" } //TODO
            });

            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "55555",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "6789", Uiccid = "12345678912347" } //this ImsiSponsorPrefix is not defined by SimProfile
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Validate_InputHaveTwoParsedOasis_UpdateImsiesDetected_RequestOneHaveSimWithMissingImsieSponsorsDefinedBySimProfile_ThrowInvalidOperationException()
        {
            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(new List<SimInfo> {
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive, SimStatus = SimStatusActive, Uiccid = "1234567891234" }, //TODO
                new SimInfo(){ SimType = "OASIS_TATA", ProductStatus = ProductStatusActive,SimStatus = SimStatusActive, Uiccid = "12345678912347" } //TODO
            });

            _parsedDataInput.ParsedOasisRequests.Add(new OasisRequest()
            {
                Id = 1,
                CampaignId = 1,
                Iccid = "12345678912347",
                Notes = "note",
                Status = string.Empty,
                SubscriberListId = Guid.NewGuid(),
                TargetSimProfileId = 101
            });

            _simContentUnitOfWorkMock.Setup(x => x.SimContentRepository.GetSimContentBatch(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(new List<SimContent> {
                new SimContent() { ImsiSponsorPrefix = "12345",Uiccid = "1234567891234" },
                new SimContent() { ImsiSponsorPrefix = "55555",Uiccid = "1234567891234" }, 
                new SimContent() { ImsiSponsorPrefix = "55555", Uiccid = "12345678912347" }//Sim does not have Imsies defined by SimProfile
            });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
        }

        [TestMethod]
        public async Task Validate_InputHasUnchangedSimProfileWithLeasedForCampaign_ValidationPasses()
        {
            _parsedOasisRequest.OriginalSimProfileId = 102;
            _existingCampaign.OriginalSimProfile = 102;

            _simProfileSponsorList.Add(new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "132" }
            );

            _simContent.IsLeasedForOngoingCampaign = true;
            _simContent.ImsiSponsorPrefix = "132";
            _simProfileSponsor1.SponsorPrefix = "132";
            _simProfileSponsor2.SponsorPrefix = "132";
            
            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(
                new List<SimInfo>
                {
                    new SimInfo
                    {
                        SimType = "OASIS_TATA",
                        ProductStatus = ProductStatusActive,
                        SimStatus = SimStatusActive,
                        Uiccid = "1234567891234"
                    }
                });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsNull(validationResults.ValidatedOasisRequests.Single().ErrorMessage);
        }

        [TestMethod]
        public async Task Validate_InputHasUnchangedSimProfileWithNewImsi_ValidationPasses()
        {
            _simProfileSponsorList.Add(new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "1234" }
            );

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(
                new List<SimInfo>
                {
                    new SimInfo
                    {
                        SimType = "OASIS_TATA",
                        ProductStatus = ProductStatusActive,
                        SimStatus = SimStatusActive,
                        Uiccid = "1234567891234"
                    }
                });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsNull(validationResults.ValidatedOasisRequests.Single().ErrorMessage);
        }
        
        [TestMethod]
        public async Task Validate_InputHasUnchangedSimProfileWithChangedMCCList_ValidationPasses()
        {
            _simProfileSponsorList.Add(new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "1234" }
            );

            _simContent.ImsiSponsorPrefix = "132";
            _simProfileSponsor1.SponsorPrefix = "132";
            _simProfileSponsor1.MCC = "444132";
            _simProfileSponsor2.SponsorPrefix = "132";
            _parsedDataInput.ParsedOasisRequests.First().OriginalSimProfileId = 102;
            _existingCampaign.OriginalSimProfile = 102;

            _simProfileSponsorList.Add(new SimProfileSponsor
            {
                SimProfileId = 102,
                SponsorPrefix = "132",
                MCC = "888999000"
            });
            
            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(
                new List<SimInfo>
                {
                    new SimInfo
                    {
                        SimType = "OASIS_TATA",
                        ProductStatus = ProductStatusActive,
                        SimStatus = SimStatusActive,
                        Uiccid = "1234567891234"
                    }
                });

            var validationResults = await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);

            Assert.IsNull(validationResults.ValidatedOasisRequests.Single().ErrorMessage);
        }


        [TestMethod]
        public async Task Validate_InputHasUnchangedSimProfile_ValidationFails()
        {
            _simProfileSponsorList.Add(new SimProfileSponsor
                { SimProfileId = 102, SponsorExternalId = "First", MCC = "1234567891234", SponsorPrefix = "1234" }
            );

            _simContent.ImsiSponsorPrefix = "132";
            _simProfileSponsor1.SponsorPrefix = "132";
            _simProfileSponsor1.MCC = "888999000";
            _simProfileSponsor2.SponsorPrefix = "132";
            _parsedDataInput.ParsedOasisRequests.First().OriginalSimProfileId = 102;
            _existingCampaign.OriginalSimProfile = 102;

            _simProfileSponsorList.Add(new SimProfileSponsor
            {
                SimProfileId = 102,
                SponsorPrefix = "132",
                MCC = "888999000"
            });

            _simInfoUnitOfWorkMock.Setup(x => x.SimInfoRepository.GetSimInfoBatch(It.IsAny<List<string>>())).Returns(
                new List<SimInfo>
                {
                    new SimInfo
                    {
                        SimType = "OASIS_TATA",
                        ProductStatus = ProductStatusActive,
                        SimStatus = SimStatusActive,
                        Uiccid = "1234567891234"
                    }
                });

            bool error = false;
            try
            {
                await _otaCampaignSubscribersValidateMock.ValidateAsync(_parsedDataInput);
            }
            catch (InvalidOperationException ex)
            {
                error = ex.Message.Contains("already adhere to target Sim Profile");
            }
            Assert.IsTrue(error, "Error for 'already adhere to target Sim Profile'");
        }
    }
}
