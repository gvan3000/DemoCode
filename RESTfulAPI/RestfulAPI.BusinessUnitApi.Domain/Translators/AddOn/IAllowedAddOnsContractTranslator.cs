using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public interface IAllowedAddOnsContractTranslator
    {
        BusinessUnitAddOnsContract Translate(IEnumerable<Guid> addOnIds, Guid businessUnitId);
    }
}
