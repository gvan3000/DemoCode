using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications
{
    public class CreateNotificationTranslator : ITranslate<CreateNotificationModel, CreateBusinessUnitNotificationConfigurationContract>
    {
        private readonly INotificationTypeTranslator _notificationTypeTranslator;

        public CreateNotificationTranslator(INotificationTypeTranslator notificationTypeTranslator)
        {
            _notificationTypeTranslator = notificationTypeTranslator;
        }

        public CreateBusinessUnitNotificationConfigurationContract Translate(CreateNotificationModel input)
        {
            if (input == null)
                return null;

            var translated = new CreateBusinessUnitNotificationConfigurationContract()
            {
                ProcessId = _notificationTypeTranslator.Translate(input.Type.GetValueOrDefault()),
                Deliveries = input?.Deliveries
                    ?.Select(x => new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                    {
                        DeliveryMethod = (DeliveryMethodEnum)x.DeliveryMethod,
                        DeliveryValue = x.DeliveryValue,
                        DeliveryOptions = x.DeliveryOptions != null ? new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                        {
                            Type = (DeliveryOptionsEnumDeliveryOptionsType)x.DeliveryOptions.Type,
                            Username = x.DeliveryOptions.Username,
                            Password = x.DeliveryOptions.Password
                        }
                        : null
                    })
                    .ToArray()
            };

            return translated;
        }

        
    }
}
