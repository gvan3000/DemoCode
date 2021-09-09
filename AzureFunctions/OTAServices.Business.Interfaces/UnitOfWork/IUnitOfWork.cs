using System.Threading.Tasks;

namespace OTAServices.Business.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
    }
}
