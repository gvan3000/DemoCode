using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels
{
    /// <summary>
    /// ProductTypeList Model
    /// </summary>
    public class ProductTypeListResponseModel
    {
        /// <summary>
        /// List of ProductTypeModel
        /// </summary>
        [JsonProperty("product_types")]
        public List<ProductTypeModel> ProductTypes { get; set; }
    }
}
