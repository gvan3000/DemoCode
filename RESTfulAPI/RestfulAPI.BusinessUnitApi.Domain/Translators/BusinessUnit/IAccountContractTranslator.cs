using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public interface IAccountContractTranslator: ITranslate<AccountContract, BusinessUnitModel>
    {
        BusinessUnitListModel Translate(List<AccountContract> accounts, List<BusinessUnitExtraInfoModel> propositionsAddOns, 
                                        List<PricePlanContract> pricePlans, bool resolveParentChildRelationship = false);
    }
}
