using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using OTAServices.Business.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersParseData : IOTACampaignSubscribersParseData
    {
        private const int BatchSize = 10000;

        private readonly IJsonLogger _logger;
        private readonly IOtaDbUnitOfWork _otaDbUnitOfWork;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;

        public OTACampaignSubscribersParseData(IOtaDbUnitOfWork otaDbUnitOfWork, IProvisioningDbUnitOfWork provisioningDbUnitOfWork, IJsonLogger logger)
        {
            _logger = logger;
            _otaDbUnitOfWork = otaDbUnitOfWork;
            _provisioningDbUnitOfWork = provisioningDbUnitOfWork;
        }

        public OTACampaignSubscribersParseDataResult Parse(OTACampaignSubscribersStarter input)
        {
            _logger.LogEntry(input);
            try
            {
                ValidateFileContent(input.FileContent);

                var parsedOasisRequests = ParseFileContent(input.FileContent);

                var result = new OTACampaignSubscribersParseDataResult(input.FileName, parsedOasisRequests);

                _logger.LogExit(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to parse {input.FileName}.");
                throw;
            }
        }

        private List<OasisRequest> ParseFileContent(string fileContent)
        {
            var res = new List<OasisRequest>();

            var fileLines = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var campaignTargetSimPair = new Dictionary<int, int>();

            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            for (var i = 1; i < fileLines.Length; i++)
            {
                var csvDataLine = csvParser.Split(fileLines[i]);

                var oasisRequest = new OasisRequest
                {
                    CampaignId = int.Parse(csvDataLine[0]),
                    Iccid = GetContentWithoutQuotes(csvDataLine[2]),
                    Notes = GetContentWithoutQuotes(csvDataLine[4])
                };

                if (string.IsNullOrEmpty(GetContentWithoutQuotes(csvDataLine[3])))
                {
                    if (!campaignTargetSimPair.ContainsKey(oasisRequest.CampaignId))
                    {
                        var campaign = _otaDbUnitOfWork.OTACampaignRepository.GetCampaign(oasisRequest.CampaignId);

                        if (campaign == null)
                        {
                            throw new InvalidOperationException($"Campaign with id {oasisRequest.CampaignId} does not exist in OTA DB.");
                        }

                        var targetSimProfile = campaign.TargetSimProfile;
                        oasisRequest.TargetSimProfileId = targetSimProfile;
                        campaignTargetSimPair.Add(oasisRequest.CampaignId, targetSimProfile);
                    }
                    else
                    {
                        oasisRequest.TargetSimProfileId = campaignTargetSimPair[oasisRequest.CampaignId];
                    }
                }
                else
                {
                    oasisRequest.TargetSimProfileId = int.Parse(GetContentWithoutQuotes(csvDataLine[3]));
                }

                oasisRequest.Status = string.Empty;

                res.Add(oasisRequest);
            }

            SetOriginalProfileIds(res);

            return res;
        }

        private void SetOriginalProfileIds(List<OasisRequest> res)
        {
            var oasisRequestsBatches = res.BatchBy(BatchSize);

            foreach (var oasisRequestsBatch in oasisRequestsBatches)
            {
                var iccidsSimProfiles = _provisioningDbUnitOfWork.SimOrderLineRepository.GetSimProfileByUiccidBatch(oasisRequestsBatch.Select(x => x.Iccid).ToList());

                var iccidsSimProfileDictionary = new Dictionary<string, int?>();

                foreach (var iccidSimProfile in iccidsSimProfiles)
                {
                    iccidsSimProfileDictionary.Add(iccidSimProfile.Uiccid, iccidSimProfile.SimProfileId);
                }

                foreach (var oasisRequest in oasisRequestsBatch)
                {
                    oasisRequest.OriginalSimProfileId = iccidsSimProfileDictionary[oasisRequest.Iccid];
                }
            }
        }

        private static void ValidateFileContent(string fileContent)
        {
            var fileLines = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (fileLines.Length == 0)
            {
                throw new InvalidOperationException("Passed CSV does not have header.");
            }

            if (fileLines.Length == 1)
            {
                throw new InvalidOperationException("Passed CSV does not have content.");
            }

            var headers = fileLines[0].Split(',');

            if (!IsOTA_CAMPGN_DTL(headers))
            {
                throw new InvalidOperationException("Passed CSV does not have proper headers.");
            }
        }

        private static bool IsOTA_CAMPGN_DTL(string[] headers)
        {
            return headers.Length == 5
                && headers[0] == "OTA_CAMPGN_ID" 
                && headers[1] == "REL_NO"
                && headers[2] == "ICCID" 
                && headers[3] == "TARGT_SIM_PRFIL_ID"
                && headers[4] == "COMMT";
        }

        private static string GetContentWithoutQuotes(string content)
        {
            return content.Replace("\"", string.Empty);
        }
    }
}