using OCSServices.Matrixx.Api.Client.Contracts.Base;
using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber
{
    [MatrixxContract(Name = "MtxResponseSubscriber")]
    public class SubscriberQueryResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public Guid ExternalId { get; set; }

        [MatrixxContractMember(Name = "DeviceIdArray")]
        public StringValueCollection DeviceList { get; set; }

        [MatrixxContractMember(Name = "Status")]
        public int Status { get; set; }

        [MatrixxContractMember(Name = "StatusDescription")]
        public string StatusDescription { get; set; }

        [MatrixxContractMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [MatrixxContractMember(Name = "LastName")]
        public string LastName { get; set; }

        [MatrixxContractMember(Name = "ContactEmail")]
        public string ContactEmail { get; set; }

        [MatrixxContractMember(Name = "ContactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }

        [MatrixxContractMember(Name = "NotificationPreference")]
        public int NotificationPreference { get; set; }

        [MatrixxContractMember(Name = "TimeZone")]
        public string TimeZone { get; set; }

        [MatrixxContractMember(Name = "ParentGroupIdArray")]
        public StringValueCollection ParentGroupList { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public CustomSubscriberConfigurationCollection CustomSubscriberConfigurationList { get; set; }

        [MatrixxContractMember(Name = "PurchasedOfferArray")]
        public PurchasedOfferInfoCollection PurchaseInfoList { get; set; }

        [MatrixxContractMember(Name = "BalanceArray")]
        public BalanceInfoCollection BalanceInfoList { get; set; }

        [MatrixxContractMember(Name = "AdminGroupIdArray")]
        public StringValueCollection AdminGroupList { get; set; }

        [MatrixxContractMember(Name = "AdminGroupCount")]
        public int AdminGroupCount { get; set; }

        [MatrixxContractMember(Name = "AdminGroupCursor")]
        public int AdminGroupCursor { get; set; }

        [MatrixxContractMember(Name = "ParentGroupCount")]
        public int ParentGroupCount { get; set; }

        [MatrixxContractMember(Name = "ParentGroupCursor")]
        public int ParentGroupCursor { get; set; }

        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }
    }
}
