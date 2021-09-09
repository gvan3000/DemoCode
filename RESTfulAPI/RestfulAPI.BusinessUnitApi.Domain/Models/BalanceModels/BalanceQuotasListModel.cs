using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    public class BalanceQuotasListModel
    {
        /// <summary>
        /// balance quotas
        /// </summary>
        [JsonProperty("balance_allowances")]
        public List<BalanceQuotaModel> BalanceAllowances { get; set; }
    }
}
