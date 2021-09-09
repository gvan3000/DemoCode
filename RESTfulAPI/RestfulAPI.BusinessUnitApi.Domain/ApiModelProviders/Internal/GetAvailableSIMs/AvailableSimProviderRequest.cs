using System;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs
{
	public class AvailableSimProviderRequest
	{
		public Guid AccountId { get; set; }
		public string Status { get; set; }
		public int? PerPage { get; set; }
		public int? Page { get; set; }
	}
}