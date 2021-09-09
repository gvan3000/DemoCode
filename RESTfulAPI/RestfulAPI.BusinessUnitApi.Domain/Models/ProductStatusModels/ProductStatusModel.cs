using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductStatusModels
{
    public class ProductStatusModel
    {
        [JsonProperty(PropertyName = "status")]
        public List<string> Status { get; set; }
    }
}
