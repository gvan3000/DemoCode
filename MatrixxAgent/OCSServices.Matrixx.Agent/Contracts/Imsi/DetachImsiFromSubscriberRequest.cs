using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Imsi
{
    public class DetachImsiFromSubscriberRequest
    {
        public Guid CrmProductId { get; set; }
        public List<string> Imsis { get; set; }
    }
}
