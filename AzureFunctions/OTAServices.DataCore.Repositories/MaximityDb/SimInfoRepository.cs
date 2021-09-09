using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.MaximityDb
{
    public class SimInfoRepository : ISimInfoRepository
    {
        #region [ Private fields ]

        private readonly MaximityDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public SimInfoRepository(MaximityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SimInfo> GetSimInfoBatch(List<string> uiccids)
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

            return _dbContext.SimInfo.FromSql("SELECT * FROM GetSimInfoBatch(@p1)", new[] { uiccidsParam }).ToList();
        }

        #endregion
    }
}
