using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications
{
    public class UpdateNotificationValidation : NotificationValidation, IValidateModel<UpdateNotificationModel, object>
    {
        public ProviderOperationResult<object> ValidateModel(UpdateNotificationModel input)
        {
            string validationErrorMessage = ValidateOneDeliveryMethodPerEventType(input.Deliveries);
            if (!string.IsNullOrEmpty(validationErrorMessage))
                return ProviderOperationResult<object>.InvalidInput(nameof(input.Deliveries), validationErrorMessage);

            validationErrorMessage = ValidateUsernameAndPasswordRequired(input.Deliveries);
            if (!string.IsNullOrEmpty(validationErrorMessage))
                return ProviderOperationResult<object>.InvalidInput(nameof(Delivery.DeliveryOptions), validationErrorMessage);

            foreach (var delivery in input.Deliveries)
            {
                if (delivery.DeliveryOptions != null)
                {
                    validationErrorMessage = ValidDeliveryOptionForHttp(delivery.DeliveryMethod, delivery.DeliveryOptions);
                    if (!string.IsNullOrEmpty(validationErrorMessage))
                        return ProviderOperationResult<object>.InvalidInput(nameof(Delivery.DeliveryOptions), validationErrorMessage); ;
                }

                if (delivery.DeliveryMethod == NotificationsEnums.DeliveryMethod.HTTP && !ValidateUrl(delivery.DeliveryValue))
                {
                    return ProviderOperationResult<object>.InvalidInput(nameof(Delivery.DeliveryValue), ProvideDeliveryValueValidationError(delivery.DeliveryMethod));
                }              
            }

            return ProviderOperationResult<object>.OkResult(new object());
        }        
    }    
}
