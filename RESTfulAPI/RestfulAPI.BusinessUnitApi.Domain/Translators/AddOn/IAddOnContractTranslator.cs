using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public interface IAddOnContractTranslator
    {
        AddOnListModel Translate(AddOnsContract input, List<BusinessUnitAddOnMatrixxResourceContract> resourceContracts);
    }
}
