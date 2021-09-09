using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestfulAPI.TeleenaServiceReferences.Constants;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    /// <summary>
    /// Notification data model
    /// </summary>
    public class GetNotificationDataModel
    {
        /// <summary>
        /// Type of the Notification
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationType Type { get; set; }

        /// <summary>
        /// List of Deliveries
        /// </summary>
        [JsonProperty("deliveries")]
        public List<Delivery> Deliveries { get; set; }
    }
}
