using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels
{
    public class PreferredLanguageModel
    {
        /// <summary>
        /// Preferred Language id
        /// </summary>
        [JsonProperty("language-id")]
        public Guid LanguageId { get; set; }

        /// <summary>
        /// Preferred Language name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Preferred Language is default
        /// </summary>
        [JsonProperty("is-default")]
        public bool IsDefault { get; set; }
    }
}
