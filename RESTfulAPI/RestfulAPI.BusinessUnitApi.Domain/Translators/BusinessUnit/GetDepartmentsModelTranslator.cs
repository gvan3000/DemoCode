using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    public class GetDepartmentsModelTranslator : IGetDepartmentsModelTranslator
    {
        public GetDepartmentsModel Translate(List<DepartmentCostCenterContract> contract)
        {
            if (contract == null)
            {
                return null;
            }

            var translated = contract.Select(x => new GetDepartmentModel { Id = x.Id, Name = x.DepartmentName, CostCenter = x.CostCenterName}).ToList();
           
            GetDepartmentsModel getDepartmentsModel = new GetDepartmentsModel
            {
                GetDepartments = translated
            };

            return getDepartmentsModel;
            
        }
    }
}
