using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class SimContentRepository : ISimContentRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public SimContentRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion
        public List<SimContent> GetSimContentBatch(List<string> uiccids, int campaignId)
        {
            DataTable uiccidsDataTable = new DataTable();

            var stringListDataTable = new DataTable();
            stringListDataTable.Columns.Add(new DataColumn("Uiccid", typeof(string)));

            foreach (var uiccid in uiccids)
            {
                stringListDataTable.Rows.Add(new object[] { uiccid });
            }

            SqlParameter uiccidsParam = new SqlParameter();
            uiccidsParam.ParameterName = "p1";
            uiccidsParam.Value = stringListDataTable;
            uiccidsParam.SqlDbType = SqlDbType.Structured;
            uiccidsParam.TypeName = "dbo.UiccidList";

            SqlParameter campaignIdParam = new SqlParameter();
            campaignIdParam.ParameterName = "p2";
            campaignIdParam.Value = campaignId;
            campaignIdParam.SqlDbType = SqlDbType.Int;

            return _dbContext.SimContent.FromSql("SELECT * FROM GetSimContentBatch(@p1, @p2)", new[] { uiccidsParam, campaignIdParam }).ToList();
        }
    }
}
