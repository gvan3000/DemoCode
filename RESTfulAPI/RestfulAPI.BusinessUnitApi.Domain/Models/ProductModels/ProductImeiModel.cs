using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    public class ProductImeiModel
    {
        /// <summary>
        /// Unique identifier of the product
        /// </summary>
        [JsonProperty("product_id")]
        public Guid Id { get; set; }

        /// <summary>
        /// IMEI of the product
        /// </summary>
        [JsonProperty("imei")]
        public string Imei { get; set; }

        /// <summary>
        /// ICCID of the product
        /// </summary>
        [JsonProperty("iccid")]
        public string Iccid { get; set; }
        
        /// <summary>
        /// Timestamp of ProductImei record
        /// </summary>
        [JsonProperty("timestamp", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("imei_sv")]
        public string ImeiSV { get; set; }

        [JsonProperty("imei_sv_timestamp", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTimeOffset? ImeiSVTimeStamp { get; set; }
    }
}