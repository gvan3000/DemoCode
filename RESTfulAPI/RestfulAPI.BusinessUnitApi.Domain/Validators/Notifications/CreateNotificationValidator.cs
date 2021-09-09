using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications
{
    public class CreateNotificationValidator : NotificationValidation, IValidateModel<CreateNotificationModel, CreateNotificationModelResponse>
    {
        public ProviderOperationResult<CreateNotificationModelResponse> ValidateModel(CreateNotificationModel input)
        {
            string validationErrorMessage = ValidateOneDeliveryMethodPerEventType(input.Deliveries);
            if (!string.IsNullOrEmpty(validationErrorMessage))
                return ProviderOperationResult<CreateNotificationModelResponse>.InvalidInput(nameof(input.Deliveries), validationErrorMessage);

            validationErrorMessage = ValidateUsernameAndPasswordRequired(input.Deliveries);
            if (!string.IsNullOrEmpty(validationErrorMessage))
                return ProviderOperationResult<CreateNotificationModelResponse>.InvalidInput(nameof(Delivery.DeliveryOptions), validationErrorMessage);

            foreach (var delivery in input.Deliveries)
            {
                if (delivery.DeliveryOptions != null)
                {
                    validationErrorMessage = ValidDeliveryOptionForHttp(delivery.DeliveryMethod, delivery.DeliveryOptions);
                    if (!string.IsNullOrEmpty(validationErrorMessage))
                        return ProviderOperationResult<CreateNotificationModelResponse>.InvalidInput(nameof(Delivery.DeliveryOptions), validationErrorMessage); ;
                }

                if (delivery.DeliveryMethod == NotificationsEnums.DeliveryMethod.HTTP && !ValidateUrl(delivery.DeliveryValue))
                {
                    return ProviderOperationResult<CreateNotificationModelResponse>.InvalidInput(nameof(Delivery.DeliveryValue), ProvideDeliveryValueValidationError(delivery.DeliveryMethod));
                }
            }

            return ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse());
        }
    }
}
