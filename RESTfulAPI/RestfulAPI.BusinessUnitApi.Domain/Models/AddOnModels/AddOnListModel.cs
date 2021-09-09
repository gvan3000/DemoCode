using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels
{
    public class AddOnListModel
    {
        [JsonProperty("add_ons")]
        public List<AddOnModel> AddOns { get; set; }
    }
}
