using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface IPreferredLanguageProvider
    {
        /// <summary>
        /// Get available languages for business unit by company id
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<AvailableLanguagesResponseModel>> GetAvailableLanguagesAsync(Guid businessUnitId, Guid companyId);

        /// <summary>
        /// Get business unit preferred languages by business unit id
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<PreferredLanguageResponseModel>> GetAccountLanguagesAsync(Guid businessUnitId);

        /// <summary>
        /// Update business unit preferred languages
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> UpdateAccountLanguagesAsync(Guid businessUnitId, Guid companyId, UpdatePreferredLanguagesRequestModel input);
    }
}
