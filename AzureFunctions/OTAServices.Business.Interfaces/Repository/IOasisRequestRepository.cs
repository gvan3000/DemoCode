using System;
using System.Collections.Generic;
using OTAServices.Business.Entities.OTACampaignSubscribers;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IOasisRequestRepository
    {
        void AddOasisRequests(List<OasisRequest> oasisRequests);
        void DeleteOasisRequests(List<int> ids);
        OasisRequest GetByIccidAndSubscriberListId(string iccid, Guid subscriberListId);
    }
}
