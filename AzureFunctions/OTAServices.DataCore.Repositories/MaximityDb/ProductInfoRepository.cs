using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.MaximityDb
{
    public class ProductInfoRepository : IProductInfoRepository
    {
        #region  [ Private fields ]
        private readonly MaximityDbContext _dbContext;
        private readonly string SP_GET_PRODUCT_INFO_FOR_OTA_CAMPAIGN = "dbo.GetProductInfoForOTACampaign @UiccidList";
        #endregion

        #region  [ Constructor ]
        public ProductInfoRepository(MaximityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        public List<ProductInfo> GetProductInfos(List<string> uiccids)
        {

            DataTable uiccidsDataTable = new DataTable();

            var stringListDataTable = new DataTable();
            stringListDataTable.Columns.Add(new DataColumn("Uiccid", typeof(string)));

            foreach (var uiccid in uiccids)
            {
                stringListDataTable.Rows.Add(new object[] { uiccid });
            }

            SqlParameter uiccidsParam = new SqlParameter
            {
                ParameterName = "UiccidList",
                Value = stringListDataTable,
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.UiccidList"
            };

            return _dbContext.ProductInfo.FromSql(SP_GET_PRODUCT_INFO_FOR_OTA_CAMPAIGN, uiccidsParam).ToList();
        }
    }
}
