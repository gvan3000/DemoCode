using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    /// <summary>
    /// Propositions response model
    /// </summary>
    public class PropositionsResponseModel
    {
        /// <summary>
        /// list of available propositions 
        /// </summary>
        [JsonProperty("propositions")]
        public List<PropositionModel> Propositions { get; set; } = new List<PropositionModel>();
    }
}
