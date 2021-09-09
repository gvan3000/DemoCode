using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ProductService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Product
{
    /// <summary>
    /// ProductModel translator
    /// </summary>
    public class ProductModelTranslator : ITranslate<GetProductResponse, ProductModel>
    {
        /// <summary>
        /// Translate ProductResponse to ProductModel
        /// </summary>
        /// <param name="input">GetProductResponse</param>
        /// <returns>ProductModel</returns>
        public ProductModel Translate(GetProductResponse input)
        {
            if (input == null)
                return null;

            Guid? translatedPropositionId = null;

            if (input.PropositionId != null && !Equals(Guid.Empty, input.PropositionId))
            {
                translatedPropositionId = input.PropositionId.GetValueOrDefault();
            }

            var result = new ProductModel
            {
                Id = input.ProductId,
                Status = input.ProductStatus,
                Iccid = input.IccId,
                Msisdns = RemoveNullValues(input.Msisdns),
                Imsis = RemoveNullValues(input.Imsis),
                PropositionId = translatedPropositionId,
                BusinessUnitId = input.BusinessUnitId,
                PersonId = input.PersonId,
                CreationDate = input.CreationDate.GetValueOrDefault(),
                ActivationDate = input.ActivationDate,
                DeactivationDate = input.DeActivationDate,
                BillingCycleStartDay = input.BillCycleOffset,
                ProductTypeId = input.ProductTypeId,
                PlanId = input.PlanId,
                LastStatusChangeDate = input.LastProductStatusChangeDate,
                DepartmentAndCostCenterId = input.DepartmentCostCenterId
            };
            return result;
        }

        private string[] RemoveNullValues(string[] input)
        {
            List<string> nonNullList = new List<string>();

            if (input == null || input.Length == 0)
            {
                return nonNullList.ToArray();
            }

            nonNullList = input.ToList();
            nonNullList.RemoveAll(x => x == null);

            return nonNullList.ToArray();
        }
    }
}
