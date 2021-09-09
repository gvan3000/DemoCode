using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    /// <summary>
    /// Result of Business Unit creation request
    /// </summary>
    public class CreateBusinessUnitResponseModel
    {
        /// <summary>
        /// Id of the business unit
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// URL for the business unit
        /// </summary>
        [Url]
        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
