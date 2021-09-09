using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels
{
    /// <summary>
    /// AddOn Cumulative list model
    /// </summary>
    public class AddOnCumulativeListModel
    {
        /// <summary>
        /// List of AddOnCumulative model
        /// </summary>
        [JsonProperty("add-ons")]
        public List<AddOnCumulativeModel> AddOns { get; set; }
    }
}
