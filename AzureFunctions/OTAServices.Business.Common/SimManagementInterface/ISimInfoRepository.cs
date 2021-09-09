using OTAServices.Business.Entities.SimManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Common.SimManagementInterface
{
    public interface ISimInfoRepository
    {
        List<SimInfo> GetSimInfoBatch(List<string> uiccids);
    }
}
