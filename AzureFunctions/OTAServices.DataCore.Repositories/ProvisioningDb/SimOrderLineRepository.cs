using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class SimOrderLineRepository : ISimOrderLineRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;
        private readonly string SP_SET_SIM_PROFILE_FOR_UICCID_BATCH = "SetSimProfileForUiccidBatch @UiccidSimProfileList";
        private readonly string SP_GET_SIM_PROFILE_BY_UICCID_BATCH = "GetSimProfileIdByUiccidBatch @UiccidList";
        #endregion

        #region [ Constructor ]

        public SimOrderLineRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UiccidSimProfileId> GetSimProfileByUiccidBatch(List<string> uiccids)
        {
            DataTable uiccidList = new DataTable();

            uiccidList.Columns.Add(new DataColumn("Uiccid", typeof(string)));

            foreach (var uiccid in uiccids)
            {
                uiccidList.Rows.Add(new object[] { uiccid });
            }

            SqlParameter uiccidListParameter = new SqlParameter
            {
                ParameterName = "UiccidList",
                Value = uiccidList,
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.UiccidList"
            };

            return _dbContext.UiccidSimProfileId.FromSql(SP_GET_SIM_PROFILE_BY_UICCID_BATCH, new[] { uiccidListParameter }).ToList();
        }
        #endregion

        public void SetSimOrderLineSimProfileIdBatch(List<UiccidSimProfileId> data)
        {
            DataTable uiccidSimProfileList = new DataTable();
            uiccidSimProfileList.Columns.Add(new DataColumn("Uiccid", typeof(string)));
            uiccidSimProfileList.Columns.Add(new DataColumn("SimProfileId", typeof(int)));

            foreach (var item in data)
            {
                uiccidSimProfileList.Rows.Add(new object[] { item.Uiccid, item.SimProfileId });
            }

            SqlParameter uiccidsSimProfileListParam = new SqlParameter
            {
                ParameterName = "UiccidSimProfileList",
                Value = uiccidSimProfileList,
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.UiccidSimProfileList"
            };

            _dbContext.Database.ExecuteSqlCommand(SP_SET_SIM_PROFILE_FOR_UICCID_BATCH, new[] { uiccidsSimProfileListParam });
        }
    }
}
