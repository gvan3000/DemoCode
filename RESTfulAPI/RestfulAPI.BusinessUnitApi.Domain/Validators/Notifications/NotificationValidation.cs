using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Constants;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.NotificationsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications
{
    public class NotificationValidation
    {
        private const string EMAIL_VALIDATION_ERROR = "The email is not in valid form";
        private const string SMS_VALIDATION_ERROR = "The sms delivery value is not in the valid form";
        private const string URL_VALIDATION_ERROR = "Url is not in the valid form";

        protected bool ValidateDeliveryValue(DeliveryMethod deliveryMethod, string deliveryValue)
        {
            bool result;
            switch (deliveryMethod)
            {
                case DeliveryMethod.Email:
                    result = ValidateEmail(deliveryValue);
                    break;
                case DeliveryMethod.SMS:
                    result = ValidateSms(deliveryValue);
                    break;
                case DeliveryMethod.HTTP:
                    result = ValidateUrl(deliveryValue);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        protected bool ValidateEmail(string email)
        {
            bool isValid = Regex.IsMatch(email, RegexValidationConstants.EmailAddressFormat, RegexOptions.IgnoreCase);

            return isValid;
        }

        protected bool ValidateUrl(string url)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        protected string ProvideDeliveryValueValidationError(DeliveryMethod deliveryMethod)
        {
            string validationError = string.Empty;
            switch (deliveryMethod)
            {
                case DeliveryMethod.Email:
                    validationError = EMAIL_VALIDATION_ERROR;
                    break;
                case DeliveryMethod.SMS:
                    validationError = SMS_VALIDATION_ERROR;
                    break;
                case DeliveryMethod.HTTP:
                    validationError = URL_VALIDATION_ERROR;
                    break;
                default:
                    break;
            }

            return validationError;
        }

        protected bool ValidateSms(string sms)
        {
            Regex regex = new Regex(RegexValidationConstants.ValidActiveMsisdn);

            return regex.IsMatch(sms);
        }

        protected string ValidDeliveryOptionForHttp(NotificationsEnums.DeliveryMethod deliveryMethod, DeliveryOption deliveryOptions)
        {
            if (deliveryMethod != DeliveryMethod.HTTP && deliveryOptions != null)
            {
                return $"Unsupported, can only be specified for HTTP delivery method";
            }

            if (deliveryOptions != null && deliveryOptions.Type != DeliveryOptionsType.Basic)
            {
                return $"Only supported option is BASIC";
            }

            return string.Empty;
        }

        protected string ValidateOneDeliveryMethodPerEventType(List<Delivery> deliveries)
        {
            string errorMessage = deliveries.GroupBy(x => x.DeliveryMethod).All(x => x.Count() <= 1)
                                    ? string.Empty
                                    : $"Only one type of delivery per event type is allowed (one sms, one email and / or one http)";
            return errorMessage;
        }

        protected string ValidateUsernameAndPasswordRequired(List<Delivery> deliveries)
        {
            IEnumerable<DeliveryOption> deliveryOptions = deliveries?.Select(x => x.DeliveryOptions).Where(x => x != null);
            if (deliveryOptions != null)
            {
                if (deliveryOptions.Any(x => x.Type == DeliveryOptionsType.Basic
                    && string.IsNullOrWhiteSpace(x.Username)))
                {
                    return $"{nameof(DeliveryOption.Username)} is required when delivery option type is BASIC";
                }

                if (deliveryOptions.Any(x => x.Type == DeliveryOptionsType.Basic
                    && string.IsNullOrWhiteSpace(x.Password)))
                {
                    return $"{nameof(DeliveryOption.Password)} is required when delivery option type is BASIC";
                }              
            }

            return string.Empty;
        }
    }
}
