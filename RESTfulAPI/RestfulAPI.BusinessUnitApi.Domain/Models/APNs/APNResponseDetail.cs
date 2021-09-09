using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    public class APNResponseDetail
    {
        /// <summary>
        /// Apn id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Apn name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
