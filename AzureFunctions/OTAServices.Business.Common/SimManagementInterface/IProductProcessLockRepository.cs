using OTAServices.Business.Entities.SimManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface IProductProcessLockRepository
    {
        void AddProductProcessLockBulk(List<string> uiccids);
        void DeleteProductProcessLockBulk(List<string> uiccids);
    }
}
