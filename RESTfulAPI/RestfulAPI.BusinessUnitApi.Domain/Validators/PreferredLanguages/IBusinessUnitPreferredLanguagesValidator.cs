using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages
{
    public interface IBusinessUnitPreferredLanguagesValidator
    {
        ProviderOperationResult<object> ValidateModel(UpdatePreferredLanguagesRequestModel input);
    }
}
