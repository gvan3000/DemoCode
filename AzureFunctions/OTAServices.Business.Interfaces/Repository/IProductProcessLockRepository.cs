using System.Collections.Generic;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IProductProcessLockRepository
    {
        void AddProductProcessLockBulk(List<string> uiccids);
        void DeleteProductProcessLockBulk(List<string> uiccids);
    }
}
