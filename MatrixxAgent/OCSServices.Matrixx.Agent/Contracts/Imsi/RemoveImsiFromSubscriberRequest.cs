using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Imsi
{
    public class RemoveImsiFromSubscriberRequest
    {
        public Guid ProductId { get; set; }
        public string Imsi { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
