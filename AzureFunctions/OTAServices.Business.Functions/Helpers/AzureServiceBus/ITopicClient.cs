
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Helpers
{
    public interface ITopicClient
    {
        Task SendToTopic(string msg);
    }
}
