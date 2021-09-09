using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using Teleena.AzureStorage.Client;
using Teleena.Sftp.Client;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaign
{
    public class OTACampaignFinalizeProcessing : IOTACampaignFinalizeProcessing
    {
        private const string ContainerName = "otacampaign";

        private readonly string _otaCampaignInboundPath;
        private readonly string _otaCampaignProcessedPath;
        private readonly string _otaCampaignErrorPath;
        private readonly ITeleenaSftpClient _sftpClient;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAzureStorageClient _azureStorageClient;
        private readonly IJsonLogger _logger;

        public OTACampaignFinalizeProcessing(
            string otaCampaignInboundPath, 
            string otaCampaignProcessedPath, 
            string otaCampaignErrorPath,
            ITeleenaSftpClient sftpClient, 
            IDateTimeProvider dateTimeProvider,
            IAzureStorageClient azureStorageClient, 
            IJsonLogger logger)
        {
            _otaCampaignInboundPath = otaCampaignInboundPath;
            _otaCampaignProcessedPath = otaCampaignProcessedPath;
            _otaCampaignErrorPath = otaCampaignErrorPath;
            _sftpClient = sftpClient;
            _dateTimeProvider = dateTimeProvider;
            _azureStorageClient = azureStorageClient;
            _logger = logger;
        }

        public void FinalizeWithError(OTACampaignStarter input)
        {
            _logger.LogEntry(input);
            _logger.LogMessage($"Started handling {nameof(OTACampaignFinalizeProcessing)}.{nameof(FinalizeWithError)} - {nameof(input.FileName)}={input.FileName}");

            try
            {
                DeleteFileFromBlobStorage(input.FileName);

                MoveFileFromInboundFolder(input.FileName, _otaCampaignErrorPath);

                _logger.LogExit(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed in {nameof(FinalizeWithError)}.");
                throw;
            }
        }

        public void FinalizeWithSuccess(OTACampaignSaveCampaingResult input)
        {
            _logger.LogEntry(input);
            _logger.LogMessage($"Started handling {nameof(OTACampaignFinalizeProcessing)}.{nameof(FinalizeWithSuccess)} - {nameof(input.FileName)}={input.FileName}");

            try
            {
                DeleteFileFromBlobStorage(input.FileName);

                MoveFileFromInboundFolder(input.FileName, _otaCampaignProcessedPath);

                _logger.LogExit(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed in {nameof(FinalizeWithSuccess)}.");
                throw;
            }
        }

        private void MoveFileFromInboundFolder(string fileName, string destinationPath)
        {
            var fileNameWithSourcePath = FileHelper.GetFileNameWithSourcePath(_otaCampaignInboundPath, fileName);

            var processedFileName = FileHelper.GetProcessedFileNameWithTimeStamp(fileName, _dateTimeProvider);
            var destinationPathWithProcessedFileName = FileHelper.GetFileNameWithDestinationPath(destinationPath, processedFileName);
            _sftpClient.MoveFileIfExist(fileNameWithSourcePath, destinationPathWithProcessedFileName);
        }

        private void DeleteFileFromBlobStorage(string fileName)
        {
            _azureStorageClient.DeleteBlobIfExistsAsync(ContainerName, fileName);
        }
    }
}