using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using System.Collections.Generic;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IProductInfoRepository
    {
        List<ProductInfo> GetProductInfos(List<string> uiccids);
    }
}
