using Newtonsoft.Json;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels
{
    /// <summary>
    /// ProductType Model
    /// </summary>
    public class ProductTypeModel
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
    }
}
