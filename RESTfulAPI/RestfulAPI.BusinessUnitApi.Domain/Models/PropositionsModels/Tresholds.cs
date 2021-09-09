using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    /// <summary>
    /// Tresholds
    /// </summary>
    public class Tresholds
    {
        /// <summary>
        /// Id of the treshold
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Treshold type. values - BUNDLE, QUOTA
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public TypeOfTreshold Type { get; set; }

        /// <summary>
        /// treshold limit
        /// </summary>
        [JsonProperty("limit")]
        public decimal Limit { get; set; }
    }
}
