using OTAServices.Business.Entities.ImsiManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.ImsiManagementInterface
{
    public interface IImsiSponsorsStatusRepository
    {
        List<ImsiSponsorsStatus> GetImsiSponsorsStatusBySimProfileId(int simProfileId);
    }
}
