using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Functions.Helpers
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}
