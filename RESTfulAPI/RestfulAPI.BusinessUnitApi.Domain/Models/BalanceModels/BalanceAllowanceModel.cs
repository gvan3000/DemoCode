using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    /// <summary>
    /// Maximium allowed usage for the product data containing model
    /// </summary>
    public class BalanceAllowanceModel
    {
        /// <summary>
        /// Initial amount used
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Outstanding amount
        /// </summary>
        [JsonProperty("outstanding_amount")]
        public decimal? OutstandingAmount { get; set; }

        /// <summary>
        /// Possible values : MINUTES, SMS, MB
        /// </summary>
        [JsonProperty("unit_type")]
        public string UnitType { get; set; }

        /// <summary>
        /// Possible values: VOICE, SMS, DATA
        /// </summary>
        [JsonProperty("service_type_code")]
        public string ServiceTypeCode { get; set; }
    }
}
