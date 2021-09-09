using System.Collections.Generic;
using OTAServices.Business.Entities.SimManagement;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface ISimContentRepository
    {
        List<SimContent> GetSimContentBatch(List<string> uiccids, int campaignId);
    }
}
