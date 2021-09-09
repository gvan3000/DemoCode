using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AddOnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public class PurchaseAddOnResultTranslator : ITranslate<PurchaseAddOnResultContract, AddAddOnResponseModel>
    {
        public AddAddOnResponseModel Translate(PurchaseAddOnResultContract input)
        {
            var result = new AddAddOnResponseModel();

            if (input == null)
            {
                result.Fail = true;
                result.Message = string.Empty;
                return result;
            }

            result.Fail = !input.Success;
            result.Message = input.Message;

            return result;
        }
    }
}
