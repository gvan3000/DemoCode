using System;
using System.Collections.Generic;
using System.Text;

namespace OTAServices.Business.Entities.SimManagement
{
    public class SimInfo
    {
        public string Uiccid { get; set; }
        public Guid SimStatus { get; set; }
        public string SimType { get; set; }
        public Guid? ProductStatus { get; set; }
    }
}
