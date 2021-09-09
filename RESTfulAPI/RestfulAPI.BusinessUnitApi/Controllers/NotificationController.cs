using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.WebApi.Core;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Manage and find business unit notifications
    /// </summary>
    [RoutePrefix("business-units/{id}/notifications")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationConfigurationProvider _notificationConfigurationProvider;

        /// <summary>
        /// Initialize Notification controller
        /// </summary>
        /// <param name="notificationConfigurationProvider">Notification configuration provider</param>
        public NotificationController(INotificationConfigurationProvider notificationConfigurationProvider)
        {
            _notificationConfigurationProvider = notificationConfigurationProvider;
        }

        /// <summary>
        /// Get business unit notification configuration
        /// </summary>
        /// <param name="id">Id of the business unit</param>
        /// <returns></returns>
        [HttpGet]
        [Route]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [ResponseType(typeof(GetNotificationListDataModel))]
        [Description("6.28 Get Notifications")]
        public async Task<IHttpActionResult> GetBusinesUnitConfigurations(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _notificationConfigurationProvider.GetBusinessUnitNotificationsAsync(id);

            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Update specific Business Unit Notification
        /// </summary>
        /// <param name="businessUnitId">Business Unit Id</param>
        /// <param name="type">Notification type</param>
        /// <param name="input">Update notification model</param>
        /// <returns>Returns response did it succeeded or failed</returns>
        [HttpPatch]
        [Route("{notificationId}")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.30 Updates a business unit notification. Overwrites all the current deliveries.")]
        public async Task<IHttpActionResult> UpdateNotification([FromUri(Name = "id")] Guid businessUnitId, [FromUri(Name = "notificationId")] string type, [FromBody] UpdateNotificationModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.TryParse(type, out NotificationType typeEnum) || typeEnum == 0)
            {
                ModelState.AddModelError("notificationId", $"Invalid value {type}, does not exist");
                return BadRequest(ModelState);
            }

			var availableNotifications = await _notificationConfigurationProvider.GetBusinessUnitNotificationsAsync(businessUnitId);
			if (!availableNotifications.IsSuccess || !availableNotifications.Result.Notifications.Exists(x=>x.Type==typeEnum))
			{
				ModelState.AddModelError("notificationId", $"Business Unit does not contain requested notification: {type}");
				return availableNotifications.IsSuccess ? BadRequest(ModelState) : ActionResultFromProviderOperation(availableNotifications);
			}

			input.Type = typeEnum;
            input.BusinessUnitId = businessUnitId;

            Common.ProviderOperationResult<object> response = await _notificationConfigurationProvider.UpdateBusinessUnitNotificationAsync(input);
            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Delete business unit notification configuration
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <param name="notificationType">Type of the notification configuration process</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{notificationId}")]
        [Description("6.31 Delete Notifications")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> DeleteBusinessUnitConfigurations([FromUri(Name = "id")]Guid businessUnitId, [FromUri(Name = "notificationId")]NotificationType notificationType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _notificationConfigurationProvider.DeleteBusinessUnitNotificationAsync(businessUnitId, notificationType);

            return ActionResultFromProviderOperation(response);
        }

        [HttpPost]
        [Route]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.27 Create Notifications")]
        [ResponseType(typeof(CreateNotificationModelResponse))]
        public async Task<IHttpActionResult> CreateNotification([FromUri(Name = "id")] Guid businessUnitId, [FromBody] CreateNotificationModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _notificationConfigurationProvider.CreateNotificationAsync(businessUnitId, input);
            return ActionResultFromProviderOperation(providerResult);
        }
    }
}