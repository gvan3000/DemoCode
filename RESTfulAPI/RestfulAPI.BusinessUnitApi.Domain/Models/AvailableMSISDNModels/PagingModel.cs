using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels
{
    /// <summary>
    /// PagingModel
    /// </summary>
    public class PagingModel
    {
        /// <summary>
        /// The MSISDN
        /// </summary>
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }
}
