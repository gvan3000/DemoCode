using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    /// <summary>
    /// Notification configuration list data model
    /// </summary>
    public class GetNotificationListDataModel
    {
        /// <summary>
        /// List of Notifications Configurations
        /// </summary>
        [JsonProperty("notifications")]
        public List<GetNotificationDataModel> Notifications { get; set; }
    }
}
