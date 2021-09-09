using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.NotificationsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    /// <summary>
    /// Delivery options model
    /// </summary>
    public class DeliveryOption
    {
        /// <summary>
        /// Delivery options type
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public DeliveryOptionsType? Type { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Users password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}