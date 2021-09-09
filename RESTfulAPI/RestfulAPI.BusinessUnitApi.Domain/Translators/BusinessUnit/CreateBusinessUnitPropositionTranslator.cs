using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public class CreateBusinessUnitPropositionTranslator : ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract>
    {
        public AccountPropositionsContract Translate(Models.BusinessUnitModels.Proposition input)
        {
            var result = new AccountPropositionsContract()
            {
                PropositionId = input.Id,
                EndUserSubscription = input.EndUserSubscription
            };

            return result;
        }
    }
}
