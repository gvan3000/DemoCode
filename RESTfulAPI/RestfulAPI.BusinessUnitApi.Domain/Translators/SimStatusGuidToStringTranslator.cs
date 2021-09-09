using System;
using System.Linq;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators
{
	public class SimStatusGuidToStringTranslator : ITranslate<Guid, SimStatusWrapper>
	{
		private readonly ISysCodeConstants _sysCodeConstants;


		public SimStatusGuidToStringTranslator(ISysCodeConstants sysCodeConstants)
		{
			_sysCodeConstants = sysCodeConstants;
		}

		public SimStatusWrapper Translate(Guid input)
		{
			return new SimStatusWrapper { Status = _sysCodeConstants.SimStatuses.FirstOrDefault(k => k.Value.Equals(input)).Key };
		}
	}

	public class SimStatusWrapper
	{
		public string Status { get; set; }
	}
}
