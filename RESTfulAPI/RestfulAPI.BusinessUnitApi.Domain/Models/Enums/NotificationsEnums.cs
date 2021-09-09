namespace RestfulAPI.BusinessUnitApi.Domain.Models.Enums
{
    public class NotificationsEnums
    {
        /// <summary>
        /// Delivery method enum
        /// </summary>
        public enum DeliveryMethod
        {
            Email = 0,
            SMS = 1,
            HTTP = 2
        }

        /// <summary>
        /// Delivery options type enum
        /// </summary>
        public enum DeliveryOptionsType
        {
            Basic = 0
        }

        /// <summary>
        /// Notification processes enum
        /// </summary>
        public enum NotificationType
        {
            /// <summary>
            /// General low balance notification
            /// </summary>
            LOWBALANCE = 1,

            /// <summary>
            /// General empty balance notification
            /// </summary>
            EMPTYBALANCE = 2,

            /// <summary>
            /// Topup success notification
            /// </summary>
            TOPUP = 3,

            /// <summary>
            /// Unknown / not applicable notification
            /// </summary>
            NOTDEFINED = 4,

            /// <summary>
            /// Low balance notificaton for voice service
            /// </summary>
            LOWBALANCE_VOICE = 5,

            /// <summary>
            /// Low balance notifiation for SMS service
            /// </summary>
            LOWBALANCE_SMS = 6,

            /// <summary>
            /// Low balance notification for data service
            /// </summary>
            LOWBALANCE_DATA = 7,

            /// <summary>
            /// Empty balance notification for vocie service
            /// </summary>
            EMPTYBALANCE_VOICE = 8,

            /// <summary>
            /// Empty balance notification for SMS service
            /// </summary>
            EMPTYBALANCE_SMS = 9,

            /// <summary>
            /// Empty balance service for data service
            /// </summary>
            EMPTYBALANCE_DATA = 10,

            /// <summary>
            /// Low balance on PCRF add-on
            /// </summary>
            LOWBALANCE_PCRF = 11,

            /// <summary>
            /// Low balance on data limit add-on
            /// </summary>
            LOWBALANCE_DATALIMIT = 12,

            /// <summary>
            /// low balance for roaming data add-on
            /// </summary>
            LOWBALANCE_ROAMINGDATA = 13,

            /// <summary>
            /// Low balance for roaming SMS add-on
            /// </summary>
            LOWBALANCE_ROAMINGSMS = 14,

            /// <summary>
            /// Low balance for roaming voice add-on
            /// </summary>
            LOWBALANCE_ROAMINGVOICE = 15,

            /// <summary>
            /// Low balance for base data add-on
            /// </summary>
            LOWBALANCE_BASEADDONDATA = 16,

            /// <summary>
            /// Low balance for base SMS add-on
            /// </summary>
            LOWBALANCE_BASEADDONSMS = 17,

            /// <summary>
            /// Low balance for base voice add-on
            /// </summary>
            LOWBALANCE_BASEADDONVOICE = 18,

            /// <summary>
            /// Low balance for base quota (general cache) add-on
            /// </summary>
            LOWBALANCE_BASEADDONQUOTA = 19,

            /// <summary>
            /// Empty balance for PCRF add-on
            /// </summary>
            EMPTYBALANCE_PCRF = 20,

            /// <summary>
            /// Empty balance for data limit add-on
            /// </summary>
            EMPTYBALANCE_DATALIMIT = 21,

            /// <summary>
            /// Empty balance for roaming data add-on
            /// </summary>
            EMPTYBALANCE_ROAMINGDATA = 22,

            /// <summary>
            /// Empty balance for roaming SMS add-on
            /// </summary>
            EMPTYBALANCE_ROAMINGSMS = 23,

            /// <summary>
            /// Empty balance for roaming voice add-on
            /// </summary>
            EMPTYBALANCE_ROAMINGVOICE = 24,

            /// <summary>
            /// Empty balance for base data add-on
            /// </summary>
            EMPTYBALANCE_BASEADDONDATA = 25,

            /// <summary>
            /// Empty balance for base SMS add-on
            /// </summary>
            EMPTYBALANCE_BASEADDONSMS = 26,

            /// <summary>
            /// Empty balance for base voice add-on
            /// </summary>
            EMPTYBALANCE_BASEADDONVOICE = 27,

            /// <summary>
            /// Empty balance for base quota (general cache) add-on
            /// </summary>
            EMPTYBALANCE_BASEADDONQUOTA = 28,

            /// <summary>
            /// Esim profile update
            /// </summary>
            ESIMPROFILE_UPDATED = 29,

            /// <summary>
            /// Non-chargeable notification Opt-out 
            /// </summary>
            NONCHARGEABLENOTIFICATION_OPTOUT = 30,
        }
    }
}
