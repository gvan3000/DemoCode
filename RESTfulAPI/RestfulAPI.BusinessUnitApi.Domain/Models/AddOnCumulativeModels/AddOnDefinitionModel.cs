using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.AddOnEnums;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels
{
    /// <summary>
    /// AddOn Definition Model
    /// </summary>
    public class AddOnDefinitionModel
    {
        /// <summary>
        /// AddOn ServiceTypeCode
        /// </summary>
        [JsonProperty("service_type_code")]
        [JsonConverter(typeof(StringEnumConverter))]        
        public ServiceTypeCode? ServiceTypeCode { get; set; }
        
        /// <summary>
        /// AddOn Amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// AddOn UnitType
        /// </summary>
        [JsonProperty("unit_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UnitType UnitType { get; set; }
    }
}
