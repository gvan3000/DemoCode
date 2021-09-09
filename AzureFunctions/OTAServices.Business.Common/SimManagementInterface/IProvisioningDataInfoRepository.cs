using System.Collections.Generic;
using OTAServices.Business.Entities.Common;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface IProvisioningDataInfoRepository
    {
        List<ProvisioningDataInfo> GetProvisioningDataInfos(List<string> uiccids);
    }
}
