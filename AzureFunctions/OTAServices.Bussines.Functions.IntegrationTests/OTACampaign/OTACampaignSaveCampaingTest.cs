using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Implementations.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaign
{
    [TestClass]
    public class OTACampaignSaveCampaingTest
    {
        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */

        private string _fileName;
        private Campaign _dataCampaign;
        private Campaign _existingCampaign;
        private OTACampaignParseDataResult _otaCampaignParseDataResult;

        private Mock<IOtaDbUnitOfWork> _otaDbUnitOfWork;

        private OTACampaignSaveCampaing _otaCampaignImportDataMock;

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
            _otaCampaignImportDataMock = new OTACampaignSaveCampaing(_otaDbUnitOfWork.Object, new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
            _otaDbUnitOfWork = new Mock<IOtaDbUnitOfWork>();

            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.AddCampaign(It.IsAny<Campaign>()));
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.UpdateCampaign(It.IsAny<Campaign>()));
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.GetCampaign(It.IsAny<int>())).Returns(default(Campaign));
        }

        private void SetupObjects()
        {
            _otaCampaignParseDataResult = new OTACampaignParseDataResult(_fileName);

            _dataCampaign = new Campaign
            {
                Id = 1,
                Description = "Description",
                EndDate = DateTime.UtcNow,
                IccidCount = 3,
                StartDate = DateTime.UtcNow,
                TargetSimProfile = 1,
                Type = "Type"
            };

            _existingCampaign = new Campaign
            {
                Id = 1,
                Description = "Description",
                EndDate = DateTime.UtcNow,
                IccidCount = 5,
                StartDate = DateTime.UtcNow,
                TargetSimProfile = 1,
                Type = "Type"
            };
            

            _otaCampaignParseDataResult.Campaigns.Add(_dataCampaign);
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_1.csv";
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async System.Threading.Tasks.Task SaveCampaing_ExceptionIsThrownFromOTACampaignRepository_ThrowExceptionToCallerAsync()
        {
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.AddCampaign(It.IsAny<Campaign>())).Throws(new Exception());

            await _otaCampaignImportDataMock.SaveCampaingAsync(_otaCampaignParseDataResult);  
        }

        [TestMethod]
        public async System.Threading.Tasks.Task SaveCampaing_ProcessParsedCampaign_FileNameWrittenInResultAndAddCampaignViaRepositoryAsync()
        {
            var result = await _otaCampaignImportDataMock.SaveCampaingAsync(_otaCampaignParseDataResult);

            Assert.IsTrue(result.FileName == _otaCampaignParseDataResult.FileName);

            _otaDbUnitOfWork.Verify(x => x.OTACampaignRepository.AddCampaign(_dataCampaign));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task SaveCampaing_ProcessParsedCampaign_UpdateCampaignViaRepositoryAsyncAndIccCountIsUpdated()
        {
            _otaDbUnitOfWork.Setup(x => x.OTACampaignRepository.GetCampaign(It.IsAny<int>())).Returns(_existingCampaign);

            var result = await _otaCampaignImportDataMock.SaveCampaingAsync(_otaCampaignParseDataResult);

            _otaDbUnitOfWork.Verify(x => x.OTACampaignRepository.UpdateCampaign(_existingCampaign));

            Assert.IsTrue(_existingCampaign.IccidCount == _dataCampaign.IccidCount);
        }
    }
}
