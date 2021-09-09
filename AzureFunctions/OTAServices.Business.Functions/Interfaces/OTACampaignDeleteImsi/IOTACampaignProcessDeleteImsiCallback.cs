using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi
{
    public interface IOTACampaignProcessDeleteImsiCallback
    {
        Task UpdateDeleteImsiCallback(DeleteImsiCallbackResult callback);
    }
}