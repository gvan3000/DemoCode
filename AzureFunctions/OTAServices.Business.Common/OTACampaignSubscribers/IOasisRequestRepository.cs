using OTAServices.Business.Entities.OTACampaignSubscribers;
using System.Collections.Generic;

namespace OTAServices.Business.Common.OTACampaignSubscribers
{
    public interface IOasisRequestRepository
    {
        void AddOasisRequests(List<OasisRequest> oasisRequests);
        void DeleteOasisRequests(List<int> ids);
    }
}
