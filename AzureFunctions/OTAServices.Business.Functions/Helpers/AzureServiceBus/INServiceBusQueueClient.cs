
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Helpers
{
    public interface INServiceBusQueueClient
    {
        Task SendToNserviceBus(string msg, string messageType);
    }
}
