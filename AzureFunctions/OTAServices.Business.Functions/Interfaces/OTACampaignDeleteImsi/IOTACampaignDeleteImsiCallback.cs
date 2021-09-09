using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi
{
    public interface IOTACampaignDeleteImsiCallback
    {
        Task DeleteImsi(OasisCallback callback, string subscribersListId, string imsi);
    }
}
