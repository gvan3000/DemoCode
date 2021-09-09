using System.Collections.Generic;
using OTAServices.Business.Entities.SimManagement;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface ISimInfoRepository
    {
        List<SimInfo> GetSimInfoBatch(List<string> uiccids);
    }
}
