using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Group;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Group
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupCreate")]
    public class CreateGroupRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "Name")]
        public string Name { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "NotificationPreference")]
        public int? NotificationPreference { get; set; }

        [MatrixxContractMember(Name = "GroupReAuthPreference")]
        public int? GroupReAuthPreference { get; set; }

        [MatrixxContractMember(Name = "AdminArray")]
        public GroupAdmin GroupAdmin { get; set; }
    }
}
