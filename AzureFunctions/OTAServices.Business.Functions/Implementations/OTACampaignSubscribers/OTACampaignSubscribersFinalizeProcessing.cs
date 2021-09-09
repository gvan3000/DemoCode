using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using Teleena.AzureStorage.Client;
using Teleena.Sftp.Client;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersFinalizeProcessing : IOTACampaignSubscribersFinalizeProcessing
    {
        private const string ContainerName = "otacampaignsubscribers";

        private readonly string _otaCampaignSubscribersInboundPath;
        private readonly string _otaCampaignSubscribersProcessedPath;
        private readonly string _otaCampaignSubscribersErrorPath;
        private readonly ITeleenaSftpClient _sftpClient;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJsonLogger _logger;
        private readonly IAzureStorageClient _azureStorageClient;

        public OTACampaignSubscribersFinalizeProcessing(
            string otaCampaignSubscribersInboundPath, 
            string otaCampaignSubscribersProcessedPath, 
            string otaCampaignSubscribersErrorPath,
            ITeleenaSftpClient sftpClient, 
            IDateTimeProvider dateTimeProvider,
            IAzureStorageClient azureStorageClient, 
            IJsonLogger logger)
        {
            _otaCampaignSubscribersInboundPath = otaCampaignSubscribersInboundPath;
            _otaCampaignSubscribersProcessedPath = otaCampaignSubscribersProcessedPath;
            _otaCampaignSubscribersErrorPath = otaCampaignSubscribersErrorPath;
            _sftpClient = sftpClient;
            _dateTimeProvider = dateTimeProvider;
            _azureStorageClient = azureStorageClient;
            _logger = logger;
        }

        public void FinalizeWithError(OTACampaignSubscribersStarter input)
        {
            _logger.LogEntry(input);
            _logger.LogMessage($"Started handling {nameof(OTACampaignSubscribersFinalizeProcessing)}.{nameof(FinalizeWithError)} - {nameof(input.FileName)}={input.FileName}");

            try
            {
                DeleteFileFromBlobStorage(input.FileName);

                MoveFileFromInboundFolder(input.FileName, _otaCampaignSubscribersErrorPath);

                _logger.LogExit(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to move file {input.FileName}.");
                throw;
            }
        }

        public void FinalizeWithSuccess(OTACampaignSubscribersTriggerSagaResult input)
        {
            _logger.LogEntry(input);
            _logger.LogMessage($"Started handling {nameof(OTACampaignSubscribersFinalizeProcessing)}.{nameof(FinalizeWithSuccess)} - {nameof(input.FileName)}={input.FileName}");
            try
            {
                DeleteFileFromBlobStorage(input.FileName);

                MoveFileFromInboundFolder(input.FileName, _otaCampaignSubscribersProcessedPath);

                _logger.LogExit(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to move file {input.FileName}.");
                throw;
            }
        }

        private void MoveFileFromInboundFolder(string fileName, string destinationPath)
        {
            var fileNameWithSourcePath = FileHelper.GetFileNameWithSourcePath(_otaCampaignSubscribersInboundPath, fileName);

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
