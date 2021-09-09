using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.Common;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class ProvisioningDataInfoRepository : IProvisioningDataInfoRepository
    {
        #region  [ Private fields ]
        private readonly ProvisioningDbContext _dbContext;
        #endregion

        #region  [ Constructor ]
        public ProvisioningDataInfoRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        public List<ProvisioningDataInfo> GetProvisioningDataInfos(List<string> uiccids)
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

            return _dbContext.ProvisioningDataInfo.FromSql("SELECT * FROM GetProvisioiningDataInfoBatch(@UiccidList)", new[] { uiccidsParam }).ToList();
        }
    }
}
