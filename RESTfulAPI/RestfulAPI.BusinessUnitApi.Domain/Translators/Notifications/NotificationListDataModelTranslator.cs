using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications
{
    public class NotificationListDataModelTranslator : ITranslate<GetNotificationConfigurationListResponse, GetNotificationListDataModel>
    {
        private readonly INotificationTypeTranslator _notificationTypeTranslator;

        public NotificationListDataModelTranslator(INotificationTypeTranslator notificationTypeTranslator)
        {
            _notificationTypeTranslator = notificationTypeTranslator;
        }

        public GetNotificationListDataModel Translate(GetNotificationConfigurationListResponse input)
        {
            var notificationListDataModel = new GetNotificationListDataModel
            {
                Notifications = new List<GetNotificationDataModel>()
            };

            notificationListDataModel.Notifications = input?.NotificationsConfiguration != null ?
                input.NotificationsConfiguration.Select(x => new GetNotificationDataModel
                {
                    Type = TranslateType(x.Type),
                    Deliveries = x.Deliveries != null ?
                        x.Deliveries.Select(d =>
                            new Models.NotificationModels.Delivery
                            {
                                DeliveryMethod = (NotificationsEnums.DeliveryMethod)d.DeliveryMethod,
                                DeliveryValue = d.DeliveryValue,
                                DeliveryOptions = Translate(d.DeliveryOptions)
                            }).ToList() : new List<Models.NotificationModels.Delivery>()

                }).ToList() : new List<GetNotificationDataModel>();

            return notificationListDataModel;
        }

        private Models.NotificationModels.DeliveryOption Translate(TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption input)
        {
            if (input == null)
            {
                return null;
            }

            var result = new Models.NotificationModels.DeliveryOption()
            {
                Username = input.Username,
                Password = input.Password,
                Type = (NotificationsEnums.DeliveryOptionsType)input.Type
            };

            return result;
        }

        private NotificationType TranslateType(string input)
        {
            return _notificationTypeTranslator.Translate(input);
        }
    }
}
