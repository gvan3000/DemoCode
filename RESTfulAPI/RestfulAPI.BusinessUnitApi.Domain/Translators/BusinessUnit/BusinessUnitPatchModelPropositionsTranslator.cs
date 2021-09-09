using System.Collections.Generic;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public class BusinessUnitPatchModelPropositionsTranslator : ITranslate<BusinessUnitPatchModel, UpdateBusinessUnitPropositionsContract>
    {
        public UpdateBusinessUnitPropositionsContract Translate(BusinessUnitPatchModel input)
        {
            UpdateBusinessUnitPropositionsContract patchPropositionsContract = null;

            UpdateBusinessUnitPropositionContract contract = null;

            List<UpdateBusinessUnitPropositionContract> list = new List<UpdateBusinessUnitPropositionContract>();

            foreach (var item in input.Propositions)
            {
                contract = new UpdateBusinessUnitPropositionContract
                {
                    Id = item.Id,
                    EndUserSubscritpion = item.EndUserSubscription
                };

                list.Add(contract);
            }

            patchPropositionsContract = new UpdateBusinessUnitPropositionsContract();
            patchPropositionsContract.Propositions = list;

            return patchPropositionsContract;
        }
    }
}
