using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestfulAPI.Common.Validation.Attributes;
using BalanceConsts = RestfulAPI.Constants.BalanceConstants;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    public class BalanceQuotaModel
    {
        /// <summary>
        /// balance amaount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// service type code
        /// </summary>
        [RestrictEnum(typeof(BalanceConsts.ServiceType), BalanceConsts.ServiceType.DATA, BalanceConsts.ServiceType.SMS, BalanceConsts.ServiceType.VOICE)]
        [JsonProperty("service_type_code")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BalanceConsts.ServiceType ServiceTypeCode { get; set; }

        /// <summary>
        /// unit type
        /// </summary>
        [JsonProperty("unit_type")]
        public string UnitType { get; set; }

        /// <summary>
        /// Is Capped
        /// </summary>
        [JsonProperty("is_capped")]
        public bool IsCapped { get; set; }
    }
}
