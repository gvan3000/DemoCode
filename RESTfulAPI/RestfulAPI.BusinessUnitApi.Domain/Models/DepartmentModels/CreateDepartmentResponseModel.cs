using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels
{
    public class CreateDepartmentResponseModel
    {
        /// <summary>
        /// Id Of The Department
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Url For The Department
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
