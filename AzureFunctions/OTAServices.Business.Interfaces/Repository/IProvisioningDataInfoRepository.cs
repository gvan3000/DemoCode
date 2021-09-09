using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using System.Collections.Generic;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IProvisioningDataInfoRepository
    {
        List<ProvisioningDataInfo> GetProvisioningDataInfos(List<string> uiccids);
    }
}
