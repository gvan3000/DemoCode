using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    public class APNRequestDetail
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
