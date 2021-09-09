using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels
{
    public class AvailableLanguageModel
    {
        /// <summary>
        /// Available Language Id
        /// </summary>
        [JsonProperty("language-id")]
        public Guid LanguageId { get; set; }

        /// <summary>
        /// Available Language Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
