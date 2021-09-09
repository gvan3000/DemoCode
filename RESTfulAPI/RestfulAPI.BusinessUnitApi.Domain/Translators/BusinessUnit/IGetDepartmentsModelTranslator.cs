using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public interface IGetDepartmentsModelTranslator
    {
        GetDepartmentsModel Translate(List<DepartmentCostCenterContract> contract);
    }
}
