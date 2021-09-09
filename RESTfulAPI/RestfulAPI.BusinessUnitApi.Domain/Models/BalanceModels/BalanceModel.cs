using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    /// <summary>
    /// Shared balance model
    /// </summary>
    public class BalanceModel
    {
        /// <summary>
        /// Name of the balance.
        /// </summary>
        [JsonProperty(PropertyName = "balance")]
        public string Balance { get; set; }

        /// <summary>
        /// Current balance amount.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The unit in which the amount is expresses. Possible values: minutes, USD, EUR, MB, SMS, etc.
        /// </summary>
        [JsonProperty(PropertyName = "unit_type")]
        public string UnitType { get; set; }

        /// <summary>
        /// Start date of the balance in UTC. Format: yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        [JsonProperty(PropertyName = "start_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Expiration date of the balance in UTC. Format: yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        [JsonProperty(PropertyName = "expiration_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// The balance service type(s). Possible values: Voice, SMS and Data. 
        /// </summary>
        [JsonProperty(PropertyName = "service_type", ItemConverterType =typeof(StringEnumConverter))]
        public List<Constants.BalanceConstants.ServiceType> ServiceType { get; set; }

        /// <summary>
        /// Monthly recharge amount.
        /// </summary>
        [JsonProperty(PropertyName = "initial_amount")]
        public decimal? InitialAmount { get; set; }

        /// <summary>
        /// Indicates an add-on balance.
        /// </summary>
        [JsonProperty(PropertyName = "is_addon")]
        public bool IsAddOn { get; set; }

        /// <summary>
        /// Indicates if balance is transferred from another product / business unit
        /// </summary>
        [JsonProperty(PropertyName = "is_transferred")]
        public bool IsTransferred { get; set; }

        /// <summary>
        /// Contains additional data about origin of balance
        /// </summary>
        [JsonProperty(PropertyName = "origin")]
        public BalanceOrigin Origin { get; set; }
    }
}

