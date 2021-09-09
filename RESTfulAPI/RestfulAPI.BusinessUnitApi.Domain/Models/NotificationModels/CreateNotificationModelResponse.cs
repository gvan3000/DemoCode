using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels
{
    public class CreateNotificationModelResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
