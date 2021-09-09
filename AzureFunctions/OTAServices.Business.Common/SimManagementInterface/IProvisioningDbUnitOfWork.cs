
using OTAServices.Business.Common.ImsiManagementInterface;
using OTAServices.Business.Common.LeaseRequestInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface IProvisioningDbUnitOfWork : IUnitOfWork
    {
        ISimContentRepository SimContentRepository { get; }
        ISimProfileSponsorRepository SimProfileSponsorRepository { get; }
        IImsiInfoRepository ImsiInfoRepository { get; }
        ILeaseRequestRepository LeaseRequestRepository { get; }
        IImsiSponsorsStatusRepository ImsiSponsorsStatusRepository { get; }
        IProvisioningDataInfoRepository ProvisioningDataInfoRepository { get; }
    }
}
