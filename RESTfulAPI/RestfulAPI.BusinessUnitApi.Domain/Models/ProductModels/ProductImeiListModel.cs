using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    public class ProductImeiListModel
    {
        [JsonProperty("result")]
        public List<ProductImeiModel> Products { get; set; } = new List<ProductImeiModel>();
    }
}