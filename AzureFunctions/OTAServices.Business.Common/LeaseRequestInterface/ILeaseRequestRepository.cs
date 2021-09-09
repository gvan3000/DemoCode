using OTAServices.Business.Entities.LeaseRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.LeaseRequestInterface
{
    public interface ILeaseRequestRepository
    {
        void AddLeaseRequest(LeaseRequest leaseRequest);
    }
}
