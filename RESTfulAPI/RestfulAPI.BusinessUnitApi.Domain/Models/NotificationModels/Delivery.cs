using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.NotificationsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    /// <summary>
    /// Delivery data model
    /// </summary>
    public class Delivery
    {
        /// <summary>
        /// Delivery method
        /// </summary>
        [JsonProperty("delivery_method")]
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public DeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// Delivery value
        /// </summary>
        [JsonProperty("delivery_value")]
        [Required]
        public string DeliveryValue { get; set; }

        /// <summary>
        /// Delivery options
        /// </summary>
        [JsonProperty("delivery_options", NullValueHandling = NullValueHandling.Ignore)]
        public DeliveryOption DeliveryOptions { get; set; }
    }
}
