using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels
{
    /// <summary>
    /// AddOn Cumulative Model
    /// </summary>
    public class AddOnCumulativeModel
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the entity
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of the entity
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// PcrfType of the entity
        /// </summary>
        [JsonProperty("pcrf_type")]
        public string PcrfType { get; set; }

        /// <summary>
        /// DataSpeedType Id
        /// </summary>
        [JsonProperty("speed_id")]
        public Guid? SpeedId { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// IsOneTime
        /// </summary>
        [JsonProperty("is_one_time")]
        public bool? IsOneTime { get; set; }

        /// <summary>
        /// ResourceId
        /// </summary>
        [JsonProperty("resource_id")]
        public int? ResourceId { get; set; }

        /// <summary>
        /// Definitions
        /// </summary>
        [JsonProperty("definitions")]
        public List<AddOnDefinitionModel> Definitions { get; set; }      
    }
}
