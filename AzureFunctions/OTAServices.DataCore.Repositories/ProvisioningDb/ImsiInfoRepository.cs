using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class ImsiInfoRepository : IImsiInfoRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;

        private readonly string SP_GET_IMSI_INFOS_FOR_OTA_CAMPAIGN = "dbo.GetImsiInfosForOTACampaign @UiccidList,@CampaignId";
        #endregion

        #region [ Constructor ]

        public ImsiInfoRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion
        
        public List<ImsiInfo> GetImsiInfos(List<string> uiccids, int campaignId)
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

            SqlParameter campaignIdParm = new SqlParameter
            {
                ParameterName = "CampaignId",
                Value = campaignId,
                SqlDbType = SqlDbType.Int
            };
            
            return _dbContext.ImsiInfo.FromSql(SP_GET_IMSI_INFOS_FOR_OTA_CAMPAIGN, new[] { uiccidsParam, campaignIdParm }).ToList();
        }
    }
}
