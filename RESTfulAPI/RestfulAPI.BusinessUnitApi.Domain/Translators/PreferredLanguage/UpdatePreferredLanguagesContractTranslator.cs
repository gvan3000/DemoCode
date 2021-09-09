using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.PreferredLanguage
{
    public class UpdatePreferredLanguagesContractTranslator : ITranslate<UpdatePreferredLanguagesRequestModel, UpdateAccountLanguagesContract>
    {
        public UpdateAccountLanguagesContract Translate(UpdatePreferredLanguagesRequestModel input)
        {
            if (input == null)
            {
                return null;
            }

            var accountLanguagesList = new List<AccountLanguageContract>();

            foreach (var accountLanguage in input.PreferredLanguages)
            {
                accountLanguagesList.Add(new AccountLanguageContract
                {
                    LanguageId = accountLanguage.LanguageId,
                    IsDefault = accountLanguage.IsDefault
                });
            }

            var requestContract = new UpdateAccountLanguagesContract
            {
                AccountLanguages = accountLanguagesList.ToArray()
            };

            return requestContract;
        }
    }
}
