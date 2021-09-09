using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators
{
    public class SimStatusTranslator : ITranslate<string, Guid>
    {
        private readonly ISysCodeConstants _sysCodeConstants;


        public SimStatusTranslator(ISysCodeConstants sysCodeConstants)
        {
            _sysCodeConstants = sysCodeConstants;
        }

        public Guid Translate(string input)
        {
            var output = _sysCodeConstants.SimStatuses
                .Where(x => x.Key.Equals(input, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value;

            return output;
        }
    }
}
