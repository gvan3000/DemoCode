using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.Business.Interfaces.UnitOfWork
{
    public interface IOtaDbUnitOfWork : IUnitOfWork
    {
        IOasisRequestRepository OasisRequestRepository { get; }
        IOTACampaignRepository OTACampaignRepository { get; }
        IDeleteIMSICallbackRepository DeleteIMSICallbackRepository { get; }
    }
}
