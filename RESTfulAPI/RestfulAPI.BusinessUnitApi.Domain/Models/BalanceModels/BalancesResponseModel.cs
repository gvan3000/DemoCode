using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    /// <summary>
    /// Returns list of Balances shared balance model/>
    /// </summary>
    public class BalancesResponseModel
    {
        /// <summary>
        /// List of Balances
        /// </summary>
        [JsonProperty(PropertyName = "balances")]
        public List<BalanceModel> Balances { get; set; }

        /// <summary>
        /// Initialize new instance of Balance response
        /// </summary>
        public BalancesResponseModel()
        {
            Balances = new List<BalanceModel>();
        }
    }
}
