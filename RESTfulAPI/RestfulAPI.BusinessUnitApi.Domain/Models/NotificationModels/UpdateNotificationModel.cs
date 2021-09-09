using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestfulAPI.Common.Validation.Attributes;
using RestfulAPI.TeleenaServiceReferences.Constants;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    /// <summary>
    /// Patch model for product notification update
    /// </summary>
    public class UpdateNotificationModel
    {
        /// <summary>
        /// List of deliveries to be set for notification
        /// </summary>
        [JsonProperty("deliveries")]
        [RequiredNotEmpty]
        public List<Delivery> Deliveries { get; set; }

        /// <summary>
        /// Id of product notification, will be filled in manually from the route
        /// </summary>
        [JsonIgnore]
        public NotificationType? Type { get; set; }

        /// <summary>
        /// Id of business unit whose notification is being changed, will be filled manually in from the route
        /// </summary>
        [JsonIgnore]
        public Guid BusinessUnitId { get; set; }
    }
}
