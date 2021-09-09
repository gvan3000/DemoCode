using System.Collections.Generic;
using OTAServices.Business.Entities.SimManagement;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IImsiInfoRepository
    {
        List<ImsiInfo> GetImsiInfos(List<string> uiccids, int campaignId);
    }
}
