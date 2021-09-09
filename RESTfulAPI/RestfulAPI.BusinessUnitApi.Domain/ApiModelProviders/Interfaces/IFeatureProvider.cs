using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface IFeatureProvider
    {
        Task<bool> IsCompanyFeatureEnabled(Guid companyId, string featureCode);
    }
}
