using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance
{
    /// <summary>
    /// Proposition Info model
    /// </summary>
    public class PropositionInfoModel
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        public  Guid PropositionId { get; set; }

        /// <summary>
        /// EndUserSubscription
        /// </summary>
        public bool? EndUserSubscription { get; set; }

        /// <summary>
        /// CommercialOfferPropositionCode
        /// </summary>
        public string CommercialOfferPropositionCode { get; set; }

        /// <summary>
        /// QuotaOfferCode
        /// </summary>
        public string QuotaOfferCode { get; set; }

        /// <summary>
        /// CommercialOfferDefintions
        /// </summary>
        public List<CommercialOfferDefinition> CommercialOfferDefinitions { get; set; }
    }

    /// <summary>
    /// CommercialOfferDefinition
    /// </summary>
    public class CommercialOfferDefinition
    {
        /// <summary>
        /// SubscriptionTypeCode
        /// </summary>
        public string SubscriptionTypeCode { get; set; }

        /// <summary>
        /// ServiceTypeCode
        /// </summary>
        public string ServiceTypeCode { get; set; }

        /// <summary>
        /// CommercialOfferDefinitionCode
        /// </summary>
        public string CommercialOfferDefinitionCode { get; set; }        

    }
}
