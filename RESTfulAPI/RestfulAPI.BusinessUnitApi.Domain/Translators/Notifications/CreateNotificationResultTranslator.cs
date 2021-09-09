using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications
{
    public class CreateNotificationResultTranslator : ITranslate<CreateBusinessUnitNotificationConfigurationResult, CreateNotificationModelResponse>
    {
        private readonly ICustomAppConfiguration _customAppConfiguration;

        public CreateNotificationResultTranslator(ICustomAppConfiguration customAppConfiguration)
        {
            _customAppConfiguration = customAppConfiguration;
        }

        public CreateNotificationModelResponse Translate(CreateBusinessUnitNotificationConfigurationResult input)
        {
            if (input == null)
                return null;

            var hostName = _customAppConfiguration.GetConfigurationValue(ConfigurationConstants.RestfulApiDomainNameSection, ConfigurationConstants.RestAPIDomainName);
            var businessUnitApiPath = _customAppConfiguration.GetConfigurationValue(ConfigurationConstants.RestfullApiPathSection, ConfigurationConstants.BusinessUnitApi);

            var result = new CreateNotificationModelResponse()
            {
                Id = input.NotificationId,
                Location = $"{hostName}/{businessUnitApiPath}/business-units/{input.AccountId}/notifications" // no endpoint for now for single notification
            };

            return result;
        }
    }
}
