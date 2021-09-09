using Microsoft.Practices.Unity;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Business.Interfaces;

namespace OCSServices.Matrixx.Agent
{
    public static class AgentConfiguration
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IAgent, Agent>(new HierarchicalLifetimeManager());
            container.RegisterType<IMessageBuilderUnitOfWork, MessageBuilderUnitOfWork>();
            container.RegisterType<Api.Client.ApiClient.IClient, Api.Client.ApiClient.Client>(new HierarchicalLifetimeManager());
        }
    }
}
