using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public class CreateBusinessUnitTranslator : ITranslate<BusinessUnitCreateModel, AddAccountContract>
    {
        private readonly ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract> _propositionTranslator;

        public CreateBusinessUnitTranslator(ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract> propositionTranslator)
        {
            _propositionTranslator = propositionTranslator;
        }

        public AddAccountContract Translate(BusinessUnitCreateModel input)
        {
            var result = new AddAccountContract()
            {
                PersonId = input.PersonId,
                CustomerNumber = input.CustomerId,
                UserName = input.Name,
                EnablePlans = input.IsUsingPlans
            };

            if (input.Propositions == null || input.Propositions.Count() < 0)
            {
                return result;
            }

            result.Propositions = input.Propositions.Select(_propositionTranslator.Translate).ToList();

            return result;
        }
    }
}
