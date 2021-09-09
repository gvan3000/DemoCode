
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Helpers
{
    public interface IQueueClient
    {
        Task SendToQueue(string msg);
    }
}
