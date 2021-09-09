using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PropositionService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition
{
    public class AllowedPropositionContractTranslator : ITranslate<AllowedPropositionContract, Models.BusinessUnitModels.Proposition>
    {
        public Models.BusinessUnitModels.Proposition Translate(AllowedPropositionContract input)
        {
            if (input == null)
            {
                return null;
            }

            return new Models.BusinessUnitModels.Proposition()
            {
                EndUserSubscription = input.EndUserSubscription,
                Id = input.PropositionId
            };
        }
    }
}
