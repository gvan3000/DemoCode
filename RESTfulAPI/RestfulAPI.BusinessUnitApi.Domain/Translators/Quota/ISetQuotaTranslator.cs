using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;
using System.Security.Claims;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Quota
{
    /// <summary>
    /// Produces set quota contract for business unit to be passed to the wcf service
    /// </summary>
    public interface ISetQuotaTranslator
    {
        /// <summary>
        /// Create service contract to pass to balnce service to set business unit quota
        /// </summary>
        /// <param name="accountId">id of business unit (account)</param>
        /// <param name="input">input model with amount</param>
        /// <param name="user">authenticated user making the call</param>
        /// <param name="balanceType">name of balance type for which quota is being set</param>
        /// <returns></returns>
        TopUpAccountRequestContract Translate(Guid accountId, SetBusinessUnitQuotaModel input, ClaimsPrincipal user, CompanyBalanceTypeTopUpSettingContract balanceType);
    }
}
