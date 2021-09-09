using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.BalanceService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BalanceProfile
{
    public class BalanceProfileTranslator : ITranslate<SysCodeContract, BalanceProfileModel>
    {
        public BalanceProfileModel Translate(SysCodeContract input)
        {
            if (input == null)
                return null;

            var translated = new BalanceProfileModel()
            {
                Code = input.Code,
                Id = input.Id
            };

            return translated;
        }
    }
}
