using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels
{
    /// <summary>
    /// Available Msisdns response model
    /// </summary>
    public class AvailableMSISDNResponseModel
    {
        /// <summary>
        /// List of msisdns
        /// </summary>
        [JsonProperty("msisdns")]
        public List<MsisdnModel> Msisdns { get; set; }

        /// <summary>
        /// Paging
        /// </summary>
        [JsonProperty("paging")]
        public PagingModel Paging { get; set; }
    }
}
