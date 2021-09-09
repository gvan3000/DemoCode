using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    public class PagingInfo
    {
        [JsonProperty("total_products")]
        public int TotalProducts { get; set; }
    }
}
