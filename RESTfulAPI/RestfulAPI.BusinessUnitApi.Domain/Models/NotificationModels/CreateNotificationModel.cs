using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestfulAPI.Common.Validation.Attributes;
using RestfulAPI.TeleenaServiceReferences.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    public class CreateNotificationModel
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public NotificationType? Type { get; set; }
        [JsonProperty("deliveries")]
        [RequiredNotEmpty]
        public List<Delivery> Deliveries { get; set; }
    }
}
