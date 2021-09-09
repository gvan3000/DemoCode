namespace RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels
{
    /// <summary>
    /// Page Size info model
    /// </summary>
    public class PageSizeInfo
    {
        /// <summary>
        /// Size of the page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// If searching is paged
        /// </summary>
        public bool IsPaged { get; set; }
    }
}
