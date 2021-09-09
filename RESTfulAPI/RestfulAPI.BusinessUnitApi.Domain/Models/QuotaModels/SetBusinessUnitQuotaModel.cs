using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels
{
    public class SetBusinessUnitQuotaModel
    {
        [JsonProperty("amount")]
        [Range(0, 100000)]
        public double Amount { get; set; }
    }
}
