using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersParseDataTests
    {
        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */
        private Campaign _existingCampaign;

        private string _fileContentOTACampaignSubscribers;
        private string _fileNameOTACampaignSubscribers;
        private OasisRequest _dataOasisRequest;
        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;

        private OTACampaignSubscribersParseData _otaCampaignSubscribersParseDataMock;
        private Mock<IProvisioningDbUnitOfWork> _provisioningUnitOfWork;
        private List<UiccidSimProfileId> _simProfileUiccidList;

        [TestInitialize]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
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
            _fileContentOTACampaignSubscribers = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTACampaignSubscribers", "SampleFiles", "OTA_CAMPGN_DTL_11.csv"));
            _fileNameOTACampaignSubscribers = "OTA_CAMPGN_DTL_11.csv";
        }

        private void SetupObjects()
        {
            _dataOasisRequest = new OasisRequest()
            {
                CampaignId = 11,
                Iccid = "893107011805171299",
                Notes = "A comment for subscriber 893107011805171299",
                TargetSimProfileId = 101,
                Status = string.Empty
            };

            _existingCampaign = new Campaign()
            {
                Description = "description",
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                IccidCount = 100,
                Id = 1,
                TargetSimProfile = 101,
                Type = "OASIS"
            };
            _simProfileUiccidList = new List<UiccidSimProfileId>
            {
                new UiccidSimProfileId()
                    {SimProfileId = _existingCampaign.TargetSimProfile, Uiccid = _dataOasisRequest.Iccid}
            };
        }

        private void SetupStubs()
        {
            _otaDbUnitOfWork = new Mock<IOtaDbUnitOfWork>();
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.GetCampaign(It.IsAny<int>())).Returns(_existingCampaign);

            _provisioningUnitOfWork = new Mock<IProvisioningDbUnitOfWork>();
            _provisioningUnitOfWork.Setup(x =>
                x.SimOrderLineRepository.GetSimProfileByUiccidBatch(It.IsAny<List<string>>())).Returns(_simProfileUiccidList);

        }

        private void SetupMock()
        {
            _otaCampaignSubscribersParseDataMock = new OTACampaignSubscribersParseData(_otaDbUnitOfWork.Object, _provisioningUnitOfWork.Object, new AzureFunctionJsonLogger());
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        public void Parse_SampleOTA_CAMPGN_11_ValidateCountOfDtosReturnedAndValuesOnFirstDto()
        {
            _provisioningUnitOfWork.Setup(x =>
                x.SimOrderLineRepository.GetSimProfileByUiccidBatch(It.IsAny<List<string>>())).Returns(new List<UiccidSimProfileId>() {
                   new UiccidSimProfileId() { Uiccid = "893107011805171299", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171308", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171423", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171486", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171521", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171547", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171550", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171554", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171562", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171564", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171599", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171611", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171647", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171761", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169228", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169230", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169313", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169330", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169406", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169427", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169443", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169526", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169530", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169545", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169552", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169677", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704909855", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704909984", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910019", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910176", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910257", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910258", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910306", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910314", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910317", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910343", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910344", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910394", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910404", SimProfileId = 100 }
            });

            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            var res = _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);

            Assert.IsTrue(res.ParsedOasisRequests.Count == 39);

            Assert.IsTrue(res.ParsedOasisRequests[0].CampaignId == _dataOasisRequest.CampaignId);
            Assert.IsTrue(res.ParsedOasisRequests[0].Iccid == _dataOasisRequest.Iccid);
            Assert.IsTrue(res.ParsedOasisRequests[0].Notes == _dataOasisRequest.Notes);
            Assert.IsTrue(res.ParsedOasisRequests[0].TargetSimProfileId == _dataOasisRequest.TargetSimProfileId);
            Assert.IsTrue(res.ParsedOasisRequests[0].Status == _dataOasisRequest.Status);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV does not have header.")]
        public void Parse_NoFileContent_ThrowInvalidOperationException()
        {
            _fileContentOTACampaignSubscribers = string.Empty;
            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV does not have proper headers.")]
        public void Parse_OTA_CAMPGN_DTL_11WithWrongHeaders_ThrowInvalidOperationException()
        {
            _fileContentOTACampaignSubscribers = "SomeWrongHeader,SomeOtherWrongHeader";
            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV does not have proper headers.")]
        public void Parse_OTA_CAMPGN_DTL_11WithProperHeadersNoContent_ThrowInvalidOperationException()
        {
            _fileContentOTACampaignSubscribers = "OTA_CAMPGN_ID,REL_NO,ICCID,TARGT_SIM_PRFIL_ID,COMMT";
            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        [ExpectedException(typeof(FormatException))]
        public void Parse_WrongFileContentOTA_CAMPGN_ID_NotInt_ThrowFormatException()
        {
            _fileContentOTACampaignSubscribers = _fileContentOTACampaignSubscribers.Replace("11", "asdf");
            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignSubscribers\SampleFiles\")]
        [ExpectedException(typeof(FormatException))]
        public void Parse_WrongContentTARGT_SIM_PRFIL_ID_NotInt_ThrowFormatException()
        {
            _fileContentOTACampaignSubscribers = _fileContentOTACampaignSubscribers.Replace("101", "asdf");
            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);
        }

        [TestMethod]
        public void Parse_FileWithEmptySIMProfiles_ParseSuccess()
        {
            _provisioningUnitOfWork.Setup(x =>
               x.SimOrderLineRepository.GetSimProfileByUiccidBatch(It.IsAny<List<string>>())).Returns(new List<UiccidSimProfileId>() {
                   new UiccidSimProfileId() { Uiccid = "893107011805171299", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171308", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171423", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171486", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171521", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171547", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171550", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171554", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171562", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171564", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171599", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171611", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171647", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805171761", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169228", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169230", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169313", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169330", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169406", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169427", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169443", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169526", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169530", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169545", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169552", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011805169677", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704909855", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704909984", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910019", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910176", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910257", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910258", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910306", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910314", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910317", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910343", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910344", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910394", SimProfileId = 100 },
                   new UiccidSimProfileId() { Uiccid = "893107011704910404", SimProfileId = 100 }
               });

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTACampaignSubscribers", "SampleFiles", "OTA_CAMPAGN_DTL_WITH_EMPTY_SIMPRFLID.csv");
            _fileContentOTACampaignSubscribers = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTACampaignSubscribers", "SampleFiles", "OTA_CAMPAGN_DTL_WITH_EMPTY_SIMPRFLID.csv"));
            _fileNameOTACampaignSubscribers = "OTA_CAMPAGN_DTL_WITH_EMPTY_SIMPRFLID.csv";

            var otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContentOTACampaignSubscribers, _fileNameOTACampaignSubscribers);

            var res = _otaCampaignSubscribersParseDataMock.Parse(otaCampaignSubscribersStarter);

            Assert.IsTrue(res.ParsedOasisRequests.Count == 39);
        }

    }
}
