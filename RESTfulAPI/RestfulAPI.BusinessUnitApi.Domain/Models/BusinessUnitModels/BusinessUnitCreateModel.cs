using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    public class BusinessUnitCreateModel
    {
        [JsonProperty("name")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [JsonProperty("parent_id")]
        public Guid? ParentId { get; set; }

        [JsonProperty("customer_id")]
        [StringLength(100)]
        public string CustomerId { get; set; }

        [JsonProperty("person_id")]
        [Required]
        public Guid? PersonId { get; set; } //needs to be nullable in order to display proper validation message

        [JsonProperty("wholesale_priceplan")]
        public string WholesalePriceplan { get; set; }

        [JsonProperty("propositions")]
        public Proposition[] Propositions { get; set; }

        [JsonProperty("add_ons")]
        public List<Guid> AddOns { get; set; }

        [JsonProperty("billing_cycle_start_day")]
        [Range(1, int.MaxValue, ErrorMessage = "The field BillingCycleStartDay must be greater than 0.")]
        public int? BillingCycleStartDay { get; set; }

        [JsonProperty("oa_based_sms_routing")]
        public bool? UseOABasedSmsRouting { get; set; }

        [JsonProperty("is_using_plans")]
        public bool? IsUsingPlans { get; set; }

    }
}


