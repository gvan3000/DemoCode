using System;
using System.Collections.Generic;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public class AllowedAddOnsContractTranslator : IAllowedAddOnsContractTranslator
    {
        public BusinessUnitAddOnsContract Translate(IEnumerable<Guid> addOnIds, Guid businessUnitId)
        {
            BusinessUnitAddOnsContract contract = new BusinessUnitAddOnsContract
            {
                AccountId = businessUnitId,
                AllowedAddOns = addOnIds.ToList()
            };

            return contract;
        }
    }
}
