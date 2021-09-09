using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrixx.Agent.Base.Interfaces;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.AgentFactory
{
    public interface IAgentFactory
    {
        IAgent Get(Endpoint endpoint);
    }
}
