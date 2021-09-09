using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;
using Newtonsoft.Json.Converters;
using static RestfulAPI.Constants.BalanceConstants;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    public class SetBalanceModel : IValidatableObject
    {
        /// <summary>
        /// Amount to be used per bill cycle
        /// </summary>
        [JsonProperty("amount")]
        [Required]
        public decimal? Amount { get; set; }

        /// <summary>
        /// Possible values:
        /// if service_type_code is VOICE -> MINUTES
        /// if service_type_code is SMS -> SMS
        /// if service_type_code is DATA -> GB / MB / kB
        /// </summary>
        [JsonProperty("unit_type")]
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public UnitType UnitTypeValue { get; set; } //validation should be done in provider

        /// <summary>
        /// Service Type Coe - Possible values: VOICE SMS DATA
        /// </summary>
        [JsonProperty("service_type_code")]
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType ServiceTypeCode { get; set; }

        /// <summary>
        /// Validate ServiceType vs UnitType values
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((ServiceTypeCode == ServiceType.DATA && UnitTypeValue == UnitType.MB 
                || ServiceTypeCode == ServiceType.DATA && UnitTypeValue == UnitType.GB 
                || ServiceTypeCode == ServiceType.DATA && UnitTypeValue == UnitType.kB)
                || (ServiceTypeCode == ServiceType.SMS && UnitTypeValue == UnitType.SMS)
                || (ServiceTypeCode == ServiceType.VOICE && UnitTypeValue == UnitType.MINUTES)
                || (ServiceTypeCode == ServiceType.QUOTA && UnitTypeValue == UnitType.MONETARY))//UnitType.MONETARY added in the SPEC
            {
                yield return ValidationResult.Success;
            }
            else
            {
                yield return new ValidationResult("service_type_code and unit_type are mismatched");
            }
        }
    }
}
