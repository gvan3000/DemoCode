using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels
{
    /// <summary>
    /// Msisdn model
    /// </summary>
    public class MsisdnModel
    {
        /// <summary>
        /// The MSISDN
        /// </summary>
        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        /// <summary>
        /// Msisdn status. Possible values: ACTIVATED, AVAILABLE, QUARANTINE, RETURNED, RESERVED, OUTPORTED
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// ISO-3166-1 2-digit country code
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Id of the product
        /// </summary>
        [JsonProperty("product_id")]
        public Guid? ProductId { get; set; }

        /// <summary>
        /// IsVirtual 
        /// </summary>
        [JsonProperty("is_virtual")]
        public bool IsVirtual { get; set; }
    }
}
