using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.TeleenaServiceReferences.PropositionService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition
{
    public interface IPropositionContractTranslator
    {
        PropositionsResponseModel Translate(PropositionsContract activePropositions, PropositionsContract productCreationPropositions);
    }
}
