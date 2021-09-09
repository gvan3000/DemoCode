using System.Linq;
using RestfulAPI.Common;
using System.Collections.Generic;
using RestfulAPI.TeleenaServiceReferences.ProductTypeService;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.ProductType
{
    /// <summary>
    /// ProductTypeContract Translator
    /// </summary>
    public class ProductTypeContractTranslator : ITranslate<ProductTypeContract[], ProductTypeListResponseModel>
    {
        /// <summary>
        /// Converts ProductTypeContract array to ProductTypeList response model
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ProductTypeListResponseModel Translate(ProductTypeContract[] input)
        {
            ProductTypeListResponseModel output = new ProductTypeListResponseModel
            {
                ProductTypes = new List<ProductTypeModel> { }
            };

            if (input == null || input.Count() < 1)
                return output;

            output.ProductTypes = input.Select(x => new ProductTypeModel { Id = x.Id, Name = x.Name }).ToList();

            return output;
        }
    }
}
