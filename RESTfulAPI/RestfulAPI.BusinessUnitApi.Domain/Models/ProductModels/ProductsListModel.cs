using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    public class ProductsListModel
    {
        [JsonProperty("products")]
        public List<ProductModel> Products { get; set; } = new List<ProductModel>();
        [JsonProperty("paging", NullValueHandling = NullValueHandling.Ignore)]
        public PagingInfo PagingInfo { get; set; }
    }
}
