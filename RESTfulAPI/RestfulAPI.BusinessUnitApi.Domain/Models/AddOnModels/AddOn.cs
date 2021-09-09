using System;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels
{
    /// <summary>
    /// AddOn Model
    /// </summary>
    public class AddOn
    {
        /// <summary>
        /// Id of the entuty
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Type of entity.
        /// </summary>
        [JsonProperty("add_on_type")]
        public string AddOnType { get; set; }

        /// <summary>
        /// IsOneTime
        /// </summary>
        [JsonProperty("is_one_time")]
        public bool IsOneTime { get; set; }
    }
}
