using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels
{
    public class GetDepartmentsModel
    {
        [JsonProperty("departments")]
        public List<GetDepartmentModel> GetDepartments { get; set; }
    }
}
