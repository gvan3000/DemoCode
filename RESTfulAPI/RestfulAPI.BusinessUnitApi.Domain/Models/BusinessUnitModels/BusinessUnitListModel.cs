using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    /// <summary>
    /// BusinesUnitlistModel
    /// </summary>
    public class BusinessUnitListModel
    {
        /// <summary>
        /// List of BusinessUnits
        /// </summary>
        [JsonProperty("business_units")]
        public List<BusinessUnitModel> BusinessUnits { get; set; }
    }
}
