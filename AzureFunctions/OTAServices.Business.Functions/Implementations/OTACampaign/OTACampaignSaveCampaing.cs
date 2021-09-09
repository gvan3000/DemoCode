using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaign
{
    public class OTACampaignSaveCampaing : IOTACampaignSaveCampaing
    {
        private readonly IOtaDbUnitOfWork _otaDbUnitOfWork;
        private readonly IJsonLogger _logger;

        public OTACampaignSaveCampaing(IOtaDbUnitOfWork otaDbUnitOfWork, IJsonLogger logger)
        {
            _otaDbUnitOfWork = otaDbUnitOfWork;
            _logger = logger;
        }

        public async Task<OTACampaignSaveCampaingResult> SaveCampaingAsync(OTACampaignParseDataResult parsedData)
        {
            _logger.LogEntry(parsedData);
            try
            {
                _otaDbUnitOfWork.BeginTransaction();

                foreach (var otaCampaign in parsedData.Campaigns)
                {
                    var existingCampaign = _otaDbUnitOfWork.OTACampaignRepository.GetCampaign(otaCampaign.Id);

                    if (existingCampaign == null)
                    {
                        _otaDbUnitOfWork.OTACampaignRepository.AddCampaign(otaCampaign);
                    }
                    else
                    {
                        MapValues(existingCampaign, otaCampaign);
                        _otaDbUnitOfWork.OTACampaignRepository.UpdateCampaign(existingCampaign);
                    }
                }

                await _otaDbUnitOfWork.CommitTransactionAsync();

                _logger.LogExit(null);
                return new OTACampaignSaveCampaingResult(parsedData.FileName);
            }
            catch (Exception ex)
            {
                _otaDbUnitOfWork.RollbackTransaction();
                _logger.LogException(ex, $"Failed to import file {parsedData.FileName}.");
                throw;
            }
        }

        private static void MapValues(Campaign existingCampaign, Campaign oTAcampaign)
        {
            existingCampaign.IccidCount = oTAcampaign.IccidCount;
            existingCampaign.Description = oTAcampaign.Description;
            existingCampaign.EndDate = oTAcampaign.EndDate;
            existingCampaign.StartDate = oTAcampaign.StartDate;
            existingCampaign.TargetSimProfile = oTAcampaign.TargetSimProfile;
            existingCampaign.Type = oTAcampaign.Type;
        }
    }
}