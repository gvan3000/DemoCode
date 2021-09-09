using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels
{
    /// <summary>
    /// Holds a collection of company balance profiles
    /// </summary>
    public class BalanceProfileListModel
    {
        /// <summary>
        /// a collection of company balance profiles
        /// </summary>
        [JsonProperty("balance_profiles")]
        public List<BalanceProfileModel> BalanceProfiles { get; set; }
    }
}
