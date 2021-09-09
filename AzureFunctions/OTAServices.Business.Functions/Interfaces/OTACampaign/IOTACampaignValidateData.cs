using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using SimProfileServiceReference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaign
{
    public interface IOTACampaignValidateData
    {
        Task<OTACampaignParseDataResult> ValidateAsync(OTACampaignParseDataResult parsedData);
    }
}
