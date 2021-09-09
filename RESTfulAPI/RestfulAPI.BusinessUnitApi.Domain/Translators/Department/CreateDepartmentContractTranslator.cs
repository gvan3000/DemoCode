using System;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Department
{
    public class CreateDepartmentContractTranslator : ICreateDepartmentContractTranslator
    {
        public AddDepartmentCostCenterContract Translate(CreateDepartmentModel createModel, Guid businessUnitId)
        {
            if (createModel == null)
            {
                return null;
            }

            var result = new AddDepartmentCostCenterContract
            {
                AccountId = businessUnitId,
                CostCenterName = createModel.CostCenter ?? string.Empty,
                DepartmentName = createModel.Name
            };

            return result;
        }
    }
}
