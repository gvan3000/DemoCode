using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// provides company balance profiles
    /// </summary>
    public interface IBalanceProfileProvider
    {
        /// <summary>
        /// Fetch company balance profiles based on supplied business unit id
        /// </summary>
        /// <param name="businessUnitId">busines unit id for which balance profiles are being returned</param>
        /// <returns>model with filledi n listo f balance profiles on success or error indication in case of errors</returns>
        Task<ProviderOperationResult<BalanceProfileListModel>> GetBalanceProfilesAsync(Guid businessUnitId);
    }
}
