using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.Constants;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Business unit notification configuration provider interface
    /// </summary>
    public interface INotificationConfigurationProvider
    {
        /// <summary>
        /// Get Business delivery configurations
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<GetNotificationListDataModel>> GetBusinessUnitNotificationsAsync(Guid businessUnitId);

        /// <summary>
        /// Delete business unit Notification configuration
        /// </summary>
        /// <param name="businessUnitId">Business unit Id</param>
        /// <param name="notificationType">Notification type</param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> DeleteBusinessUnitNotificationAsync(Guid businessUnitId, NotificationType notificationType);

        /// <summary>
        /// Update Business Unit notification configuration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> UpdateBusinessUnitNotificationAsync(UpdateNotificationModel model);

        /// <summary>
        /// Create business unit level notification configuration
        /// </summary>
        /// <param name="businessUnitId">id of business unit for which notification is being created</param>
        /// <param name="input">model holding details about notification being created</param>
        /// <returns>success or failure indication</returns>
        Task<ProviderOperationResult<CreateNotificationModelResponse>> CreateNotificationAsync(Guid businessUnitId, CreateNotificationModel input);
    }
}
