using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels
{
    public class GetDepartmentModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("cost_center")]
        public string CostCenter { get; set; }

    }
}
