using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    /// <summary>
    /// List of all products with the maximum allowed usage per balance data model
    /// </summary>
    public class ProductAllowedBalanceModel
    {
        /// <summary>
        /// Id of the product
        /// </summary>
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// List of all maximum allowed usage for the product
        /// </summary>
        [JsonProperty("balance_allowances")]
        public List<BalanceAllowanceModel> BalanceAllowances { get; set; }
    }
}
