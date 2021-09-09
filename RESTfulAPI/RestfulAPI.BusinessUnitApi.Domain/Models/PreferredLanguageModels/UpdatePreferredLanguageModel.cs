using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels
{
    public class UpdatePreferredLanguageModel
    {
        /// <summary>
        /// Preferred Language id
        /// </summary>
        [JsonProperty("language-id")]
        public Guid LanguageId { get; set; }

        /// <summary>
        /// Preferred Language is default
        /// </summary>
        [JsonProperty("is-default")]
        public bool IsDefault { get; set; }
    }
}
