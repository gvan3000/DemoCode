using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels
{
    public class UpdateDepartmentModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cost_center")]
        public string CostCenter { get; set; }

        [JsonIgnore]
        public Guid Id { get; set; }
    }
}
