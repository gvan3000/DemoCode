using System.Linq;
using Matrixx.Agent.Base.Interfaces;
using Matrixx.Agent.V4x;
using Matrixx.Agent.V5x;
using Microsoft.Practices.Unity;
using SplitProvisioning.Base.Data;
using Agent4 = Matrixx.Agent.V4x.Agent;
using Agent5 = Matrixx.Agent.V5x.Agent;

namespace OCSServices.Matrixx.AgentFactory
{
    public class AgentFactory : IAgentFactory
    {
        private IUnityContainer _container;

        public AgentFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IAgent Get(Endpoint endpoint)
        {
            var version = endpoint.EndpointAttributes?.Where(x => x.EndpointAttributeTypeId == (int)EndpointAttributeType.MatrixxVersion).FirstOrDefault()?.Value;

            switch (version)
            {
                case "4":
                    return _container.Resolve<Agent4>();
                case "5":
                    return _container.Resolve<Agent5>();
                default:
                    return null;
            }

        }
    }
}
