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
    public class PreferredLanguageResponseModelTranslator : ITranslate<GetAccountLanguageContract[], PreferredLanguageResponseModel>
    {
        public PreferredLanguageResponseModel Translate(GetAccountLanguageContract[] input)
        {
            if (input == null)
            {
                return null;
            }

            var responseModel = new PreferredLanguageResponseModel() { PreferredLanguages = new List<PreferredLanguageModel>() };
            foreach (var accountLanguage in input)
            {
                responseModel.PreferredLanguages.Add(new PreferredLanguageModel() { Name = accountLanguage.Name, LanguageId = accountLanguage.LanguageId, IsDefault = accountLanguage.IsDefault});
            }

            return responseModel;
        }
    }
}
