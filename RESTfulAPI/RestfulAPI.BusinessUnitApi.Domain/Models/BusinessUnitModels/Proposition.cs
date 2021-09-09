using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    /// <summary>
    /// Proposition
    /// </summary>
    public class Proposition
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [JsonProperty("id")]
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// End User Subscription
        /// </summary>
        [JsonProperty("end_user_subscription")]
        [Required]
        public bool EndUserSubscription { get; set; }

        /// <summary>
        /// Id of the Account
        /// </summary>
        [JsonIgnore]
        public Guid? BusinessUnitId { get; set; }
    }
}
