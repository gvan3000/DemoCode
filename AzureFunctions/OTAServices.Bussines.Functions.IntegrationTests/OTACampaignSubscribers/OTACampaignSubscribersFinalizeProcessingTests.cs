using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using Teleena.AzureStorage.Client;
using Teleena.Sftp.Client;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaignSubscribers
{
    [TestClass]
    public class OTACampaignSubscribersFinalizeProcessingTests
    {
        /*
           Recommended unit test method naming:

           methodUnderTest_scenarioUnderTest_expectedResult
        */

        private readonly string _otaCampaignSubscribersInbountPath = "OTACampaign\\ToTeleena\\";
        private readonly string _otaCampaignSubscribersProcessedPath = "OTACampaign\\Processed\\";
        private readonly string _otaCampaignSubscribersErrorPath = "OTACampaign\\Error\\";
        private string _fileName;
        private string _fileContent;
        private DateTime _currentDataTime;

        private OTACampaignSubscribersTriggerSagaResult _otaCampaignSubscribersTriggerSagaDataRes;
        private OTACampaignSubscribersStarter _otaCampaignSubscribersStarter;

        private Mock<ITeleenaSftpClient> _sftp;
        private Mock<IAzureStorageClient> _azureStorage;
        private Mock<IDateTimeProvider> _dateTimeProvider;

        private OTACampaignSubscribersFinalizeProcessing _otaCampaignSubscribersFinalizeImportMock;

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
            _otaCampaignSubscribersFinalizeImportMock = new OTACampaignSubscribersFinalizeProcessing(
                _otaCampaignSubscribersInbountPath,
                _otaCampaignSubscribersProcessedPath,
                _otaCampaignSubscribersErrorPath,
                _sftp.Object,
                _dateTimeProvider.Object,
                _azureStorage.Object,
                new AzureFunctionJsonLogger());
        }

        private void SetupStubs()
        {
            _sftp = new Mock<ITeleenaSftpClient>();
            _sftp.Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>()));

            _azureStorage = new Mock<IAzureStorageClient>();
            _azureStorage.Setup(x => x.DeleteBlobIfExistsAsync(It.IsAny<string>(), It.IsAny<string>()));

            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _dateTimeProvider.Setup(x => x.GetCurrentDateTime()).Returns(_currentDataTime);
        }

        private void SetupObjects()
        {
            _otaCampaignSubscribersTriggerSagaDataRes = new OTACampaignSubscribersTriggerSagaResult(_fileName);
            _otaCampaignSubscribersStarter = new OTACampaignSubscribersStarter(_fileContent, _fileName);
        }

        private void SetupValues()
        {
            _fileName = "OTA_CAMPGN_DTL_11.csv";
            _fileContent = "content";
            _currentDataTime = DateTime.Now;
        }

        private string GetSourcePath(string fileName)
        {
            string sourcePath = string.Concat(_otaCampaignSubscribersInbountPath, fileName);
            return sourcePath;
        }

        private string GetDestinationPath(string fileName, string processedPath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = Path.GetExtension(fileName);

            string destinationPath = string.Concat(
                processedPath,
                fileNameWithoutExtension,
                "_processedAt_",
                _currentDataTime.Year,
                "_",
                _currentDataTime.Month,
                "_",
                _currentDataTime.Day,
                "_",
                _currentDataTime.Hour,
                "_",
                _currentDataTime.Second,
                fileExtension
                );
            return destinationPath;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FinalizeWithSuccess_WhenExceptionIsThrown_PassItToCaller()
        {
            _sftp.Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            _otaCampaignSubscribersFinalizeImportMock.FinalizeWithSuccess(_otaCampaignSubscribersTriggerSagaDataRes);
        }

        [TestMethod]
        public void FinalizeWithSuccess_WithProvidedFileName_MoveFileToProcessedFolder()
        {
            _otaCampaignSubscribersFinalizeImportMock.FinalizeWithSuccess(_otaCampaignSubscribersTriggerSagaDataRes);

            var fileName = _otaCampaignSubscribersTriggerSagaDataRes.FileName;

            var sourcePath = GetSourcePath(fileName);
            var destinationPath = GetDestinationPath(fileName, _otaCampaignSubscribersProcessedPath);

            _sftp.Verify(x => x.MoveFileIfExist(sourcePath, destinationPath));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FinalizeWithError_WhenExceptionIsThrown_PassItToCaller()
        {
            _sftp.Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            _otaCampaignSubscribersFinalizeImportMock.FinalizeWithError(_otaCampaignSubscribersStarter);
        }

        [TestMethod]
        public void FinalizeWithError_WithProvidedFileName_MoveFileToErrorFolder()
        {
            _otaCampaignSubscribersFinalizeImportMock.FinalizeWithError(_otaCampaignSubscribersStarter);

            var fileName = _otaCampaignSubscribersTriggerSagaDataRes.FileName;

            var sourcePath = GetSourcePath(fileName);
            var destinationPath = GetDestinationPath(fileName, _otaCampaignSubscribersErrorPath);

            _sftp.Verify(x => x.MoveFileIfExist(sourcePath, destinationPath));
        }
    }
}
