using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels
{
    /// <summary>
    /// Describes purchased add-on for business unit
    /// </summary>
    public class AddOnModel
    {
        /// <summary>
        /// Add-on id of purchased add-on
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Type of purchased add-on
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Amount purchased if add-on supports ammount
        /// </summary>
        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// Start date for purchase (if applicable)
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Date when purchase expires (if applicable)
        /// </summary>
        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Resource id used to uniquely identify the purchase, it is needed when canceling the purchase and can be null if its unknown
        /// </summary>
        [JsonProperty("resource_id")]
        public int? ResourceId { get; set; }
    }
}
