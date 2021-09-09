using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    /// <summary>
    /// ProductModel
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Status of the entity
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Iccid of the product
        /// </summary>
        [JsonProperty("iccid")]
        public string Iccid { get; set; }

        /// <summary>
        /// Msisdn list
        /// </summary>
        [JsonProperty("msisdns")]
        public string[] Msisdns { get; set; }

        /// <summary>
        /// Imsi list
        /// </summary>
        [JsonProperty("imsis")]
        public string[] Imsis { get; set; }

        /// <summary>
        /// Plan Id
        /// </summary>
        [JsonProperty("plan_id")]
        public Guid? PlanId { get; set; }

        /// <summary>
        /// PropositionId
        /// </summary>
        [JsonProperty("proposition_id")]
        public Guid? PropositionId { get; set; }

        /// <summary>
        /// Product Type Id
        /// </summary>
        [JsonProperty("product_type_id")]
        public Guid? ProductTypeId { get; set; }

        /// <summary>
        /// Id of the BusinessUnit
        /// </summary>
        [JsonProperty("business_unit_id")]
        public Guid BusinessUnitId { get; set; }

        /// <summary>
        /// Id of the person
        /// </summary>
        [JsonProperty("person_id")]
        public Guid PersonId { get; set; }

        /// <summary>
        /// start day for billing.
        /// </summary>
        [JsonProperty("billing_cycle_start_day")]
        public int? BillingCycleStartDay { get; set; }

        [JsonProperty("department_id")]
        public Guid? DepartmentAndCostCenterId { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonProperty("creation_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Activation date
        /// </summary>
        [JsonProperty("activation_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime? ActivationDate { get; set; }

        /// <summary>
        /// Deactivatio date
        /// </summary>
        [JsonProperty("deactivation_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// Last time product status was changed
        /// </summary>
        [JsonProperty("status_date", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime? LastStatusChangeDate { get; set; }

        /// <summary>
        /// Location to fetch the products details
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }
    }
}