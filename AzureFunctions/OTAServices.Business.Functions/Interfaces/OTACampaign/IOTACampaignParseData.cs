using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Functions.Interfaces.OTACampaign
{
    public interface IOTACampaignParseData
    {
        OTACampaignParseDataResult Parse(OTACampaignStarter input);
    }
}
