using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.Business.Interfaces.UnitOfWork
{
    public interface IProvisioningDbUnitOfWork : IUnitOfWork
    {
        ISimContentRepository SimContentRepository { get; }
        ISimProfileSponsorRepository SimProfileSponsorRepository { get; }
        IImsiInfoRepository ImsiInfoRepository { get; }
        ISubscriberListLeaseRequestRepository LeaseRequestRepository { get; }
        IImsiSponsorsStatusRepository ImsiSponsorsStatusRepository { get; }
        IProvisioningDataInfoRepository ProvisioningDataInfoRepository { get; }
        ISimOrderLineRepository SimOrderLineRepository { get; }
    }
}
