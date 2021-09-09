using Newtonsoft.Json;
using System.Collections.Generic;

namespace OTAServices.Business.Entities.Helpers
{
    public static class Extensions
    {
        public static string ToJsonForLogging(this object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings).Replace("{", "{{").Replace("}", "}}"); ;
        }
    }
}
