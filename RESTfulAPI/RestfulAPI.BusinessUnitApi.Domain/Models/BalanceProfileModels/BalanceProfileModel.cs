using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels
{
    /// <summary>
    /// Holds detail information about company balance profile used in create product when company does not use propositions
    /// </summary>
    public class BalanceProfileModel
    {
        /// <summary>
        /// Id needed for create product process
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Descriptive code that shows what balance profile does
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
