using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using System.Text.RegularExpressions;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaign
{
    public class OTACampaignParseData : IOTACampaignParseData
    {
        private readonly IJsonLogger _logger;

        public OTACampaignParseData(IJsonLogger logger)
        {
            _logger = logger;
        }

        public OTACampaignParseDataResult Parse(OTACampaignStarter input)
        {
            _logger.LogEntry(input);

            try
            {
                ValidateFile(input);
                ValidateHeaders(input);

                var res = new OTACampaignParseDataResult(input.FileName);

                var fileLines = input.FileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                for (var i = 1; i < fileLines.Length; i++)
                {
                    var csvDataLine = csvParser.Split(fileLines[i]);

                    ValidateCampaign(csvDataLine);

                    var campaign = new Campaign
                    {
                        Id = int.Parse(csvDataLine[0]),
                        Description = GetContentWithoutQuotes(csvDataLine[1]),
                        Type = GetContentWithoutQuotes(csvDataLine[2]),
                        IccidCount = int.Parse(csvDataLine[3]),
                        TargetSimProfile = int.Parse(csvDataLine[4]),
                        EndDate = DateTime.Parse(GetContentWithoutQuotes(csvDataLine[5])),
                        StartDate = DateTime.Now
                    };

                    res.Campaigns.Add(campaign);
                }

                _logger.LogExit(res);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to parse file {input.FileName}.");
                throw;
            }
        }

        private static void ValidateFile(OTACampaignStarter input)
        {
            var fileLines = input.FileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (fileLines.Length == 0)
            {
                throw new InvalidOperationException("Passed CSV does not have header.");
            }

            if (fileLines.Length == 1)
            {
                throw new InvalidOperationException("Passed CSV does not have content.");
            }
        }

        private static void ValidateHeaders (OTACampaignStarter input)
        {
            var fileLines = input.FileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var csvDataLine = fileLines[0].Split(',');

            if (csvDataLine.Length != 6)
            {
                throw new InvalidOperationException("Passed CSV invalid header column count.");
            }

            if (csvDataLine[0] != "ID" || csvDataLine[1] != "OTA_CAMPGN_DESCR" || csvDataLine[2] != "OTA_CAMPGN_TYPE" || csvDataLine[3] != "ICCID_CNT" || csvDataLine[4] != "TARGT_SIM_PRFIL_ID" || csvDataLine[5] != "TARGT_DT")
            {
                throw new InvalidOperationException("Passed CSV invalid column format.");
            }
        }

        private static void ValidateCampaign(string[] input)
        {
            if (GetContentWithoutQuotes(input[1]).Trim() == "") throw new InvalidOperationException("Passed Description invalid value.");
            if (GetContentWithoutQuotes(input[2]).Trim() == "") throw new InvalidOperationException("Passed Type invalid value.");
            if (int.Parse(input[3]) < 1) throw new InvalidOperationException("Passed Iccid count invalid value.");
            if (int.Parse(input[4]) < 1) throw new InvalidOperationException("Passed Target Sim Profile invalid value.");
            if (DateTime.Parse(GetContentWithoutQuotes(input[5])) < DateTime.Now) throw new InvalidOperationException("Passed End date must be valid date, in future.");
        }

        private static string GetContentWithoutQuotes(string content)
        {
            return content.Replace("\"", string.Empty);
        }
    }
}