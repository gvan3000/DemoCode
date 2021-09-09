using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Subscriber
{
    public class SetSubscriberFirstNameRequest
    {
        public Guid CrmProductId { get; set; }
        public string FirstName { get; set; }
    }
}
