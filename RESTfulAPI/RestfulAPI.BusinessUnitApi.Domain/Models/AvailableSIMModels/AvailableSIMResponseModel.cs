using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels
{
    public class AvailableSIMResponseModel
    {
        [JsonProperty("iccid")]
        public List<string> Iccid { get; set; }
    }
}
