using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    /// <summary>
    /// Represents single APN set with list of APNs
    /// </summary>
    public class APNSet
    {
        /// <summary>
        /// Id of the APN set
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the APN set
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// List of APNs belonging to the APN set
        /// </summary>
        [JsonProperty("apns")]
        public List<APNResponseDetail> APNs { get; set; }
    }
}
