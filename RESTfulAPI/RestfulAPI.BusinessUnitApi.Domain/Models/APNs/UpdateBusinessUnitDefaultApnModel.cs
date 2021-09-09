using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    public class UpdateBusinessUnitDefaultApnModel
    {
        [JsonProperty("id")]
        [Required]
        public Guid Id { get; set; }
    }
}
