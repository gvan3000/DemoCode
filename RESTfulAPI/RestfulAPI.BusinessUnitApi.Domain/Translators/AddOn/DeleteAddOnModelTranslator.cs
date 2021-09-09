using System;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.TeleenaServiceReferences.AddOnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public class DeleteAddOnModelTranslator : IDeleteAddOnModelTranslator
    {
        public AddOnCancelParam Translate(DeleteAddOnModel deleteModel, Guid businessUnitId)
        {
            return new AddOnCancelParam()
            {
                AccountId = businessUnitId,
                AddOnId = deleteModel.AddOnId,
                ResourceId = deleteModel.ResourceId
            };

        }
    }
}
