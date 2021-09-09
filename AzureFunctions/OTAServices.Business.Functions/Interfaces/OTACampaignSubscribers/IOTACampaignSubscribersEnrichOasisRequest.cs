using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers
{
    public interface IOTACampaignSubscribersEnrichOasisRequest
    {
        Task<OTACampaignSubscribersEnrichOasisRequestResult> EnrichOasisRequestAsync(OTACampaignSubscribersLeaseImsiResult input);
    }
}
