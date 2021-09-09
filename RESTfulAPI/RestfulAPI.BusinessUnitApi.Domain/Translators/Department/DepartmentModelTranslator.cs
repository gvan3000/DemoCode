using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Department
{
    /// <summary>
    /// Department Response Data Model Translator
    /// </summary>
    public class DepartmentModelTranslator : ITranslate<DepartmentCostCenterContract, CreateDepartmentResponseModel>
    {
        private readonly ICustomAppConfiguration _customAppConfiguration;

        public DepartmentModelTranslator(ICustomAppConfiguration customAppConfiguration)
        {
            _customAppConfiguration = customAppConfiguration;
        }

        /// <summary>
        /// Translates Department response contract to CreateDepartmentResponseModel
        /// </summary>
        /// <param name="input">DepartmentCostCenterContract</param>
        /// <returns><see cref="CreateDepartmentResponseModel"/></returns>
        public CreateDepartmentResponseModel Translate(DepartmentCostCenterContract input)
        {
            if (input == null)
            {
                return null;
            }

            var hostName = _customAppConfiguration.GetConfigurationValue(ConfigurationConstants.RestfulApiDomainNameSection, ConfigurationConstants.RestAPIDomainName);
            var businessUnitApiPath = _customAppConfiguration.GetConfigurationValue(ConfigurationConstants.RestfullApiPathSection, ConfigurationConstants.BusinessUnitApi);

            var result = new CreateDepartmentResponseModel
            {
                Id = input.Id,
                Location = $"{hostName}/{businessUnitApiPath}/business-units/{input.AccountId}/departments"
            };

            return result;
        }
    }
}
