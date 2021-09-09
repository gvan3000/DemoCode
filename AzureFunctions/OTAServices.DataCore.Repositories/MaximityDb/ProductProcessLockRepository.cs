using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Interfaces.Repository;

namespace OTAServices.DataCore.MaximityDb
{
    public class ProductProcessLockRepository : IProductProcessLockRepository
    {
        #region  [ Private fields ]
        private readonly MaximityDbContext _dbContext;
        private readonly string SP_ADDPRODUCTPRCESSLOCKBULK = "EXEC dbo.AddProductProcessLockBulk @UiccidList, @SysCode, @CreatedBy";
        private readonly string SP_DELETEPRODUCTPROCESSLOCKBULK = "EXEC dbo.DeleteProductProcessLockBulk @UiccidList, @SysCode";
        #endregion

        #region  [ Constructor ]
        public ProductProcessLockRepository(MaximityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddProductProcessLockBulk(List<string> uiccids)
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

            SqlParameter sysCodeParam = new SqlParameter
            {
                ParameterName = "SysCode",
                Value = "SIM_OTA_CAMPAIGN",
                SqlDbType = SqlDbType.NChar
            };

            SqlParameter createdByParam = new SqlParameter
            {
                ParameterName = "CreatedBy",
                Value = Guid.Empty,
                SqlDbType = SqlDbType.UniqueIdentifier
            };

            _dbContext.Database.ExecuteSqlCommand(SP_ADDPRODUCTPRCESSLOCKBULK, uiccidsParam, sysCodeParam, createdByParam);
        }

        public void DeleteProductProcessLockBulk(List<string> uiccids)
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

            SqlParameter sysCodeParam = new SqlParameter
            {
                ParameterName = "SysCode",
                Value = "SIM_OTA_CAMPAIGN",
                SqlDbType = SqlDbType.NChar
            };

            _dbContext.Database.ExecuteSqlCommand(SP_DELETEPRODUCTPROCESSLOCKBULK, uiccidsParam, sysCodeParam);
        }

        #endregion
    }
}
