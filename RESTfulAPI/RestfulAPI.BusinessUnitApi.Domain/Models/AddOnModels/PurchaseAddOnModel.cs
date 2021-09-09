using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels
{
    /// <summary>
    /// Purchase AddOn Model
    /// </summary>
    public class PurchaseAddOnModel
    {
        /// <summary>
        /// Id of add-on
        /// </summary>
        [JsonProperty("add_on_id")]
        [Required]
        public Guid AddOnId { get; set; }
    }
}
