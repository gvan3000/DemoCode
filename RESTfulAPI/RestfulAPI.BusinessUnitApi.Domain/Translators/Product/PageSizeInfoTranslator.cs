using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Product
{
    /// <summary>
    /// Page Size Info translator
    /// </summary>
    public class PageSizeInfoTranslator : ITranslate<int?, PageSizeInfo>
    {
        /// <summary>
        /// Translates page size value to PageSizeInfo model
        /// </summary>
        /// <param name="input">page size value</param>
        /// <returns><see cref="PageSizeInfo"/> </returns>
        public PageSizeInfo Translate(int? input)
        {
            var pageSizeInfo = new PageSizeInfo();

            if (input.GetValueOrDefault(0) <= 0)
            {
                pageSizeInfo.IsPaged = false;
                pageSizeInfo.PageSize = int.MaxValue;
            }
            else
            {
                pageSizeInfo.IsPaged = true;
                pageSizeInfo.PageSize = Math.Min(input.GetValueOrDefault(1000), 1000);
            };

            return pageSizeInfo;
        }
    }
}
