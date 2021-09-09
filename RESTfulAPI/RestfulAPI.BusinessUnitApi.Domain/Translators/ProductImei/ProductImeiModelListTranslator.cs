using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.ProductImei
{
    public class ProductImeiModelListTranslator : ITranslate<ProductImeiByBusinessUnitDataContract[], ProductImeiListModel>
    {
        public ProductImeiListModel Translate(ProductImeiByBusinessUnitDataContract[] input)
        {
            if (input == null)
            {
                return null;
            }

            return new ProductImeiListModel
            {
                Products = input.Select(p => new ProductImeiModel
                {
                    Id = p.ProductId,
                    Imei = p.Imei,
                    Iccid = p.Iccid,
                    CreationDate = p.CreationDate,
                    ImeiSV = p.ImeiSV,
                    ImeiSVTimeStamp = p.ImeiSVTimeStamp
                }).ToList()
            };
        }
    }
}