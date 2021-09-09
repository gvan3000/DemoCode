using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{

    public class SessionContextInfo : MatrixxObject
    {
        /// <summary>
        /// Service context Id.  Used to look up context data within session
        /// </summary>
        [MatrixxContractMember(Name ="ServiceId")]
        public long ServiceId { get; set; }

        /// <summary>
        /// Quantity on which context is being rated.  Values are: 1 => Actual duration 2 => Active duration 3 => Monetary 4 => Total data 
        /// 5 => In data 6 => Out data 7 => Service specific
        /// </summary>
        [MatrixxContractMember(Name = "QuantityType")]
        public long QuantityType { get; set; }

        /// <summary>
        /// Unit for authorized and used quantities.  Values are: 0 => none 100 => seconds 101 => minutes 102 => hours 
        /// 103 => days 104 => weeks 200 => bytes 201 => kbytes 202 => mbytes 203 => gbytes
        /// </summary>
        [MatrixxContractMember(Name = "QuantityUnit")]
        public uint QuantityUnit { get; set; }

        /// <summary>
        /// Used quantity for this session
        /// </summary>
        [MatrixxContractMember(Name = "UsedQuantity")]
        public decimal UsedQuantity { get; set; }

        /// <summary>
        /// Total authorized quantity for this session
        /// </summary>
        [MatrixxContractMember(Name = "AuthQuantity")]
        public decimal AuthQuantity { get; set; }

        /// <summary>
        /// Time session context was last updated
        /// </summary>
        [MatrixxContractMember(Name = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }
    }
}
