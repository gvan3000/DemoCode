using OTAServices.Business.Entities.SimManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface ISimOrderLineRepository
    {
        void SetSimOrderLineSimProfileIdBatch(List<UiccidSimProfileId> data);
        List<UiccidSimProfileId> GetSimProfileByUiccidBatch(List<string> uiccids);
    }
}
