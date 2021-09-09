using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Entities.SimManagement
{
    public class SimContent
    {
        public string Uiccid { get; set; }
        public string ImsiSponsorPrefix { get; set; }
        public bool IsLeasedForOngoingCampaign { get; set; }
    }
}
