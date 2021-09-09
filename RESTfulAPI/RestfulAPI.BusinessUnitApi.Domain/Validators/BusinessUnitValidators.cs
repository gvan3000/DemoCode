using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Validators.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications;
using RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators
{
    public class BusinessUnitValidators : IBusinessUnitValidators
    {
        private IUpdateBusinessUnitApnsValidator _updateBusinessUnitApnsValidator;

        public IUpdateBusinessUnitApnsValidator UpdateBusinessUnitApnsValidator
        {
            get
            {
                if (_updateBusinessUnitApnsValidator == null)
                {
                    _updateBusinessUnitApnsValidator = new UpdateBusinessUnitApnsValidator();
                }

                return _updateBusinessUnitApnsValidator;
            }
        }

        private IDeleteApnValidator _deleteApnValidator;
        public IDeleteApnValidator DeleteApnValidator
        {
            get
            {
                if (_deleteApnValidator == null)
                {
                    _deleteApnValidator = new DeleteApnValidator();
                }

                return _deleteApnValidator;
            }
        }

        private IUpdateBusinessUnitDefaultApnValidator _updateBusinessUnitDefaultApnValidator;
        public IUpdateBusinessUnitDefaultApnValidator UpdateBusinessUnitDefaultApnValidator
        {
            get
            {
                if (_updateBusinessUnitDefaultApnValidator == null)
                {
                    _updateBusinessUnitDefaultApnValidator = new UpdateBusinessUnitDefaultApnValidator();
                }

                return _updateBusinessUnitDefaultApnValidator;
            }
        }

        private IBusinessUnitPreferredLanguagesValidator _updatePreferredLanguagesValidator;
        public IBusinessUnitPreferredLanguagesValidator UpdatePreferredLanguagesValidator
        {
            get
            {
                if (_updatePreferredLanguagesValidator == null)
                {
                    _updatePreferredLanguagesValidator = new BusinessUnitPreferredLanguagesValidator();
                }

                return _updatePreferredLanguagesValidator;
            }
        }

        private IValidateModel<UpdateNotificationModel, object> _updateProductNotificationValidator;
        public IValidateModel<UpdateNotificationModel, object> UpdateBusinessUnitNotificationValidator
        {
            get
            {
                if (_updateProductNotificationValidator == null)
                    _updateProductNotificationValidator = new UpdateNotificationValidation();

                return _updateProductNotificationValidator;
            }
        }

        private IValidateModel<CreateNotificationModel, CreateNotificationModelResponse> _createBusinessUnitValidator;
        public IValidateModel<CreateNotificationModel, CreateNotificationModelResponse> CreateBusinessUnitNotificationValidator
        {
            get
            {
                if (_createBusinessUnitValidator == null)
                    _createBusinessUnitValidator = new CreateNotificationValidator();

                return _createBusinessUnitValidator;
            }
        }

    }
}
