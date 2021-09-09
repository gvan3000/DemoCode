using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.Business.Interfaces.UnitOfWork
{
    public interface IMaximityDbUnitOfWork : IUnitOfWork
    {
        ISimInfoRepository SimInfoRepository { get; }
        IProductInfoRepository ProductInfoRepository { get; }
        IProductProcessLockRepository ProductProcessLockRepository { get; }
    }
}
