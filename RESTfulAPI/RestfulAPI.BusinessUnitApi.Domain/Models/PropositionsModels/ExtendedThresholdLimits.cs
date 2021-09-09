using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    public class ExtendedThresholdLimits
    {
        [JsonProperty("limit")]
        public decimal Limit { get; set; }

        [JsonProperty("meter_type")]
        public string MeterTypeName { get; set; }

        [JsonProperty("unit_type")]
        public string DataTypeUnitName { get; set; }
    }
}
