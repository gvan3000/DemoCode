using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Device
{
    public class DeviceSessionResponse : BasicResponse
    {
        /// <summary>
        /// Total authorized quantity for this session
        /// </summary>
        public decimal AuthQuantity { get; set; }

        /// <summary>
        /// Unit for authorized and used quantities. Values are: 0 => none 100 => seconds 101 => minutes 102 => hours 
        /// 103 => days 104 => weeks 200 => bytes 201 => kbytes 202 => mbytes 203 => gbytes
        /// </summary>
        public uint QuantityUnit { get; set; }

        /// <summary>
        /// Start time of the entire session
        /// </summary>
        public DateTime? SessionStartTime { get; set; }
    }
}
