using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestfulAPI.Common.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;
using static RestfulAPI.Constants.BalanceConstants;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    /// <summary>
    /// Commercial offer config
    /// </summary>
    public class CommercialOfferConfig
    {
        /// <summary>
        /// Id of the Commercial offer config
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the commercial offer config
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Code of the commercial offer config
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Is a shared wallet proposition
        /// </summary>
        [JsonProperty("is_shared_wallet")]
        public bool IsSharedWallet { get; set; }

        /// <summary>
        /// Type of service. Possible values - Voice, SMS and Data
        /// </summary>
        [RestrictEnum(typeof(ServiceType), ServiceType.DATA, ServiceType.SMS, ServiceType.VOICE)]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("service_type_code")]
        public ServiceType ServiceTypeCode { get; set; }

        /// <summary>
        /// Unique code of the subscription
        /// </summary>
        [JsonProperty("subscription_code")]
        public string SubscriptionCode { get; set; }

        /// <summary>
        /// Out of bundle usage limit
        /// </summary>
        [JsonProperty("quota")]
        public decimal Quota { get; set; }

        /// <summary>
        /// Period amount for bundle
        /// </summary>
        [JsonProperty("bundle_amount")]
        public decimal BundleAmount { get; set; }

        /// <summary>
        /// Specifies the recharge period for bundle. Possible values - MONTH
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("period_code")]
        public PeriodCode PeriodCode { get; set; }

        /// <summary>
        /// List of thresholds for the bundle
        /// </summary>
        [JsonProperty("treshold_limits")]
        public List<Tresholds> TresholdLimits { get; set; }

        /// <summary>
        /// List of zones connectivity is accepted. Possible values:1 to 9
        /// </summary>
        [Range(1, 9)]
        [JsonProperty("white_zones")]
        public List<int> WhiteZones { get; set; }

        /// <summary>
        /// List of zones connectivity is rejectedPossible values:1 to 9
        /// </summary>
        [Range(1, 9)]
        [JsonProperty("black_zones")]
        public List<int> BlackZones { get; set; }

        /// <summary>
        /// List of available zones
        /// </summary>
        [JsonProperty("available_zones")]
        public int? AvailableZones { get; set; }

        /// <summary>
        /// Metering
        /// </summary>
        [JsonProperty("metering", NullValueHandling = NullValueHandling.Ignore)]
        public Metering Metering { get; set; }

        /// <summary>
        /// FUP Metering
        /// </summary>
        [JsonProperty("metering_fup", NullValueHandling = NullValueHandling.Ignore)]
        public List<Metering> FUPMeterings { get; set; }
    }
}
