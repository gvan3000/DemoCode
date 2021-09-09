using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.BalanceService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public interface IBalanceTranslator : ITranslate<AccountBalanceWithBucketsContract[], Models.BalanceModels.BalancesResponseModel>
    {
    }
}
