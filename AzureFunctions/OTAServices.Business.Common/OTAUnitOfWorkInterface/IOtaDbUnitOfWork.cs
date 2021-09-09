using OTAServices.Business.Common.OTACampaignInterface;
using OTAServices.Business.Common.OTACampaignSubscribers;

namespace OTAServices.Business.Common.OTAUnitOfWorkInterface
{
    public interface IOtaDbUnitOfWork : IUnitOfWork
    {
        IOasisRequestRepository OasisRequestRepository { get; }
        IOTACampaignRepository OTACampaignRepository { get; }
    }
}
