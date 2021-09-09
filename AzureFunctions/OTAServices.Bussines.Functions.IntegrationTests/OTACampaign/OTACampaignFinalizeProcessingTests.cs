using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using Teleena.AzureStorage.Client;
using Teleena.Sftp.Client;
using TeleenaFileLogging.AzureFunctions;

namespace OTAServices.Bussines.Functions.IntegrationTests.OTACampaign
{
    [TestClass]
    public class OTACampaignFinalizeProcessingTests
    {
        private DateTime _currentDate;
        private string _OTAcampaignInboundPath;
        private string _OTAcampaignProcessedPath;
        private string _OTAcampaignErrorPath;
        private string _fileName;
        private string _fileContent;

        private OTACampaignSaveCampaingResult _OTACampaignImportDataRes;
        private OTACampaignStarter _OTACampaignStarter;

        private OTACampaignFinalizeProcessing _OTACampaignFinalizeImportMock;

        private Mock<ITeleenaSftpClient> _sftp;
        private Mock<IAzureStorageClient> _azureStorage;
        private Mock<IDateTimeProvider> _dateTimeProvider;

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
            _currentDate = DateTime.Now;
            _OTAcampaignInboundPath = "OTACampaign\\ToTeleena\\";
            _OTAcampaignProcessedPath = "OTACampaign\\Processed\\";
            _OTAcampaignErrorPath = "OTACampaign\\Error\\";
            _fileName = "test.csv";
            _fileContent = "content";
        }

        private void SetupObjects()
        {
            _OTACampaignImportDataRes = new OTACampaignSaveCampaingResult(_fileName);
            _OTACampaignStarter = new OTACampaignStarter(_fileContent, _fileName);
        }

        private void SetupStubs()
        {

            _sftp = new Mock<ITeleenaSftpClient>();
            _sftp.Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>()));

            _azureStorage = new Mock<IAzureStorageClient>();
            _azureStorage.Setup(x => x.DeleteBlobIfExistsAsync(It.IsAny<string>(), It.IsAny<string>()));

            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _dateTimeProvider.Setup(x => x.GetCurrentDateTime()).Returns(_currentDate);
        }

        private void SetupMock()
        {
            _OTACampaignFinalizeImportMock = new OTACampaignFinalizeProcessing(
                _OTAcampaignInboundPath,
                _OTAcampaignProcessedPath,
                _OTAcampaignErrorPath,
                _sftp.Object,
                _dateTimeProvider.Object,
                _azureStorage.Object, 
                new AzureFunctionJsonLogger());
        }

        private string GetSourcePath(string fileName)
        {
            string sourcePath = string.Concat(_OTAcampaignInboundPath, fileName);
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
                _currentDate.Year,
                "_",
                _currentDate.Month,
                "_",
                _currentDate.Day,
                "_",
                _currentDate.Hour,
                "_",
                _currentDate.Second,
                fileExtension
                );
            return destinationPath;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FinalizeWithSuccess_WhenExceptionIsThrown_PassItToCaller()
        {
            _sftp
                .Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            _OTACampaignFinalizeImportMock.FinalizeWithSuccess(_OTACampaignImportDataRes);
        }

        [TestMethod]
        public void FinalizeWithSuccess_MoveFileToProcessedFolder_WithProperSuffix()
        {
            _OTACampaignFinalizeImportMock.FinalizeWithSuccess(_OTACampaignImportDataRes);

            var fileName = _OTACampaignImportDataRes.FileName;

            var sourcePath = GetSourcePath(fileName);
            var destinationPath = GetDestinationPath(fileName, _OTAcampaignProcessedPath);

            _sftp.Verify(x => x.MoveFileIfExist(sourcePath, destinationPath));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FinalizeWithError_WhenExceptionIsThrown_PassItToCaller()
        {
            _sftp
                .Setup(x => x.MoveFileIfExist(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            _OTACampaignFinalizeImportMock.FinalizeWithError(_OTACampaignStarter);
        }

        [TestMethod]
        public void FinalizeWithError_MoveFileToErrorFolder_WithProperSuffix()
        {
            _OTACampaignFinalizeImportMock.FinalizeWithError(_OTACampaignStarter);

            var fileName = _OTACampaignStarter.FileName;

            var sourcePath = GetSourcePath(fileName);
            var destinationPath = GetDestinationPath(fileName, _OTAcampaignErrorPath);

            _sftp.Verify(x => x.MoveFileIfExist(sourcePath, destinationPath));
        }
    }
}
