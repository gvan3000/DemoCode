
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface IMaximityDbUnitOfWork : IUnitOfWork
    {
        ISimInfoRepository SimInfoRepository { get; }
        IProductInfoRepository ProductInfoRepository { get; }
        IProductProcessLockRepository ProductProcessLockRepository { get; }
    }
}
