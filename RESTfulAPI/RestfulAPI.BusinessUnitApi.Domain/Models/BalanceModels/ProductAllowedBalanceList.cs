using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{    
    /// <summary>
    /// 
    /// </summary>
    public class ProductAllowedBalanceList
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("product_allowed_balances")]
        public List<ProductAllowedBalanceModel> ProductAllowedBalances { get; set; }
    }
}
