using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    public class Metering
    {
        [JsonProperty("meter_type")]
        public string MeterType { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("unit_type")]
        public string UnitType { get; set; }

        [JsonProperty("zones")]
        public List<int> Zones { get; set; }

        [JsonProperty("treshold_limits")]
        public List<ThresholdLimits> ThresholdLimits { get; set; }
    }
}
