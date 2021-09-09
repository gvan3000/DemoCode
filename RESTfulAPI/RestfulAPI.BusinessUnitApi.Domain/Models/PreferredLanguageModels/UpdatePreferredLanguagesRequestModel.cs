using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels
{
    public class UpdatePreferredLanguagesRequestModel
    {
        /// <summary>
        /// List of Preferred Languages for update
        /// </summary>
        [JsonProperty("allowed-languages")]
        public List<UpdatePreferredLanguageModel> PreferredLanguages { get; set; }
    }
}
