using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels
{
    public class DeleteAddOnModel
    {

        [JsonProperty("add_on_id")]
        [Required]
        public Guid AddOnId { get; set; }

        [JsonProperty("resource_id")]
        [Required]
        public int ResourceId { get; set; }

        [JsonProperty("end_type")]
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public AddOnEnums.EndType EndType { get; set; }
    }
}
