using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public interface ISaveQuotaDistributionContractTranslator
    {
        SaveQuotaDistributionContract Translate(Guid businessUnitId, Guid productId, PropositionInfoModel propositionInfo, SetBalanceModel request);
    }
}
