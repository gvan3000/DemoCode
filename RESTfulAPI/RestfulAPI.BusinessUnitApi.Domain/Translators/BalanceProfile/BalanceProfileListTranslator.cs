using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BalanceProfile
{
    public class BalanceProfileListTranslator: ITranslate<SysCodeContract[], BalanceProfileListModel>
    {
        private readonly ITranslate<SysCodeContract, BalanceProfileModel> _innerTranslator;

        public BalanceProfileListTranslator(ITranslate<SysCodeContract, BalanceProfileModel> innerTranslator)
        {
            _innerTranslator = innerTranslator;
        }

        public BalanceProfileListModel Translate(SysCodeContract[] input)
        {
            if (input == null)
                return null;

            var translated = new BalanceProfileListModel()
            {
                BalanceProfiles = input.Select(_innerTranslator.Translate).ToList()
            };

            return translated;
        }
    }
}
