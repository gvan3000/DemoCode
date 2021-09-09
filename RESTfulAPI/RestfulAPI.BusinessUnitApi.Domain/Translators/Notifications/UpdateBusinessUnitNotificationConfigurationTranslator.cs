using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.Translators;
using contract = RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.NotificationTranslators
{
    public class UpdateBusinessUnitNotificationConfigurationTranslator : ITranslate<UpdateNotificationModel, contract.UpdateBusinessUnitNotificationConfigurationContract>
    {
        private readonly INotificationTypeTranslator _notificationTypeTranslator;

        public UpdateBusinessUnitNotificationConfigurationTranslator(INotificationTypeTranslator notificationTypeTranslator)
        {
            _notificationTypeTranslator = notificationTypeTranslator;
        }

        public contract.UpdateBusinessUnitNotificationConfigurationContract Translate(UpdateNotificationModel input)
        {
            if (input == null)
                return null;

            var translated = new contract.UpdateBusinessUnitNotificationConfigurationContract()
            {
                Type = _notificationTypeTranslator.Translate(input.Type.GetValueOrDefault()),
                BusinessUnitId = input.BusinessUnitId,
                Deliveries = input.Deliveries?.Select(x => new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                {
                    DeliveryMethod = (contract.DeliveryMethodEnum)x.DeliveryMethod,
                    DeliveryValue = x.DeliveryValue,
                    DeliveryOptions = TranslateDeliveryOptions(x.DeliveryOptions)
                })
                .ToArray()
            };

            return translated;
        }

        private contract.DeliveryOption TranslateDeliveryOptions(DeliveryOption options)
        {
            if (options == null)
            {
                return null;
            }
            contract.DeliveryOption contractDeliveryOption = new contract.DeliveryOption()
            {
                Password = options.Password,
                Type = (contract.DeliveryOptionsEnumDeliveryOptionsType)options.Type,
                Username = options.Username
            };

            return contractDeliveryOption;
        }
    }
}
