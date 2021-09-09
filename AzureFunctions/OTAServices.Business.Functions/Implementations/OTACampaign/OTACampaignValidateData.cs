using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using SimProfileServiceReference;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaign
{
    public class OTACampaignValidateData : IOTACampaignValidateData
    {
        private readonly SimProfileService _simProfileService;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;
        private readonly IJsonLogger _logger;

        public OTACampaignValidateData(SimProfileService simProfileService, IProvisioningDbUnitOfWork provisioningDbUnitOfWork, IJsonLogger logger)
        {
            _simProfileService = simProfileService;
            _provisioningDbUnitOfWork = provisioningDbUnitOfWork;
            _logger = logger;
        }

        public async Task<OTACampaignParseDataResult> ValidateAsync(OTACampaignParseDataResult parsedData)
        {
            _logger.LogEntry(parsedData);

            try
            {
                await ValidateCampaigns(parsedData);

                _logger.LogExit(null);
                return parsedData;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to validate parsed data {parsedData.FileName}.");
                throw;
            }
        }

        private async Task ValidateCampaigns(OTACampaignParseDataResult parsedData)
        {
            foreach (var campaign in parsedData.Campaigns)
            {
                //valid date
                if (DateTime.Compare(DateTime.Now, campaign.EndDate) > 0)
                {
                    throw new InvalidOperationException("Given date has passed.");
                }

                //valid Iccid count
                if (campaign.IccidCount == 0)
                {
                    throw new InvalidOperationException("Iccid Count is 0.");
                }

                //valid target sim profile
                var simProfile = await _simProfileService.GetSimProfileAsync(new GetSimProfileByIdContract { Id = campaign.TargetSimProfile });
                if (simProfile == null)
                {
                    throw new InvalidOperationException("Sim Profile does not exist.");
                }

                //OtaCampaignType type
                if (!campaign.Type.Equals(OasisCampaignType.OASIS_CAMPGN_MNGR.ToString()))
                {
                    throw new InvalidOperationException("Invalid Campaign Type.");
                }

                //check if the IMSI count it available in the ProvisioningDB through ProvisioningWcf
                var result = _provisioningDbUnitOfWork.ImsiSponsorsStatusRepository.GetImsiSponsorsStatusBySimProfileId(simProfile.Id);

                if (!result.Any())
                {
                    throw new InvalidOperationException($"There are no Imsi Sponsors related with used Target Sim Profile with id {simProfile.Id}");
                }

                if (result.Any(x => x.ImsiCount < campaign.IccidCount))
                {
                    throw new InvalidOperationException($"There are not enough available imsis for sponsor : {result.First(x => x.ImsiCount < campaign.IccidCount).ExternalId}");
                }
            }
        }
    }
}