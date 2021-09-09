using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Department
{
    public class UpdateDepartmentContractTranslator : ITranslate<UpdateDepartmentModel, UpdateDepartmentCostCenterContract>
    {
        public UpdateDepartmentCostCenterContract Translate(UpdateDepartmentModel input)
        {
            if (input == null)
                return null;

            var result = new UpdateDepartmentCostCenterContract()
            {
                Id = input.Id,
                DepartmentName = input.Name,
                CostCenterName = input.CostCenter
            };

            return result;
        }
    }
}
