using System;
using System.Threading.Tasks;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface ISmscSettingProvider
    {
        Task<ProviderOperationResult<object>> CreateOrUpdate(bool useOaBasedSmsRouting, Guid accountId, string user);
    }
}
