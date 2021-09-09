using RestfulAPI.Common;
using System;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages
{
    public class BusinessUnitPreferredLanguagesValidator : IBusinessUnitPreferredLanguagesValidator
    {
        public ProviderOperationResult<object> ValidateModel(UpdatePreferredLanguagesRequestModel input)
        {
            if (input == null)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input),
                    $"The preferred languages list is null.");
            }

            if (input.PreferredLanguages == null || input.PreferredLanguages.Count == 0)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input.PreferredLanguages),
                    $"The preferred languages list is empty.");
            }

            foreach (var preferredLanguage in input.PreferredLanguages)
            {
                var result = ValidateUpdatePreferredLanguageModel(preferredLanguage);

                if (!result.IsSuccess)
                    return result;
            }

            return ProviderOperationResult<object>.OkResult();
        }

        private ProviderOperationResult<object> ValidateUpdatePreferredLanguageModel(UpdatePreferredLanguageModel input)
        {
            if (input == null)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input.LanguageId),
                    $"The preferred language is null.");
            }

            if (input.LanguageId == Guid.Empty)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input.LanguageId),
                    $"The preferred language Id is GUID empty.");
            }

            return ProviderOperationResult<object>.OkResult();
        }
    }
}
