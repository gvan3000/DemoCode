using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface IDeleteIMSICallbackRepository
    {
        void Add(DeleteIMSICallback campaign);
        void Update(DeleteIMSICallback campaign);
        DeleteIMSICallback GetByImsiAndOasisRequestId(string imsi, int oasisRequestId);
    }
}
