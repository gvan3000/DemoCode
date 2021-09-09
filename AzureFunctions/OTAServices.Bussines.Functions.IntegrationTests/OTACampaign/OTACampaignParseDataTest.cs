using Microsoft.VisualStudio.TestTools.UnitTesting;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using System.IO;
using OTAServices.Business.Functions.Implementations.OTACampaign;
using Microsoft.Extensions.Logging;
using Moq;
using OTAServices.Business.Entities.OTACampaign;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaign
{
    [TestClass]
    public class OTACampaignParseDataTest
    {
        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */

        /* TODO - refactor these tests */

        private string _fileContentOTACampaig;
        private string _fileNameOTACampaign;
        private Campaign _dataOtaCampaign;

        private OTACampaignParseData _otaCampaignParseDataMock;

        [TestInitialize]
        [DeploymentItem(@"OTACampaign\SampleFiles\")]
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
            _fileContentOTACampaig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTACampaign", "SampleFiles", "OTA_CAMPGN_1.csv"));
            _fileNameOTACampaign = "OTA_CAMPGN_1.csv";
        }

        // NOTE: Parsing not culture independent 
        private void SetupObjects()
        {
            _dataOtaCampaign = new Campaign
            {
                Id = 1,
                Description = "My first OTA Campaign!",
                Type = "OASIS_CAMPAIGN_MNGR",
                IccidCount = 39,
                TargetSimProfile = 101,
                EndDate = DateTime.Parse("8/8/2118")
            };
        }

        private void SetupStubs()
        {
        }

        private void SetupMock()
        {
            _otaCampaignParseDataMock = new OTACampaignParseData(new AzureFunctionJsonLogger());
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaign\SampleFiles\")]
        public void Parse_SampleOTA_CAMPGN_1_ValidateCountOfDtosReturnedAndValuesOnFirstDto()
        {
            var OTACampaignStarter = new OTACampaignStarter(_fileContentOTACampaig, _fileNameOTACampaign);
            
            var res = _otaCampaignParseDataMock.Parse(OTACampaignStarter);

            Assert.IsTrue(res.Campaigns.Count == 1);

            Assert.IsTrue(res.Campaigns[0].Id == _dataOtaCampaign.Id);
            Assert.IsTrue(res.Campaigns[0].Description == _dataOtaCampaign.Description);
            Assert.IsTrue(res.Campaigns[0].Type == _dataOtaCampaign.Type);
            Assert.IsTrue(res.Campaigns[0].IccidCount == _dataOtaCampaign.IccidCount);
            Assert.IsTrue(res.Campaigns[0].TargetSimProfile == _dataOtaCampaign.TargetSimProfile);
            Assert.IsTrue(res.Campaigns[0].EndDate == _dataOtaCampaign.EndDate);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaign\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV does not have header.")]
        public void Parse_NoContent_ThrowInvalidOperationException()
        {
            string fileContent = string.Empty;
            var OTACampaignStarter = new OTACampaignStarter(fileContent, _fileNameOTACampaign);

            var res = _otaCampaignParseDataMock.Parse(OTACampaignStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaignDataSync\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV invalid header column count.")]
        public void Parse_BadColumnCount_ThrowInvalidOperationException()
        {
            string fileContent = "SomeWrongHeader,SomeOtherWrongHeader";
            var OTACampaignStarter = new OTACampaignStarter(fileContent, _fileNameOTACampaign);

            var res = _otaCampaignParseDataMock.Parse(OTACampaignStarter);
        }

        [TestMethod]
        [DeploymentItem(@"OTACampaign\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV invalid header column count.")]
        public void Parse_NoContentExceptHeader_ThrowInvalidOperationException()
        {
            string fileContent = "ID,OTA_CAMPGN_DESCR,OTA_CAMPGN_TYPE,ICCID_CNT,TARGT_SIM_PRFIL_ID,TARGT_DT";
            var OTACampaignStarter = new OTACampaignStarter(fileContent, _fileNameOTACampaign);

            var res = _otaCampaignParseDataMock.Parse(OTACampaignStarter);
        }


        [TestMethod]
        [DeploymentItem(@"OTACampaign\SampleFiles\")]
        [ExpectedException(typeof(InvalidOperationException), "Passed CSV invalid column format.")]
        public void Parse_BadColumnFormat_ThrowInvalidOperationException()
        {
            string fileContent = "SomeWrongHeader,SomeOtherWrongHeader,SomeWrongHeader,SomeOtherWrongHeader,SomeWrongHeader,SomeOtherWrongHeader"; ;
            var OTACampaignStarter = new OTACampaignStarter(fileContent, "OTA_CAMPGN_BAD_COLUMN_FORMAT.csv");

            var res = _otaCampaignParseDataMock.Parse(OTACampaignStarter);
        }
    }
}
