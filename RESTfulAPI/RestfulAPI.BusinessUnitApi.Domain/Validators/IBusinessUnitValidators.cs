using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Validators.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators
{
    /// <summary>
    /// Collection of model validators used for business unit domain
    /// </summary>
    public interface IBusinessUnitValidators
    {
        /// <summary>
        /// Validates update business unit model
        /// </summary>
        IUpdateBusinessUnitApnsValidator UpdateBusinessUnitApnsValidator { get; }

        /// <summary>
        /// Validate provided apn name against apns linked to the business unit
        /// </summary>
        IDeleteApnValidator DeleteApnValidator { get; }

        /// <summary>
        /// validate set default apn for business unit
        /// </summary>
        IUpdateBusinessUnitDefaultApnValidator UpdateBusinessUnitDefaultApnValidator { get; }

        /// <summary>
        /// validate update preferred languages for business unit
        /// </summary>
        IBusinessUnitPreferredLanguagesValidator UpdatePreferredLanguagesValidator { get; }

        /// <summary>
        /// Validate update of business unit Notification configuration
        /// </summary>
        IValidateModel<Models.NotificationModels.UpdateNotificationModel, object> UpdateBusinessUnitNotificationValidator { get; }

        /// <summary>
        /// Validates create business unit notification model
        /// </summary>
        IValidateModel<CreateNotificationModel, CreateNotificationModelResponse> CreateBusinessUnitNotificationValidator { get; }
    }
}
