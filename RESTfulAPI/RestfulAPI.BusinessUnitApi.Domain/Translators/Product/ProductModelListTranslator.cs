using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ProductService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Product
{
    /// <summary>
    /// ProductModelList Transaltor
    /// </summary>
    public class ProductModelListTranslator : ITranslate<GetProductResponse[], List<ProductModel>>
    {
        private readonly ITranslate<GetProductResponse, ProductModel> _singleTranslator;

        /// <summary>
        /// Initialize ProductModelListTranslator
        /// </summary>
        /// <param name="singleModelTranslator"></param>
        public ProductModelListTranslator(ITranslate<GetProductResponse, ProductModel> singleModelTranslator)
        {
            _singleTranslator = singleModelTranslator;
        }

        /// <summary>
        /// Translates ProductResponse array to ProductModel list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<ProductModel> Translate(GetProductResponse[] input)
        {
            var result = input.Select(_singleTranslator.Translate).ToList();
            return result;
        }
    }
}
