using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels
{
    public class AvailableLanguagesResponseModel
    {
        [JsonProperty("available-languages")]
        public List<AvailableLanguageModel> AvailableLanguages { get; set; }
    }
}
