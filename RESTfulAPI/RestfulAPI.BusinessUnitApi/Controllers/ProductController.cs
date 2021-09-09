using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RestfulAPI.Constants;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Product controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class ProductController : BaseApiController
    {
        private readonly IProductProvider _productProvider;

        /// <summary>
        /// Create new instance of Product controller
        /// </summary>
        /// <param name="productProvider"></param>
        public ProductController(IProductProvider productProvider)
        {
            _productProvider = productProvider;
        }

        /// <summary>
        /// Get a list of all products for the business unit
        /// </summary>
        /// <param name="id">Id of a requested business-unit</param>
        /// <param name="includeChildren">parameter used to communicate if we want only products for specified BU or for it and all of its sub units</param>
        /// <param name="creationDateFrom">Used to return products that are created after specified date and time.</param>
        /// <param name="pageSize">indicates desired page size or size of the resulting list</param>
        /// <param name="pageNumber">indicates page number for which results are returned, page numbers are 1 based</param>
        /// <returns>List of<see cref="ProductModel"/> filled with list of all products of the business unit</returns>
        [HttpGet]
        [Route("{id}/products")]
        [ResponseType(typeof(ProductsListModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.4 Get Products")]
        public async Task<IHttpActionResult> Get(Guid id,
            [FromUri(Name = "creation_date_from")] DateTime? creationDateFrom = null,
            [FromUri(Name = "children")] bool? includeChildren = null,
            [FromUri(Name = "per_page")] int? pageSize = null,
            [FromUri(Name = "page")] int pageNumber = 1)
        {
            if (creationDateFrom != null && creationDateFrom < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue)
            {
                ModelState.AddModelError("creation_date_from", $"the value {creationDateFrom} is not valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _productProvider.GetProductsForBusinessUnitAsync(id, UserCompanyId, creationDateFrom, includeChildren, pageNumber, pageSize);

            return ActionResultFromProviderOperation(providerResult);
        }

        /// <summary>
        /// Gets a list of ProductImei records for all products in a business unit. Can be include or exclude child Business Units, and can be filtered by IMEI or ICCID.
        /// </summary>
        /// <param name="id">ID of the Business Unit</param>
        /// <param name="imei">IMEI to filter results by</param>
        /// <param name="iccid">ICCID to filter results by</param>
        /// <param name="includeChildren">Indicates whether child Business Units should be included (default: <see langword="false"/>)</param>
        /// <returns>List of<see cref="ProductImeiModel"/></returns>
        [HttpGet]
        [Route("{id}/imei")]
        [ResponseType(typeof(ProductImeiListModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.31 Get Products IMEI")]
        public async Task<IHttpActionResult> GetProductImei(Guid id, 
            [FromUri(Name = "imei")] string imei = null, 
            [FromUri(Name = "iccid")] string iccid = null, 
            [FromUri(Name = "children")] bool includeChildren = false)
        {
            if (!string.IsNullOrWhiteSpace(imei) && imei.Length < ImeiConstants.MinLength)
            {
                ModelState.AddModelError("imei", $"imei must have at least {ImeiConstants.MinLength} digits");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var request = new GetProductImeiByBusinessUnitDataContract
            {
                BusinessUnitId = id,
                Iccid = iccid,
                Imei = imei?.Substring(0, ImeiConstants.MinLength),
                IncludeChildren = includeChildren
            };

            var result = await _productProvider.GetProductImeisForBusinessUnitAsync(request);

            return ActionResultFromProviderOperation(result);
        }
    }
}