using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    /// <summary>
    /// Represents a list of APN sets
    /// </summary>
    public class APNSetList
    {
        /// <summary>
        /// List of APN sets
        /// </summary>
        [JsonProperty("apn_sets")]
        public List<APNSet> APNSets { get; set; }
    }
}
