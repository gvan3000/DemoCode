using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.PreferredLanguage
{
    public class AvailableLanguagesResponseModelTranslator : ITranslate<GetCompanyLanguageContract[], AvailableLanguagesResponseModel>
    {
        public AvailableLanguagesResponseModel Translate(GetCompanyLanguageContract[] input)
        {
            if (input == null)
            {
                return null;
            }

            var responseModel = new AvailableLanguagesResponseModel() { AvailableLanguages = new List<AvailableLanguageModel>() };
            foreach (var availableLanguage in input)
            {
                responseModel.AvailableLanguages.Add(new AvailableLanguageModel() { Name = availableLanguage.Name, LanguageId = availableLanguage.LanguageId });
            }

            return responseModel;
        }
    }
}
