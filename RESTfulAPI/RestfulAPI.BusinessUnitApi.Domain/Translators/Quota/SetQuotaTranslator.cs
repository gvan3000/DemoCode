using System;
using System.Linq;
using System.Security.Claims;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.BalanceService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Quota
{
    public class SetQuotaTranslator : ISetQuotaTranslator
    {
        public TopUpAccountRequestContract Translate(Guid accountId, SetBusinessUnitQuotaModel input, ClaimsPrincipal user, CompanyBalanceTypeTopUpSettingContract balanceType)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("Must be set to valid value", nameof(accountId));
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (balanceType == null)
                throw new ArgumentNullException(nameof(balanceType));

            var setQuotaRequest = new TopUpAccountRequestContract()
            {
                AccountId = accountId,
                Amount = input.Amount,
                BalanceExpiry = BalanceConstants.BALANCE_EXPIRY,
                BalanceExpiryUnit = BalanceConstants.DAYS,
                BalanceMode = BalanceConstants.DELTA,
                BalanceType = balanceType.BalanceTypeName,
                NewBucket = false,
                OriginatingUserName = user.Claims.First(x => x.Type == CustomClaimTypes.EmailClaimType || x.Type == ClaimTypes.Email).Value
            };
            return setQuotaRequest;
        }
    }
}
