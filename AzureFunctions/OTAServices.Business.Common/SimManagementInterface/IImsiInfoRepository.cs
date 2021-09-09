using System.Collections.Generic;
using OTAServices.Business.Entities.Common;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface IImsiInfoRepository
    {
        List<ImsiInfo> GetImsiInfos(List<string> uiccids, int campaignId);
    }
}
