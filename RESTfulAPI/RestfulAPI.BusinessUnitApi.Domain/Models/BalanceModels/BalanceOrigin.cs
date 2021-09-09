using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels
{
    public class BalanceOrigin
    {
        /// <summary>
        /// Id of add-on that produced this balance or <c>null</c> if it did not come from add-on
        /// </summary>
        [JsonProperty("add_on_id")]
        public Guid? AddOnId { get; set; }

        /// <summary>
        /// Id of commercial offer that produced this balance or <c>null</c> if it did not come from proposition commercial offer
        /// </summary>
        [JsonProperty("commercial_offer_id")]
        public Guid? CommercialOfferId { get; set; }

        /// <summary>
        /// ResourceId; To distinguish the different add-on's of the same type.
        /// </summary>
        [JsonProperty(PropertyName = "resource_id")]
        public int? AddOnResourceId { get; set; }
    }
}
