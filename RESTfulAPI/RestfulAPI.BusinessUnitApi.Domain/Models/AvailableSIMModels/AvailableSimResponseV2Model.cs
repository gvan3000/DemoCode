using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels
{
	public class AvailableSimResponseV2Model
	{
		[JsonProperty("sims")]
		public IEnumerable<Sim> Sims { get; set; }

		[JsonProperty("paging")]
		public Paging Paging { get; set; }
	}

	public class Sim
	{
		[JsonProperty("iccid")]
		public string Iccid { get; set; }

        [JsonProperty("eid")]
        public string EID { get; set; }

        [JsonProperty("status")]
		public string Status { get; set; }
	}

	public class Paging
	{
		[JsonProperty("total_results")]
		public int TotalResults { get; set; }
	}
}
