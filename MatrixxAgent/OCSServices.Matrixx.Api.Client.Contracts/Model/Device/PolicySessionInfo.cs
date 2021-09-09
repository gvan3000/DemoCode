using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class PolicySessionInfo : MatrixxObject
    {
        /// <summary>
        /// Id of the session
        /// </summary>
        [MatrixxContractMember(Name = "SessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Time session was last updated
        /// </summary>
        [MatrixxContractMember(Name = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Start time of the entire session
        /// </summary>
        [MatrixxContractMember(Name = "SessionStartTime")]
        public DateTime? SessionStartTime { get; set; }

        /// <summary>
        /// The host which initiated the session
        /// </summary>
        [MatrixxContractMember(Name = "SourceHost")]
        public string SourceHost { get; set; }

        /// <summary>
        /// The realm which initiated the session
        /// </summary>
        [MatrixxContractMember(Name = "SourceRealm")]
        public string SourceRealm { get; set; }

        /// <summary>
        /// Type of the policy session: 1 => Sy, 2 => Gx.
        /// </summary>
        [MatrixxContractMember(Name = "SessionType")]
        public long SessionType { get; set; }

        /// <summary>
        /// Name of the policy profile
        /// </summary>
        [MatrixxContractMember(Name = "PolicyProfileName")]
        public string PolicyProfileName { get; set; }

        /// <summary>
        /// Custom extension attributes
        /// </summary>
        [MatrixxContractMember(Name = "Attr")]
        public PolicySessionCustomExtension Attr { get; set; }
    }
}
