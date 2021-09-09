using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Balance;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Group
{
    [MatrixxContract(Name = "MtxResponseGroup")]
    public class GroupQueryResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "Name")]
        public string Name { get; set; }

        [MatrixxContractMember(Name = "Status")]
        public int Status { get; set; }

        [MatrixxContractMember(Name = "StatusDescription")]
        public string StatusDescription { get; set; }

        [MatrixxContractMember(Name = "SubscriberCount")]
        public int SubscriberCount { get; set; }

        [MatrixxContractMember(Name = "AdminCount")]
        public int AdminCount { get; set; }

        [MatrixxContractMember(Name = "AdminCursor")]
        public int AdminCursor { get; set; }

        [MatrixxContractMember(Name = "SubscriberMemberCount")]
        public int SubscriberMemberCount { get; set; }

        [MatrixxContractMember(Name = "SubscriberMemberCursor")]
        public long SubscriberMemberCursor { get; set; }

        [MatrixxContractMember(Name = "GroupMemberCount")]
        public int GroupMemberCount { get; set; }

        [MatrixxContractMember(Name = "GroupMemberCursor")]
        public int GroupMemberCursor { get; set; }

        [MatrixxContractMember(Name = "NotificationPreference")]
        public int NotificationPreference { get; set; }

        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }


        [MatrixxContractMember(Name = "BalanceArray")]
        public BalanceInfoCollection BalanceInfoList { get; set; }
    }
}
