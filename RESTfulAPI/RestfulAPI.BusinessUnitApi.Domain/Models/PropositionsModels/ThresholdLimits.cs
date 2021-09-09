using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels
{
    public class ThresholdLimits
    {
        [JsonProperty("limit")]
        public decimal Limit { get; set; }
    }
}
