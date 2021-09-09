using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    /// <summary>
    /// BuisinesUnit addons and propositions model
    /// </summary>
    public class BusinessUnitExtraInfoModel
    {
        /// <summary>
        /// Id of th Account
        /// </summary>
        public Guid BusinessUnitId { get; set; }
        /// <summary>
        /// List of Business Units Propositions
        /// </summary>
        public List<AllowedPropositionContract> Propositions { get; set; }
        /// <summary>
        /// List of Busines Units AddOns
        /// </summary>
        public List<SimpleAddOnContract> AddOns { get; set; }
    }
}
