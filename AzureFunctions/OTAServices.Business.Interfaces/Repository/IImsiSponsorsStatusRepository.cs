using System.Collections.Generic;
using OTAServices.Business.Entities.ImsiManagement;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IImsiSponsorsStatusRepository
    {
        List<ImsiSponsorsStatus> GetImsiSponsorsStatusBySimProfileId(int simProfileId);
    }
}
