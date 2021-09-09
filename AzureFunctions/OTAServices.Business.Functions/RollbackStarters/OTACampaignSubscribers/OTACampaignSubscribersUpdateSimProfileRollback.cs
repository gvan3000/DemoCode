using OTAServices.Business.Entities.SimManagement;
using System.Collections.Generic;

namespace OTAServices.Business.Functions.RollbackStarters.OTACampaignSubscribers
{
    public class OTACampaignSubscribersUpdateSimProfileRollback
    {
        public OTACampaignSubscribersUpdateSimProfileRollback(string fileName,  List<UiccidSimProfileId> uiccidSimProfileIds)
        {
            FileName = fileName;
            UiccidSimProfileIds = uiccidSimProfileIds;
        }

        public List<UiccidSimProfileId> UiccidSimProfileIds { get; private set; }

        public string FileName { get; private set; }
    }
}
