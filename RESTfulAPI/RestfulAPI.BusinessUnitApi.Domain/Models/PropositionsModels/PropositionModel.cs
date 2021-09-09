using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    /// <summary>
    /// Proposition model
    /// </summary>
    [JsonObject("proposition")]
    public class PropositionModel
    {
        /// <summary>
        /// Id of the proposition
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the proposition
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Unique code of the proposition
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// If true proposition can e used in created product
        /// </summary>
        [JsonProperty("is_available_for_product_creation")]
        public bool IsAvailableForProductCreation { get; set; }

        /// <summary>
        /// Unique code of the product type
        /// </summary>
        [JsonProperty("product_type_code")]
        public string ProductTypeCode { get; set; }

        /// <summary>
        /// Unique code for the commercial offer proposition
        /// </summary>
        [JsonProperty("co_proposition_code")]
        public string CoPropositionCode { get; set; }

        /// <summary>
        /// TestCommercialOfferPropositionCode
        /// </summary>
        [JsonProperty("test_co_proposition_code")]
        public string TestComPropositionCode { get; set; }

        /// <summary>
        /// Balance profile code
        /// </summary>
        [JsonProperty("balance_profile_code")]
        public string BalanceProfileCode { get; set; }

        /// <summary>
        /// Unique code for the product life cycle
        /// </summary>
        [JsonProperty("product_life_cycle_code")]
        public string ProductLifeCycleCode { get; set; }

        /// <summary>
        /// List of all commercial offer configuration for this proposition
        /// </summary>
        [JsonProperty("co_configurations")]
        public List<CommercialOfferConfig> CoConfigurations { get; set; }
    }
}
