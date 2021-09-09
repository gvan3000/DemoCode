using System.Threading.Tasks;

namespace OTAServices.Business.Common
{
    public interface IUnitOfWork
    {
        Task CommitTransactionAsync();
    }
}
