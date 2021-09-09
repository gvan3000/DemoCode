using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Functions.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}
