using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Department
{
    public interface ICreateDepartmentContractTranslator
    {
        AddDepartmentCostCenterContract Translate(CreateDepartmentModel createModel, Guid businessUnitId);
    }
}
