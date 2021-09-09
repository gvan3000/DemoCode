using Newtonsoft.Json;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    /// <summary>
    /// BusinessUnit model
    /// </summary>
    public class BusinessUnitModel
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the entity
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// CustomerId
        /// </summary>
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        /// <summary>
        /// ParentId of the entity
        /// </summary>
        [JsonProperty("parent_id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// PersonId
        /// </summary>
        [JsonProperty("person_id")]
        public Guid? PersonId { get; set; }

        /// <summary>
        /// List of plans linked to the business unit
        /// </summary>
        [JsonProperty("plans")]
        public List<Guid?> PlanIds { get; set; }

        /// <summary>
        /// Is entity SharedWallet
        /// </summary>
        [JsonProperty("has_shared_wallet")]
        public bool HasSharedWallet { get; set; }
        
        /// <summary>
        /// WholesalePriceplan
        /// </summary>
        [JsonProperty("wholesale_priceplan")]
        public string WholesalePricePlan { get; set; }

        /// <summary>
        /// Billing cycle start day
        /// </summary>
        [JsonProperty("billing_cycle_start_day")]
        public int? BillCycleStartDay { get; set; }

        /// <summary>
        /// List of propositions
        /// </summary>
        [JsonProperty("propositions")]
        public List<Proposition> Propositions { get; set; }

        /// <summary>
        /// List of AddOns
        /// </summary>
        [JsonProperty("add_ons")]
        public List<AddOn> AddOns { get; set; }

        // this will remove property from contract if set to null but will serialize empty array if empty list is used
        /// <summary>
        /// List of children 
        /// </summary>
        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
        public List<BusinessUnitModel> Children { get; set; }

        /// <summary>
        /// RateKey
        /// </summary>
        [JsonIgnore]
        public int RateKey { get; set; }
    }
}


