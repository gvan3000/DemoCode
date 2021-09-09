using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public interface IDeleteAddOnModelTranslator
    {
        AddOnCancelParam Translate(DeleteAddOnModel deleteModel, Guid businessUnitId);
    }
}
