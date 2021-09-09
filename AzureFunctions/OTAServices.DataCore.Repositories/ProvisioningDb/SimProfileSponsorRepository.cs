using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class SimProfileSponsorRepository : ISimProfileSponsorRepository
    {
        #region [ Private fields ]

        private readonly ProvisioningDbContext _dbContext;
        private const string SP_GET_SIM_PROFILE_SPONSOR_MCC_BATCH = "SELECT * FROM GetSimProfileSponsorMccBatch(@p1)";

        #endregion

        #region [ Constructor ]

        public SimProfileSponsorRepository(ProvisioningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SimProfileSponsor> GetSimProfileSponsorList(List<int> simProfilesIds)
        {
            DataTable simProfileIdsDataTable = new DataTable();

            var intListDataTable = new DataTable();
            intListDataTable.Columns.Add(new DataColumn("SimProfileId", typeof(int)));

            foreach (var simProfileId in simProfilesIds)
            {
                intListDataTable.Rows.Add(new object[] { simProfileId });
            }

            var simProfileIdsParam = new SqlParameter();
            simProfileIdsParam.ParameterName = "p1";
            simProfileIdsParam.Value = intListDataTable;
            simProfileIdsParam.SqlDbType = SqlDbType.Structured;
            simProfileIdsParam.TypeName = "dbo.SimProfilesIdsList";

            var simProfileSponsorMccList = _dbContext.SimProfileSponsorMcc.FromSql(SP_GET_SIM_PROFILE_SPONSOR_MCC_BATCH, new[] { simProfileIdsParam }).ToList();

            var simProfileSponsorList = new List<SimProfileSponsor>();

            foreach (var simProfileSponsorMcc in simProfileSponsorMccList)
            {
                if (string.IsNullOrEmpty(simProfileSponsorMcc.MCC))
                {
                    throw new InvalidOperationException($"There is SimProfile-ImsiSponsor combination (for ImsiSponsor prefix {simProfileSponsorMcc.SponsorPrefix}) without any PrefferedNetworkList entry. SimProfile with id={simProfileSponsorMcc.SimProfileId} could be original profile of some ICCID from target list or target profile.");
                }

                //Look for existing SimProfileId-Sponsor combination
                var simProfileSponsor = simProfileSponsorList.FirstOrDefault(x => x.SimProfileId == simProfileSponsorMcc.SimProfileId && x.SponsorPrefix == simProfileSponsorMcc.SponsorPrefix);

                if (simProfileSponsor == null)
                {
                    simProfileSponsorList.Add(new SimProfileSponsor() { SimProfileId = simProfileSponsorMcc.SimProfileId, SponsorExternalId = simProfileSponsorMcc.SponsorExternalId, SponsorPrefix = simProfileSponsorMcc.SponsorPrefix, MCC = simProfileSponsorMcc.MCC });
                }
                else
                {
                    //Concatenate MCC, so if we have 123, 456 and 789 it will be 123456789 prepared for ImsieInfo and OASIS CSV
                    simProfileSponsor.MCC = String.Concat(simProfileSponsor.MCC, simProfileSponsorMcc.MCC);
                }
            }

            return simProfileSponsorList;
        }  
        #endregion
    }
}
