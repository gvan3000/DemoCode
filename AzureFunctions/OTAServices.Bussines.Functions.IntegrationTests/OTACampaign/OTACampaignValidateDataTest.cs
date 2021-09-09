using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Entities.ImsiManagement;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Implementations.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using SimProfileServiceReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaign
{
    [TestClass]
    public class OTACampaignValidateDataTest
    {
        /*
          Recommended unit test method naming:

          methodUnderTest_scenarioUnderTest_expectedResult
        */


        private OTACampaignParseDataResult _OTACampaignParseDataRes;

        private OTACampaignValidateData _OTACampaignValidateData;

        private Mock<SimProfileService> _simProfileService;
        private Mock<IProvisioningDbUnitOfWork> _provisioningDbUnitOfWork;

        private SimProfileContract _simProfileContract;

        private string _fileName;
        private Campaign _campaign;
        private ImsiSponsorsStatus _imsiSponsorsStatus;
        private List<ImsiSponsorsStatus> _imsiSponsorsStatuses;

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
            _fileName = "test.csv";
        }

        private void SetupObjects()
        {
            _simProfileContract = new SimProfileContract
            {
                Id = 101,
                Name = "some name",
                Notes = "some note",
                Manufacturer = "some manufacturer",
                EffectiveFrom = DateTime.Now
            };

            _campaign = new Campaign
            {
                Id = 1,
                Description = "Description",
                EndDate = new DateTime(2666, 12, 12),
                IccidCount = 3,
                StartDate = DateTime.UtcNow,
                TargetSimProfile = 1,
                Type = "OASIS_CAMPAIGN_MNGR"
            };

            _OTACampaignParseDataRes = new OTACampaignParseDataResult(_fileName);

            _OTACampaignParseDataRes.Campaigns.Add(_campaign);
            _imsiSponsorsStatus = new ImsiSponsorsStatus
            {
                Id = 59,
                ExternalId = "TestSponsor",
                ImsiCount = 60
            };

            _imsiSponsorsStatuses = new List<ImsiSponsorsStatus>
            {
                _imsiSponsorsStatus
            };
        }

        private void SetupStubs()
        {
            _simProfileService = new Mock<SimProfileService>();

            _simProfileService
                .Setup(x => x.GetSimProfileAsync(It.IsAny<GetSimProfileByIdContract>()))
                .Returns(Task.FromResult(_simProfileContract));

            _provisioningDbUnitOfWork = new Mock<IProvisioningDbUnitOfWork>();

            _provisioningDbUnitOfWork
                .Setup(x => x.ImsiSponsorsStatusRepository.GetImsiSponsorsStatusBySimProfileId(It.IsAny<int>()))
                .Returns(_imsiSponsorsStatuses);
        }

        private void SetupMock()
        {
            _OTACampaignValidateData = new OTACampaignValidateData(_simProfileService.Object, _provisioningDbUnitOfWork.Object, new AzureFunctionJsonLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Given date has passed.")]
        public void Validate_TargetDatePassed_ThrowInvalidOperationException()
        {
            _campaign.EndDate = new DateTime(1666, 12, 12);

            var res = _OTACampaignValidateData.ValidateAsync(_OTACampaignParseDataRes).GetAwaiter().GetResult();

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Sim Profile does not exist.")]
        public void Validate_SimProvileIsNull_ThrowInvalidOperationException()
        {
            _simProfileService
               .Setup(x => x.GetSimProfileAsync(It.IsAny<GetSimProfileByIdContract>()))
               .Returns(Task.FromResult<SimProfileContract>(null));

            var res = _OTACampaignValidateData.ValidateAsync(_OTACampaignParseDataRes).GetAwaiter().GetResult();

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Invalid Campaign Type.")]
        public void Validate_BadCampaignDescription_ThrowInvalidOperationException()
        {
            _campaign.Type = string.Empty;

            var res = _OTACampaignValidateData.ValidateAsync(_OTACampaignParseDataRes).GetAwaiter().GetResult();

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Iccid Count not available.")]
        public void Validate_CountNotAvailable_ThrowInvalidOperationException()
        {
            _imsiSponsorsStatus.ImsiCount = 0;

            var res = _OTACampaignValidateData.ValidateAsync(_OTACampaignParseDataRes).GetAwaiter().GetResult();

        }


    }
}
